namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class PurpleBombCaterpillarBloodDropPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.PurpleBombCaterpillarBloodDrop_Shader;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_PurpleBombCaterpillarBloodDrop.Value);
		effect.Parameters["uIlluminationThreshold"].SetValue(0.99f);
		Texture2D lightness = Commons.ModAsset.Point_lowContrast.Value;
		Ins.Batch.BindTexture<Vertex2D>(lightness);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
