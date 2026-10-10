using Everglow.Yggdrasil.KelpCurtain.Dusts;

namespace Everglow.Yggdrasil.KelpCurtain.Walls;

public class WaterErodedGreenBrickWall_Fixed : ModWall
{
	public override void SetStaticDefaults()
	{
		Main.wallHouse[Type] = true;
		DustType = ModContent.DustType<WaterErodedGreenBrickDust>();
		AddMapEntry(new Color(19, 51, 71));
	}

	public override bool WallFrame(int i, int j, bool randomizeFrame, ref int style, ref int frameNumber)
	{
		return base.WallFrame(i, j, randomizeFrame, ref style, ref frameNumber);
	}

	public override void PostDraw(int i, int j, SpriteBatch spriteBatch)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var offsetScreen = new Vector2(Main.offScreenRange);
		if (Main.drawToScreen)
		{
			offsetScreen = Vector2.Zero;
		}
		Vector2 drawPos = new Point(i, j).ToWorldCoordinates() - Main.screenPosition + offsetScreen;
		var frame = new Rectangle(0 + i % 12 * 16 - 4, 204 + j % 12 * 16 - 4, 28, 28);
		spriteBatch.Draw(texture, drawPos, frame, Lighting.GetColor(i, j), 0, frame.Size() * 0.5f, 1, SpriteEffects.None, 0);
	}

	public override bool CanExplode(int i, int j)
	{
		return false;
	}

	public override void KillWall(int i, int j, ref bool fail)
	{
		fail = true;
		base.KillWall(i, j, ref fail);
	}

	public override bool CanBeTeleportedTo(int i, int j, Player player, string context)
	{
		return false;
	}

	public override void RandomUpdate(int i, int j, bool underground)
	{
		Tile tile = Main.tile[i, j];
		tile.LiquidType = LiquidID.Water;
		tile.LiquidAmount = 255;
		base.RandomUpdate(i, j, underground);
	}
}
