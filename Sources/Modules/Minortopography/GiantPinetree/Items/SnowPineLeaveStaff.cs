using Terraria.GameContent.Creative;

namespace Everglow.Minortopography.GiantPinetree.Items;

public class SnowPineLeaveStaff : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
		CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
	}

	public override void SetDefaults()
	{
		Item.DefaultToPlaceableTile(ModContent.TileType<TilesAndWalls.PineLeaves>());
		Item.width = 48;
		Item.height = 48;
		Item.tileWand = ItemID.BorealWood;
		Item.value = 2000;
		Item.consumable = false;
		Item.maxStack = 1;
	}
}
