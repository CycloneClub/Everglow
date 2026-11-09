#if DEBUG
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

namespace Everglow.Example.Quest.WorldSide;

public sealed class ExampleDialogueQuest : WorldQuestBase
{
	public override void Initialize()
	{
		int npcType = ModContent.NPCType<ExamplePerson>();
		Objectives
			.Add(new WorldTalkObjective(npcType)
				.WithDialogue(d => d
					.WithText("Dialogue.Intro", "Dialogue.Request")
					.WithConfirmButtonText("Dialogue.Accept")
					.WithCompletionText("Dialogue.Accepted")))
			.Add(new WorldGiveObjective(npcType, ItemID.DirtBlock, 10)
				.WithDialogue(d => d
					.WithText("Dialogue.Request")
					.WithConfirmButtonText("Dialogue.Deliver")
					.WithUnavailableText("Dialogue.Missing")
					.WithCompletionText("Dialogue.Thanks", "Dialogue.Goodbye"))
				.WithRewards(new Item(ItemID.SilverCoin)));
	}
}
#endif
