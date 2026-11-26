using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class RedpaperGiantBanner : BannerItemBase<LanternMoonBannerTile>
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 8;

	public override int BannerNPCType => ModContent.NPCType<RedpaperGiant>();
}
