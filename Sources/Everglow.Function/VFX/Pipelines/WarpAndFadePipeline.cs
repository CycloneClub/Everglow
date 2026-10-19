using Everglow.Commons.Enums;

namespace Everglow.Commons.VFX.Pipelines;

/// <summary>
/// 脏绘制法实现逐渐扭曲消散Pipeline
/// </summary>
public class WarpAndFadePipeline : PostPipeline
{
	private RenderTarget2D saveScreenTarget; // 保存的原始屏幕
	private RenderTarget2D canvasScreen; // 残迹RenderTarget
	private RenderTarget2D canvasScreenSwap; // 旧残迹RenderTarget

	public Vector2 TotalMovedPosition; // 距离上次刷新的屏幕坐标
	public Vector2 LastRenderPosition; // 上次刷新的屏幕坐标

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			saveScreenTarget?.Dispose();
			canvasScreen?.Dispose();
			canvasScreenSwap?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = ModAsset.WarpAndFade;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		var gd = Main.instance.GraphicsDevice;
		saveScreenTarget = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		canvasScreen = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		canvasScreenSwap = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public void BeginRender()
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 保存原画
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		var cur = Main.screenTarget;
		gd.SetRenderTarget(saveScreenTarget);
		gd.Clear(Color.Transparent);
		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.End();

		// 处理画布
	}

	public override void Render(RenderTarget2D rt2D)
	{
		BeginRender();
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 保存旧画布RenderTarget
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(canvasScreenSwap);
		gd.Clear(Color.Transparent);
		sb.Draw(canvasScreen, Vector2.Zero, Color.White);
		sb.End();
		TotalMovedPosition = Main.screenPosition - LastRenderPosition;

		gd.SetRenderTarget(canvasScreen);
		gd.Clear(Color.Transparent);
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		if (!Main.gamePaused)
		{
			gd.Textures[1] = ModAsset.Noise_RGB.Value;
			gd.SamplerStates[1] = SamplerState.AnisotropicWrap;
			effect.Value.Parameters["strength"].SetValue(0.005f);
			effect.Value.Parameters["deltaY"].SetValue((float)Main.time * 0.003f);
			effect.Value.Parameters["mulSize"].SetValue(1.5f);
			effect.Value.Parameters["fade"].SetValue(0.07f);
			effect.Value.CurrentTechnique.Passes[0].Apply();
		}
		sb.Draw(canvasScreenSwap, -TotalMovedPosition, Color.White);
		sb.End();
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		if (!Main.gamePaused)
		{
			Effect wcs = VFXManager.DefaultEffect.Value;
			wcs.Parameters["uTransform"].SetValue(
				Matrix.CreateTranslation(new Vector3(0)) *
				Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
			wcs.CurrentTechnique.Passes[0].Apply();
			sb.Draw(rt2D, Vector2.zeroVector, Color.White);
		}
		LastRenderPosition = Main.screenPosition;
		sb.End();
		EndRender();
	}

	public void EndRender()
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;

		// 逐层展开绘制的画布
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(Main.screenTarget);
		gd.Clear(Color.Transparent);
		sb.Draw(saveScreenTarget, Vector2.Zero, Color.White);
		sb.Draw(canvasScreen, Vector2.Zero, Color.White);
		sb.End();
	}
}
