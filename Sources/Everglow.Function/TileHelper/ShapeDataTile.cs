using Everglow.Commons.Utilities;
using Terraria.Audio;
using Terraria.ObjectData;

namespace Everglow.Commons.TileHelper;

public abstract class ShapeDataTile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileCut[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.newTile.Height = TotalHeight;
		TileObjectData.newTile.Width = TotalWidth;
		TileObjectData.newTile.CoordinateHeights = new int[TotalHeight];
		for (int i = 0; i < TotalHeight; i++)
		{
			TileObjectData.newTile.CoordinateHeights[i] = 16;
			if (i == TotalHeight - 1)
			{
				TileObjectData.newTile.CoordinateHeights[i] = 20;
			}
		}
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
	}

	public int[,] PixelHasTile;

	/// <summary>
	/// If top, bottom, left or right is not <see cref="PixelHasTile"/>, it must be solid tile for valid placement.
	/// </summary>
	public int[,] PixelHasAttach;
	public int TotalWidth;
	public int TotalHeight;

	/// <summary>
	/// 是否掉落多个物品
	/// </summary>
	public bool MultiItem = false;

	/// <summary>
	/// 物品种类
	/// </summary>
	public int CustomItemType = -1;

	/// <summary>
	/// 必填项,实际有物块的轮廓
	/// </summary>
	public virtual string ShapePath => Texture + "_shape.bmp";

	public override void Load()
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>(ShapePath);
		imageData.ProcessPixelRows(accessor =>
		{
			PixelHasTile = new int[accessor.Width, accessor.Height];
			PixelHasAttach = new int[accessor.Width, accessor.Height];
			TotalWidth = accessor.Width;
			TotalHeight = accessor.Height;
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					PixelHasTile[x, y] = pixel.R;
					PixelHasAttach[x, y] |= pixel.G;
				}
			}
		});
		base.Load();
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
		{
			return;
		}

		var thisTile = Main.tile[i, j];
		int x0 = i - thisTile.TileFrameX / 18;
		int y0 = j - thisTile.TileFrameY / 18;
		int times = 1;
		for (int x = 0; x < TotalWidth; x++)
		{
			for (int y = 0; y < TotalHeight; y++)
			{
				var tile = Main.tile[x0 + x, y0 + y];
				if (tile.TileFrameX == x * 18 && tile.TileFrameY == y * 18)
				{
					if (tile.TileType == Type && PixelHasTile[x, y] >= 200)
					{
						times++;
						tile.HasTile = false;
						for (int f = 0; f < 15; f++)
						{
							Dust.NewDust(new Vector2(x0 + x, y0 + y) * 16, 16, 16, DustType, 0, 0, 0, default, 1);
						}
						if (MultiItem)
						{
							CustomDropItem(x0 + x, y0 + y);
						}
					}
				}
			}
		}
		if (!MultiItem)
		{
			CustomDropItem(i, j);
		}
		SoundEngine.PlaySound(HitSound, new Vector2(i * 16, j * 16));
	}

	public virtual void CustomDropItem(int i, int j)
	{
		if (CustomItemType > 0)
		{
			Item.NewItem(WorldGen.GetItemSource_FromTileBreak(i, j), i * 16, j * 16, 16, 16, new Item(CustomItemType, 1));
		}
	}

	/// <summary>
	/// 从左上角安置一个造型物块
	/// </summary>
	/// <param name="pos"></param>
	public virtual void PlaceOriginAtTopLeft(Point pos)
	{
		PlaceOriginAtTopLeft(pos.X, pos.Y);
	}

	/// <summary>
	/// 从左上角安置一个造型物块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public virtual void PlaceOriginAtTopLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY - TotalHeight || y < 0)
		{
			return;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, j] >= 200)
				{
					Tile tile = Main.tile[x + i, y + j];
					tile.TileType = Type;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)(j * 18);
					tile.HasTile = true;
				}
			}
		}
	}

	/// <summary>
	/// 从左下角安置一个造型物块
	/// </summary>
	/// <param name="pos"></param>
	public virtual void PlaceOriginAtBottomLeft(Point pos)
	{
		PlaceOriginAtBottomLeft(pos.X, pos.Y);
	}

	/// <summary>
	/// 从左下角安置一个造型物块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	public virtual void PlaceOriginAtBottomLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY || y - TotalHeight < 0)
		{
			return;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, TotalHeight - j - 1] >= 200)
				{
					Tile tile = Main.tile[x + i, y - j];
					tile.TileType = Type;
					tile.TileFrameX = (short)(i * 18);
					tile.TileFrameY = (short)((TotalHeight - j - 1) * 18);
					tile.HasTile = true;
				}
			}
		}
	}

	public virtual void PlaceAtTileObjectDataOrigin(int i, int j)
	{
		TileObjectData tileObjectData = TileObjectData.GetTileData(Type, Main.LocalPlayer.HeldItem.placeStyle);
		i -= tileObjectData.Origin.X;
		j -= tileObjectData.Origin.Y;
		if (i + TotalWidth > Main.maxTilesX || i < 0 || j + TotalHeight > Main.maxTilesY || j < 0)
		{
			return;
		}
		for (int x = 0; x < TotalWidth; x++)
		{
			for (int y = 0; y < TotalHeight; y++)
			{
				if (PixelHasTile[x, y] >= 200)
				{
					Tile tile = Main.tile[x + i, y + j];
					tile.TileType = Type;
					tile.TileFrameX = (short)(x * 18);
					tile.TileFrameY = (short)(y * 18);
					tile.HasTile = true;
				}
			}
		}
		NetMessage.SendTileSquare(-1, i, j, TotalWidth, TotalHeight);
	}

	/// <summary>
	/// 判定底部平坦以及能否容纳下异形块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool CanPlaceAtBottomLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY || y - TotalHeight < 0)
		{
			return false;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, TotalHeight - j - 1] >= 200)
				{
					Tile tile = Main.tile[x + i, y - j];
					if (tile.HasTile)
					{
						if (!TileFragile(tile))
						{
							return false;
						}
					}
					if (j == 0)
					{
						if (!TileSolidWithNoSlope(x + i, y + 1))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	/// <summary>
	/// 判定顶部平坦以及能否容纳下异形块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public bool CanPlaceAtTopLeft(int x, int y)
	{
		if (x > Main.maxTilesX - TotalWidth || x < 0 || y > Main.maxTilesY || y - TotalHeight < 0)
		{
			return false;
		}

		for (int i = 0; i < TotalWidth; i++)
		{
			for (int j = 0; j < TotalHeight; j++)
			{
				if (PixelHasTile[i, j] >= 200)
				{
					Tile tile = Main.tile[x + i, y + j];
					if (tile.HasTile)
					{
						if (!TileFragile(tile))
						{
							return false;
						}
					}
					if (j == 0)
					{
						if (!TileSolidWithNoSlope(x + i, y - 1))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	/// <summary>
	/// Check collision when place the tile.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="checkAttach"></param>
	/// <returns></returns>
	public bool NoCollideInSolidGrid(int i, int j, bool checkAttach = true)
	{
		if (Main.dedServ)
		{
			return false;
		}
		TileObjectData tileObjectData = TileObjectData.GetTileData(Type, Main.LocalPlayer.HeldItem.placeStyle);
		i -= tileObjectData.Origin.X;
		j -= tileObjectData.Origin.Y;
		for (int x = 0; x < TotalWidth; x++)
		{
			for (int y = 0; y < TotalHeight; y++)
			{
				if (PixelHasTile[x, y] >= 200)
				{
					var tile = TileUtils.SafeGetTile(i + x, j + y);
					if (tile.HasTile)
					{
						if (!TileFragile(tile))
						{
							return false;
						}
					}
				}
				if (checkAttach)
				{
					if (PixelHasAttach[x, y] > 200)
					{
						if (!ValidAttachCheck(i, j, x, y))
						{
							return false;
						}
					}
				}
			}
		}
		return true;
	}

	public bool ValidAttachCheck(int i, int j, int x, int y)
	{
		if (!TileSolidWithNoSlope(i + x + 1, j + y) && PixelHasTile[x + 1, y] < 200)
		{
			return false;
		}
		if (!TileSolidWithNoSlope(i + x, j + y + 1) && PixelHasTile[x, y + 1] < 200)
		{
			return false;
		}
		if (!TileSolidWithNoSlope(i + x - 1, j + y) && PixelHasTile[x - 1, y] < 200)
		{
			return false;
		}
		if (!TileSolidWithNoSlope(i + x, j + y - 1) && PixelHasTile[x, y - 1] < 200)
		{
			return false;
		}
		return true;
	}

	public bool TileFragile(Tile tile)
	{
		bool fragile = false;
		if (Main.tileCut[tile.TileType])
		{
			fragile = true;
		}
		if (Main.tilePile[tile.TileType])
		{
			fragile = true;
		}
		return fragile;
	}

	public bool TileSolidWithNoSlope(int i, int j)
	{
		return TileSolidWithNoSlope(TileUtils.SafeGetTile(i, j));
	}

	public bool TileSolidWithNoSlope(Tile tile)
	{
		if (!tile.HasTile)
		{
			return false;
		}
		if (tile.Slope != SlopeType.Solid)
		{
			return false;
		}
		if (tile.IsHalfBlock)
		{
			return false;
		}
		if (!(Main.tileSolidTop[tile.TileType] || Main.tileSolid[tile.TileType]))
		{
			return false;
		}
		return true;
	}
}
