using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.IsleOfBloom;

public class MossyDockWood : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<OldMoss>()] = true;
		Main.tileMerge[ModContent.TileType<OldMoss>()][Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<MossyDockWoodDust>();
		AddMapEntry(new Color(109, 67, 55));
	}

	public override bool CanExplode(int i, int j)
	{
		var tileTop = Main.tile[i, j - 1];
		if (tileTop.HasTile && tileTop.TileType == ModContent.TileType<BlackAwningBoatSign>())
		{
			return false;
		}
		return base.CanExplode(i, j);
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		var tileTop = Main.tile[i, j - 1];
		if (tileTop.HasTile && tileTop.TileType == ModContent.TileType<BlackAwningBoatSign>())
		{
			noBreak = true;
		}
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}
}
