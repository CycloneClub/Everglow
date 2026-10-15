using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class PlayerQuestViewAdapterTest
{
	[TestMethod]
	public void Create_IncludesSourceAndObjectiveIcons()
	{
		var source = new StubSource("source");
		var subSource = new StubSource("sub-source");
		var icon = new StubIcon();
		var quest = new StubQuest
		{
			SourceValue = source,
			SubSourceValue = subSource,
		};
		quest.Objectives.Add(new StubObjective("objective") { Icon = icon });

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.HasCount(2, view.Icons);
		var sourceIcon = (QuestSourceIcon)view.Icons[0];
		Assert.AreSame(source, sourceIcon.Source);
		Assert.AreSame(subSource, sourceIcon.SubSource);
		Assert.AreSame(icon, view.Icons[1]);
	}

	[TestMethod]
	public void Create_SnapshotsObjectiveIcons()
	{
		var objectiveIcon = new StubIcon();
		var addedLater = new StubIcon();
		var objective = new StubObjective("objective") { Icon = objectiveIcon };
		var quest = new StubQuest();
		quest.Objectives.Add(objective);

		QuestView view = PlayerQuestViewAdapter.Create(quest);
		objective.Icon = addedLater;

		Assert.HasCount(2, view.Icons);
		Assert.IsInstanceOfType<QuestSourceIcon>(view.Icons[0]);
		Assert.AreSame(objectiveIcon, view.Icons[1]);
	}

	[TestMethod]
	public void Create_NoObjectiveIconsProducesSourceIcon()
	{
		var quest = new StubQuest();

		QuestView view = PlayerQuestViewAdapter.Create(quest);

		Assert.IsNotNull(view.Icons);
		Assert.HasCount(1, view.Icons);
		Assert.IsInstanceOfType<QuestSourceIcon>(view.Icons[0]);
	}
}
