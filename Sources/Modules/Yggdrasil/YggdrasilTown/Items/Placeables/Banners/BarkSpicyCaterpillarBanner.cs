using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class BarkSpicyCaterpillarBanner : BannerItemBase
{
	public override int BannerTileStyle => 18;

	public override int BannerNPCType => ModContent.NPCType<BarkSpicyCaterpillar>();
}
