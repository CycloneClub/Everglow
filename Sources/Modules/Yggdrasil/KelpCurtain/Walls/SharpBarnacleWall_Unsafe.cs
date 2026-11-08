namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class SharpBarnacleWall_Unsafe : SharpBarnacleWall
{
	public override string Texture => ModAsset.SharpBarnacleWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
