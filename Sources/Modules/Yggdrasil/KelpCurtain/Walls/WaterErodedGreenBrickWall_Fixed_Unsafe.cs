namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class WaterErodedGreenBrickWall_Fixed_Unsafe : WaterErodedGreenBrickWall_Fixed
{
	public override string Texture => ModAsset.WaterErodedGreenBrickWall_Fixed_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
