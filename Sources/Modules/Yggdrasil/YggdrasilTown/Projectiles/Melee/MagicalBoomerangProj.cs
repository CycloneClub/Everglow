using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.WorldBuilding;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class MagicalBoomerangProj : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 3600;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Melee;
		Projectile.width = 30;
		Projectile.height = 30;
		Timer = 0;
	}

	public bool Returning = false;

	public int Timer = 0;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Returning)
		{
			return false;
		}
		Projectile.velocity *= -1;
		Returning = true;
		for (int i = 0; i < 24; i++)
		{
			var dustVFX = new MagicalBoomerangDust
			{
				velocity = new Vector2(0, Main.rand.NextFloat(6)).RotatedByRandom(MathHelper.TwoPi),
				gravity = true,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(20, 90),
				scale = Main.rand.NextFloat(6, 12),
				rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
	}

	public override void AI()
	{
		Player player = Main.player[Projectile.owner];
		Timer++;
		Projectile.rotation += 0.3f;
		if (!Returning && Timer > 60)
		{
			Returning = true;
		}
		if (Returning)
		{
			Projectile.tileCollide = false;
			Vector2 toPlayer = player.Center + player.velocity - Projectile.Center - Projectile.velocity;
			float speed = 13f;
			speed += (Timer - 60) / 10f;
			if (toPlayer.Length() < speed * 2)
			{
				Projectile.Kill();
			}
			Projectile.velocity = Projectile.velocity * 0.9f + toPlayer.NormalizeSafe() * speed * 0.1f;
		}
		if (Timer > 5 && Main.mouseLeft && Main.mouseLeftRelease)
		{
			Projectile.Kill();
			float speed = 13f;
			Vector2 startVel = new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi) * speed;
			for (int i = 0; i < 8; i++)
			{
				Vector2 vel = startVel.RotatedBy(i / 8f * MathHelper.TwoPi);
				var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, vel, ModContent.ProjectileType<MagicalBoomerangSubProj>(), (int)(Projectile.damage * 0.7), Projectile.knockBack * 0.7f, Projectile.owner);
				proj.rotation = vel.ToRotation();
				for (int step = 0; step < 20; step++)
				{
					var dustVFX = new MagicalBoomerangDust
					{
						velocity = Vector2.zeroVector,
						Active = true,
						Visible = true,
						position = Projectile.Center + vel / 5f * step,
						maxTime = Main.rand.Next(30, 46),
						scale = Main.rand.NextFloat(30, 40),
						rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFX);

					float distanceSide = (20 - step) / 5f;
					var dustVFXLeft = new MagicalBoomerangDust
					{
						velocity = Vector2.zeroVector,
						Active = true,
						Visible = true,
						position = Projectile.Center + vel / 5f * step + vel.RotatedBy(1.2f) / 6f * distanceSide,
						maxTime = Main.rand.Next(30, 46),
						scale = Main.rand.NextFloat(30, 40),
						rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFXLeft);
					var dustVFXRight = new MagicalBoomerangDust
					{
						velocity = Vector2.zeroVector,
						Active = true,
						Visible = true,
						position = Projectile.Center + vel / 5f * step + vel.RotatedBy(-1.2f) / 6f * distanceSide,
						maxTime = Main.rand.Next(30, 46),
						scale = Main.rand.NextFloat(30, 40),
						rotation = Main.rand.NextFloat(MathHelper.TwoPi),
						ai = new float[] { 0, 0, 0 },
					};
					Ins.VFXManager.Add(dustVFXRight);
				}
			}
		}
		Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.7f, 1f) * 0.5f);
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		if (!Returning)
		{
			Returning = true;
		}
		for (int i = 0; i < 48; i++)
		{
			var dustVFX = new MagicalBoomerangDust
			{
				velocity = new Vector2(0, Main.rand.NextFloat(8)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity * 0.5f,
				gravity = true,
				Active = true,
				Visible = true,
				position = Projectile.Center,
				maxTime = Main.rand.Next(20, 90),
				scale = Main.rand.NextFloat(6, 12),
				rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void OnKill(int timeLeft) => base.OnKill(timeLeft);

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D boomerang = ModAsset.MagicalBoomerangProj.Value;
		Texture2D boomerangGlow = ModAsset.MagicalBoomerangProj_glow.Value;
		Texture2D shape = ModAsset.MagicalBoomerangProj_shape.Value;
		Texture2D bloom = ModAsset.MagicalBoomerangProj_bloom.Value;
		Main.EntitySpriteDraw(boomerang, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, boomerang.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(boomerangGlow, Projectile.Center - Main.screenPosition, null, new Color(0.3f, 0.7f, 1f, 0), Projectile.rotation, boomerangGlow.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

		float value = (30 - Timer) / 30f;
		if (value > 0)
		{
			Main.EntitySpriteDraw(shape, Projectile.Center - Main.screenPosition, null, Color.Lerp(Color.White, new Color(0f, 0.2f, 0.8f, 1f), MathF.Pow(1 - value, 0.5f)) * value, Projectile.rotation, shape.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		value = (240 - Timer) / 240f;
		if (value > 0)
		{
			Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0) * value, Projectile.rotation, bloom.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
