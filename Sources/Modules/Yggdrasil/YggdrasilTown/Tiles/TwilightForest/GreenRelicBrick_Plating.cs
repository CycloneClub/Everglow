using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest.Traps;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Tiles.TwilightForest;

public class GreenRelicBrick_Plating : ModTile
{
	public override bool TileFrame(int i, int j, ref bool resetFrame, ref bool noBreak)
	{
		resetFrame = false;
		noBreak = true;
		return base.TileFrame(i, j, ref resetFrame, ref noBreak);
	}

	public Vector3 GetColor(int style)
	{
		Vector3 colorV3 = new Vector3(0);
		switch (style)
		{
			case 0:
				return new Vector3(1f, 0, 0);
			case 1:
				return new Vector3(0.01f, 0.63f, 0.3f);
			case 2:
				return new Vector3(0f, 0.3f, 1);
			case 3:
				return new Vector3(0.85f, 0.57f, 0);
			case 4:
				return new Vector3(0.25f, 0f, 0.95f);
			case 5:
				return new Vector3(1f, 0.3f, 0f);
			case 6:
				return new Vector3(0.3f, 1f, 1f);
			case 7:
				return new Vector3(0.6f, 1f, 0.1f);
		}
		return colorV3;
	}

	public override bool PreDraw(int i, int j, SpriteBatch spriteBatch)
	{
		Tile tile = Main.tile[i, j];
		int colorStyle = tile.TileFrameX / 18;
		Vector3 laserColor = GetColor(colorStyle);
		Color drawColor = new Color(laserColor.X, laserColor.Y, laserColor.Z, 0);
		var zero = new Vector2(Main.offScreenRange, Main.offScreenRange);

		if (Main.drawToScreen)
		{
			zero = Vector2.Zero;
		}
		Texture2D diamond = Commons.ModAsset.TileBlock.Value;
		spriteBatch.Draw(diamond, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, null, drawColor, MathHelper.PiOver4, diamond.Size() * 0.5f, 1, SpriteEffects.None, 0);

		float timer = (float)(Main.time * 0.03 + (i + j) * 0.27f) % 1f;
		spriteBatch.Draw(diamond, new Point(i, j).ToWorldCoordinates() - Main.screenPosition + zero, null, drawColor * (1 - timer), MathHelper.PiOver4, diamond.Size() * 0.5f, 1 + timer, SpriteEffects.None, 0);
		return false;
	}

	public override void PostSetDefaults()
	{
		Main.tileFrameImportant[Type] = true;
		Main.tileSolid[Type] = false;
		Main.tileBlockLight[Type] = true;
		DustType = ModContent.DustType<GreenRelicBrick_Dust>();
		HitSound = SoundID.Dig;
		MinPick = 400;
		AddMapEntry(new Color(13, 13, 13));
	}

	public override void NearbyEffects(int i, int j, bool closer)
	{
		if (closer)
		{
			Tile tile = Main.tile[i, j];
			int colorStyle = tile.TileFrameX / 18;
			Vector2 tilePos = new Point(i, j).ToWorldCoordinates();
			foreach (var player in Main.player)
			{
				if ((player.Center - tilePos).Length() < 24)
				{
					if (player.GetModPlayer<ColorLaserPlayer>().ImmuneStyle != colorStyle)
					{
						player.GetModPlayer<ColorLaserPlayer>().ImmuneStyle = colorStyle;
						player.GetModPlayer<ColorLaserPlayer>().ImmuneTimer = 6000;
						for (int k = 0; k < 12; k++)
						{
							Dust dust = Dust.NewDustDirect(tilePos, 0, 0, ModContent.DustType<LaserPlatingDust>());
							dust.velocity = new Vector2(0, Main.rand.NextFloat(3f, 3.5f)).RotatedBy(k / 12f * MathHelper.TwoPi);
							Vector3 c = GetColor(colorStyle);
							dust.color = new Color(c.X, c.Y, c.Z);
							dust.noGravity = true;
							dust.scale = 2;
						}
						SoundEngine.PlaySound(SoundID.DD2_WitherBeastDeath, tilePos);
					}
					else
					{
						player.GetModPlayer<ColorLaserPlayer>().ImmuneTimer = 6000;
					}
				}
			}
		}
	}
}
