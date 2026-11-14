namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class DragonScaleWoodWall_Unsafe : DragonScaleWoodWall
{
	public override string Texture => ModAsset.DragonScaleWoodWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
