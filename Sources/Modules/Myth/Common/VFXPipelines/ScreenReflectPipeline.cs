using ReLogic.Content;

namespace Everglow.Myth.Common.VFXPipelines;

internal class ScreenReflectPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModContent.Request<Effect>("Everglow/Myth/Effects/ScreenReflect", AssetRequestMode.ImmediateLoad);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["tex1"].SetValue(Main.screenTarget);
		effect.Parameters["uScreenWidth"].SetValue(Main.screenWidth);
		effect.Parameters["uScreenHeight"].SetValue(Main.screenHeight);
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}
