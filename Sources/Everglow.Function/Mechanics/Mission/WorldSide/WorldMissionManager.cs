using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.Core;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide;

public class WorldMissionManager
{
	// Add a config option for update interval if necessary.
#if DEBUG
	public const int UpdateInterval = 1;
	public const int NetUpdateInterval = 1;
#else
	public const int UpdateInterval = 30;
	public const int NetUpdateInterval = 60;
#endif

	public static bool SendText { get; private set; } = true;

	public static WorldMissionManager Instance => ModContent.GetInstance<WorldMissionSystem>().Manager;

	public static bool NetUpdate => Instance.UpdateTimer % NetUpdateInterval == 0;

	public static bool NormalUpdate => Instance.UpdateTimer % UpdateInterval == 0;

	private IGameStateProvider _gameState;

	private List<WorldMissionBase> _missions = [];

	public IReadOnlyList<WorldMissionBase> Missions => _missions;

	public IReadOnlyList<WorldMissionBase> ActiveMissions => _missions.Where(m => m.State == WorldMissionState.Active).ToList();

	private int UpdateTimer => (int)_gameState.TimeForVisualEffects;

	public WorldMissionManager()
	{
		_gameState = GameStateProvider.Default;
	}

	/// <summary>
	/// For test only
	/// </summary>
	/// <param name="gameStateProvider"></param>
	/// <param name="missions"></param>
	public WorldMissionManager(IGameStateProvider gameStateProvider)
	{
		_gameState = gameStateProvider;
		SendText = false;
	}

	public void Load()
	{
		// Initialize mission manager: load missions, set up main hooks, etc.
		_missions = [];

		var source = ModLoader.Mods
			.Select(m => (m.Name, AssemblyManager.GetLoadableTypes(m.Code).AsEnumerable()))
			.Concat([(nameof(Everglow), Ins.ModuleManager.Types)])
			.Distinct();
		foreach (var (modName, modTypes) in source)
		{
			foreach (var mT in modTypes.Where(t => t.IsSubclassOf(typeof(WorldMissionBase)) && !t.IsAbstract))
			{
				var mission = Activator.CreateInstance(mT) as WorldMissionBase;
				mission.WhoAmI = _missions.Count;
				_missions.Add(mission);
			}
		}
		Main.OnTickForInternalCodeOnly += Update;
	}

	public void Unload()
	{
		// Clean up mission manager: clear mission data, remove hooks, etc.
		Main.OnTickForInternalCodeOnly -= Update;
		Reset();
		_missions = null;
	}

	public void Initialize()
	{
		foreach (var m in _missions)
		{
			m.Initialize();
		}
	}

	public void Reset()
	{
		_missions.ForEach(m =>
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
			foreach (var m in _missions.Where(m => m.State == WorldMissionState.Locked))
			{
				if (m.CanUnlock())
				{
					m.Unlock();
				}
			}

			// Check active
			foreach (var m in _missions.Where(m => m.State == WorldMissionState.Active))
			{
				m.Update();
			}
		}

		if (NetUpdate && !NetUtils.IsSingle)
		{
			foreach (var m in _missions.Where(m => m.State == WorldMissionState.Active))
			{
				m.OnMPSync();
			}
		}
	}

	public WorldMissionBase GetMission(int whoAmI) =>
		_missions.FirstOrDefault(m => m.WhoAmI == whoAmI);

	public WorldMissionBase GetMission(string name) =>
		_missions.FirstOrDefault(m => m.Name == name);

	public WorldMissionBase GetMission<T>()
		where T : WorldMissionBase =>
		_missions.OfType<T>().FirstOrDefault();

	public IEnumerable<WorldMissionBase> GetMissions(WorldMissionState state) =>
		_missions.Where(m => m.State == state);

	public void AddMission(WorldMissionBase mission)
	{
		if (_missions.Any(m => m.Name == mission.Name))
		{
			throw new InvalidOperationException($"Mission with name {mission.Name} already exists.");
		}
		_missions.Add(mission);
	}

	public void GiveRewards(WorldMissionBase mission)
	{
		// The request for giving rewards should be sent to server first for validation
		// before the actual method to give rewards is called.
		throw new NotImplementedException();
	}

	public bool ResetMission(WorldMissionBase mission)
	{
		throw new NotImplementedException();
	}

	public static void NewText(string text, byte R, byte G, byte B)
	{
		NewText(text, new Color(R, G, B));
	}

	public static void NewText(string text, Color color)
	{
		if (SendText)
		{
			Main.NewText(text, color);
			Console.WriteLine(text);
		}
	}

	#region Persistence & Netcode

	public void NetSend(BinaryWriter writer)
	{
		foreach (var m in _missions)
		{
			m.NetSend(writer);
		}
		Console.WriteLine("Full sync msg sent!");
	}

	public void NetReceive(BinaryReader reader)
	{
		foreach (var m in _missions)
		{
			m.NetReceive(reader);
		}
		Console.WriteLine("Full sync msg received!");
	}

	public void SaveData(TagCompound tag)
	{
		foreach (var m in _missions)
		{
			var mTag = new TagCompound();
			m.SaveData(mTag);
			tag.Add(m.Name, mTag);
		}
	}

	public void LoadData(TagCompound tag)
	{
		Reset();

		foreach (var m in _missions)
		{
			if (tag.TryGet<TagCompound>(m.Name, out var mTag))
			{
				m.LoadData(mTag);
			}
			else
			{
				// Handle missing mission data if necessary.
			}
		}
	}

	#endregion
}
