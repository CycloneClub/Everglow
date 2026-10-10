using Terraria;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class Sunflower : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 3000;
		Main.projFrames[Projectile.type] = 5;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.timeLeft > 2950)
		{
			Projectile.soundDelay = 10;
			if (Projectile.velocity.X != oldVelocity.X && Math.Abs(oldVelocity.X) > 1f)
			{
				Projectile.velocity.X = oldVelocity.X * -0.9f;
			}

			if (Projectile.velocity.Y != oldVelocity.Y && Math.Abs(oldVelocity.Y) > 1f)
			{
				Projectile.velocity.Y = oldVelocity.Y * -0.9f;
			}
		}
		return false;
	}

	/*public override void PostDraw(Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>("Everglow/Myth/Misc/Projectiles/Weapon/Melee/Sunflower_Glow").Value;
            Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 46 * Projectile.frame,46,46), new Color(0.7f,0.7f,0.7f, 0), Projectile.rotation, new Vector2(23f, 23f), 1f, SpriteEffects.None, 0f);
        }*/
	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModContent.Request<Texture2D>("Everglow/Myth/Misc/Projectiles/Weapon/Melee/Sunflower").Value;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, 46 * Projectile.frame, 46, 46), lightColor, Projectile.rotation, new Vector2(23), 1f, SpriteEffects.None, 0f);
		return false;
	}

	public override void AI()
	{
		float num7 = (float)Math.Sqrt(Projectile.velocity.X * Projectile.velocity.X + Projectile.velocity.Y * Projectile.velocity.Y);
		Player p = Main.player[Projectile.owner];
		Projectile.rotation += 0.05f * num7;
		float num6 = (float)Math.Sqrt((p.Center.X - Projectile.Center.X) * (p.Center.X - Projectile.Center.X) + (p.Center.Y - Projectile.Center.Y) * (p.Center.Y - Projectile.Center.Y));
		if (Projectile.timeLeft <= 2950)
		{
			if (num7 < 9f)
			{
				Projectile.velocity *= 1.2f;
			}

			if (num7 > 10f)
			{
				Projectile.velocity *= 0.86f;
			}

			int num3 = Player.FindClosest(Projectile.Center, 1, 1);
			Projectile.velocity = Projectile.velocity * 0.98f + (p.Center - Projectile.Center) / num6 * 3.5f;
			Projectile.tileCollide = false;
		}
		else
		{
			if (num7 < 9f)
			{
				Projectile.velocity *= 1.2f;
			}

			if (num7 > 10f)
			{
				Projectile.velocity *= 0.96f;
			}

			Projectile.velocity = Projectile.velocity * 0.995f + (p.Center - Projectile.Center) / num6 * 0.15f;
		}
		if (num6 < 60 && Projectile.timeLeft < 2950)
		{
			Projectile.timeLeft = 0;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		Vector2 v1 = target.Center;
		if (Projectile.frame < 4)
		{
			for (int t = 0; t < 4; t++)
			{
				Vector2 v2 = new Vector2(0, Main.rand.NextFloat(0, 4f)).RotatedByRandom(Math.PI * 2d);
				int y = Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), v1.X, v1.Y, v2.X, v2.Y, ModContent.ProjectileType<SunFlowerpetal>(), Projectile.damage / 2, 0.5f, Main.myPlayer);
				Main.projectile[y].scale = Main.rand.NextFloat(0.9f, 1.1f);
				Main.projectile[y].damage = (int)(Projectile.damage * Main.projectile[y].scale);
				Main.projectile[y].frame = Main.rand.Next(0, 8);
			}
			Projectile.frame++;
		}
	}
}
