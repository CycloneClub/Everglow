using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class Lantern_ExplosionEffect : ModProjectile
{
	public override string Texture => Commons.ModAsset.Empty_Mod;

	public int Timer = 0;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 10;
		Projectile.width = 60;
		Projectile.height = 60;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		for (int g = 0; g < 6; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(12f, 20f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new HitEffectSpark
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(16, 20),
				DrawColor = new Color(1f, 0.4f, 0, 0),
				LightFlat = 1f,
				SpeedDecay = 0.8f,
				GravityAcc = 0.15f,
				SelfLight = true,
				Scale = Main.rand.NextFloat(16f, 28f),
			};
			Ins.VFXManager.Add(spark);
		}
		for (int g = 0; g < 18; g++)
		{
			float sqrtSpeed = MathF.Sqrt(Main.rand.NextFloat(1f));
			Vector2 newVelocity = new Vector2(0, sqrtSpeed * 5).RotatedByRandom(MathHelper.TwoPi);
			var somg = new LanternFlameDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(20), 0).RotatedByRandom(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(30, 60),
				Scale = Main.rand.NextFloat(32f, 88f),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				RotateSpeed = Main.rand.NextFloat(-0.8f, 0.8f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public override void AI()
	{
		Timer++;
		if (Timer > 2)
		{
			Projectile.friendly = false;
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		target.AddBuff(BuffID.OnFire3, 200);
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float timeValue = Projectile.timeLeft / 6f;
		Color drawColor = Color.Lerp(new Color(0.7f, 0.1f, 0f, 0), new Color(1f, 1f, 1f, 0), timeValue);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, drawColor, MathHelper.PiOver2, star.Size() * 0.5f, new Vector2(timeValue, Projectile.scale) * 1.25f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, drawColor, 0, star.Size() * 0.5f, new Vector2(timeValue, Projectile.scale) * 1f, SpriteEffects.None, 0);
		for (int k = 0; k < 3; k++)
		{
			Main.EntitySpriteDraw(star, Projectile.Center - Main.screenPosition, null, drawColor, Projectile.whoAmI + MathHelper.Pi / 3f * k * 2, star.Size() * 0.5f, new Vector2(timeValue, Projectile.scale) * 0.75f, SpriteEffects.None, 0);
		}
		return false;
	}
}
