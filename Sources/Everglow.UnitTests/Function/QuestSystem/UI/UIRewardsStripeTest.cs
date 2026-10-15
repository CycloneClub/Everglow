using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class UIRewardsStripeTest
{
	[TestInitialize]
	public void Initialize() => Terraria.Program.SavePath = string.Empty;

	[TestMethod]
	public void SetRewards_CreatesStripeItemsFromRewardViews()
	{
		var reward = new RewardView
		{
			Item = new Item { type = ItemID.GoldBar, stack = 7 },
		};
		var stripe = new UIRewardsStripe();
		stripe.Info.Width.SetValue(500f);
		stripe.Info.Height.SetValue(200f);
		stripe.Calculation();

		stripe.SetRewards([reward]);

		Assert.HasCount(1, stripe.Rewards);
		Assert.AreSame(reward, stripe.Rewards[0]);
		Assert.HasCount(1, stripe.ChildrenElements);
		var stripeItem = (UIRewardsStripeItem)stripe.ChildrenElements[0];
		Assert.AreSame(reward, stripeItem.Reward);
	}

	[TestMethod]
	public void Constructor_CachesItemQuestIconForItemReward()
	{
		var reward = new RewardView
		{
			Item = new Item { type = ItemID.GoldBar, stack = 7 },
		};

		var stripeItem = new UIRewardsStripeItem(reward);

		Assert.IsInstanceOfType<ItemQuestIcon>(stripeItem.Icon);
	}
}
