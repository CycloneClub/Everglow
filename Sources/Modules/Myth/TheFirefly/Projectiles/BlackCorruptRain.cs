using Everglow.Myth.Common;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlackCorruptRain : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Black Corrupt Rain");
	}

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.ignoreWater = true;
		if (Main.masterMode || Main.getGoodWorld)
		{
			Projectile.tileCollide = false;
		}
		else
		{
			Projectile.tileCollide = true;
		}
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = -1;
		Projectile.scale = 1f;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	private bool initialization = true;
	private double x;
	private float y;
	private float b;
	private float stre2 = 1;

	public override void AI()
	{
		if (initialization)
		{
			x = Projectile.velocity.Length();
			b = Main.rand.Next(-50, 50);
			initialization = false;
			if (Main.rand.Next(0, 2) == 1)
			{
				y = (float)Math.Sin(x / 5 * Math.PI) / 1000f + 1;
			}
			else
			{
				y = (float)Math.Sin(-x / 5 * Math.PI) / 1000f + 1;
			}
		}
		if (stre2 > 0.2)
		{
			stre2 -= 0.005f;
		}

		Projectile.velocity *= 0.995f;
		if (Projectile.timeLeft < 995)
		{
			Vector2 vector = Projectile.Center - new Vector2(4) - Projectile.velocity.SafeNormalize(Vector2.Zero) * 5;
			int index = Dust.NewDust(vector, 0, 0, DustID.SpookyWood, 0f, 0f, 0, default, Projectile.scale * 0.8f);
			Main.dust[index].velocity *= 0.0f;
			Main.dust[index].noGravity = true;
			Main.dust[index].scale *= 1.2f;
			Main.dust[index].alpha = 200;
		}
		if (Projectile.timeLeft is < 600 and >= 585)
		{
			if (y < 1)
			{
				Projectile.scale *= y / (Projectile.timeLeft / 585f);
			}
			else
			{
				Projectile.scale *= y * Projectile.timeLeft / 585f;
			}
		}
		if (Projectile.timeLeft < 580 && Projectile.timeLeft >= 100 + b)
		{
			Projectile.scale *= y;
		}

		if (Projectile.timeLeft < 100 + b)
		{
			Projectile.scale *= 0.95f;
		}

		Projectile.velocity.Y += 0.01f;
		Lighting.AddLight(Projectile.Center, (255 - Projectile.alpha) * 0f / 255f * Projectile.scale, (255 - Projectile.alpha) * 0.01f / 255f, (255 - Projectile.alpha) * 0.6f / 255f * Projectile.scale);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color((int)(255 * stre2), (int)(255 * stre2), (int)(255 * stre2), 0), Projectile.rotation, new Vector2(56f, 56f), Projectile.scale, SpriteEffects.None, 0);
		return true;
	}
}
