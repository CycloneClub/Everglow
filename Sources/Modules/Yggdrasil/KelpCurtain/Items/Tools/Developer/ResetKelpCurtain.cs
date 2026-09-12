using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.KelpCurtain.Items.Tools.Developer;

public class ResetKelpCurtain : ModItem
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

	public Point OldMousePos = default;

	public override void HoldItem(Player player) => base.HoldItem(player);

	public override bool CanUseItem(Player player)
	{
		KelpCurtainBiome.StratumBoundCurve.Clear();
		ClearRectangleAreaExclude(20, (int)(Main.maxTilesY * 0.75f), Main.maxTilesX - 20, (int)(Main.maxTilesY * 0.9f), ModContent.TileType<StoneScaleWood>());

		KelpCurtainGeneration.BuildKelpCurtain();

		// Point mouseTile = Main.MouseWorld.ToTileCoordinates();
		// Point mouseTile = Main.MouseWorld.ToTileCoordinates();
		// if (OldMousePos != default(Point))
		// {
		// KelpCurtainGeneration.ConnectMossyTunnel(OldMousePos, mouseTile, 8);
		// }
		// OldMousePos = mouseTile;
		// YggdrasilWorldGeneration.GenerateStalactite(Main.MouseWorld / 16f, 8, Main.rand.NextFloat(9, 42), ModContent.TileType<StoneScaleWood>());
		return false;
	}

	public static void ClearRectangleAreaExclude(int x0, int y0, int x1, int y1, int excludeType)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = TileUtils.SafeGetTile(x, y);
				if (tile.TileType != excludeType)
				{
					tile.ClearEverything();
				}
			}
		}
	}

	public override bool? UseItem(Player player)
	{
		return base.UseItem(player);
	}
}
