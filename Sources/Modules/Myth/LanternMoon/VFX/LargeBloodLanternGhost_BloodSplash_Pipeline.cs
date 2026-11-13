namespace Everglow.Myth.LanternMoon.VFX;

public class LargeBloodLanternGhost_BloodSplashPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.LargeBloodLanternGhost_BloodSplash;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Cell.Value);
		effect.Parameters["uTransform"].SetValue(model * projection);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
