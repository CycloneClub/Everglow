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
};
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

	private Asset<Effect> m_boxKernelEffect;
	private Asset<Effect> m_gaussianKernelEffect;
	private Asset<Effect> m_fogScreenEffect;
	private Asset<Effect> m_temporalInterpEffect;

	private RenderTarget2D[] m_blurRenderTargets;
	private RenderTarget2D m_renderTargetSwap;
	private RenderTarget2D m_filteredScreenTarget;


	private Color[] m_lightMap;
	private RenderTarget2D m_lightTexture;
	private RenderTarget2D m_prevLightTexture;
	private RenderTarget2D m_lightSwapTarget;

	private int m_frameWidth, m_frameHeight;
	private int m_screenWidth, m_screenHeight;
	private int m_tileWidth, m_tileHeight;
	private bool m_shouldResetRenderTargets;

	private readonly int MAX_BLUR_LEVELS = 10;

	private int m_maxBlurLevel;

	private int m_startTileX, m_startTileY;
	private int m_oldStartTileX, m_oldStartTileY;

	private bool m_enableLightUpload;
	private bool m_enableTemporalFilter;
	private Vector2 m_screenPosition;
	private const SurfaceFormat m_surfaceFormat = SurfaceFormat.Rgba1010102;

	private int m_switchCounter = 0;
	private int m_totalSwitchCounter = 0;
	private bool m_useGaussian = true;
	private FogState m_beginState, m_currentState, m_targetState;


	/// <summary>
	/// 光晕效果的模糊卷积核半径，该值为2^k
	/// </summary>
	public int BloomRadius
	{
		get
		{
			return m_currentState.BloomRadius;
		}
		set
		{

			m_currentState.BloomRadius = value;
		}
	}

	public FogPass()
	{
		m_screenWidth = 0;
		m_screenHeight = 0;

		m_boxKernelEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/BoxFilter"); // QuickEffect does not work, conversion failed.
		m_gaussianKernelEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/GBlur"); // Same as above
		m_fogScreenEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/Fog"); // Same as above
		m_temporalInterpEffect = ModContent.Request<Effect>("Everglow/Myth/Effects/Temporal"); // Same as above

		m_blurRenderTargets = new RenderTarget2D[MAX_BLUR_LEVELS];
		m_shouldResetRenderTargets = true;
		m_enableTemporalFilter = false;
		m_enableLightUpload = true;
		m_enableTemporalFilter = true;
		m_screenPosition = Vector2.Zero;
	}

	public void Preprocess()
	{
		UpdateParameters();
	}

	private void UpdateParameters()
	{
		var fogConfig = ModContent.GetInstance<FogConfigs>();

		BloomRadius = fogConfig.MaxBloomRadius;
		m_currentState.BloomIntensity = fogConfig.BloomIntensity;
		m_currentState.LuminanceThreashold = fogConfig.LightLuminanceThreashold;
		m_currentState.ViewAbsorptionRatio = new Vector3(fogConfig.FogAbsorptionR,
			fogConfig.FogAbsorptionG,
			fogConfig.FogAbsorptionB);
		m_currentState.BloomScatteringRatio = fogConfig.FogBloomRate;
		// m_currentState.FogScatterWithDistance = fogConfig.FogScatterWithDistance;

		m_shouldResetRenderTargets |= m_currentState.OffscreenTileCount != fogConfig.OffscreenTiles;
		m_currentState.OffscreenTileCount = fogConfig.OffscreenTiles;
		m_useGaussian = fogConfig.GaussianKernel;
		m_enableLightUpload = fogConfig.EnableLightUpload;
		m_enableTemporalFilter = fogConfig.EnableTemporalInterp;

		m_currentState.Enabled = fogConfig.EnableScattering;
	}

	private void ResetLightMap()
	{
		m_tileWidth = (m_screenWidth + 15) / 16 + m_currentState.OffscreenTileCount * 2 + 2;
		m_tileHeight = (m_screenHeight + 15) / 16 + m_currentState.OffscreenTileCount * 2 + 2;
		m_lightMap = new Color[m_tileWidth * m_tileHeight];
		m_lightTexture = new RenderTarget2D(Main.graphics.GraphicsDevice, m_tileWidth, m_tileHeight, false,
			SurfaceFormat.Color, DepthFormat.None);
	}

	private void DisposePrevRenderTargets()
	{
		for (int i = 0; i < m_blurRenderTargets.Length; i++)
		{
			m_blurRenderTargets[i]?.Dispose();
		}
		m_renderTargetSwap?.Dispose();
		m_filteredScreenTarget?.Dispose();
	}

	public void SwitchState(in FogState state, int interpTime)
	{
		m_beginState = m_currentState;
		m_targetState = state;
		m_totalSwitchCounter = interpTime;
		m_switchCounter = interpTime;
	}

	public void Update()
	{
		if (m_switchCounter > 0)
		{
			// 渐变开始
			if (m_switchCounter == m_totalSwitchCounter)
			{
				if (m_targetState.Enabled)
				{
					m_currentState.Enabled = true;
					m_currentState.BloomRadius = m_targetState.BloomRadius;
					m_currentState.OffscreenTileCount = m_targetState.OffscreenTileCount;
					m_shouldResetRenderTargets = true;
				}
			}
			m_switchCounter--;

			// 渐变结束
			if (m_switchCounter == 0 && !m_targetState.Enabled)
			{
				m_currentState.Enabled = false;
				m_currentState.BloomRadius = m_targetState.BloomRadius;
				m_currentState.OffscreenTileCount = m_targetState.OffscreenTileCount;
				m_shouldResetRenderTargets = true;
			}

			float progress = 1f - m_switchCounter / (float)m_totalSwitchCounter;

			m_currentState.BloomIntensity = MathHelper.Lerp(m_beginState.BloomIntensity,
				m_targetState.BloomIntensity, progress);
			m_currentState.BloomScatteringRatio = MathHelper.Lerp(m_beginState.BloomScatteringRatio,
				m_targetState.BloomScatteringRatio, progress);
			m_currentState.LuminanceThreashold = MathHelper.Lerp(m_beginState.LuminanceThreashold,
				m_targetState.LuminanceThreashold, progress);
			m_currentState.ViewAbsorptionRatio = Vector3.Lerp(m_beginState.ViewAbsorptionRatio,
				m_targetState.ViewAbsorptionRatio, progress * progress);
		}

		if (Main.time % 400 < 1)
		{
			if ((int)(Main.time / 400) % 2 == 0)
				SwitchState(Default, 150);
			else
			{
				SwitchState(DayThickFog, 150);
			}
		}
	}

	private void ResetRenderTargets()
	{
		DisposePrevRenderTargets();

		m_frameWidth = m_tileWidth * 16;
		m_frameHeight = m_tileHeight * 16;

		int l = 0;
		for (; l < MAX_BLUR_LEVELS; l++)
		{
			if (m_frameWidth >> l == 0 || m_frameHeight >> l == 0)
			{
				break;
			}

			m_blurRenderTargets[l] = new RenderTarget2D(Main.graphics.GraphicsDevice,
					m_frameWidth >> l, m_frameWidth >> l, false,
					m_surfaceFormat, DepthFormat.None);
		}
		m_maxBlurLevel = Math.Min(l, 8);

		for (int i = 0; i < m_maxBlurLevel; i++)
		{
			m_blurRenderTargets[i] = new RenderTarget2D(Main.graphics.GraphicsDevice,
					m_frameWidth >> i, m_frameHeight >> i, false,
					m_surfaceFormat, DepthFormat.None);
		}

		m_renderTargetSwap = new RenderTarget2D(Main.graphics.GraphicsDevice,
				m_frameWidth >> Math.Min(m_maxBlurLevel - 1, 4 + BloomRadius), m_frameHeight >> Math.Min(m_maxBlurLevel - 1, 4 + BloomRadius),
				false, m_surfaceFormat, DepthFormat.None);
		m_filteredScreenTarget = new RenderTarget2D(Main.graphics.GraphicsDevice,
				m_screenWidth, m_screenHeight,
				false, m_surfaceFormat, DepthFormat.None);

		m_prevLightTexture = new RenderTarget2D(Main.graphics.GraphicsDevice,
			m_tileWidth, m_tileHeight,
			false, SurfaceFormat.Color, DepthFormat.None);
		m_lightSwapTarget = new RenderTarget2D(Main.graphics.GraphicsDevice,
			m_tileWidth, m_tileHeight,
			false, SurfaceFormat.Color, DepthFormat.None);

		m_shouldResetRenderTargets = false;
	}

	public void ExtractLightMap()
	{
		m_screenPosition = Main.screenPosition;

		int rows = m_lightTexture.Height;
		int cols = m_lightTexture.Width;

		Parallel.For(0, rows, i =>
		{
			for (int j = 0; j < cols; j++)
			{
				m_lightMap[i * cols + j] = Color.Transparent;
			}
		});

		m_startTileX = Math.Max(0, (int)(m_screenPosition.X / 16) - m_currentState.OffscreenTileCount);
		int endTileX = Math.Min(Main.maxTilesX - 1,
			(int)((m_screenPosition.X + m_screenWidth) / 16) + m_currentState.OffscreenTileCount);

		int i = 0;
		while (endTileX - m_startTileX < cols)
		{
			if (i % 2 == 0)
			{
				if (m_startTileX > 0)
				{
					m_startTileX--;
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

		m_startTileY = Math.Max(0, (int)(m_screenPosition.Y / 16) - m_currentState.OffscreenTileCount);
		int endTileY = Math.Min(Main.maxTilesY - 1,
			(int)((m_screenPosition.Y + m_screenHeight) / 16) + m_currentState.OffscreenTileCount);

		while (endTileY - m_startTileY < rows)
		{
			if (i % 2 == 0)
			{
				if (m_startTileY > 0)
				{
					m_startTileY--;
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

		Parallel.For(m_startTileY, endTileY, i =>
		{
			for (int j = m_startTileX; j < endTileX; j++)
			{
				int x = j - m_startTileX;
				int y = i - m_startTileY;
				var color = Lighting.GetColor(j, i);

				var s = color.ToVector3();
				if ((s.X + s.Y + s.Z) * 0.333f > m_currentState.LuminanceThreashold)
				{
					m_lightMap[y * cols + x] = color;
				}
			}
		});

		// CPU bound 性能大头
		if (m_enableLightUpload)
		{
			if (m_enableTemporalFilter)
			{
				m_lightSwapTarget.SetData(m_lightMap);
				var spriteBatch = Main.spriteBatch;
				var graphicsDevice = Main.graphics.GraphicsDevice;
				var temporalEffect = m_temporalInterpEffect.Value;
				graphicsDevice.SetRenderTarget(m_lightTexture);
				spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.Opaque,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);
				graphicsDevice.Textures[1] = m_prevLightTexture;
				graphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
				temporalEffect.Parameters["uImageSize0"].SetValue(m_lightSwapTarget.Size());
				temporalEffect.Parameters["uImageSize1"].SetValue(m_prevLightTexture.Size());
				temporalEffect.Parameters["uAlpha"].SetValue(0.2f);
				temporalEffect.Parameters["uOffset"].SetValue(new Vector2(m_startTileX - m_oldStartTileX,
					m_startTileY - m_oldStartTileY));

				temporalEffect.CurrentTechnique.Passes[0].Apply();
				spriteBatch.Draw(m_lightSwapTarget, Vector2.Zero, Color.White);
				spriteBatch.End();

				graphicsDevice.SetRenderTarget(m_prevLightTexture);
				spriteBatch.Begin(SpriteSortMode.Immediate,
									BlendState.Opaque,
									SamplerState.PointClamp,
									DepthStencilState.None,
									RasterizerState.CullNone);
				spriteBatch.Draw(m_lightTexture, Vector2.Zero, Color.White);
				spriteBatch.End();
			}
			else
			{
				m_lightTexture.SetData(m_lightMap);
			}
		}

		m_oldStartTileX = m_startTileX;
		m_oldStartTileY = m_startTileY;
	}

	public void Apply(RenderTarget2D screenTarget1, RenderTarget2D screenTarget2)
	{
		UpdateParameters();
		if (!m_currentState.Enabled)
		{
			return;
		}

		// 因为涉及光照数据获取，这里暂时不支持截屏，原版会在截屏结束后把光照信息抹除
		if (screenTarget1 != Main.screenTarget)
		{
			return;
		}

		if (m_screenWidth != Main.screenWidth || m_screenHeight != Main.screenHeight
			|| m_shouldResetRenderTargets)
		{
			m_screenWidth = Main.screenWidth;
			m_screenHeight = Main.screenHeight;
			ResetLightMap();
			ResetRenderTargets();
		}
		ExtractLightMap();

		Generate(BloomRadius, 4);

		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;
		int x = m_startTileX * 16;
		int y = m_startTileY * 16;
		graphicsDevice.SetRenderTarget(m_filteredScreenTarget);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone, null, Main.Transform);
		spriteBatch.Draw(m_blurRenderTargets[0], new Rectangle((int)(x - m_screenPosition.X),
			(int)(y - m_screenPosition.Y), m_blurRenderTargets[0].Width, m_blurRenderTargets[0].Height),
			Color.White);
		spriteBatch.End();

		graphicsDevice.SetRenderTarget(screenTarget2);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone, null);
		spriteBatch.Draw(screenTarget1, Vector2.Zero,
			Color.White);
		spriteBatch.End();

		var fogEffect = m_fogScreenEffect.Value;
		graphicsDevice.SetRenderTarget(screenTarget1);
		graphicsDevice.Clear(Color.Transparent);
		fogEffect.Parameters["uImageSize0"].SetValue(new Vector2(m_screenWidth, m_screenHeight));

		// fogEffect.Parameters["uAbsorption"].SetValue(absorption);
		fogEffect.Parameters["uViewAbsorptionRatio"].SetValue(m_currentState.ViewAbsorptionRatio * m_currentState.ViewAbsorptionRatio);
		fogEffect.Parameters["uBloomIntensity"].SetValue(m_currentState.BloomIntensity);
		fogEffect.Parameters["uBloomScatteringRatio"].SetValue(m_currentState.BloomScatteringRatio);
		fogEffect.Parameters["uBloomAbsorptionRate"].SetValue(0f);
		fogEffect.Parameters["uFogScatterWithDistance"].SetValue(false);

		spriteBatch.Begin(SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		{
			graphicsDevice.Textures[1] = m_filteredScreenTarget;
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

		graphicsDevice.SetRenderTarget(m_blurRenderTargets[4]);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone, null);
		spriteBatch.Draw(m_lightTexture, Vector2.Zero, Color.White);
		spriteBatch.End();

		var filterBox = m_boxKernelEffect.Value;

		int downLevels = Math.Min(m_maxBlurLevel - 4, down);

		// Downsampling
		for (int i = 4; i < 4 + downLevels; i++)
		{
			int curWidth = m_frameWidth >> i + 1;
			int curHeight = m_frameHeight >> i + 1;
			Main.graphics.GraphicsDevice.SetRenderTarget(m_blurRenderTargets[i + 1]);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.None,
				RasterizerState.CullNone, null);
			filterBox.Parameters["uImageSize0"].SetValue(m_blurRenderTargets[i].Size());
			filterBox.Parameters["uDelta"].SetValue(1.0f);
			filterBox.CurrentTechnique.Passes[0].Apply();
			spriteBatch.Draw(m_blurRenderTargets[i], new Rectangle(0, 0, curWidth, curHeight),
				Color.White);
			spriteBatch.End();
		}

		ApplyGaussian(downLevels + 4);

		// Upsampling
		for (int i = 4 + downLevels - 1; i >= 0; i--)
		{
			int curWidth = m_frameWidth >> i;
			int curHeight = m_frameHeight >> i;
			graphicsDevice.SetRenderTarget(m_blurRenderTargets[i]);
			graphicsDevice.Clear(Color.Transparent);
			spriteBatch.Begin(SpriteSortMode.Immediate,
				BlendState.Opaque,
				SamplerState.PointClamp,
				DepthStencilState.Default,
				RasterizerState.CullNone, null);
			filterBox.Parameters["uImageSize0"].SetValue(m_blurRenderTargets[i + 1].Size());
			filterBox.Parameters["uDelta"].SetValue(1.0f);
			filterBox.CurrentTechnique.Passes[0].Apply();
			spriteBatch.Draw(m_blurRenderTargets[i + 1], new Rectangle(0, 0, curWidth, curHeight),
				Color.White);
			spriteBatch.End();
		}
	}

	private void ApplyGaussian(int level)
	{
		if (!m_useGaussian)
		{
			return;
		}

		var gaussianFilter = m_gaussianKernelEffect.Value;
		var spriteBatch = Main.spriteBatch;
		var graphicsDevice = Main.graphics.GraphicsDevice;

		var target = m_blurRenderTargets[level];

		gaussianFilter.Parameters["uImageSize0"].SetValue(target.Size());
		gaussianFilter.Parameters["uDelta"].SetValue(1.0f);

		// Blur
		graphicsDevice.SetRenderTarget(m_renderTargetSwap);
		graphicsDevice.Clear(Color.Transparent);
		spriteBatch.Begin(SpriteSortMode.Immediate,
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
		spriteBatch.Begin(SpriteSortMode.Immediate,
			BlendState.Opaque,
			SamplerState.PointClamp,
			DepthStencilState.Default,
			RasterizerState.CullNone, null);
		gaussianFilter.Parameters["uHorizontal"].SetValue(false);
		gaussianFilter.CurrentTechnique.Passes[0].Apply();
		spriteBatch.Draw(m_renderTargetSwap, Vector2.Zero,
			Color.White);
		spriteBatch.End();
	}
}
