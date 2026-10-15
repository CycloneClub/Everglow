using Everglow.Commons.Interfaces;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;

public class ScreenScaringEffectPipeline : Pipeline
{
	private RenderTarget2D screenTextureAsset; // 反射区域

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			screenTextureAsset?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = ModAsset.ScreenScaringShader;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		var gd = Main.instance.GraphicsDevice;
		screenTextureAsset = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public override void BeginRender()
	{
		var graphicsDevice = Main.graphics.GraphicsDevice;
		var spriteBatch = Main.spriteBatch;

		graphicsDevice.SetRenderTarget(Main.screenTargetSwap);

		// 保存原屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone);
		spriteBatch.Draw(Main.screenTarget, Vector2.Zero, Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(screenTextureAsset);

		// 以另一种方法绘制保存下来的屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone, null, Matrix.Invert(Main.GameViewMatrix.TransformationMatrix));
		spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(Main.screenTarget);

		// 绘制原屏幕
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.Default,
			RasterizerState.CullNone);
		spriteBatch.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
		spriteBatch.End();

		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None,
			RasterizerState.CullNone);

		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(0, 0, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();

		Main.graphics.GraphicsDevice.Textures[0] = screenTextureAsset;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		BeginRender();
		foreach (var visual in visuals)
		{
			visual.Draw();
		}
		EndRender();
	}

	public override void EndRender()
	{
		var spriteBatch = Main.spriteBatch;
		spriteBatch.End();
	}
}
