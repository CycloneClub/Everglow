using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using ModLiquidLib.ModLoader;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class DarkSludgeLiquid_SolidBlock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<SplashDust_DarkSludge>();
		MinPick = int.MaxValue;
		AddMapEntry(new Color(31, 26, 45));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void RandomUpdate(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		tile.ClearTile();
		tile.LiquidType = LiquidLoader.LiquidType<DarkSludgeLiquid>();
		tile.LiquidAmount = byte.MaxValue;
		WorldGen.SquareTileFrame(i, j);
	}
}
