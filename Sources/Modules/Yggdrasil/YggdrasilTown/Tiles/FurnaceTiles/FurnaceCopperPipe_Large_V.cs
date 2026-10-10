using Terraria.DataStructures;
using Terraria.Enums;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

public class FurnaceCopperPipe_Large_V : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileMergeDirt[Type] = false;
		Main.tileNoAttach[Type] = true;
		Main.tileBlendAll[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x2);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 2;

		TileObjectData.newTile.AnchorTop = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.AnchorBottom = new AnchorData(AnchorType.None, 0, 0);
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16 };
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(1, 1);
		TileObjectData.addTile(Type);
		DustType = DustID.Copper;
		HitSound = default;

		AddMapEntry(new Color(132, 84, 58));
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		var tile = Main.tile[i, j];
		var topLeft = new Point(i - tile.TileFrameX % 36 / 18, j - tile.TileFrameY / 18);
		int style = 3;

		var upTile = Main.tile[topLeft + new Point(0, -1)];
		var downTile = Main.tile[topLeft + new Point(0, 3)];
		if (ConnectUp(upTile))
		{
			if (ConnectDown(downTile))
			{
				style = 1;
			}
			else
			{
				style = 2;
			}
			int upStyle = upTile.TileFrameX / 36;
			if (upStyle == 3)
			{
				SetFrame(0, topLeft + new Point(0, -3));
			}
			if (upStyle == 2)
			{
				SetFrame(1, topLeft + new Point(0, -3));
			}
		}
		else if (ConnectDown(downTile))
		{
			style = 0;
		}

		if (ConnectDown(downTile))
		{
			int downStyle = downTile.TileFrameX / 36;
			if (downStyle == 3)
			{
				SetFrame(2, topLeft + new Point(0, 3));
			}
			if (downStyle == 0)
			{
				SetFrame(1, topLeft + new Point(0, 3));
			}
		}
		SetFrame(style, topLeft);
		resetFrame = false;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public bool ConnectUp(Tile tile)
	{
		if (tile.TileType == ModContent.TileType<FurnaceCopperPipe_Large_Corner>())
		{
			return (tile.TileFrameX == 0 && tile.TileFrameY == 36) || (tile.TileFrameX == 72 && tile.TileFrameY == 36);
		}
		return tile.TileType == Type && tile.TileFrameX % 36 == 0 && tile.TileFrameY == 36;
	}

	public bool ConnectDown(Tile tile)
	{
		if (tile.TileType == ModContent.TileType<FurnaceCopperPipe_Large_Corner>())
		{
			return (tile.TileFrameX == 108 && tile.TileFrameY == 0) || (tile.TileFrameX == 180 && tile.TileFrameY == 0);
		}
		return tile.TileType == Type && tile.TileFrameX % 36 == 0 && tile.TileFrameY == 0;
	}

	public void SetFrame(int style, Point topLeft)
	{
		var tile = Main.tile[topLeft];
		if (tile.TileType == Type)
		{
			for (int x = 0; x < 2; x++)
			{
				for (int y = 0; y < 3; y++)
				{
					var framedTile = Main.tile[topLeft + new Point(x, y)];
					framedTile.TileFrameX = (short)(style * 36 + x * 18);
					framedTile.TileFrameY = (short)(y * 18);
				}
			}
		}
	}
}
