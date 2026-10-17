namespace Everglow.Myth.LanternMoon.VFX;

public class LanternGhostKing_Matrix_DissolvePipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		Ins.Batch.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Ins.Batch.GraphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		Ins.Batch.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_RGB_Large.Value;
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.Parameters["size1"].SetValue(Vector2.One);
		effect.Value.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.03f);
		effect.Value.Parameters["warpScale"].SetValue(1f);
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = ModAsset.LanternGhostKing_Matrix_Shader;
	}
}
