namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class OldMossWall_Unsafe : OldMossWall
{
	public override string Texture => ModAsset.OldMossWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
