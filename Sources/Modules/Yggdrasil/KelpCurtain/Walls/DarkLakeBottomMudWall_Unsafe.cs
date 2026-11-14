namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class DarkLakeBottomMudWall_Unsafe : DarkLakeBottomMudWall
{
	public override string Texture => ModAsset.DarkLakeBottomMudWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
