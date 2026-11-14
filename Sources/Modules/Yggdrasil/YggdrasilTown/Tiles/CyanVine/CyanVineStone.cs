namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;

public class CyanVineStone : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[ModContent.TileType<StoneScaleWood>()][Type] = true;
		TileID.Sets.ChecksForMerge[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<Dusts.CyanVine>();
		MineResist = 4f;
		Main.tileSpelunker[Type] = true;
		TileID.Sets.Ore[Type] = true;
		AddMapEntry(new Color(84, 130, 154));
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		// Keep the host's transition frames while vanilla handles slopes, half blocks and variants.
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<StoneScaleWood>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
