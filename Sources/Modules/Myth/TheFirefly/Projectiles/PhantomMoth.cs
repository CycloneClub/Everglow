using Everglow.Myth.TheFirefly.Dusts;
using Terraria;

namespace Everglow.Myth.TheFirefly.Projectiles;

public class PhantomMoth : ModProjectile
{
	public override string Texture => "Everglow/Myth/TheFirefly/Projectiles/ButterflyDream";

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 4;
	}

	public override void SetDefaults()
	{
		Projectile.width = 36;
		Projectile.height = 34;
		Projectile.netImportant = true;
		Projectile.friendly = false;
		Projectile.hostile = false;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 300;
		Projectile.usesLocalNPCImmunity = false;
		Projectile.tileCollide = true;
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
		Projectile.spriteDirection = Projectile.velocity.X > 0 ? -1 : 1;
		if (Projectile.timeLeft < 280)
		{
			Projectile.friendly = true;
			Vector2 TargetPos = Vector2.Zero;
			for (int j = 0; j < 200; j++)
			{
				if (Main.npc[j].CanBeChasedBy(Projectile, false) && Collision.CanHit(Projectile.Center, 1, 1, Main.npc[j].Center, 1, 1) && Main.npc[j].active && !Main.npc[j].dontTakeDamage)
				{
					Vector2 v0 = Main.npc[j].Center;
					Vector2 v1 = Projectile.Center;

					if ((v0 - v1).Length() < 600)
					{
						TargetPos = v0;
						break;
					}
				}
			}
			if (TargetPos != Vector2.Zero)
			{
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, Projectile.DirectionTo(TargetPos) * 15, 0.05f);
			}
			else if (Projectile.timeLeft > 240)
			{
				var AimPos = new Vector2(Projectile.ai[0], Projectile.ai[1]);
				Vector2 v0 = (AimPos - Projectile.Center).SafeNormalize(Vector2.Zero);
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, v0 * 15, 0.05f);
			}
		}
		else
		{
			Projectile.velocity *= 0.98f;
		}
		if (Projectile.timeLeft < 10)
		{
			Projectile.scale -= 0.1f;
		}

		if (Projectile.timeLeft == 300)
		{
			Projectile.frame = Main.rand.Next(3);
		}

		if (Projectile.frame > 3)
		{
			Projectile.frame = 0;
		}

		if (Projectile.timeLeft % 6 == 0)
		{
			Projectile.frame++;
		}

		if (Projectile.timeLeft % 3 == 0)
		{
			int index = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<BlueGlowAppear>(), 0f, 0f, 100, default, Main.rand.NextFloat(0.7f, 1.9f) * Projectile.timeLeft / 300f);
			Main.dust[index].velocity = Projectile.velocity * 0.5f;
		}
		int index2 = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, ModContent.DustType<BlueParticleDark2>(), 0f, 0f, 0, default, Main.rand.NextFloat(3.7f, 5.1f));
		Main.dust[index2].velocity = Projectile.velocity * 0.5f;
		Main.dust[index2].alpha = (int)(Main.dust[index2].scale * 50) + 300 - Projectile.timeLeft;
	}

	public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft > 0)
		{
			float k = Math.Min(timeLeft / 200f, 1f);
			for (int i = 0; i < 18; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Clentaminator_Blue, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f, 0, default, 0.6f * k);
			}
			for (int i = 0; i < 6; i++)
			{
				int index = Dust.NewDust(Projectile.position - new Vector2(8), Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 100, Color.Blue, Main.rand.NextFloat(0.7f, 1.2f) * k);
				Main.dust[index].velocity = new Vector2(0, Main.rand.NextFloat(5f, 10f)).RotatedByRandom(6.283) * k;
				Main.dust[index].noGravity = true;
			}
		}
	}

	public override Color? GetAlpha(Color lightColor)
	{
		return new Color(0.6f, 0.6f, 0.9f, 0) * ((1 - Projectile.alpha / 255f) * Projectile.timeLeft / 300f);
	}
}
