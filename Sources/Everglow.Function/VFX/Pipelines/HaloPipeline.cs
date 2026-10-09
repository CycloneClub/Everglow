using Everglow.Commons.Enums;

namespace Everglow.Commons.VFX.Pipelines;

public class HaloPipeline : PostPipeline
{
	private RenderTarget2D haloScreen;
	private RenderTarget2D haloScreenSwap;

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			haloScreen?.Dispose();
			haloScreenSwap?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = ModAsset.SolarHalo;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		var gd = Main.instance.GraphicsDevice;
		haloScreen = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		haloScreenSwap = new RenderTarget2D(gd, (int)size.X, (int)size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;
		var effect = this.effect.Value;

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);

		gd.SetRenderTarget(haloScreen);
		effect.Parameters["uTransform"].SetValue(Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Parameters["uHaloSize"].SetValue(6);
		effect.Parameters["uHaloRange"].SetValue(0.4f);
		effect.Parameters["uSunMoonLight"].SetValue(Color.White.ToVector4());
		effect.Parameters["uSunMoonPos"].SetValue(GetSunPos() / new Vector2(Main.screenWidth, Main.screenHeight));
		effect.Parameters["uScreenYDevideByX"].SetValue(Main.screenWidth / (float)Main.screenHeight);
		effect.Parameters["uRainbow"].SetValue(ModAsset.Noise_SolarSpectrum.Value);
		effect.CurrentTechnique.Passes["HaloRing"].Apply();
		sb.Draw(rt2D, Vector2.Zero, Color.White);

		sb.End();

		var cur = Ins.VFXManager.CurrentRenderTarget;
		Ins.VFXManager.SwapRenderTarget();
		gd.SetRenderTarget(Ins.VFXManager.CurrentRenderTarget);
		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		sb.Draw(cur, Vector2.Zero, Color.White);

		gd.BlendState = BlendState.AlphaBlend;
		sb.Draw(haloScreen, Vector2.Zero, new Color(255, 255, 255, 0));
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
