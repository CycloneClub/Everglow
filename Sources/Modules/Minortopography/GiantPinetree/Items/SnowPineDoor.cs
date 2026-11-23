namespace Everglow.Minortopography.GiantPinetree.Items;

public class SnowPineDoor : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<TilesAndWalls.SnowPineDoorClosed>());
		Item.height = 28;
		Item.value = 150;
	}
}
