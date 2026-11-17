using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Example.Items;

namespace Everglow.Example.BgSlides;

public class ExampleBgSlide0 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.ExampleBgSlide0.Value;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ExampleBackgroundTool>();
	}
}
