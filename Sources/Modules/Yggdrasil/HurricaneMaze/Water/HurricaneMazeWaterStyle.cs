using static Terraria.ModLoader.ModContent;
using Everglow.Yggdrasil.HurricaneMaze.Dusts;

namespace Everglow.Yggdrasil.HurricaneMaze.Water;

public class HurricaneMazeWaterStyle : ModWaterStyle
{
	public override int ChooseWaterfallStyle() => GetInstance<HurricaneMazeWaterfallStyle>().Slot;

	public override int GetSplashDust() => ModContent.DustType<HurricaneMazeWater>();

	public override int GetDropletGore() => Slot;

	public override void LightColorMultiplier(ref float r, ref float g, ref float b)
	{
		r = 0.8f;
		g = 0.9f;
		b = 1.01f;
	}

	public override Color BiomeHairColor() => new Color(0, 114, 161);
}
