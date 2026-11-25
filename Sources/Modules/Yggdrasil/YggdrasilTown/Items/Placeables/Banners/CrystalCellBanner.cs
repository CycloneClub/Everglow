using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CrystalCellBanner : BannerItemBase
{
	public override int BannerTileStyle => 21;

	public override int BannerNPCType => ModContent.NPCType<CrystalCell>();
}
