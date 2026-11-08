namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public sealed class FemaleLampWoodWall_Unsafe : FemaleLampWoodWall
{
	public override string Texture => ModAsset.FemaleLampWoodWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
