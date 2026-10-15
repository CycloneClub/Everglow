using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class MetroSign : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 7;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(153, 45, 45));
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}
}
