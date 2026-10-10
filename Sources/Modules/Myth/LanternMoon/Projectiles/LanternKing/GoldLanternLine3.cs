using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.LanternMoon.VFX;

namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

// Move linearly, not curve.
public class GoldLanternLine3 : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override string Texture => ModAsset.GoldLaser_Mod;

	public override void SetCustomDefaults()
	{
		TrailLength = 40;
		TrailColor = new Color(1, 0.35f, 0, 0f);
		TrailWidth = 4f;
		TrailBackgroundDarkness = 0.1f;
		SelfLuminous = true;
		TrailTexture = Commons.ModAsset.Trail.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_black.Value;
		TrailShader = Commons.ModAsset.Trailing.Value;
		Projectile.timeLeft = 300;
		Projectile.hostile = true;
		Projectile.friendly = false;
	}

	public override void Behaviors()
	{
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 1f, 0) * TrailWidth / 7f);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void DrawSelf()
	{
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		float width = 2f;
		if (Projectile.timeLeft < 150)
		{
			width *= Projectile.timeLeft / 150f;
		}
		width *= 1 + (MathF.Sin((float)Main.time * 0.23f + Projectile.whoAmI) + 0.5f) * 0.7f;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.75f, 0, 0), MathHelper.PiOver2, star.Size() / 2f, new Vector2(width / 3.5f, 0.5f), SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.75f, 0, 0), 0, star.Size() / 2f, new Vector2(width / 3.5f, 0.5f), SpriteEffects.None, 0);

		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.75f, 0, 0), MathHelper.PiOver2 + (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 10f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.75f, 0, 0), (float)Main.timeForVisualEffects * 0.04f, star.Size() / 2f, width / 10f, SpriteEffects.None, 0);

		Texture2D spot = Commons.ModAsset.LightPoint2.Value;
		Main.spriteBatch.Draw(spot, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 0.7f, 0), (float)Main.timeForVisualEffects * 0.04f, spot.Size() / 2f, width, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void DestroyEntityEffect()
	{
		for (int x = 0; x < 12; x++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(4f, 8f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new GoldenLineStar()
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = Projectile.Center,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(20, 40),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
	}
}
