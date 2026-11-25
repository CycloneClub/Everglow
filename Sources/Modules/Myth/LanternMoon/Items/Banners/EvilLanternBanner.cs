using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class EvilLanternBanner : BannerItemBase
{

	public override int BannerTileStyle => 3;

	public override int BannerNPCType => ModContent.NPCType<EvilLantern>();
}
