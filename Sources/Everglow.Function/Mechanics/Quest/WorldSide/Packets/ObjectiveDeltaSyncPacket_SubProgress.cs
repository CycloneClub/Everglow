using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Netcode;
using Everglow.Commons.Netcode.Abstracts;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Packets;

public class ObjectiveDeltaSyncPacket_SubProgress : IPacket
{
	private string _questName;

	private IDeltaSyncObjective syncObjective;

	public ObjectiveDeltaSyncPacket_SubProgress()
	{
	}

	public ObjectiveDeltaSyncPacket_SubProgress(string questName, IDeltaSyncObjective objective)
	{
		_questName = questName;
		syncObjective = objective;
	}

	public void Receive(BinaryReader reader, int whoAmI)
	{
		var questName = reader.ReadString();
		var objectiveId = reader.ReadInt32();
		var quest = WorldQuestManager.Instance.GetQuest(questName);
		if (quest is null
			|| quest.State != WorldQuestState.Active
			|| objectiveId < 0
			|| objectiveId >= quest.Objectives.AllObjectives.Count)
		{
			return;
		}

		var objective = quest.Objectives.AllObjectives[objectiveId];
		if (!objective.CanProgress || !quest.ActiveObjectives.Contains(objective))
		{
			return;
		}

		objective.ReceiveDelta(reader);
		ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_MainProgress(questName, objective), RouteDestination.AllDownstream);
	}

	public void Send(BinaryWriter writer)
	{
		writer.Write(_questName); // Quest id
		writer.Write((syncObjective as WorldObjectiveBase).ObjectiveID); // Objective id
		syncObjective.SendDelta(writer);
	}

	[HandlePacket(typeof(ObjectiveDeltaSyncPacket_SubProgress))]
	public class QuestDeltaSyncPacketHandler : IPacketHandler
	{
		public void Handle(IPacket packet, int whoAmI)
		{
		}
	}
}
