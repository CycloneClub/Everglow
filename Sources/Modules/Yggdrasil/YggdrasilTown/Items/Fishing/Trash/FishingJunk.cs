using Everglow.Commons.Utilities;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.Trash;

public abstract class FishingJunk : ModItem
{
	public override string LocalizationCategory => LocalizationUtils.Categories.Fishing;

	public override string Texture => Commons.ModAsset.White_Mod;

	public override void SetStaticDefaults()
	{
		Item.ResearchUnlockCount = 1;
	}

	public override void SetDefaults()
	{
		Item.width = 24;
		Item.height = 24;
		Item.scale = 0.1f;
		Item.maxStack = Item.CommonMaxStack;
		Item.rare = ItemRarityID.Gray;
		Item.value = 0;
	}
}
