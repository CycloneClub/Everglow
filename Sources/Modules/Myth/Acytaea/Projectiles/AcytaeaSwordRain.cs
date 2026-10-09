using static System.Net.Mime.MediaTypeNames;

namespace Everglow.Myth.Acytaea.Projectiles;

public class AcytaeaSwordRain : ModProjectile
{
	public override string Texture => "Everglow/Myth/Acytaea/Projectiles/AcytaeaSword_projectile";

	public override void SetDefaults()
	{
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 300;
		Projectile.extraUpdates = 0;
		Projectile.scale = 1f;
		Projectile.hostile = true;
		Projectile.friendly = false;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.penetrate = -1;
		Projectile.DamageType = DamageClass.Melee;

		Projectile.width = 80;
		Projectile.height = 80;
	}

	public override void AI()
	{
		if (Projectile.timeLeft % 4 == 1 && Projectile.timeLeft > 120)
		{
			Projectile p0 = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center - new Vector2(Main.rand.NextFloat(-100f, 100f), 2000), new Vector2(0, 40), ModContent.ProjectileType<AcytaeaFlySword_2>(), Projectile.damage * 2 / 3, 4);
			p0.timeLeft = 600;
		}
		if (Projectile.timeLeft < 15)
		{
			Projectile.scale *= 0.7f;
		}
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		AmmoHit();
		Projectile.tileCollide = false;
		return false;
	}

	public void AmmoHit()
	{
	}

	public override bool PreDraw(ref Color lightColor)
	{
		float timeValue = (float)Main.time * 0.2f;
		float sizeH = 1f + 0.3f * MathF.Cos(timeValue);
		float sizeV = 1f + 0.1f * MathF.Sin(timeValue * 0.2f);
		Texture2D texBlack = Commons.ModAsset.StarSlash_black.Value;
		Main.spriteBatch.Draw(texBlack, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, 0, texBlack.Size() * 0.5f, new Vector2(sizeH, 0.5f) * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(texBlack, Projectile.Center - Main.screenPosition, null, Color.White * 0.5f, MathHelper.PiOver2, texBlack.Size() * 0.5f, new Vector2(sizeH, 1.5f * sizeV) * Projectile.scale, SpriteEffects.None, 0);
		Texture2D tex = Commons.ModAsset.StarSlash.Value;
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 0, 0.4f, 0), 0, tex.Size() * 0.5f, new Vector2(sizeH, 0.5f) * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 0, 0.4f, 0), MathHelper.PiOver2, tex.Size() * 0.5f, new Vector2(sizeH, 1.5f * sizeV) * Projectile.scale, SpriteEffects.None, 0);

		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.2f, 0.4f, 0), MathHelper.PiOver4 * 3 + timeValue * 0.2f, tex.Size() * 0.5f, 0.6f * Projectile.scale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(tex, Projectile.Center - Main.screenPosition, null, new Color(1f, 0.2f, 0.4f, 0), MathHelper.PiOver4 + timeValue * 0.2f, tex.Size() * 0.5f, 0.6f * Projectile.scale, SpriteEffects.None, 0);

		float deltaX = (300 - Projectile.timeLeft) / 14f;
		float scaleW = 1.9f - deltaX;
		if (scaleW > 0)
		{
			deltaX = MathF.Sin(deltaX);
			Main.spriteBatch.Draw(tex, Projectile.Center + new Vector2(deltaX * 150, 0) - Main.screenPosition, null, new Color(1f, 0, 0.4f, 0), 0, tex.Size() * 0.5f, new Vector2(scaleW, 5f) * Projectile.scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(tex, Projectile.Center - new Vector2(deltaX * 150, 0) - Main.screenPosition, null, new Color(1f, 0, 0.4f, 0), 0, tex.Size() * 0.5f, new Vector2(scaleW, 5f) * Projectile.scale, SpriteEffects.None, 0);
		}
		return false;
	}
}
