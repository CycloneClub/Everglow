using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Tests.ExampleNPC;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class GiveItemQuestTest : PlayerQuestBase
{
	public override string DisplayName => GetType().Name;

	public GiveItemQuestTest()
	{
		Objectives.Add(new GiveItemObjective([ItemID.DirtBlock], 10, ModContent.NPCType<ExamplePerson>(), "Give me xxxxx.", "Thank you"));
	}
}
