using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class Dendroid_NormalBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 11;

	public override int BannerNPCType => ModContent.NPCType<Dendroid_Normal>();
}
