using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using static Terraria.ModLoader.ModContent;

namespace Everglow.Yggdrasil.YggdrasilTown.Water;

public class YggdrasilTownWaterStyle : ModWaterStyle
{
	public override int ChooseWaterfallStyle() => GetInstance<YggdrasilTownWaterfallStyle>().Slot;

	public override int GetSplashDust() => ModContent.DustType<YggdrasilTownWater>();

	public override int GetDropletGore() => this.Slot;

	public override void LightColorMultiplier(ref float r, ref float g, ref float b)
	{
		r = 0.8f;
		g = 0.9f;
		b = 1.01f;
	}

	public override Color BiomeHairColor() => new Color(74, 100, 153);
}
