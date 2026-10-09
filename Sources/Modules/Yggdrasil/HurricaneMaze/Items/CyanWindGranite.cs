namespace Everglow.Yggdrasil.HurricaneMaze.Items;

public class CyanWindGranite : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Item.width = 16;
		Item.height = 16;
		Item.rare = ItemRarityID.White;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useTurn = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.maxStack = 999;
		Item.value = 0;
		Item.createTile = ModContent.TileType<Tiles.CyanWindGranite>();
	}
}
