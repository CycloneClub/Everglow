using Everglow.Commons.Utilities;

namespace Everglow.Commons.Templates;

public abstract class FishingJunk : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Fishing;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Gray;
		Item.value = 0;
	}
}
