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
			// 背景 = 零速度(0.5 偏置)。8bit 无法精确表示 0.5, Push 里用死区消除量化残差。
			Main.instance.GraphicsDevice.Clear(new Color(0.5f, 0.5f, 0f, 0.5f));
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
