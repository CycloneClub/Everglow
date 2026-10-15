using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Magic;

public class AmberLiquidProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetDefaults()
	{
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 3600;
		Projectile.DamageType = DamageClass.Magic;
	}

	public bool Collided = false;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override void AI()
	{
		Projectile.frameCounter++;
		if (Projectile.frameCounter > 4)
		{
			Projectile.frameCounter = 0;
			Projectile.frame++;
			if (Projectile.frame >= 8)
			{
				Projectile.frame = 0;
			}
		}
		Projectile.rotation = Projectile.velocity.ToRotation();
		if (!Collided)
		{
			if (Collision.SolidCollision(Projectile.position + new Vector2(Projectile.velocity.X, 0), 20, 20))
			{
				Collided = true;
				Projectile.velocity.X *= -1;
				for (int i = 0; i < 15; i++)
				{
					GenerateOrangeSpark();
					GenerateSmog();
				}
				SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
			}
			if (Collision.SolidCollision(Projectile.position + new Vector2(0, Projectile.velocity.Y), 20, 20))
			{
				Collided = true;
				Projectile.velocity.Y *= -1;
				for (int i = 0; i < 15; i++)
				{
					GenerateOrangeSpark();
					GenerateSmog();
				}
				SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
			}
		}

		base.AI();
	}

	public void GenerateSmog()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2f, 8f)).RotatedByRandom(MathHelper.TwoPi);
		var somg = new AmberSmogDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(30, 45),
			Scale = Main.rand.NextFloat(50f, 65f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
		};
		Ins.VFXManager.Add(somg);
	}

	public void GenerateOrangeSpark()
	{
		Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4.0f, 8f)).RotatedByRandom(MathHelper.TwoPi);
		var spark = new AmberSparkDust
		{
			Velocity = newVelocity,
			Active = true,
			Visible = true,
			Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
			MaxTime = Main.rand.Next(30, 45),
			Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(17f, 27.0f)),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f) },
		};
		Ins.VFXManager.Add(spark);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D texture = ModAsset.AmberLiquidProj.Value;
		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 50, 44, 50), lightColor, Projectile.rotation, new Vector2(22, 25), Projectile.scale, SpriteEffects.None, 0);
		if (Projectile.timeLeft > 3500)
		{
			Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, new Rectangle(0, Projectile.frame * 50, 44, 50), new Color(1f, 1f, 1f, 0) * ((Projectile.timeLeft - 3500) / 100f), Projectile.rotation, new Vector2(22, 25), Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolumeScale(0.8f).WithPitchOffset(1f), Projectile.Center);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<AmberLiquidProj_Explosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 1);
		base.OnKill(timeLeft);
	}
}
