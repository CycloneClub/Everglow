using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles;

public class CocoonRock : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileMerge[ModContent.TileType<DarkCocoon>()][Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoon>()] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			18,
		};
		TileObjectData.newTile.CoordinateWidth = 16;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		DustType = 191;
		AddMapEntry(new Color(25, 24, 25));
		HitSound = SoundID.Dig;
	}
}
