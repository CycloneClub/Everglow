using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Example.Items;

namespace Everglow.Example.BgSlides;

public class ExampleBgSlide1 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.ExampleBgSlide1.Value;
		Distance = 45f;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ExampleBackgroundTool>();
	}
}
