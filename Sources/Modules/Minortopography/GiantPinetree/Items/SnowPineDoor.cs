namespace Everglow.Minortopography.GiantPinetree.Items;

public class SnowPineDoor : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.width = 14;
		Item.height = 28;
		Item.maxStack = Item.CommonMaxStack;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.value = 150;
		Item.createTile = ModContent.TileType<TilesAndWalls.SnowPineDoorClosed>();
	}
}
