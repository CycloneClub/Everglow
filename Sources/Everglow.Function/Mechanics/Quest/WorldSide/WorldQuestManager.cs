using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide;

public class WorldQuestManager
{
	// Add a config option for update interval if necessary.
#if DEBUG
	public const int UpdateInterval = 1;
	public const int NetUpdateInterval = 1;
#else
	public const int UpdateInterval = 30;
	public const int NetUpdateInterval = 60;
#endif

	public static WorldQuestManager Instance => ModContent.GetInstance<WorldQuestSystem>().Manager;

	public static bool NetUpdate => Instance.UpdateTimer % NetUpdateInterval == 0;

	public static bool NormalUpdate => Instance.UpdateTimer % UpdateInterval == 0;

	public event Action<QuestIdentity> QuestStatusUpdated;
	public event Action<QuestIdentity> QuestObjectiveUpdated;
	public static event Action<QuestNotification> NotificationRequested;

	private IGameStateProvider _gameState;

	private List<WorldQuestBase> _quests = [];

	public IReadOnlyList<WorldQuestBase> Quests => _quests;

	public IReadOnlyList<WorldQuestBase> ActiveQuests => _quests.Where(m => m.State == WorldQuestState.Active).ToList();

	private int UpdateTimer => (int)_gameState.TimeForVisualEffects;

	public WorldQuestManager()
	{
		_gameState = GameStateProvider.Default;
	}

	/// <summary>
	/// For test only
	/// </summary>
	/// <param name="gameStateProvider"></param>
	public WorldQuestManager(IGameStateProvider gameStateProvider)
	{
		_gameState = gameStateProvider;
	}

	public void Load()
	{
		// Initialize quest manager: load quests, set up main hooks, etc.
		_quests = [];

		var source = ModLoader.Mods
			.Select(m => (m.Name, AssemblyManager.GetLoadableTypes(m.Code).AsEnumerable()))
			.Concat([(nameof(Everglow), Ins.ModuleManager.Types)])
			.Distinct();
		foreach (var (modName, modTypes) in source)
		{
			foreach (var mT in modTypes.Where(t => t.IsSubclassOf(typeof(WorldQuestBase)) && !t.IsAbstract))
			{
				var quest = Activator.CreateInstance(mT) as WorldQuestBase;
				quest.WhoAmI = _quests.Count;
				_quests.Add(quest);
			}
		}
		Main.OnTickForInternalCodeOnly += Update;
	}

	public void Unload()
	{
		// Clean up quest manager: clear quest data, remove hooks, etc.
		Main.OnTickForInternalCodeOnly -= Update;
		Reset();
		_quests = null;
		QuestStatusUpdated = null;
		QuestObjectiveUpdated = null;
		NotificationRequested = null;
	}

	public void Initialize()
	{
		foreach (var m in _quests)
		{
			m.Initialize();
		}
	}

	public void Reset()
	{
		_quests.ForEach(m =>
		{
			m.Deactivate();
			m.Reset();
		});
	}

	public void Update()
	{
		// Main.gamePaused always be false here when triggered by Main.OnTickForInternalCodeOnly hook.
		// Main.gameInactive always be true on the server
		if (_gameState.GameMenu)
		{
			return;
		}

		if (NormalUpdate)
		{
			// Check locked
			foreach (var m in _quests.Where(m => m.State == WorldQuestState.Locked))
			{
				if (m.CanUnlock()
					&& m.State == WorldQuestState.Locked)
				{
					m.Unlock();
					OnQuestStatusUpdated(m);
				}
			}

			// Check active
			foreach (var m in _quests.Where(m => m.State == WorldQuestState.Active))
			{
				WorldQuestState oldState = m.State;
				m.Update();
				OnQuestObjectiveUpdated(m);
				if (m.State != oldState)
				{
					OnQuestStatusUpdated(m);
				}
			}
		}

		if (NetUpdate && !NetUtils.IsSingle)
		{
			foreach (var m in _quests.Where(m => m.State == WorldQuestState.Active))
			{
				m.OnMPSync();
			}
		}
	}

	public WorldQuestBase GetQuest(int whoAmI) =>
		_quests.FirstOrDefault(m => m.WhoAmI == whoAmI);

	public WorldQuestBase GetQuest(string name) =>
		_quests.FirstOrDefault(m => m.Name == name);

	public WorldQuestBase GetQuest<T>()
		where T : WorldQuestBase =>
		_quests.OfType<T>().FirstOrDefault();

	public IEnumerable<WorldQuestBase> GetQuests(WorldQuestState state) =>
		_quests.Where(m => m.State == state);

	public void AddQuest(WorldQuestBase quest)
	{
		if (_quests.Any(m => m.Name == quest.Name))
		{
			throw new InvalidOperationException($"Quest with name {quest.Name} already exists.");
		}
		_quests.Add(quest);
	}

	internal bool TryRetryObjective(string questName, int objectiveId)
	{
		if (!NetUtils.IsSingle && !NetUtils.IsMainServer)
		{
			return false;
		}

		WorldQuestBase quest = GetQuest(questName);
		if (quest is null || !quest.TryRetryObjectiveCore(objectiveId))
		{
			return false;
		}

		OnQuestObjectiveUpdated(quest);
		if (NetUtils.IsMainServer)
		{
			ModIns.PacketResolver.Route(new QuestSyncPacket(quest), RouteDestination.AllDownstream);
		}

		return true;
	}

	internal bool TryClaimReward(string questName, int whoAmI)
	{
		if (!NetUtils.IsMainServer
			|| !NetUtils.TryGetConnectedPlayerName(whoAmI, out string playerName))
		{
			return false;
		}

		WorldQuestBase quest = GetQuest(questName);
		if (quest is null || QuestHintRules.HasContent(quest.Hint))
		{
			return false;
		}

		bool playerIsInMainWorld = PlayerUtils.TryGetActivePlayer(whoAmI, out Player player);
		if (playerIsInMainWorld
			&& !string.Equals(player.name, playerName, StringComparison.Ordinal))
		{
			return false;
		}

		if (!quest.TryRecordRewardClaim(playerName))
		{
			return false;
		}

		if (playerIsInMainWorld)
		{
			quest.GiveRewards(player);
		}
		else
		{
			ModIns.PacketResolver.Route(
				new QuestGiveRewardPacket(quest.Name, whoAmI, playerName),
				RouteDestination.AllDownstream);
		}

		OnQuestStatusUpdated(quest);
		ModIns.PacketResolver.Route(new QuestSyncPacket(quest), RouteDestination.AllDownstream);
		return true;
	}

	internal bool TryGiveRewards(
		string questName,
		int whoAmI,
		string expectedPlayerName,
		int sourceWhoAmI)
	{
		if (!NetUtils.IsSubServer
			|| sourceWhoAmI != -1
			|| !PlayerUtils.TryGetActivePlayer(whoAmI, out Player player)
			|| !string.Equals(player.name, expectedPlayerName, StringComparison.Ordinal))
		{
			return false;
		}

		WorldQuestBase quest = GetQuest(questName);
		if (quest is null
			|| QuestHintRules.HasContent(quest.Hint)
			|| !quest.TryRecordRewardClaim(expectedPlayerName))
		{
			return false;
		}

		quest.GiveRewards(player);
		OnQuestStatusUpdated(quest);
		return true;
	}

	public bool ResetQuest(WorldQuestBase quest)
	{
		throw new NotImplementedException();
	}

	#region Persistence & Netcode

	public void NetSend(BinaryWriter writer)
	{
		foreach (var m in _quests)
		{
			m.NetSend(writer);
		}
		Console.WriteLine("Full sync msg sent!");
	}

	public void NetReceive(BinaryReader reader)
	{
		foreach (var m in _quests)
		{
			m.NetReceive(reader);
			OnQuestStatusUpdated(m);
			OnQuestObjectiveUpdated(m);
		}
		Console.WriteLine("Full sync msg received!");
	}

	public void SaveData(TagCompound tag)
	{
		foreach (var m in _quests)
		{
			var mTag = new TagCompound();
			m.SaveData(mTag);
			tag.Add(m.Name, mTag);
		}
	}

	public void LoadData(TagCompound tag)
	{
		Reset();

		foreach (var m in _quests)
		{
			if (tag.TryGet<TagCompound>(m.Name, out var mTag))
			{
				m.LoadData(mTag);
			}
			else
			{
				// Handle missing quest data if necessary.
			}

			OnQuestStatusUpdated(m);
			OnQuestObjectiveUpdated(m);
		}
	}

	#endregion

	public void OnQuestStatusUpdated(WorldQuestBase quest) => QuestStatusUpdated?.Invoke(GetIdentity(quest));

	public void OnQuestObjectiveUpdated(WorldQuestBase quest) => QuestObjectiveUpdated?.Invoke(GetIdentity(quest));

	internal static void Notify(WorldQuestBase quest, QuestNotificationType type, string detail = null) =>
		NotificationRequested?.Invoke(new QuestNotification(GetIdentity(quest), type, detail));

	private static QuestIdentity GetIdentity(WorldQuestBase quest) => new(QuestSide.World, quest.Name, quest.Name);
}
