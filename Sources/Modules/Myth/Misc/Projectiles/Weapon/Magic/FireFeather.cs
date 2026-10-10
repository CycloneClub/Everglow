using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.Misc.Projectiles.Weapon.Magic.FireFeatherMagic;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Magic;

public class FireFeather : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.ignoreWater = false;
		Projectile.DamageType = DamageClass.Magic;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 360;
		Projectile.penetrate = 1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 2;
	}

	public int TimeToKill = -1;
	private ModProjectile magicArray = null;

	public override void OnSpawn(IEntitySource source)
	{
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile.active)
			{
				if (projectile.type == ModContent.ProjectileType<FireFeatherMagicArray>())
				{
					if (projectile.owner == Projectile.owner)
					{
						magicArray = projectile.ModProjectile;
						break;
					}
				}
			}
		}
	}

	public override void AI()
	{
		if (TimeToKill >= 0 && TimeToKill <= 2)
		{
			Projectile.Kill();
		}

		if (TimeToKill <= 15 && TimeToKill > 0)
		{
			Projectile.velocity = Projectile.oldVelocity;
		}

		TimeToKill--;
		if (TimeToKill >= 0)
		{
			if (TimeToKill < 10)
			{
				Projectile.damage = 0;
				Projectile.friendly = false;
			}
			Projectile.velocity *= 0f;
		}
		else
		{
			Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
			if (Projectile.timeLeft >= 320)
			{
				Projectile.velocity *= 1.01f;
			}
			else
			{
				if (Projectile.wet)
				{
					Projectile.velocity.Y -= 0.3f;
					Projectile.velocity *= 0.96f;
					Projectile.timeLeft -= Main.rand.Next(40, 80);
				}
				if (Projectile.timeLeft < 300)
				{
					if (Projectile.extraUpdates == 0)
					{
						Projectile.velocity.Y += 0.1f;
					}
				}
			}
		}
		if (Main.rand.NextBool(6))
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(0, 2)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(0.8f, 1.7f));
		}
		if (Main.rand.NextBool(2))
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.9f),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-3f, 2f),
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(0.1f, 12.0f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
		if (Main.rand.NextBool(2))
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 0.6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireDust
			{
				Velocity = newVelocity + Projectile.velocity * Main.rand.NextFloat(0f, 0.9f),
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + Projectile.velocity * Main.rand.NextFloat(-1f, 2f),
				MaxTime = Main.rand.Next(11, 25),
				Scale = Main.rand.NextFloat(0.1f, 12.0f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
			};
			Ins.VFXManager.Add(spark);
		}
		if (Projectile.timeLeft <= 100 && TimeToKill < 0)
		{
			if (magicArray != null)
			{
				var arrayProj = magicArray as FireFeatherMagicArray;
				arrayProj.WingPower += 0.1f;
			}
			AmmoHit();
		}
		if (Projectile.position.X <= 320 || Projectile.position.X >= Main.maxTilesX * 16 - 320)
		{
			Projectile.Kill();
		}
		if (Projectile.position.Y <= 320 || Projectile.position.Y >= Main.maxTilesY * 16 - 320)
		{
			Projectile.Kill();
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		if (TimeToKill >= 0)
		{
			return;
		}
		SpriteEffects spriteEffects = SpriteEffects.None;
		if (Projectile.spriteDirection == -1)
		{
			spriteEffects = SpriteEffects.FlipHorizontally;
		}

		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		int frameHeight = texture.Height / Main.projFrames[Projectile.type];
		int startY = frameHeight * Projectile.frame;
		var sourceRectangle = new Rectangle(0, startY, texture.Width, frameHeight);
		Vector2 origin = sourceRectangle.Size() / 2f;
		float offsetX = 20f;
		origin.X = Projectile.spriteDirection == 1 ? sourceRectangle.Width - offsetX : offsetX;
		float amount = 1f;
		if (Projectile.timeLeft >= 1040)
		{
			amount = (1080 - Projectile.timeLeft) / 40f;
		}
		Color aimColor = new Color(1f, 1f, 1f, 1f);
		Color drawColor = Color.Lerp(lightColor, aimColor, amount);
		if (Projectile.wet)
		{
			float value = 0.6f;
			if (Projectile.timeLeft < 700)
			{
				value = (Projectile.timeLeft - 100) / 1000f;
			}
			aimColor = new Color(value, value / 12f, 0f, 1f);
			drawColor = aimColor;
		}

		Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), sourceRectangle, drawColor, Projectile.rotation, origin, Projectile.scale, spriteEffects, 0);
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (magicArray != null)
		{
			var arrayProj = magicArray as FireFeatherMagicArray;
			arrayProj.WingPower += 0.1f;
		}
		AmmoHit();
		return false;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (magicArray != null)
		{
			var arrayProj = magicArray as FireFeatherMagicArray;
			arrayProj.WingPower += 2f;
			target.AddBuff(BuffID.OnFire3, 600);
		}
		else
		{
			target.AddBuff(BuffID.OnFire, 600);
		}

		AmmoHit();
	}

	public void AmmoHit()
	{
		TimeToKill = 20;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.velocity = Projectile.oldVelocity;

		SoundEngine.PlaySound(SoundID.DD2_BetsyFlameBreath.WithVolume(0.3f).WithPitchOffset(0.8f), Projectile.Center);
		for (int j = 0; j < 4; j++)
		{
			Vector2 v = new Vector2(0, Main.rand.NextFloat(7, 20)).RotatedByRandom(MathHelper.TwoPi);
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, ModContent.DustType<Dusts.FireFeather>(), v.X, v.Y, 150, default, Main.rand.NextFloat(1.8f, 3.7f));
		}
		GenerateFire(3);
		GenerateSmog(3);
		if (!Projectile.wet)
		{
			GenerateSpark(14);
		}
	}

	public void GenerateSmog(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var smog = new FireSmogDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(37, 45),
				Scale = Main.rand.NextFloat(20f, 35f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(smog);
		}
	}

	public void GenerateFire(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var fire = new FireDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) + newVelocity * 3,
				MaxTime = Main.rand.Next(9, 25),
				Scale = Main.rand.NextFloat(20f, 30f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(fire);
		}
	}

	public void GenerateSpark(int frequency)
	{
		float mulVelocity = 1f;
		for (int g = 0; g < frequency; g++)
		{
			Vector2 newVelocity = new Vector2(0, mulVelocity * Main.rand.NextFloat(2f, 6f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new FireSparkDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - Projectile.velocity + newVelocity * 3,
				MaxTime = Main.rand.Next(137, 245),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(0.1f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.13f, 0.13f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
