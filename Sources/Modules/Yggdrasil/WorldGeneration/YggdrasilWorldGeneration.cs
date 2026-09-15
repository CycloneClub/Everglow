using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.CorruptWormHive.Tiles;
using Everglow.Yggdrasil.HurricaneMaze.Tiles;
using Everglow.Yggdrasil.KelpCurtain.Tiles;
using Everglow.Yggdrasil.KelpCurtain.Walls;
using Everglow.Yggdrasil.YggdrasilTown.Liquids;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Walls;
using ModLiquidLib.ModLoader;
using ReLogic.Utilities;
using Terraria.IO;
using Terraria.WorldBuilding;
using static Everglow.Commons.Utilities.TileUtils;
using static Everglow.Yggdrasil.WorldGeneration.KelpCurtainGeneration;
using static Everglow.Yggdrasil.WorldGeneration.YggdrasilTownGeneration;

namespace Everglow.Yggdrasil.WorldGeneration;

public class YggdrasilWorldGeneration : ModSystem
{
	public class YggdrasilWorldGenPass : GenPass
	{
		public YggdrasilWorldGenPass()
			: base("Yggdrasil, the Tree World", 500)
		{
		}

		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			Main.statusText = Terraria.Localization.Language.GetTextValue("Mods.Everglow.Common.WorldSystem.BuildtheTreeWorld");
			TotalInitialize();
			Main.spawnTileX = 1475;
			Main.spawnTileY = 20600;
			BuildYggdrasilTown();

			BuildKelpCurtain();
			EndGenPass();
			Main.statusText = string.Empty;
		}
	}

	public class MainWorldPylonRelicGenPass_Yggdrasil : GenPass
	{
		public MainWorldPylonRelicGenPass_Yggdrasil()
			: base("Inserting Yggdrasil Relic...", 501)
		{
		}

		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			progress.Message = "Inserting Yggdrasil Relic...";
			MainWorldGeneration_Yggdrasil.BuildYggdrasilPylonRelic();
		}
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks, ref double totalWeight) => tasks.Add(new MainWorldPylonRelicGenPass_Yggdrasil());

	public static int[,] PerlinPixel2 = new int[1024, 1024];
	public static int[,] CellPixel = new int[512, 512];
	public static int[,] MeltingPixel = new int[256, 256];

	/// <summary>
	/// A float value based on the texture;0.0f~1.0f.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static float GetLargeSmokeTexturePixel2(float x, float y)
	{
		return PerlinPixel2[(int)Math.Abs(x) % 1024, (int)Math.Abs(y) % 1024] / 255f;
	}

	/// <summary>
	/// A float value based on the texture;0.0f~1.0f.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static float GetCellPixel(float x, float y)
	{
		return CellPixel[(int)Math.Abs(x) % 512, (int)Math.Abs(y) % 512] / 255f;
	}

	/// <summary>
	/// A float value based on the texture;0.0f~1.0f.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static float GetMeltingPixel(float x, float y)
	{
		return MeltingPixel[(int)Math.Abs(x) % 256, (int)Math.Abs(y) % 256] / 255f;
	}

	/// <summary>
	/// When put some MapIO or other pieces of buildings, exclude them from being smoothened.
	/// </summary>
	public static List<Rectangle> ShouldNotSmoothAreas = new List<Rectangle>();

	/// <summary>
	/// 总初始化
	/// </summary>
	public static void TotalInitialize()
	{
		FillPerlinPixel();
	}

	/// <summary>
	/// 噪声信息获取
	/// </summary>
	public static void FillPerlinPixel()
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Yggdrasil/WorldGeneration/Noise_perlin.bmp");
		var perlinCoordCenter = new Vector2(GenRand.NextFloat(0f, 1f), GenRand.NextFloat(0f, 1f));
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
					PerlinPixel2[x, y] = pixel.R;
				}
			}
		});

		imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Yggdrasil/WorldGeneration/Noise_cell.bmp");
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
					CellPixel[x, y] = pixel.R;
				}
			}
		});

		imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Yggdrasil/WorldGeneration/Noise_melting.bmp");
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
					MeltingPixel[x, y] = pixel.R;
				}
			}
		});
	}

	public static void EndGenPass()
	{
		Main.statusText = "Finished";
	}

	/// <summary>
	/// smoothTopValue: 0 is absolute smooth, 1 is max rough.
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="height"></param>
	/// <param name="type"></param>
	/// <param name="smoothTopValue"></param>
	/// <param name="topThick"></param>
	public static void GenerateStalactite(Vector2 center, float radius, float height, int type, float smoothTopValue = 0.25f, float topThick = 4)
	{
		int cX = (int)center.X;
		int cY = (int)center.Y;
		int heightLimit = CheckSpaceDown(cX, cY);
		bool hitBottom = false;
		if (heightLimit < height)
		{
			height = heightLimit;
		}
		if (heightLimit < height * 1.85f)
		{
			hitBottom = true;
		}
		for (int i = -(int)radius; i <= radius; i++)
		{
			for (int j = 0; j <= height; j++)
			{
				float xValue = i / radius;
				float yValue = j / height;
				float cosX = MathF.Cos(xValue * MathHelper.PiOver2);
				float minCosX = Math.Min(cosX, 1 - Math.Abs(xValue));
				float limit = MathF.Pow(minCosX, 2);
				if (yValue < limit)
				{
					Tile tile = SafeGetTile(i + cX, j + cY);
					tile.TileType = (ushort)type;
					tile.HasTile = true;
				}
			}
		}
		if (hitBottom)
		{
			for (int i = -(int)radius; i <= radius; i++)
			{
				for (int j = 0; j <= height; j++)
				{
					float xValue = i / radius;
					float yValue = j / height;
					float cosX = MathF.Cos(xValue * MathHelper.PiOver2);
					float minCosX = Math.Min(cosX, 1 - Math.Abs(xValue));
					float limit = MathF.Pow(minCosX, 1);
					if (yValue < limit)
					{
						Tile tile = SafeGetTile(i + cX, cY + heightLimit - j);
						tile.TileType = (ushort)type;
						tile.HasTile = true;
					}
				}
			}
		}
		for (int i = -(int)radius; i <= radius; i++)
		{
			for (int j = 0; j <= topThick + 3; j++)
			{
				int x = i + cX;
				int y = cY - j;
				float value = j / topThick + (GetLargeSmokeTexturePixelR(x, y) * 256 - 128) / 64f * smoothTopValue;
				if (value < 1)
				{
					Tile tile = SafeGetTile(x, y);
					tile.TileType = (ushort)type;
					tile.HasTile = true;
				}
			}
		}
	}

	/// <summary>
	/// Fill liquids by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void PlaceRectangleAreaOfLiquid(int x0, int y0, int x1, int y1, int type)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				if (ChestSafe(x, y))
				{
					tile.liquid = (byte)type;
					tile.LiquidAmount = 255;
				}
			}
		}
	}

	/// <summary>
	/// Fill tiles by given area:(center and half side length);type = -1 to kill tiles.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void PlaceSquareAreaOfBlock(Point point, int halfSideRange, int type)
	{
		Point startPoint = point - new Point(halfSideRange, halfSideRange);
		for (int x = 0; x <= halfSideRange * 2; x += 1)
		{
			for (int y = 0; y <= halfSideRange * 2; y += 1)
			{
				Point check = startPoint + new Point(x, y);
				Tile tile = SafeGetTile(check);
				if (ChestSafe(check.X, check.Y))
				{
					if (type >= 0)
					{
						tile.TileType = (ushort)type;
						tile.HasTile = true;
					}
					else
					{
						tile.HasTile = false;
					}
				}
			}
		}
	}

	/// <summary>
	/// Fill liquids by given area:(center and half side length);type = -1 to kill tiles.
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void PlaceSquareAreaOfLiquid(Point point, int halfSideRange, int type)
	{
		Point startPoint = point - new Point(halfSideRange, halfSideRange);
		for (int x = 0; x <= halfSideRange * 2; x += 1)
		{
			for (int y = 0; y <= halfSideRange * 2; y += 1)
			{
				Point check = startPoint + new Point(x, y);
				Tile tile = SafeGetTile(check);
				if (ChestSafe(check.X, check.Y))
				{
					tile.liquid = (byte)type;
					tile.LiquidAmount = 255;
				}
			}
		}
	}

	/// <summary>
	/// Fill walls by given area:(center and half side length)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void PlaceSquareAreaOfWall(Point point, int halfSideRange, int type)
	{
		Point startPoint = point - new Point(halfSideRange, halfSideRange);
		for (int x = 0; x <= halfSideRange * 2; x += 1)
		{
			for (int y = 0; y <= halfSideRange * 2; y += 1)
			{
				Point check = startPoint + new Point(x, y);
				Tile tile = SafeGetTile(check);
				if (ChestSafe(check.X, check.Y))
				{
					tile.WallType = (ushort)type;
				}
			}
		}
	}

	/// <summary>
	/// Fill walls by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void PlaceRectangleAreaOfWall(int x0, int y0, int x1, int y1, int type)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				tile.WallType = (ushort)type;
			}
		}
		SmoothTile_XXYY(x0, y0, x1, y1);
	}

	/// <summary>
	/// Clear everything by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void ClearRectangleArea(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				tile.ClearEverything();
			}
		}
		SmoothTile_XXYY(x0, y0, x1, y1);
	}

	/// <summary>
	/// Clear tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void KillRectangleAreaOfTile(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				if (ChestSafe(x, y))
				{
					tile.HasTile = false;
				}
			}
		}
		SmoothTile_XXYY(x0, y0, x1, y1);
	}

	/// <summary>
	/// Clear tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	public static void KillRectangleAreaOfLiquid(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				tile.LiquidAmount = 0;
			}
		}
	}

	/// <summary>
	/// Clear tiles by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	public static void KillRectangleAreaOfWire(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				tile.RedWire = false;
				tile.GreenWire = false;
				tile.BlueWire = false;
				tile.YellowWire = false;
			}
		}
	}

	/// <summary>
	/// Clear walls by given area:(x0:left, y0:top, x1:right, y1:bottom)
	/// </summary>
	/// <param name="x0"></param>
	/// <param name="y0"></param>
	/// <param name="x1"></param>
	/// <param name="y1"></param>
	/// <param name="type"></param>
	public static void KillRectangleAreaOfWall(int x0, int y0, int x1, int y1)
	{
		for (int x = x0; x <= x1; x += 1)
		{
			for (int y = y0; y <= y1; y += 1)
			{
				Tile tile = SafeGetTile(x, y);
				tile.WallType = 0;
			}
		}
		SmoothTile_XXYY(x0, y0, x1, y1);
	}

	/// <summary>
	/// Place a prefabricated mapIO anchored top left at (x, y).
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="Path"></param>
	public static void QuickBuild(int x, int y, string Path, bool stopSmooth = true)
	{
		var mapIO = new MapIO(x, y);
		mapIO.Read(ModIns.Mod.GetFileStream(Path));
		if (stopSmooth)
		{
			ShouldNotSmoothAreas.Add(new Rectangle(x, y, mapIO.width, mapIO.height));
		}
		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
	}

	/// <summary>
	/// Return the tile-embedded-depth by the given point, a float value, but 1 tile's length = 1.0f.
	/// </summary>
	/// <returns></returns>
	public static float EmbeddingDepth(int x, int y, int maxRange = 4)
	{
		float minDepth = maxRange;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				if (!SafeGetTile(i + x, j + y).HasTile)
				{
					if (new Vector2(i, j).Length() <= minDepth)
					{
						minDepth = new Vector2(i, j).Length();
					}
				}
			}
		}
		return minDepth;
	}

	/// <summary>
	/// Return the wall-embedded-depth by the given point.
	/// </summary>
	/// <returns></returns>
	public static float EmbeddingWallDepth(int x, int y, int maxRange = 50)
	{
		float depth = maxRange;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				if (new Vector2(i, j).Length() <= depth)
				{
					if (SafeGetTile(i + x, j + y).wall <= 0)
					{
						depth = new Vector2(i, j).Length();
					}
				}
			}
		}
		return depth;
	}

	/// <summary>
	/// Return the certain type tile-embedded-depth by the given point.
	/// </summary>
	/// <returns></returns>
	public static int EmbeddingDepthOfTileType(int x, int y, int type, int maxRange = 4)
	{
		int depth = 0;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				if (new Vector2(i, j).Length() <= maxRange)
				{
					Tile tile = SafeGetTile(i + x, j + y);
					if (tile.HasTile && tile.TileType == type)
					{
						depth++;
					}
				}
			}
		}
		return depth;
	}

	/// <summary>
	/// Return a unit normal perpendicular to tile edge near by the given point.
	/// </summary>
	/// <returns></returns>
	public static Vector2 TerrianSurfaceNormal(int x, int y, int maxRange = 4, int excludeTileType = -1)
	{
		Vector2 v0 = Vector2.zeroVector;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange && v1.Length() > 0)
				{
					Tile tile = SafeGetTile(i + x, j + y);
					if (tile.HasTile && tile.TileType != excludeTileType)
					{
						v0 += Vector2.Normalize(v1) / v1.Length();
					}
					else
					{
						v0 -= Vector2.Normalize(v1) / v1.Length();
					}
				}
			}
		}
		if (v0.Length() < 0.1f)
		{
			return Vector2.zeroVector;
		}
		return Vector2.Normalize(v0);
	}

	/// <summary>
	/// Return tilt angle of tile edge near by the given point(in rad).
	/// </summary>
	/// <returns></returns>
	public static float TerrianSurfaceAngle(int x, int y, int maxRange = 4, int excludeTileType = -1)
	{
		Vector2 v0 = Vector2.zeroVector;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange && v1.Length() > 0)
				{
					Tile tile = SafeGetTile(i + x, j + y);
					if (tile.HasTile && tile.TileType != excludeTileType)
					{
						v0 += Vector2.Normalize(v1) / v1.Length();
					}
					else
					{
						v0 -= Vector2.Normalize(v1) / v1.Length();
					}
				}
			}
		}
		if (v0.Length() < 0.1f)
		{
			return -1;
		}
		return v0.ToRotation();
	}

	/// <summary>
	/// 返回一点附近的地势法线的离散度
	/// </summary>
	/// <returns></returns>
	public static float TerrianSurfaceDiscontinuity(int x, int y, int maxRange = 4, int excludeTileType = -1)
	{
		List<Vector2> normalsVector = new List<Vector2>();
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange)
				{
					Vector2 v0 = TerrianSurfaceNormal(i + x, j + y, maxRange, excludeTileType);
					if (v0 != Vector2.zeroVector)
					{
						normalsVector.Add(v0);
					}
				}
			}
		}
		if (normalsVector.Count <= 1)
		{
			return 0;
		}

		// 计算平均向量
		float meanX = normalsVector.Select(v => v.X).Average();
		float meanY = normalsVector.Select(v => v.Y).Average();

		Tuple<float, float> meanVector = Tuple.Create(meanX, meanY);

		// 计算离散度
		double dispersion = normalsVector.Sum(v =>
			Math.Pow(v.X - meanX, 2) + Math.Pow(v.Y - meanY, 2)) / (normalsVector.Count - 1);

		return (float)dispersion;
	}

	/// <summary>
	/// 返回一点附近的某种物块的边缘法线
	/// </summary>
	/// <returns></returns>
	public static Vector2 TerrianTypeTileSurfaceNormal(int x, int y, int type, int maxRange = 4)
	{
		Vector2 v0 = Vector2.zeroVector;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange && v1.Length() > 0)
				{
					Tile tile = SafeGetTile(i + x, j + y);
					if (tile.HasTile && tile.TileType == type)
					{
						v0 += Vector2.Normalize(v1) / v1.Length();
					}
					else
					{
						v0 -= Vector2.Normalize(v1) / v1.Length();
					}
				}
			}
		}
		if (v0.Length() < 0.1f)
		{
			return Vector2.zeroVector;
		}
		return Vector2.Normalize(v0);
	}

	/// <summary>
	/// 返回一点附近的某种物块的边缘倾角
	/// </summary>
	/// <returns></returns>
	public static float TerrianTypeTileSurfaceAngle(int x, int y, int type, int maxRange = 4)
	{
		Vector2 v0 = Vector2.zeroVector;
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange && v1.Length() > 0)
				{
					Tile tile = SafeGetTile(i + x, j + y);
					if (tile.HasTile && tile.TileType == type)
					{
						v0 += Vector2.Normalize(v1) / v1.Length();
					}
					else
					{
						v0 -= Vector2.Normalize(v1) / v1.Length();
					}
				}
			}
		}
		if (v0.Length() < 0.1f)
		{
			return -1;
		}
		return v0.ToRotation();
	}

	/// <summary>
	/// 返回一点附近的某种物块的边缘法线的离散度
	/// </summary>
	/// <returns></returns>
	public static float TerrianTypeTileSurfaceDiscontinuity(int x, int y, int type, int maxRange = 4)
	{
		List<Vector2> normalsVector = new List<Vector2>();
		for (int i = -maxRange; i <= maxRange; i++)
		{
			for (int j = -maxRange; j <= maxRange; j++)
			{
				Vector2 v1 = new Vector2(i, j);
				if (v1.Length() <= maxRange)
				{
					Vector2 v0 = TerrianTypeTileSurfaceNormal(i + x, j + y, type, maxRange);
					if (v0 != Vector2.zeroVector)
					{
						normalsVector.Add(v0);
					}
				}
			}
		}
		if (normalsVector.Count <= 1)
		{
			return 0;
		}

		// 计算平均向量
		float meanX = normalsVector.Select(v => v.X).Average();
		float meanY = normalsVector.Select(v => v.Y).Average();

		Tuple<float, float> meanVector = Tuple.Create(meanX, meanY);

		// 计算离散度
		double dispersion = normalsVector.Sum(v =>
			Math.Pow(v.X - meanX, 2) + Math.Pow(v.Y - meanY, 2)) / (normalsVector.Count - 1);

		return (float)dispersion;
	}

	/// <summary>
	/// 返回一点到100格以内最近物块的距离
	/// </summary>
	/// <returns></returns>
	public static float To100NearestBlockDistance(int x, int y)
	{
		float minDis = 100;
		for (int i = -100; i <= 100; i++)
		{
			for (int j = -100; j <= 100; j++)
			{
				Tile tile = SafeGetTile(i + x, j + y);
				if (tile.HasTile)
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						minDis = v1.Length();
					}
				}
			}
		}
		return minDis;
	}

	/// <summary>
	/// 返回一点到100格以内最近指定种物块的距离
	/// </summary>
	/// <returns></returns>
	public static float To100NearestTileTypeBlockDistance(int x, int y, int type)
	{
		float minDis = 100;
		for (int i = -100; i <= 100; i++)
		{
			for (int j = -100; j <= 100; j++)
			{
				Tile tile = SafeGetTile(i + x, j + y);
				if (tile.HasTile && tile.TileType == type)
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						minDis = v1.Length();
					}
				}
			}
		}
		return minDis;
	}

	/// <summary>
	/// 距离最近的物块坐标,可以排除一种
	/// </summary>
	/// <returns></returns>
	public static Point NearestBlockCoordinateIn100Tile(int x, int y, int excludeTileType = -1)
	{
		float minDis = 100;
		Point attachPoint = new Point(x, y);
		for (int i = -100; i <= 100; i++)
		{
			for (int j = -100; j <= 100; j++)
			{
				Tile tile = SafeGetTile(i + x, j + y);
				if (tile.HasTile && tile.TileType != excludeTileType)
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						attachPoint = new Point(i + x, j + y);
						minDis = v1.Length();
					}
				}
			}
		}
		return attachPoint;
	}

	/// <summary>
	/// 返回一点到100格以内最近空旷的距离
	/// </summary>
	/// <returns></returns>
	public static float To100NearestEmptyDistance(int x, int y)
	{
		float minDis = 100;
		for (int i = -100; i <= 100; i++)
		{
			for (int j = -100; j <= 100; j++)
			{
				Tile tile = SafeGetTile(i + x, j + y);
				if (!tile.HasTile)
				{
					Vector2 v1 = new Vector2(i, j);
					if (v1.Length() < minDis)
					{
						minDis = v1.Length();
					}
				}
			}
		}
		return minDis;
	}

	/// <summary>
	/// 沿着地势表面匍匐行进铺设物块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="step"></param>
	/// <param name="thick"></param>
	/// <param name="clockwise"></param>
	public static void CrawlCarpetOfTile(int x, int y, int step, int thick, int type, bool clockwise = false)
	{
		Point checkPoint = NearestBlockCoordinateIn100Tile(x, y, type);
		if (!SafeGetTile(checkPoint).HasTile)
		{
			return;
		}
		Vector2 velocity = TerrianSurfaceNormal(checkPoint.X, checkPoint.Y).RotatedBy(clockwise ? MathHelper.PiOver2 : -MathHelper.PiOver2);
		Vector2 position = checkPoint.ToVector2();
		for (int i = 0; i < step; i++)
		{
			float thickValue = thick * Math.Min((step / 2f - MathF.Abs(step / 2f - i)) * 0.2f, 1f);
			Vector2 normal = TerrianSurfaceNormal((int)position.X, (int)position.Y);
			CircleTile(position + normal * thickValue, position, type);
			position += velocity;
			int count = 0;
			while (!SafeGetTile(position).HasTile)
			{
				count++;
				position += TerrianSurfaceNormal((int)position.X, (int)position.Y);
				if (count > 100)
				{
					break;
				}
			}
			velocity = TerrianSurfaceNormal((int)position.X, (int)position.Y).RotatedBy(clockwise ? MathHelper.PiOver2 : -MathHelper.PiOver2);
		}
	}

	/// <summary>
	/// 沿着某种物块表面匍匐行进铺设物块
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="step"></param>
	/// <param name="thick"></param>
	/// <param name="clockwise"></param>
	public static void CrawlCarpetOfTypeTile(int x, int y, int step, int thick, int type, int backgroundType, bool clockwise = false)
	{
		Point checkPoint = NearestBlockCoordinateIn100Tile(x, y, type);
		if (!SafeGetTile(checkPoint).HasTile)
		{
			return;
		}
		Vector2 velocity = TerrianTypeTileSurfaceNormal(checkPoint.X, checkPoint.Y, backgroundType).RotatedBy(clockwise ? MathHelper.PiOver2 : -MathHelper.PiOver2);
		Vector2 position = checkPoint.ToVector2();
		for (int i = 0; i < step; i++)
		{
			float thickValue = thick * Math.Min((step / 2f - MathF.Abs(step / 2f - i)) * 0.2f, 1f);
			Vector2 normal = TerrianTypeTileSurfaceNormal((int)position.X, (int)position.Y, backgroundType);
			CircleTile(position + normal * thickValue, position, type);
			position += velocity;
			int count = 0;
			while (!SafeGetTile(position).HasTile)
			{
				count++;
				position += TerrianTypeTileSurfaceNormal((int)position.X, (int)position.Y, backgroundType);
				if (count > 100)
				{
					break;
				}
			}
			velocity = TerrianTypeTileSurfaceNormal((int)position.X, (int)position.Y, backgroundType).RotatedBy(clockwise ? MathHelper.PiOver2 : -MathHelper.PiOver2);
		}
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate, and (type >= 0, place that type of tile, type = -1,clear tiles; tile = -2,clear everything).
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleTile(Vector2 center, float radius, int type, bool force = false)
	{
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius)
				{
					if (ChestSafe(center + new Vector2(x, y)))
					{
						if (type == -2)
						{
							tile.ClearEverything();
						}
						else if (type == -1)
						{
							tile.HasTile = false;
						}
						else if (type >= 0)
						{
							if (force)
							{
								tile.TileType = (ushort)type;
								tile.HasTile = true;
							}
							else
							{
								if (!tile.HasTile)
								{
									tile.TileType = (ushort)type;
									tile.HasTile = true;
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// Set a center and radius of a circle in tile coordinate, and (type >= 0, place that type of tile, type = -1,clear tiles; tile = -2,clear everything).
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleTile(Point center, float radius, int type, bool force = false)
	{
		CircleTile(center.ToVector2(), radius, type, force);
	}

	/// <summary>
	/// Set 2 point as diameter that defines a circle in tile coordinate, and (type >= 0, place that type of tile, type = -1,clear tiles; tile = -2,clear everything).
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleTile(Vector2 pointA, Vector2 pointB, int type, bool force = false)
	{
		float radius = (pointA - pointB).Length() * 0.5f;
		Vector2 center = (pointA + pointB) * 0.5f;
		CircleTile(center, radius, type, force);
	}

	/// <summary>
	/// 圆心和直径布设圆形墙,=-1清理物块,-2清理全部
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleWall(Vector2 center, float radius, int type, bool force = false)
	{
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				if (new Vector2(x, y).Length() <= radius)
				{
					if (type == -1)
					{
						tile.ClearEverything();
					}
					else if (type >= 0)
					{
						if (force)
						{
							tile.WallType = (ushort)type;
						}
						else
						{
							if (tile.WallType <= 0)
							{
								tile.WallType = (ushort)type;
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 两点为直径布设圆形墙
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleWall(Vector2 pointA, Vector2 pointB, int type, bool force = false)
	{
		float radius = (pointA - pointB).Length() * 0.5f;
		Vector2 center = (pointA + pointB) * 0.5f;
		CircleWall(center, radius, type, force);
	}

	/// <summary>
	/// Transform the tile within the circle(center, radius) to the type, but with a random noise affect on the bound.<br/>
	/// force = true, directly set the tile to the type; force = false, only set the tile to the type when there is no tile. <br/>
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleTileWithRandomNoise(Vector2 center, float radius, int type, float noiseSize = 10f, bool force = false)
	{
		int x0CoordPerlin = GenRand.Next(1024);
		int y0CoordPerlin = GenRand.Next(1024);
		int radiusI = (int)radius;
		for (int x = -radiusI; x <= radiusI; x++)
		{
			for (int y = -radiusI; y <= radiusI; y++)
			{
				Tile tile = SafeGetTile(center + new Vector2(x, y));
				float aValue = GetLargeSmokeTexturePixelR(x + x0CoordPerlin, y + y0CoordPerlin);
				if (ChestSafe(center + new Vector2(x, y)))
				{
					if (new Vector2(x, y).Length() <= radius - aValue * noiseSize)
					{
						if (type == -1)
						{
							tile.ClearEverything();
						}
						else if (type >= 0)
						{
							if (force)
							{
								tile.TileType = (ushort)type;
								tile.HasTile = true;
							}
							else
							{
								if (!tile.HasTile)
								{
									tile.TileType = (ushort)type;
									tile.HasTile = true;
								}
							}
						}
					}
				}
			}
		}
	}

	/// <summary>
	/// 圆心半径,布设不规则圆形物块(小于半径),=-1清理物块,-2清理全部
	/// </summary>
	/// <param name="center"></param>
	/// <param name="radius"></param>
	/// <param name="type"></param>
	/// <param name="force"></param>
	public static void CircleTileWithRandomNoise(Point center, float radius, int type, float noiseSize = 10f, bool force = false)
	{
		CircleTileWithRandomNoise(center.ToVector2(), radius, type, noiseSize, force);
	}

	public static void CircleTileWithRandomNoise(Vector2 pointA, Vector2 pointB, int type, float noiseSize = 10f, bool force = false)
	{
		float radius = (pointA - pointB).Length() * 0.5f;
		Vector2 center = (pointA + pointB) * 0.5f;
		CircleTileWithRandomNoise(center, radius, type, noiseSize, force);
	}

	/// <summary>
	/// type = 0:Kill,type = 1:place Tiles,type = 2:place Walls
	/// </summary>
	/// <param name="Shapepath"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="type"></param>
	public static void ShapeTile(string Shapepath, int a, int b, int type)
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Yggdrasil/WorldGeneration/" + Shapepath);
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				for (int x = 0; x < pixelRow.Length; x++)
				{
					ref var pixel = ref pixelRow[x];
					Tile tile = SafeGetTile(x + a, y + b);
					switch (type)// 21是箱子
					{
						case 0:
							if (pixel.R == 255 && pixel.G == 0 && pixel.B == 0)
							{
								if (tile.TileType != TileID.Containers && SafeGetTile(x + a, y + b - 1).TileType != TileID.Containers)
								{
									tile.ClearEverything();
								}
							}
							break;
						case 1:
							// 天穹古道
							if (pixel.R == 44 && pixel.G == 40 && pixel.B == 37)// 石化龙鳞木
							{
								tile.TileType = (ushort)ModContent.TileType<StoneScaleWood>();
								tile.HasTile = true;
							}
							if (pixel.R == 155 && pixel.G == 173 && pixel.B == 183)// 青缎矿
							{
								tile.TileType = (ushort)ModContent.TileType<CyanVineStone>();
								tile.HasTile = true;
							}

							if (pixel.R == 31 && pixel.G == 26 && pixel.B == 45)// 黑淤泥
							{
								tile.LiquidType = LiquidLoader.LiquidType<DarkSludgeLiquid>();
								tile.LiquidAmount = byte.MaxValue;
							}

							// 苍苔蔓帘
							if (pixel.R == 82 && pixel.G == 62 && pixel.B == 44)// 龙鳞木
							{
								tile.TileType = (ushort)ModContent.TileType<DragonScaleWood>();
								tile.HasTile = true;
							}
							if (pixel.R == 81 && pixel.G == 107 && pixel.B == 18)// 古苔藓
							{
								tile.TileType = (ushort)ModContent.TileType<OldMoss>();
								tile.HasTile = true;
							}
							if (pixel.R == 53 && pixel.G == 29 && pixel.B == 26)// 天穹泥
							{
								tile.TileType = (ushort)ModContent.TileType<MossProneSandSoil>();
								tile.HasTile = true;
							}

							// 飓风迷宫
							if (pixel.R == 65 && pixel.G == 84 && pixel.B == 63)// 青岗岩
							{
								tile.TileType = (ushort)ModContent.TileType<CyanWindGranite>();
								tile.HasTile = true;
							}

							// 蛆败之穴
							if (pixel.R == 107 && pixel.G == 34 && pixel.B == 21)// 血解光石
							{
								tile.TileType = (ushort)ModContent.TileType<BloodLightCrystal>();
								tile.HasTile = true;
								ModContent.GetInstance<BloodLightCrystalEntity>().Place(x + a, y + b);
							}

							// 常规
							if (pixel.R == 0 && pixel.G == 0 && pixel.B == 255)// 水
							{
								tile.LiquidType = LiquidID.Water;
								tile.LiquidAmount = 200;
								tile.HasTile = false;
							}
							if (pixel.R == 128 && pixel.G == 128 && pixel.B == 128)// 岩石
							{
								tile.TileType = TileID.Stone;
								tile.HasTile = true;
							}
							if (pixel.R == 186 && pixel.G == 168 && pixel.B == 84)// 沙
							{
								tile.TileType = TileID.Sand;
								tile.HasTile = true;
							}
							break;
						case 2:
							if (pixel.R == 24 && pixel.G == 0 && pixel.B == 0)// 石化龙鳞木
							{
								if (tile.TileType != TileID.Containers && SafeGetTile(x + a, y + b - 1).TileType != TileID.Containers)
								{
									tile.WallType = (ushort)ModContent.WallType<StoneDragonScaleWoodWall>();
								}
							}
							if (pixel.R == 40 && pixel.G == 32 && pixel.B == 31)// 龙鳞木
							{
								if (tile.TileType != TileID.Containers && SafeGetTile(x + a, y + b - 1).TileType != TileID.Containers)
								{
									tile.WallType = (ushort)ModContent.WallType<DragonScaleWoodWall>();
								}
							}
							if (pixel.R == 56 && pixel.G == 56 && pixel.B == 56)// 石墙
							{
								if (tile.TileType != TileID.Containers && SafeGetTile(x + a, y + b - 1).TileType != TileID.Containers)
								{
									tile.WallType = WallID.Stone;
								}
							}
							if (pixel.R == 25 && pixel.G == 14 && pixel.B == 12)// 天穹土墙
							{
								if (tile.TileType != TileID.Containers && SafeGetTile(x + a, y + b - 1).TileType != TileID.Containers)
								{
									tile.WallType = (ushort)ModContent.WallType<MossProneSandSoilWall>();
								}
							}
							break;
						case 3: // 天穹古道建筑
							if (pixel.R == 121 && pixel.G == 5 && pixel.B == 255)// FolkHouseofChineseStyle TypeA  28x11
							{
								QuickBuild(x, y, ModAsset.MapIOs_1FolkHouseofChineseStyleTypeA28x11_Path);
							}

							if (pixel.R == 120 && pixel.G == 5 && pixel.B == 255)// FolkHouseofChineseStyle TypeB  28x11
							{
								QuickBuild(x, y, ModAsset.MapIOs_1FolkHouseofChineseStyleTypeB28x11_Path);
							}

							if (pixel.R == 122 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWood＆StoneStruture TypeA  28x11
							{
								QuickBuild(x, y, ModAsset.MapIOs_2FolkHouseofWoodAndStoneStrutureTypeA28x11_Path);
							}

							if (pixel.R == 123 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWood＆StoneStruture TypeB  28x11
							{
								QuickBuild(x, y, ModAsset.MapIOs_2FolkHouseofWoodAndStoneStrutureTypeB28x11_Path);
							}

							if (pixel.R == 124 && pixel.G == 5 && pixel.B == 255)// Smithy TypeA  22x8
							{
								QuickBuild(x, y, ModAsset.MapIOs_3SmithyTypeA22x8_Path);
							}

							if (pixel.R == 125 && pixel.G == 5 && pixel.B == 255)// Smithy TypeB  22x8
							{
								QuickBuild(x, y, ModAsset.MapIOs_3SmithyTypeB22x8_Path);
							}

							if (pixel.R == 126 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeA  22x10
							{
								QuickBuild(x, y, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeA22x10_Path);
							}

							if (pixel.R == 127 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeB  22x10
							{
								QuickBuild(x, y, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeB22x10_Path);
							}

							if (pixel.R == 128 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeC  22x10
							{
								QuickBuild(x, y, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeC22x10_Path);
							}

							if (pixel.R == 129 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeD  22x10
							{
								QuickBuild(x, y, ModAsset.MapIOs_4FolkHouseofWoodStrutureTypeD22x10_Path);
							}

							if (pixel.R == 130 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeA  23x13
							{
								QuickBuild(x, y, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeA23x13_Path);
							}

							if (pixel.R == 131 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeB  23x13
							{
								QuickBuild(x, y, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeB23x13_Path);
							}

							if (pixel.R == 132 && pixel.G == 5 && pixel.B == 255)// FolkHouseofWoodStruture TypeC  23x13
							{
								QuickBuild(x, y, ModAsset.MapIOs_5TwoStoriedFolkHouseTypeC23x13_Path);
							}

							if (pixel.R == 133 && pixel.G == 5 && pixel.B == 255)// Church 80x51
							{
								QuickBuild(x, y, ModAsset.Church80x51_Path);
							}

							break;
						case 4:
							if (pixel.R == 195 && pixel.G == 217 && pixel.B == 229)// 大青缎矿
							{
								PlaceLargeCyanVineOre(x, y);
							}

							if (pixel.R == 195 && pixel.G == 217 && pixel.B == 230)// 中青缎矿
							{
								PlaceMiddleCyanVineOre(x, y);
							}

							if (pixel.R == 195 && pixel.G == 217 && pixel.B == 231)// 小青缎矿
							{
								PlaceSmallCyanVineOre(x, y);
							}

							if (pixel.R == 195 && pixel.G == 217 && pixel.B == 232)// 倒挂小青缎矿
							{
								PlaceSmallUpCyanVineOre(x, y);
							}

							if (pixel.R == 195 && pixel.G == 217 && pixel.B == 233)// 倒挂大青缎矿
							{
								PlaceLargeUpCyanVineOre(x, y);
							}

							break;
					}
				}
			}
		});
	}

	/// <summary>
	/// Digtunnel, mimic from WorldGen vanilla.
	/// </summary>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <param name="xDir"></param>
	/// <param name="yDir"></param>
	/// <param name="Steps"></param>
	/// <param name="Size"></param>
	/// <param name="Wet"></param>
	/// <returns></returns>
	public static Vector2D DigTunnel(double x, double y, double xDir, double yDir, int steps, int size, bool wet = false, int wallType = -1)
	{
		double startX = x;
		double startY = y;
		try
		{
			double xVel = 0.0;
			double yVel = 0.0;
			double checkSize = size;
			startX = Utils.Clamp(startX, checkSize + 1.0, Main.maxTilesX - checkSize - 1.0);
			startY = Utils.Clamp(startY, checkSize + 1.0, Main.maxTilesY - checkSize - 1.0);
			for (int i = 0; i < steps; i++)
			{
				for (int j = (int)(startX - checkSize); j <= startX + checkSize; j++)
				{
					for (int k = (int)(startY - checkSize); k <= startY + checkSize; k++)
					{
						if (Math.Abs(j - startX) + Math.Abs(k - startY) < checkSize * (1.0 + GenRand.Next(-10, 11) * 0.005) && j >= 0 && j < Main.maxTilesX && k >= 0 && k < Main.maxTilesY)
						{
							Tile tile = SafeGetTile(j, k);
							if (ChestSafe(j, k))
							{
								tile.active(active: false);
								if (wet)
								{
									tile.liquid = byte.MaxValue;
								}
								if (wallType != -1)
								{
									tile.WallType = (ushort)wallType;
								}
							}
						}
					}
				}

				checkSize += GenRand.Next(-50, 51) * 0.03;
				if (checkSize < size * 0.6)
				{
					checkSize = size * 0.6;
				}

				if (checkSize > size * 2)
				{
					checkSize = size * 2;
				}

				xVel += GenRand.Next(-20, 21) * 0.01;
				yVel += GenRand.Next(-20, 21) * 0.01;
				if (xVel < -1.0)
				{
					xVel = -1.0;
				}

				if (xVel > 1.0)
				{
					xVel = 1.0;
				}

				if (yVel < -1.0)
				{
					yVel = -1.0;
				}

				if (yVel > 1.0)
				{
					yVel = 1.0;
				}

				startX += (xDir + xVel) * 0.6;
				startY += (yDir + yVel) * 0.6;
			}
		}
		catch
		{
		}

		return new Vector2D(startX, startY);
	}

	/// <summary>
	/// Digtunnel, only dig in request type of tile, mimic from WorldGen vanilla.
	/// </summary>
	/// <param name="X"></param>
	/// <param name="Y"></param>
	/// <param name="xDir"></param>
	/// <param name="yDir"></param>
	/// <param name="Steps"></param>
	/// <param name="Size"></param>
	/// <param name="Wet"></param>
	/// <returns></returns>
	public static Vector2D DigTunnelInRequsetTiles(double x, double y, double xDir, double yDir, int steps, int size, int type, bool wet = false, int wallType = -1)
	{
		double startX = x;
		double startY = y;
		try
		{
			double xVel = 0.0;
			double yVel = 0.0;
			double checkSize = size;
			startX = Utils.Clamp(startX, checkSize + 1.0, Main.maxTilesX - checkSize - 1.0);
			startY = Utils.Clamp(startY, checkSize + 1.0, Main.maxTilesY - checkSize - 1.0);
			for (int i = 0; i < steps; i++)
			{
				for (int j = (int)(startX - checkSize); j <= startX + checkSize; j++)
				{
					for (int k = (int)(startY - checkSize); k <= startY + checkSize; k++)
					{
						if (Math.Abs(j - startX) + Math.Abs(k - startY) < checkSize * (1.0 + GenRand.Next(-10, 11) * 0.005) && j >= 0 && j < Main.maxTilesX && k >= 0 && k < Main.maxTilesY)
						{
							Tile tile = SafeGetTile(j, k);
							if (ChestSafe(j, k))
							{
								if (tile.TileType == type)
								{
									tile.active(active: false);
									if (wet)
									{
										tile.liquid = byte.MaxValue;
									}
									if (wallType != -1)
									{
										tile.WallType = (ushort)wallType;
									}
								}
								else
								{
									return new Vector2D(startX, startY);
								}
							}
							Tile tileSafe = SafeGetTile((int)(j + (xVel + xDir) * 3), (int)(k + (yVel + yDir) * 3));
							if (tile.TileType != type)
							{
								return new Vector2D(startX, startY);
							}
						}
					}
				}

				checkSize += GenRand.Next(-50, 51) * 0.03;
				if (checkSize < size * 0.6)
				{
					checkSize = size * 0.6;
				}

				if (checkSize > size * 2)
				{
					checkSize = size * 2;
				}

				xVel += GenRand.Next(-20, 21) * 0.01;
				yVel += GenRand.Next(-20, 21) * 0.01;
				if (xVel < -1.0)
				{
					xVel = -1.0;
				}

				if (xVel > 1.0)
				{
					xVel = 1.0;
				}

				if (yVel < -1.0)
				{
					yVel = -1.0;
				}

				if (yVel > 1.0)
				{
					yVel = 1.0;
				}

				startX += (xDir + xVel) * 0.6;
				startY += (yDir + yVel) * 0.6;
			}
		}
		catch
		{
		}

		return new Vector2D(startX, startY);
	}

	/// <summary>
	/// Return true when the given point do not contained by any rectangles in ShouldNotSmoothAreas.
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <returns></returns>
	public static bool CanLegallySmooth(int x, int y)
	{
		foreach (Rectangle rectangle in ShouldNotSmoothAreas)
		{
			if (rectangle.Contains(x, y))
			{
				return false;
			}
		}
		return true;
	}

	/// <summary>
	/// Return true when no chest in given aren(include one row above).
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="width"></param>
	/// <param name="height"></param>
	/// <returns></returns>
	public static bool ChestSafeArea(int x, int y, int width, int height)
	{
		for (int x0 = x; x0 <= x + width; x0++)
		{
			for (int y0 = y; y0 <= y + height; y0++)
			{
				Tile tile = SafeGetTile(x, y);
				Tile tileUp = SafeGetTile(x, y - 1);
				if (TileID.Sets.BasicChest[tile.TileType] || TileID.Sets.BasicChest[tileUp.TileType])
				{
					return false;
				}
			}
		}
		return true;
	}

	/// <summary>
	/// Return true when no chest in given aren(include one row above).
	/// </summary>
	/// <param name="center"></param>
	/// <param name="size"></param>
	/// <returns></returns>
	public static bool ChestSafeArea(Vector2 center, Vector2 size)
	{
		return ChestSafeArea((int)center.X, (int)center.Y, (int)size.X, (int)size.Y);
	}
}
