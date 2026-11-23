namespace Everglow.Commons.TileHelper;

public class AirTileItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<AirTile>());
		Item.consumable = false;
		Item.maxStack = 1;
	}
}
