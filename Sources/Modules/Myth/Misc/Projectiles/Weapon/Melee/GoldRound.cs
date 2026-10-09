using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee;

public class GoldRound : ModProjectile
{
	public override void SetStaticDefaults()
	{
	}

	public override void SetDefaults()
	{
		Projectile.width = 16;
		Projectile.height = 16;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.penetrate = 1;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 1500;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	private float omega = 0.4f;

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
	}

	public override void AI()
	{
		Projectile.rotation += omega;
		Projectile.velocity *= 0.98f;
		Player player = Main.player[Projectile.owner];
		if (Projectile.timeLeft < 1470)
		{
			Projectile.friendly = true;
			Vector2 aimTarget = player.Center;
			float minDis = 1500;
			Projectile aimProj = null;
			foreach (Projectile proj in Main.projectile)
			{
				if (proj.active)
				{
					if (proj.owner == Projectile.owner && proj.type == ModContent.ProjectileType<GoldRoundYoyo>())
					{
						if ((proj.Center - Projectile.Center).Length() < minDis)
						{
							minDis = (proj.Center - Projectile.Center).Length();
							aimTarget = proj.Center;
							aimProj = proj;
						}
					}
				}
			}
			if (minDis < 30)
			{
				Projectile.Kill();
				if (aimProj != null)
				{
					GoldRoundYoyo gRY = aimProj.ModProjectile as GoldRoundYoyo;
					if (gRY != null)
					{
						gRY.Power += 15;
					}
				}
			}
			Projectile.velocity += Vector2.Normalize(aimTarget - Projectile.Center - Projectile.velocity) * 5f;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		for (int i = 0; i < 6; i++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(1.5f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			int num = Dust.NewDust(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, 87, 0f, 0f, 100, default, 1.2f);
			Main.dust[num].velocity *= v;
			Main.dust[num].noGravity = true;
		}
	}

	public override void OnKill(int timeLeft)
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D t = ModAsset.GoldRound.Value;
		Color c0 = new Color(0.5f, 0.3f, 0.04f, 0);
		Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, t.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
		if (Projectile.timeLeft > 1470)
		{
			for (int x = 0; x < Projectile.timeLeft - 1470; x += 3)
			{
				Main.spriteBatch.Draw(t, Projectile.Center - Main.screenPosition, null, c0, Projectile.rotation, t.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
			}
		}
		return false;
	}
}
