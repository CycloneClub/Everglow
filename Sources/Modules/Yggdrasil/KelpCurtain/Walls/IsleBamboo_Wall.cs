using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public class IsleBamboo_Wall : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = DustType = ModContent.DustType<IsleBamboo_DryDust>();

		AddMapEntry(new Color(75, 52, 23));
	}
}
