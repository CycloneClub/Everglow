using Everglow.Commons.DataStructures;
using Everglow.Commons.Templates.Weapons;
using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.CityOfMagicFlute.Projectiles.Ranged;

public class TerraViewerHowitzer_shoot : TrailingProjectile
{
	public override void SetCustomDefaults()
	{
		Projectile.width = 30;
		Projectile.height = 30;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = false;
		Projectile.tileCollide = true;
		Projectile.timeLeft = 2400;
		Projectile.alpha = 0;
		Projectile.penetrate = 30;
		Projectile.scale = 1f;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.penetrate = 10;
		Projectile.extraUpdates = 2;
		ProjectileID.Sets.DrawScreenCheckFluff[Projectile.type] = 14400;

		TrailLength = 30;
		TrailTexture = Commons.ModAsset.Trail_4.Value;
		TrailTextureBlack = Commons.ModAsset.Trail_4_black.Value;
		TrailBackgroundDarkness = 0.3f;
		TrailColor = new Color(0.5f, 0.9f, 0.9f, 0);
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
		Projectile.usesOwnerLight = true;
	}

	public override void Behaviors()
	{
		Projectile.rotation = MathF.Atan2(Projectile.velocity.Y, Projectile.velocity.X);
		if (Timer == 90)
		{
			Projectile.friendly = true;
		}
	}



	public override void DestroyEntityEffect()
	{
		if (TimeAfterEntityDestroy < 0)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<TerraViewerHowitzer_shoot_explosion>(), Projectile.damage, Projectile.knockBack * 0.4f, Projectile.owner, MathF.Sqrt(Projectile.ai[0]) * 3);
		}
		ShakerManager.AddShaker(Projectile.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 60, 12f, 200, 0.9f, 0.8f, 600);
		SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.Center);
	}

	public override Color GetTrailColor(int style, Vector2 worldPos, int index, ref float factor, float extraValue0 = 0, float extraValue1 = 0) => base.GetTrailColor(style, worldPos, index, ref factor, extraValue0, extraValue1);

	public override Vector3 ModifyTrailTextureCoordinate(float factor, float timeValue, float phase, float widthValue) => base.ModifyTrailTextureCoordinate(factor, timeValue, phase, widthValue);

	public override void DrawSelf()
	{
		Texture2D star = ModAsset.TerraViewerHowitzer_shoot.Value;
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition - Projectile.velocity, null, Lighting.GetColor(Projectile.Center.ToTileCoordinates()), Projectile.rotation, star.Size() / 2f, 1f, SpriteEffects.None, 0);
	}
}
