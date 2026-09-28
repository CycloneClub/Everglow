using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.Union;

public class UnionMarbleTile_Khaki : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = false;

		Main.tileBlendAll[Type] = true;
		Main.tileBlockLight[Type] = true;

		DustType = ModContent.DustType<TownBrick_Khaki_Dust>();
		HitSound = default;

		AddMapEntry(new Color(194, 165, 134));
	}
}
