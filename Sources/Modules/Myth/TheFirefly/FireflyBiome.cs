using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Backgrounds;
using Everglow.Myth.TheFirefly.Water;

namespace Everglow.Myth.TheFirefly;

public class FireflyBiomeBG : ModSurfaceBackgroundStyle
{
	public override void ModifyFarFades(float[] fades, float transitionSpeed)
	{
	}
}

public class FireflyBiome : ModBiome
{
	public override int Music => MusicLoader.GetMusicSlot(ModIns.Mod, ModAsset.MothBiome_Path);

	public override SceneEffectPriority Priority => SceneEffectPriority.Environment;

	public override string BestiaryIcon => ModAsset.FireflyIcon_Mod;

	public override string BackgroundPath => base.BackgroundPath;

	public override string MapBackground => ModAsset.FireflyBiomeInside_Background_Mod;

	public override ModWaterStyle WaterStyle => ModContent.GetInstance<FireflyWaterStyle>();

	public override Color? BackgroundColor => base.BackgroundColor;

	public override ModSurfaceBackgroundStyle SurfaceBackgroundStyle => ModContent.GetInstance<FireflyBiomeBG>();

	public override ModUndergroundBackgroundStyle UndergroundBackgroundStyle => ModContent.Find<ModUndergroundBackgroundStyle>("Everglow/MothUndergroundBackground");

	public override bool IsBiomeActive(Player player)
	{
		return MothBackground.BiomeActive();
	}

	public override void OnInBiome(Player player)
	{
		base.OnInBiome(player);
	}
}
