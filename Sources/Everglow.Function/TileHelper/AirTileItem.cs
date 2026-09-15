namespace Everglow.Commons.TileHelper;

public class AirTileItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.useTime = 5;
		Item.useAnimation = 5;
		Item.createTile = ModContent.TileType<AirTile>();
		Item.useStyle = ItemUseStyleID.Swing;
		Item.autoReuse = true;
	}
}
