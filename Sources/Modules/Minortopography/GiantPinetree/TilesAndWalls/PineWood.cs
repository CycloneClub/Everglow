namespace Everglow.Minortopography.GiantPinetree.TilesAndWalls;

public class PineWood : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][TileID.PineTree] = true;
		Main.tileMerge[TileID.PineTree][Type] = true;
		DustType = DustID.BorealWood;
		HitSound = SoundID.Dig;
		AddMapEntry(new Color(60, 45, 39));
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}
}
