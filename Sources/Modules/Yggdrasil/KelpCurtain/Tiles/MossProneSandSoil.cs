using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class MossProneSandSoil : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		TileID.Sets.ChecksForMerge[Type] = true;
		Main.tileMerge[ModContent.TileType<OldMoss>()][Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DragonScaleWood>()] = true;
		Main.tileMerge[Type][TileID.Stone] = true;
		DustType = ModContent.DustType<MossProneSandSoilDust>();
		MinPick = 50;
		HitSound = SoundID.Dig;
		AddMapEntry(new Color(53, 29, 26));
	}

	public override void ModifyFrameMerge(int i, int j, ref int up, ref int down, ref int left, ref int right, ref int upLeft, ref int upRight, ref int downLeft, ref int downRight)
	{
		int hostType = ModContent.TileType<OldMoss>();
		Tile tile = Main.tile[i, j];
		WorldGen.TileMergeAttempt(-2, hostType, ref up, ref down, ref left, ref right, ref upLeft, ref upRight, ref downLeft, ref downRight);

		// BlendAll runs before this hook and hides the host type. Restore it without undoing shape culling.
		up = RestoreHostTransition(up, i, j - 1);
		down = RestoreHostTransition(down, i, j + 1);
		left = RestoreHostTransition(left, i - 1, j);
		right = RestoreHostTransition(right, i + 1, j);
		upLeft = RestoreHostTransition(upLeft, i - 1, j - 1);
		upRight = RestoreHostTransition(upRight, i + 1, j - 1);
		downLeft = RestoreHostTransition(downLeft, i - 1, j + 1);
		downRight = RestoreHostTransition(downRight, i + 1, j + 1);

		int RestoreHostTransition(int mergeType, int x, int y)
		{
			if (mergeType != Type)
			{
				return mergeType;
			}
			Tile neighbor = Main.tile[x, y];
			if (!neighbor.HasTile || neighbor.TileType != hostType)
			{
				return mergeType;
			}

			// These full-edge connections are forced by vanilla before BlendAll, not by material merging.
			if (x == i && y < j && (tile.BottomSlope || neighbor.TopSlope || neighbor.IsHalfBlock))
			{
				return mergeType;
			}
			if (x == i && y > j && (tile.TopSlope || neighbor.BottomSlope))
			{
				return mergeType;
			}
			if (y == j && tile.IsHalfBlock && neighbor.IsHalfBlock)
			{
				return mergeType;
			}
			return -2;
		}
	}
}
