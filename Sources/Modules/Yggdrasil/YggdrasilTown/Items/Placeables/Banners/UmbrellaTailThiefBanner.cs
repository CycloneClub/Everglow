using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class UmbrellaTailThiefBanner : BannerItemBase<YggdrasilTownBannerTile>
{
	public override int BannerTileStyle => 19;

	public override int BannerNPCType => ModContent.NPCType<UmbrellaTailThief>();
}
