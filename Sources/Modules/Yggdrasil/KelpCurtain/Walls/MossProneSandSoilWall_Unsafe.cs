namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public sealed class MossProneSandSoilWall_Unsafe : MossProneSandSoilWall
{
	public override string Texture => ModAsset.MossProneSandSoilWall_Mod;

	public override void SetStaticDefaults()
	{
		base.SetStaticDefaults();
		Main.wallHouse[Type] = false;
	}
}
