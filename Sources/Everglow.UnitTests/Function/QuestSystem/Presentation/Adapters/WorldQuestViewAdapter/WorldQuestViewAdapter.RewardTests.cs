using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Terraria;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_MapsRewardItemsByReferenceAndSnapshotsWithoutClaimingThem()
	{
		var firstReward = new Item { type = 1, stack = 3 };
		var secondReward = new Item { type = 2, stack = 5 };
		var quest = new StubQuest();
		quest.SetState(WorldQuestState.Completed);
		quest.AddReward(firstReward);

		QuestView view = WorldQuestViewAdapter.Create(quest);
		quest.AddReward(secondReward);

		Assert.HasCount(1, view.Rewards);
		Assert.AreSame(firstReward, view.Rewards[0].Item);
		Assert.AreEqual(string.Empty, view.Rewards[0].Description);
		Assert.IsEmpty(quest.RewardClaimedPlayers);
	}
}
