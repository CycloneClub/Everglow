using ReLogic.Content;

namespace Everglow.Myth.Acytaea;

internal class NPPipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin(BlendState.Additive);
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) *
			Main.GameViewMatrix.ZoomMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1)
			);
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = Commons.ModAsset.Shader2D;
	}
}

internal class AcytaeaPipeline : PostPipeline
{
	private Asset<Texture2D> texture;
	public override void Render(RenderTarget2D rt2D)
	{
		Ins.Batch.Begin();
		var effect = this.effect.Value;
		Main.instance.GraphicsDevice.Textures[1] = texture.Value;
		// TODO 常量待优化，目前测试用
		effect.Parameters["m"].SetValue(0.62f);
		effect.Parameters["n"].SetValue(0.01f);
		effect.CurrentTechnique.Passes[0].Apply();
		Ins.Batch.Draw(rt2D, Vector2.Zero, Color.White);
		Ins.Batch.End();
	}

	public override void Load()
	{
		texture = ModAsset.Cosmic;
		effect = ModAsset.BigTentacle;
	}
}
