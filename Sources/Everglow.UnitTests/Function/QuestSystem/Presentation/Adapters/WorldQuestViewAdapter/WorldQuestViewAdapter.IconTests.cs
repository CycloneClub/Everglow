using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_IncludesSourceAndObjectiveIcons()
	{
		var source = new StubSource("world-source");
		var objectiveIcon = new StubIcon();
		var quest = new StubQuest { SourceValue = source };
		quest.Objectives.Add(new StubObjective { Icon = objectiveIcon });

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.HasCount(2, view.Icons);
		var sourceIcon = (QuestSourceIcon)view.Icons[0];
		Assert.AreSame(source, sourceIcon.Source);
		Assert.IsNull(sourceIcon.SubSource);
		Assert.AreSame(objectiveIcon, view.Icons[1]);
	}

	[TestMethod]
	public void Create_NoObjectiveIconsProducesSourceIcon()
	{
		var quest = new StubQuest();

		QuestView view = WorldQuestViewAdapter.Create(quest);

		Assert.HasCount(1, view.Icons);
		Assert.IsInstanceOfType<QuestSourceIcon>(view.Icons[0]);
	}
}
