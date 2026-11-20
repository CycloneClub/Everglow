using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class WizardLanternBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 9;

	public override int BannerNPCType => ModContent.NPCType<WizardLantern>();
}
