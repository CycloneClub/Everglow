using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class PurpleBombCaterpillarBanner : BannerItemBase
{
	public override int BannerTileStyle => 30;

	public override int BannerNPCType => ModContent.NPCType<PurpleBombCaterpillar>();
}
