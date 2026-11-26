using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class AncientPhantom_MagicBanner : BannerItemBase<YggdrasilTownBannerTile>
{
	public override int BannerTileStyle => 1;

	public override int BannerNPCType => ModContent.NPCType<AncientPhantom_Magic>();
}
