using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class LampFruitBorerBanner : BannerItemBase
{
	public override int BannerTileStyle => 27;

	public override int BannerNPCType => ModContent.NPCType<LampFruitBorer>();
}
