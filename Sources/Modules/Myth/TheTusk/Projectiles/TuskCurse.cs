using Everglow.Commons.Templates.Weapons;
using Everglow.Myth.TheTusk.Projectiles.Weapon;
using Terraria.Audio;

namespace Everglow.Myth.TheTusk.Projectiles;

public class TuskCurse : TrailingProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetCustomDefaults()
	{
		TrailColor = new Color(1, 0, 0, 0f);
		TrailTexture = Commons.ModAsset.Trail_2_thick.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_2_black_thick.Value;
		base.SetCustomDefaults();
	}

	public override void Behaviors()
	{
		Projectile.rotation = (float)Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		Projectile.velocity *= 0.98f;
		Projectile.velocity.Y += 0.4f;
	}

	public override void DestroyEntityEffect()
	{
		SoundEngine.PlaySound(SoundID.DD2_SkeletonHurt, Projectile.Center);
		for (int h = 0; h < 20; h++)
		{
			Vector2 v3 = new Vector2(0, (float)Math.Sin(h * Math.PI / 4d + Projectile.ai[0]) + 5).RotatedBy(h * Math.PI / 10d) * Main.rand.NextFloat(0.2f, 1.1f);
			Dust dust = Dust.NewDustDirect(new Vector2(Projectile.Center.X, Projectile.Center.Y) - new Vector2(4, 4), 0, 0, DustID.VampireHeal, 0, 0, 0, default, 15f * Main.rand.NextFloat(0.4f, 1.1f));
			dust.noGravity = true;
			dust.velocity = v3;
		}
		Projectile.NewProjectile(Terraria.Entity.InheritSource(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<TuskBloodPool>(), Projectile.damage, Projectile.knockBack, Projectile.owner, 0f, 0f);
	}

	public override void DrawSelf()
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Color lightColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition - Projectile.velocity, null, lightColor, Projectile.rotation, texMain.Size() / 2f, 1f, SpriteEffects.None, 0);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue)
	{
		float x = factor - timeValue * 1f;
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
