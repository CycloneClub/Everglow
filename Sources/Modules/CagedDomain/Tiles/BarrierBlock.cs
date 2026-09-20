using Everglow.CagedDomain.Items;

namespace Everglow.CagedDomain.Tiles;

public class BarrierBlock : ModTile
{
	public override void SetStaticDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileMergeDirt[Type] = true;
		Main.tileBlockLight[Type] = false;
		AddMapEntry(Color.Transparent);
		MinPick = int.MaxValue;
	}

	public override void PlaceInWorld(int i, int j, Item item)
	{
		var tile = Main.tile[i, j];
		tile.IsTileInvisible = true;
	}

	public override void NumDust(int i, int j, bool fail, ref int num) => num = 0;

	public override bool CanKillTile(int i, int j, ref bool blockDamaged) => false;

	public override bool CanExplode(int i, int j) => false;

	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		if (PlayerHeldBarrierItem(Main.LocalPlayer))
		{
			var tile = Main.tile[i, j];
			Vector2 zero = new Vector2(Main.offScreenRange, Main.offScreenRange);
			if (Main.drawToScreen)
			{
				zero = Vector2.Zero;
			}
			var frame = new Rectangle(tile.TileFrameX, tile.TileFrameY, 16, 16);
			Texture2D tex = Commons.ModAsset.HalfTiles.Value;
			var drawColor = new Color(1f, 0, 0, 0);
			int count = 10;
			if (tile.Slope == SlopeType.SlopeDownLeft)
			{
				frame = new Rectangle(36, 0, 16, 16);
			}
			if (tile.Slope == SlopeType.SlopeDownRight)
			{
				frame = new Rectangle(54, 0, 16, 16);
			}
			if (tile.Slope == SlopeType.SlopeUpLeft)
			{
				frame = new Rectangle(90, 0, 16, 16);
			}
			if (tile.Slope == SlopeType.SlopeUpRight)
			{
				frame = new Rectangle(72, 0, 16, 16);
			}
			if (tile.IsHalfBlock)
			{
				frame = new Rectangle(18, 0, 16, 16);
			}
			if (tile.Slope == SlopeType.Solid && !tile.IsHalfBlock)
			{
				tex = ModAsset.BarrierBlock_Visable.Value;
				drawColor = Color.White;
				count = 1;
			}
			for (int k = 0; k < count; k++)
			{
				spriteBatch.Draw(tex, new Vector2(i, j) * 16 - Main.screenPosition + zero, frame, drawColor, 0, new Vector2(0), 1f, SpriteEffects.None, 0);
			}
		}
		return base.PreDraw(i, j, spriteBatch);
	}

	public static bool PlayerHeldBarrierItem(Player player)
	{
		// return true;
		return player.HeldItem.type == ModContent.ItemType<BarrierBlock_Item>() || player.HeldItem.type == ModContent.ItemType<BarrierPlatform_Item>() || player.HeldItem.hammer > 0 | player.HeldItem.pick > 0;
	}
}
