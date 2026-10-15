using Everglow.CagedDomain.Dusts;
using Everglow.Commons.Utilities;
using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.CagedDomain.Tiles;

public class SluiceWallLamp : ModTile
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
		TileObjectData.newTile.Width = 3;

		TileObjectData.newTile.CoordinateHeights = new int[2];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.newTile.Origin = new(1, 0);

		TileObjectData.addTile(Type);
		DustType = ModContent.DustType<SluiceWallLamp_dust>();
		AddMapEntry(new Color(255, 217, 142));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameX < 54)
		{
			r = 1.3f;
			g = 1.1f;
			b = 0.6f;
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
		FurnitureUtils.LightHitwire(i, j, Type, 3, 2);
	}
}
