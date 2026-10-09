using System.Reflection;
using Everglow.Myth.TheFirefly.Backgrounds;
using ReLogic.Content;
using Terraria.GameContent.Shaders;

namespace Everglow.Myth.TheFirefly.Water;

public class FireFlyWaterSystem : ModSystem
{
	private WaterDustRenderer waterDustRenderer;

	public override void OnModLoad()
	{
		if (!Main.dedServ)
		{
			waterDustRenderer = new WaterDustRenderer();
		}
	}

	public override void PostDrawTiles()
	{
		waterDustRenderer.PresentDusts();
	}
}

internal class WaterDustRenderer
{
	private RenderTarget2D[] dustTargetSwap = null;
	private int currentDustTarget;
	private Vector2 lastDrawPosition;
	private Asset<Effect> dustLogicEffect;
	private Asset<Effect> dustDrawEffect;
	private Asset<Effect> dustSpawnEffect;

	private int oldScreenWidth, oldScreenHeight;

	private RenderTarget2D CurrentDustTarget
	{
		get
		{
			return dustTargetSwap[currentDustTarget];
		}
	}

	private RenderTarget2D NextDustTarget
	{
		get
		{
			return dustTargetSwap[currentDustTarget ^ 1];
		}
	}

	public WaterDustRenderer()
	{
		dustLogicEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/DustLogic");
		dustDrawEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/DustDraw");
		dustSpawnEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/DustSpawn");
		dustTargetSwap = new RenderTarget2D[2];
		Ins.MainThread.AddTask(() =>
		{
			dustTargetSwap[0] = new RenderTarget2D(
				Main.graphics.GraphicsDevice,
				Main.screenWidth,
				Main.screenHeight,
				false, SurfaceFormat.Color,
				DepthFormat.None);
			dustTargetSwap[1] = new RenderTarget2D(
				Main.graphics.GraphicsDevice,
				Main.screenWidth,
				Main.screenHeight,
				false, SurfaceFormat.Color,
				DepthFormat.None);
		});

		currentDustTarget = 0;
		lastDrawPosition = Vector2.Zero;

		On_WaterShaderData.PreDraw += WaterShaderData_PreDraw;

		oldScreenWidth = Main.screenWidth;
		oldScreenHeight = Main.screenHeight;
	}

	private void WaterShaderData_PreDraw(On_WaterShaderData.orig_PreDraw orig, WaterShaderData self, GameTime gameTime)
	{
		orig(self, gameTime);

		if (CurrentDustTarget == null || NextDustTarget == null
			|| !dustLogicEffect.IsLoaded || !dustSpawnEffect.IsLoaded
			|| dustDrawEffect.IsDisposed || dustDrawEffect.IsDisposed)
		{
			return;
		}

		var graphicsDevice = Main.graphics.GraphicsDevice;
		var spriteBatch = Main.spriteBatch;

		var motionVector = lastDrawPosition - Main.screenPosition;

		if (!Main.gamePaused)
		{
			var waterShader = (WaterShaderData)Terraria.Graphics.Effects.Filters.Scene["WaterDistortion"].GetShader();
			var disortionTarget = (RenderTarget2D)typeof(WaterShaderData).GetField("_distortionTarget", BindingFlags.NonPublic | BindingFlags.Instance)
				.GetValue(waterShader);
			if (disortionTarget == null)
			{
				return;
			}

			var lastDistortionDrawOffset = (Vector2)typeof(WaterShaderData).GetField("_lastDistortionDrawOffset", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(waterShader);
			Vector2 value = new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f * (Vector2.One - Vector2.One / Main.GameViewMatrix.Zoom);
			Vector2 value2 = (Main.drawToScreen ? Vector2.Zero : new Vector2(Main.offScreenRange, Main.offScreenRange)) - Main.screenPosition - value;
			Vector2 offset = -(value2 * 0.25f - lastDistortionDrawOffset) / new Vector2(disortionTarget.Width, disortionTarget.Height);
			Vector2 targetPos = Main.screenPosition - Main.sceneWaterPos
				+ new Vector2(Main.offScreenRange, Main.offScreenRange) + value - new Vector2(Main.offScreenRange, Main.offScreenRange);

			dustLogicEffect.Wait();
			dustSpawnEffect.Wait();
			var dustLogicShader = dustLogicEffect.Value.CurrentTechnique.Passes[0];
			var dustSpawnShader = dustSpawnEffect.Value.CurrentTechnique.Passes[0];
			graphicsDevice.SetRenderTarget(NextDustTarget);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullNone);
			{
				dustLogicShader.Apply();
				spriteBatch.Draw(CurrentDustTarget, motionVector, Color.White);
			}
			spriteBatch.End();
			Vector2 screenSizeZoom = new Vector2(Main.screenWidth, Main.screenHeight) / Main.GameViewMatrix.Zoom;

			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp,
				DepthStencilState.None, RasterizerState.CullNone);
			{
				graphicsDevice.Textures[1] = disortionTarget;
				graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				graphicsDevice.Textures[2] = Main.waterTarget;
				graphicsDevice.SamplerStates[2] = SamplerState.PointClamp;

				dustSpawnEffect.Value.Parameters["uResolution"].SetValue(screenSizeZoom);
				dustSpawnEffect.Value.Parameters["uTargetPos"].SetValue(targetPos);
				dustSpawnEffect.Value.Parameters["uInvWaterSize"].SetValue(new Vector2(1f / Main.waterTarget.Width, 1f / Main.waterTarget.Height));
				dustSpawnEffect.Value.Parameters["uZoom"].SetValue(new Vector2(1f / Main.GameViewMatrix.Zoom.X, 1f / Main.GameViewMatrix.Zoom.Y));
				dustSpawnEffect.Value.Parameters["uOffset"].SetValue(offset);
				dustSpawnEffect.Value.Parameters["uThreasholdMin"].SetValue(0.03f);
				dustSpawnEffect.Value.Parameters["uThreasholdMax"].SetValue(0.08f);
				dustSpawnEffect.Value.Parameters["uSpawnChance"].SetValue(0.02f);
				dustSpawnEffect.Value.Parameters["uWaterDisortionTargetSize"].SetValue(new Vector2(disortionTarget.Width, disortionTarget.Height));
				dustSpawnEffect.Value.Parameters["uVFXTime"].SetValue((float)Main.timeForVisualEffects * 0.2f);

				dustSpawnShader.Apply();

				var invZoom = new Vector2(1f / Main.GameViewMatrix.Zoom.X, 1f / Main.GameViewMatrix.Zoom.Y);
				var size = new Vector2(NextDustTarget.Width, NextDustTarget.Height);
				Vector2 topLeft = size * 0.5f * (Vector2.One - invZoom);
				size *= invZoom;

				spriteBatch.Draw(CurrentDustTarget, new Rectangle((int)topLeft.X, (int)topLeft.Y, (int)size.X, (int)size.Y), Color.White);
			}
			spriteBatch.End();
			EndPass();
		}
	}

	public void PresentDusts()
	{
		if (CurrentDustTarget == null || NextDustTarget == null)
		{
			return;
		}

		MothBackground mbione = ModContent.GetInstance<MothBackground>();
		if (!MothBackground.BiomeActive())
		{
			return;
		}

		if (oldScreenWidth != Main.screenWidth || oldScreenHeight != Main.screenHeight)
		{
			OnResolutionChanged();
			oldScreenWidth = Main.screenWidth;
			oldScreenHeight = Main.screenHeight;
		}

		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;
		dustDrawEffect.Wait();
		var dustDraw = dustDrawEffect.Value.CurrentTechnique.Passes[0];

		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp,
			DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		dustDrawEffect.Value.Parameters["uColor"].SetValue(new Vector3(0f, 0.9f, 1.0f));
		dustDrawEffect.Value.Parameters["uResolution"].SetValue(new Vector2(Main.screenWidth, Main.screenHeight));
		dustDrawEffect.Value.Parameters["uResolutionInv"].SetValue(new Vector2(1f / Main.screenWidth, 1f / Main.screenHeight));
		dustDraw.Apply();
		spriteBatch.Draw(
			CurrentDustTarget,
			new Rectangle(0, 0, CurrentDustTarget.Width, CurrentDustTarget.Height), Color.White);
		spriteBatch.End();
	}

	private void EndPass()
	{
		var graphicsDevice = Main.graphics.GraphicsDevice;
		lastDrawPosition = Main.screenPosition;
		currentDustTarget = (currentDustTarget + 1) % 2;
		graphicsDevice.SetRenderTarget(null);
	}

	private void OnResolutionChanged()
	{
		dustTargetSwap[0] = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth,
			Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None);
		dustTargetSwap[1] = new RenderTarget2D(
			Main.graphics.GraphicsDevice,
			Main.screenWidth, Main.screenHeight, false, SurfaceFormat.Color, DepthFormat.None);
	}

	public void Unload()
	{
		On_WaterShaderData.PreDraw -= WaterShaderData_PreDraw;
	}
}
