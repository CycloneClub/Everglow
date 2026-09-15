using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public class GravelStoreWall_Thin : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = ModContent.DustType<GravelStore_Dust>();
		AddMapEntry(new Color(169, 178, 175));
	}

	public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
	{
		//Tile tileBelow = WorldGenMisc.SafeGetTile(i, j - 1);
		//if (tileBelow.WallType == ModContent.WallType<GravelStoreWall_Thick>())
		//{
		//	style = 4;
		//}
		return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
	}
}
