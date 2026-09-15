using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

public class HumicMud : ModTile, ISceneTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkLakeBottomMud>()] = true;
		Main.tileMerge[ModContent.TileType<DarkLakeBottomMud>()][Type] = true;
		DustType = ModContent.DustType<HumicMud_Dust>();
		AddMapEntry(new Color(34, 43, 39));
	}

	public void AddScene(int i, int j)
	{
		HumicMud_Algae_fore leaf = new HumicMud_Algae_fore { Position = new Vector2(i, j) * 16, Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type };
		leaf.scale = 1f;
		leaf.style = (i + j) % 4;
		Ins.VFXManager.Add(leaf);
	}
}
