using Everglow.Commons.Mechanics.Mission.Hooks;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;

public class KillNPCObjective : PlayerObjectiveBase
{
	public KillNPCObjective()
	{
	}

	public KillNPCObjective(List<int> npcTypes, int npcCount, bool enableIndividualCounter = false, Func<Player, NPC, bool> condition = null)
	{
		if (npcTypes.Count == 0 || npcCount <= 0)
		{
			throw new InvalidDataException();
		}

		NPCTypes = npcTypes;
		NPCCount = npcCount;
		EnableIndividualCounter = enableIndividualCounter;
		Condition = condition;
	}

	public List<int> NPCTypes { get; private set; } = [];

	public int NPCCount { get; private set; }

	public int KilledCount { get; private set; }

	public bool EnableIndividualCounter { get; private set; }

	public Func<Player, NPC, bool> Condition { get; set; }

	public override void OnInitialize()
	{
		base.OnInitialize();
		AssetUtils.LoadVanillaNPCTextures(NPCTypes);
	}

	public override bool CheckCompletion() => Progress >= 1f;

	public override float Progress => Math.Clamp((EnableIndividualCounter
		? KilledCount
		: PlayerMissionManager.NPCKillCounter.Where(x => NPCTypes.Contains(x.Key)).Sum(x => x.Value)) / (float)NPCCount, 0f, 1f);

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		foreach (var npcType in NPCTypes)
		{
			var npc = new NPC();
			npc.SetDefaults(npcType);
			iconGroup.Add(NPCMissionIcon.Create(npcType, npc.TypeName));
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		string progress = EnableIndividualCounter
				? $"({KilledCount}/{NPCCount})"
				: $"({PlayerMissionManager.NPCKillCounter.Where((pair) => NPCTypes.Contains(pair.Key)).Sum(pair => pair.Value)}/{NPCCount})";

		if (NPCTypes.Count > 1)
		{
			var npcString = string.Join(',', NPCTypes.ConvertAll(npcType =>
			{
				var npc = new NPC();
				npc.SetDefaults(npcType);
				return npc.TypeName;
			}));
			lines.Add($"击杀 {npcString} 合计{NPCCount}个 {progress}\n");
		}
		else
		{
			var npc = new NPC();
			npc.SetDefaults(NPCTypes.First());
			lines.Add($"击杀 {npc.TypeName} {NPCCount}个 {progress}\n");
		}
	}

	public override void Activate(PlayerMissionBase sourceMission)
	{
		MissionGlobalNPC.OnKillNPCEvent += CountKill;
	}

	public override void Deactivate()
	{
		MissionGlobalNPC.OnKillNPCEvent -= CountKill;
	}

	/// <summary>
	/// Count a matching NPC kill for this objective.
	/// </summary>
	/// <param name="npc">The killed NPC.</param>
	public void CountKill(NPC npc)
	{
		if (!EnableIndividualCounter || !NPCTypes.Contains(npc.type))
		{
			return;
		}

		if (Condition != null && !Condition(Main.LocalPlayer, npc))
		{
			return;
		}

		KilledCount++;
		if (KilledCount > NPCCount)
		{
			KilledCount = NPCCount;
		}
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (!EnableIndividualCounter)
		{
			return;
		}

		if (tag.TryGet<int>(nameof(KilledCount), out var killedCount))
		{
			KilledCount = Math.Min(killedCount, NPCCount);
		}
		else if (tag.TryGet<TagCompound>("DemandNPC", out var demandNPC))
		{
			var count = demandNPC.GetInt("Counter");
			if (count > 0)
			{
				KilledCount = Math.Min(count, NPCCount);
			}
		}
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(KilledCount), KilledCount);
	}
}
