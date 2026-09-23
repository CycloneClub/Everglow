using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Everglow.Yggdrasil.YggdrasilTown.Walls;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class GiantFurnacePlaceItem : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 10;
		Item.height = 30;
		Item.maxStack = Item.CommonMaxStack;
		Item.createTile = ModContent.TileType<GiantFurnace>();
		Item.placeStyle = 0;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.useAnimation = 15;
		Item.useTime = 15;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.consumable = true;
		Item.value = Item.sellPrice(0, 0, 1, 0);
		Item.rare = ItemRarityID.White;
	}

	public override void HoldItem(Player player)
	{
		// Main.placementPreview = true;
	}

	public override bool CanUseItem(Player player)
	{
		// var giantFurnace = TileLoader.GetTile(ModContent.TileType<GiantFurnace>()) as GiantFurnace;
		// if (giantFurnace != null)
		// {
		// int x = (int)(Main.MouseWorld.X / 16 - 24);
		// int y = (int)(Main.MouseWorld.Y / 16);
		// giantFurnace.PlaceOriginAtBottomLeft(x, y);
		// Item.stack--;
		// return false;
		// }
		int x = Main.MouseWorld.ToTileCoordinates().X;
		int y = Main.MouseWorld.ToTileCoordinates().Y;
		YggdrasilWorldGeneration.QuickBuild(x, y, ModAsset.YggdrasilTown_New_706x275_Path);
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return false;
	}
}
