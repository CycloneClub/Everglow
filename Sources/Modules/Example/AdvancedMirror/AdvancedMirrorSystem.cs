namespace Everglow.Example.AdvancedMirror;

/// <summary>只将演示墙附加到通用反射系统。</summary>
public class AdvancedMirrorSystem : ModSystem
{
	public override void PostSetupContent() => AdvancedMirrorWallProfiles.Register();
}
