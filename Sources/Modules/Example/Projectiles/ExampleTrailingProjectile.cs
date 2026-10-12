using Everglow.Commons.Templates.Weapons;
using Everglow.Commons.Utilities;

namespace Everglow.Example.Projectiles;

public class ExampleTrailingProjectile : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MiscsProjectiles;

	public override void SetCustomDefaults()
	{
		TrailTexture = Commons.ModAsset.Trail_8.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_8_black.Value;
		Projectile.tileCollide = false;
		TrailLength = 30;
		TrailBackgroundDarkness = 1;
	}

	public override void Behaviors()
	{
		Vector2 offsetTarget = new Vector2(0, 150).RotatedBy(Main.time * 0.24f);
		offsetTarget.Y *= 0.4f;
		offsetTarget = offsetTarget.RotatedBy(-Main.time * 0.082f);
		Vector2 targetPos = Main.MouseWorld + offsetTarget;
		Vector2 toTarget = targetPos - Projectile.Center - Projectile.velocity;
		toTarget = toTarget.NormalizeSafe() * 45f;
		Projectile.velocity = Projectile.velocity * 0.85f + toTarget * 0.15f;
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		DestroyEntity();
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		DestroyEntity();
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		DestroyEntity();
		Projectile.tileCollide = false;
		return false;
	}

	public override void DestroyEntity()
	{
		base.DestroyEntity();
	}

	public override void DestroyEntityEffect()
	{
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override void DrawSelf()
	{
		base.DrawSelf();
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		factor *= 5;
		if (style == 1)
		{
			return GetColorFromWavelength(580 + 200 * Math.Sin(worldPos.X * 0.005f + worldPos.Y * 0.005f));
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public Color GetColorFromWavelength(double wavelength)
	{
		double r = 0, g = 0, b = 0;

		if (wavelength >= 380 && wavelength < 440)
		{
			r = -(wavelength - 440) / (440 - 380);
			g = 0;
			b = 1;
		}
		else if (wavelength >= 440 && wavelength < 490)
		{
			r = 0;
			g = (wavelength - 440) / (490 - 440);
			b = 1;
		}
		else if (wavelength >= 490 && wavelength < 510)
		{
			r = 0;
			g = 1;
			b = -(wavelength - 510) / (510 - 490);
		}
		else if (wavelength >= 510 && wavelength < 580)
		{
			r = (wavelength - 510) / (580 - 510);
			g = 1;
			b = 0;
		}
		else if (wavelength >= 580 && wavelength < 645)
		{
			r = 1;
			g = -(wavelength - 645) / (645 - 580);
			b = 0;
		}
		else if (wavelength >= 645 && wavelength <= 780)
		{
			r = 1;
			g = 0;
			b = 0;
		}
		else
		{
			r = 0;
			g = 0;
			b = 0;
		}
		return new Color((float)r, (float)g, (float)b, 0);
	}
}
