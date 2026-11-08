namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class DarkForestSoilWall_Unsafe : DarkForestSoilWall
{
	public override string Texture => ModAsset.DarkForestSoilWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
