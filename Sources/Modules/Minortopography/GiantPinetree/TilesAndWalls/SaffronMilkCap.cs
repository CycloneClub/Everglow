using Terraria.ObjectData;

namespace Everglow.Minortopography.GiantPinetree.TilesAndWalls;

public class SaffronMilkCap : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.CoordinateHeights = new[] { 18 };
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(119, 77, 63));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		return false;
	}
}
