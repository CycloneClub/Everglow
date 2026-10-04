using Everglow.Commons.Templates.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class CyanBullet : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.aiStyle = -1;
		Projectile.penetrate = 6;
		Projectile.timeLeft = 3600;

		SelfLuminous = true;
		WarpStrength = 0.1f;
		TrailBackgroundDarkness = 0.8f;
		TrailTexture = Commons.ModAsset.Trail_7.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_7_black.Value;
		TrailLength = 7;
		TrailColor = new Color(0.2f, 0.9f, 0.5f, 0f);
		TrailWidth = 5f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.damage += 2;
		base.OnSpawn(source);
	}

	public override bool PreAI()
	{
		if (TimeAfterEntityDestroy >= 0)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.7f, 0.8f) * TimeAfterEntityDestroy / 7f);
		}
		return true;
	}

	public override void Behaviors()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			Lighting.AddLight(Projectile.Center, new Vector3(0.3f, 0.7f, 0.8f));
		}
		if (Projectile.timeLeft == 3540)
		{
			Projectile.damage -= 2;
		}
	}

	public override void DrawSelf()
	{
		Texture2D light_dark = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(light_dark, Projectile.Center - Main.screenPosition, null, new Color(0.6f, 0.6f, 0.6f, 0.6f), Projectile.velocity.ToRotationSafe() + MathHelper.PiOver2, light_dark.Size() * 0.5f, 0.7f, SpriteEffects.None, 0);
		Texture2D light = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(light, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.7f, 0.6f, 0f), Projectile.velocity.ToRotationSafe() + MathHelper.PiOver2, light.Size() * 0.5f, 0.7f, SpriteEffects.None, 0);

		Texture2D ball = Commons.ModAsset.TileBlock.Value;
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.7f, 0.4f, 0.5f), (float)Main.time * 0.03f + Projectile.whoAmI, ball.Size() * 0.5f, 0.4f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(ball, Projectile.Center - Main.screenPosition, null, new Color(0.1f, 0.7f, 0.4f, 0.5f), (float)Main.time * 0.03f + Projectile.whoAmI + MathHelper.PiOver4, ball.Size() * 0.5f, 0.4f, SpriteEffects.None, 0);
		return;
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			float value = index / (float)SmoothedOldPos.Count;
			Color color = Color.Lerp(TrailColor, new Color(0, 14, 255, 0), value);
			return color;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);
	}

	public override void DestroyEntityEffect()
	{
		SoundEngine.PlaySound(SoundID.NPCHit4.WithVolumeScale(0.8f), Projectile.Center);
		for (int i = 0; i < 5; i++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(2.0f, 24f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new Spark_MoonBladeDust
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(3, 7),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(16f, 27.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
