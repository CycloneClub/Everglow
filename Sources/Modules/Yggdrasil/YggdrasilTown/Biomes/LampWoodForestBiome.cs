using Everglow.Yggdrasil.Common;
using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.YggdrasilTown.Background;

namespace Everglow.Yggdrasil.YggdrasilTown.Biomes;

public class LampWoodForest : ModBiome
{
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.NewYggdrasilTownBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.BossMedium;

	public override string BestiaryIcon => ModAsset.YggdrasilTownIcon_Mod;

	public override string BackgroundPath => ModAsset.LampWood_MapBackground_Mod;

	public override string MapBackground => ModAsset.LampWood_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.YggdrasilTownWaterStyle>();

	public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => base.UndergroundBackgroundStyle;

	public override Color? BackgroundColor => base.BackgroundColor;

	/// <summary>
	/// LampWoodAnchor Y
	/// </summary>
	public float LampWoodCenterY = 0;

	public int OldTileCount = 0;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		int tileCount = ModContent.GetInstance<YggdrasilBiomeTileCounter>().DarkForestGrassCount;
		if (tileCount > 100 && OldTileCount < tileCount)
		{
			LampWoodCenterY = Main.screenPosition.Y;
		}
		OldTileCount = tileCount;
		return tileCount > 150;
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
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.YggdrasilTown.Background.LampWoodSky"))
		{
			Vector2 lampWoodAnchor = new Vector2(YggdrasilTownBiome.GetBiomeCenter().X, LampWoodCenterY);

			LampWoodSky sky = new LampWoodSky { WorldAnchor = lampWoodAnchor };
			bgSystem.AddBackgroundSlide(sky);

			LampWoodFar far = new LampWoodFar { WorldAnchor = lampWoodAnchor + new Vector2(6000, -16000) };
			bgSystem.AddBackgroundSlide(far);

			LampWoodMiddle middle = new LampWoodMiddle { WorldAnchor = lampWoodAnchor + new Vector2(0, -3000) };
			bgSystem.AddBackgroundSlide(middle);

			LampWoodClose_Middle closeMiddle = new LampWoodClose_Middle { WorldAnchor = lampWoodAnchor + new Vector2(4100, 3000) };
			bgSystem.AddBackgroundSlide(closeMiddle);

			LampWoodClose close = new LampWoodClose { WorldAnchor = lampWoodAnchor + new Vector2(0, 1000) };
			bgSystem.AddBackgroundSlide(close);
		}

		base.OnInBiome(player);
	}
}
