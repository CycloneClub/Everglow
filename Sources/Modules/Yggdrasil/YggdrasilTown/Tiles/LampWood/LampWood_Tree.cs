using Everglow.Yggdrasil.KelpCurtain.Gores;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;

public class LampWood_Tree : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		Main.tileLavaDeath[Type] = false;
		Main.tileFrameImportant[Type] = true;
		Main.tileBlockLight[Type] = false;
		Main.tileLighted[Type] = true;
		Main.tileAxe[Type] = true;
		Main.tileNoAttach[Type] = false;

		TileID.Sets.IsATreeTrunk[Type] = true;

		AddMapEntry(new Color(49, 41, 96));
		DustType = ModContent.DustType<LampWood_Dust>();
		AdjTiles = new int[] { Type };
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<LampWood_Wood>());
	}

	public override bool CanDrop(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 2)
		{
			for (int x = 0; x < 64; x++)
			{
				Dust.NewDust(new Vector2(i * 16 - 40, j * 16 - 100), 96, 96, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
			}
			for (int x = 0; x < 32; x++)
			{
				Dust.NewDust(new Vector2(i * 16 - 40, j * 16 - 100), 96, 96, ModContent.DustType<LampWood_Dust_fluorescent>(), 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
			}
		}
		else
		{
			for (int x = 0; x < 9; x++)
			{
				Dust.NewDust(new Vector2(i * 16, j * 16), 16, 16, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
			}
		}
		return false;
	}

	private void Shake(int i, int j, int frameY)
	{
		if (frameY == 2)
		{
			for (int x = 0; x < 18; x++)
			{
				Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.NextFloat(16f), 0).RotatedByRandom(6.283), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283), ModContent.GoreType<CyatheaLeaf_tiny>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
			for (int x = 0; x < 6; x++)
			{
				Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.NextFloat(16f), 0).RotatedByRandom(6.283), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283), ModContent.GoreType<CyatheaLeaf_small>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
			for (int x = 0; x < 3; x++)
			{
				Gore.NewGore(null, new Vector2(i * 16, j * 16) + new Vector2(Main.rand.NextFloat(16f), 0).RotatedByRandom(6.283), new Vector2(0, Main.rand.NextFloat(1f)).RotatedByRandom(6.283), ModContent.GoreType<CyatheaLeaf_large>(), Main.rand.NextFloat(0.65f, 1.45f));
			}
		}
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		Tile tile = Main.tile[i, j];
		if (tile.TileFrameY == 2)
		{
			if (Main.rand.NextBool(7))
			{
				float wind = Main.windSpeedCurrent / 15f;
				float rot = wind + (float)Math.Sin(j + Main.timeForVisualEffects / 30f) * wind * 0.3f;
				Vector2 v0 = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 40f).RotatedByRandom(6.283);
				Dust dust = Dust.NewDustDirect(new Vector2(i * 16, j * 16) + v0 + new Vector2(0, -180).RotatedBy(rot), 16, 16, ModContent.DustType<LampWood_Dust_fluorescent_appear>());
				dust.alpha = 0;
				dust.rotation = Main.rand.NextFloat(0.7f, 1.4f);
				dust.velocity = v0 * 0.03f + new Vector2(wind * 4, 0);
			}
		}
		base.NearbyEffects(i, j, closer);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		int deltaY = -1; // 向上破坏的自变化Y坐标
		if (!fail)
		{
			deltaY = -1; // 向上破坏的自变化Y坐标
			while (Main.tile[i, j + deltaY].TileType == Type && deltaY > -100)
			{
				Tile baseTile = Main.tile[i, j + deltaY];
				baseTile.HasTile = false;
				deltaY -= 1;
			}
		}
		else
		{
			while (Main.tile[i, j + deltaY].HasTile && Main.tile[i, j + deltaY].TileType == Type && deltaY > -100)
			{
				Shake(i, j + deltaY, Main.tile[i, j + deltaY].TileFrameY);
				CanDrop(i, j + deltaY);
				deltaY -= 1;
			}
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Texture2D treeTexture = ModAsset.LampWood_Tree.Value;
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Tile tile = Main.tile[i, j];
		int Width;
		int Height = 16;
		int TexCoordY;
		int OffsetY = 20;
		int OffsetX = 0;
		float Rot = 0;
		Color color = Lighting.GetColor(i, j);
		switch (tile.TileFrameY)
		{
			default:
				return false;

			case 0:  // 树桩
				Width = 38;
				Height = 22;
				TexCoordY = 304;
				break;

			case 1:  // 树干
				Width = 24;
				TexCoordY = 236;
				break;

			case 2:  // 树冠
				Width = 200;
				Height = 234;
				TexCoordY = 0;
				float Wind = Main.windSpeedCurrent / 15f;
				Rot = Wind + (float)Math.Sin(j + Main.timeForVisualEffects / 30f) * Wind * 0.3f;
				OffsetY = 22;
				break;
			case 3:  // 树干长串
				Width = 38;
				Height = 48;
				TexCoordY = 254;
				break;
		}
		var origin = new Vector2(Width / 2f, Height);
		spriteBatch.Draw(treeTexture, new Vector2(i * 16 + OffsetX + 8, j * 16 + OffsetY) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX * Width, TexCoordY, Width, Height), color, Rot, origin, 1, j % 2 == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		spriteBatch.Draw(treeTexture, new Vector2(i * 16 + OffsetX + 8, j * 16 + OffsetY) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX * Width, TexCoordY + 400, Width, Height), new Color(1f, 1f, 1f, 0), Rot, origin, 1, j % 2 == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		if (TexCoordY == 0)
		{
			Lighting.AddLight(i, j - 2, 0.5f, 0.3f, 0);
		}
		return false;
	}
}
