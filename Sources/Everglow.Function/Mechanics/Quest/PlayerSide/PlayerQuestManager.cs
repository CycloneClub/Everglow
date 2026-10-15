using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using MathNet.Numerics;

using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide;

public class PlayerQuestManager
{
	public const int UpdateInterval = 20;
	public static PlayerQuestManager Instance => ModContent.GetInstance<PlayerQuestSystem>().Manager;

	public event Action<QuestIdentity> QuestAdded;
	public event Action<QuestIdentity> QuestRemoved;
	public event Action<QuestIdentity> QuestStatusUpdated;
	public event Action<QuestIdentity> QuestObjectiveUpdated;

	private List<PlayerQuestBase> _quests = [];

	public IReadOnlyList<PlayerQuestBase> Quests => _quests;

	/// <summary>
	/// 历史杀怪计数
	/// </summary>
	private Dictionary<int, int> _nPCKillCounter = [];
	private bool _loaded;

	/// <summary>
	/// 已接受的任务
	/// </summary>
	private IEnumerable<PlayerQuestBase> AcceptedQuests => _quests.Where(m => m.State == PlayerQuestState.Accepted);

	/// <summary>
	/// 历史杀怪计数
	/// </summary>
	public IReadOnlyDictionary<int, int> NPCKillCounter => _nPCKillCounter;

	#region TML Integration

	public void Load()
	{
		if (!Main.dedServ && !_loaded)
		{
			Main.OnTickForInternalCodeOnly += Update;
			Ins.HookManager.AddHook(Commons.Enums.CodeLayer.PostSaveAndQuit, Clear);
			QuestGlobalNPC.OnKillNPCEvent += QuestGlobalNPC_SpecialOnKill_CountKill;
			_loaded = true;
		}
	}

	public void Unload()
	{
		if (_loaded)
		{
			Main.OnTickForInternalCodeOnly -= Update;
			QuestGlobalNPC.OnKillNPCEvent -= QuestGlobalNPC_SpecialOnKill_CountKill;
			_loaded = false;
		}

		Clear();
		QuestAdded = null;
		QuestRemoved = null;
		QuestStatusUpdated = null;
		QuestObjectiveUpdated = null;
	}

	/// <summary>
	/// Initialize quest manager with player quest data
	/// </summary>
	/// <param name="data"></param>
	public void ApplyData(PlayerQuestManagerData data)
	{
		if (data == null)
		{
			return;
		}

		List<PlayerQuestBase> oldQuests = _quests;
		_nPCKillCounter = data.NPCKillCounter.ToDictionary();
		_quests = data.Quests.ToList();

		foreach (PlayerQuestBase quest in oldQuests)
		{
			OnQuestRemoved(quest);
		}

		foreach (var m in AcceptedQuests)
		{
			m.Activate();
		}

		foreach (PlayerQuestBase quest in _quests)
		{
			OnQuestAdded(quest);
		}
	}

	/// <summary>
	/// 清除所有任务
	/// </summary>
	public void Clear()
	{
		List<PlayerQuestBase> removedQuests = _quests.ToList();
		foreach (var quest in _quests)
		{
			quest.Deactivate();
		}

		_nPCKillCounter.Clear();
		_quests.Clear();

		foreach (PlayerQuestBase quest in removedQuests)
		{
			OnQuestRemoved(quest);
		}
	}

	#endregion

	#region In-Game Update

	/// <summary>
	/// 为任务每帧更新
	/// </summary>
	public void Update()
	{
		// Main.gamePaused always be false here when triggered by Main.OnTickForInternalCodeOnly hook.
		if (Main.gameMenu || Main.gameInactive) // || Main.gamePaused
		{
			return;
		}

		if (Main.timeForVisualEffects % UpdateInterval != 0)
		{
			return;
		}

		// 更新所有任务
		foreach (var m in AcceptedQuests)
		{
			m.Update();
			OnQuestObjectiveUpdated(m);
		}

		// 处理自动提交任务
		var autoCommitQuests = AcceptedQuests.Where(m => m.CheckComplete() && m.AutoComplete).ToList();
		if (autoCommitQuests.Count > 0)
		{
			autoCommitQuests.ForEach(m => m.OnComplete());
		}

		// 处理过期任务
		var expiredQuests = AcceptedQuests.Where(m => m.CheckExpire()).ToList();
		if (expiredQuests.Count > 0)
		{
			expiredQuests.ForEach(m => m.OnExpire());
		}

		// 检测可提交状态改变的任务，将状态改变为可提交的任务抛出信息
		foreach (var m in AcceptedQuests.ToList())
		{
			if (m.CheckComplete() != m.OldCheckComplete)
			{
				m.OldCheckComplete = m.CheckComplete();

				m.OnCheckCompleteChange();

				// 由不可提交改变到可提交状态的任务, 发送消息提示
				if (m.CheckComplete())
				{
					Main.NewText($"[{m.Name}]任务可以提交了", 250, 250, 150);
				}
			}
		}
	}

	/// <summary>
	/// 记录杀怪
	/// </summary>
	/// <param name="npc">被击杀的NPC</param>
	/// <exception cref="InvalidParameterException">参数为空或npc类型错误</exception>
	public void QuestGlobalNPC_SpecialOnKill_CountKill(NPC npc)
	{
		if (npc.type <= NPCID.None)
		{
			Ins.Logger.Warn($"Invalid npc type {npc.type}.");
			return;
		}

		// Update NPC kill history
		if (!_nPCKillCounter.TryAdd(npc.type, 1))
		{
			_nPCKillCounter[npc.type]++;
		}
	}

	#endregion

	#region Manager Logic

	/// <summary>
	/// 获取某个任务
	/// </summary>
	/// <param name="questName">任务名字，或者说 ID</param>
	/// <returns></returns>
	public PlayerQuestBase GetQuest(string questName) =>
		_quests.FirstOrDefault(m => m.Name == questName);

	/// <summary>
	/// 添加任务
	/// </summary>
	/// <param name="quest">任务</param>
	/// <param name="state">任务状态</param>
	public void AddQuest(PlayerQuestBase quest, PlayerQuestState state, bool showText = true)
	{
		if (!_quests.Any(m => m.Name == quest.Name))
		{
			_quests.Add(quest);
			quest.State = state;

			if (showText)
			{
				Main.NewText($"新的任务任务已添加[{quest.DisplayName}]", 250, 250, 150);
			}

			if (state == PlayerQuestState.Accepted)
			{
				quest.Activate();
			}

			OnQuestAdded(quest);
		}
	}

	/// <summary>
	/// 移除指定任务名的任务
	/// </summary>
	/// <param name="questName">任务名字，或者说 ID</param>
	/// <returns></returns>
	public bool RemoveQuest(string questName)
	{
		List<PlayerQuestBase> removedQuests = _quests.Where(m => m.Name == questName).ToList();
		foreach (var m in removedQuests)
		{
			m.Deactivate();
		}

		var removed = _quests.RemoveAll(m => m.Name == questName);

		foreach (PlayerQuestBase quest in removedQuests)
		{
			OnQuestRemoved(quest);
		}

		return removed > 0;
	}

	/// <summary>
	/// 更改指定任务的状态
	/// </summary>
	/// <param name="quest">任务实例</param>
	/// <param name="fromState">任务当前状态</param>
	/// <param name="toState">任务目标状态</param>
	public void ChangeQuestState(PlayerQuestBase quest, PlayerQuestState fromState, PlayerQuestState toState)
	{
		if (fromState == toState)
		{
			return;
		}

		quest.State = toState;

		if (toState == PlayerQuestState.Accepted)
		{
			quest.Activate();
		}
		else
		{
			quest.Deactivate();
		}

		OnQuestStatusUpdated(quest);
	}

	/// <summary>
	/// 获取指定状态的所有任务
	/// <para/>注: 该方法返回的是任务列表的副本，修改该副本不会造成任何影响
	/// </summary>
	/// <param name="state">任务状态</param>
	/// <returns></returns>
	public List<PlayerQuestBase> GetQuests(PlayerQuestState state) => _quests.Where(m => m.State == state).ToList();

	#endregion

	#region Data Persistance

	/// <summary>
	/// Save quests to player file data.
	/// <br/>Should only be called by <see cref="ModPlayer.SaveData(TagCompound)"/>.
	/// </summary>
	/// <param name="tag">Provided by <see cref="ModPlayer.SaveData(TagCompound)"/>.</param>
	public void SaveData(TagCompound tag)
	{
		tag.Add(nameof(_nPCKillCounter), _nPCKillCounter.ToList());
		tag.Add("_quests", _quests.ConvertAll(m =>
		{
			TagCompound mainT = [];
			TagCompound t = [];
			m.SaveData(t);
			mainT.Add("Type", m.GetType().FullName);
			mainT.Add("Data", t);
			return mainT;
		}));
	}

	/// <summary>
	/// Load quests from player file data.
	/// <br/>Should only be called by <see cref="ModPlayer.LoadData(TagCompound)"/>.
	/// </summary>
	/// <param name="tag">Provided by <see cref="ModPlayer.LoadData(TagCompound)"/>.</param>
	public PlayerQuestManagerData LoadData(TagCompound tag)
	{
		// Load npc kill counter.
		var nPCKillCounter = new Dictionary<int, int>();
		tag.TryGet<List<KeyValuePair<int, int>>>(nameof(_nPCKillCounter), out var nPCKillCounterStorage);
		if (nPCKillCounterStorage != null && nPCKillCounterStorage.Count > 0)
		{
			nPCKillCounter = nPCKillCounterStorage.ToDictionary();
		}

		// Load quests.
		var quests = new List<PlayerQuestBase>();

		if (tag.TryGet<IList<TagCompound>>("_quests", out var questTags))
		{
			foreach (var mTag in questTags)
			{
				if (mTag.TryGet<string>("Type", out var typeName)
					&& mTag.TryGet<TagCompound>("Data", out var data))
				{
					var type = Ins.ModuleManager.Types.FirstOrDefault(t => t.FullName == typeName);
					if (type != null
						&& Activator.CreateInstance(type) is PlayerQuestBase m)
					{
						m.LoadData(data);
						quests.Add(m);
					}
					else
					{
						Ins.Logger.Warn($"Invalid type {typeName} detected from player file.");
					}
				}
				else
				{
					Ins.Logger.Warn($"Invalid quest data detected from player file.");
				}
			}
		}

		return new PlayerQuestManagerData(nPCKillCounter, quests);
	}

	#endregion

	public void OnQuestAdded(PlayerQuestBase quest) => QuestAdded?.Invoke(GetIdentity(quest));

	public void OnQuestRemoved(PlayerQuestBase quest) => QuestRemoved?.Invoke(GetIdentity(quest));

	public void OnQuestStatusUpdated(PlayerQuestBase quest) => QuestStatusUpdated?.Invoke(GetIdentity(quest));

	public void OnQuestObjectiveUpdated(PlayerQuestBase quest) => QuestObjectiveUpdated?.Invoke(GetIdentity(quest));

	private static QuestIdentity GetIdentity(PlayerQuestBase quest) => new(QuestSide.Player, quest.Name, quest.InstanceId);
}
