namespace Everglow.Yggdrasil.YggdrasilTown.Walls;

public class StoneDragonScaleWoodWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.WoodFurniture;
		AddMapEntry(new Color(28, 26, 25));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override bool Drop(int i, int j, ref int type) => false;
}
