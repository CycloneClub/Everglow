using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilTownElevatorTunnelLamp : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = 2;
		TileObjectData.newTile.Width = 1;

		TileObjectData.newTile.CoordinateHeights = new int[2];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 0);

		TileObjectData.addTile(Type);
		AddMapEntry(new Color(255, 196, 110));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX >= 18)
		{
			r = g = b = 0;
		}
		else
		{
			r = 1f;
			g = 0.8f;
			b = 0.45f;
		}
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override void HitWire(int i, int j)
	{
		FurnitureUtils.LightHitwire(i, j, Type, 1, 2);
		base.HitWire(i, j);
	}
}
