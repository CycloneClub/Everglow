using Terraria.Audio;
using Terraria.ObjectData;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.CyanVine;

public class CyanVineOreLargeUp : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;

		MineResist = 24f;
		HitSound = SoundID.NPCHit4;
		MinPick = 40;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style1x1);
		TileObjectData.addTile(Type);
		TileID.Sets.Ore[Type] = true;
		AddMapEntry(new Color(84, 130, 154));
		DustType = ModContent.DustType<Dusts.CyanVine>();
		AdjTiles = new int[] { Type };
	}
	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		if (fail)
			return;
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
					if (tile.TileType == ModContent.TileType<CyanVineOreTile>() && tile.HasTile)
						tile.HasTile = false;
				}
			}
		}
		SoundEngine.PlaySound(SoundID.NPCHit4, new Vector2(i * 16, j * 16));
		int Times = Main.rand.Next(14, 21);
		for (int d = 0; d < Times; d++)
		{
			Item.NewItem(null, i * 16 + Main.rand.Next(90) - 45, j * 16 + Main.rand.Next(64), 16, 16, ModContent.ItemType<CyanVineOre>());
		}
		for (int f = 0; f < 11; f++)
		{
			Vector2 vF = new Vector2(0, Main.rand.NextFloat(0, 3f)).RotatedByRandom(6.28d);
			Gore.NewGore(null, new Vector2(i * 16 + Main.rand.Next(90) - 16, j * 16 + Main.rand.Next(64)) + vF, vF, ModContent.Find<ModGore>("Everglow/CyanVineOre" + Main.rand.Next(13).ToString()).Type, 1f);
			vF = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(6.28d);
			Dust.NewDust(new Vector2(i * 16 + Main.rand.Next(90) - 16, j * 16 + Main.rand.Next(64)) + vF, 0, 0, ModContent.DustType<Dusts.CyanVine>(), vF.X, vF.Y);
		}
	}
	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
			zero = Vector2.Zero;
		if (tile.TileFrameX % 90 == 36 && tile.TileFrameY == 0)
		{
			for (int x = -2; x < 3; x++)
			{
				for (int y = 0; y < 4; y++)
				{
					Color cTile = Lighting.GetColor(i + x, j + y);
					Texture2D tex = ModAsset.CyanVineOreLargeUp.Value;
					if (y == 0)
					{
						spriteBatch.Draw(tex, new Vector2((i + x) * 16, (j + y) * 16 - 2) - Main.screenPosition + zero, new Rectangle(x * 18 + tile.TileFrameX, y * 18 + tile.TileFrameY + 2, 16, 18), cTile, 0, new Vector2(0), 1, SpriteEffects.None, 0);
						continue;
					}
					spriteBatch.Draw(tex, new Vector2((i + x) * 16, (j + y) * 16 - 4) - Main.screenPosition + zero, new Rectangle(x * 18 + tile.TileFrameX, y * 18 + tile.TileFrameY, 16, 18), cTile, 0, new Vector2(0), 1, SpriteEffects.None, 0);
				}
			}
		}
		else
		{
			return false;
		}

		return false;
	}
}
