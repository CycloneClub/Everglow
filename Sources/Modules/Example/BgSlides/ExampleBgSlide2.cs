using Everglow.Commons.Mechanics.CustomBackground;
using Everglow.Example.Items;

namespace Everglow.Example.BgSlides;

public class ExampleBgSlide2 : BackgroundSlideBase
{
	public override void SetDefaults()
	{
		base.SetDefaults();
		Texture = ModAsset.ExampleBgSlide2.Value;
		Distance = 15f;
	}

	public override bool CanActive()
	{
		return Main.LocalPlayer.HeldItem.type == ModContent.ItemType<ExampleBackgroundTool>();
	}
}
