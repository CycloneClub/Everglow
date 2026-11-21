using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CyanOreBeetleBanner : BannerItemBase
{
	public override int BannerTileStyle => 22;

	public override int BannerNPCType => ModContent.NPCType<CyanOreBeetle>();
}
