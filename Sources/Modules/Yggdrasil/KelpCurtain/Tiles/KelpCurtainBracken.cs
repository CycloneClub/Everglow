using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class KelpCurtainBracken : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLavaDeath[Type] = true;
		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			30,
		};
		TileObjectData.newTile.CoordinateWidth = 20;
		TileObjectData.newTile.DrawYOffset = -12;
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = true;
		TileObjectData.addTile(Type);
		TileID.Sets.SwaysInWindBasic[Type] = true;
		DustType = ModContent.DustType<SucculentHerbDust>();
		AddMapEntry(new Color(89, 130, 46));
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameX > 88)
		{
			tile.TileFrameX -= 22;
		}
		if (tile.TileFrameX < 66)
		{
			tile.TileFrameX += 22;
		}
		base.RandomUpdate(i, j, underground);
	}
}
