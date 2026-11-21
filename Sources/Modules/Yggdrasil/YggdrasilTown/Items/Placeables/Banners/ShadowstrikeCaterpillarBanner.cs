using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class ShadowstrikeCaterpillarBanner : BannerItemBase
{
	public override int BannerTileStyle => 32;

	public override int BannerNPCType => ModContent.NPCType<ShadowstrikeCaterpillar>();
}
