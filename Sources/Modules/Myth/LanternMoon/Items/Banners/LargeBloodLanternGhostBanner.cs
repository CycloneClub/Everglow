using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class LargeBloodLanternGhostBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 5;

	public override int BannerNPCType => ModContent.NPCType<LargeBloodLanternGhost>();
}
