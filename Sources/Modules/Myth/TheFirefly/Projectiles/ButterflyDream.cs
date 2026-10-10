using Everglow.Myth.TheFirefly.Dusts;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class ButterflyDream : ModProjectile
{
	public override void SetStaticDefaults()
	{
		// base.DisplayName.SetDefault("蓝蝶幻梦");
		Main.projFrames[Projectile.type] = 4;
		// ProjectileID.Sets.TrailCacheLength[Projectile.type] = 10;
		// ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
	}

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 34;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 600;
		Projectile.tileCollide = false;
		Projectile.usesLocalNPCImmunity = false;
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(Projectile.timeLeft);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		Projectile.timeLeft = reader.ReadInt32();
	}

	public override void AI()
	{
		if (Projectile.ai[1] == 0)
		{
			Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
			Projectile.velocity += new Vector2(0, 0.2f * Projectile.ai[0]);
			if (Projectile.timeLeft == 600)
			{
				Projectile.frame = Main.rand.Next(4);
			}

			Projectile.velocity.Y *= 0.98f;
			if (Projectile.timeLeft % 3 == 0)
			{
				int num90 = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.7f, 1.9f));
				Main.dust[num90].velocity = Projectile.velocity * 0.5f;
			}
		}

		if (Projectile.ai[1] == 1)// 限制圈，ai0：npc
		{
			if (Projectile.timeLeft == 800)
			{
				Projectile.alpha = 200;
			}

			if (Projectile.timeLeft > 740)
			{
				Projectile.alpha -= 4;
				Projectile.hostile = false;

				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Vector2.Normalize(Projectile.velocity) * 1200, 0.1f);
			}
			else
			{
				Projectile.alpha = 0;
				Projectile.hostile = true;
			}

			NPC npc = Main.npc[(int)Projectile.ai[0]];
			// Projectile.Center -= Projectile.velocity;
			float sin = (float)Math.Sin(Projectile.timeLeft * 0.06f);
			Projectile.velocity = Projectile.velocity.RotatedBy(-0.01f);
			Projectile.velocity = Vector2.Normalize(Projectile.velocity) * (Projectile.velocity.Length() + sin * 6 - 0.7f);

			Projectile.Center = npc.Center + Projectile.velocity;
			if (Projectile.timeLeft < 740 && Projectile.timeLeft % 2 == 0)
			{
				int num90 = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<BlueGlow>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.4f, 0.9f));
				Main.dust[num90].velocity = Vector2.Normalize(Projectile.velocity.RotatedBy(1.57f)) * 2;
			}
		}

		if (Projectile.frame > 3)
		{
			Projectile.frame = 0;
		}

		if (Projectile.timeLeft % 10 == 0)
		{
			Projectile.frame++;
		}
	}

	public override bool ShouldUpdatePosition()
	{
		if (Projectile.ai[1] == 1)
		{
			return false;
		}

		return true;
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
			int index = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f));
			Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283);
			Main.dust[index].noGravity = true;
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(1f, 1f, 1f, 0) * (1 - Projectile.alpha / 255f);
	}
}
