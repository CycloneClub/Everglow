using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Background;
using Everglow.Yggdrasil.KelpCurtain.Water;

namespace Everglow.Yggdrasil.KelpCurtain.Biomes;

public class IsleOfBloomBiome : ModBiome
{
	public List<Vector2> Polygon_Bound = new List<Vector2>();

	/// <summary>
	/// TODO: BGM
	/// </summary>
	public override int Music => YggdrasilContent.QuickMusic(ModAsset.KelpCurtainBGM_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => ModAsset.IsleOfBloomIcon_Mod;

	public override string MapBackground => ModAsset.KelpCurtain_MapBackground_Mod;

	/// <summary>
	/// TODO:WaterStyle
	/// </summary>
	public override ModWaterStyle WaterStyle => ModContent.GetInstance<KelpCurtainWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override bool IsBiomeActive(Player player)
	{
		bool flag1 = player.InModBiome<KelpCurtainBiome>();
		if (flag1 && Polygon_Bound.Count <= 0)
		{
			Polygon_Bound.Add(new Vector2(865, 18715));
			Polygon_Bound.Add(new Vector2(865, 18655));
			Polygon_Bound.Add(new Vector2(900, 18620));
			Polygon_Bound.Add(new Vector2(940, 18480));
			Polygon_Bound.Add(new Vector2(900, 18450));
			Polygon_Bound.Add(new Vector2(470, 18460));
			Polygon_Bound.Add(new Vector2(495, 18630));
			Polygon_Bound.Add(new Vector2(475, 18715));
		}
		bool flag2 = MathUtils.IntersectsPolygonAABB(Polygon_Bound, Main.screenPosition / 16f, (Main.screenPosition + new Vector2(Main.screenWidth, Main.screenHeight)) / 16f);
		return flag1 && flag2;
	}

	public override void OnInBiome(Player player)
	{
		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
		if (!bgSystem.HasBgSlide("Everglow.Yggdrasil.KelpCurtain.Background.IsleOfBloom_Underground_sky"))
		{
			AddBackground(bgSystem);
		}
		base.OnInBiome(player);
	}

	public void AddBackground(BackgroundSystem bgSystem)
	{
		List<Vector2> polygon = new List<Vector2>();
		Vector2 centerPosWorld = YggdrasilWorld.KelpCurtain_IsleOfBloom_CaveCenter.ToWorldCoordinates();
		polygon.Add(centerPosWorld + new Vector2(0, -12) * 16);
		polygon.Add(centerPosWorld + new Vector2(130, -4) * 16);
		polygon.Add(centerPosWorld + new Vector2(130, 4) * 16);
		polygon.Add(centerPosWorld + new Vector2(0, 12) * 16);
		polygon.Add(centerPosWorld + new Vector2(-130, -4) * 16);
		polygon.Add(centerPosWorld + new Vector2(-130, 4) * 16);
		List<Point> bgArea = TileUtils.GetPolygonAreaOfTilePos(polygon);

		IsleOfBloom_Underground_close iob_bg_close = new IsleOfBloom_Underground_close();
		iob_bg_close.WorldAnchor = centerPosWorld;
		iob_bg_close.BgTiles = bgArea;
		bgSystem.AddBackgroundSlide(iob_bg_close);

		IsleOfBloom_Underground_middle iob_bg_middle = new IsleOfBloom_Underground_middle();
		iob_bg_middle.WorldAnchor = centerPosWorld;
		iob_bg_middle.BgTiles = bgArea;
		bgSystem.AddBackgroundSlide(iob_bg_middle);

		IsleOfBloom_Underground_far iob_bg_far = new IsleOfBloom_Underground_far();
		iob_bg_far.WorldAnchor = centerPosWorld;
		iob_bg_far.BgTiles = bgArea;
		bgSystem.AddBackgroundSlide(iob_bg_far);

		IsleOfBloom_Underground_sky iob_bg_sky = new IsleOfBloom_Underground_sky();
		iob_bg_sky.WorldAnchor = centerPosWorld;
		bgSystem.AddBackgroundSlide(iob_bg_sky);
	}
}
