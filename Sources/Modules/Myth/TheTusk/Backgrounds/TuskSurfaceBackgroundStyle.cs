namespace Everglow.Myth.TheTusk.Backgrounds;

public class TuskSurfaceBackgroundStyle : ModSurfaceBackgroundStyle
{
	public override void ModifyFarFades(float[] fades, float transitionSpeed)
	{
	}

	public override int ChooseFarTexture()
	{
		return BackgroundTextureLoader.GetBackgroundSlot(ModAsset.Empty_Mod);
	}

	public override int ChooseMiddleTexture()
	{
		return BackgroundTextureLoader.GetBackgroundSlot(ModAsset.Empty_Mod);
	}

	public override int ChooseCloseTexture(ref float scale, ref double parallax, ref float a, ref float b)
	{
		b = 1000;
		return BackgroundTextureLoader.GetBackgroundSlot(ModAsset.Empty_Mod);
	}
}
