namespace Everglow.Yggdrasil.Common.Walls;

public sealed class YggdrasilBlackRockWall_Unsafe : YggdrasilBlackRockWall
{
	public override string Texture => ModAsset.YggdrasilBlackRockWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
