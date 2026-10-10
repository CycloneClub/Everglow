using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.DataStructures;
using Terraria.GameContent.ObjectInteractions;
using Terraria.ObjectData;

namespace Everglow.Myth.TheFirefly.Tiles.Furnitures;

public class GlowWoodClock : ModTile
{
	public override void SetStaticDefaults()
	{
		// Properties
		Main.tileFrameImportant[Type] = true;
		Main.tileNoAttach[Type] = true;
		Main.tileLavaDeath[Type] = true;
		TileID.Sets.Clock[Type] = true;
		TileID.Sets.HasOutlines[Type] = true;
		TileID.Sets.DisableSmartCursor[Type] = true;

		DustType = ModContent.DustType<BlueGlow>();
		AdjTiles = new int[] { TileID.GrandfatherClocks };

		// Placement
		TileObjectData.newTile.CopyFrom(TileObjectData.Style2xX);
		TileObjectData.newTile.Height = 5;
		TileObjectData.newTile.CoordinateHeights = new[] { 16, 16, 16, 16, 18 };
		TileObjectData.addTile(Type);
	}

	public override void NumDust(int i, int j, bool fail, ref int num)
	{
		num = 0;
	}

	public override bool HasSmartInteract(int i, int j, SmartInteractScanSettings settings)
	{
		return true;
	}

	public override bool RightClick(int x, int y)
	{
		return FurnitureUtils.ClockRightClick();
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var tile = Main.tile[i, j];
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		Texture2D tex = ModAsset.GlowWoodClockGlow.Value;
		spriteBatch.Draw(tex, new Vector2(i * 16, j * 16) - Main.screenPosition + zero, new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16), new Color(0.8f, 0.8f, 0.8f, 0), 0, new Vector2(0), 1, SpriteEffects.None, 0);
		if (tile.TileFrameX == 18 && tile.TileFrameY == 72)
		{
			Vector2 offset = new Vector2(3.5f, -47);
			Texture2D hourHand = ModAsset.GlowWoodClock_hourNeedle.Value;
			Texture2D minuteHand = ModAsset.GlowWoodClock_minuteNeedle.Value;
			spriteBatch.Draw(hourHand, new Vector2(i * 16, j * 16) - Main.screenPosition + zero + offset, null, Color.White, MythUtils.GetHourHandRotation(), new Vector2(0.5f, 6f), 2, SpriteEffects.None, 0);
			spriteBatch.Draw(minuteHand, new Vector2(i * 16, j * 16) - Main.screenPosition + zero + offset, null, Color.White, MythUtils.GetMinuteHandRotation(), new Vector2(0.5f, 6f), 1.8f, SpriteEffects.None, 0);
		}
		base.PostDraw(i, j, spriteBatch);
	}
}
