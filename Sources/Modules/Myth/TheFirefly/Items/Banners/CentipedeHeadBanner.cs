using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;
using Everglow.Myth.TheFirefly.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class CentipedeHeadBanner : BannerItemBase<TheFireflyBannerTile>
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 0;

	public override int BannerNPCType => ModContent.NPCType<CentipedeHead>();
}
