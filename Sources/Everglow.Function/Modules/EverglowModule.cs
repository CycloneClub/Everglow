using System.Reflection;

namespace Everglow.Commons.Modules;

public abstract class EverglowModule : IModule
{
	public EverglowModule()
	{
		Code = GetType().Assembly;
	}

	public Assembly Code { get; }

	public virtual bool Condition => true;

	public abstract string Name { get; }

	public virtual void Load()
	{
	}

	public virtual void Unload()
	{
	}
}
