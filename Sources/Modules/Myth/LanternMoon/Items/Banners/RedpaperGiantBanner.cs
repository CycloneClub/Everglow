using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.Tiles;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class RedpaperGiantBanner : BannerItemBase<LanternMoonBannerTile>
{
	public override int BannerTileStyle => 8;

	public override int BannerNPCType => ModContent.NPCType<RedpaperGiant>();
}
