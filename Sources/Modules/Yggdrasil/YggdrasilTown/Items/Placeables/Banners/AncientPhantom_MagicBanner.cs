using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class AncientPhantom_MagicBanner : BannerItemBase
{
	public override int BannerTileStyle => 15;

	public override int BannerNPCType => ModContent.NPCType<AncientPhantom_Magic>();
}
