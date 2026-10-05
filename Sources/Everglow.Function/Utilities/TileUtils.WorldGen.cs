using Everglow.Commons.TileHelper;
using Terraria.ObjectData;
using Terraria.Utilities;

namespace Everglow.Commons.Utilities;

public partial class TileUtils
{
	public static int[,] PerlinPixelR = new int[512, 512];
	public static int[,] PerlinPixelG = new int[512, 512];
	public static int[,] PerlinPixelB = new int[512, 512];

	public static int[,] LargeSmokeTexturePixelR = new int[1024, 1024];
	public static int[,] LargeSmokeTexturePixelG = new int[1024, 1024];
	public static int[,] LargeSmokeTexturePixelB = new int[1024, 1024];

	public static UnifiedRandom GenRand = new UnifiedRandom();

	/// <summary>
	/// 0: Forceful<br/>
	/// 1: NoTile<br/>
	/// 2: HasTile<br/>
	/// 3: HasWall<br/>
	/// 4: NoWall<br/>
	/// 5: HasLiquid<br/>
	/// 6: NoLiquid<br/>
	/// 7: HasTileAndWall<br/>
	/// 8: TileButNoWallOnly<br/>
	/// 9: WallButNoTileOnly<br/>
	/// 10: SolidBlock<br/>
	/// 11: NoneSolidTile<br/>
	/// </summary>
	public enum TileChangeState
	{
		Forceful,
		NoTile,
		HasTile,
		HasWall,
		NoWall,
		HasLiquid,
		NoLiquid,
		HasTileAndWall,
		TileButNoWallOnly,
		WallButNoTileOnly,
		SolidBlock,
		NoneSolidTile,
	}

	public static bool CanChangeTile(Tile tile, int state)
	{
		switch (state)
		{
			case (int)TileChangeState.Forceful:
				return true;
			case (int)TileChangeState.NoTile:
				if (!tile.HasTile)
				{
					return true;
				}
				break;
			case (int)TileChangeState.HasTile:
				if (tile.HasTile)
				{
					return true;
				}
				break;
			case (int)TileChangeState.HasWall:
				if (tile.wall > 0)
				{
					return true;
				}
				break;
			case (int)TileChangeState.NoWall:
				if (tile.wall <= 0)
				{
					return true;
				}
				break;
			case (int)TileChangeState.HasLiquid:
				if (tile.LiquidAmount > 0)
				{
					return true;
				}
				break;
			case (int)TileChangeState.NoLiquid:
				if (tile.LiquidAmount <= 0)
				{
					return true;
				}
				break;
			case (int)TileChangeState.HasTileAndWall:
				if (tile.wall > 0 && tile.HasTile)
				{
					return true;
				}
				break;
			case (int)TileChangeState.TileButNoWallOnly:
				if (tile.wall <= 0 && tile.HasTile)
				{
					return true;
				}
				break;
			case (int)TileChangeState.WallButNoTileOnly:
				if (tile.wall > 0 && !tile.HasTile)
				{
					return true;
				}
				break;
			case (int)TileChangeState.SolidBlock:
				if (tile.HasTile && IsTileSolid(tile))
				{
					return true;
				}
				break;
			case (int)TileChangeState.NoneSolidTile:
				if (tile.HasTile && !IsTileSolid(tile))
				{
					return true;
				}
				break;
		}

		return false;
	}

	public static void TotalInitialize()
	{
		GenRand = WorldGen.genRand;
		FillPerlinPixel();
	}

	public static void FillPerlinPixel()
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>(ModAsset.WorldGen_Noise_rgb_Mod);
		Vector2 perlinCoordCenter = new Vector2(GenRand.NextFloat(0f, 1f), GenRand.NextFloat(0f, 1f));
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				int newY = (int)(accessor.Height * perlinCoordCenter.Y + y) % accessor.Height;
				var pixelRow = accessor.GetRowSpan(newY);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					int newX = (int)(accessor.Width * perlinCoordCenter.X + x) % accessor.Width;
					ref var pixel = ref pixelRow[newX];
					PerlinPixelR[x, y] = pixel.R;
					PerlinPixelG[x, y] = pixel.G;
					PerlinPixelB[x, y] = pixel.B;
				}
			}
		});

		imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>(ModAsset.WorldGenLarge_rgb_Mod);
		perlinCoordCenter = new Vector2(GenRand.NextFloat(0f, 1f), GenRand.NextFloat(0f, 1f));
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				int newY = (int)(accessor.Height * perlinCoordCenter.Y + y) % accessor.Height;
				var pixelRow = accessor.GetRowSpan(newY);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					int newX = (int)(accessor.Width * perlinCoordCenter.X + x) % accessor.Width;
					ref var pixel = ref pixelRow[newX];
					LargeSmokeTexturePixelR[x, y] = pixel.R;
					LargeSmokeTexturePixelG[x, y] = pixel.G;
					LargeSmokeTexturePixelB[x, y] = pixel.B;
				}
			}
		});
	}

	/// <summary>
	/// A float value based on the noise texture's red channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetPerlinPixelR(double x, double y)
	{
		return GetLerpTexturePixelValue(PerlinPixelR, x, y);
	}

	/// <summary>
	/// A float value based on the noise texture's green channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetPerlinPixelG(double x, double y)
	{
		return GetLerpTexturePixelValue(PerlinPixelG, x, y);
	}

	/// <summary>
	/// A float value based on the noise texture's blue channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetPerlinPixelB(double x, double y)
	{
		return GetLerpTexturePixelValue(PerlinPixelB, x, y);
	}

	/// <summary>
	/// A float value based on the noise texture's red channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetLargeSmokeTexturePixelR(double x, double y)
	{
		return GetLerpTexturePixelValue(LargeSmokeTexturePixelR, x, y);
	}

	/// <summary>
	/// A float value based on the noise texture's green channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetLargeSmokeTexturePixelG(double x, double y)
	{
		return GetLerpTexturePixelValue(LargeSmokeTexturePixelG, x, y);
	}

	/// <summary>
	/// A float value based on the noise texture's blue channel.
	/// </summary>
	/// <returns>0f~1f</returns>
	public static float GetLargeSmokeTexturePixelB(double x, double y)
	{
		return GetLerpTexturePixelValue(LargeSmokeTexturePixelB, x, y);
	}

	public static float GetLerpTexturePixelValue(int[,] colorMap, double x, double y)
	{
		return colorMap[(int)(Math.Abs(x) % colorMap.GetLength(0)), (int)(Math.Abs(y) % colorMap.GetLength(1))] / 255f;
	}

	/// <summary>
	/// Use (x, y) as the top left corner to place a frame important tile area with given width and height, and set the frameX and frameY of each tile in this area according to their position in this area.
	/// </summary>
	/// <param name="path"></param>
	public static void PlaceFrameImportantTiles(int x, int y, int width, int height, int type, int startX = 0, int startY = 0)
	{
		if (x > Main.maxTilesX - width || x < 0 || y > Main.maxTilesY - height || y < 0)
		{
			return;
		}

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Tile tile = Main.tile[x + i, y + j];
				tile.TileType = (ushort)type;
				tile.TileFrameX = (short)(i * 18 + startX);
				tile.TileFrameY = (short)(j * 18 + startY);
				tile.HasTile = true;
			}
		}
	}

	/// <summary>
	/// place frame important tiles above the area with (x, y - 1) as the BOTTOM left corner and given width and height (It means that Y = y in most case should be solid tile), and set the frameX and frameY of each tile in this area according to their position in this area.
	/// </summary>
	/// <param name="startX">TileFrameX at left side, +18 each tile towards right.</param>
	/// <param name="startY">TileFrameX at top side, +18 each tile towards down.</param>
	public static void PlaceFrameImportantTilesAbove(int x, int y, int width, int height, int type, int startX = 0, int startY = 0)
	{
		if (x > Main.maxTilesX - width || x < 0 || y > Main.maxTilesY - height || y < 0)
		{
			return;
		}

		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				Tile tile = Main.tile[x + i, y + j - height];
				tile.TileType = (ushort)type;
				tile.TileFrameX = (short)(i * 18 + startX);
				tile.TileFrameY = (short)(j * 18 + startY);
				tile.HasTile = true;
			}
		}
	}

	/// <summary>
	/// Place frame important tiles at TileObjectData.Origin.
	/// </summary>
	/// <param name="tileCoord"></param>
	/// <param name="type"></param>
	/// <param name="startX"></param>
	/// <param name="startY"></param>
	public static void PlaceFrameImportantTilesAtTileObjectDataOrigin(Point tileCoord, int type, int startX = 0, int startY = 0)
	{
		int i = tileCoord.X;
		int j = tileCoord.Y;
		TileObjectData tileObjectData = TileObjectData.GetTileData(type, Main.LocalPlayer.HeldItem.placeStyle);
		i -= tileObjectData.Origin.X;
		j -= tileObjectData.Origin.Y;
		int width = tileObjectData.Width;
		int height = tileObjectData.Height;
		for (int x = 0; x < width; x++)
		{
			for (int y = 0; y < height; y++)
			{
				var tile = SafeGetTile(x + i, y + j);
				tile.TileType = (ushort)type;
				tile.TileFrameX = (short)((x * 18) + startX);
				tile.TileFrameY = (short)((y * 18) + startY);
				tile.HasTile = true;
			}
		}
	}

	/// <returns>True when the given point can be killed safely(without chest).</returns>
	public static bool ChestSafe(int x, int y)
	{
		Tile tile = SafeGetTile(x, y);
		Tile tileUp = SafeGetTile(x, y - 1);
		return !TileID.Sets.BasicChest[tile.TileType] && !TileID.Sets.BasicChest[tileUp.TileType];
	}

	/// <param name="center">Tile coordinate, no world coordinate.</param>
	/// <returns></returns>
	public static bool ChestSafe(Vector2 center)
	{
		return ChestSafe((int)center.X, (int)center.Y);
	}

	/// <returns>True when the given tile can be killed safely(without chest).</returns>
	public static bool ChestSafe(Tile tile)
	{
		return ChestSafe(tile.X(), tile.Y());
	}

	/// <summary>
	/// Fill tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0">Left</param>
	/// <param name="y0">Top</param>
	/// <param name="x1">Right</param>
	/// <param name="y1">Bottom</param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceRectangleAreaOfBlock(int x0, int y0, int x1, int y1, int type, int force = 0)
	{
		if (x0 > x1)
		{
			(x0, x1) = (x1, x0);
		}
		if (y0 > y1)
		{
			(y0, y1) = (y1, y0);
		}
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				ChangeTile(tile, type, force);
			}
		}
	}

	/// <summary>
	/// Fill tiles by given area:(x, y, w, h) where (x, y) is the top left corner.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void PlaceRectangleAreaOfBlock_XYWH(int x, int y, int w, int h, int type, int force = 0)
	{
		PlaceRectangleAreaOfBlock(x, y, x + w - 1, y + h - 1, type, force);
	}

	/// <summary>
	/// Fill tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0">Left</param>
	/// <param name="y0">Top</param>
	/// <param name="x1">Right</param>
	/// <param name="y1">Bottom</param>
	/// <param name="type">WallID: place the tile.<br/>
	/// -1: Kill wall.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceRectangleAreaOfWall(int x0, int y0, int x1, int y1, int type, int force = 0)
	{
		if (x0 > x1)
		{
			(x0, x1) = (x1, x0);
		}
		if (y0 > y1)
		{
			(y0, y1) = (y1, y0);
		}
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				ChangeWall(tile, type, force);
			}
		}
	}

	/// <summary>
	/// Fill tiles by given area:(x, y, w, h) where (x, y) is the top left corner.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	/// <param name="type">WallID</param>
	/// <param name="force"></param>
	public static void PlaceRectangleAreaOfWall_XYWH(int x, int y, int w, int h, int type, int force = 0)
	{
		PlaceRectangleAreaOfWall(x, y, x + w - 1, y + h - 1, type, force);
	}

	/// <summary>
	/// Fill tiles by given area. The pos is the top left corner and size is the width and height. So the area is (pos.X, pos.Y) to (pos.X + size.X, pos.Y + size.Y).
	/// </summary>
	/// <param name="pos">Tile coord.</param>
	/// <param name="size"></param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceRectangleAreaOfBlock(Vector2 pos, Vector2 size, int type, int force = 0)
	{
		PlaceRectangleAreaOfBlock((int)pos.X, (int)pos.Y, (int)(size + pos).X, (int)(size + pos).Y, type, force);
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="center">Tile coord center in Vector2.</param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfBlock(Vector2 center, float radius, int type, int force = 0)
	{
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius)
				{
					ChangeTile(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="center">Tile coord center in Vector2.</param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfWall(Vector2 center, float radius, int type, int force = 0)
	{
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius)
				{
					ChangeWall(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="center">Tile coord center in Vector2.</param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfLiquid(Vector2 center, float radius, int type, int force = 0)
	{
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius)
				{
					ChangeLiquid(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfBlock(Point center, float radius, int type, int force = 0)
	{
		PlaceCircleAreaOfBlock(center.ToVector2(), radius, type, force);
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfWall(Point center, float radius, int type, int force = 0)
	{
		PlaceCircleAreaOfWall(center.ToVector2(), radius, type, force);
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate.
	/// </summary>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfLiquid(Point center, float radius, int type, int force = 0)
	{
		PlaceCircleAreaOfLiquid(center.ToVector2(), radius, type, force);
	}

	/// <summary>
	/// Get a list of tile coordinates in a circle area with random noise.
	/// </summary>
	/// <param name="pos">the center of the circle</param>
	/// <param name="radius">the radius of the circle</param>
	/// <param name="noiseSize">the max random noise that can be added to the radius</param>
	/// <returns></returns>
	public static List<Point> GetCircleAreaOfTilePosWithRandomNoise(Point pos, float radius, float noiseSize = 3f)
	{
		List<Point> tiles = new List<Point>();
		int x0CoordPerlin = GenRand.Next(512);
		int y0CoordPerlin = GenRand.Next(512);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				float aValue = GetPerlinPixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				if (new Vector2(x, y).Length() <= radius - aValue * noiseSize)
				{
					tiles.Add(new Point(x, y) + pos);
				}
			}
		}
		return tiles;
	}

	/// <summary>
	/// Get a list of tile coordinates in a circle area with random noise.
	/// </summary>
	/// <param name="pos">the center of the circle</param>
	/// <param name="radius">the radius of the circle</param>
	/// <param name="noiseSize">the max random noise that can be added to the radius</param>
	/// <returns></returns>
	public static List<Point> GetCircleAreaOfTilePos(Point pos, float radius)
	{
		List<Point> tiles = new List<Point>();
		int radiusI = (int)(radius + 1);
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				if (new Vector2(x, y).Length() <= radius)
				{
					tiles.Add(new Point(x, y) + pos);
				}
			}
		}
		return tiles;
	}

	/// <summary>
	/// Transform the tile within the circle(center, radius) to the type, but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center">Tile coord center in Vector2.</param>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfBlockWithRandomNoise(Vector2 center, float radius, int type, float noiseSize = 3f, int force = 0)
	{
		int x0CoordPerlin = GenRand.Next(512);
		int y0CoordPerlin = GenRand.Next(512);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				float aValue = GetPerlinPixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				if (new Vector2(x, y).Length() <= radius - aValue * noiseSize)
				{
					Tile tile = SafeGetTile(center + new Vector2(x, y));
					ChangeTile(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Transform the tile within the circle(center, radius) to the type, but with a random noise affect on the bound.
	/// </summary>
	/// <param name="type">TileID: place the tile.<br/>
	/// -1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCircleAreaOfBlockWithRandomNoise(Point center, float radius, int type, float noiseSize = 10f, int force = 0)
	{
		PlaceCircleAreaOfBlockWithRandomNoise(center.ToVector2(), radius, type, noiseSize, force);
	}

	/// <summary>
	/// Transform the wall within the circle(center, radius) to the type, but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center">Tile coord</param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="noiseSize"></param>
	/// <param name="force"></param>
	public static void PlaceCircleAreaOfWallWithRandomNoise(Point center, float radius, int type, float noiseSize = 10f, int force = 0)
	{
		PlaceCircleAreaOfWallWithRandomNoise(center.ToVector2(), radius, type, noiseSize, force);
	}

	/// <summary>
	/// Transform the wall within the circle(center, radius) to the type, but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center">Tile coord</param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="noiseSize"></param>
	/// <param name="force"></param>
	public static void PlaceCircleAreaOfWallWithRandomNoise(Vector2 center, float radius, int type, float noiseSize = 3f, int force = 0)
	{
		int x0CoordPerlin = GenRand.Next(512);
		int y0CoordPerlin = GenRand.Next(512);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				float aValue = GetPerlinPixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				if (new Vector2(x, y).Length() <= radius - aValue * noiseSize)
				{
					Tile tile = SafeGetTile(center + new Vector2(x, y));
					ChangeWall(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Kill the tile within the circle(center, radius), but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center">Tile coord</param>
	/// <param name="radius"></param>
	/// <param name="type_be_killed"></param>
	/// <param name="killStyle">-1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="noiseSize"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void KillCircleAreaOfBlockWithRandomNoiseInCertainTypeOfTile(Vector2 center, float radius, List<int> type_be_killed, int killStyle = -1, float noiseSize = 10f, int force = 0)
	{
		int x0CoordPerlin = GenRand.Next(512);
		int y0CoordPerlin = GenRand.Next(512);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				float aValue = GetPerlinPixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius - aValue * noiseSize && type_be_killed.Contains(tile.TileType))
				{
					ChangeTile(tile, killStyle, force);
				}
			}
		}
	}

	/// <summary>
	/// Kill the tile within the circle(center, radius), but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center">Tile coord</param>
	/// <param name="radius"></param>
	/// <param name="type_be_killed"></param>
	/// <param name="killStyle">-1: Kill tile.<br/>
	/// -2: ClearEverything</param>
	/// <param name="noiseSize"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void KillCircleAreaOfBlockWithRandomNoiseInCertainTypeOfTile(Point center, float radius, List<int> type_be_killed, int killStyle = -1, float noiseSize = 10f, int force = 0)
	{
		KillCircleAreaOfBlockWithRandomNoiseInCertainTypeOfTile(center.ToVector2(), radius, type_be_killed, killStyle, noiseSize, force);
	}

	/// <summary>
	/// Kill the wall within the circle(center, radius), but with a random noise affect on the bound.
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type_be_killed"></param>
	/// <param name="killStyle">Default to 0, only wall will be removed.</param>
	/// <param name="noiseSize"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void KillCircleAreaOfWallWithRandomNoiseInCertainType(Point center, float radius, List<int> type_be_killed, int killStyle = 0, float noiseSize = 10f, int force = 0)
	{
		int x0CoordPerlin = GenRand.Next(512);
		int y0CoordPerlin = GenRand.Next(512);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				float aValue = GetPerlinPixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				Tile tile = SafeGetTile(center + new Point(x, y));
				if (new Vector2(x, y).Length() <= radius - aValue * noiseSize && type_be_killed.Contains(tile.wall))
				{
					ChangeWall(tile, killStyle, force);
				}
			}
		}
	}

	/// <summary>
	/// Transform the tile within the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfBlock(List<Vector2> polygon, int type, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		var bounds = MathUtils.GetPolygonAABBBound_Vector4(polygon);
		for (int x = (int)bounds.X - 8; x < bounds.Z; x += 16)
		{
			for (int y = (int)bounds.Y - 8; y < bounds.W; y += 16)
			{
				if (MathUtils.IsPointInPolygon(polygon, new Vector2(x, y)))
				{
					Tile tile = SafeGetTile(new Vector2(x, y).ToTileCoordinates());
					ChangeTile(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Transform the tiles within the polygon to the type. The polygon is a list of TILE coordinates in Point, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">TILE coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfBlock(List<Point> polygon, int type, int force = 0)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		PlacePolygonAreaOfBlock(polygon_Vector2, type, force);
	}

	/// <summary>
	/// Transform the walls within the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfWall(List<Vector2> polygon, int type, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		var bounds = MathUtils.GetPolygonAABBBound_Vector4(polygon);
		for (int x = (int)bounds.X; x <= bounds.Z; x += 16)
		{
			for (int y = (int)bounds.Y; y <= bounds.W; y += 16)
			{
				if (MathUtils.IsPointInPolygon(polygon, new Vector2(x, y)))
				{
					Tile tile = SafeGetTile(new Vector2(x, y).ToTileCoordinates());
					ChangeWall(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Transform the liquid within the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfLiquid(List<Vector2> polygon, int type, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		var bounds = MathUtils.GetPolygonAABBBound_Vector4(polygon);
		for (int x = (int)bounds.X; x <= bounds.Z; x += 16)
		{
			for (int y = (int)bounds.Y; y <= bounds.W; y += 16)
			{
				if (MathUtils.IsPointInPolygon(polygon, new Vector2(x, y)))
				{
					Tile tile = SafeGetTile(new Vector2(x, y).ToTileCoordinates());
					ChangeLiquid(tile, type, force);
				}
			}
		}
	}

	/// <summary>
	/// Transform the tile within the polygon to the type. The polygon is a list of TILE coordinates in Point, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">TILE coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfWall(List<Point> polygon, int type, int force = 0)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		PlacePolygonAreaOfWall(polygon_Vector2, type, force);
	}

	/// <summary>
	/// Automatically offset the polygon by the anchorPos, then transform the tile within the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="anchorPos"></param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfBlockWithOffset(List<Vector2> polygon, Vector2 anchorPos, int type, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		List<Vector2> newPolygon = polygon;
		for (int i = 0; i < newPolygon.Count; i++)
		{
			newPolygon[i] += anchorPos;
		}
		PlacePolygonAreaOfBlock(newPolygon, type, force);
	}

	/// <summary>
	/// Automatically offset the polygon by the anchorPos, then transform the tile within the polygon to the type. The polygon is a list of TILE coordinates in Point, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">TILE coord</param>
	/// <param name="anchorPos"></param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonAreaOfBlockWithOffset(List<Point> polygon, Vector2 anchorPos, int type, int force = 0)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		PlacePolygonAreaOfBlockWithOffset(polygon_Vector2, anchorPos, type, force);
	}

	/// <summary>
	/// Transform the tiles at the edge of the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonBoundOfBlock(List<Vector2> polygon, int type, float thick, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		for (int k = 0; k < polygon.Count; k++)
		{
			int nextIndex = k + 1;
			if (nextIndex == polygon.Count)
			{
				nextIndex = 0;
			}
			PlaceLineBlock(polygon[k], polygon[nextIndex], thick, type, force);
		}
	}

	/// <summary>
	/// Transform the tiles at the edge of the polygon to the type. The polygon is a list of TILE coordinates in Point, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">TILE coord</param>
	/// <param name="type"></param>
	/// <param name="thick"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonBoundOfBlock(List<Point> polygon, int type, float thick, int force = 0)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		PlacePolygonBoundOfBlock(polygon_Vector2, type, thick, force);
	}

	/// <summary>
	/// Transform the tiles at the edge of the polygon to the type. The polygon is a list of WORLD coordinates in Vector2, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonBoundOfWall(List<Vector2> polygon, int type, float thick, int force = 0)
	{
		if (polygon.Count < 3)
		{
			return;
		}
		for (int k = 0; k < polygon.Count; k++)
		{
			int nextIndex = k + 1;
			if (nextIndex == polygon.Count)
			{
				nextIndex = 0;
			}
			PlaceLineWall(polygon[k], polygon[nextIndex], thick, type, force);
		}
	}

	/// <summary>
	/// Transform the tiles at the edge of the polygon to the type. The polygon is a list of TILE coordinates in Point, and the area is determined by the point-in-polygon test.
	/// </summary>
	/// <param name="polygon">TILE coord</param>
	/// <param name="type"></param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlacePolygonBoundOfWall(List<Point> polygon, int type, float thick, int force = 0)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		PlacePolygonBoundOfWall(polygon_Vector2, type, thick, force);
	}

	/// <summary>
	/// Transform the tiles beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">WORLD coord</param>
	/// <param name="pos1">WORLD coord</param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineBlock(Vector2 pos0, Vector2 pos1, float thick, int type, int force = 0)
	{
		Vector2 dir = pos0 - pos1;
		if (dir == Vector2.zeroVector)
		{
			return;
		}
		Vector2 normalizedDir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2);
		List<Vector2> tiltRect = new List<Vector2>();
		tiltRect.Add(pos0 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 - normalizedDir * thick * 0.5f);
		tiltRect.Add(pos0 - normalizedDir * thick * 0.5f);
		PlacePolygonAreaOfBlock(tiltRect, type, force);
	}

	/// <summary>
	/// Transform the tiles beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">TILE coord</param>
	/// <param name="pos1">TILE coord</param>
	/// <param name="thick"></param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineBlock(Point pos0, Point pos1, float thick, int type, int force = 0)
	{
		PlaceLineBlock(pos0.ToWorldCoordinates(), pos1.ToWorldCoordinates(), thick, type, force);
	}

	/// <summary>
	/// Transform the walls beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">WORLD coord</param>
	/// <param name="pos1">WORLD coord</param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineWall(Vector2 pos0, Vector2 pos1, float thick, int type, int force = 0)
	{
		Vector2 dir = pos0 - pos1;
		if (dir == Vector2.zeroVector)
		{
			return;
		}
		Vector2 normalizedDir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2);
		List<Vector2> tiltRect = new List<Vector2>();
		tiltRect.Add(pos0 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 - normalizedDir * thick * 0.5f);
		tiltRect.Add(pos0 - normalizedDir * thick * 0.5f);
		PlacePolygonAreaOfWall(tiltRect, type, force);
	}

	/// <summary>
	/// Transform the liquid beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">WORLD coord</param>
	/// <param name="pos1">WORLD coord</param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineLiquid(Vector2 pos0, Vector2 pos1, float thick, int type, int force = 0)
	{
		Vector2 dir = pos0 - pos1;
		if (dir == Vector2.zeroVector)
		{
			return;
		}
		Vector2 normalizedDir = dir.NormalizeSafe().RotatedBy(MathHelper.PiOver2);
		List<Vector2> tiltRect = new List<Vector2>();
		tiltRect.Add(pos0 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 + normalizedDir * thick * 0.5f);
		tiltRect.Add(pos1 - normalizedDir * thick * 0.5f);
		tiltRect.Add(pos0 - normalizedDir * thick * 0.5f);
		PlacePolygonAreaOfLiquid(tiltRect, type, force);
	}

	/// <summary>
	/// Transform the walls beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">TILE coord</param>
	/// <param name="pos1">TILE coord</param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineWall(Point pos0, Point pos1, float thick, int type, int force = 0)
	{
		PlaceLineWall(pos0.ToWorldCoordinates(), pos1.ToWorldCoordinates(), thick, type, force);
	}

	/// <summary>
	/// Transform the liquid beside the line(pos0, pos1) to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="pos0">TILE coord</param>
	/// <param name="pos1">TILE coord</param>
	/// <param name="thick">WORLD coord</param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceLineLiquid(Point pos0, Point pos1, float thick, int type, int force = 0)
	{
		PlaceLineLiquid(pos0.ToWorldCoordinates(), pos1.ToWorldCoordinates(), thick, type, force);
	}

	/// <summary>
	///
	/// </summary>
	/// <param name="tile"></param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void ChangeTile(Tile tile, int type, int force)
	{
		if (ChestSafe(tile) && CanChangeTile(tile, force))
		{
			if (type >= 0)
			{
				tile.TileType = (ushort)type;
				tile.HasTile = true;
			}
			else if (type == -1)
			{
				tile.HasTile = false;
			}
			else if (type == -2)
			{
				tile.ClearEverything();
			}
		}
	}

	public static void ChangeWall(Tile tile, int type, int force)
	{
		if (CanChangeTile(tile, force))
		{
			if (type >= 0)
			{
				tile.WallType = (ushort)type;
			}
			else if (type == -1)
			{
				tile.WallType = WallID.None;
			}
			else if (type == -2)
			{
				tile.ClearEverything();
			}
		}
	}

	public static void ChangeLiquid(Tile tile, int type, int force)
	{
		if (ChestSafe(tile) && CanChangeTile(tile, force))
		{
			if (type >= 0)
			{
				tile.LiquidType = (ushort)type;
			}
			else if (type == -1)
			{
				tile.HasTile = false;
			}
			else if (type == -2)
			{
				tile.ClearEverything();
			}
		}
	}

	/// <summary>
	/// Smooth tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	public static void SmoothTile_XXYY(int x0, int y0, int x1, int y1)
	{
		x0 = Math.Clamp(x0, 20, Main.maxTilesX - 20);
		x1 = Math.Clamp(x1, 20, Main.maxTilesX - 20);
		y0 = Math.Clamp(y0, 20, Main.maxTilesY - 20);
		y1 = Math.Clamp(y1, 20, Main.maxTilesY - 20);
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				if (!ChestSafe(x, y))
				{
					continue;
				}
				Tile.SmoothSlope(x, y, false, false);
				WorldGen.TileFrame(x, y, true, false);
				WorldGen.SquareWallFrame(x, y, true);
			}
		}
	}

	/// <summary>
	/// Smooth tiles by given area:(x, y, w, h)
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	public static void SmoothTile_XYWH(int x, int y, int w, int h)
	{
		SmoothTile_XXYY(x, y, x + w, y + h);
	}

	/// <summary>
	/// Smooth tiles by given tile list. Note that the tile list should be in the same area, otherwise it may cause some unexpected result.
	/// </summary>
	/// <param name="tiles"></param>
	public static void SmoothTile_List(List<Point> tiles)
	{
		foreach (var pos in tiles)
		{
			if (!ChestSafe(pos.X, pos.Y))
			{
				continue;
			}
			Tile.SmoothSlope(pos.X, pos.Y, false, false);
			WorldGen.TileFrame(pos.X, pos.Y, true, true);
			WorldGen.SquareWallFrame(pos.X, pos.Y, true);
		}
	}

	public static void SmoothTile_PolygonArea(List<Point> polygon_TilePos)
	{
		List<Vector2> polygon_WorldPos = new List<Vector2>();
		foreach (var point in polygon_TilePos)
		{
			polygon_WorldPos.Add(point.ToWorldCoordinates());
		}
		SmoothTile_PolygonArea(polygon_WorldPos);
	}

	public static void SmoothTile_PolygonArea(List<Vector2> polygon_WorldPos)
	{
		if (polygon_WorldPos.Count < 3)
		{
			return;
		}
		var bounds = MathUtils.GetPolygonAABBBound_Vector4(polygon_WorldPos);
		for (int x = (int)bounds.X; x <= bounds.Z; x += 16)
		{
			for (int y = (int)bounds.Y; y <= bounds.W; y += 16)
			{
				if (MathUtils.IsPointInPolygon(polygon_WorldPos, new Vector2(x, y)))
				{
					var point = new Vector2(x, y).ToTileCoordinates();
					Tile.SmoothSlope(point.X, point.Y, false, false);
					WorldGen.TileFrame(point.X, point.Y, true, true);
					WorldGen.SquareWallFrame(point.X, point.Y, true);
				}
			}
		}
	}

	/// <summary>
	/// Use BFS algorithm get the continue tiles from the check point.
	/// </summary>
	/// <param name="checkPoint"></param>
	/// <param name="includeWall">If true, tile with no block but only wall will also be included.</param>
	/// <param name="maxCount"></param>
	/// <param name="theseTypeOnly">Filter by tile type list, null = no filter</param>
	/// <returns></returns>
	public static List<Point> BFSContinueTile(Point checkPoint, bool includeWall = false, int maxCount = 512, List<int> theseTypeOnly = null)
	{
		int maxContinueCount = maxCount;
		(int, int)[] directions =
		{
		(0, 1),
		(1, 0),
		(0, -1),
		(-1, 0),
		};
		Queue<Point> queueChecked = new Queue<Point>();
		HashSet<Point> visitedHash = new HashSet<Point>();

		queueChecked.Enqueue(checkPoint);
		visitedHash.Add(checkPoint);

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);

				if (checkX < 20 || checkX >= Main.maxTilesX - 20 || checkY < 20 || checkY >= Main.maxTilesY - 20)
				{
					continue;
				}

				if (visitedHash.Contains(point))
				{
					continue;
				}

				Tile tile = SafeGetTile(checkX, checkY);
				bool tileValid = tile.HasTile || (includeWall && tile.WallType > WallID.None);
				if (!tileValid)
				{
					continue;
				}

				bool typeOk = true;
				if (theseTypeOnly != null)
				{
					typeOk = theseTypeOnly.Contains(tile.TileType);
				}
				if (!typeOk)
				{
					continue;
				}

				if (visitedHash.Count >= maxContinueCount)
				{
					continue;
				}

				visitedHash.Add(point);
				queueChecked.Enqueue(point);
			}
		}
		return new List<Point>(visitedHash);
	}

	public static List<Point> BFSContinueWall(Point checkPoint, int maxCount = 512, List<int> theseTypeOnly = default)
	{
		int maxContinueCount = maxCount;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),
		};
		Queue<Point> queueChecked = new Queue<Point>();

		// Add first point to the queue.
		queueChecked.Enqueue(checkPoint);
		List<Point> visited = new List<Point>();

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);
				Tile tile = SafeGetTile(checkX, checkY);

				// Check bound and obstruction.
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					(tile.WallType > WallID.None || theseTypeOnly.Contains(WallID.None)) && !visited.Contains(point))
				{
					if (theseTypeOnly == default)
					{
						queueChecked.Enqueue(point);
						visited.Add(point);
					}
					else if (theseTypeOnly.Contains(tile.WallType))
					{
						queueChecked.Enqueue(point);
						visited.Add(point);
					}
				}
			}
			if (queueChecked.Count > maxContinueCount || visited.Count > maxContinueCount)
			{
				break;
			}
		}
		return visited;
	}

	/// <summary>
	/// Use BFS algorithm get the continue tiles from the check point.
	/// </summary>
	/// <param name="checkPoint"></param>
	/// <param name="includeWall">If true, tile is empty but has wall will NOT count as empty.</param>
	/// <param name="maxCount"></param>
	/// <returns></returns>
	public static List<Point> BFSContinueEmpty(Point checkPoint, bool includeWall = false, int maxCount = 512, List<int> ignoreTheseType = default)
	{
		int maxContinueCount = maxCount;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),
		};
		Queue<Point> queueChecked = new Queue<Point>();

		// Add first point to the queue.
		queueChecked.Enqueue(checkPoint);
		List<Point> visited = new List<Point>();

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);
				Tile tile = SafeGetTile(checkX, checkY);

				// Check bound and obstruction.
				bool flag0 = !tile.HasTile && (!includeWall || tile.WallType <= WallID.None);
				bool flag1 = tile.HasTile && ignoreTheseType != default && ignoreTheseType.Contains(tile.TileType);
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					(flag0 || flag1) && !visited.Contains(point))
				{
					queueChecked.Enqueue(point);
					visited.Add(point);
				}
			}
			if (queueChecked.Count > maxContinueCount || visited.Count > maxContinueCount)
			{
				break;
			}
		}
		return visited;
	}

	public static List<Point> BFSSurface(Point checkPoint, int maxCount = 512, bool ignoreFrameImportant = true, List<int> theseTypeOnly = default)
	{
		int maxContinueCount = maxCount;
		(int, int)[] directions =
		{
			(0, 1),
			(1, 0),
			(0, -1),
			(-1, 0),

			// (1, 1),
			// (1, -1),
			// (-1, -1),
			// (-1, 1),
		};
		Queue<Point> queueChecked = new Queue<Point>();

		// Add first point to the queue.
		queueChecked.Enqueue(checkPoint);
		List<Point> visited_air = new List<Point>();
		List<Point> visited_tile = new List<Point>();
		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directions)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);
				Tile tile = SafeGetTile(checkX, checkY);
				Tile tile_up = SafeGetTile(checkX, checkY - 1);
				Tile tile_bottom = SafeGetTile(checkX, checkY + 1);
				Tile tile_left = SafeGetTile(checkX - 1, checkY);
				Tile tile_right = SafeGetTile(checkX + 1, checkY);

				// Tile tile_upleft = SafeGetTile(checkX - 1, checkY - 1);
				// Tile tile_bottomleft = SafeGetTile(checkX - 1, checkY + 1);
				// Tile tile_upright = SafeGetTile(checkX + 1, checkY - 1);
				// Tile tile_bottomright = SafeGetTile(checkX + 1, checkY - 1);

				// bool flag0 = !tile_upleft.HasTile || !tile_upright.HasTile || !tile_bottomleft.HasTile || !tile_bottomright.HasTile;
				// Check bound and obstruction.
				bool flag0 = checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20;
				bool flag1 = tile.HasTile && (visited_air.Contains(new Point(checkX, checkY - 1)) || visited_air.Contains(new Point(checkX, checkY + 1)) || visited_air.Contains(new Point(checkX - 1, checkY)) || visited_air.Contains(new Point(checkX + 1, checkY)));
				bool flag2 = !tile.HasTile && (tile_up.HasTile || tile_bottom.HasTile || tile_left.HasTile || tile_right.HasTile);
				bool flag3 = !ignoreFrameImportant || !Main.tileFrameImportant[tile.TileType];
				if (flag0 && (flag1 || flag2) && flag3 && !visited_air.Contains(point) && !visited_tile.Contains(point))
				{
					if (theseTypeOnly == default)
					{
						queueChecked.Enqueue(point);
						if (flag1)
						{
							visited_tile.Add(point);
						}
						if (flag2)
						{
							visited_air.Add(point);
						}
					}
					else if (theseTypeOnly.Contains(tile.TileType))
					{
						queueChecked.Enqueue(point);
						if (flag1)
						{
							visited_tile.Add(point);
						}
						if (flag2)
						{
							visited_air.Add(point);
						}
					}
				}
			}
			if (queueChecked.Count > maxContinueCount || visited_tile.Count + visited_air.Count > maxContinueCount)
			{
				break;
			}
		}
		return visited_tile;
	}

	public static List<Point> GenerateRandomSeeds(int xBoundLeft, int xBoundRight, int yBoundTop, int yBoundBottom, int expect_Count, float minDistance = 20)
	{
		List<Point> result = new List<Point>();
		HashSet<Point> used = new HashSet<Point>();

		int width = xBoundRight - xBoundLeft;
		int height = yBoundBottom - yBoundTop;
		float cellSize = minDistance / (float)Math.Sqrt(2);
		int gridW = (int)Math.Ceiling(width / cellSize) + 2;
		int gridH = (int)Math.Ceiling(height / cellSize) + 2;

		Point?[,] grid = new Point?[gridW, gridH];
		List<Point> active = new List<Point>();

		int startX = (xBoundLeft + xBoundRight) / 2;
		int startY = (yBoundTop + yBoundBottom) / 2;
		result.Add(new Point(startX, startY));
		used.Add(new Point(startX, startY));
		active.Add(new Point(startX, startY));
		SetGrid(grid, cellSize, xBoundLeft, yBoundTop, startX, startY);

		while (active.Count > 0 && result.Count < expect_Count)
		{
			int idx = Main.rand.Next(active.Count);
			var current = active[idx];
			bool found = false;

			for (int i = 0; i < 8; i++)
			{
				float angle = Main.rand.NextFloat(0f, MathHelper.TwoPi);
				float dist = Main.rand.NextFloat(minDistance, minDistance * 2f);

				int newX = current.X + (int)Math.Round(Math.Cos(angle) * dist);
				int newY = current.Y + (int)Math.Round(Math.Sin(angle) * dist);

				if (newX < xBoundLeft || newX > xBoundRight ||
					newY < yBoundTop || newY > yBoundBottom)
				{
					continue;
				}

				int gx = (int)((newX - xBoundLeft) / cellSize);
				int gy = (int)((newY - yBoundTop) / cellSize);

				bool tooClose = false;

				for (int cx = Math.Max(0, gx - 1); cx <= Math.Min(gridW - 1, gx + 1); cx++)
				{
					for (int cy = Math.Max(0, gy - 1); cy <= Math.Min(gridH - 1, gy + 1); cy++)
					{
						var pt = grid[cx, cy];
						if (pt.HasValue)
						{
							float d = Vector2.Distance(
								new Vector2(newX, newY),
								new Vector2(pt.Value.X, pt.Value.Y));
							if (d < minDistance)
							{
								tooClose = true;
								break;
							}
						}
					}
					if (tooClose)
					{
						break;
					}
				}

				if (!tooClose && !used.Contains(new Point(newX, newY)))
				{
					result.Add(new Point(newX, newY));
					used.Add(new Point(newX, newY));
					active.Add(new Point(newX, newY));
					SetGrid(grid, cellSize, xBoundLeft, yBoundTop, newX, newY);
					found = true;
					break;
				}
			}

			if (!found)
			{
				active.RemoveAt(idx);
			}
		}

		return result;
	}

	private static void SetGrid(Point?[,] grid, float cellSize, int ox, int oy, int x, int y)
	{
		int gx = (int)((x - ox) / cellSize);
		int gy = (int)((y - oy) / cellSize);
		grid[gx, gy] = new Point(x, y);
	}

	public static bool TileAtOriginPos(Tile tile)
	{
		int style = TileObjectData.GetTileStyle(tile);
		TileObjectData tileObjectData = TileObjectData.GetTileData(tile.TileType, style);
		return tile.TileFrameX - style * tileObjectData.Width * 18 == tileObjectData.Origin.X * 18 && tile.TileFrameY == tileObjectData.Origin.Y * 18;
	}

	public static void PlaceWallAround(Tile tile, int wallType, bool middle = true, bool top = true, bool bottom = true, bool left = true, bool right = true)
	{
		PlaceWallAround(tile.X(), tile.Y(), wallType, middle, top, bottom, left, right);
	}

	public static void PlaceWallAround(Point pos, int wallType, bool middle = true, bool top = true, bool bottom = true, bool left = true, bool right = true)
	{
		PlaceWallAround(pos.X, pos.Y, wallType, middle, top, bottom, left, right);
	}

	public static void PlaceWallAround(int x, int y, int wallType, bool middle = true, bool top = true, bool bottom = true, bool left = true, bool right = true)
	{
		Tile tile = SafeGetTile(x, y);
		if (middle)
		{
			tile.wall = (ushort)wallType;
		}
		Tile tile_top = SafeGetTile(x, y - 1);
		if (top)
		{
			tile_top.wall = (ushort)wallType;
		}
		Tile tile_bottom = SafeGetTile(x, y + 1);
		if (bottom)
		{
			tile_bottom.wall = (ushort)wallType;
		}
		Tile tile_left = SafeGetTile(x - 1, y);
		if (left)
		{
			tile_left.wall = (ushort)wallType;
		}
		Tile tile_right = SafeGetTile(x + 1, y);
		if (right)
		{
			tile_right.wall = (ushort)wallType;
		}
	}

	public static void PlaceTileAround(int x, int y, int tileType, bool middle = true, bool top = true, bool bottom = true, bool left = true, bool right = true, int force = 0)
	{
		Tile tile = SafeGetTile(x, y);
		if (middle)
		{
			ChangeTile(tile, tileType, force);
		}
		Tile tile_top = SafeGetTile(x, y - 1);
		if (top)
		{
			ChangeTile(tile_top, tileType, force);
		}
		Tile tile_bottom = SafeGetTile(x, y + 1);
		if (bottom)
		{
			ChangeTile(tile_bottom, tileType, force);
		}
		Tile tile_left = SafeGetTile(x - 1, y);
		if (left)
		{
			ChangeTile(tile_left, tileType, force);
		}
		Tile tile_right = SafeGetTile(x + 1, y);
		if (right)
		{
			ChangeTile(tile_right, tileType, force);
		}
	}

	private static readonly (int, int)[] directionsLiquid =
	{
		(1, 0),
		(0, 1),
		(-1, 0),
	};

	/// <summary>
	/// Fill water or other liquid below pos. pos : in World coord.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="type"></param>
	/// <param name="maxCount"></param>
	public static void FillLiquid(Point pos, int type = 0, int maxCount = 900)
	{
		Queue<Point> queueChecked = new Queue<Point>();

		// 将起始点加入队列
		queueChecked.Enqueue(pos);
		List<Point> visited = new List<Point>();

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directionsLiquid)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Point point = new Point(checkX, checkY);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					!Collision.IsWorldPointSolid(point.ToWorldCoordinates()) && !visited.Contains(point))
				{
					queueChecked.Enqueue(point);
					visited.Add(point);
				}
			}
			if (queueChecked.Count > maxCount || visited.Count > maxCount)
			{
				break;
			}
		}
		if (visited.Count < maxCount)
		{
			foreach (var checked_pos in visited)
			{
				Tile tile = SafeGetTile(checked_pos);
				tile.LiquidType = (byte)type;
				tile.LiquidAmount = 255;
			}
		}
	}

	/// <summary>
	/// Fill water or other liquid below center.center : in World coord.
	/// </summary>
	/// <param name="center">World coord</param>
	/// <param name="type"></param>
	public static void FillLiquid(Vector2 center, int type = 0, int maxCount = 900)
	{
		FillLiquid(center.ToTileCoordinates(), type, maxCount);
	}

	/// <summary>
	/// Fill water or other liquid below pos.
	/// </summary>
	/// <param name="i"></param>
	/// <param name="j"></param>
	/// <param name="type"></param>
	/// <param name="maxCount"></param>
	public static void FillLiquid(int i, int j, int type = 0, int maxCount = 900)
	{
		FillLiquid(new Point(i, j), type, maxCount);
	}

	/// <summary>
	/// Fill water or other liquid below center.center : in tile coord.
	/// </summary>
	/// <param name="center">Tile coord</param>
	public static List<Point> BFSGetCanFillLiquidTiles(Vector2 center, int maxCount = 900)
	{
		return BFSGetCanFillLiquidTiles(center.ToTileCoordinates(), maxCount);
	}

	/// <summary>
	/// Fill water or other liquid below pos. pos : in tile coord.
	/// </summary>
	/// <param name="pos">Tile coord</param>
	public static List<Point> BFSGetCanFillLiquidTiles(Point pos, int maxCount = 900)
	{
		Queue<Point> queueChecked = new Queue<Point>();

		// 将起始点加入队列
		queueChecked.Enqueue(pos);
		List<Point> visited = new List<Point>();

		while (queueChecked.Count > 0)
		{
			var tilePos = queueChecked.Dequeue();

			foreach (var (dx, dy) in directionsLiquid)
			{
				int checkX = tilePos.X + dx;
				int checkY = tilePos.Y + dy;
				Tile Tile = SafeGetTile(checkX, checkY);
				Point point = new Point(checkX, checkY);

				// 检查边界和障碍物
				if (checkX >= 20 && checkX < Main.maxTilesX - 20 && checkY >= 20 && checkY < Main.maxTilesY - 20 &&
					!Collision.IsWorldPointSolid(new Point(checkX, checkY).ToWorldCoordinates()) && !visited.Contains(point))
				{
					queueChecked.Enqueue(new Point(checkX, checkY));
					visited.Add(point);
				}
			}
			if (queueChecked.Count > maxCount || visited.Count > maxCount)
			{
				break;
			}
		}
		return visited;
	}

	public static List<Point> BFSGetCanFillLiquidTiles(int i, int j, int maxCount = 900)
	{
		return BFSGetCanFillLiquidTiles(new Point(i, j), maxCount);
	}

	/// <summary>
	/// Get the nearest tile of the type. type : tile type, -1 : empty tile, -2 : empty tile without wall. maxDistance : the max distance to check. return : the offset to the nearest tile of the type. if no tile of the type is found within maxDistance, return new Vector2(maxDistance + 1).
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="type"></param>
	/// <param name="maxDistance"></param>
	/// <returns></returns>
	public static Vector2 ToNearestTypeOfTile(int x, int y, int type, float maxDistance = 100)
	{
		Vector2 direction = new Vector2(maxDistance + 1);
		float minDis = maxDistance;
		for (int i = -(int)maxDistance; i <= maxDistance; i++)
		{
			for (int j = -(int)maxDistance; j <= maxDistance; j++)
			{
				Tile tile = SafeGetTile(i + x, j + y);
				bool flag0 = tile.HasTile && tile.TileType == type;
				bool flag1 = type == -1 && !tile.HasTile;
				bool flag2 = type == -2 && !tile.HasTile && tile.wall <= 0;
				if (flag0 || flag1 || flag2)
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						minDis = v1.Length();
						direction = v1;
					}
				}
			}
		}
		return direction * 16;
	}

	/// <summary>
	/// Get the nearest tile of the type. type : tile type, -1 : empty tile, -2 : empty tile without wall. maxDistance : the max distance to check. return : the offset to the nearest tile of the type. if no tile of the type is found within maxDistance, return Vector2.zeroVector.
	/// </summary>
	/// <param name="worldPos"></param>
	/// <param name="type"></param>
	/// <param name="maxDistance"></param>
	/// <returns></returns>
	public static Vector2 ToNearestTypeOfTile(Vector2 worldPos, int type, float maxDistance = 100)
	{
		int x = worldPos.ToTileCoordinates().X;
		int y = worldPos.ToTileCoordinates().Y;
		return ToNearestTypeOfTile(x, y, type, maxDistance);
	}

	/// <summary>
	/// Return the summary of air-to-tile distances of given point to top and to bottom.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceHeight(int x, int y)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (x0 > Main.maxTilesX || x0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile)
		{
			if (y0 > Main.maxTilesY)
			{
				break;
			}
			y0++;
			count++;
		}
		x0 = x;
		y0 = y - 1;
		while (!SafeGetTile(x0, y0).HasTile)
		{
			if (y0 < 0)
			{
				break;
			}
			y0--;
			count++;
		}
		return count;
	}

	/// <summary>
	/// Return the summary of air-to-tile distances of given point to left and to right.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceWidth(int x, int y)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (y0 > Main.maxTilesY || y0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile)
		{
			if (x0 > Main.maxTilesX)
			{
				break;
			}
			x0++;
			count++;
		}
		x0 = x - 1;
		y0 = y;
		while (!SafeGetTile(x0, y0).HasTile)
		{
			if (x0 < 0)
			{
				break;
			}
			x0--;
			count++;
		}
		return count;
	}

	/// <summary>
	/// Return the air-to-tile distance from given point to LEFT.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceLeft(int x, int y, bool ignore_none_solid = false)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (y0 > Main.maxTilesY || y0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile || (!IsTileSolid(x0, y0) && ignore_none_solid))
		{
			if (x0 < 0)
			{
				break;
			}
			x0--;
			count++;
		}
		return count;
	}

	/// <summary>
	/// Return the air-to-tile distance from given point to RIGHT.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceRight(int x, int y, bool ignore_none_solid = false)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (y0 > Main.maxTilesY || y0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile || (!IsTileSolid(x0, y0) && ignore_none_solid))
		{
			if (x0 > Main.maxTilesX)
			{
				break;
			}
			x0++;
			count++;
		}
		return count;
	}

	/// <summary>
	/// Return the air-to-tile distance from given point to TOP.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceUp(int x, int y, bool ignore_none_solid = false)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (x0 > Main.maxTilesX || x0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile || (!IsTileSolid(x0, y0) && ignore_none_solid))
		{
			if (y0 < 0)
			{
				break;
			}
			y0--;
			count++;
		}
		return count;
	}

	/// <summary>
	/// Return the air-to-tile distance from given point to BOTTOM.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static int CheckSpaceDown(int x, int y, bool ignore_none_solid = false)
	{
		int count = 0;
		int x0 = x;
		int y0 = y;
		if (y0 > Main.maxTilesY || y0 < 0)
		{
			return count;
		}
		while (!SafeGetTile(x0, y0).HasTile || (!IsTileSolid(x0, y0) && ignore_none_solid))
		{
			if (y0 > Main.maxTilesY)
			{
				break;
			}
			y0++;
			count++;
		}
		return count;
	}

	public static int CheckSpaceHeight(Point pos)
	{
		return CheckSpaceHeight(pos.X, pos.Y);
	}

	public static int CheckSpaceWidth(Point pos)
	{
		return CheckSpaceWidth(pos.X, pos.Y);
	}

	public static int CheckSpaceLeft(Point pos, bool ignore_none_solid = false)
	{
		return CheckSpaceLeft(pos.X, pos.Y, ignore_none_solid);
	}

	public static int CheckSpaceRight(Point pos, bool ignore_none_solid = false)
	{
		return CheckSpaceRight(pos.X, pos.Y, ignore_none_solid);
	}

	public static int CheckSpaceUp(Point pos, bool ignore_none_solid = false)
	{
		return CheckSpaceUp(pos.X, pos.Y, ignore_none_solid);
	}

	public static int CheckSpaceDown(Point pos, bool ignore_none_solid = false)
	{
		return CheckSpaceDown(pos.X, pos.Y, ignore_none_solid);
	}

	/// <summary>
	/// Return the tile type if all tiles in the given area are the same, otherwise return -3.No any tile, return -1. If the area is out of world, return -4.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	/// <returns></returns>
	public static int GetUniformTile(int x, int y, int w, int h)
	{
		if (x < 0 || y < 0 || x + w > Main.maxTilesX || y + h > Main.maxTilesY)
		{
			return -4;
		}

		Tile tile = SafeGetTile(x, y);
		int targetId = tile.TileType;
		if (!tile.HasTile)
		{
			targetId = -1;
		}
		for (int px = x; px < x + w; px++)
		{
			for (int py = y; py < y + h; py++)
			{
				Tile checkTile = SafeGetTile(px, py);
				int checkType = checkTile.TileType;
				if (!checkTile.HasTile)
				{
					checkType = -1;
				}
				if (checkType != targetId)
				{
					return -3;
				}
			}
		}

		return targetId;
	}

	/// <summary>
	/// Return the tile type if all tiles in the given area are the same, otherwise return -3.No any tile, return -1. If the area is out of world, return -4.
	/// </summary>
	/// <param name="point"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	/// <returns></returns>
	public static int GetUniformTile(Point point, int w, int h)
	{
		return GetUniformTile(point.X, point.Y, w, h);
	}

	/// <summary>
	/// Fill liquid in the given area. Type: vanilla Liquid.ID.
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="w"></param>
	/// <param name="h"></param>
	/// <param name="type"></param>
	public static void PlaceRectangleAreaOfLiquid_XYWH(int x, int y, int w, int h, int type)
	{
		for (int i = x; i <= x + w; i++)
		{
			for (int j = y; j <= y + h; j++)
			{
				Tile tile = SafeGetTile(i, j);
				if (ChestSafe(x, y))
				{
					tile.liquid = (byte)type;
					tile.LiquidAmount = 255;
				}
			}
		}
	}

	/// <summary>
	///  Fill all chest by given area if exist.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <param name="contents"></param>
	public static void FillChestXYWH(int x, int y, int width, int height, List<Item> contents)
	{
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				WorldGenMisc.TryFillChest(x + i, y + j, contents);
			}
		}
	}

	/// <summary>
	/// Tile coord. Ignore platforms.
	/// </summary>
	/// <param name="point"></param>
	/// <returns></returns>
	public static bool IsTileSolid(Point point)
	{
		if (!WorldGen.InWorld(point.X, point.Y, 1))
		{
			return false;
		}
		Tile tile = Main.tile[point.X, point.Y];
		return IsTileSolid(tile);
	}

	public static bool IsTileSolid(int x, int y)
	{
		return IsTileSolid(new Point(x, y));
	}

	public static bool IsTileSolid(Tile tile)
	{
		return tile == null || !tile.HasTile || tile.IsActuated || !Main.tileSolid[tile.TileType]
			? false
			: tile.TileType <= TileID.Dirt || (!TileID.Sets.Platforms[tile.TileType] && tile.TileType != TileID.PlanterBox);
	}

	/// <summary>
	/// Get the tile positions in the given polygon area. World coord.
	/// </summary>
	/// <param name="polygon"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	/// <returns></returns>
	public static List<Point> GetPolygonAreaOfTilePos(List<Vector2> polygon)
	{
		List<Point> polygon_Pos = new List<Point>();
		if (polygon.Count < 3)
		{
			return polygon_Pos;
		}
		var bounds = MathUtils.GetPolygonAABBBound_Vector4(polygon);
		bounds.X = bounds.X - bounds.X % 16 - 8;
		bounds.Y = bounds.Y - bounds.Y % 16 - 8;
		bounds.Z = bounds.Z - bounds.Z % 16 + 8;
		bounds.W = bounds.W - bounds.W % 16 + 8;
		for (int x = (int)bounds.X; x <= bounds.Z; x += 16)
		{
			for (int y = (int)bounds.Y; y <= bounds.W; y += 16)
			{
				if (MathUtils.IsPointInPolygon(polygon, new Vector2(x, y)))
				{
					polygon_Pos.Add(new Vector2(x, y).ToTileCoordinates());
				}
			}
		}
		return polygon_Pos;
	}

	/// <summary>
	/// Get the tile positions in the given polygon area. Tile coord.
	/// </summary>
	/// <param name="polygon"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	/// <returns></returns>
	public static List<Point> GetPolygonAreaOfTilePos(List<Point> polygon)
	{
		List<Vector2> polygon_Vector2 = new List<Vector2>();
		foreach (var point in polygon)
		{
			polygon_Vector2.Add(point.ToWorldCoordinates());
		}
		return GetPolygonAreaOfTilePos(polygon_Vector2);
	}

	/// <summary>
	/// Get the tile positions in the given rectangle area. World coord.
	/// </summary>
	/// <param name="polygon"></param>
	/// <returns></returns>
	public static List<Point> GetAABBAreaOfTile(Vector2 worldPos, Vector2 size)
	{
		List<Point> tilePoses = new List<Point>();
		for (int x = worldPos.ToTileCoordinates().X; x <= (worldPos + size).ToTileCoordinates().X; x++)
		{
			for (int y = worldPos.ToTileCoordinates().Y; y <= (worldPos + size).ToTileCoordinates().Y; y++)
			{
				tilePoses.Add(new Point(x, y));
			}
		}
		return tilePoses;
	}

	/// <summary>
	/// Get the tile positions in the given rectangle area. Tile coord.
	/// </summary>
	/// <param name="polygon"></param>
	/// <returns></returns>
	public static List<Point> GetAABBAreaOfTile(int x, int y, int w, int h)
	{
		int minX = Math.Min(x, x + w);
		int minY = Math.Min(y, y + h);
		int maxX = Math.Max(x, x + w);
		int maxY = Math.Max(y, y + h);
		List<Point> tilePoses = new List<Point>();
		for (int i = minX; i <= maxX; i++)
		{
			for (int j = minY; j <= maxY; j++)
			{
				tilePoses.Add(new Point(i, j));
			}
		}
		return tilePoses;
	}

	/// <summary>
	/// Get the tile positions in the given rectangle area. Tile coord.
	/// </summary>
	/// <param name="polygon"></param>
	/// <returns></returns>
	public static List<Point> GetAABBAreaOfTile(Point start, Point end)
	{
		int minX = Math.Min(start.X, end.X);
		int minY = Math.Min(start.Y, end.Y);
		int maxX = Math.Max(start.X, end.X);
		int maxY = Math.Max(start.Y, end.Y);
		List<Point> tilePoses = new List<Point>();
		for (int x = minX; x <= maxX; x++)
		{
			for (int y = minY; y <= maxY; y++)
			{
				tilePoses.Add(new Point(x, y));
			}
		}
		return tilePoses;
	}

	public static void PlaceTileListTowardDownUntilCollide(Point pos, int type, int max = 1024)
	{
		for (int t = 0; t < max; t++)
		{
			var tile = SafeGetTile(pos + new Point(0, t));
			if (!tile.HasTile || !Main.tileSolid[tile.TileType])
			{
				tile.TileType = (ushort)type;
				tile.HasTile = true;
			}
			else
			{
				break;
			}
		}
	}

	public static void PlaceTileListTowardUpUntilCollide(Point pos, int type, int max = 1024)
	{
		for (int t = 0; t < max; t++)
		{
			var tile = SafeGetTile(pos + new Point(0, -t));
			if (!tile.HasTile)
			{
				tile.TileType = (ushort)type;
				tile.HasTile = true;
			}
			else
			{
				break;
			}
		}
	}

	public static void PlaceTileListTowardLeftUntilCollide(Point pos, int type, int max = 1024)
	{
		for (int t = 0; t < max; t++)
		{
			var tile = SafeGetTile(pos + new Point(-t, 0));
			if (!tile.HasTile)
			{
				tile.TileType = (ushort)type;
				tile.HasTile = true;
			}
			else
			{
				break;
			}
		}
	}

	public static void PlaceTileListTowardRightUntilCollide(Point pos, int type, int max = 1024)
	{
		for (int t = 0; t < max; t++)
		{
			var tile = SafeGetTile(pos + new Point(t, 0));
			if (!tile.HasTile)
			{
				tile.TileType = (ushort)type;
				tile.HasTile = true;
			}
			else
			{
				break;
			}
		}
	}

	public static Point PointLerp(Point pos0, Point pos1, float value)
	{
		return new Point((int)(pos0.X + (pos1.X - pos0.X) * value), (int)(pos0.Y + (pos1.Y - pos0.Y) * value));
	}

	/// <summary>
	/// Get the centroid of the given points.
	/// </summary>
	/// <param name="points"></param>
	/// <returns></returns>
	public static Point GetCentroid(List<Point> points)
	{
		if (points.Count <= 0)
		{
			return default;
		}
		Point point = new Point(0, 0);
		foreach (var p in points)
		{
			point += p;
		}
		return new Point(point.X / points.Count, point.Y / points.Count);
	}

	/// <summary>
	/// Find the nearest tile position with tile within 100 range. Return default if not found.
	/// </summary>
	/// <param name="pos"></param>
	/// <param name="force"><see cref="TileChangeState"/>, Default to 10, SolidTile</param>
	/// <returns></returns>
	public static Point FindNearestTileWithin100Range(Point pos, int force = 10)
	{
		float minDis = 100;
		Point point = default;
		for (int i = -100; i <= 100; i++)
		{
			for (int j = -100; j <= 100; j++)
			{
				Tile tile = SafeGetTile(i + pos.X, j + pos.Y);
				if (CanChangeTile(tile, force))
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						minDis = v1.Length();
						point = new Point(i + pos.X, j + pos.Y);
					}
				}
			}
		}
		return point;
	}

	public static float PointDistance(Point p0, Point p1)
	{
		return Vector2.Distance(p0.ToVector2(), p1.ToVector2());
	}

	public static void ConnectWire(Point point0, Point point1, bool red = true, bool green = false, bool blue = false, bool yellow = false)
	{
		int distanceX = Math.Abs((point1 - point0).X);
		int distanceY = Math.Abs((point1 - point0).Y);
		Point check = point0;
		int dirX = 1;
		int dirY = 1;
		if (point1.X < point0.X)
		{
			dirX = -1;
		}
		if (point1.Y < point0.Y)
		{
			dirY = -1;
		}
		for (int y = 0; y < distanceY; y++)
		{
			Tile tile = SafeGetTile(check);
			tile.RedWire = red;
			tile.GreenWire = green;
			tile.BlueWire = blue;
			tile.YellowWire = yellow;
			check.Y += dirY;
		}
		for (int x = 0; x <= distanceX; x++)
		{
			Tile tile = SafeGetTile(check);
			tile.RedWire = red;
			tile.GreenWire = green;
			tile.BlueWire = blue;
			tile.YellowWire = yellow;
			check.X += dirX;
		}
	}

	/// <summary>
	/// Transform the tiles beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">Tile Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveBlock(List<Point> curve, float thick, int type, int force = 0)
	{
		for (int k = 0; k < curve.Count - 1; k++)
		{
			PlaceLineBlock(curve[k], curve[k + 1], thick, type, force);
			if (k >= 1)
			{
				PlaceCircleAreaOfBlock(curve[k], thick / 32, type, force);
			}
		}
	}

	/// <summary>
	/// Transform the tiles beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">Tile Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveWall(List<Point> curve, float thick, int type, int force = 0)
	{
		for (int k = 0; k < curve.Count - 1; k++)
		{
			PlaceLineWall(curve[k], curve[k + 1], thick, type, force);
			if (k >= 1)
			{
				PlaceCircleAreaOfWall(curve[k], thick / 32, type, force);
			}
		}
	}

	/// <summary>
	/// Transform the liquid beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">Tile Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveLiquid(List<Point> curve, float thick, int type, int force = 0)
	{
		for (int k = 0; k < curve.Count - 1; k++)
		{
			PlaceLineLiquid(curve[k], curve[k + 1], thick, type, force);
			if (k >= 1)
			{
				PlaceCircleAreaOfLiquid(curve[k], thick / 32, type, force);
			}
		}
	}

	/// <summary>
	/// Transform the tiles beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">World Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveBlock(List<Vector2> curve, float thick, int type, int force = 0)
	{
		List<Point> curve_Point = new List<Point>();
		foreach (var pos in curve)
		{
			curve_Point.Add(pos.ToTileCoordinates());
		}
		PlaceCurveBlock(curve_Point, thick, type, force);
	}

	/// <summary>
	/// Transform the tiles beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">World Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveWall(List<Vector2> curve, float thick, int type, int force = 0)
	{
		List<Point> curve_Point = new List<Point>();
		foreach (var pos in curve)
		{
			curve_Point.Add(pos.ToTileCoordinates());
		}
		PlaceCurveWall(curve_Point, thick, type, force);
	}

	/// <summary>
	/// Transform the liquid beside the curve to the type. Notice that thick is in World Coord, 16 for a tile size.
	/// </summary>
	/// <param name="curve">World Coord</param>
	/// <param name="thick">World Coord</param>
	/// <param name="type"></param>
	/// <param name="force"><see cref="TileChangeState"/></param>
	public static void PlaceCurveLiquid(List<Vector2> curve, float thick, int type, int force = 0)
	{
		List<Point> curve_Point = new List<Point>();
		foreach (var pos in curve)
		{
			curve_Point.Add(pos.ToTileCoordinates());
		}
		PlaceCurveLiquid(curve_Point, thick, type, force);
	}

	/// <summary>
	/// Place a prefabricated MapIO anchored top left at (x, y).
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="path"></param>
	public static void BuildMapIO(int x, int y, string path, bool saveOldWall = false)
	{
		var mapIO = new MapIO(x, y);
		int width = 0;
		int height = 0;
		ushort[,] wallType = null;
		if (saveOldWall)
		{
			width = mapIO.ReadWidth(ModIns.Mod.GetFileStream(path));
			height = mapIO.ReadHeight(ModIns.Mod.GetFileStream(path));
			wallType = new ushort[width, height];
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					wallType[i, j] = SafeGetTile(i + x, j + y).WallType;
				}
			}
		}
		mapIO.Read(ModIns.Mod.GetFileStream(path));
		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
		if (saveOldWall)
		{
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < height; j++)
				{
					var tile = SafeGetTile(i + x, j + y);
					if (tile.WallType == WallID.None)
					{
						tile.WallType = wallType[i, j];
					}
				}
			}
		}
	}

	public static void BuildMapIO(Point point, string path)
	{
		BuildMapIO(point.X, point.Y, path);
	}
}
