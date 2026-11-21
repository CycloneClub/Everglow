using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CarapaceCaterpillarBanner : BannerItemBase
{
	public override int BannerTileStyle => 19;

	public override int BannerNPCType => ModContent.NPCType<CarapaceCaterpillar>();
}
