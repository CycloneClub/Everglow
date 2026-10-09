using Everglow.Myth.Common;
using Everglow.Myth.TheFirefly.Dusts;
using Terraria.Localization;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlackCorruptRainFriendly : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// DisplayName.SetDefault("Black Corrupt Rain");
	}

	public override void SetDefaults()
	{
		Projectile.width = 10;
		Projectile.height = 10;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = true;
		Projectile.extraUpdates = 3;
		Projectile.timeLeft = 1000;
		Projectile.alpha = 0;
		Projectile.penetrate = 1;
		Projectile.scale = 1f;
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color?(new Color(255, 255, 255, 0));
	}

	private bool initialization = true;
	private double X;
	private float Y;
	private float b;
	private float Stre2 = 1;

	public override void AI()
	{
		if (initialization)
		{
			X = Projectile.velocity.Length();
			b = Main.rand.Next(-50, 50);
			initialization = false;
			if (Main.rand.Next(0, 2) == 1)
			{
				Y = (float)Math.Sin(X / 5d * Math.PI) / 1000f + 1;
			}
			else
			{
				Y = (float)Math.Sin(-X / 5d * Math.PI) / 1000f + 1;
			}
		}
		if (Stre2 > 0.2)
		{
			Stre2 -= 0.005f;
		}

		Projectile.velocity *= 0.995f;
		if (Projectile.timeLeft < 995)
		{
			Vector2 vector = Projectile.Center - new Vector2(4) - Projectile.velocity.SafeNormalize(Vector2.Zero) * 5;
			int index = Dust.NewDust(vector, 0, 0, DustID.SpookyWood, 0f, 0f, 0, default, Projectile.scale * 0.6f * Projectile.velocity.Length());
			Main.dust[index].velocity *= 0.0f;
			Main.dust[index].noGravity = true;
			Main.dust[index].scale *= 1.2f;
			Main.dust[index].alpha = 200;
		}
		if (Projectile.timeLeft is < 600 and >= 585)
		{
			if (Y < 1)
			{
				Projectile.scale *= Y / (Projectile.timeLeft / 585f);
			}
			else
			{
				Projectile.scale *= Y * Projectile.timeLeft / 585f;
			}
		}
		if (Projectile.timeLeft < 580 && Projectile.timeLeft >= 100 + b)
		{
			Projectile.scale *= Y;
		}

		if (Projectile.timeLeft < 100 + b)
		{
			Projectile.scale *= 0.95f;
		}

		Projectile.velocity.Y += 0.001f;
		float kColor = (255 - Projectile.alpha) / 255f;
		Lighting.AddLight(Projectile.Center, 0, kColor * 0.01f, kColor * 0.6f * Projectile.scale);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		int C = (int)(255 * Stre2);
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(C, C, C, 0), Projectile.rotation, new Vector2(56f, 56f), Projectile.scale, SpriteEffects.None, 0);
		return true;
	}

	public override void OnKill(int timeLeft)
	{
		for (int j = 0; j < timeLeft / 24; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * Projectile.scale * 0.3f;
			int dust0 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<BlueGlowAppear>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(0.6f, 1.8f) * Projectile.scale * 0.4f);
			Main.dust[dust0].noGravity = true;
		}
		for (int j = 0; j < timeLeft / 12; j++)
		{
			Vector2 v0 = new Vector2(Main.rand.NextFloat(0, 6f), 0).RotatedByRandom(6.283) * Projectile.scale * 0.3f;
			int dust1 = Dust.NewDust(Projectile.Center + Vector2.Normalize(Projectile.velocity) * 16f - new Vector2(4), 0, 0, ModContent.DustType<BlueParticleDark2>(), v0.X, v0.Y, 100, default, Main.rand.NextFloat(3.7f, 5.1f));
			Main.dust[dust1].alpha = (int)(Main.dust[dust1].scale * 50);
		}
	}
}
