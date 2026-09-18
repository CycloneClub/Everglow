using Everglow.Commons.DeveloperContent.Items;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.KelpCurtain.Background;
using Everglow.Yggdrasil.YggdrasilTown.Background;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class YggdrasilTownBiome : ModBiome
{
	public override int Music => GetMusic();

	public int GetMusic()
	{
		if (Mod != null)
		{
			if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
			{
				return MusicLoader.GetMusicSlot(Mod, ModAsset.Arena_BGM_Path);
			}
			if (YggdrasilTownCentralSystem.InFurnace_YggdrasilTown())
			{
				return MusicLoader.GetMusicSlot(Mod, ModAsset.FurnaceArea_BGM_Path);
			}
			return !Main.dayTime ? MusicLoader.GetMusicSlot(Mod, ModAsset.NewYggdrasilTownBGM_Path) : MusicLoader.GetMusicSlot(Mod, ModAsset.YggdrasilTownBGM_Path);
		}
		else
		{
			return 0;
		}
	}

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.YggdrasilTown_MapBackground_Mod;

	public override string MapBackground => ModAsset.YggdrasilTown_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		return BiomeActive();
	}

	public static bool CheckedBiomeCenter = false;

	public static Vector2 BiomeCenter = Vector2.zeroVector;

	/// <summary>
	/// 地形中心
	/// </summary>
	public static Vector2 GetBiomeCenter()
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			CheckedBiomeCenter = false;
			return Vector2.zeroVector;
		}
		if (!CheckedBiomeCenter)
		{
			for (int x = 20; x < Main.maxTilesX - 20; x++)
			{
				for (int y = Main.maxTilesY - 2000; y < Main.maxTilesY - 20; y++)
				{
					Tile tile = TileUtils.SafeGetTile(x, y);
					if (tile.TileType == ModContent.TileType<YggdrasilCommandBlock>())
					{
						if (tile.TileFrameX == 0)
						{
							if (tile.TileFrameY == 0)
							{
								CheckedBiomeCenter = true;
								return new Point(x, y).ToWorldCoordinates();
							}
						}
					}
				}
			}
			CheckedBiomeCenter = true;
		}
		else if (BiomeCenter == Vector2.zeroVector)
		{
			return new Vector2(487, 20711).ToWorldCoordinates();
		}
		return new Vector2(487, 20711).ToWorldCoordinates();
	}

	/// <summary>
	/// 判定是否开启地形
	/// </summary>
	/// <returns></returns>
	public static bool BiomeActive()
	{
		if (YggdrasilTownCentralSystem.InCanteen_YggdrasilTown())
		{
			return true;
		}
		if (YggdrasilTownCentralSystem.InUnion_YggdrasilTown())
		{
			return true;
		}
		if (YggdrasilTownCentralSystem.InPlayerRoom_YggdrasilTown())
		{
			return true;
		}
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			return true;
		}
		if (BiomeCenter == Vector2.zeroVector)
		{
			BiomeCenter = GetBiomeCenter();
		}
		if (Main.screenPosition.Y > BiomeCenter.Y - 18000)
		{
			if (SubworldSystem.IsActive<YggdrasilWorld>())
			{
				return true;
			}
		}
		return false;
	}

	public override void OnInBiome(Player player)
	{
		if (Main.maxRaining > 0)
		{
			Main.maxRaining = 0;
			Main.StopRain();
			Main.raining = false;
		}
		if (Main.slimeRain)
		{
			Main.StopSlimeRain();
		}
		Main.bloodMoon = false;

		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
		Town_Sky town_Sky = new Town_Sky();
		town_Sky.WorldAnchor = BiomeCenter;
		bgSystem.AddBackgroundSlide(town_Sky);

		Town_Far town_Far = new Town_Far();
		town_Far.WorldAnchor = BiomeCenter;
		bgSystem.AddBackgroundSlide(town_Far);

		Town_Middle town_Middle = new Town_Middle();
		town_Middle.WorldAnchor = BiomeCenter + new Vector2(0, -500);
		bgSystem.AddBackgroundSlide(town_Middle);

		Town_Close town_Close = new Town_Close();
		town_Close.WorldAnchor = BiomeCenter + new Vector2(0, -900);
		bgSystem.AddBackgroundSlide(town_Close);
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.YggdrasilTown_Construct"))
		{
			AddBackground(bgSystem);
		}
		base.OnInBiome(player);
	}

	public void AddBackground(BackgroundSystem bgSystem)
	{
		List<Vector2> polygon = new List<Vector2>();
		Vector2 centerPosWorld = BiomeCenter + new Vector2(228, -464);
		polygon.Add(centerPosWorld + new Vector2(-210, 0) * 16);
		polygon.Add(centerPosWorld + new Vector2(-210, -30) * 16);
		polygon.Add(centerPosWorld + new Vector2(-150, -60) * 16);
		polygon.Add(centerPosWorld + new Vector2(-50, -89) * 16);
		polygon.Add(centerPosWorld + new Vector2(-20, -89) * 16);
		polygon.Add(centerPosWorld + new Vector2(60, -50) * 16);
		polygon.Add(centerPosWorld + new Vector2(120, -65) * 16);
		polygon.Add(centerPosWorld + new Vector2(170, -60) * 16);
		polygon.Add(centerPosWorld + new Vector2(210, -20) * 16);
		polygon.Add(centerPosWorld + new Vector2(210, 0) * 16);
		List<Point> bgArea = TileUtils.GetPolygonAreaOfTilePos(polygon);

		YggdrasilTown_Construct ytc = new YggdrasilTown_Construct();
		ytc.WorldAnchor = centerPosWorld + new Vector2(32, 704);
		ytc.TileAnchor = centerPosWorld.ToTileCoordinates() + new Point(-14, 29);
		ytc.BgTiles = bgArea;
		bgSystem.AddBackgroundSlide(ytc);

		GiantFurnace_Construct gfc = new GiantFurnace_Construct();
		gfc.WorldAnchor = centerPosWorld + new Vector2(183, 63) * 16 + new Vector2(4, 24);
		gfc.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 134, (int)centerPosWorld.Y / 16 + 32, 92, 60);
		gfc.TileAnchor = centerPosWorld.ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(gfc);

		GiantFurnace_Construct_far gfcf = new GiantFurnace_Construct_far();
		gfcf.WorldAnchor = centerPosWorld + new Vector2(183, 63) * 16 + new Vector2(-264, -164);
		gfcf.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 106, (int)centerPosWorld.Y / 16 + 34, 112, 60);
		gfcf.TileAnchor = centerPosWorld.ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(gfcf);

		GiantFurnace_Construct_sky gfcs = new GiantFurnace_Construct_sky();
		gfcs.WorldAnchor = centerPosWorld + new Vector2(0, -240);
		gfcs.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 106, (int)centerPosWorld.Y / 16 + 4, 112, 90);
		gfcs.TileAnchor = centerPosWorld.ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(gfcs);

		FurnaceScoreShop fSS = new FurnaceScoreShop();
		fSS.WorldAnchor = centerPosWorld + new Vector2(-124, 920);
		fSS.BgTiles = TileUtils.GetAABBAreaOfTile((int)centerPosWorld.X / 16 + 102, (int)centerPosWorld.Y / 16 + 78, 20, 14);
		fSS.TileAnchor = centerPosWorld.ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(fSS);
	}
}
