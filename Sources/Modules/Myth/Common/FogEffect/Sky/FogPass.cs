using Everglow.Myth.Common.FogEffect.Configs;
using ReLogic.Content;

namespace Everglow.Myth.Common.FogEffect.Sky;

public struct FogState
{
	/// <summary>
	/// 是否开启大雾效果
	/// </summary>
	public bool Enabled;
	/// <summary>
	/// 雾散射随着距离增大而增加的速率
	/// </summary>
	public float BloomScatteringRatio;
	/// <summary>
	/// 单位距离的雾会吸收多少亮度，该值越大则雾浓度越高，可见性越差
	/// </summary>
	public Vector3 ViewAbsorptionRatio;
	/// <summary>
	/// 原版光照物块光强的阈值，阈值越大那么亮度比较暗的光照环境不会加入光晕计算
	/// </summary>
	public float LuminanceThreashold;
	/// <summary>
	/// 超出屏幕计算光照的物块格子数
	/// </summary>
	public int OffscreenTileCount;
	/// <summary>
	/// 光晕效果的强度，越强光晕越亮
	/// </summary>
	public float BloomIntensity;
	/// <summary>
	/// 散射效果的模糊半径
	/// </summary>
	public int BloomRadius;
}

public class FogPass
{
	public static FogState DayThickFog = new FogState
	{
		Enabled = true,
		BloomScatteringRatio = 0.16f,
		ViewAbsorptionRatio = new Vector3(0.05f),
		LuminanceThreashold = 0.2f,
		OffscreenTileCount = 8,
		BloomIntensity = 0.5f,
		BloomRadius = 2,
	};

	public static FogState Default = new FogState
	{
		Enabled = false,
		BloomScatteringRatio = 0f,
		ViewAbsorptionRatio = new Vector3(0f),
		LuminanceThreashold = 0f,
		OffscreenTileCount = 0,
		BloomIntensity = 0f,
		BloomRadius = 0,
	};

	private Asset<Effect> boxKernelEffect;
	private Asset<Effect> gaussianKernelEffect;
	private Asset<Effect> fogScreenEffect;
	private Asset<Effect> temporalInterpEffect;

	private RenderTarget2D[] blurRenderTargets;
	private RenderTarget2D renderTargetSwap;
	private RenderTarget2D filteredScreenTarget;

	private Color[] lightMap;
	private RenderTarget2D lightTexture;
	private RenderTarget2D prevLightTexture;
	private RenderTarget2D lightSwapTarget;

	private int frameWidth;
	private int frameHeight;
	private int screenWidth;
	private int screenHeight;
	private int tileWidth;
	private int tileHeight;
	private bool shouldResetRenderTargets;

	private readonly int maxBlurLevels = 10;

	private int maxBlurLevel;

	private int startTileX;
	private int startTileY;
	private int oldStartTileX;
	private int oldStartTileY;

	private bool enableLightUpload;
	private bool enableTemporalFilter;
	private Vector2 screenPosition;
	private const SurfaceFormat surfaceFormat = SurfaceFormat.Rgba1010102;

	private int switchCounter = 0;
	private int totalSwitchCounter = 0;
	private bool useGaussian = true;
	private FogState beginState;
	private FogState currentState;
	private FogState targetState;

	/// <summary>
	/// 光晕效果的模糊卷积核半径，该值为2^k
	/// </summary>
	public int BloomRadius
	{
		get
		{
			return currentState.BloomRadius;
		}

		set
		{
			currentState.BloomRadius = value;
		}
	}

	public FogPass()
	{
		screenWidth = 0;
		screenHeight = 0;

		boxKernelEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/BoxFilter"); // QuickEffect does not work, conversion failed.
		gaussianKernelEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/GBlur"); // Same as above
		fogScreenEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/Fog"); // Same as above
		temporalInterpEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/Temporal"); // Same as above

		blurRenderTargets = new RenderTarget2D[maxBlurLevels];
		shouldResetRenderTargets = true;
		enableTemporalFilter = false;
		enableLightUpload = true;
		enableTemporalFilter = true;
		screenPosition = Vector2.Zero;
	}

	public void Preprocess()
	{
		UpdateParameters();
	}

	private void UpdateParameters()
	{
		var fogConfig = ModContent.GetInstance<FogConfigs>();

		BloomRadius = fogConfig.MaxBloomRadius;
		currentState.BloomIntensity = fogConfig.BloomIntensity;
		currentState.LuminanceThreashold = fogConfig.LightLuminanceThreashold;
		currentState.ViewAbsorptionRatio = new Vector3(
			fogConfig.FogAbsorptionR,
			fogConfig.FogAbsorptionG,
			fogConfig.FogAbsorptionB);
		currentState.BloomScatteringRatio = fogConfig.FogBloomRate;
		// m_currentState.FogScatterWithDistance = fogConfig.FogScatterWithDistance;

		shouldResetRenderTargets |= currentState.OffscreenTileCount != fogConfig.OffscreenTiles;
		currentState.OffscreenTileCount = fogConfig.OffscreenTiles;
		useGaussian = fogConfig.GaussianKernel;
		enableLightUpload = fogConfig.EnableLightUpload;
		enableTemporalFilter = fogConfig.EnableTemporalInterp;

		currentState.Enabled = fogConfig.EnableScattering;
	}

	private void ResetLightMap()
	{
		tileWidth = (screenWidth + 15) / 16 + currentState.OffscreenTileCount * 2 + 2;
		tileHeight = (screenHeight + 15) / 16 + currentState.OffscreenTileCount * 2 + 2;
		lightMap = new Color[tileWidth * tileHeight];
		lightTexture = new RenderTarget2D(Main.graphics.GraphicsDevice, tileWidth, tileHeight, false,
			SurfaceFormat.Color, DepthFormat.None);
	}

	private void DisposePrevRenderTargets()
	{
		for (int i = 0; i < blurRenderTargets.Length; i++)
		{
			blurRenderTargets[i]?.Dispose();
		}
		renderTargetSwap?.Dispose();
		filteredScreenTarget?.Dispose();
	}

	public void SwitchState(in FogState state, int interpTime)
	{
		beginState = currentState;
		targetState = state;
		totalSwitchCounter = interpTime;
		switchCounter = interpTime;
	}

	public void Update()
	{
		if (switchCounter > 0)
		{
			// 渐变开始
			if (switchCounter == totalSwitchCounter)
			{
				if (targetState.Enabled)
				{
					currentState.Enabled = true;
					currentState.BloomRadius = targetState.BloomRadius;
					currentState.OffscreenTileCount = targetState.OffscreenTileCount;
					shouldResetRenderTargets = true;
				}
			}
			switchCounter--;

			// 渐变结束
			if (switchCounter == 0 && !targetState.Enabled)
			{
				currentState.Enabled = false;
				currentState.BloomRadius = targetState.BloomRadius;
				currentState.OffscreenTileCount = targetState.OffscreenTileCount;
				shouldResetRenderTargets = true;
			}

			float progress = 1f - switchCounter / (float)totalSwitchCounter;

			currentState.BloomIntensity = MathHelper.Lerp(
				beginState.BloomIntensity,
				targetState.BloomIntensity, progress);
			currentState.BloomScatteringRatio = MathHelper.Lerp(
				beginState.BloomScatteringRatio,
				targetState.BloomScatteringRatio, progress);
			currentState.LuminanceThreashold = MathHelper.Lerp(
				beginState.LuminanceThreashold,
				targetState.LuminanceThreashold, progress);
			currentState.ViewAbsorptionRatio = Vector3.Lerp(
				beginState.ViewAbsorptionRatio,
				targetState.ViewAbsorptionRatio, progress * progress);
		}

		if (Main.time % 400 < 1)
		{
			if ((int)(Main.time / 400) % 2 == 0)
			{
				SwitchState(Default, 150);
			}
			else
			{
				SwitchState(DayThickFog, 150);
			}
		}
	}

	private void ResetRenderTargets()
	{
		DisposePrevRenderTargets();

		frameWidth = tileWidth * 16;
		frameHeight = tileHeight * 16;

		int l = 0;
		for (; l < maxBlurLevels; l++)
		{
			if (frameWidth >> l == 0 || frameHeight >> l == 0)
			{
				break;
			}

			blurRenderTargets[l] = new RenderTarget2D(
				Main.graphics.GraphicsDevice,
				frameWidth >> l, frameWidth >> l, false,
				surfaceFormat, DepthFormat.None);
		}
		maxBlurLevel = Math.Min(l, 8);

		for (int i = 0; i < maxBlurLevel; i++)
		{
			blurRenderTargets[i] = new RenderTarget2D(
				Main.graphics.GraphicsDevice,
				frameWidth >> i, frameHeight >> i, false,
				surfaceFormat, DepthFormat.None);
		}

		renderTargetSwap = new RenderTarget2D(
			Main.graphics.GraphicsDevice,
			frameWidth >> Math.Min(maxBlurLevel - 1, 4 + BloomRadius), frameHeight >> Math.Min(maxBlurLevel - 1, 4 + BloomRadius),
			false, surfaceFormat, DepthFormat.None);
		filteredScreenTarget = new RenderTarget2D(
			Main.graphics.GraphicsDevice,
			screenWidth, screenHeight,
			false, surfaceFormat, DepthFormat.None);

		prevLightTexture = new RenderTarget2D(
			Main.graphics.GraphicsDevice,
			tileWidth, tileHeight,
			false, SurfaceFormat.Color, DepthFormat.None);
		lightSwapTarget = new RenderTarget2D(
			Main.graphics.GraphicsDevice,
			tileWidth, tileHeight,
			false, SurfaceFormat.Color, DepthFormat.None);

		shouldResetRenderTargets = false;
	}

	public void ExtractLightMap()
	{
		screenPosition = Main.screenPosition;

		int rows = lightTexture.Height;
		int cols = lightTexture.Width;

		Parallel.For(0, rows, i =>
		{
			for (int j = 0; j < cols; j++)
			{
				lightMap[i * cols + j] = Color.Transparent;
			}
		});

		startTileX = Math.Max(0, (int)(screenPosition.X / 16) - currentState.OffscreenTileCount);
		int endTileX = Math.Min(
			Main.maxTilesX - 1,
			(int)((screenPosition.X + screenWidth) / 16) + currentState.OffscreenTileCount);

		int i = 0;
		while (endTileX - startTileX < cols)
		{
			if (i % 2 == 0)
			{
				if (startTileX > 0)
				{
					startTileX--;
				}
			}
			else
			{
				if (endTileX < Main.maxTilesX - 1)
				{
					endTileX++;
				}
			}
			i++;
		}

		startTileY = Math.Max(0, (int)(screenPosition.Y / 16) - currentState.OffscreenTileCount);
		int endTileY = Math.Min(
			Main.maxTilesY - 1,
			(int)((screenPosition.Y + screenHeight) / 16) + currentState.OffscreenTileCount);

		while (endTileY - startTileY < rows)
		{
			if (i % 2 == 0)
			{
				if (startTileY > 0)
				{
					startTileY--;
				}
			}
			else
			{
				if (endTileY < Main.maxTilesY - 1)
				{
					endTileY++;
				}
			}
			i++;
		}

		Parallel.For(startTileY, endTileY, i =>
		{
			for (int j = startTileX; j < endTileX; j++)
			{
				int x = j - startTileX;
				int y = i - startTileY;
				var color = Lighting.GetColor(j, i);

				var s = color.ToVector3();
				if ((s.X + s.Y + s.Z) * 0.333f > currentState.LuminanceThreashold)
				{
					lightMap[y * cols + x] = color;
				}
			}
		});

		// CPU bound 性能大头
		if (enableLightUpload)
		{
			if (enableTemporalFilter)
			{
				lightSwapTarget.SetData(lightMap);
				var spriteBatch = Main.spriteBatch;
				var graphicsDevice = Main.graphics.GraphicsDevice;
				var temporalEffect = temporalInterpEffect.Value;
				graphicsDevice.SetRenderTarget(lightTexture);
				spriteBatch.Begin(
					SpriteSortMode.Immediate,
					BlendState.Opaque,
					SamplerState.PointClamp,
					DepthStencilState.None,
					RasterizerState.CullNone);
				graphicsDevice.Textures[1] = prevLightTexture;
				graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				temporalEffect.Parameters["uImageSize0"].SetValue(lightSwapTarget.Size());
				temporalEffect.Parameters["uImageSize1"].SetValue(prevLightTexture.Size());
				temporalEffect.Parameters["uAlpha"].SetValue(0.2f);
				temporalEffect.Parameters["uOffset"].SetValue(new Vector2(
					startTileX - oldStartTileX,
					startTileY - oldStartTileY));

				temporalEffect.CurrentTechnique.Passes[0].Apply();
				spriteBatch.Draw(lightSwapTarget, Vector2.Zero, Color.White);
				spriteBatch.End();

				graphicsDevice.SetRenderTarget(prevLightTexture);
				spriteBatch.Begin(
					SpriteSortMode.Immediate,
					BlendState.Opaque,
					SamplerState.PointClamp,
					DepthStencilState.None,
					RasterizerState.CullNone);
				spriteBatch.Draw(lightTexture, Vector2.Zero, Color.White);
				spriteBatch.End();
			}
			else
			{
				lightTexture.SetData(lightMap);
			}
		}

		oldStartTileX = startTileX;
		oldStartTileY = startTileY;
	}

	public void Apply(RenderTarget2D screenTarget1, RenderTarget2D screenTarget2)
	{
		UpdateParameters();
		if (!currentState.Enabled)
		{
			return;
		}

		// 因为涉及光照数据获取，这里暂时不支持截屏，原版会在截屏结束后把光照信息抹除
		if (screenTarget1 != Main.screenTarget)
		{
			return;
		}

		if (screenWidth != Main.screenWidth || screenHeight != Main.screenHeight
			|| shouldResetRenderTargets)
		{
			screenWidth = Main.screenWidth;
			screenHeight = Main.screenHeight;
			ResetLightMap();
			ResetRenderTargets();
		}
		ExtractLightMap();

		Generate(BloomRadius, 4);

		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;
		int x = startTileX * 16;
		int y = startTileY * 16;
		graphicsDevice.SetRenderTarget(filteredScreenTarget);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null, Main.Transform);
		spriteBatch.Draw(blurRenderTargets[0], new Rectangle(
			(int)(x - screenPosition.X),
			(int)(y - screenPosition.Y), blurRenderTargets[0].Width, blurRenderTargets[0].Height),
			Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(screenTarget2);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		spriteBatch.Draw(screenTarget1, Vector2.Zero,
			Color.White);
		spriteBatch.End();

		var fogEffect = fogScreenEffect.Value;
		graphicsDevice.SetRenderTarget(screenTarget1);
		graphicsDevice.Clear(Color.Transparent);
		fogEffect.Parameters["uImageSize0"].SetValue(new Vector2(screenWidth, screenHeight));

		// fogEffect.Parameters["uAbsorption"].SetValue(absorption);
		fogEffect.Parameters["uViewAbsorptionRatio"].SetValue(currentState.ViewAbsorptionRatio * currentState.ViewAbsorptionRatio);
		fogEffect.Parameters["uBloomIntensity"].SetValue(currentState.BloomIntensity);
		fogEffect.Parameters["uBloomScatteringRatio"].SetValue(currentState.BloomScatteringRatio);
		fogEffect.Parameters["uBloomAbsorptionRate"].SetValue(0f);
		fogEffect.Parameters["uFogScatterWithDistance"].SetValue(false);

		spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		{
			graphicsDevice.Textures[1] = filteredScreenTarget;
			graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
			fogEffect.CurrentTechnique.Passes[0].Apply();

			spriteBatch.Draw(screenTarget2, Vector2.Zero,
				Color.White);
		}
		spriteBatch.End();
	}

	private void Generate(int down, int up)
	{
		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;

		graphicsDevice.SetRenderTarget(blurRenderTargets[4]);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone, null);
		spriteBatch.Draw(lightTexture, Vector2.Zero, Color.White);
		spriteBatch.End();

		var filterBox = boxKernelEffect.Value;

		int downLevels = Math.Min(maxBlurLevel - 4, down);

		// Downsampling
		for (int i = 4; i < 4 + downLevels; i++)
		{
			int curWidth = frameWidth >> i + 1;
			int curHeight = frameHeight >> i + 1;
			Main.graphics.GraphicsDevice.SetRenderTarget(blurRenderTargets[i + 1]);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(
				SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone, null);
			filterBox.Parameters["uImageSize0"].SetValue(blurRenderTargets[i].Size());
			filterBox.Parameters["uDelta"].SetValue(1.0f);
			filterBox.CurrentTechnique.Passes[0].Apply();
			spriteBatch.Draw(blurRenderTargets[i], new Rectangle(0, 0, curWidth, curHeight),
				Color.White);
			spriteBatch.End();
		}

		ApplyGaussian(downLevels + 4);

		// Upsampling
		for (int i = 4 + downLevels - 1; i >= 0; i--)
		{
			int curWidth = frameWidth >> i;
			int curHeight = frameHeight >> i;
			graphicsDevice.SetRenderTarget(blurRenderTargets[i]);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(
				SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone, null);
			filterBox.Parameters["uImageSize0"].SetValue(blurRenderTargets[i + 1].Size());
			filterBox.Parameters["uDelta"].SetValue(1.0f);
			filterBox.CurrentTechnique.Passes[0].Apply();
			spriteBatch.Draw(blurRenderTargets[i + 1], new Rectangle(0, 0, curWidth, curHeight),
				Color.White);
			spriteBatch.End();
		}
	}

	private void ApplyGaussian(int level)
	{
		if (!useGaussian)
		{
			return;
		}

		var gaussianFilter = gaussianKernelEffect.Value;
		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;

		var target = blurRenderTargets[level];

		gaussianFilter.Parameters["uImageSize0"].SetValue(target.Size());
		gaussianFilter.Parameters["uDelta"].SetValue(1.0f);

		// Blur
		graphicsDevice.SetRenderTarget(renderTargetSwap);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		gaussianFilter.Parameters["uHorizontal"].SetValue(true);
		gaussianFilter.CurrentTechnique.Passes[0].Apply();
		spriteBatch.Draw(target, Vector2.Zero,
			Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(target);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		gaussianFilter.Parameters["uHorizontal"].SetValue(false);
		gaussianFilter.CurrentTechnique.Passes[0].Apply();
		spriteBatch.Draw(renderTargetSwap, Vector2.Zero,
			Color.White);
		spriteBatch.End();
	}
}
