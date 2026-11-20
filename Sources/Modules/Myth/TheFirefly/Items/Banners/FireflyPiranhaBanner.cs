using Everglow.Commons.Templates;
using Everglow.Myth.TheFirefly.NPCs;

namespace Everglow.Myth.TheFirefly.Items.Banners;

public class FireflyPiranhaBanner : BannerItemBase
{
	public override int BannerTileStyle => 12;

	public override int BannerNPCType => ModContent.NPCType<FireflyPiranha>();
}
