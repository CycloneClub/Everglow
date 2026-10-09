using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using MonoMod.Cil;
using MonoMod.RuntimeDetour;
using MonoMod.Utils;

namespace Everglow.Commons.Hooks;

public class HookManager : IHookManager
{
	public TerrariaFunction disableFlags;

	public Dictionary<CodeLayer, List<HookHandler>> hooks = requiredHookType.Keys.ToDictionary(l => l, l => new List<HookHandler>());

	private static readonly Dictionary<CodeLayer, Type> requiredHookType = new()
	{
		[CodeLayer.PreDrawFilter] = typeof(Action),
		[CodeLayer.PostDrawTiles] = typeof(Action),
		[CodeLayer.PostDrawProjectiles] = typeof(Action),
		[CodeLayer.PostDrawDusts] = typeof(Action),
		[CodeLayer.PostDrawNPCs] = typeof(Action),
		[CodeLayer.PostDrawPlayers] = typeof(Action),
		[CodeLayer.PostDrawMapIcons] = typeof(Action<Vector2, Vector2, Rectangle?, float>),
		[CodeLayer.PostDrawBG] = typeof(Action),
		[CodeLayer.PostUpdateEverything] = typeof(Action),
		[CodeLayer.PostUpdateProjectiles] = typeof(Action),
		[CodeLayer.PostUpdatePlayers] = typeof(Action),
		[CodeLayer.PostUpdateNPCs] = typeof(Action),
		[CodeLayer.PostUpdateDusts] = typeof(Action),
		[CodeLayer.PostUpdateInvasions] = typeof(Action),

		[CodeLayer.PreEnterWorld_Single] = typeof(Action),
		[CodeLayer.PostEnterWorld_Single] = typeof(Action),
		[CodeLayer.PreEnterWorld_Server] = typeof(Action),
		[CodeLayer.PostEnterWorld_Server] = typeof(Action),
		[CodeLayer.PreSaveAndQuit] = typeof(Action),
		[CodeLayer.PostSaveAndQuit] = typeof(Action),

		[CodeLayer.ResolutionChanged] = typeof(Action<Vector2>),
	};

	private List<IDisposable> monoHooks = new();

	private static string ToString(Type type)
	{
		var sb = new StringBuilder();
		if (type.IsGenericType)
		{
			sb.Append(type.Name, 0, type.Name.Length - 2)
				.Append('<')
				.Append(string.Join(',', type.GetGenericArguments().Select(ToString)))
				.Append('>');
		}
		else
		{
			sb.Append(type.Name);
		}
		return sb.ToString();
	}

	public IHookHandler AddHook<T>(CodeLayer layer, T hook, [CallerMemberName] string name = default, [CallerFilePath] string file = default)
		where T : Delegate
	{
		Debug.Assert(typeof(T).IsAssignableTo(requiredHookType[layer]), $"Hook type not match, {ToString(requiredHookType[layer])} != {ToString(typeof(T))}");
		var handler = new HookHandler(layer, hook, $"{Path.GetFileNameWithoutExtension(file)}.{name}");
		hooks[layer].Add(handler);
		return handler;
	}

	public IHookHandler AddHook(MethodInfo target, Delegate hook)
	{
		var on = new Hook(target, hook);
		monoHooks.Add(on);
		return new OnHookHandler
		{
			Hook = on,
		};
	}

	public IHookHandler AddHook(MethodInfo target, ILContext.Manipulator hook)
	{
		var il = new ILHook(target, hook);
		monoHooks.Add(il);
		return new ILHookHandler
		{
			Hook = il,
		};
	}

	public void Disable(TerrariaFunction function)
	{
		disableFlags |= function;
	}

	public void Dispose()
	{
		foreach (var hook in monoHooks)
		{
			hook.Dispose();
		}
		ClearReflectionCache();
		GC.SuppressFinalize(this);
	}

	public static void ClearReflectionCache()
	{
		ReflectionHelper.AssemblyCache.Clear();
		ReflectionHelper.AssembliesCache.Clear();
		ReflectionHelper.ResolveReflectionCache.Clear();
	}

	public void Enable(TerrariaFunction function)
	{
		disableFlags &= ~function;
	}

	public void RemoveHook(IHookHandler handler)
	{
		switch (handler)
		{
			case ILHookHandler il:
				il.Hook.Dispose();
				monoHooks.Remove(il.Hook);
				break;

			case OnHookHandler on:
				on.Hook.Dispose();
				monoHooks.Remove(on.Hook);
				break;

			case HookHandler hook:
				hooks[hook.Layer].Remove(hook);
				break;
		}
	}
}
