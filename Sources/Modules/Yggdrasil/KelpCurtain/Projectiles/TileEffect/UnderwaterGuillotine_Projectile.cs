using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.TileEffect;

public class UnderwaterGuillotine_Projectile : ModProjectile
{
	public Point TileTopLeft;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.width = 128;
		Projectile.height = 92;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 100000;
		Projectile.friendly = true;
		Projectile.hostile = true;
		base.SetDefaults();
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.usesOwnerLight = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		if (Timer == 0)
		{
			Projectile.Top = TileTopLeft.ToWorldCoordinates() + new Vector2(64, 0) + new Vector2(-8);
			Projectile.velocity *= 0;
			Projectile.velocity.Y = 8;
		}
		Timer++;
		if (Timer < 30)
		{
			Projectile.velocity.Y += 2;
			if (Timer > 5 && Collision.SolidCollision(Projectile.position + new Vector2(0, 80), 128, 16))
			{
				Timer = 30;
				Projectile.damage /= 60;
				ShakerManager.AddShaker(Projectile.Center, new Vector2(0, -1), 40, 0.5f, 200, 0.9f, 0.8f, 150);
				Collision.HitTiles(Projectile.position, new Vector2(0, -1), Projectile.width, Projectile.height);

				for (int g = 0; g < 15; g++)
				{
					Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(12f, 18f)).RotatedByRandom(MathHelper.TwoPi);
					newVelocity.Y = -Math.Abs(newVelocity.Y);
					newVelocity.Y *= Main.rand.NextFloat(0.1f, 1.4f);
					newVelocity.X *= 0.01f;
					Vector2 posCheck = Projectile.Top + new Vector2((g - 7) * 10, 8);
					for (int h = 0; h < 100; h++)
					{
						posCheck.Y += 41;
						if (Collision.IsWorldPointSolid(posCheck))
						{
							break;
						}
					}
					var somg = new RockSmog_Cone_FallingSandDust
					{
						Velocity = newVelocity,
						Active = true,
						Visible = true,
						Position = posCheck,
						MaxTime = Main.rand.NextFloat(90, Math.Max(newVelocity.Y * 6, 163)),
						Scale = Main.rand.NextFloat(6f, 9f),
						Rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
					};
					Ins.VFXManager.Add(somg);
				}
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), Projectile.Center);
				// These code was over-destructive, so deleted
				// for(int x = 0;x < 8;x++)
				// {
				// for(int y = 0; y < 3; y++)
				// {
				// Tile tile = TileUtils.SafeGetTile(Projectile.TopLeft.ToTileCoordinates() + new Point(x, y));
				// ModTile mt = TileLoader.GetTile(tile.TileType);
				// }
				// }
			}
		}
		else
		{
			if (Timer >= 150)
			{
				if (Timer == 150)
				{
					Collision.HitTiles(Projectile.position, new Vector2(0, -1), Projectile.width, Projectile.height);
				}
				Projectile.velocity.Y -= 0.2f;
				if (Projectile.velocity.Y < -6)
				{
					Projectile.velocity.Y = -6;
				}
				int boundY = TileTopLeft.Y * 16 + 8;
				if (Projectile.Hitbox.Top().Y <= boundY)
				{
					int deltaY = (int)(boundY - Projectile.Hitbox.Top().Y);
					Projectile.height -= deltaY;
					Projectile.Top = new Vector2(Projectile.Top.X, boundY);
					if (Projectile.height <= 0)
					{
						Projectile.height = 0;
						Projectile.Kill();
						return;
					}
				}
			}
			else
			{
				Projectile.velocity *= 0;
			}
		}
	}



	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		Texture2D texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Rectangle frameMain = new Rectangle(0, 30, 128, 92);
		int deltaY = 92 - Projectile.height;
		Vector2 drawPos = Projectile.Top;
		if (deltaY > 0)
		{
			frameMain = new Rectangle(0, 30 + deltaY, 128, 92 - deltaY);
		}
		Color lightAxe = Lighting.GetColor(drawPos.ToTileCoordinates());
		Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frameMain, lightAxe, 0, new Vector2(64, 0), 1f, SpriteEffects.None, 0);
		int boundY = TileTopLeft.Y * 16 + 8;
		for (int k = 0; k < 1000; k++)
		{
			drawPos.Y -= 8;
			Rectangle frameChain = new Rectangle(58, 16, 12, 12);
			if (k % 2 == 1)
			{
				frameChain = new Rectangle(58, 0, 12, 14);
			}
			if (drawPos.Y < boundY)
			{
				break;
			}
			Color light = Lighting.GetColor(drawPos.ToTileCoordinates());
			Main.EntitySpriteDraw(texture, drawPos - Main.screenPosition, frameChain, light, 0, frameChain.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		}
		return false;
	}
}
