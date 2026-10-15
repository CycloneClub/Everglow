namespace Everglow.CagedDomain.Items;

public class SluiceWallLamp_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<Tiles.SluiceWallLamp>());
		Item.width = 24;
		Item.height = 18;
		Item.value = 1000;
	}
}
