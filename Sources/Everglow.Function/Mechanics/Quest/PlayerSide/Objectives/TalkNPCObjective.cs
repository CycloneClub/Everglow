using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class TalkNPCObjective : PlayerObjectiveBase
{
	public TalkNPCObjective()
	{
	}

	public TalkNPCObjective(int type)
	{
		NPCType = type > NPCID.None
			? type
			: throw new InvalidDataException($"NPC type should more than 1.");
	}

	public int NPCType { get; set; }

	public string NPCText => GetText("NPCText");

	public override float Progress => Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType ? 1f : 0f;

	public override bool CheckCompletion() => Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType;

	public override void Complete()
	{
		base.Complete();

		if (!string.IsNullOrEmpty(NPCText))
		{
			Main.npcChatText = NPCText;
		}
	}

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		iconGroup.Add(NPCQuestIcon.Create(NPCType, npc.TypeName));
	}

	public override string GetObjectiveText()
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);

		return QuestText.Get("Objectives.Talk", npc.TypeName);
	}
}
