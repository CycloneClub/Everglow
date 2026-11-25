using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class AncientPhantom_BowBanner : BannerItemBase
{
	public override int BannerTileStyle => 14;

	public override int BannerNPCType => ModContent.NPCType<AncientPhantom_Bow>();
}
