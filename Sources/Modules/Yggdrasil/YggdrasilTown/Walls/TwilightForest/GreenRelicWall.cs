using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;

namespace Everglow.Yggdrasil.YggdrasilTown.Walls.TwilightForest;

public class GreenRelicWall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_Dust>();
		AddMapEntry(new Color(10, 31, 31));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
