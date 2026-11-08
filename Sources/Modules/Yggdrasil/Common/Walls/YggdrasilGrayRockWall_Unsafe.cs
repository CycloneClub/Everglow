namespace Everglow.Yggdrasil.Common.Walls;

public sealed class YggdrasilGrayRockWall_Unsafe : YggdrasilGrayRockWall
{
	public override string Texture => ModAsset.YggdrasilGrayRockWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
