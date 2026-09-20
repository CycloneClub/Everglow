using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Background;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class MidnightBayouBiome : ModBiome
{
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.NewYggdrasilTownBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.MidnightBayou_MapBackground_Mod;

	public override string MapBackground => ModAsset.MidnightBayou_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => base.UndergroundBackgroundStyle;

	public override Color? BackgroundColor => base.BackgroundColor;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		return InMidnightBayou(Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f);
	}

	public static bool InMidnightBayou(Vector2 position)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return false;
		}
		return (new Point(1395, Main.maxTilesY - 405).ToWorldCoordinates() - position).Length() < 5000;
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
		if (Main.rand.NextBool(15))
		{
			Vector2 anchorPos = YggdrasilTownBackground.OriginPylonCenter;
			Texture2D tex = ModAsset.MidnightBayouBackgroundCloud_0.Value;
			switch (Main.rand.Next(4))
			{
				case 0:
					tex = ModAsset.MidnightBayouBackgroundCloud_0.Value;
					break;
				case 1:
					tex = ModAsset.MidnightBayouBackgroundCloud_1.Value;
					break;
				case 2:
					tex = ModAsset.MidnightBayouBackgroundCloud_2.Value;
					break;
				case 3:
					tex = ModAsset.MidnightBayouBackgroundCloud_3.Value;
					break;
			}

			var cloud = new MidnightBayouBackgroundCloud
			{
				Position = anchorPos + new Vector2(Main.rand.Next(-1000, 3000), Main.rand.NextFloat(850, 960)),
				Velocity = new Vector2(Main.windSpeedCurrent * 0.2f, 0),
				Fade = 0,
				AnchorPos = anchorPos,
				Scale = Main.rand.NextFloat(0.85f, 1.15f),
				MaxTime = Main.rand.NextFloat(300, 1500),
				CloudTexture = tex,
			};
			Ins.VFXManager.Add(cloud);
		}
		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Tiles.PylonSquare"))
		{
			MidnightBayou_Sky m_Sky = new MidnightBayou_Sky();
			m_Sky.WorldAnchor = YggdrasilTownBackground.OriginPylonCenter;
			bgSystem.AddBackgroundSlide(m_Sky);

			MidnightBayou_Middle_0 m_Middle_0 = new MidnightBayou_Middle_0();
			m_Middle_0.WorldAnchor = YggdrasilTownBackground.OriginPylonCenter;
			bgSystem.AddBackgroundSlide(m_Middle_0);

			MidnightBayou_Middle_1 m_Middle_1 = new MidnightBayou_Middle_1();
			m_Middle_1.WorldAnchor = YggdrasilTownBackground.OriginPylonCenter + new Vector2(0, 900);
			bgSystem.AddBackgroundSlide(m_Middle_1);

			MidnightBayou_Middle_2 m_Middle_2 = new MidnightBayou_Middle_2();
			m_Middle_2.WorldAnchor = YggdrasilTownBackground.OriginPylonCenter + new Vector2(0, 3100);
			bgSystem.AddBackgroundSlide(m_Middle_2);

			MidnightBayou_Close m_Close = new MidnightBayou_Close();
			m_Close.WorldAnchor = YggdrasilTownBackground.OriginPylonCenter + new Vector2(1200, 2700);
			bgSystem.AddBackgroundSlide(m_Close);
			AddBackground(bgSystem);
		}
		base.OnInBiome(player);
	}

	public void AddBackground(BackgroundSystem bgSystem)
	{
		Point center = YggdrasilTownGeneration.YggdrasilTownTopLeft + new Point(1195, 134);
		PylonSquare pSquare = new PylonSquare();
		pSquare.WorldAnchor = center.ToWorldCoordinates() + new Vector2(-816, 370);
		pSquare.BgTiles = TileUtils.GetAABBAreaOfTile(center.X, center.Y, 100, 60);
		pSquare.TileAnchor = (YggdrasilTownBiome.BiomeCenter + new Vector2(228, -464)).ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(pSquare);

		PylonSquare_Chain pSC = new PylonSquare_Chain();
		center = YggdrasilTownGeneration.YggdrasilTownTopLeft + new Point(1214, 68);
		pSC.WorldAnchor = center.ToWorldCoordinates() + new Vector2(48, 370);
		pSC.BgTiles = TileUtils.GetAABBAreaOfTile(center.X, center.Y, 5, 120);
		pSC.TileAnchor = (YggdrasilTownBiome.BiomeCenter + new Vector2(228, -464)).ToTileCoordinates() + new Point(-14, 29);
		bgSystem.AddBackgroundSlide(pSC);

		PylonSquare_Chain pSC1 = new PylonSquare_Chain();
		center = YggdrasilTownGeneration.YggdrasilTownTopLeft + new Point(1270, 68);
		pSC1.WorldAnchor = center.ToWorldCoordinates() + new Vector2(48, 370);
		pSC1.BgTiles = TileUtils.GetAABBAreaOfTile(center.X, center.Y, 5, 120);
		pSC1.TileAnchor = (YggdrasilTownBiome.BiomeCenter + new Vector2(228, -464)).ToTileCoordinates() + new Point(-14, 29);
		pSC1.Texture = ModAsset.PylonSquare_Chain_Flip.Value;
		bgSystem.AddBackgroundSlide(pSC1);
	}
}
