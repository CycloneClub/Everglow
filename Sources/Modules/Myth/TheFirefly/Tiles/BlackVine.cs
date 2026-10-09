using Everglow.Commons.Physics;
using Terraria.DataStructures;
using Terraria.ObjectData;
using Terraria.Enums;
using Terraria.GameContent.Drawing;
using Everglow.Commons.TileHelper;
using Terraria.ID;

namespace Everglow.Myth.TheFirefly.Tiles;

public class BlackVine : ModTile
{
	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = false;
		TileID.Sets.IsVine[Type] = true;
		TileID.Sets.VineThreads[Type] = true;
		DustType = 191;
		Main.tileCut[Type] = true;
		var modTranslation = CreateMapEntryName();
		AddMapEntry(new Color(11, 11, 11), modTranslation);
		HitSound = SoundID.Grass;
	}
	public override void PlaceInWorld(int i, int j, Item item)
	{
	}
	public override IEnumerable<Item> GetItemDrops(int i, int j)
	{
		yield break;
	}
	public override void RandomUpdate(int i, int j)
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
		base.RandomUpdate(i, j);
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
