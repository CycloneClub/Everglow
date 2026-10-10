using Everglow.Commons.DataStructures;

namespace Everglow.Myth.Acytaea.Projectiles;

public class Acytaea_ShineStar_Town : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 60;
		Projectile.extraUpdates = 4;
		Projectile.scale = 1f;
		Projectile.hostile = false;
		Projectile.friendly = true;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 40;
		Projectile.height = 40;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		return false;
	}

	public override void PostDraw(Player player, Color lightColor)
	{
		float value = Projectile.timeLeft / 60f;
		Vector2 newScale = new Vector2(1f, value * 2) * MathF.Sin(value * MathF.PI) * 1f;
		Texture2D star = Commons.ModAsset.StarSlash.Value;
		Texture2D dark = Commons.ModAsset.Point_black.Value;
		Main.spriteBatch.Draw(dark, Projectile.Center - Main.screenPosition, null, Color.White * value, 0, dark.Size() / 2f, 1f, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 0, 0, 0), Projectile.timeLeft * 0.02f, star.Size() / 2f, newScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(star, Projectile.Center - Main.screenPosition, null, new Color(255, 0, 0, 0), Projectile.timeLeft * 0.02f + MathHelper.PiOver2, star.Size() / 2f, newScale, SpriteEffects.None, 0);
	}
}
