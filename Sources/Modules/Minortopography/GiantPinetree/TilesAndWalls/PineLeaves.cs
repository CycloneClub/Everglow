using Everglow.Commons.Utilities;
using Everglow.Minortopography.GiantPinetree.Dusts;

namespace Everglow.Minortopography.GiantPinetree.TilesAndWalls;

public class PineLeaves : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][TileID.PineTree] = true;
		Main.tileMerge[TileID.PineTree][Type] = true;
		TileUtils.Sets.TileFragile[Type] = true;
		TileID.Sets.SnowBiome[Type] = 5;
	}

	public override void PostSetDefaults()
	{
		DustType = ModContent.DustType<PineDust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(36, 64, 50));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}
}

public class PineSnowSystem : ModSystem
{
	public int PineLeavesCount;

	public override void TileCountsAvailable(ReadOnlySpan<int> tileCounts)
	{
		PineLeavesCount = tileCounts[ModContent.TileType<PineLeaves>()];
	}
}
