namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class SpringOfQuicksand_SandTrail_Pipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.SpringOfQuicksand_SandTrail_Shader;
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Sand.Value);
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.SpringOfQuicksand_Sandflow_HeatMap.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);

		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
