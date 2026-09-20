using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class VampireMatCave_WoodenBoarding : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		DustType = ModContent.DustType<VampireMatCave_WoodenBoarding_Dust>();
		AddMapEntry(new Color(63, 44, 44));
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) => base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
}
