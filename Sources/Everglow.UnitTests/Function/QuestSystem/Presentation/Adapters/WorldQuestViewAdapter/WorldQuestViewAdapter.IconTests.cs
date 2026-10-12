using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_OnlyIncludesCurrentObjectiveIconsAndKeepsSourceSeparate()
	{
		var source = new StubSource("source");
		var currentIcon = new StubIcon();
		var futureIcon = new StubIcon();
		var quest = new StubQuest { SourceValue = source };
		quest.Objectives.Add(new StubObjective() { Icon = currentIcon });
		quest.Objectives.Add(new StubObjective() { Icon = futureIcon });

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.AreSame(source, view.Source);
		Assert.HasCount(1, view.Icons);
		Assert.AreSame(currentIcon, view.Icons[0]);
	}

	[TestMethod]
	public void Create_ParallelObjectivesBothContributeIcons()
	{
		var firstIcon = new StubIcon();
		var secondIcon = new StubIcon();
		var quest = new StubQuest();
		quest.Objectives.AddParallel(new StubObjective() { Icon = firstIcon }, new StubObjective() { Icon = secondIcon });

		QuestView view = WorldQuestViewAdapter.Create(quest);

		CollectionAssert.AreEqual(new[] { firstIcon, secondIcon }, view.Icons.ToArray());
	}

	[TestMethod]
	public void Create_NoObjectiveIconsProducesEmptyIcons()
	{
		var quest = new StubQuest();

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.IsNotNull(view.Icons);
		Assert.IsEmpty(view.Icons);
	}
}
