using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class EucaLeafyBanner : BannerItemBase
{
	public override int BannerTileStyle => 24;

	public override int BannerNPCType => ModContent.NPCType<EucaLeafy>();
}
