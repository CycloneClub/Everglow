using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

public partial class WorldQuestViewAdapterTest
{
	[TestMethod]
	public void Create_MapsRewardItemsByReferenceAndSnapshotsWithoutClaimingThem()
	{
		var firstReward = new Item { type = ItemID.IronPickaxe, stack = 3 };
		var secondReward = new Item { type = ItemID.DirtBlock, stack = 5 };
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
