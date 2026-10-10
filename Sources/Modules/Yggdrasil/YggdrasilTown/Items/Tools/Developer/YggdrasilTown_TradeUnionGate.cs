using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class YggdrasilTown_TradeUnionGate : ModItem
{
	public override void SetDefaults()
	{
		Item.width = 30;
		Item.height = 30;
		Item.useTurn = true;
		Item.useAnimation = 4;
		Item.useTime = 4;
		Item.autoReuse = false;
		Item.useStyle = ItemUseStyleID.Swing;
	}

	public override bool CanUseItem(Player player)
	{
		// int x0 = (int)(Main.MouseWorld.X / 16);
		// int y0 = (int)(Main.MouseWorld.Y / 16);
		// TileUtils.PlaceFrameImportantTiles(x0, y0, 1, 1, ModContent.TileType<MarbleGate>(), 0, 0);
		Main.NewText(YggdrasilTownCentralSystem.TownPos(Main.MouseWorld));
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
