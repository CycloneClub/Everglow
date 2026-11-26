using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;
using Everglow.Myth.TheFirefly.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class FireflyPiranhaInfectedBanner : BannerItemBase<TheFireflyBannerTile>
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 3;

	public override int BannerNPCType => ModContent.NPCType<FireflyPiranhaInfected>();
}
