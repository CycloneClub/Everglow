using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class WhisperingGhostBanner : BannerItemBase
{
	public override int BannerTileStyle => 34;

	public override int BannerNPCType => ModContent.NPCType<WhisperingGhost>();
}
