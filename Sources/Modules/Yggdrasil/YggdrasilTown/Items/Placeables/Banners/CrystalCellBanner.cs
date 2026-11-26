using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CrystalCellBanner : BannerItemBase<YggdrasilTownBannerTile>
{
	public override int BannerTileStyle => 7;

	public override int BannerNPCType => ModContent.NPCType<CrystalCell>();
}
