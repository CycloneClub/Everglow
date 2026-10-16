using Everglow.Commons.Mechanics.MirrorReflection;
using Everglow.Commons.Mechanics.MirrorReflection.Core;

namespace Everglow.Example.AdvancedMirror;

/// <summary>只注册演示墙；原版墙列表由 Function 中的主系统加载。</summary>
internal static class AdvancedMirrorWallProfiles
{
	public static void Register() => ModContent.GetInstance<MirrorReflectionSystem>()
		.RegisterWall(ModContent.WallType<AdvancedMirrorWall>(), ReadPanel);

	private static MirrorReflectionDefinition ReadPanel() => ModContent.GetInstance<AdvancedMirrorConfigUI>().Definition;
}
