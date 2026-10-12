using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Myth.LanternMoon.Projectiles.Item_Shoot;

public class FlameCylinder : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetCustomDefaults()
	{
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Projectile.type] = true;

		TrailLength = 400;
		TrailColor = new Color(1, 0.65f, 0, 0f);
		TrailWidth = 120f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail_0.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		Projectile.timeLeft = 300;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.extraUpdates = 3;
		WarpStrength = 8f;
	}

	public override void Behaviors()
	{
		Projectile.velocity *= 0.984f;
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0) * TrailWidth / 7f);
		if (Projectile.timeLeft < 120)
		{
			WarpStrength = Projectile.timeLeft / 120f;
		}
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 0)
		{
			float value0 = index / (float)SmoothedOldPos.Count;
			value0 = MathF.Sin(value0 * MathHelper.Pi);
			var drawC = Color.Lerp(Color.Transparent, Color.White, value0);
			if (Projectile.timeLeft < 120)
			{
				drawC = Color.Lerp(Color.Transparent, drawC, Projectile.timeLeft / 120f);
			}
			return drawC;
		}
		if (style == 1)
		{
			float value0 = 1 - index / (float)SmoothedOldPos.Count;
			var drawC = new Color(value0, value0 * value0 * 0.7f, value0 * value0 * value0 * 0.2f, 0);
			if (Projectile.timeLeft < 120)
			{
				drawC = Color.Lerp(Color.Transparent, drawC, Projectile.timeLeft / 120f);
			}
			return drawC;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 0.1f;
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

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 25; x++)
		{
			var spark = new RayDustDust
			{
				Velocity = new Vector2(0, Main.rand.NextFloat(2, 6f)).RotateRandom(MathHelper.TwoPi),
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				MaxTime = Main.rand.Next(57, 255),
				Scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(8f, 17.0f)),
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0 },
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
