using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Myth.LanternMoon.Projectiles.Weapons;

public class LanternZoneBullet : ModProjectile
{
	public Vector2 TargetPosition;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 3600;
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = false;
		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		TargetPosition = Main.MouseWorld;
	}

	public override void AI()
	{
		var toTarget = TargetPosition - Projectile.Center;
		var toTargetNext = toTarget - Projectile.velocity;
		if (toTargetNext.Length() > toTarget.Length())
		{
			Projectile.Kill();
		}
		Projectile.rotation = Projectile.velocity.ToRotationSafe();
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0f, 0.1f));
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(new SoundStyle(ModAsset.gildingrevolver_release_aperture_Mod), Projectile.Center);
		Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<LanternZone>(), 0, 0, Projectile.owner);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D black = ModAsset.LanternZoneBullet_black.Value;
		Texture2D bullet = ModContent.Request<Texture2D>(Texture).Value;
		Texture2D bloom = ModAsset.LanternZoneBullet_bloom.Value;

		Main.EntitySpriteDraw(black, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, Projectile.rotation, black.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bullet, Projectile.Center - Main.screenPosition, null, Color.White * 0.75f, Projectile.rotation, bullet.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Main.EntitySpriteDraw(bloom, Projectile.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), Projectile.rotation, bloom.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return false;
	}
}
