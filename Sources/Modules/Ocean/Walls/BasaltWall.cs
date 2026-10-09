namespace Everglow.Ocean.Walls;

public class BasaltWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.Granite;
		AddMapEntry(new Color(1, 1, 1));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
