using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilWorldGeneration;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Tools.Developer;

public class BoneAndPlatformCreate : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

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
		int x0 = (int)(Main.MouseWorld.X / 16);
		int y0 = (int)(Main.MouseWorld.Y / 16);
		if (player.direction == -1)
		{
			x0 -= 60;
		}
		TileUtils.PlaceFrameImportantTiles(x0, y0, 60, 1, ModContent.TileType<BoneAndPlatform_tile>(), 0, 0);
		return false;
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
