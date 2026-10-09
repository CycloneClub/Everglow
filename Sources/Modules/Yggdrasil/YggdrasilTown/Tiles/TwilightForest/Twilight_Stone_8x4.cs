using Everglow.Commons.TileHelper;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class Twilight_Stone_8x4 : ShapeDataTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileWaterDeath[Type] = false;

		DustType = ModContent.DustType<TwilightStone_Dust>();
		AddMapEntry(new Color(39, 50, 52));
	}
}
