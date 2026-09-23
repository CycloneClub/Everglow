namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class YggdrasilTownAccessGate_CrackPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.YggdrasilTownAccessGate_Effect;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_turtleCrack.Value);
		effect.Parameters["uCoordScale"].SetValue(new Vector2(0.2f, 0.6f));
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
