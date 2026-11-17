using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.WorldGeneration;
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

	public bool IsTheCenterRelic = false;

	private const float RelicBiomeRadius = 400f;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		if (!SubworldSystem.IsActive<YggdrasilWorld>())
		{
			IsTheCenterRelic = false;
			return false;
		}

		Vector2 position = player.Center;
		IsTheCenterRelic = (position - YggdrasilTownGeneration.TwilightRelicCenter.ToWorldCoordinates()).Length() < RelicBiomeRadius * 16f;
		if (IsTheCenterRelic)
		{
			return true;
		}

		foreach (YggdrasilWorld.TwilightBubble bubble in YggdrasilWorld.TwilightBubbles)
		{
			if ((position - bubble.Center.ToWorldCoordinates()).Length() < bubble.Radius * 16f)
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
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.TwilightSky"))
		{
			Vector2 veryCloseAnchor = YggdrasilTownBackground.Stratum1Center + new Vector2(0, 2300);
			if (!IsTheCenterRelic)
			{
				foreach (YggdrasilWorld.TwilightBubble bubble in YggdrasilWorld.TwilightBubbles)
				{
					if ((player.Center - bubble.Center.ToWorldCoordinates()).Length() < bubble.Radius * 16f)
					{
						veryCloseAnchor = bubble.Center.ToWorldCoordinates() + new Vector2(0, bubble.Radius * 5);
						break;
					}
				}
			}

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

			TwilightVeryClose veryClose = new TwilightVeryClose { WorldAnchor = veryCloseAnchor };
			bgSystem.AddBackgroundSlide(veryClose);

			TwilightVeryClose_Glow veryCloseGlow = new TwilightVeryClose_Glow { WorldAnchor = veryCloseAnchor };
			bgSystem.AddBackgroundSlide(veryCloseGlow);
		}

		base.OnInBiome(player);
	}
}
