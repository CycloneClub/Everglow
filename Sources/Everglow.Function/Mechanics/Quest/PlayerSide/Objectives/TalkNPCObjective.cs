using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class TalkNPCObjective : PlayerObjectiveBase
{
	public TalkNPCObjective()
	{
	}

	public TalkNPCObjective(int type, string text)
	{
		NPCType = type > NPCID.None
			? type
			: throw new InvalidDataException($"NPC type should more than 1.");

		NPCText = !string.IsNullOrEmpty(text)
			? text
			: throw new ArgumentNullException("Argument 'text' should not be empty!");
	}

	public int NPCType { get; set; }

	public string NPCText { get; set; }

	public override float Progress => Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType ? 1f : 0f;

	public override bool CheckCompletion() => Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType;

	public override void Complete()
	{
		base.Complete();

		Main.npcChatText = NPCText;
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

		return $"和{npc.TypeName}对话";
	}
}
