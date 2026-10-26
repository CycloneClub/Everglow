using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class MagicSpringOre_Tile : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[ModContent.TileType<StoneScaleWood>()][Type] = true;
		TileID.Sets.ChecksForMerge[Type] = true;
		TileID.Sets.DrawsWalls[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		DustType = ModContent.DustType<MagicSpringOreDust>();
		MinPick = 0;
		HitSound = SoundID.Shimmer1;
		AddMapEntry(new Color(0, 149, 214));
	}

	public override bool CanExplode(int i, int j)
	{
		return true;
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		// Keep the host's transition frames while vanilla handles slopes, half blocks and variants.
		WorldGen.TileMergeAttempt(-2, ModContent.TileType<StoneScaleWood>(), ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);
	}
}
