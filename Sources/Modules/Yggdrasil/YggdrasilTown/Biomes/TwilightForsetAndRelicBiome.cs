using Everglow.Yggdrasil.Common;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Background;
using SubworldLibrary;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class TwilightForsetAndRelic : ModBiome
{
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.NewYggdrasilTownBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.LampWood_MapBackground_Mod;

	public override string MapBackground => ModAsset.LampWood_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => base.UndergroundBackgroundStyle;

	public override Color? BackgroundColor => base.BackgroundColor;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			return false;
		}
		return (new Point(Main.maxTilesX / 2, 20000).ToWorldCoordinates() - (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f)).Length() < 5000;
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
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.TwilightSky"))
		{
			TwilightSky sky = new TwilightSky { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(sky);

			TwilightFar far = new TwilightFar { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(far);

			TwilightMiddleFar middleFar = new TwilightMiddleFar { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(middleFar);

			TwilightMiddle middle = new TwilightMiddle { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(middle);

			TwilightMiddleClose middleClose = new TwilightMiddleClose { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(middleClose);

			TwilightClose close = new TwilightClose { WorldAnchor = YggdrasilTownBackground.Stratum1Center };
			bgSystem.AddBackgroundSlide(close);
		}

		base.OnInBiome(player);
	}
}
