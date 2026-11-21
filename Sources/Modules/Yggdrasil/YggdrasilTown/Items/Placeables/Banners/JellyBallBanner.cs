using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class JellyBallBanner : BannerItemBase
{
	public override int BannerTileStyle => 26;

	public override int BannerNPCType => ModContent.NPCType<JellyBall>();
}
