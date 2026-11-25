using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class RockElementalBanner : BannerItemBase
{
	public override int BannerTileStyle => 31;

	public override int BannerNPCType => ModContent.NPCType<RockElemental>();
}
