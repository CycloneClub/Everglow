using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class OldMoss : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlendAll[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileNoSunLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DragonScaleWood>()] = true;
		Main.tileMerge[Type][ModContent.TileType<MossProneSandSoil>()] = true;
		Main.tileMerge[Type][TileID.Stone] = true;
		Main.tileMerge[TileID.Stone][Type] = true;
		DustType = DustType = ModContent.DustType<OldMossDust>();
		MinPick = 50;
		HitSound = SoundID.Dig;
		AddMapEntry(new Color(68, 91, 27));
	}

	public override void RandomUpdate(int i, int j)
	{
		// if (Main.tile[i, j].Slope == SlopeType.Solid && Main.tile[i + 1, j].TileType == Type && Main.tile[i + 1, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].TileType == Type &&
		// !Main.tile[i, j + 1].HasTile && !Main.tile[i + 1, j + 1].HasTile && !Main.tile[i - 1, j + 1].HasTile)// 巨大帘幕苔
		// {
		// WorldGen.PlaceTile(i, j + 1, ModContent.TileType<KelpMoss_large_tile>());
		// }
		// if (Main.tile[i, j].Slope == SlopeType.Solid && !Main.tile[i, j + 1].HasTile)// 雨帘苔
		// {
		// Tile tile = Main.tile[i, j + 1];
		// tile.TileType = (ushort)ModContent.TileType<KelpMoss>();
		// tile.HasTile = true;
		// }
		if (TileUtils.CanPlaceMultiAtTopTowardsUpRight(i, j, 4, 2) && Main.rand.NextBool(4))
		{
			TileUtils.PlaceFrameImportantTilesAbove(i, j, 4, 2, ModContent.TileType<RottenStump_4x2>(), Main.rand.Next(2) * 72);
			return;
		}
		if (TileUtils.CanPlaceMultiAtTopTowardsUpRight(i, j, 3, 2) && Main.rand.NextBool(3))
		{
			int typePlaced = ModContent.TileType<RottenStump_3x2>();
			if (Main.rand.NextBool())
			{
				typePlaced = ModContent.TileType<SucculentHerb_type0>();
			}
			TileUtils.PlaceFrameImportantTilesAbove(i, j, 3, 2, typePlaced, Main.rand.Next(2) * 54);
			return;
		}
		if (TileUtils.CanPlaceMultiAtTopTowardsUpRight(i, j, 2, 2) && Main.rand.NextBool(4))
		{
			TileUtils.PlaceFrameImportantTilesAbove(i, j, 2, 2, ModContent.TileType<SucculentHerb_type1>(), Main.rand.Next(2) * 36);
			return;
		}
		if (TileUtils.CanPlaceMultiAtTopTowardsUpRight(i, j, 1, 1))
		{
			int typePlaced = ModContent.TileType<KelpCurtainBracken>();
			if (Main.rand.NextBool(3))
			{
				typePlaced = ModContent.TileType<SucculentHerb_bud_type1>();
				if (Main.rand.NextBool(3) && TileUtils.SafeGetTile(i, j - 1).LiquidAmount <= 0)
				{
					typePlaced = ModContent.TileType<KelpCurtainBoletus>();
					TileUtils.PlaceFrameImportantTilesAbove(i, j, 1, 1, typePlaced, Main.rand.Next(4) * 30);
					return;
				}
				else
				{
					TileUtils.PlaceFrameImportantTilesAbove(i, j, 1, 1, typePlaced, Main.rand.Next(2) * 18);
					return;
				}
			}

			TileUtils.PlaceFrameImportantTilesAbove(i, j, 1, 1, typePlaced, Main.rand.Next(2) * 154);
			return;
		}
		if (Main.tile[i, j].Slope == SlopeType.Solid && Main.tile[i + 1, j].Slope == SlopeType.Solid && Main.tile[i - 1, j].Slope == SlopeType.Solid && Main.tile[i + 2, j].Slope == SlopeType.Solid && Main.tile[i - 2, j].Slope == SlopeType.Solid &&
				Main.tile[i, j + 1].Slope == SlopeType.Solid && Main.tile[i + 1, j + 1].Slope == SlopeType.Solid && Main.tile[i - 1, j + 1].Slope == SlopeType.Solid && Main.tile[i + 2, j + 1].Slope == SlopeType.Solid && Main.tile[i - 2, j + 1].Slope == SlopeType.Solid)// 树木
		{
			int MaxHeight = 0;
			for (int x = -2; x < 3; x++)
			{
				for (int y = -1; y > -8; y--)
				{
					if (j + y > 20)
					{
						if (Main.tile[i + x, j + y].HasTile || Main.tile[i + x, j + y].LiquidAmount > 3)
						{
							return;
						}
					}
					MaxHeight = -y;
				}
			}
			if (MaxHeight > 3)
			{
				BuildCyatheaTree(i, j - 1, MaxHeight);
			}
		}
	}

	public static void BuildCyatheaTree(int i, int j, int height = 0)
	{
		if (j < 30)
		{
			return;
		}

		int trueHeight = Main.rand.Next(3, height);

		for (int g = 0; g < trueHeight; g++)
		{
			Tile tile = Main.tile[i, j - g];
			if (g == 0)
			{
				tile.TileType = (ushort)ModContent.TileType<YggdrasilCyathea>();
				tile.TileFrameY = 0;
				tile.TileFrameX = (short)Main.rand.Next(2);
				tile.HasTile = true;
				continue;
			}
			if (g == trueHeight - 1)
			{
				tile.TileType = (ushort)ModContent.TileType<YggdrasilCyathea>();
				tile.TileFrameY = 2;
				tile.TileFrameX = (short)Main.rand.Next(2);
				tile.HasTile = true;
				continue;
			}
			tile.TileType = (ushort)ModContent.TileType<YggdrasilCyathea>();
			tile.TileFrameY = 1;
			tile.TileFrameX = (short)Main.rand.Next(4);
			tile.HasTile = true;
		}
	}
}
