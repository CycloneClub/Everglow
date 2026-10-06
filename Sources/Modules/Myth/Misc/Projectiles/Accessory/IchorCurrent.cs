using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;

namespace Everglow.Myth.Misc.Projectiles.Accessory;

public class IchorCurrent : ModProjectile
{
	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 2;
		Projectile.timeLeft = 240;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Magic;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
	}

	public void GenerateVFX(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			var blood = new IchorDrop
			{
				Velocity = Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(32, 64),
				Scale = Main.rand.NextFloat(6f, 14f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public void GenerateVFXII(int Frequency)
	{
		for (int g = 0; g < Frequency; g++)
		{
			var blood = new IchorSplash
			{
				Velocity = Projectile.velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(12, 36),
				Scale = Main.rand.NextFloat(1f, 5f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 0.0f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override void AI()
	{
		int vfxFrequency = 200;
		if (Ins.VisualQuality.High)
		{
			vfxFrequency = 3;
		}
		if (VFXManager.InScreen(Projectile.position, 160))
		{
			if (Main.rand.NextBool(vfxFrequency))
			{
				GenerateVFX(1);
				if (Ins.VisualQuality.High && Main.rand.NextBool(vfxFrequency))
				{
					GenerateVFXII(1);
				}
			}
		}

		float kTime = 1f;
		if (Projectile.timeLeft < 90f)
		{
			kTime = Projectile.timeLeft / 90f;
		}

		Lighting.AddLight((int)(Projectile.Center.X / 16), (int)(Projectile.Center.Y / 16), 0.32f * kTime, 0.23f * kTime, 0);
		for (int x = 0; x < 8; x++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) + Projectile.velocity * Main.rand.NextFloat(1f);
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Ichor, 0, 0, 0, default, 0.6f);
			d0.noGravity = true;
			d0.velocity *= 0;
		}
		Projectile.velocity.Y += 0.15f;

		if (Projectile.timeLeft == 210)
		{
			Projectile.friendly = true;
		}
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		SoundEngine.PlaySound(SoundID.Drip, Projectile.Center);
		for (int x = 0; x < 15; x++)
		{
			Vector2 BasePos = Projectile.Center - new Vector2(4) - Projectile.velocity;
			var d0 = Dust.NewDustDirect(BasePos, 0, 0, DustID.Ichor, 0, 0, 0, default, 0.6f);
			d0.noGravity = true;
		}
		if (Projectile.ai[0] != 3)
		{
			for (int x = 0; x < 3; x++)
			{
				Vector2 velocity = new Vector2(0, Main.rand.NextFloat(2f, 6f)).RotatedByRandom(6.283) - Projectile.velocity * 0.2f;
				var p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center + velocity * -2, velocity, ModContent.ProjectileType<IchorCurrent>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 3f/*If ai[0] equal to 3, another ai will be execute*/);
				p.friendly = false;
				p.CritChance = Projectile.CritChance;
			}
		}
		target.AddBuff(BuffID.Ichor, 600);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return false;
	}
}
