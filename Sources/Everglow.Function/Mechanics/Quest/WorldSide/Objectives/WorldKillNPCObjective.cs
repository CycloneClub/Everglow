using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldKillNPCObjective : WorldObjectiveBase
{
	public WorldKillNPCObjective()
	{
	}

	public WorldKillNPCObjective(int type, int count)
		: this([type], count)
	{
	}

	public WorldKillNPCObjective(List<int> npcTypes, int npcCount)
	{
		if (npcTypes.Count == 0 || npcCount <= 0)
		{
			throw new InvalidDataException();
		}

		NPCTypes = npcTypes;
		NPCCount = npcCount;
	}

	private int _localKillCount = 0;

	public List<int> NPCTypes { get; private set; } = [];

	public int NPCCount { get; private set; }

	public int KilledCount { get; private set; }

	public override float Progress => Math.Clamp(KilledCount / (float)NPCCount, 0, 1);

	public override bool NeedDeltaSync { get; protected set; } = false;

	public override bool CheckCompletion() => KilledCount >= NPCCount;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var npcType in NPCTypes)
		{
			var npc = new NPC();
			npc.SetDefaults(npcType);
			iconGroup.Add(NPCQuestIcon.Create(npcType, npc.TypeName));
		}
	}

	public override string GetObjectiveText()
	{
		string progress = $"({KilledCount}/{NPCCount})";

		if (NPCTypes.Count > 1)
		{
			var npcString = string.Join(',', NPCTypes.ConvertAll(npcType =>
			{
				var npc = new NPC();
				npc.SetDefaults(npcType);
				return npc.TypeName;
			}));
			return $"击杀 {npcString} 合计{NPCCount}个 {progress}";
		}

		var single = new NPC();
		single.SetDefaults(NPCTypes.First());
		return $"击杀 {single.TypeName} {NPCCount}个 {progress}";
	}

	public override void Activate(WorldQuestBase sourceQuest)
	{
		QuestGlobalNPC.OnNPCKilled += WorldQuestGlobalNPC_OnNPCKilled;
	}

	public override void Deactivate()
	{
		QuestGlobalNPC.OnNPCKilled -= WorldQuestGlobalNPC_OnNPCKilled;
	}

	public void CountKill(NPC npc)
	{
		if (!NPCTypes.Contains(npc.netID))
		{
			return;
		}

		CountKill();
	}

	private void CountKill(int count = 1)
	{
		KilledCount += count;
		if (KilledCount > NPCCount)
		{
			KilledCount = NPCCount;
		}
	}

	private void WorldQuestGlobalNPC_OnNPCKilled(NPC npc)
	{
		if (!NPCTypes.Contains(npc.netID))
		{
			return;
		}

		if (NetUtils.IsSingle)
		{
			CountKill();
		}
		else if (NetUtils.IsMainServer)
		{
			CountKill();
			NeedDeltaSync = true;
		}
		else if (NetUtils.IsSubServer)
		{
			_localKillCount++;
			NeedDeltaSync = true;
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		KilledCount = 0;
		_localKillCount = 0;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(KilledCount), KilledCount);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet<int>(nameof(KilledCount), out var cc))
		{
			KilledCount = cc;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(KilledCount);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		KilledCount = reader.ReadInt32();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(_localKillCount);
		_localKillCount = 0;
		NeedDeltaSync = false;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		var count = br.ReadInt32();
		CountKill(count);
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(KilledCount);
		NeedDeltaSync = false;
	}

	public override void ReceiveMain(BinaryReader br)
	{
		KilledCount = br.ReadInt32();
	}
}
