using Everglow.Commons.VFX.Scene;
using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Traps;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class GreenRelicBrick_Trap : ModTile, ISceneTile
{
	public void AddScene(int i, int j)
	{
		Tile tile = Main.tile[i, j];
		int colorStyle = tile.TileFrameX / 18;
		int greenBrick = ModContent.TileType<GreenRelicBrick>();
		for (int k = 0; k < 4; k++)
		{
			if (k == 0)
			{
				var tileUp = Main.tile[i, j - 10];
				if (tileUp.TileType != greenBrick || !tileUp.HasTile)
				{
					continue;
				}
			}
			if (k == 1)
			{
				var tileUp = Main.tile[i + 10, j];
				if (tileUp.TileType != greenBrick || !tileUp.HasTile)
				{
					continue;
				}
			}
			if (k == 2)
			{
				var tileUp = Main.tile[i, j + 10];
				if (tileUp.TileType != greenBrick || !tileUp.HasTile)
				{
					continue;
				}
			}
			if (k == 3)
			{
				var tileUp = Main.tile[i - 10, j];
				if (tileUp.TileType != greenBrick || !tileUp.HasTile)
				{
					break;
				}
			}
			ColorLasersTrap trap = new ColorLasersTrap { Position = new Point(i, j).ToWorldCoordinates(), Active = true, Visible = true, OriginTilePos = new Point(i, j), OriginTileType = Type, StartRotation = k * MathHelper.PiOver2, Style = Main.rand.Next(2) };
			Ins.VFXManager.Add(trap);
		}
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Color lightColor = Lighting.GetColor(i, j);
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}

		spriteBatch.Draw(ModAsset.GreenRelicBrick_Trap.Value, new Vector2(i, j) * 16 - Main.screenPosition + zero, new Rectangle(0, 0, 16, 16), lightColor, 0, Vector2.zeroVector, 1, SpriteEffects.None, 0);
		return false;
	}

	public override void PostSetDefaults()
	{
		Main.tileSolid[Type] = true;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_dust>();
		HitSound = SoundID.Dig;
		MinPick = 400;
		AddMapEntry(new Color(35, 58, 58));
	}
}
