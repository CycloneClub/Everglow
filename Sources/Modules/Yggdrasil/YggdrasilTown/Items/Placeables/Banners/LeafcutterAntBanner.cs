using Everglow.Commons.Templates;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Banners;

public class LeafcutterAntBanner : BannerItemBase<YggdrasilTownBannerTile>
{
	public override string Texture => BannerItemBase_Mod;

	public override int BannerTileStyle => 14;

	public override int BannerNPCType => ModContent.NPCType<LeafcutterAnt>();
}
