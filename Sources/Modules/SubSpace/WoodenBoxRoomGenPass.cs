using Everglow.Commons.TileHelper;
using Terraria.IO;
using Terraria.WorldBuilding;

namespace Everglow.SubSpace;

public class WoodenBoxRoomGenPass : GenPass
{
	public WoodenBoxRoomGenPass()
		: base("Wooden Box", 500)
	{
	}

	/// <summary>
	/// 预设的房间
	/// </summary>
	public static string MapIOPathOfNewRoom;

	public static Point ModifedSpawnPos = new Point(-5, -5);

	public static Point AnchorForMapIO = new Point(-5, -5);

	public static Action RoomGen;

	public override void ApplyPass(GenerationProgress progress, GameConfiguration configuration)
	{
		Main.statusText = "Test";
		BuildWoodenRoom();
		if (RoomWorld.SpawnPos != default)
		{
			if (RoomWorld.SpawnPos != new Point(-5, -5))
			{
				if (RoomWorld.SpawnPos.X is > 5 and < 295 && RoomWorld.SpawnPos.Y is > 5 and < 295)
				{
					RoomWorld.OldSpawnPos.X = Main.spawnTileX;
					RoomWorld.OldSpawnPos.Y = Main.spawnTileY;
					Main.spawnTileX = RoomWorld.SpawnPos.X;
					Main.spawnTileY = RoomWorld.SpawnPos.Y;
				}
			}
			Main.LocalPlayer.Center = RoomWorld.SpawnPos.ToWorldCoordinates();
			RoomWorld.SpawnPos = default;
		}
	}

	/// <summary>
	/// 建造一个木制世界房间
	/// </summary>
	public static void BuildWoodenRoom()
	{
		// 如果没有记录就开一个新房间
		if (!ReadWorldSave())
		{
			if (MapIOPathOfNewRoom == string.Empty)
			{
				// 获取对应层级的世界材料
				GetLayerMaterial();

				// 用木头填满世界
				for (int x = 20; x < Main.maxTilesX - 20; x++)
				{
					for (int y = 20; y < Main.maxTilesY - 20; y++)
					{
						Tile tile = Main.tile[x, y];
						tile.wall = (ushort)RoomWallType;
						if (!(Math.Abs(x - Main.maxTilesX / 2) < 10 && Math.Abs(y - Main.maxTilesY / 2) < 10))
						{
							tile.TileType = (ushort)RoomTileType;
							tile.HasTile = true;
						}
						else
						{
							tile.HasTile = false;
						}
					}
				}
			}

			// 有的话就启用
			else
			{
				QuickBuildInside();
				MapIOPathOfNewRoom = string.Empty;
				if (ModifedSpawnPos != new Point(-5, -5))
				{
					if (ModifedSpawnPos.X is > 5 and < 295 && ModifedSpawnPos.Y is > 5 and < 295)
					{
						Main.spawnTileX = ModifedSpawnPos.X;
						Main.spawnTileY = ModifedSpawnPos.Y;
					}
				}
			}
			// 如果没有预设的房间就搓一个房间
			if (RoomGen == default)
			{
				// BuildWoodenSquareRoom();
			}
			// 有的话就启用
			else
			{
				RoomGen();
				RoomGen = default;
			}
		}
	}

	/// <summary>
	/// 建造一个固定好模板的房间
	/// </summary>
	public static void BuildMapIORoom()
	{
		if (QuickBuild(5, 5, MapIOPathOfNewRoom))
		{
			MapIOPathOfNewRoom = string.Empty;
		}
	}

	/// <summary>
	/// 读取已有部分
	/// </summary>
	/// <returns></returns>
	public static bool ReadWorldSave()
	{
		string path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow_RoomWorlds", Main.worldID.ToString());
		if (RoomWorld.OriginalWorld != null)
		{
			path = Path.Combine(Main.SavePath, "Mods", "ModDatas", "Everglow_RoomWorlds", Main.worldID.ToString(), RoomWorld.OriginalWorld.Name.ToString());
		}
		string readPath = path + "\\RoomMapIO" + RoomWorld.AnchorWorldCoordinate.X.ToString() + "_" + RoomWorld.AnchorWorldCoordinate.Y.ToString() + "_" + RoomWorld.LayerDepth + ".mapio";
		return QuickBuild(5, 5, readPath);
	}

	/// <summary>
	/// 建造内部已有的
	/// </summary>
	public static void QuickBuildInside()
	{
		int x = 5;
		int y = 5;
		if (AnchorForMapIO != new Point(-5, -5))
		{
			if (AnchorForMapIO.X is > 5 and < 295 && AnchorForMapIO.Y is > 5 and < 295)
			{
				(x, y) = (AnchorForMapIO.X, AnchorForMapIO.Y);
			}
		}

		// 用木头填满世界
		for (int x0 = 20; x0 < Main.maxTilesX - 20; x0++)
		{
			for (int y0 = 20; y0 < Main.maxTilesY - 20; y0++)
			{
				Tile tile = Main.tile[x0, y0];
				tile.wall = (ushort)RoomWallType;
				if (!(Math.Abs(x0 - Main.maxTilesX / 2) < 10 && Math.Abs(y0 - Main.maxTilesY / 2) < 10))
				{
					tile.TileType = (ushort)RoomTileType;
					tile.HasTile = true;
				}
				else
				{
					tile.HasTile = false;
				}
			}
		}
		var mapIO = new MapIO(x, y);

		if (MapIOPathOfNewRoom is not null && MapIOPathOfNewRoom != string.Empty)
		{
			mapIO.Read(ModIns.Mod.GetFileStream(MapIOPathOfNewRoom));
		}
	}

	/// <summary>
	/// 建造规定路径的建筑
	/// </summary>
	/// <param name="x"></param>
	/// <param name="y"></param>
	/// <param name="pathName"></param>
	/// <returns></returns>
	public static bool QuickBuild(int x, int y, string pathName)
	{
		var mapIO = new MapIO(x, y);

		if (!File.Exists(pathName))
		{
			return false;
		}

		mapIO.Read(pathName);

		var it = mapIO.GetEnumerator();
		while (it.MoveNext())
		{
			WorldGen.SquareTileFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
			WorldGen.SquareWallFrame(it.CurrentCoord.X, it.CurrentCoord.Y);
		}
		return true;
	}

	public static void GetLayerMaterial()
	{
		switch (RoomWorld.LayerDepth)
		{
			case 1:
				RoomTileType = TileID.WoodBlock;
				RoomWallType = WallID.Wood;
				break;
			case 2:
				RoomTileType = TileID.RichMahogany;
				RoomWallType = WallID.RichMaogany;
				break;
			case 3:
				RoomTileType = TileID.PalmWood;
				RoomWallType = WallID.PalmWood;
				break;
			case 4:
				RoomTileType = TileID.Shadewood;
				RoomWallType = WallID.Shadewood;
				break;
			default:
				RoomTileType = TileID.BorealWood;
				RoomWallType = WallID.BorealWood;
				break;
		}
	}

	public static int RoomTileType;
	public static int RoomWallType;
}
