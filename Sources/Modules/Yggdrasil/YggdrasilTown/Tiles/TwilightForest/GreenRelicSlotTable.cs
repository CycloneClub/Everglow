using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria.Audio;
using Terraria.ObjectData;
using static Everglow.Commons.Utilities.TileUtils;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class GreenRelicSlotTable : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;

		Main.tileWaterDeath[Type] = false;
		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3);
		TileObjectData.newTile.Height = 3;
		TileObjectData.newTile.Width = 3;
		TileObjectData.newTile.CoordinateHeights = new int[]
		{
			16,
			16,
			18,
		};
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.addTile(Type);
		AddMapEntry(new Color(70, 99, 99));
		DustType = ModContent.DustType<GreenRelicBrick_dust>();
		MinPick = 9999;
	}

	public override bool RightClick(int i, int j)
	{
		if (Main.netMode == NetmodeID.Server)
		{
			return false;
		}
		Player player = Main.LocalPlayer;
		foreach (var item in player.inventory)
		{
			if (item.type == ModContent.ItemType<CelticKeyStone>())
			{
				item.stack--;
				if (item.stack <= 0)
				{
					item.active = false;
					return false;
				}
				Tile tile = TileUtils.SafeGetTile(i, j);
				Point topLeftPoint = new Point(i - tile.TileFrameX / 18, j - tile.TileFrameY / 18);
				YggdrasilWorldGeneration.KillRectangleAreaOfTile(topLeftPoint.X - 1, topLeftPoint.Y, topLeftPoint.X + 3, topLeftPoint.Y + 42);
				SmoothTile_XXYY(topLeftPoint.X - 2, topLeftPoint.Y, topLeftPoint.X + 4, topLeftPoint.Y + 42);
				for (int x = -2; x < 3; x++)
				{
					for (int y = 0; y < 40; y++)
					{
						if (Main.rand.NextBool(4))
						{
							Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
							int type = ModContent.Find<ModGore>("Everglow/CyanBrickGore" + Main.rand.Next(7)).Type;
							Gore.NewGore(WorldGen.GetProjectileSource_TileBreak(i, j), (topLeftPoint + new Point(x, y)).ToWorldCoordinates(), v0, type, Main.rand.NextFloat(0.75f, 1.25f));
						}
					}
				}
				ShakerManager.AddShaker(topLeftPoint.ToWorldCoordinates() + new Vector2(8), Vector2.One.RotatedByRandom(MathHelper.Pi), 120, 20f, 120, 0.9f, 0.8f, 300);
				SoundEngine.PlaySound(SoundID.DD2_OgreGroundPound, topLeftPoint.ToWorldCoordinates() + new Vector2(8));
			}
		}
		return false;
	}

	public override void MouseOver(int i, int j)
	{
		if (Main.netMode == NetmodeID.Server)
		{
			return;
		}
		Player player = Main.LocalPlayer;
		player.cursorItemIconEnabled = true;
		player.cursorItemIconID = ModContent.ItemType<CelticKeyStone>();
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		return base.PreDraw(i, j, spriteBatch);
	}
}
