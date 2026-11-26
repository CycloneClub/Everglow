using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class JellyBallBanner : BannerItemBase<YggdrasilTownBannerTile>
{
	public override int BannerTileStyle => 12;

	public override int BannerNPCType => ModContent.NPCType<JellyBall>();
}
