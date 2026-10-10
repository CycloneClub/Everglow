using Everglow.Commons.Utilities.BackgroundHelper;
using Everglow.Example.BgSlides;

namespace Everglow.Example.Items;

public class ExampleBackgroundTool : ModItem
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.Tools;

	public override void SetDefaults() => base.SetDefaults();

	public override void HoldItem(Player player)
	{
		BackgroundSystem bgSystem = ModContent.GetInstance<BackgroundSystem>();
		ExampleBgSlide0 exSlide0 = new ExampleBgSlide0();
		exSlide0.WorldAnchor = Main.MouseWorld;
		exSlide0.Shader = BackgroundSlideBase.Effects.XClamp_YClamp_Shader;
		bgSystem.AddBackgroundSlide(exSlide0);

		ExampleBgSlide1 exSlide1 = new ExampleBgSlide1();
		exSlide1.WorldAnchor = Main.MouseWorld;
		exSlide1.Shader = BackgroundSlideBase.Effects.XClamp_YClamp_Shader;
		bgSystem.AddBackgroundSlide(exSlide1);

		ExampleBgSlide2 exSlide2 = new ExampleBgSlide2();
		exSlide2.WorldAnchor = Main.MouseWorld;
		exSlide2.Shader = BackgroundSlideBase.Effects.XClamp_YClamp_Shader;
		bgSystem.AddBackgroundSlide(exSlide2);

		ExampleBgSlide3 exSlide3 = new ExampleBgSlide3();
		exSlide3.WorldAnchor = Main.MouseWorld;
		exSlide3.Shader = BackgroundSlideBase.Effects.XClamp_YClamp_Shader;
		bgSystem.AddBackgroundSlide(exSlide3);

		if (Main.mouseLeft && Main.mouseLeftRelease)
		{
			for (int i = 0; i < 10; i++)
			{
				ExampleBgSlideCloud exSlidec = new ExampleBgSlideCloud();
				exSlidec.WorldAnchor = Main.MouseWorld + new Vector2(200, 0).RotatedBy(i / 10f * MathHelper.TwoPi);
				bgSystem.AddBackgroundSlide(exSlidec);
			}
		}
	}
}
