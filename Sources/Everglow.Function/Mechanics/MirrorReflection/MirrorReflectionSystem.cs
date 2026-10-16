using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Everglow.Commons.ObjectPool;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.MirrorReflection;

/// <summary>
/// 反射系统入口：加载原版材质、接受内容注册，并管理墙面收集、场景捕获和绘制的生命周期。
/// </summary>
public class MirrorReflectionSystem : ModSystem
{
	private MirrorWallReflectionCollector wallCollector;
	private MirrorScreenCapture sceneCapture;
	private MirrorScreenCaptureRenderHooks captureRenderHooks;
	private bool drawingWorld;
	private MirrorReflectionRenderer renderer;
	private MirrorNormalMapProvider normalMaps;
	private IReadOnlyList<MirrorWallReflectionCollector.ReflectionGroup> groups = [];

	/// <summary>
	/// 在内容的 PostSetupContent 中调用，附加或覆盖一种墙的反射材质。
	/// </summary>
	/// <param name="wallType">已分配的墙类型编号。</param>
	/// <param name="profile">该类型墙面使用的材质。</param>
	public void RegisterWall(int wallType, MirrorReflectionDefinition profile) => wallCollector.Register(wallType, profile);

	/// <summary>
	/// 在内容的 PostSetupContent 中注册动态材质，例如演示面板参数。
	/// </summary>
	/// <param name="wallType">已分配的墙类型编号。</param>
	/// <param name="provider">每次收集可见墙面时调用一次的参数提供者。</param>
	public void RegisterWall(int wallType, Func<MirrorReflectionDefinition> provider) => wallCollector.Register(wallType, provider);

	public override void Load()
	{
		wallCollector = new MirrorWallReflectionCollector(VanillaMirrorWallProfiles.Create());
		if (!Main.dedServ)
		{
			renderer = new MirrorReflectionRenderer();
			normalMaps = new MirrorNormalMapProvider();
			sceneCapture = new MirrorScreenCapture();
			captureRenderHooks = new MirrorScreenCaptureRenderHooks();
			// 原版绘制接入由 System 持有，所有阶段均由下方钩子主动调用。
			On_Main.DoDraw += DrawFrame;
			On_Main.DoDraw_WallsTilesNPCs += DrawWorld;
			On_Main.DrawWoF += BeginForeground;
			On_Main.DrawItems += EndForeground;
			On_Main.DrawMap += DrawMap;
		}
	}

	public override void Unload()
	{
		if (!Main.dedServ)
		{
			On_Main.DoDraw -= DrawFrame;
			On_Main.DoDraw_WallsTilesNPCs -= DrawWorld;
			On_Main.DrawWoF -= BeginForeground;
			On_Main.DrawItems -= EndForeground;
			On_Main.DrawMap -= DrawMap;
		}
		sceneCapture?.Release();
		captureRenderHooks?.Dispose();
		captureRenderHooks = null;
		drawingWorld = false;
		sceneCapture = null;
		normalMaps?.Dispose();
		normalMaps = null;
		renderer = null;
		wallCollector = null;
		groups = [];
	}

	public override void OnWorldLoad() => groups = [];

	public override void OnWorldUnload() => groups = [];

	private void DrawFrame(On_Main.orig_DoDraw orig, Main self, GameTime gameTime)
	{
		try
		{
			orig(self, gameTime);
		}
		finally
		{
			try
			{
				// 异常或其它模组跳过 DrawItems 时，恢复原画面，不绘制倒影。
				if (sceneCapture.IsCapturing && sceneCapture.IsDrawingToScreenTarget())
				{
					FinishCapture(false);
				}
			}
			finally
			{
				sceneCapture.Release();
				captureRenderHooks.Reset();
			}
		}
	}

	private void DrawWorld(On_Main.orig_DoDraw_WallsTilesNPCs orig, Main self)
	{
		drawingWorld = true;
		try
		{
			orig(self);
		}
		finally
		{
			drawingWorld = false;
		}
	}

	private void BeginForeground(On_Main.orig_DrawWoF orig, Main self)
	{
		// DrawWoF 紧接墙面绘制；即使没有血肉墙，原版也会调用此入口。
		if (drawingWorld && !sceneCapture.IsCapturing && PrepareCapture())
		{
			// Begin 恢复外层批次时就应启用捕获混合规则；未开始捕获则立即关闭。
			captureRenderHooks.Capturing = true;
			try
			{
				sceneCapture.Begin();
			}
			finally
			{
				captureRenderHooks.Capturing = sceneCapture.IsCapturing;
			}
		}
		orig(self);
	}

	private void EndForeground(On_Main.orig_DrawItems orig, Main self)
	{
		orig(self);
		if (sceneCapture.IsCapturing && sceneCapture.IsDrawingToScreenTarget())
		{
			FinishCapture(true);
		}
	}

	private void DrawMap(On_Main.orig_DrawMap orig, Main self, GameTime gameTime)
	{
		if (!sceneCapture.BeginMap())
		{
			orig(self, gameTime);
			return;
		}
		try
		{
			orig(self, gameTime);
		}
		finally
		{
			sceneCapture.EndMap();
		}
	}

	private bool PrepareCapture()
	{
		if (Main.dedServ || Main.gameMenu || Main.mapFullscreen || Main.drawToScreen || !Ins.VisualQuality.High)
		{
			return false;
		}
		MirrorReflectionConfig config = ModContent.GetInstance<MirrorReflectionConfig>();
		if (!config.Enabled)
		{
			return false;
		}
		// 1. 解析材质定义与参数；2. 收集可见墙格并完成分组。
		var materials = wallCollector.ResolveMaterials();
		groups = wallCollector.CollectVisibleWalls(materials);
		return groups.Count > 0;
	}

	/// <summary>主动完成前景捕获、倒影处理、场景合成与特效恢复；失败时恢复原画面。</summary>
	private void FinishCapture(bool drawReflection)
	{
		var state = Main.spriteBatch.GetState();
		if (state.HasValue)
		{
			Main.spriteBatch.End();
		}
		bool foregroundSaved = false;
		bool composed = false;
		ResourceLocker<RenderTarget2D> projectionTarget = null;
		ResourceLocker<RenderTarget2D[]> reflectionTargets = null;
		try
		{
			sceneCapture.SaveForeground();
			foregroundSaved = true;
			captureRenderHooks.Capturing = false;

			if (drawReflection)
			{
				projectionTarget = Ins.RenderTargetPool.GetRenderTarget2D();
				reflectionTargets = Ins.RenderTargetPool.GetRenderTarget2DArray(groups.Count);
				ProcessReflections(projectionTarget.Resource, reflectionTargets.Resource);
			}

			sceneCapture.RestoreBackground();
			if (drawReflection)
			{
				renderer.DrawReflection(reflectionTargets.Resource, groups);
			}
			sceneCapture.RestoreForeground();
			composed = true;

			// Warp/屏幕采样反射使用完整场景；普通 VFX 留在原层并参与倒影。
			captureRenderHooks.Flush();
		}
		finally
		{
			try
			{
				if (foregroundSaved && !composed)
				{
					sceneCapture.RestoreBackground();
					sceneCapture.RestoreForeground();
				}
			}
			finally
			{
				try
				{
					reflectionTargets?.Release();
					projectionTarget?.Release();
					sceneCapture.Release();
					captureRenderHooks.Reset();
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
	}

	/// <summary>取得法线纹理，并为各组生成共用的倒影。</summary>
	private void ProcessReflections(RenderTarget2D projectionTarget, RenderTarget2D[] reflectionTargets)
	{
		int sampleCount = ModContent.GetInstance<MirrorReflectionConfig>().GetBlurSampleCount();
		for (int i = 0; i < groups.Count; i++)
		{
			var definition = groups[i].Definition;
			Texture2D normal = normalMaps.GetTexture(definition.NormalPattern);
			renderer.ProcessForeground(sceneCapture.Foreground, projectionTarget, reflectionTargets[i], definition, normal, sampleCount);
		}
	}
}
