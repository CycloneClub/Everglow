using Terraria.Audio;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Ranged;

public class CyatheaArrow_spike : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.RangedProjectiles;

	private const int MaxDuration = 6 * 60;
	private const int MinDuration = 4 * 60;

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;

		Projectile.arrow = true;
		Projectile.friendly = false;
		Projectile.DamageType = DamageClass.Ranged;

		Projectile.penetrate = 1;

		Projectile.timeLeft = 1200;
	}

	public override void AI()
	{
		if (Projectile.timeLeft == 1198)
		{
			Projectile.friendly = true;
		}
		Projectile.velocity.Y += 0.1f;

		Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

		if (Projectile.velocity.Y > 16f)
		{
			Projectile.velocity.Y = 16f;
		}
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation + MathF.PI, texMain.Size() / 2f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		SoundEngine.PlaySound(SoundID.Dig, Projectile.position);
		Collision.HitTiles(Projectile.position + Projectile.velocity, Projectile.velocity, Projectile.width, Projectile.height);
	}
}
