using Terraria;
using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles.Typeless;

public class FlowerPetalPurple : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Flower Petal Purple");
		Main.projFrames[Projectile.type] = 8;
	}

	public override void SetDefaults()
	{
		Projectile.width = 12;
		Projectile.height = 14;
		Projectile.friendly = false;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 9000;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		if (Projectile.timeLeft < 60)
		{
			return new Color?(new Color(0.5f * Projectile.timeLeft / 60f, 0.5f * Projectile.timeLeft / 60f, 0.5f * Projectile.timeLeft / 60f, 0));
		}
		else
		{
			return new Color?(new Color(0.5f, 0.5f, 0.5f, 0));
		}
	}

	public float num2 = 0;
	public bool Hittil = false;
	private int TLF = 400;

	public override void AI()
	{
		if (Projectile.timeLeft >= 8999)
		{
			TLF = Main.rand.Next(600, 1000);
			Projectile.timeLeft = TLF;
		}
		if (num2 == 0)
		{
			num2 = Main.rand.Next(-100, 100) / 1000f;
		}

		if (Projectile.timeLeft < TLF - 20)
		{
			Projectile.friendly = true;
		}

		if (Projectile.velocity.Length() > 0.1f)
		{
			Projectile.frameCounter++;
			if (Projectile.frameCounter > 6)
			{
				Projectile.frame++;
				Projectile.frameCounter = 0;
			}
			if (Projectile.frame > 7)
			{
				Projectile.frame = 0;
			}
		}
		if (!Hittil)
		{
			Projectile.rotation += num2;
			if (Projectile.velocity.Length() < 3.6f && Projectile.timeLeft > 60)
			{
				Projectile.velocity.Y += 0.025f;
			}

			if (Projectile.timeLeft > 60)
			{
				Projectile.velocity.X += (float)Math.Sin(Projectile.timeLeft / 30f) * 0.035f;
			}

			if (Projectile.velocity.Length() > 3.6f)
			{
				Projectile.velocity *= 0.96f;
			}

			Projectile.velocity += new Vector2(Main.windSpeedCurrent * 0.05f, 0);
		}
		if (Projectile.timeLeft >= 60)
		{
			Projectile.alpha = 0;
			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.12f / 255f, (255 - Projectile.alpha) * 0f / 255f, (255 - Projectile.alpha) * 0.12f / 255f);
		}
		else
		{
			Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0.12f / 255f * Projectile.timeLeft / 60f, (255 - Projectile.alpha) * 0f / 255f * Projectile.timeLeft / 60f, (255 - Projectile.alpha) * 0.12f / 255f * Projectile.timeLeft / 60f);
			Projectile.alpha = (int)((60 - Projectile.timeLeft) / 60f * 255f);
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		Projectile.timeLeft = 60;
		if (Projectile.oldVelocity.Y > 0)
		{
			Projectile.velocity *= 0;
			Hittil = true;
			Projectile.tileCollide = false;
		}
		else
		{
			Projectile.velocity.Y *= -1;
			Projectile.position += Projectile.velocity * 2;
		}
		return false;
	}

	/*public override Color? GetAlpha(Color lightColor)
        {
            if (Projectile.timeLeft > 60)
            {
                return new Color?(new Color(1f,1f,1f,1f));
            }
            else
            {
                return new Color?(new Color(1f * Projectile.timeLeft / 60f, 1f * Projectile.timeLeft / 60f, 1f * Projectile.timeLeft / 60f, 1f * Projectile.timeLeft / 60f));
            }
        }*/
	/*public override void OnKill(int timeLeft)
	{
            Main.PlaySound(2, (int)Projectile.position.X, (int)Projectile.position.Y, 37, 0.5f, 0f);
        }*/
	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture2D = (Texture2D)ModContent.Request<Texture2D>(Texture);
		int num = texture2D.Height / Main.projFrames[Projectile.type];
		int y = num * Projectile.frame;
		Main.spriteBatch.Draw(texture2D, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY + Projectile.height / 2f), new Rectangle?(new Rectangle(0, y, texture2D.Width, num)), Projectile.GetAlpha(lightColor), Projectile.rotation, new Vector2(texture2D.Width / 2f, num / 2f), Projectile.scale, SpriteEffects.None, 0f);
		return false;
	}
}
