namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class StoneDragonScaleWoodWall_Unsafe : StoneDragonScaleWoodWall
{
	public override string Texture => ModAsset.StoneDragonScaleWoodWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
