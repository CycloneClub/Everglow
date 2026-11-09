using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Terraria.ModLoader.IO;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class TalkNPCObjective : PlayerObjectiveBase, IQuestDialogueObjective
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

	private bool dialogueSubmitted;

	public ObjectiveDialogue Dialogue { get; set; }

	public int NPCType { get; set; }

	public override float Progress => CheckCompletion() ? 1f : 0f;

	public override bool CheckCompletion() => dialogueSubmitted;

	public virtual string GetChat() => QuestText.Get("Objectives.Talk", Lang.GetNPCNameValue(NPCType));

	public bool TrySubmitObjective(Player player)
	{
		if (!CanProgress || !QuestChat.CanSubmitObjective(player, NPCType))
		{
			return false;
		}
		dialogueSubmitted = true;
		return true;
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		dialogueSubmitted = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag["DialogueSubmitted"] = dialogueSubmitted;
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		dialogueSubmitted = tag.GetBool("DialogueSubmitted");
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
