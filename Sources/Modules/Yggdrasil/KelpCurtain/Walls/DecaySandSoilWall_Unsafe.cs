namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class DecaySandSoilWall_Unsafe : DecaySandSoilWall
{
	public override string Texture => ModAsset.DecaySandSoilWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
