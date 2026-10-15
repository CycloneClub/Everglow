using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class WorldObjectiveTextTest
{
	[TestMethod]
	public void ReachObjective_ReturnsAuthoredTextUnchanged()
	{
		const string text = "[TextDrawer,Text='到达目标',Color='1,2,3,255']";
		var objective = new WorldReachObjective(_ => true, text);

		Assert.AreEqual(text, objective.GetObjectiveText());
	}

	[TestMethod]
	public void ExploreObjective_AppendsSynchronizedProgressToAuthoredText()
	{
		var objective = new WorldExploreObjective(500, _ => true, "在丛林中探索");

		Assert.AreEqual("在丛林中探索 (0/500)", objective.GetObjectiveText());
	}

	[TestMethod]
	[DataRow("")]
	[DataRow(" ")]
	public void ConditionObjective_RejectsBlankAuthoredText(string text)
	{
		Assert.ThrowsExactly<ArgumentException>(() => new WorldReachObjective(_ => true, text));
	}

	[TestMethod]
	public void ConditionObjective_RejectsNullAuthoredText()
	{
		Assert.ThrowsExactly<ArgumentNullException>(() => new WorldReachObjective(_ => true, null));
	}
}
