using Everglow.Commons.Mechanics.Mission.Hooks;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;

public class WorldKillNPCObjective : WorldObjectiveBase
{
	public WorldKillNPCObjective()
	{
	}

	public WorldKillNPCObjective(int type, int count)
	{
		NPCType = type;
		NPCCount = count;
	}

	private int _localKillCount = 0;

	public int NPCType { get; private set; }

	public int NPCCount { get; private set; }

	public int KilledCount { get; private set; }

	public override float Progress => Math.Clamp(KilledCount / (float)NPCCount, 0, 1);

	public override bool NeedDeltaSync { get; protected set; } = false;

	public override bool CheckCompletion() => KilledCount >= NPCCount;

	public override void GetObjectivesText() => throw new NotImplementedException();

	public override void Activate(WorldMissionBase sourceMission)
	{
		MissionGlobalNPC.OnNPCKilled += WorldMissionGlobalNPC_OnNPCKilled;
	}

	public override void Deactivate()
	{
		MissionGlobalNPC.OnNPCKilled -= WorldMissionGlobalNPC_OnNPCKilled;
	}

	private void CountKill(int count = 1)
	{
		KilledCount += count;
		if (KilledCount > NPCCount)
		{
			KilledCount = NPCCount;
		}
	}

	private void WorldMissionGlobalNPC_OnNPCKilled(NPC npc)
	{
		if (npc.netID == NPCType)
		{
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
