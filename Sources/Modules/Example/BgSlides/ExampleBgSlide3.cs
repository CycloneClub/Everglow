using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Example.Items;

namespace Everglow.Example.BgSlides;

public class ExampleBgSlide3 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.ExampleBgSlide3.Value;
		Distance = 5f;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ExampleBackgroundTool>();
	}
}
