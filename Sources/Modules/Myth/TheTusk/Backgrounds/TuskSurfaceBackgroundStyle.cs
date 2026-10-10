namespace Everglow.Myth.TheTusk.Backgrounds;

public class TuskSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
{
	public override void ModifyFarFades(float[] fades, float transitionSpeed)
	{
	}

	public override int ChooseFarTexture()
	{
		return BackgroundTextureLoader.GetBackgroundSlot("Everglow/Myth/TheTusk/Backgrounds/Empty");
	}

	public override int ChooseMiddleTexture()
	{
		return BackgroundTextureLoader.GetBackgroundSlot("Everglow/Myth/TheTusk/Backgrounds/Empty");
	}

	public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
	{
		b = 1000;
		return BackgroundTextureLoader.GetBackgroundSlot("Everglow/Myth/TheTusk/Backgrounds/Empty");
	}
}
