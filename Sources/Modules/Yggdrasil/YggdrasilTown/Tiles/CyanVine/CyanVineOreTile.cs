using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;

public class CyanVineOreTile : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;

		MineResist = 24f;
		HitSound = SoundID.NPCHit4;
		MinPick = 40;
		DustType = ModContent.DustType<Dusts.CyanVine>();
		TileID.Sets.Ore[Type] = true;

		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(80, 130, 154), modTranslation);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
		{
			return;
		}

		var ThisTile = Main.tile[i, j];
		int X0 = i - ThisTile.TileFrameX / 18;
		int Y0 = j - ThisTile.TileFrameY / 18;
		for (int x = 0; x < 5; x++)
		{
			for (int y = 0; y < 3; y++)
			{
				var tile = Main.tile[X0 + x, Y0 + y];
				if (tile.TileFrameX == x * 18 && tile.TileFrameY == y * 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreTile>())
					{
						tile.HasTile = false;
					}
				}
				if (tile.TileFrameX % 108 == x * 18 + 18 && tile.TileFrameY == y * 18 + 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreLarge>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
				if (tile.TileFrameX % 108 == x * 18 && tile.TileFrameY == y * 18 + 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreLarge>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
				if (tile.TileFrameX % 72 == x * 18 && tile.TileFrameY == y * 18 + 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreMiddle>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
				if (tile.TileFrameX % 54 == x * 18 && tile.TileFrameY == y * 18 + 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreSmall>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
				if (tile.TileFrameX % 54 == x * 18 && tile.TileFrameY == y * 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreSmallUp>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
				if (tile.TileFrameX % 54 == x * 18 && tile.TileFrameY == y * 18)
				{
					if (tile.TileType == ModContent.TileType<CyanVineOreLargeUp>())
					{
						WorldGen.KillTile(X0 + x, Y0 + y);
					}
				}
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return false;
	}
}
