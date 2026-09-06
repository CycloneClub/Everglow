using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
public class QuestViewComparerTest
{
	[TestMethod]
	public void Compare_SortsQuestTypesByDocumentedDisplayOrder()
	{
		QuestView[] quests =
		[
			new QuestView { State = QuestViewState.Active, Type = QuestType.None },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Legend },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Daily },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Challenge },
			new QuestView { State = QuestViewState.Active, Type = QuestType.Achievement },
			new QuestView { State = QuestViewState.Active, Type = QuestType.SideStory },
			new QuestView { State = QuestViewState.Active, Type = QuestType.MainStory },
		];

		QuestType[] actualOrder =
		[
			.. quests
				.OrderBy(quest => quest, QuestViewComparer.Instance)
				.Select(quest => quest.Type),
		];

		CollectionAssert.AreEqual(
			new[]
			{
				QuestType.MainStory,
				QuestType.SideStory,
				QuestType.Achievement,
				QuestType.Challenge,
				QuestType.Daily,
				QuestType.Legend,
				QuestType.None,
			},
			actualOrder);
	}

	[TestMethod]
	public void Compare_SortsActiveBeforeAvailableAndLockedLast()
	{
		QuestView[] quests =
		[
			new QuestView { State = QuestViewState.Available, Type = QuestType.MainStory, DisplayName = "A" },
			new QuestView { State = QuestViewState.Locked, Type = QuestType.MainStory, DisplayName = "B" },
			new QuestView { State = QuestViewState.Active, Type = QuestType.MainStory, DisplayName = "C" },
			new QuestView { State = QuestViewState.Completed, Type = QuestType.MainStory, DisplayName = "D" },
			new QuestView { State = QuestViewState.Failed, Type = QuestType.MainStory, DisplayName = "E" },
		];

		QuestViewState[] actualOrder =
		[
			.. quests
				.OrderBy(quest => quest, QuestViewComparer.Instance)
				.Select(quest => quest.State),
		];

		CollectionAssert.AreEqual(
			new[]
			{
				QuestViewState.Active,
				QuestViewState.Available,
				QuestViewState.Completed,
				QuestViewState.Failed,
				QuestViewState.Locked,
			},
			actualOrder);
	}

	[TestMethod]
	public void Compare_SortsNoneAfterUndefinedQuestType()
	{
		var undefinedType = (QuestType)int.MaxValue;
		QuestView[] quests =
		[
			new QuestView { State = QuestViewState.Active, Type = QuestType.None },
			new QuestView { State = QuestViewState.Active, Type = undefinedType },
		];

		QuestType[] actualOrder =
		[
			.. quests
				.OrderBy(quest => quest, QuestViewComparer.Instance)
				.Select(quest => quest.Type),
		];

		CollectionAssert.AreEqual(new[] { undefinedType, QuestType.None }, actualOrder);
	}
}
