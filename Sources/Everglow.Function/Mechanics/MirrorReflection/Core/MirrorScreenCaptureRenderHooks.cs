using Everglow.Commons.Graphics;
using Everglow.Commons.Interfaces;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.Mechanics.MirrorReflection.Core;

/// <summary>
/// 捕获期间的混合状态与 VFX 绘制钩子：保留加法光照的覆盖率，将依赖完整场景的绘制延后执行。
/// </summary>
internal sealed class MirrorScreenCaptureRenderHooks : IDisposable
{
	private readonly List<Action> deferred = [];
	private readonly BlendState additiveLight = CustomBlendStates.NewBlendState(
		"Mirror additive light", Blend.SourceAlpha, Blend.Zero, Blend.One, Blend.One);

	private readonly IHookHandler blendHook;
	private readonly IHookHandler warpHook;
	private readonly IHookHandler screenReflectionHook;

	public MirrorScreenCaptureRenderHooks()
	{
		blendHook = Ins.HookManager.AddHook(
			typeof(GraphicsDevice).GetProperty(nameof(GraphicsDevice.BlendState)).SetMethod,
			(Action<Action<GraphicsDevice, BlendState>, GraphicsDevice, BlendState>)SetBlendState);
		warpHook = Ins.HookManager.AddHook(
			typeof(WarpPipeline).GetMethod(nameof(WarpPipeline.Render)),
			(Action<Action<WarpPipeline, IEnumerable<IVisual>>, WarpPipeline, IEnumerable<IVisual>>)DrawDeferred);
		screenReflectionHook = Ins.HookManager.AddHook(
			typeof(ScreenReflectionPipeline).GetMethod(nameof(ScreenReflectionPipeline.Render)),
			(Action<Action<ScreenReflectionPipeline, IEnumerable<IVisual>>, ScreenReflectionPipeline, IEnumerable<IVisual>>)DrawDeferred);
	}

	public bool Capturing { get; set; }

	public void Flush()
	{
		Capturing = false;
		try
		{
			foreach (var call in deferred)
			{
				call();
			}
		}
		finally
		{
			deferred.Clear();
		}
	}

	public void Reset()
	{
		Capturing = false;
		deferred.Clear();
	}

	public void Dispose()
	{
		Reset();
		Ins.HookManager.RemoveHook(screenReflectionHook);
		Ins.HookManager.RemoveHook(warpHook);
		Ins.HookManager.RemoveHook(blendHook);
		Main.QueueMainThreadAction(additiveLight.Dispose);
	}

	private void SetBlendState(Action<GraphicsDevice, BlendState> orig, GraphicsDevice device, BlendState blend)
	{
		if (Capturing && ReferenceEquals(blend, BlendState.Additive))
		{
			RenderTargetBinding[] bindings = device.GetRenderTargets();
			if (bindings.Length == 1 && (bindings[0].RenderTarget == Main.screenTarget || bindings[0].RenderTarget == Main.screenTargetSwap))
			{
				// 保持原 Additive 的 RGB 公式；发光不能增加前景对背景的遮挡。
				blend = additiveLight;
			}
		}
		orig(device, blend);
	}

	private void DrawDeferred<T>(Action<T, IEnumerable<IVisual>> orig, T pipeline, IEnumerable<IVisual> visuals)
	{
		if (Capturing)
		{
			// 仅推迟读取完整屏幕的两条管线；普通 TileVFX 保留原来的层级和反射。
			IVisual[] snapshot = visuals.ToArray();
			deferred.Add(() => orig(pipeline, snapshot));
			return;
		}
		orig(pipeline, visuals);
	}
}
