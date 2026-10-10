using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Legacies;

public class BacterialAgent_proj : TrailingProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.MagicProjectiles;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.7f, 1f, 0.4f, 0);
		TrailTexture = Commons.ModAsset.Trail_2.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailWidth = 50;
		TrailLength = 30;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		target.AddBuff(ModContent.BuffType<LichenInfected>(), 900);
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void Behaviors()
	{
		Projectile.rotation += 0.15f;
		Projectile.velocity.Y += 0.2f;
		Projectile.velocity *= 0.99f;

		if (Main.rand.NextBool(3))
		{
			for (int x = 0; x < 1; x++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(0.5f, 1.8f)).RotatedByRandom(6.283f) + Projectile.velocity * 0.75f;
				var splash = new LichenSlimeSplash
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(12, 68),
					Scale = Main.rand.NextFloat(6f, 18f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
				};
				Ins.VFXManager.Add(splash);
			}
		}
		if (Main.rand.NextBool(2))
		{
			for (int x = 0; x < 1; x++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f, 8f)).RotatedByRandom(6.283f) * Main.rand.NextFloat(0.2f, 0.5f) + Projectile.velocity * 0.75f;
				float mulScale = Main.rand.NextFloat(6f, 15f);
				var blood = new LichenSlimeDrop
				{
					Velocity = afterVelocity,
					Active = true,
					Visible = true,
					Position = Projectile.Center,
					MaxTime = Main.rand.Next(32, 164),
					Scale = mulScale,
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
		}
	}

	public override void DestroyEntityEffect()
	{
		int times = 10;
		for (int x = 0; x < times; x++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f, 8f)).RotatedByRandom(6.283f);
			var splash = new LichenSlimeSplash
			{
				Velocity = afterVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(12, 48),
				Scale = Main.rand.NextFloat(6f, 18f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(splash);
		}
		for (int x = 0; x < times * 2; x++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(5f, 8f)).RotatedByRandom(6.283f) * Main.rand.NextFloat(6f, 15f);
			float mulScale = Main.rand.NextFloat(6f, 15f);
			var blood = new LichenSlimeDrop
			{
				Velocity = afterVelocity / mulScale,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(32, 94),
				Scale = mulScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		Projectile p = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<BacterialAgent_explosion>(), Projectile.damage, Projectile.knockBack * 4f, Projectile.owner, 30);
		p.rotation = Main.rand.NextFloat(6.283f);
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		DrawTrail();
		if (TimeAfterEntityDestroy > 0)
		{
			return false;
		}
		return true;
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.5f;
		float y = 1;
		float z = widthValue;
		if (phase == 2)
		{
			y = 0;
		}
		if (phase % 2 == 1)
		{
			y = 0.5f;
		}
		return new Vector3(x, y, z);
	}
}
