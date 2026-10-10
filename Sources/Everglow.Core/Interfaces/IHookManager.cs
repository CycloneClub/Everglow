using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Commons.Enums;
using MonoMod.Cil;

namespace Everglow.Commons.Interfaces;

public interface IHookManager : IDisposable
{
	public IHookHandler AddHook<T>(CodeLayer layer, T hook, [CallerMemberName] string name = default, [CallerFilePath] string file = default)
		where T : Delegate;

	public IHookHandler AddHook(MethodInfo target, Delegate hook);

	public IHookHandler AddHook(MethodInfo target, ILContext.Manipulator hook);

	public void RemoveHook(IHookHandler handler);

	public void Disable(TerrariaFunction function);

	public void Enable(TerrariaFunction function);
}
