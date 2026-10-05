namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.ProjectileEffects;

public class MiningPowerPickaxe_Proj_WavePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.MiningPowerPickaxe_Proj_Wave_Shader;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);

		model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.EffectMatrix;
		Vector2 mouseScreen = Vector2.Transform(Main.MouseWorld, model * projection);
		mouseScreen += Vector2.One;
		mouseScreen.Y = 2 - mouseScreen.Y;
		mouseScreen *= new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		effect.Parameters["uMousePos"].SetValue(mouseScreen);
		effect.Parameters["colorParaPlus"].SetValue(0.5f);
		effect.Parameters["colorParaMul"].SetValue(1.3f);
		effect.Parameters["disPara"].SetValue(0.02f);
		effect.Parameters["uTime"].SetValue(((float)Main.time * 0.2f) % 8f - 1.8f);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
