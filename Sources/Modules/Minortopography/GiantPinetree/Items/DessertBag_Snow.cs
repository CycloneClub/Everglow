using Terraria.GameContent;
using Terraria.GameContent.Creative;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Minortopography.GiantPinetree.Items;

// Basic code for a boss treasure bag
public class DessertBag_Snow : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Miscs;

	public override void SetStaticDefaults()
	{
		ItemID.Sets.OpenableBag[Type] = true; // ..But this set ensures that dev armor will only be dropped on special world seeds, since that's the behavior of pre-hardmode boss bags.
	}

	public override void SetDefaults()
	{
		Item.maxStack = 9999;
		Item.consumable = true;
		Item.width = 26;
		Item.height = 40;
		Item.rare = ItemRarityID.White;
		Item.ResearchUnlockCount = 1;
	}

	public override bool CanRightClick()
	{
		return true;
	}

	public override void ModifyItemLoot(ItemLoot itemLoot)
	{
		IItemDropRule[] oreBars = new IItemDropRule[]
		{
			ItemDropRule.Common(ItemID.Eggnog, 1, 2, 4),
			ItemDropRule.Common(ItemID.SugarCookie, 1, 2, 4),
			ItemDropRule.Common(ItemID.GingerbreadCookie, 1, 2, 4),
			ItemDropRule.Common(ItemID.ChristmasPudding, 1, 2, 4),
			ItemDropRule.Common(ItemID.SliceOfCake, 1, 2, 4),
		};
		itemLoot.Add(new OneFromRulesRule(1, oreBars));
	}
}
