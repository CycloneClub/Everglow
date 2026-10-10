namespace Everglow.Myth.TheFirefly.Walls;

public class GlowWoodWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustID.SpookyWood;
		AddMapEntry(new Color(10, 10, 10));
	}
}
