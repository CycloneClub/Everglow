namespace Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest;

public sealed class GreenRelicWall_Unsafe : GreenRelicWall
{
	public override string Texture => ModAsset.GreenRelicWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
