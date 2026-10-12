namespace Everglow.Myth.TheFirefly.Items;

public class FireflyMoss_Item : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Placeables;

	public override void SetDefaults()
	{
		Item.width = 20;
		Item.height = 18;
		Item.useStyle = ItemUseStyleID.Swing;
		Item.useAnimation = 15;
		Item.useTime = 10;
		Item.useTurn = true;
		Item.autoReuse = true;
		Item.consumable = true;
		Item.maxStack = Item.CommonMaxStack;
	}

	public override bool? UseItem(Player player)
	{
		int i = (int)(Main.MouseWorld.X / 16);
		int j = (int)(Main.MouseWorld.Y / 16);
		Tile tile = Main.tile[i, j];
		if (tile.TileType == ModContent.TileType<Tiles.DarkCocoon>())
		{
			tile.TileType = (ushort)ModContent.TileType<Tiles.DarkCocoonMoss>();
			WorldGen.SquareTileFrame(i, j, true);
			NetMessage.SendTileSquare(player.whoAmI, i, j, TileChangeType.None);
			return true;
		}
		else
		{
			return false;
		}
	}

	public override bool CanUseItem(Player player)
	{
		int i = (int)(Main.MouseWorld.X / 16);
		int j = (int)(Main.MouseWorld.Y / 16);
		Tile tile = Main.tile[i, j];
		if (tile.TileType == ModContent.TileType<Tiles.DarkCocoon>())
		{
			return true;
		}
		else
		{
			return false;
		}
	}
}
