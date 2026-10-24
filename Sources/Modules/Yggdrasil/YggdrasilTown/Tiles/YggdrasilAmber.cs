using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilAmber : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<StoneScaleWood>()] = true;
		Main.tileMerge[ModContent.TileType<StoneScaleWood>()][Type] = true;
		TileID.Sets.ChecksForMerge[ModContent.TileType<StoneScaleWood>()] = true;
		TileID.Sets.DrawsWalls[Type] = true;
		Main.tileMergeDirt[Type] = false;
		Main.tileBlendAll[Type] = false;
		Main.tileBlockLight[Type] = false;
		Main.tileShine2[Type] = false;
		DustType = ModContent.DustType<YggdrasilAmberDust>();
		MinPick = 0;
		RegisterItemDrop(ModContent.ItemType<Items.Materials.YggdrasilAmber>());
		HitSound = SoundID.Shimmer1;
		AddMapEntry(new Color(198, 124, 19, 163));
	}

	public override bool CanExplode(int i, int j)
	{
		return true;
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		Tile tile = Main.tile[i, j];
		Tile tile2 = Main.tile[i, j - 1];
		Tile tile3 = Main.tile[i, j + 1];
		Tile tile4 = Main.tile[i - 1, j];
		Tile tile5 = Main.tile[i + 1, j];
		Tile tile6 = Main.tile[i - 1, j + 1];
		Tile tile7 = Main.tile[i + 1, j + 1];
		Tile tile8 = Main.tile[i - 1, j - 1];
		Tile tile9 = Main.tile[i + 1, j - 1];
		int upLeft = -1;
		int up = -1;
		int upRight = -1;
		int left = -1;
		int right = -1;
		int downLeft = -1;
		int down = -1;
		int downRight = -1;
		if (tile4 != null && tile4.HasTile)
		{
			left = Main.tileStone[tile4.type] ? 1 : tile4.type;
			if (tile4.slope() == 1 || tile4.slope() == 3)
			{
				left = -1;
			}
		}

		if (tile5 != null && tile5.HasTile)
		{
			right = Main.tileStone[tile5.type] ? 1 : tile5.type;
			if (tile5.slope() == 2 || tile5.slope() == 4)
			{
				right = -1;
			}
		}

		if (tile2 != null && tile2.HasTile)
		{
			up = Main.tileStone[tile2.type] ? 1 : tile2.type;
			if (tile2.slope() == 3 || tile2.slope() == 4)
			{
				up = -1;
			}
		}

		if (tile3 != null && tile3.HasTile)
		{
			down = Main.tileStone[tile3.type] ? 1 : tile3.type;
			if (tile3.slope() == 1 || tile3.slope() == 2)
			{
				down = -1;
			}
		}

		if (tile8 != null && tile8.HasTile)
		{
			upLeft = Main.tileStone[tile8.type] ? 1 : tile8.type;
		}

		if (tile9 != null && tile9.HasTile)
		{
			upRight = Main.tileStone[tile9.type] ? 1 : tile9.type;
		}

		if (tile6 != null && tile6.HasTile)
		{
			downLeft = Main.tileStone[tile6.type] ? 1 : tile6.type;
		}

		if (tile7 != null && tile7.HasTile)
		{
			downRight = Main.tileStone[tile7.type] ? 1 : tile7.type;
		}

		if (tile.slope() == 2)
		{
			up = -1;
			left = -1;
		}

		if (tile.slope() == 1)
		{
			up = -1;
			right = -1;
		}

		if (tile.slope() == 4)
		{
			down = -1;
			left = -1;
		}

		if (tile.slope() == 3)
		{
			down = -1;
			right = -1;
		}
		int mergeType = ModContent.TileType<StoneScaleWood>();

		int randomCase;
		if (resetFrame)
		{
			randomCase = WorldGen.genRand.Next(0, 3);
			tile.frameNumber((byte)randomCase);
		}
		else
		{
			randomCase = tile.frameNumber();
		}
		if (IsNotMergeTargetAndSelf(up, mergeType) && down == mergeType && left == Type && right == Type)
		{
			tile.TileFrameX = (short)(234 + randomCase * 18);
			tile.TileFrameY = 0;
		}
		else if (up == mergeType && IsNotMergeTargetAndSelf(down, mergeType) && left == Type && right == Type)
		{
			tile.TileFrameX = (short)(234 + randomCase * 18);
			tile.TileFrameY = 18;
		}
		else if (up == Type && down == Type && IsNotMergeTargetAndSelf(left, mergeType) && right == mergeType)
		{
			tile.TileFrameX = (short)(234 + randomCase * 18);
			tile.TileFrameY = 36;
		}
		else if (up == Type && down == Type && left == mergeType && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = (short)(234 + randomCase * 18);
			tile.TileFrameY = 54;
		}
		else if (up == Type && down == mergeType && left == Type && right == mergeType)
		{
			tile.TileFrameX = 54;
			tile.TileFrameY = (short)(108 + 36 * randomCase);
		}
		else if (up == mergeType && down == Type && left == Type && right == mergeType)
		{
			tile.TileFrameX = 54;
			tile.TileFrameY = (short)(90 + 36 * randomCase);
		}
		else if (up == Type && down == mergeType && left == mergeType && right == Type)
		{
			tile.TileFrameX = 36;
			tile.TileFrameY = (short)(108 + 36 * randomCase);
		}
		else if (up == mergeType && down == Type && left == mergeType && right == Type)
		{
			tile.TileFrameX = 36;
			tile.TileFrameY = (short)(90 + 36 * randomCase);
		}
		else if (up == Type && down == Type && left == Type && right == Type && downRight == mergeType)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY = (short)(90 + 36 * randomCase);
		}
		else if (up == Type && down == Type && left == Type && right == Type && downLeft == mergeType)
		{
			tile.TileFrameX = 18;
			tile.TileFrameY = (short)(90 + 36 * randomCase);
		}
		else if (up == Type && down == Type && left == Type && right == Type && upRight == mergeType)
		{
			tile.TileFrameX = 0;
			tile.TileFrameY = (short)(108 + 36 * randomCase);
		}
		else if (up == Type && down == Type && left == Type && right == Type && upLeft == mergeType)
		{
			tile.TileFrameX = 18;
			tile.TileFrameY = (short)(108 + 36 * randomCase);
		}
		else if (up == Type && down == Type && left == Type && right == mergeType)
		{
			tile.TileFrameX = 144;
			tile.TileFrameY = (short)(126 + 18 * randomCase);
		}
		else if (up == Type && down == Type && left == mergeType && right == Type)
		{
			tile.TileFrameX = 162;
			tile.TileFrameY = (short)(126 + 18 * randomCase);
		}
		else if (up == mergeType && down == Type && left == Type && right == Type)
		{
			tile.TileFrameX = (short)(144 + 18 * randomCase);
			tile.TileFrameY = 108;
		}
		else if (up == Type && down == mergeType && left == Type && right == Type)
		{
			tile.TileFrameX = (short)(144 + 18 * randomCase);
			tile.TileFrameY = 90;
		}
		else if (up == Type && down == mergeType && IsNotMergeTargetAndSelf(left, mergeType) && (right == Type || right == mergeType))
		{
			tile.TileFrameX = 72;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (up == Type && down == mergeType && (left == Type || left == mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 90;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (up == mergeType && down == Type && IsNotMergeTargetAndSelf(left, mergeType) && (right == Type || right == mergeType))
		{
			tile.TileFrameX = 72;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (up == mergeType && down == Type && (left == Type || left == mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 90;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (up == mergeType && IsNotMergeTargetAndSelf(down, mergeType) && IsNotMergeTargetAndSelf(left, mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 108;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && down == mergeType && IsNotMergeTargetAndSelf(left, mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 108;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == mergeType && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = (short)(18 * randomCase);
			tile.TileFrameY = 234;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && IsNotMergeTargetAndSelf(left, mergeType) && right == mergeType)
		{
			tile.TileFrameX = (short)(54 + 18 * randomCase);
			tile.TileFrameY = 234;
		}
		else if (up == Type && down == mergeType && IsNotMergeTargetAndSelf(left, mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 126;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (up == mergeType && down == Type && IsNotMergeTargetAndSelf(left, mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 126;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == mergeType && right == Type)
		{
			tile.TileFrameX = (short)(18 * randomCase);
			tile.TileFrameY = 252;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == Type && right == mergeType)
		{
			tile.TileFrameX = (short)(54 + 18 * randomCase);
			tile.TileFrameY = 252;
		}
		else if (up == mergeType && down == mergeType && left == mergeType && right == Type)
		{
			tile.TileFrameX = 216;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (up == mergeType && down == mergeType && left == Type && right == mergeType)
		{
			tile.TileFrameX = 216;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (up == Type && down == mergeType && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = 198;
			tile.TileFrameY = (short)(90 + 18 * randomCase);
		}
		else if (up == mergeType && down == Type && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = 198;
			tile.TileFrameY = (short)(144 + 18 * randomCase);
		}
		else if (up == Type && down == Type && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = 180;
			tile.TileFrameY = (short)(126 + 18 * randomCase);
		}
		else if (up == mergeType && down == mergeType && left == Type && right == Type)
		{
			tile.TileFrameX = (short)(144 + 18 * randomCase);
			tile.TileFrameY = 180;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && (down == Type || down == mergeType) && left == mergeType && right == Type)
		{
			tile.TileFrameX = (short)(18 * randomCase);
			tile.TileFrameY = 198;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && (down == Type || down == mergeType) && left == Type && right == mergeType)
		{
			tile.TileFrameX = (short)(54 + 18 * randomCase);
			tile.TileFrameY = 198;
		}
		else if ((up == Type || up == mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == mergeType && right == Type)
		{
			tile.TileFrameX = (short)(18 * randomCase);
			tile.TileFrameY = 216;
		}
		else if ((up == Type || up == mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == Type && right == mergeType)
		{
			tile.TileFrameX = (short)(54 + 18 * randomCase);
			tile.TileFrameY = 216;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && IsNotMergeTargetAndSelf(down, mergeType) && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = (short)(162 + 18 * randomCase);
			tile.TileFrameY = 198;
		}
		else if (up == mergeType && down == mergeType && IsNotMergeTargetAndSelf(left, mergeType) && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 108;
			tile.TileFrameY = (short)(216 + 18 * randomCase);
		}
		else if (up == mergeType && down == mergeType && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = (short)(108 + 18 * randomCase);
			tile.TileFrameY = 198;
		}
		else if (up == mergeType && down == mergeType && IsNotMergeTargetAndSelf(left, mergeType) && right == mergeType)
		{
			tile.TileFrameX = 72;
			tile.TileFrameY = (short)(90 + 18 * (randomCase + WorldGen.genRand.Next(3)));
		}
		else if (up == mergeType && down == mergeType && left == mergeType && IsNotMergeTargetAndSelf(right, mergeType))
		{
			tile.TileFrameX = 90;
			tile.TileFrameY = (short)(90 + 18 * (randomCase + WorldGen.genRand.Next(3)));
		}
		else if (up == mergeType && IsNotMergeTargetAndSelf(down, mergeType) && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = (short)(0 + 18 * (randomCase + WorldGen.genRand.Next(3)));
			tile.TileFrameY = 216;
		}
		else if (IsNotMergeTargetAndSelf(up, mergeType) && down == mergeType && left == mergeType && right == mergeType)
		{
			tile.TileFrameX = (short)(0 + 18 * (randomCase + WorldGen.genRand.Next(3)));
			tile.TileFrameY = 198;
		}
		else
		{
			GetFrameNormally(i, j, randomCase, up, down, left, right, upLeft, upRight, downLeft, downRight);
		}
		return false;
	}

	private void GetFrameNormally(int i, int j, int randomCase, int up, int down, int left, int right, int upLeft, int upRight, int downLeft, int downRight)
	{
		Tile tile = Main.tile[i, j];
		if (up == Type && down == Type && left == Type && right == Type)
		{
			if (upLeft != Type && upRight != Type)
			{
				switch (randomCase)
				{
					case 0:
						tile.TileFrameX = 108;
						tile.TileFrameY = 18;
						break;
					case 1:
						tile.TileFrameX = 126;
						tile.TileFrameY = 18;
						break;
					default:
						tile.TileFrameX = 144;
						tile.TileFrameY = 18;
						break;
				}
			}
			else if (downLeft != Type && downRight != Type)
			{
				switch (randomCase)
				{
					case 0:
						tile.TileFrameX = 108;
						tile.TileFrameY = 36;
						break;
					case 1:
						tile.TileFrameX = 126;
						tile.TileFrameY = 36;
						break;
					default:
						tile.TileFrameX = 144;
						tile.TileFrameY = 36;
						break;
				}
			}
			else if (upLeft != Type && downLeft != Type)
			{
				switch (randomCase)
				{
					case 0:
						tile.TileFrameX = 180;
						tile.TileFrameY = 0;
						break;
					case 1:
						tile.TileFrameX = 180;
						tile.TileFrameY = 18;
						break;
					default:
						tile.TileFrameX = 180;
						tile.TileFrameY = 36;
						break;
				}
			}
			else if (upRight != Type && downRight != Type)
			{
				switch (randomCase)
				{
					case 0:
						tile.TileFrameX = 198;
						tile.TileFrameY = 0;
						break;
					case 1:
						tile.TileFrameX = 198;
						tile.TileFrameY = 18;
						break;
					default:
						tile.TileFrameX = 198;
						tile.TileFrameY = 36;
						break;
				}
			}
			else
			{
				switch (randomCase)
				{
					case 0:
						tile.TileFrameX = 18;
						tile.TileFrameY = 18;
						break;
					case 1:
						tile.TileFrameX = 36;
						tile.TileFrameY = 18;
						break;
					default:
						tile.TileFrameX = 54;
						tile.TileFrameY = 18;
						break;
				}
			}
		}
		else if (up != Type && down == Type && left == Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 18;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 36;
					tile.TileFrameY = 0;
					break;
				default:
					tile.TileFrameX = 54;
					tile.TileFrameY = 0;
					break;
			}
		}
		else if (up == Type && down != Type && left == Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 18;
					tile.TileFrameY = 36;
					break;
				case 1:
					tile.TileFrameX = 36;
					tile.TileFrameY = 36;
					break;
				default:
					tile.TileFrameX = 54;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up == Type && down == Type && left != Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 0;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 0;
					tile.TileFrameY = 18;
					break;
				default:
					tile.TileFrameX = 0;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up == Type && down == Type && left == Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 72;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 72;
					tile.TileFrameY = 18;
					break;
				default:
					tile.TileFrameX = 72;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up != Type && down == Type && left != Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 0;
					tile.TileFrameY = 54;
					break;
				case 1:
					tile.TileFrameX = 36;
					tile.TileFrameY = 54;
					break;
				default:
					tile.TileFrameX = 72;
					tile.TileFrameY = 54;
					break;
			}
		}
		else if (up != Type && down == Type && left == Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 18;
					tile.TileFrameY = 54;
					break;
				case 1:
					tile.TileFrameX = 54;
					tile.TileFrameY = 54;
					break;
				default:
					tile.TileFrameX = 90;
					tile.TileFrameY = 54;
					break;
			}
		}
		else if (up == Type && down != Type && left != Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 0;
					tile.TileFrameY = 72;
					break;
				case 1:
					tile.TileFrameX = 36;
					tile.TileFrameY = 72;
					break;
				default:
					tile.TileFrameX = 72;
					tile.TileFrameY = 72;
					break;
			}
		}
		else if (up == Type && down != Type && left == Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 18;
					tile.TileFrameY = 72;
					break;
				case 1:
					tile.TileFrameX = 54;
					tile.TileFrameY = 72;
					break;
				default:
					tile.TileFrameX = 90;
					tile.TileFrameY = 72;
					break;
			}
		}
		else if (up == Type && down == Type && left != Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 90;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 90;
					tile.TileFrameY = 18;
					break;
				default:
					tile.TileFrameX = 90;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up != Type && down != Type && left == Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 108;
					tile.TileFrameY = 72;
					break;
				case 1:
					tile.TileFrameX = 126;
					tile.TileFrameY = 72;
					break;
				default:
					tile.TileFrameX = 144;
					tile.TileFrameY = 72;
					break;
			}
		}
		else if (up != Type && down == Type && left != Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 108;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 126;
					tile.TileFrameY = 0;
					break;
				default:
					tile.TileFrameX = 144;
					tile.TileFrameY = 0;
					break;
			}
		}
		else if (up == Type && down != Type && left != Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 108;
					tile.TileFrameY = 54;
					break;
				case 1:
					tile.TileFrameX = 126;
					tile.TileFrameY = 54;
					break;
				default:
					tile.TileFrameX = 144;
					tile.TileFrameY = 54;
					break;
			}
		}
		else if (up != Type && down != Type && left != Type && right == Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 162;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 162;
					tile.TileFrameY = 18;
					break;
				default:
					tile.TileFrameX = 162;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up != Type && down != Type && left == Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 216;
					tile.TileFrameY = 0;
					break;
				case 1:
					tile.TileFrameX = 216;
					tile.TileFrameY = 18;
					break;
				default:
					tile.TileFrameX = 216;
					tile.TileFrameY = 36;
					break;
			}
		}
		else if (up != Type && down != Type && left != Type && right != Type)
		{
			switch (randomCase)
			{
				case 0:
					tile.TileFrameX = 162;
					tile.TileFrameY = 54;
					break;
				case 1:
					tile.TileFrameX = 180;
					tile.TileFrameY = 54;
					break;
				default:
					tile.TileFrameX = 198;
					tile.TileFrameY = 54;
					break;
			}
		}
	}

	private bool IsNotMergeTargetAndSelf(int myType, int mergeType)
	{
		return myType != Type && myType != mergeType;
	}
}
