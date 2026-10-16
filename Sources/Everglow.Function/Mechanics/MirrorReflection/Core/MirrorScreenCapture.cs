using Everglow.Commons.ObjectPool;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

/// <summary>
/// 保存背景、前景和地图分层，并按调用方要求恢复画面；不注册绘制钩子或驱动反射流程。
/// </summary>
internal sealed class MirrorScreenCapture
{
	private ResourceLocker<RenderTarget2D[]> targets;
	private ResourceLocker<RenderTarget2D[]> mapLayers;
	private RenderTarget2D screen;
	private bool mapSeparated;

	private RenderTarget2D Background => targets.Resource[0];

	/// <summary>获取是否已保存背景并开始捕获前景。</summary>
	public bool IsCapturing { get; private set; }

	/// <summary>SaveForeground 后至 Release 前有效的借用纹理；调用方不得释放。</summary>
	public RenderTarget2D Foreground => targets.Resource[1];

	/// <summary>在原版屏幕目标上保存背景并清空前景区域；目标不匹配时不开始捕获。</summary>
	public void Begin()
	{
		GraphicsDevice device = Main.graphics.GraphicsDevice;
		RenderTargetBinding[] bindings = device.GetRenderTargets();
		if (bindings.Length != 1 || bindings[0].RenderTarget != Main.screenTarget)
		{
			return;
		}
		var state = Main.spriteBatch.GetState();
		if (state.HasValue)
		{
			Main.spriteBatch.End();
		}
		bool backgroundSaved = false;
		try
		{
			screen = Main.screenTarget;
			targets = Ins.RenderTargetPool.GetRenderTarget2DArray(2);
			CopyTo(Background, screen);
			backgroundSaved = true;
			device.SetRenderTarget(screen);
			device.Clear(Color.Transparent);
			IsCapturing = true;
		}
		finally
		{
			try
			{
				if (!IsCapturing)
				{
					try
					{
						if (backgroundSaved)
						{
							RestoreBackground();
						}
					}
					finally
					{
						Release();
					}
				}
			}
			finally
			{
				if (state.HasValue)
				{
					Main.spriteBatch.Begin(state.Value);
				}
			}
		}
	}

	/// <summary>保存地图前的前景并清空目标；返回是否需要在原版地图绘制后调用 EndMap。</summary>
	public bool BeginMap()
	{
		if (!IsCapturing || mapLayers != null || !IsDrawingToScreenTarget())
		{
			return false;
		}
		var state = Main.spriteBatch.GetState();
		if (state.HasValue)
		{
			Main.spriteBatch.End();
		}
		try
		{
			mapLayers = Ins.RenderTargetPool.GetRenderTarget2DArray(3);
			CopyTo(mapLayers.Resource[0], screen);
			Main.graphics.GraphicsDevice.SetRenderTarget(screen);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			return true;
		}
		finally
		{
			if (state.HasValue)
			{
				Main.spriteBatch.Begin(state.Value);
			}
		}
	}

	/// <summary>保存地图层并清空目标，供后续投射物和玩家继续绘制。</summary>
	public void EndMap()
	{
		var state = Main.spriteBatch.GetState();
		if (state.HasValue)
		{
			Main.spriteBatch.End();
		}
		try
		{
			CopyTo(mapLayers.Resource[1], screen);
			Main.graphics.GraphicsDevice.SetRenderTarget(screen);
			Main.graphics.GraphicsDevice.Clear(Color.Transparent);
			mapSeparated = true;
		}
		finally
		{
			if (state.HasValue)
			{
				Main.spriteBatch.Begin(state.Value);
			}
		}
	}

	/// <summary>保存完整前景用于倒影采样，排除叠加地图；调用方须先结束外层 SpriteBatch。</summary>
	public void SaveForeground()
	{
		if (mapSeparated)
		{
			CopyTo(mapLayers.Resource[2], screen);
			CopyTo(Foreground, mapLayers.Resource[0]);
			DrawTexture(mapLayers.Resource[2], BlendState.AlphaBlend);
		}
		else
		{
			CopyTo(Foreground, screen);
		}
	}

	/// <summary>重新绑定屏幕目标并恢复背景，覆盖中间绘制结果；调用方须先结束外层 SpriteBatch。</summary>
	public void RestoreBackground() => CopyTo(screen, Background);

	/// <summary>在当前屏幕目标上恢复原始前景及地图层级；调用方须先结束外层 SpriteBatch。</summary>
	public void RestoreForeground()
	{
		if (mapSeparated)
		{
			DrawTexture(mapLayers.Resource[0], BlendState.AlphaBlend);
			DrawTexture(mapLayers.Resource[1], BlendState.AlphaBlend);
			DrawTexture(mapLayers.Resource[2], BlendState.AlphaBlend);
		}
		else
		{
			DrawTexture(Foreground, BlendState.AlphaBlend);
		}
	}

	/// <summary>检查当前绘制目标是否仍为本次捕获的原版屏幕目标。</summary>
	public bool IsDrawingToScreenTarget()
	{
		RenderTargetBinding[] bindings = Main.graphics.GraphicsDevice.GetRenderTargets();
		return screen != null && !screen.IsDisposed && bindings.Length == 1 && bindings[0].RenderTarget == screen;
	}

	private static void CopyTo(RenderTarget2D destination, Texture2D source)
	{
		Main.graphics.GraphicsDevice.SetRenderTarget(destination);
		DrawTexture(source, BlendState.Opaque);
	}

	private static void DrawTexture(Texture2D texture, BlendState blend)
	{
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, blend, SamplerState.PointClamp,
			DepthStencilState.None, RasterizerState.CullNone);
		try
		{
			Main.spriteBatch.Draw(texture, Vector2.Zero, Color.White);
		}
		finally
		{
			Main.spriteBatch.End();
		}
	}

	/// <summary>归还捕获纹理并清空分层状态；不绘制或安排下一阶段。</summary>
	public void Release()
	{
		IsCapturing = false;
		mapSeparated = false;
		screen = null;
		mapLayers?.Release();
		mapLayers = null;
		targets?.Release();
		targets = null;
	}
}
