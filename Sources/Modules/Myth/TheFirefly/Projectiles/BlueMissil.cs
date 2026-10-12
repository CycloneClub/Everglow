using Everglow.Myth.Common;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class BlueMissil : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.BossProjectiles;

	public override void SetStaticDefaults()
	{
		// base.DisplayName.SetDefault("蓝鳞粉");
		Main.projFrames[Projectile.type] = 3;
	}

	public override void SetDefaults()
	{
		Projectile.width = 34;
		Projectile.height = 34;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 120;
		Projectile.tileCollide = true;
		Projectile.usesLocalNPCImmunity = false;
	}

	private Vector2 va;
	private float stre2 = 1;

	public override void AI()
	{
		if (stre2 > 0)
		{
			stre2 -= 0.01f;
		}

		if (Projectile.ai[0] != 2)
		{
			if (va == Vector2.Zero)
			{
				va = Projectile.velocity;
			}

			if (Projectile.timeLeft < 90)
			{
				Projectile.velocity = va;
			}
			else
			{
				Projectile.velocity *= 0.94f;
			}
		}
		int num90 = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<Dusts.BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.7f, 3.9f));
		Main.dust[num90].velocity = Projectile.velocity * 0.8f;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 18; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.6f);
		}
		for (int i = 0; i < 6; i++)
		{
			int num90 = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f));
			Main.dust[num90].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
			Main.dust[num90].noGravity = true;
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D Light = ModAsset.FixCoinLight3.Value;
		Main.spriteBatch.Draw(Light, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color((int)(255 * stre2), (int)(255 * stre2), (int)(255 * stre2), 0), Projectile.rotation, new Vector2(56f, 56f), Projectile.scale * 2, SpriteEffects.None, 0);
		Texture2D Star = ModAsset.BlueMissil.Value;
		Main.spriteBatch.Draw(Star, Projectile.Center - Main.screenPosition + new Vector2(0f, Projectile.gfxOffY), null, new Color(255, 255, 255, 0), 0, new Vector2(17f, 17f), Projectile.scale * 2, SpriteEffects.None, 0);
		return true;
	}
}
