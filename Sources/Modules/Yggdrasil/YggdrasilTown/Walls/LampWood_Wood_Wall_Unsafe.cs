namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class LampWood_Wood_Wall_Unsafe : LampWood_Wood_Wall
{
	public override string Texture => ModAsset.LampWood_Wood_Wall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
