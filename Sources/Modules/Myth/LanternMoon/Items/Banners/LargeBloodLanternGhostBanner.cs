using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class LargeBloodLanternGhostBanner : BannerItemBase<LanternMoonBannerTile>
{

	public override int BannerTileStyle => 5;

	public override int BannerNPCType => ModContent.NPCType<LargeBloodLanternGhost>();
}
