namespace Everglow.Ocean.Walls;

public class BasaltWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = 240;
		AddMapEntry(new Color(1, 1, 1));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
