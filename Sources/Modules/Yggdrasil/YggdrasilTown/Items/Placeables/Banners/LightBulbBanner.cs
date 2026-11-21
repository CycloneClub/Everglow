using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class LightBulbBanner : BannerItemBase
{
	public override int BannerTileStyle => 29;

	public override int BannerNPCType => ModContent.NPCType<LightBulb>();
}
