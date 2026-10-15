using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class TalkNPCObjectiveTest
{
	[TestMethod]
	public void Constructor_Should_ThrowException_When_NPCTypeIsLessThanOne()
	{
		for (int i = 0; i > -100; i--)
		{
			Assert.ThrowsExactly<InvalidDataException>(() => new TalkNPCObjective(i, "111"));
		}
	}

	[TestMethod]
	public void Constructor_Should_ThrowException_When_NPCTextIsNullOrEmpty()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => new TalkNPCObjective(1, string.Empty));
		Assert.ThrowsExactly<ArgumentNullException>(() => new TalkNPCObjective(1, null));
	}
}
