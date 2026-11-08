namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class GreenCourtWall_Unsafe : GreenCourtWall
{
	public override string Texture => ModAsset.GreenCourtWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
