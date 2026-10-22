namespace Everglow.Yggdrasil.CorruptWormHive.VFXs;

public class DevilSparkPipeline : Pipeline
{
	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uHeatMap"].SetValue(ModAsset.Death_Sickle_Color_Hot.Value);
		Ins.Batch.BindTexture<Vertex2D>(Commons.ModAsset.Point.Value);
		Main.graphics.GraphicsDevice.Textures[0] = Commons.ModAsset.Point.Value;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void Load()
	{
		effect = ModAsset.DevilSpark;
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
