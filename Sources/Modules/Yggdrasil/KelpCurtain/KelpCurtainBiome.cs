using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Background;
using MonoMod.Core.Platforms;
using SubworldLibrary;

namespace Everglow.Yggdrasil.KelpCurtain;

public class KelpCurtainBiome : ModBiome
{
	/// <summary>
	/// The stratumbound of 2nd and 3rd stratum.
	/// </summary>
	public static List<Point> StratumBoundCurve = new List<Point>();

	public override int Music => YggdrasilContent.QuickMusic(ModAsset.KelpCurtainBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => ModAsset.KelpCurtainIcon_Mod;

	public override string BackgroundPath => base.BackgroundPath;

	public override string MapBackground => ModAsset.KelpCurtain_MapBackground_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<Water.KelpCurtainWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override void Load()
	{
		base.Load();
	}

	public override bool IsBiomeActive(Player player)
	{
		if (SubworldSystem.IsActive<YggdrasilWorld>())
		{
			if (Main.screenPosition.Y > Main.maxTilesY * 0.72f * 16 && Main.screenPosition.Y < Main.maxTilesY * 0.9f * 16)
			{
				if (player.Center.X >= FindClosestStratumBoundPointX(player) * 16)
				{
					return true;
				}
			}
		}
		return false;
	}

	public float FindClosestStratumBoundPointX(Player player)
	{
		if (StratumBoundCurve.Count > 0)
		{
			float minY = int.MaxValue;
			float currentX = -1;
			foreach (var point in StratumBoundCurve)
			{
				float deltaY = MathF.Abs((float)point.Y * 16 - player.Center.Y);
				if (deltaY < minY)
				{
					minY = deltaY;
					currentX = point.X;
				}
			}
			return currentX;
		}
		else
		{
			return -1;
		}
	}

	public static float FindClosestStratumBoundPointX(float checkTileY)
	{
		if (StratumBoundCurve.Count > 0)
		{
			float minY = int.MaxValue;
			float currentX = -1;
			foreach (var point in StratumBoundCurve)
			{
				float deltaY = MathF.Abs((float)point.Y - checkTileY);
				if (deltaY < minY)
				{
					minY = deltaY;
					currentX = point.X;
				}
			}
			return currentX;
		}
		else
		{
			return -1;
		}
	}

	public override void OnInBiome(Player player)
	{
		YggdrasilEnvironmentLightManager.LightingScene = YggdrasilScene.KelpCurtain;
		Vector2 biomeCenter = new Vector2(9000, 157000);
		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();

		KelpCurtainSky kelpCurtainSky = new KelpCurtainSky();
		kelpCurtainSky.WorldAnchor = biomeCenter;
		bgSystem.AddBackgroundSlide(kelpCurtainSky);

		KelpCurtainFar kelpCurtainFar = new KelpCurtainFar();
		kelpCurtainFar.WorldAnchor = biomeCenter;
		bgSystem.AddBackgroundSlide(kelpCurtainFar);

		KelpCurtainMiddle kelpCurtainMiddle = new KelpCurtainMiddle();
		kelpCurtainMiddle.WorldAnchor = biomeCenter;
		bgSystem.AddBackgroundSlide(kelpCurtainMiddle);

		KelpCurtainMiddleClose kelpCurtainMiddleClose = new KelpCurtainMiddleClose();
		kelpCurtainMiddleClose.WorldAnchor = biomeCenter;
		bgSystem.AddBackgroundSlide(kelpCurtainMiddleClose);

		KelpCurtainClose kelpCurtainClose = new KelpCurtainClose();
		kelpCurtainClose.WorldAnchor = biomeCenter;
		bgSystem.AddBackgroundSlide(kelpCurtainClose);

		base.OnInBiome(player);
	}
}
