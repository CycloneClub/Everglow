using Terraria.Localization;

namespace Everglow.Myth.Misc.Projectiles.Typeless;

public class PurpleBallEffect : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("PurpleBallEffect");
	}

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.timeLeft = 240;
		Projectile.extraUpdates = 7;
		Projectile.tileCollide = false;
		Projectile.scale = 5;
	}

	public override void AI()
	{
		Projectile.velocity *= 0;
		v0 = Projectile.Center;
		if (Projectile.timeLeft >= 180)
		{
			pro = (240 - Projectile.timeLeft) * (240 - Projectile.timeLeft) / 12;
		}
		else
		{
			pro = 300;
		}
		if (Projectile.timeLeft >= 150)
		{
			scale = 1;
		}
		else
		{
			float k0 = Projectile.timeLeft / 150f;
			scale = k0 * k0 * k0 * k0;
		}
		aI0 = Projectile.ai[0];
	}

	private Vector2 v0;
	private float scale = 1;
	private int pro = 0;
	private float aI0 = 0;

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		float Col = 0;
		if (Projectile.timeLeft > 180)
		{
			float f = 1.2f - (Projectile.timeLeft - 180) / 60f;
			Col = f * f * f;
		}
		else
		{
			float f = 1.2f - (180 - Projectile.timeLeft) / 190f;
			Col = f * f * f;
		}
		Texture2D tex2 = ModContent.Request<Texture2D>("Everglow/Myth/Acytaea/Dusts/CosmicCrack3").Value;
		for (float r = 0; r < Col + 0.1; r += 0.1f)
		{
			Main.spriteBatch.Draw(tex2, v0 - Main.screenPosition, new Rectangle(0, 0, pro, 50), new Color(0.1f, 0.1f, 0.1f, 0), aI0, tex2.Size() / 2f, scale, SpriteEffects.None, 0);
			Main.spriteBatch.Draw(tex2, v0 - Main.screenPosition, new Rectangle(0, 0, 300, 50), new Color(0.1f, 0.1f, 0.1f, 0), aI0 + 1.57f, tex2.Size() / 2f, scale / 3.3f, SpriteEffects.None, 0);
		}
		return true;
	}
}
