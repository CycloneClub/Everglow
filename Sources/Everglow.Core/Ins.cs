using System.Diagnostics.Contracts;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Modules;
using Everglow.Commons.ObjectPool;
using Everglow.Commons.VFX;
using log4net;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Everglow.Commons;

/// <summary>
/// 依赖注入
/// </summary>
public static class Ins
{
	public static VFXBatch Batch => Get<VFXBatch>();

	public static SpriteBatch SpriteBatch => Get<SpriteBatch>();

	public static GraphicsDevice Device => Get<GraphicsDevice>();

	public static IHookManager HookManager => Get<IHookManager>();

	public static ILog Logger => Get<ILog>();

	public static IMainThreadContext MainThread => Get<IMainThreadContext>();

	public static ModuleManager ModuleManager => Get<ModuleManager>();

	public static RenderTargetPool RenderTargetPool => Get<RenderTargetPool>();

	public static IVFXManager VFXManager => Get<IVFXManager>();

	public static IVisualQualityController VisualQuality => Get<IVisualQualityController>();

	public static void Add<T>()
		where T : class
		=> services.AddSingleton<T>();

	public static void Add<TService, TImplementation>()
		where TService : class
		where TImplementation : class, TService
		=> services.AddSingleton<TService, TImplementation>();

	public static void Add<T>(Func<IServiceProvider, T> factory)
		where T : class
		=> services.AddSingleton(factory);

	public static void Add<T>(T instance)
		where T : class
		=> services.AddSingleton(instance);

	public static T Get<T>()
		where T : class
	{
		var service = provider?.GetService<T>();
		Debug.Assert(service != null);
		return service;
	}

	public static void Begin() => services = new ServiceCollection();

	public static void End() => provider = services.BuildServiceProvider();

	public static void Clear()
	{
		services.RemoveAll<GraphicsDevice>();
		services.RemoveAll<SpriteBatch>();
		provider?.Dispose();
		services.Clear();
	}

	private static ServiceCollection services = null;

	private static ServiceProvider provider = null;
}
