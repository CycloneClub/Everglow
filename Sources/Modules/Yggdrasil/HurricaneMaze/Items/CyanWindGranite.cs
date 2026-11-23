namespace Everglow.Yggdrasil.HurricaneMaze.Items;

public class CyanWindGranite : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.CyanWindGranite>());
		Item.width = 16;
		Item.height = 16;
		Item.rare = ItemRarityID.White;
		Item.maxStack = 999;
		Item.value = 0;
	}
}
