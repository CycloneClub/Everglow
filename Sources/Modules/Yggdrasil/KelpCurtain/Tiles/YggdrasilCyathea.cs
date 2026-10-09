using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Gores;
using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles;

public class YggdrasilCyathea : ModTile
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

		AddMapEntry(new Color(71, 86, 59));
		DustType = ModContent.DustType<YggdrasilCyatheaTrunkDust>();
		AdjTiles = new int[] { Type };
	}
	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield return new Item(ModContent.ItemType<YggdrasilCyatheaWood>());
	}
	public override bool CanDrop(int i, int j)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY == 2)
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
			for (int x = 0; x < 64; x++)
			{
				Dust.NewDust(new Vector2(i * 16 - 40, j * 16 - 100), 96, 96, DustType, 0, 0, 0, default, Main.rand.NextFloat(0.5f, 1f));
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

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		int deltaY = -1; //向上破坏的自变化Y坐标
		if (!fail)
		{
			deltaY = -1; //向上破坏的自变化Y坐标
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
		Texture2D treeTexture = ModAsset.YggdrasilCyathea.Value;
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
			zero = Vector2.Zero;
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

			case 0:  //树桩
				Width = 48;
				Height = 22;
				TexCoordY = 108;
				break;

			case 1:  //树干
				Width = 16;
				TexCoordY = 90;
				break;

			case 2:  //树冠
				Width = 90;
				Height = 90;
				TexCoordY = 0;
				float Wind = Main.windSpeedCurrent / 15f;
				Rot = Wind + (float)Math.Sin(j + Main.timeForVisualEffects / 30f) * Wind * 0.3f;
				OffsetY = 22;
				break;
		}
		var origin = new Vector2(Width / 2f, Height);
		spriteBatch.Draw(treeTexture, new Vector2(i * 16 + OffsetX + 8, j * 16 + OffsetY) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX * Width, TexCoordY, Width, Height), color, Rot, origin, 1, SpriteEffects.None, 0);

		return false;
	}
}
