using Everglow.Commons.Enums;

namespace Everglow.Commons.VFX.Pipelines;

public class HeatMapRenderPipeline_cursedFlame : PostPipeline
{
	private RenderTarget2D heatMapScreen;
	private RenderTarget2D heatMapScreenSwap;

	private static int ScreenWidth => Main.screenWidth;

	private static int ScreenHeight => Main.screenHeight;

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget();
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			heatMapScreen?.Dispose();
			heatMapScreenSwap?.Dispose();
			AllocateRenderTarget();
		}, "Realloc RenderTarget");
		effect = ModAsset.HeatMapRender;
	}

	private void AllocateRenderTarget()
	{
		var gd = Main.instance.GraphicsDevice;
		heatMapScreen = new RenderTarget2D(gd, ScreenWidth, ScreenHeight, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		heatMapScreenSwap = new RenderTarget2D(gd, ScreenWidth, ScreenHeight, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;
		var effect = this.effect.Value;

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

		gd.SetRenderTarget(heatMapScreen);
		effect.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_curseFlame.Value);
		effect.CurrentTechnique.Passes["ToHeatMap"].Apply();
		sb.Draw(rt2D, Vector2.Zero, new Color(255, 255, 255, 255));

		sb.End();

		var cur = Ins.VFXManager.CurrentRenderTarget;
		Ins.VFXManager.SwapRenderTarget();
		gd.SetRenderTarget(Ins.VFXManager.CurrentRenderTarget);
		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		sb.Draw(cur, Vector2.Zero, Color.White);

		gd.BlendState = BlendState.AlphaBlend;
		sb.Draw(heatMapScreen, Vector2.Zero, new Color(255, 255, 255, 0));
		sb.Draw(heatMapScreen, Vector2.Zero, new Color(255, 255, 255, 0) * 0.4f);
		sb.End();
	}

	public static Vector2 GetSunPos()
	{
		float HalfMaxTime = Main.dayTime ? 27000 : 16200;
		float bgTop = -Main.screenPosition.Y / (float)(Main.worldSurface * 16.0 - 600.0) * 200f;
		float value = 1 - (float)Main.time / HalfMaxTime;
		float StarX = (1 - value) * Main.screenWidth / 2f - 100 * value;
		float t = value * value;
		float StarY = bgTop + t * 250f + 180;
		if (Main.LocalPlayer != null)
		{
			if (Main.LocalPlayer.gravDir == -1)
			{
				return new Vector2(StarX, Main.screenHeight - StarY);
			}
		}

		return new Vector2(StarX, StarY);
	}
}
