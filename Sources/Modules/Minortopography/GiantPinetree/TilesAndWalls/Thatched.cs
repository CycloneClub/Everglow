namespace Everglow.Minortopography.GiantPinetree.TilesAndWalls;

public class Thatched : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][TileID.PineTree] = true;
		Main.tileMerge[TileID.PineTree][Type] = true;
		DustType = ModContent.DustType<Dusts.ThatchedDust>();
		HitSound = SoundID.Grass;
		AddMapEntry(new Color(88, 70, 64));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}
}
