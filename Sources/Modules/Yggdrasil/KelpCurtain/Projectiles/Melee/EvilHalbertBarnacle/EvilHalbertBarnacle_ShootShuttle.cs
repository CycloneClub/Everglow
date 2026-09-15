using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Melee.EvilHalbertBarnacle;

public class EvilHalbertBarnacle_ShootShuttle : ModProjectile
{
	public int Timer = 0;

	public int State = 0;

	public Vector2 Offset = Vector2.zeroVector;

	public Vector2 OffsetVel = Vector2.zeroVector;

	public override string Texture => ModAsset.EvilHalbertBarnacle_proj_Mod;

	public override void SetDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 120000;
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		if (player.ownedProjectileCounts[ModContent.ProjectileType<EvilHalbertBarnacle_proj_shuttle>()] <= 0)
		{
			State = 0;
		}
		else
		{
			State = 1;
		}
		Timer++;
		if (Projectile.timeLeft > 60)
		{
			player.ListenMouseWorld();
			Projectile.velocity = (player.MouseWorld() - player.MountedCenter).NormalizeSafe();
			Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4 * 3;
			if (Projectile.velocity.X > 0)
			{
				Projectile.spriteDirection = 1;
			}
			else
			{
				Projectile.spriteDirection = -1;
				Projectile.rotation -= MathHelper.PiOver2;
			}
			player.direction = Projectile.spriteDirection;
			Projectile.velocity = Vector2.Normalize(Projectile.velocity);

			Projectile.Center = player.MountedCenter + Projectile.velocity * 120;
			if (Timer > 3 && Main.mouseRightRelease)
			{
				Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - Projectile.velocity * 85f, Projectile.velocity * 75, ModContent.ProjectileType<EvilHalbertBarnacle_proj_shuttle>(), Projectile.damage, 2.4f, Projectile.owner);
				Projectile.timeLeft = 20;
				OffsetVel = -Projectile.velocity.RotatedByRandom(0.3f) * 35;
				for (int i = 0; i < 18; ++i)
				{
					Vector2 vel = Projectile.velocity.RotateRandom(0.6f) * Main.rand.NextFloat(0.35f, 1.15f) * 45f;
					var dust = new BarnacleTissueDust
					{
						velocity = vel,
						Active = true,
						Visible = true,
						position = Projectile.Center + Projectile.velocity * 5,
						maxTime = Main.rand.Next(60, 90),
						scale = Main.rand.NextFloat(8f, 12f),
						rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(6f, 8f), Main.rand.NextFloat(1f) },
					};
					Ins.VFXManager.Add(dust);
				}
				for (int i = 0; i < 6; ++i)
				{
					Vector2 vel = Projectile.velocity.RotateRandom(0.6f) * Main.rand.NextFloat(0.35f, 1.15f) * 45f;
					var dust = new BarnacleTissueDust
					{
						velocity = vel,
						Active = true,
						Visible = true,
						position = Projectile.Center + Projectile.velocity * 5,
						maxTime = Main.rand.Next(60, 90),
						scale = Main.rand.NextFloat(36f, 48f),
						rotation = Main.rand.NextFloat(6.283f),
						ai = new float[] { Main.rand.NextFloat(6f, 8f), Main.rand.NextFloat(1f) },
					};
					Ins.VFXManager.Add(dust);
				}
			}
		}
		else
		{
			Offset += OffsetVel;
			OffsetVel += -Offset * 0.3f;
			OffsetVel *= 0.7f;
			player.ListenMouseWorld();
			Projectile.rotation = (Projectile.Center - player.MountedCenter).ToRotation() + MathHelper.PiOver4 * 3;
			if (Projectile.velocity.X > 0)
			{
				Projectile.spriteDirection = 1;
			}
			else
			{
				Projectile.spriteDirection = -1;
				Projectile.rotation -= MathHelper.PiOver2;
			}
			player.direction = Projectile.spriteDirection;
			player.direction = Projectile.spriteDirection;
			Projectile.Center = player.MountedCenter + Projectile.velocity * 120 + Offset;
		}
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation + MathHelper.PiOver4 * 3);
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		return false;
	}

	public Vector2 oldPos;

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = ModContent.Request<Texture2D>(Texture).Value;
		if (State == 1)
		{
			tex = ModAsset.EvilHalbertBarnacle_proj_released.Value;
		}
		Vector2 velNor2 = Projectile.velocity.NormalizeSafe() * tex.Width / MathF.Sqrt(2f);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition - velNor2, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, Projectile.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None, 0);
		return false;
	}
}
