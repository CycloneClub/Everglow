using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class FireflyPiranhaInfectedBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 0;

	public override int BannerNPCType => ModContent.NPCType<FireflyPiranhaInfected>();
}
