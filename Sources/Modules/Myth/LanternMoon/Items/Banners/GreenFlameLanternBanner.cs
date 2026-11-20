using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class GreenFlameLanternBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 4;

	public override int BannerNPCType => ModContent.NPCType<GreenFlameLantern>();
}
