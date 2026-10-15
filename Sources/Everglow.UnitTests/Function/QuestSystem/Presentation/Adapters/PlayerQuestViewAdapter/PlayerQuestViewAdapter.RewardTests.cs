using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Terraria;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class PlayerQuestViewAdapterTest
{
	[TestMethod]
	public void Create_MapsRewardItemsByReferenceAndSnapshotsTheCollection()
	{
		var firstReward = new Item { type = 1, stack = 3 };
		var secondReward = new Item { type = 2, stack = 5 };
		var quest = new StubQuest();
		quest.RewardItems.Add(firstReward);

		QuestView view = PlayerQuestViewAdapter.Create(quest);
		quest.RewardItems.Add(secondReward);

		Assert.HasCount(1, view.Rewards);
		Assert.AreSame(firstReward, view.Rewards[0].Item);
		Assert.AreEqual(string.Empty, view.Rewards[0].Description);
	}
}
