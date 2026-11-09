using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class GiveItemQuestTest : ExamplePlayerQuest
{
	public override string DisplayName => GetType().Name;

	public GiveItemQuestTest()
	{
		var objective = new GiveItemObjective([ItemID.DirtBlock], 10, ModContent.NPCType<ExamplePerson>());
		objective.WithDialogue(d => d
			.WithText("Dialogue.Request")
			.WithUnavailableText("Dialogue.Missing")
			.WithCompletionText("Dialogue.Success"));
		Objectives.Add(objective);
	}
}
