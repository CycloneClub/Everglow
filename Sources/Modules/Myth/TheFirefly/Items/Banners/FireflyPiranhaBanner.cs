using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;
using Everglow.Myth.TheFirefly.Tiles;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class FireflyPiranhaBanner : BannerItemBase<TheFireflyBannerTile>
{
	public override int BannerTileStyle => 2;

	public override int BannerNPCType => ModContent.NPCType<FireflyPiranha>();
}
