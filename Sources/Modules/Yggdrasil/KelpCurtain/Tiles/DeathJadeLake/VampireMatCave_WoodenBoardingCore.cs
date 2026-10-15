using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class VampireMatCave_WoodenBoardingCore : ModTile, ISceneTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		DustType = ModContent.DustType<VampireMatCave_WoodenBoarding_Dust>();
		AddMapEntry(new Color(63, 44, 44));
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem) => base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);

	public void AddScene(int i, int j)
	{
		var vfx = new VampireMatCave_WoodenBoardingCore_VFX
		{
			Active = true,
			Visible = true,
			Position = new Point(i, j).ToWorldCoordinates(),
			OriginTilePos = new Point(i, j),
			OriginTileType = Type,
			Direction = 1,
			Texture = ModAsset.VampireMatCave_WoodenBoardingCore_VFX.Value,
		};
		Ins.VFXManager.Add(vfx);
	}
}
