using Everglow.Myth.Common;
using Everglow.Myth.TheTusk.Backgrounds;
using Everglow.Myth.TheTusk.Water;
using Everglow.Myth.TheTusk.WorldGeneration;
using Terraria.Graphics.Capture;

namespace Everglow.Myth.TheTusk;

public class TuskBiomeBG : ModSurfaceBackgroundStyle
{
	public override void ModifyFarFades(float[] fades, float transitionSpeed)
	{
	}
}

public class TuskBiome : ModBiome
{
	public override int Music => MythContent.QuickMusic("TuskBiome");

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => "Everglow/Myth/TheTusk/TuskIcon";

	public override string BackgroundPath => base.BackgroundPath;

	public override string MapBackground => "Everglow/Myth/TheTusk/Backgrounds/TuskBackground_Inside";

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<TuskWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override CaptureBiome.TileColorStyle TileColorStyle => CaptureBiome.TileColorStyle.Crimson;

	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<TuskSurfaceBackgroundStyle>();
	// public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Everglow/MothUndergroundBackground");

	public override void SetStaticDefaults()
	{
		// TODO:×çÖäÖ®ò¢
	}

	public override bool IsBiomeActive(Player player)
	{
		bool b1 = TuskGen.TuskLandActive();
		return b1;
	}

	public override void OnInBiome(Player player)
	{
		base.OnInBiome(player);
	}
}
