using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class IsleBamboo_V : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<OldMoss>()] = true;
		Main.tileMerge[ModContent.TileType<OldMoss>()][Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<IsleBamboo_DryDust>();
		AddMapEntry(new Color(124, 103, 82));
	}
}
