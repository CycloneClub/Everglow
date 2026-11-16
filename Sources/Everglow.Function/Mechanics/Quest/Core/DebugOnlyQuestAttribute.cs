using Everglow.Commons.FeatureFlags;

namespace Everglow.Commons.Mechanics.Quest.Core;

/// <summary>仅在启用调试模式时加载的任务，标记可由任务基类继承。</summary>
[AttributeUsage(AttributeTargets.Class, Inherited = true)]
public sealed class DebugOnlyQuestAttribute : Attribute
{
	public static bool IsEnabled(Type type) =>
		!Attribute.IsDefined(type, typeof(DebugOnlyQuestAttribute), inherit: true) || EverglowConfig.DebugMode;
}
