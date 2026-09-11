namespace Everglow.Commons.VFX.CommonVFXDusts.Fluid_Smoke;

public class Fluid_field_pipeline : Pipeline
{
	public override void BeginRender()
	{
		// 笔刷把速度写在 RGB、把形状写在贴图亮度里 (alpha=1), 因此必须用非预乘 alpha
		// 混合, 否则默认的 AlphaBlend 会把 RGB 乘以材质 alpha 而写不进速度。
		Ins.Batch.Begin(BlendState.NonPremultiplied);
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
