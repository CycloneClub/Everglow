using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public class ObjectiveDeltaSyncPacket_MainProgress : IPacket
{
	private string _questName;

	private IDeltaSyncObjective syncObjective;

	public ObjectiveDeltaSyncPacket_MainProgress()
	{
	}

	public ObjectiveDeltaSyncPacket_MainProgress(string questName, IDeltaSyncObjective objective)
	{
		_questName = questName;
		syncObjective = objective;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		var questName = reader.ReadString();
		var objectiveId = reader.ReadInt32();
		var quest = WorldQuestManager.Instance.GetQuest(questName);
		var objective = quest.Objectives.AllObjectives[objectiveId];
		if (objective is IDeltaSyncObjective deltaSyncObjective)
		{
			deltaSyncObjective.ReceiveMain(reader);
		}
		else
		{
			Ins.Logger.Error($"{questName} {objectiveId} {objective.GetType().Name} is not {nameof(IDeltaSyncObjective)}.");
		}
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_questName);
		writer.Write((syncObjective as WorldObjectiveBase).ObjectiveID);
		syncObjective.SendMain(writer);
	}

	[HandlePacket(typeof(ObjectiveDeltaSyncPacket_MainProgress))]
	public class QuestDeltaSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
