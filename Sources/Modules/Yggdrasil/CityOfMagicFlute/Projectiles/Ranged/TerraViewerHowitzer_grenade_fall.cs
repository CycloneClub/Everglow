using Everglow.Commons.Templates.Weapons;

namespace Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;

public class TerraViewerHowitzer_grenade_fall : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	public override void SetCustomDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 2400;
		Projectile.alpha = 0;
		Projectile.penetrate = 30;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = 10;
		Projectile.extraUpdates = 2;
		TrailLength = 20;
		TrailTexture = Commons.ModAsset.Trail_12.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_4_black.Value;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 14400;
	}

	public override void Behaviors()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Projectile.timeLeft == 2310)
		{
			Projectile.friendly = true;
		}
		if (Projectile.timeLeft < 2380)
		{
			Projectile.tileCollide = true;
		}
		Projectile.hide = true;
	}

	public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI)
	{
		behindNPCsAndTiles.Add(index);
	}

	public override void DestroyEntityEffect()
	{
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TerraViewerHowitzer_grenade_fall_explosion>(), Projectile.damage / 3, Projectile.knockBack * 0.4f, Projectile.owner, MathF.Sqrt(Projectile.ai[0]) * 3);
	}

	public override void DrawTrail()
	{
		base.DrawTrail();
	}

	public override bool PreDraw(ref Color lightColor)
	{
		return base.PreDraw(ref lightColor);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0)
	{
		if (style == 1)
		{
			float k1 = 20f;
			float colorValue0 = (2400 - Projectile.timeLeft) / k1;

			if (Projectile.timeLeft <= 2400 - k1)
			{
				colorValue0 = 1;
			}

			int trueLength = 0;
			for (int i = 1; i < Projectile.oldPos.Length; ++i)
			{
				if (Projectile.oldPos[i] == Vector2.Zero)
				{
					break;
				}

				trueLength++;
			}

			float velocityValue = 1f;
			colorValue0 *= velocityValue;
			var c0 = new Color(colorValue0, colorValue0 * colorValue0 * 0.6f, colorValue0 * colorValue0 * 0.1f, 0);
			return c0;
		}
		return base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);
	}

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		factor *= 4;
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

	public override void DrawSelf()
	{
		Texture2D star = ModAsset.TerraViewerHowitzer_grenade_fall.Value;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, TrailColor, Projectile.rotation, star.Size() / 2f, 1f, SpriteEffects.None, 0);
	}
}
