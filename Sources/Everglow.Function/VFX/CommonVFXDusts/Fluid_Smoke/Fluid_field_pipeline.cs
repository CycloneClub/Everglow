namespace Everglow.Commons.VFX.CommonVFXDusts.Fluid_Smoke;

public class Fluid_field_pipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
		if (Ins.VisualQuality.High)
		{
			Main.instance.GraphicsDevice.Clear(new Color(128, 128, 0, 128));
		}
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = ModAsset.NormalShaderForGrayBg;
	}
}