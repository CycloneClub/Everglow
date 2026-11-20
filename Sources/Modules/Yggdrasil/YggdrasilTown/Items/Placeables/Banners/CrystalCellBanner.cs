using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CrystalCellBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 21;

	public override int BannerNPCType => ModContent.NPCType<CrystalCell>();
}
