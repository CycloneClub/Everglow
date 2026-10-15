using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionLampPost_Style1 : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLighted[Type] = true;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 4;
		TileObjectData.newTile.Width = 2;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			16,
			18,
		};
		TileObjectData.newTile.Origin = new Point16(0, 3);
		TileObjectData.addTile(Type);
		DustType = DustID.Gold;
		AddMapEntry(new Color(135, 103, 90));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX == 0 && tile.TileFrameY == 18)
		{
			r = 0.99f;
			g = 0.24f;
			b = 0.24f;
		}
		else if (tile.TileFrameX == 18 && tile.TileFrameY == 18)
		{
			r = 0.27f;
			g = 0.95f;
			b = 0.24f;
		}
		else if (tile.TileFrameX == 18 && tile.TileFrameY == 0)
		{
			r = 0.25f;
			g = 0.23f;
			b = 0.02f;
		}
		else if (tile.TileFrameX == 0 && tile.TileFrameY == 0)
		{
			r = 0.15f;
			g = 0.42f;
			b = 0.08f;
		}
		else
		{
			r = 0f;
			g = 0f;
			b = 0f;
		}
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 2, 4);
	}
}
