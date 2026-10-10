using Everglow.Commons.TileHelper;

namespace Everglow.Myth.TheFirefly.Tiles;

public class DarkCocoon_petal : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoonMoss>()] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoon>()] = true;
		Main.tileMerge[ModContent.TileType<DarkCocoon>()][Type] = true;
		Main.tileMerge[Type][ModContent.TileType<DarkCocoonSpecial>()] = true;
		MinPick = 175;
		DustType = DustID.SpookyWood;
		AddMapEntry(new Color(0, 60, 103));
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		var thisTile = Main.tile[i, j];
		if (thisTile.Slope != SlopeType.Solid)
		{
			return;
		}
		bool canPlaceShrub = true;
		for (int x = 0; x < 1; x++)
		{
			for (int y = -3; y < 0; y++)
			{
				if (Main.tile[i + x, j + y].HasTile)
				{
					canPlaceShrub = false;
				}
			}
		}

		// 萤火蓝龙胆
		if (canPlaceShrub)
		{
			switch (Main.rand.Next(2))
			{
				case 0:
					TileUtils.PlaceFrameImportantTiles(i, j - 3, 1, 3, ModContent.TileType<BluishGiantGentian>(), 120 * Main.rand.Next(12));
					break;
				case 1:
					TileUtils.PlaceFrameImportantTiles(i, j - 2, 1, 2, (ushort)ModContent.TileType<BluishGiantGentian_small>(), 48 * Main.rand.Next(6));
					break;
			}
			return;
		}
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<Items.DarkCocoon>());
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}
}
