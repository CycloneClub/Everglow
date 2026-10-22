namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;

public class ArenaMessageBackgroundPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.ArenaMessageBackgroundDissolve;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Ground_Glass.Value);
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_Success_Or_Fail.Value);
		effect.Parameters["uScale"].SetValue(4f);
		effect.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.005f);
		Texture2D halo = ModAsset.ScreenPoint.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
