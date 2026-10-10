using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Pylon;
using Everglow.Myth.TheFirefly.Tiles;
using Everglow.Myth.TheFirefly.Walls;
using Everglow.SpellAndSkull.Items;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.IO;
using Terraria.ModLoader.Default;
using Terraria.ObjectData;
using Terraria.WorldBuilding;

namespace Everglow.Myth.TheFirefly.WorldGeneration;

public class MothLand : ModSystem
{
	public static void QuickBuild(int x, int y, string Path)
	{
		var mapIO = new Commons.TileHelper.MapIO(x, y);

		mapIO.Read(ModIns.Mod.GetFileStream("Myth/" + Path));

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
	}

	public static void BuildShabbyCastle()
	{
		Point16 sbpp = ShabbyPylonPos();
		string Path = "MapIOResources/ShabbyCastle0" + (WorldGen.genRand.Next(7) + 1) + ".mapio";
		var mapIO = new Commons.TileHelper.MapIO(sbpp.X, sbpp.Y);
		int Height = mapIO.ReadHeight(ModIns.Mod.GetFileStream("Myth/" + Path));
		QuickBuild(sbpp.X, sbpp.Y - Height / 2, Path);

		var pylonBottom = new Point(sbpp.X + WorldGen.genRand.Next(8, 16), sbpp.Y - Height / 2 + 8);
		ushort PylonType = (ushort)ModContent.TileType<ShabbyPylon>();
		PylonSystem.Instance.shabbyPylonEnable = false;
		for (int a = 0; a < 12; a++)
		{
			pylonBottom.Y++;
			if (Main.tile[pylonBottom.X, pylonBottom.Y].HasTile)
			{
				pylonBottom.Y -= 1;
				break;
			}
		}
		for (int i = -2; i <= 2; i++)
		{
			var PylonTile = Main.tile[pylonBottom.X + i, pylonBottom.Y + 1];
			PylonTile.TileType = TileID.GrayBrick;
			PylonTile.HasTile = true;
			PylonTile.Slope = SlopeType.Solid;
			PylonTile.IsHalfBlock = false;
		}

		TileObject.CanPlace(pylonBottom.X, pylonBottom.Y, PylonType, 0, 0, out var tileObject);
		TileObject.Place(tileObject);
		TileObjectData.CallPostPlacementPlayerHook(pylonBottom.X, pylonBottom.Y, PylonType, 0, 0, 0, tileObject);
	}

	internal class MothLandGenPass : GenPass
	{
		public MothLandGenPass()
			: base("MothLand", 500)
		{
		}

		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			Main.statusText = Terraria.Localization.Language.GetTextValue("Mods.Everglow.Common.WorldSystem.BuildMothCave");
			BuildMothCave();
			Main.spawnTileX = 723;
			Main.spawnTileY = 226;
		}
	}

	internal class WorldMothLandGenPass : GenPass
	{
		public WorldMothLandGenPass()
			: base("MothLand", 500)
		{
		}

		public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
		{
			Main.statusText = Terraria.Localization.Language.GetTextValue("Mods.Everglow.Common.WorldSystem.BuildWorldMothCave");
			BuildWorldMothCave();
			BuildShabbyCastle();
		}
	}

	public override void ModifyWorldGenTasks(List<GenPass> tasks) => tasks.Add(new WorldMothLandGenPass());

	/// <summary>
	/// 地形中心坐标
	/// </summary>
	public int fireflyCenterX = 400;

	public int fireflyCenterY = 300;

	/// <summary>
	/// type = 0:Kill,type = 1:place Tiles,type = 2:place Walls
	/// </summary>
	/// <param name="Shapepath"></param>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="type"></param>
	public static void ShapeTile(string Shapepath, int a, int b, int type)
	{
		var imageData = ImageReader.Read<SixLabors.ImageSharp.PixelFormats.Rgb24>("Everglow/Myth/TheFirefly/WorldGeneration/" + Shapepath);
		imageData.ProcessPixelRows(accessor =>
		{
			for (int y = 0; y < accessor.Height; y++)
			{
				var pixelRow = accessor.GetRowSpan(y);
				if (y + b < 20)
				{
					continue;
				}

				if (y + b > Main.maxTilesY - 20)
				{
					break;
				}

				for (int x = 0; x < pixelRow.Length; x++)
				{
					if (x + a < 20)
					{
						continue;
					}

					if (x + a > Main.maxTilesX - 20)
					{
						break;
					}

					ref var pixel = ref pixelRow[x];
					Tile tile = Main.tile[x + a, y + b];
					switch (type)// 21是箱子
					{
						case 0:
							if (pixel.R == 255 && pixel.G == 0 && pixel.B == 0)// == new SixLabors.ImageSharp.PixelFormats.Rgb24(255, 0, 0))
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.ClearEverything();
								}
							}
							break;

						case 1:
							if (pixel.R == 56 && pixel.G == 48 && pixel.B == 61)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkCocoon>();
									tile.HasTile = true;
								}
							}
							if (pixel.R == 255 && pixel.G == 0 && pixel.B == 0)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkCocoonSpecial>();
									tile.HasTile = true;
								}
							}
							if (pixel.R == 35 && pixel.G == 49 && pixel.B == 122)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkCocoonMoss>();
									tile.HasTile = true;
								}
							}
							if (pixel.R == 112 && pixel.G == 130 && pixel.B == 175)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.TileType = (ushort)ModContent.TileType<DarkCocoon_petal>();
									tile.HasTile = true;
								}
							}
							if (pixel.R == 0 && pixel.G == 0 && pixel.B == 255)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.LiquidType = LiquidID.Water;
									tile.LiquidAmount = 200;
									tile.HasTile = false;

									// WorldGen.PlaceLiquid(x, y, byte.MaxValue, 255);
								}
							}
							if (pixel.R == 28 && pixel.G == 198 && pixel.B == 255)
							{
								LargeFireBulb.PlaceMe(x + a, y + b, (ushort)WorldGen.genRand.Next(16));
							}
							if (pixel.R == 28 && pixel.G == 132 && pixel.B == 255)
							{
								LargeFireBulb.PlaceMe(x + a, y + b, (ushort)WorldGen.genRand.Next(2));
							}
							break;

						case 2:
							if (pixel.R == 0 && pixel.G == 0 && pixel.B == 5)
							{
								if (tile.TileType != TileID.Containers && Main.tile[x + a, y + b - 1].TileType != TileID.Containers)
								{
									tile.WallType = (ushort)ModContent.WallType<DarkCocoonWall>();
								}
							}
							break;

						case 3:
							if (pixel.R == 165 && pixel.G == 0 && pixel.B == 255)
							{
								TileUtils.PlaceFrameImportantTiles(a + x, b + y, 5, 7, ModContent.TileType<MothWorldDoor>());
							}

							if (pixel.R == 45 && pixel.G == 49 && pixel.B == 255)
							{
								TileUtils.PlaceFrameImportantTiles(a + x, b + y, 3, 4, ModContent.TileType<FireflyPylon>());
								TEModdedPylon moddedPylon = ModContent.GetInstance<FireflyPylonTileEntity>();
								moddedPylon.Position = new Point16(a + x, b + y);

								// TODO:I need help to generate map Icon;
								ushort PylonType = (ushort)ModContent.TileType<FireflyPylon>();
								var bottom = new Point(a + x, b + y);
								for (int i = -2; i <= 2; i++)
								{
									var PylonTile = Main.tile[bottom.X + i, bottom.Y + 1];
									PylonTile.TileType = (ushort)ModContent.TileType<DarkCocoon>();
									PylonTile.HasTile = true;
									PylonTile.Slope = SlopeType.Solid;
									PylonTile.IsHalfBlock = false;
									WorldGen.TileFrame(bottom.X + i, bottom.Y + 1);
								}

								TileObject.CanPlace(bottom.X, bottom.Y, PylonType, 0, 0, out var tileObject);
								TileObject.Place(tileObject);
								TileObjectData.CallPostPlacementPlayerHook(bottom.X, bottom.Y, PylonType, 0, 0, 0, tileObject);
							}
							break;
					}
				}
			}
		});
	}

	/// <summary>
	/// 建造流萤之茧
	/// </summary>
	public static void BuildMothCave()
	{
		// Point16 AB = CocoonPos();
		int a = 230; // AB.X;
		int b = 200; // AB.Y;
		MothLand mothLand = ModContent.GetInstance<MothLand>();
		mothLand.fireflyCenterX = a + 140;
		mothLand.fireflyCenterY = b + 140;
		Main.statusText = "CocoonStart";
		ShapeTile("CocoonWorld.bmp", 0, 0, 1);
		ShapeTile("CocoonWorldWall.bmp", 0, 0, 2);
		Main.statusText = "CocoonKillStart";
		ShapeTile("CocoonSubKill.bmp", a, b, 0);
		Main.statusText = "CocoonStart";
		ShapeTile("CocoonSub.bmp", a, b, 1);
		Main.statusText = "CocoonWallStart";
		ShapeTile("CocoonSubWall.bmp", a, b, 2);
		Main.statusText = "CocoonAnotherStart";
		ShapeTile("CocoonSub.bmp", a, b, 3);
		SmoothMothTile(a, b);
		for (int x = 20; x < Main.maxTilesX - 20; x++)
		{
			for (int y = 20; y < Main.maxTilesY - 20; y++)
			{
				RandomUpdate(x, y, ModContent.TileType<DarkCocoon>());
			}
		}
	}

	public static void BuildWorldMothCave()
	{
		Point16 AB = CocoonPos();
		int a = AB.X;
		int b = AB.Y;
		MothLand mothLand = ModContent.GetInstance<MothLand>();
		mothLand.fireflyCenterX = a;
		mothLand.fireflyCenterY = b;
		Main.statusText = "CocoonKillStart";
		ShapeTile("WorldCocoonKill.bmp", a, b, 0);
		Main.statusText = "CocoonStart";
		ShapeTile("WorldCocoon.bmp", a, b, 1);
		Main.statusText = "CocoonWallStart";
		ShapeTile("WorldCocoonWall.bmp", a, b, 2);
		Main.statusText = "CocoonAnotherStart";
		ShapeTile("WorldCocoon.bmp", a, b, 3);
		SmoothMothTile(a, b);

		for (int x = 0; x < 28; x++)
		{
			for (int y = 0; y < 24; y++)
			{
				RandomUpdate(a + x, b + y, ModContent.TileType<DarkCocoon>());
			}
		}
	}

	private static int GetCrash(int PoX, int PoY)
	{
		int CrashCount = 0;
		ushort[] DangerTileType = new ushort[]
		{
			41, // 蓝地牢砖
			43, // 绿地牢砖
			44, // 粉地牢砖
			48, // 尖刺
			49, // 水蜡烛
			50, // 书
			137, // 神庙机关
			226, // 神庙石砖
			232, // 木刺
			237, // 神庙祭坛
			481, // 碎蓝地牢砖
			482, // 碎绿地牢砖
			483, // 碎粉地牢砖
		};
		for (int x = -256; x < 257; x += 8)
		{
			for (int y = -128; y < 129; y += 8)
			{
				if (Array.Exists(DangerTileType, Ttype => Ttype == Main.tile[x + PoX, y + PoY].TileType))
				{
					CrashCount++;
				}
			}
		}
		return CrashCount;
	}

	private static int GetMergeToJungle(int PoX, int PoY)
	{
		int CrashCount = 0;
		ushort[] MustHaveTileType = new ushort[]
		{
			TileID.JungleGrass, // 丛林草方块
			TileID.JunglePlants, // 丛林草
			TileID.JungleVines, // 丛林藤
			TileID.JunglePlants2, // 高大丛林草
			TileID.PlantDetritus, // 丛林花
		};
		for (int x = -256; x < 257; x += 8)
		{
			for (int y = -128; y < 129; y += 8)
			{
				if (Array.Exists(MustHaveTileType, Ttype => Ttype == Main.tile[x + PoX, y + PoY].TileType))
				{
					CrashCount++;
				}
			}
		}
		return CrashCount;
	}

	/// <summary>
	/// 获取一个不与原版地形冲突的点
	/// </summary>
	/// <returns></returns>
	private static Point16 CocoonPos()
	{
		int PoX = WorldGen.genRand.Next(300, Main.maxTilesX - 600);
		int PoY = WorldGen.genRand.Next(500, Main.maxTilesY - 700);

		while (GetCrash(PoX, PoY) > 0 || GetMergeToJungle(PoX, PoY) <= 10)
		{
			PoX = WorldGen.genRand.Next(300, Main.maxTilesX - 600);
			PoY = WorldGen.genRand.Next(500, Main.maxTilesY - 700);
		}
		return new Point16(PoX, PoY);
	}

	/// <summary>
	/// 获取一个出生地附近的平坦地面
	/// </summary>
	/// <returns></returns>
	private static Point16 ShabbyPylonPos()
	{
		int PoX = (int)(WorldGen.genRand.Next(80, 160) * (WorldGen.genRand.Next(2) - 0.5f) * 2 - 20 + Main.maxTilesX / 2);
		int PoY = 160;

		while (!IsTileSmooth(new Point(PoX, PoY)))
		{
			PoX = (int)(WorldGen.genRand.Next(80, 240) * (WorldGen.genRand.Next(2) - 0.5f) * 2 - 20 + Main.maxTilesX / 2);
			for (int y = 160; y < Main.maxTilesY / 3; y++)
			{
				if (Main.tile[PoX, y].HasTile && Main.tile[PoX, y].TileType != TileID.Trees)
				{
					PoY = y;
					break;
				}
			}
		}
		return new Point16(PoX, PoY);
	}

	/// <summary>
	/// 判定是否平坦
	/// </summary>
	/// <param name="point"></param>
	/// <param name="Width"></param>
	/// <returns></returns>
	private static bool IsTileSmooth(Point point, int Width = 22)
	{
		if (point.X > Main.maxTilesX - 20 || point.Y > Main.maxTilesY - 20 || point.X < 20 || point.Y < 20)
		{
			return false;
		}

		int x = point.X;
		int y = point.Y;
		var LeftTile = Main.tile[x, y];
		var RightTile = Main.tile[x + Width, y];
		var LeftTileUp = Main.tile[x, y - 1];
		var RightTileUp = Main.tile[x + Width, y - 1];
		if (!LeftTileUp.HasTile && !RightTileUp.HasTile)
		{
			if (LeftTile.HasTile && RightTile.HasTile)
			{
				return true;
			}
		}
		return false;
	}

	private static void SmoothMothTile(int a, int b, int width = 256, int height = 512)
	{
		for (int y = 0; y < width; y += 1)
		{
			for (int x = 0; x < height; x += 1)
			{
				if (Main.tile[x + a, y + b].TileType == (ushort)ModContent.TileType<DarkCocoon>())
				{
					Tile.SmoothSlope(x + a, y + b, false);
					WorldGen.TileFrame(x + a, y + b, true, false);
				}
				else
				{
					WorldGen.TileFrame(x + a, y + b, true, false);
				}
				WorldGen.SquareWallFrame(x + a, y + b, true);
			}
		}
	}

	public static void BuildFluorescentTree(int i, int j, int height = 0)
	{
		if (j < 30)
		{
			return;
		}

		int Height = WorldGen.genRand.Next(7, height);

		for (int g = 0; g < Height; g++)
		{
			Tile tile = Main.tile[i, j - g];
			if (g > 3)
			{
				if (WorldGen.genRand.NextBool(5))
				{
					Tile tileLeft = Main.tile[i - 1, j - g];
					tileLeft.TileType = (ushort)ModContent.TileType<FluorescentTree>();
					tileLeft.TileFrameY = 4;
					tileLeft.TileFrameX = (short)WorldGen.genRand.Next(4);
					tileLeft.HasTile = true;
				}
				if (WorldGen.genRand.NextBool(5))
				{
					Tile tileRight = Main.tile[i + 1, j - g];
					tileRight.TileType = (ushort)ModContent.TileType<FluorescentTree>();
					tileRight.TileFrameY = 5;
					tileRight.TileFrameX = (short)WorldGen.genRand.Next(4);
					tileRight.HasTile = true;
				}
			}
			if (g == 0)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 0;
				tile.TileFrameX = 0;
				tile.HasTile = true;
				continue;
			}
			if (g == 1)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = -1;
				tile.TileFrameX = 0;
				tile.HasTile = true;
				continue;
			}
			if (g == 2)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 3;
				tile.TileFrameX = (short)WorldGen.genRand.Next(4);
				tile.HasTile = true;
				continue;
			}
			if (g == Height - 1)
			{
				tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
				tile.TileFrameY = 2;
				tile.TileFrameX = 0;
				tile.HasTile = true;
				continue;
			}
			tile.TileType = (ushort)ModContent.TileType<FluorescentTree>();
			tile.TileFrameY = 1;
			tile.TileFrameX = (short)WorldGen.genRand.Next(12);
			tile.HasTile = true;
		}
	}

	public static void RandomUpdate(int i, int j, int Type)
	{
		if (Main.tile[i, j].TileType != Type || !Main.tile[i, j].HasTile)
		{
			return;
		}

		if (WorldGen.genRand.NextBool(4))
		{
			if (Main.tile[i, j].Slope == SlopeType.Solid && Main.tile[i + 1, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].Slope == SlopeType.Solid && Main.tile[i + 2, j].Slope == SlopeType.Solid && Main.tile[i - 2, j].Slope == SlopeType.Solid &&
				Main.tile[i, j + 1].Slope == SlopeType.Solid && Main.tile[i + 1, j + 1].Slope == SlopeType.Solid && Main.tile[i - 1, j + 1].Slope == SlopeType.Solid && Main.tile[i + 2, j + 1].Slope == SlopeType.Solid && Main.tile[i - 2, j + 1].Slope == SlopeType.Solid)// 树木
			{
				int MaxHeight = 0;
				for (int x = -2; x < 3; x++)
				{
					for (int y = -1; y > -30; y--)
					{
						if (j + y > 20)
						{
							if (Main.tile[i + x, j + y].HasTile || Main.tile[i + x, j + y].LiquidAmount > 3)
							{
								return;
							}
						}
						MaxHeight = -y;
					}
				}
				if (MaxHeight > 7)
				{
					BuildFluorescentTree(i, j - 1, MaxHeight);
				}
			}
		}

		if (!Main.tile[i, j - 1].HasTile && Main.tile[i, j].Slope == SlopeType.Solid && Main.tile[i, j - 1].LiquidAmount > 0)
		{
			Tile tile = Main.tile[i, j - 1];
			tile.TileType = (ushort)ModContent.TileType<LampLotus>();
			tile.HasTile = true;
			tile.TileFrameX = (short)(28 * WorldGen.genRand.Next(8));
		}
		if (WorldGen.genRand.NextBool(6))// 黑萤藤蔓
		{
			Tile t0 = Main.tile[i, j];

			Tile t2 = Main.tile[i, j + 1];
			if (t0.Slope == SlopeType.Solid && !t2.HasTile)
			{
				t2.TileType = (ushort)ModContent.TileType<BlackVine>();
				t2.HasTile = true;
				t2.TileFrameY = (short)(WorldGen.genRand.Next(6, 9) * 18);
			}
		}
		if (WorldGen.genRand.NextBool(16))// 流萤滴
		{
			int count = 0;
			for (int x = -1; x <= 1; x++)
			{
				for (int y = 1; y <= 3; y++)
				{
					Tile t0 = Main.tile[i + x, j + y];
					if (t0.HasTile)
					{
						count++;
					}

					Tile t1 = Main.tile[i + x, j + y - 1];
					if (y == 1 && (!t1.HasTile || t1.Slope != SlopeType.Solid))
					{
						count++;
					}
				}
			}
			if (count == 0)
			{
				TileUtils.PlaceFrameImportantTiles(i - 1, j + 1, 3, 3, ModContent.TileType<Tiles.Furnitures.GlowingDrop>());
			}
		}
		if (WorldGen.genRand.NextBool(16))// 巨型萤火吊
		{
			int count = 0;
			float length = 0;
			for (int x = 0; x <= 1; x++)
			{
				for (int y = 0; y <= 4; y++)
				{
					Tile t0 = Main.tile[i + x, j + y];
					if (y == 0)
					{
						if (!t0.HasTile || t0.TileType != (ushort)ModContent.TileType<DarkCocoon>() || t0.IsHalfBlock)
						{
							count++;
						}
					}
					else
					{
						if (t0.HasTile)
						{
							count++;
						}
					}
				}
			}
			if (count == 0)
			{
				for (int y = 4; y <= 80; y++)
				{
					for (int x = 0; x <= 1; x++)
					{
						Tile t0 = Main.tile[i + x, j + y];
						if (!t0.HasTile)
						{
							length += 1 / 8f;
						}
						else
						{
							y = 81;
							break;
						}
					}
				}
				if (Main.netMode != NetmodeID.Server)
				{
					LargeFireBulb.PlaceMe(i, j, (ushort)WorldGen.genRand.Next((int)Math.Floor(length)));
				}
			}
		}
		if (!Main.tile[i, j - 1].HasTile && !Main.tile[i + 1, j - 1].HasTile && !Main.tile[i - 1, j - 1].HasTile && Main.tile[i, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].Slope == SlopeType.Solid && Main.tile[i + 1, j].Slope == SlopeType.Solid)// 黑萤苣
		{
			Tile t1 = Main.tile[i, j - 1];
			Tile t2 = Main.tile[i, j - 2];
			Tile t3 = Main.tile[i, j - 3];
			for (int x = -1; x < 2; x++)
			{
				for (int y = -3; y < 4; y++)
				{
					if (Main.tile[i + x, j + y].LiquidAmount > 3)
					{
						return;
					}
				}
			}
			if (WorldGen.genRand.NextBool(2))
			{
				switch (WorldGen.genRand.Next(1, 10))
				{
					case 1:
						t1.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
						t2.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
						t1.HasTile = true;
						t2.HasTile = true;
						short numa = (short)(WorldGen.genRand.Next(0, 6) * 48);
						t1.TileFrameX = numa;
						t2.TileFrameX = numa;
						t1.TileFrameY = 16;
						t2.TileFrameY = 0;
						break;

					case 2:
						t1.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
						t2.TileType = (ushort)ModContent.TileType<BlackStarShrubSmall>();
						t1.HasTile = true;
						t2.HasTile = true;
						short num = (short)(WorldGen.genRand.Next(0, 6) * 48);
						t2.TileFrameX = num;
						t1.TileFrameX = num;
						t1.TileFrameY = 16;
						t2.TileFrameY = 0;
						break;

					case 3:
						t1.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
						t2.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
						t3.TileType = (ushort)ModContent.TileType<BlackStarShrub>();
						t1.HasTile = true;
						t2.HasTile = true;
						t3.HasTile = true;
						short num1 = (short)(WorldGen.genRand.Next(0, 6) * 72);
						t3.TileFrameX = num1;
						t2.TileFrameX = num1;
						t1.TileFrameX = num1;
						t1.TileFrameY = 32;
						t2.TileFrameY = 16;
						t3.TileFrameY = 0;
						break;

					case 4:
						t1.TileType = (ushort)ModContent.TileType<BluishGiantGentian>();
						t2.TileType = (ushort)ModContent.TileType<BluishGiantGentian>();
						t3.TileType = (ushort)ModContent.TileType<BluishGiantGentian>();
						t1.HasTile = true;
						t2.HasTile = true;
						t3.HasTile = true;
						short num2 = (short)(WorldGen.genRand.Next(0, 12) * 120);
						t3.TileFrameX = num2;
						t2.TileFrameX = num2;
						t1.TileFrameX = num2;
						t1.TileFrameY = 36;
						t2.TileFrameY = 18;
						t3.TileFrameY = 0;
						break;

					case 5:
						WorldGen.Place3x2(i - 1, j - 1, (ushort)ModContent.TileType<BlackFrenLarge>(), WorldGen.genRand.Next(3));
						break;

					case 6:
						WorldGen.Place2x2Horizontal(i, j - 1, (ushort)ModContent.TileType<BlackFren>(), WorldGen.genRand.Next(3));
						break;

					case 7:
						WorldGen.Place3x2(i - 1, j - 1, (ushort)ModContent.TileType<BlackFrenLarge>(), WorldGen.genRand.Next(3));
						break;

					case 8:
						WorldGen.Place2x2Horizontal(i, j - 1, (ushort)ModContent.TileType<BlackFren>(), WorldGen.genRand.Next(3));
						break;

					case 9:
						WorldGen.Place2x1(i - 1, j - 1, (ushort)ModContent.TileType<CocoonRock>(), WorldGen.genRand.Next(3));
						break;

					case 10:
						WorldGen.Place2x1(i - 1, j - 1, (ushort)ModContent.TileType<CocoonRock>(), WorldGen.genRand.Next(3));
						break;
				}
			}
		}
	}

	public override void PostWorldGen()
	{
		bool placed = false;
		for (int offX = -36; offX < 36; offX += 72)
		{
			for (int offY = -36; offY < 12; offY++)
			{
				if (!placed)
				{
					placed = TrySpellbookChest(Main.spawnTileX + offX, Main.spawnTileY + offY, 36);
				}
				else
				{
					break;
				}
			}
		}
	}

	private bool TrySpellbookChest(int startX, int startY, int rangeX = 16)
	{
		int[] legalTile = new int[]
		{
				TileID.Stone,
				TileID.Grass,
				TileID.Dirt,
				TileID.SnowBlock,
				TileID.IceBlock,
				TileID.ClayBlock,
				TileID.Mud,
				TileID.JungleGrass,
				TileID.Sand,
		};
		bool canPlace = true;
		int dir = -1;
		if (startY < 4 || startY > Main.maxTilesY - 1)
		{
			return false;
		}

		for (int x = startX; dir > 0 ? x <= startX + rangeX : x >= startX - rangeX; x += dir)
		{
			if (x < 0 || x > Main.maxTilesX - 4)
			{
				continue;
			}

			for (int i = 0; i < 4; i++)
			{
				for (int j = 0; j < 4; j++)
				{
					Tile tile = Framing.GetTileSafely(x + i, startY - j);
					if (WorldGen.SolidOrSlopedTile(tile))
					{
						canPlace = false;
						break;
					}
				}
			}
			if (canPlace)
			{
				for (int i = 0; i < 4; i++)
				{
					Tile tile = Framing.GetTileSafely(x + i, startY + 1);
					if (!WorldGen.SolidTile(tile) || !legalTile.Contains(tile.TileType))
					{
						canPlace = false;
						break;
					}
				}
			}
			if (canPlace)
			{
				for (int i = 0; i < 4; i++)
				{
					for (int j = 0; j < 4; j++)
					{
						WorldGen.KillTile(x + i, startY - j);
					}
				}
				WorldGen.PlaceTile(x, startY + 1, TileID.MeteoriteBrick, forced: true);
				WorldGen.PlaceTile(x + 1, startY + 1, TileID.MeteoriteBrick, forced: true);
				WorldGen.PlaceTile(x + 2, startY + 1, TileID.MeteoriteBrick, forced: true);
				WorldGen.PlaceTile(x + 3, startY + 1, TileID.MeteoriteBrick, forced: true);
				int c = WorldGen.PlaceChest(x + 1, startY, style: 49);
				Chest chest = Main.chest[c];
				if (chest != null)
				{
					chest.name = "Spellbook Demo";
					chest.item[0].SetDefaults(ModContent.ItemType<CrystalSkull>());
					chest.item[1].SetDefaults(ItemID.WaterBolt);
					chest.item[2].SetDefaults(ItemID.DemonScythe);
					chest.item[3].SetDefaults(ItemID.BookofSkulls);
					chest.item[4].SetDefaults(ItemID.CrystalStorm);
					chest.item[5].SetDefaults(ItemID.CursedFlames);
					chest.item[6].SetDefaults(ItemID.GoldenShower);
					chest.item[7].SetDefaults(ItemID.MagnetSphere);
					chest.item[8].SetDefaults(ItemID.RazorbladeTyphoon);
					chest.item[9].SetDefaults(ItemID.LunarFlareBook);
				}
				return true;
			}
			if (x <= startX - rangeX)
			{
				dir = 1;
				x = startX;
			}
		}
		return false;
	}
}
