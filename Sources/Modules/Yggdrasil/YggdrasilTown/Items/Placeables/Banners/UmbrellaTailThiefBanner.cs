using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class UmbrellaTailThiefBanner : BannerItemBase
{
	public override int BannerTileStyle => 33;

	public override int BannerNPCType => ModContent.NPCType<UmbrellaTailThief>();
}
