using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class DarkGlimmeringRodsBanner : BannerItemBase
{
	public override int BannerTileStyle => 23;

	public override int BannerNPCType => ModContent.NPCType<DarkGlimmeringRods>();
}
