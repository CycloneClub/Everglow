using Everglow.Commons.DataStructures;
using Everglow.Commons.Graphics;
using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.KelpCurtain.VFXs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Magic;

public class DevilHeartStaff_proj_II : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MagicProjectiles;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(0.85f, 0.75f, 0.65f, 0f);
		TrailWidth = 36f;
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		TrailLength = 20;

		Projectile.DamageType = DamageClass.Magic;
		Projectile.width = 20;
		Projectile.height = 20;
		ProjTrailColor.colorList.Add((new Color(203, 73, 229, 0), 0));
		ProjTrailColor.colorList.Add((new Color(95, 99, 226, 0), 0.4f));
		ProjTrailColor.colorList.Add((new Color(206, 187, 165, 0), 0.8f));
		ProjTrailColor.colorList.Add((new Color(104, 104, 104, 0), 1f));
	}

	public GradientColor ProjTrailColor = new();

	public override void Behaviors()
	{
		Vector2 vel = new Vector2(0, Main.rand.NextFloat(0.6f, 3.4f)).RotatedByRandom(MathHelper.TwoPi) + Projectile.velocity;
		var dust = new DevilHeart_Spark
		{
			Velocity = vel,
			Active = true,
			Visible = true,
			Position = Projectile.Center,
			MaxTime = Main.rand.Next(80, 150),
			Scale = Main.rand.NextFloat(3f, 8f),
			Rotation = Main.rand.NextFloat(6.283f),
			ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
		};
		Ins.VFXManager.Add(dust);
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		base.ModifyHitNPC(target, ref modifiers);
		modifiers.FinalDamage *= 0;
		modifiers.HideCombatText();
	}

	public override void DestroyEntityEffect()
	{
		for (int i = 0; i < 40; i++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(5.6f, 8.4f)).RotatedByRandom(MathHelper.TwoPi);
			var dust = new DevilHeart_Spark
			{
				Velocity = vel,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(60, 120),
				Scale = Main.rand.NextFloat(7f, 10f),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(4.0f, 10.93f) },
			};
			Ins.VFXManager.Add(dust);
		}

		float stepMax = 240;
		float petals = 8f;
		for (int g = 0; g < stepMax; g++)
		{
			Vector2 velocity = new Vector2(0, 15f).RotatedBy(g / stepMax * MathHelper.TwoPi);
			velocity = velocity.RotatedBy(Projectile.rotation + Projectile.whoAmI + MathHelper.Pi);
			velocity *= MathF.Sin(g / (stepMax / (petals / 2)) * MathHelper.TwoPi);
			var somg = new DevilHeart_Spark_II
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = 120,
				Scale = 1,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(8.0f, 12f), MathF.Sin(g / (stepMax / petals) * MathHelper.TwoPi), MathF.Sin(g / (stepMax / (petals * 5f)) * MathHelper.TwoPi) * 0.26f },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int i = 0; i < 60; i++)
		{
			float rotSpeed = 0;
			Vector2 vel = new Vector2(4 + MathF.Sin(i / (60f / 24f) * MathHelper.TwoPi), 0).RotatedBy(i / 60f * MathHelper.TwoPi);
			var dustVFX = new BoneHeart_VFX
			{
				Omega = rotSpeed,
				Beta = -rotSpeed * 0.05f,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				Velocity = vel,
				MaxTime = vel.Length() * 12,
				Scale = 9f,
				BoneHeartColor = Color.Lerp(Color.Red, Color.White, (vel.Length() - 3) / 2f),
				ai = new float[] { Main.rand.NextFloat(1f, 8f) },
			};
			Ins.VFXManager.Add(dustVFX);
		}
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<DevilHeartStaff_proj_Kill>(), Projectile.damage, Projectile.knockBack * 2, Projectile.owner);
	}

	public override void DrawSelf()
	{
		var color = new Color(225, 68, 223, 0);
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, MathHelper.PiOver2, star.Size() / 2f, new Vector2(0.6f, 1), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, 0, star.Size() / 2f, new Vector2(0.6f, 0.6f), SpriteEffects.None, 0);

		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, color, Projectile.velocity.ToRotation() - MathHelper.PiOver2, texMain.Size() / 2f, 0.8f, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			return ProjTrailColor.GetColor(index / (float)SmoothedOldPos.Count);
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}
}
