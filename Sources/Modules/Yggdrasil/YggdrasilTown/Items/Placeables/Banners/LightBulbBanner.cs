using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using static Everglow.Commons.ModAsset;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class LightBulbBanner : BannerItemBase
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 29;

	public override int BannerNPCType => ModContent.NPCType<LightBulb>();
}
