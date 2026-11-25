using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class CrimsonSpellBanner : BannerItemBase
{
	public override int BannerTileStyle => 20;

	public override int BannerNPCType => ModContent.NPCType<CrimsonSpell>();
}
