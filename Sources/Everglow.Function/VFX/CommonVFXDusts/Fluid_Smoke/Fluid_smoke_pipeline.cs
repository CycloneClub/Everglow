using Everglow.Commons.Enums;
using Everglow.Commons.Graphics;
using log4net.Core;

namespace Everglow.Commons.VFX.CommonVFXDusts.Fluid_Smoke;

public class Fluid_smoke_Pipeline : PostPipeline
{
	private RenderTarget2D fluid_field_Screen;
	private RenderTarget2D fluid_field_ScreenSwap;
	private RenderTarget2D fluid_color_Screen;
	private RenderTarget2D fluid_color_ScreenSwap;

	public Vector2 ScreenPosOld = Vector2.zeroVector;

	public Vector2 OldMousePosition;

	public override void Load()
	{
		Ins.MainThread.AddTask(() =>
		{
			AllocateRenderTarget(new Vector2(Main.screenWidth, Main.screenHeight));
		});
		Ins.HookManager.AddHook(CodeLayer.ResolutionChanged, (Vector2 size) =>
		{
			fluid_field_Screen?.Dispose();
			fluid_field_ScreenSwap?.Dispose();
			fluid_color_Screen?.Dispose();
			fluid_color_ScreenSwap?.Dispose();
			AllocateRenderTarget(size);
		}, "Realloc RenderTarget");
		effect = ModAsset.Fluid_Canvas_Shader;
	}

	private void AllocateRenderTarget(Vector2 size)
	{
		Vector2 offseted_size = size + new Vector2(CustomOffsetRange()) * 2;
		var gd = Main.instance.GraphicsDevice;
		fluid_field_Screen = new RenderTarget2D(gd, (int)offseted_size.X, (int)offseted_size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		fluid_field_ScreenSwap = new RenderTarget2D(gd, (int)offseted_size.X, (int)offseted_size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		fluid_color_Screen = new RenderTarget2D(gd, (int)offseted_size.X, (int)offseted_size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
		fluid_color_ScreenSwap = new RenderTarget2D(gd, (int)offseted_size.X, (int)offseted_size.Y, false, gd.PresentationParameters.BackBufferFormat, DepthFormat.None);
	}

	public float CustomOffsetRange()
	{
		return 600;
	}

	public override void Render(RenderTarget2D rt2D)
	{
		var sb = Main.spriteBatch;
		var gd = Main.instance.GraphicsDevice;
		var sparse_effect = this.effect.Value;
		var normal_effect = ModAsset.Shader2D.Value;
		var projection = Matrix.CreateOrthographicOffCenter(-CustomOffsetRange(), Main.screenWidth + CustomOffsetRange(), Main.screenHeight + CustomOffsetRange(), -CustomOffsetRange(), 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0, 0, 0)) * Main.GameViewMatrix.TransformationMatrix;
		Vector2 modifiedMouseScreen = Vector2.Transform(Main.MouseScreen, model);
		Vector2 offseted_zero = Vector2.zeroVector - new Vector2(CustomOffsetRange());

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(fluid_field_Screen);
		sparse_effect.Parameters["uTransform"].SetValue(projection);
		sparse_effect.Parameters["uResolutionX"].SetValue(fluid_field_Screen.Width);
		sparse_effect.Parameters["uResolutionY"].SetValue(fluid_field_Screen.Height);
		sparse_effect.Parameters["offset_small_RT2D"].SetValue(CustomOffsetRange());
		sparse_effect.CurrentTechnique.Passes["Push"].Apply();
		gd.Clear(Color.Transparent);
		gd.Textures[1] = rt2D;
		sb.Draw(fluid_field_ScreenSwap, offseted_zero, new Color(1f, 1f, 1f, 1f));
		sb.End();

		sb.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(fluid_field_ScreenSwap);
		sparse_effect.Parameters["moveStep"].SetValue(80);
		sparse_effect.Parameters["viscosity"].SetValue(0.55f);
		sparse_effect.Parameters["kinematic"].SetValue(0.3f);
		sparse_effect.Parameters["timeStep"].SetValue(0.15f);
		sparse_effect.Parameters["VORTICITY_AMOUNT"].SetValue(0.11f);
		sparse_effect.CurrentTechnique.Passes["Fluid"].Apply();
		gd.Clear(new Color(0.5f, 0.5f, 0.5f, 0.5f));
		Vector2 deltaValue = ScreenPosOld - Main.screenPosition;
		deltaValue = Vector2.Transform(deltaValue, Matrix.Invert(projection) * model * projection);
		sb.Draw(fluid_field_Screen, deltaValue + offseted_zero, new Color(1f, 1f, 1f, 1f));
		sb.End();

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(fluid_color_Screen);
		normal_effect.Parameters["uTransform"].SetValue(projection);
		normal_effect.CurrentTechnique.Passes[0].Apply();
		gd.Clear(new Color(0, 0, 0, 1f));
		sb.Draw(fluid_color_ScreenSwap, offseted_zero, Color.White * 0.96f);
		Texture2D tex = ModAsset.SwirlPoint.Value;
		sb.Draw(tex, modifiedMouseScreen, null, new Color(1f, 1f, 1f, 0f), 0, tex.Size() * 0.5f, 0.8f, SpriteEffects.None, 0);
		sb.End();

		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.LinearClamp, DepthStencilState.None, RasterizerState.CullNone);
		gd.SetRenderTarget(fluid_color_ScreenSwap);
		sparse_effect.CurrentTechnique.Passes["Fade"].Apply();
		gd.Textures[1] = fluid_field_ScreenSwap;
		sb.Draw(fluid_color_Screen, deltaValue + offseted_zero, Color.White);
		sb.End();

		var cur = Ins.VFXManager.CurrentRenderTarget;
		Ins.VFXManager.SwapRenderTarget();
		gd.SetRenderTarget(Ins.VFXManager.CurrentRenderTarget);
		sb.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
		sb.Draw(cur, Vector2.Zero, Color.White);
		sb.Draw(fluid_field_Screen, offseted_zero, new Color(255, 255, 255, 0));
		sb.End();

		ScreenPosOld = Main.screenPosition;
		OldMousePosition = modifiedMouseScreen;
	}
}