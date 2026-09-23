using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Terraria.DataStructures;
using Terraria.ObjectData;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles;

public class YggdrasilTownAccessGateNFCTrigger : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileLighted[Type] = true;
		Main.tileLavaDeath[Type] = false;
		Main.tileNoAttach[Type] = false;
		Main.tileWaterDeath[Type] = false;

		TileObjectData.newTile.CopyFrom(TileObjectData.Style3x3Wall);
		TileObjectData.newTile.Height = 1;
		TileObjectData.newTile.Width = 1;

		TileObjectData.newTile.CoordinateHeights = new int[1];
		Array.Fill(TileObjectData.newTile.CoordinateHeights, 16);
		TileObjectData.newTile.StyleHorizontal = true;
		TileObjectData.newTile.LavaDeath = false;
		TileObjectData.newTile.Origin = new Point16(0, 0);

		TileObjectData.addTile(Type);
		AddMapEntry(new Color(127, 127, 127));
	}

	public override void ModifyLight(int i, int j, ref float r, ref float g, ref float b)
	{
		r = g = b = 0;
		var tile = Main.tile[i, j];
		if (tile.TileFrameX > 0)
		{
			g = 0.25f;
		}
		else if (tile.TileFrameY <= 5)
		{
			r = 0.25f;
			g = 0;
		}
		base.ModifyLight(i, j, ref r, ref g, ref b);
	}

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		var tile = Main.tile[i, j];
		if (tile.TileFrameY > 0)
		{
			tile.TileFrameY--;
		}
		else
		{
			tile.TileFrameY = 60;
		}

		bool noBarrier = true;
		for (int x = -5; x <= 5; x++)
		{
			for (int y = -5; y <= 5; y++)
			{
				var tileC = TileUtils.SafeGetTile(i + x, j + y);
				if (tileC.HasTile && tileC.TileType == ModContent.TileType<YggdrasilTownAccessGate_Barrier>())
				{
					noBarrier = false;
					break;
				}
			}
		}
		if (noBarrier)
		{
			tile.TileFrameX = 30;
		}
		if (tile.TileFrameX > 0)
		{
			tile.TileFrameX--;
		}
		base.NearbyEffects(i, j, closer);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		Texture2D tex = ModAsset.YggdrasilTownAccessGateNFCTrigger.Value;
		Vector2 pos = new Point(i, j).ToWorldCoordinates();
		Vector2 zero = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.zeroVector;
		}
		pos += zero;
		Rectangle frame = new Rectangle(0, 0, 16, 16);
		if (tile.TileFrameY <= 5)
		{
			frame = new Rectangle(36, 0, 16, 16);
		}
		if (tile.TileFrameX > 0)
		{
			frame = new Rectangle(18, 0, 16, 16);
		}
		spriteBatch.Draw(tex, pos - Main.screenPosition, frame, Lighting.GetColor(i, j), 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		frame.Y += 18;
		spriteBatch.Draw(tex, pos - Main.screenPosition, frame, Color.White, 0, frame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}

	public override void MouseOver(int i, int j)
	{
		Tile tile = TileUtils.SafeGetTile(i, j);
		tile.TileFrameX = 18;
		string text = "[i:" + ModContent.ItemType<YggdrasilTownAccessCard>() + "]";
		Main.instance.MouseText(text, ItemRarityID.White);
		base.MouseOver(i, j);
	}

	public override bool RightClick(int i, int j)
	{
		bool canActive = false;
		foreach (var item in Main.LocalPlayer.inventory)
		{
			if (item.type == ModContent.ItemType<YggdrasilTownAccessCard>())
			{
				canActive = true;
				break;
			}
		}
		if (canActive)
		{
			for (int x = -5; x <= 5; x++)
			{
				for (int y = -5; y <= 5; y++)
				{
					var tile = TileUtils.SafeGetTile(i + x, j + y);
					if (tile.TileType == ModContent.TileType<YggdrasilTownAccessGate>())
					{
						tile.TileFrameY = 768;
					}
				}
			}
		}
		return base.RightClick(i, j);
	}
}
