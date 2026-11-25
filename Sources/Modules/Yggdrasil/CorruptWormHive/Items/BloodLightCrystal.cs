namespace Everglow.Yggdrasil.CorruptWormHive.Items;

public class BloodLightCrystal : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.BloodLightCrystal>());
		Item.width = 16;
		Item.height = 16;
		Item.rare = ItemRarityID.White;
		Item.maxStack = 999;
		Item.value = 0;
	}
}
