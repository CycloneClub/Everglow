using Everglow.Commons.Templates;
using Everglow.Myth.LanternMoon.NPCs;
using Everglow.Myth.LanternMoon.Tiles;
using static Everglow.Commons.ModAsset;

namespace Everglow.Myth.LanternMoon.Items.Banners;

public class LittleRedPaperFigureBanner : BannerItemBase<LanternMoonBannerTile>
{

	public override int BannerTileStyle => 6;

	public override int BannerNPCType => ModContent.NPCType<LittleRedPaperFigure>();
}
