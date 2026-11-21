using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class GiantJellyBallBanner : BannerItemBase
{
	public override int BannerTileStyle => 25;

	public override int BannerNPCType => ModContent.NPCType<GiantJellyBall>();
}
