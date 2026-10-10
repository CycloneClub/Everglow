using Everglow.Yggdrasil.YggdrasilTown.Dusts;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class JellyBeadString : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;

		DustType = ModContent.DustType<JellyBallGel>();
		HitSound = SoundID.NPCHit1;
		Main.tileCut[Type] = true;
		AddMapEntry(new Color(24, 72, 174));
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
	}

	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		int deltaY = 0;
		while (Main.tile[i, j - 1 - deltaY].TileType == Type)
		{
			deltaY++;
			if (deltaY > j - 1)
			{
				break;
			}
		}
		if (deltaY > 15 + Math.Sin(i + j) * 3)
		{
			return;
		}
		if (Main.rand.NextBool(Math.Max(1, deltaY * deltaY - 40)))
		{
			var tileBelow = Main.tile[i, j + 1];
			if (!tileBelow.HasTile)
			{
				tileBelow.TileType = Type;
				tileBelow.HasTile = true;

				// 与原版一致，漆与涂料继承
				tileBelow.CopyPaintAndCoating(Main.tile[i, j]);

				// frame调整与联机同步
				WorldGen.SquareTileFrame(i, j + 1);
				if (Main.netMode is NetmodeID.Server)
				{
					NetMessage.SendTileSquare(-1, i, j + 1);
				}
			}
		}
		base.RandomUpdate(i, j, underground);
	}

	public override void KillTile(int i, int j, ref bool fail, ref bool effectOnly, ref bool noItem)
	{
		base.KillTile(i, j, ref fail, ref effectOnly, ref noItem);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Main.instance.TilesRenderer.CrawlToTopOfVineAndAddSpecialPoint(j, i);
		return false;
	}

	public override void SetDrawPositions(int i, int j, ref int width, ref int offsetY, ref int height, ref short tileFrameX, ref short tileFrameY)
	{
		offsetY = -2;
	}

	public override void SetSpriteEffects(int i, int j, ref SpriteEffects spriteEffects)
	{
		if (i % 2 == 0)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}
	}
}
