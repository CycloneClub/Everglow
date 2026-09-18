using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class ChainGrenadeProj : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 360;
		Projectile.aiStyle = -1;
		Projectile.friendly = true;
		Projectile.penetrate = 1;
		Projectile.tileCollide = true;
		Projectile.ignoreWater = true;
		Projectile.DamageType = DamageClass.Ranged;
		Projectile.width = 12;
		Projectile.height = 12;
		ProjectileID.Sets.PlayerHurtDamageIgnoresDifficultyScaling[Type] = true;
		Timer = 0;
	}

	public float Omega = 0;

	public int Timer = 0;

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		if (Projectile.velocity.Length() < 3)
		{
			Projectile.velocity *= 0;
			if (Math.Abs(Projectile.rotation % MathHelper.PiOver2 - MathHelper.PiOver4) > 0.05)
			{
				Projectile.rotation = Main.rand.Next(4) * MathHelper.PiOver2 + MathHelper.PiOver4;
			}
			Omega = 0;
		}
		else
		{
			if (Projectile.velocity.X != oldVelocity.X)
			{
				Projectile.velocity.X = -oldVelocity.X;
			}

			if (Projectile.velocity.Y != oldVelocity.Y)
			{
				Projectile.velocity.Y = -oldVelocity.Y;
			}

			Projectile.velocity *= 0.6f;
			Omega = Main.rand.NextFloat(-0.3f, .3f) * Projectile.velocity.Length() / 12f;
		}
		return false;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Omega = Main.rand.NextFloat(-0.3f, .3f);
	}

	public override void AI()
	{
		Timer++;
		if (!Collision.SolidCollision(Projectile.position, Projectile.width, Projectile.height))
		{
			Projectile.velocity.Y += 0.25f;
		}
		else
		{
		}
		Projectile.rotation += Omega;
		if (Projectile.velocity.Length() < 0.3f)
		{
			foreach (NPC npc in Main.npc)
			{
				if (npc != null && npc.active && !npc.friendly && !npc.dontTakeDamage)
				{
					if ((npc.Center - Projectile.Center).Length() < 80)
					{
						Projectile.Kill();
					}
				}
			}
		}
	}

	public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
	{
		modifiers._combatTextHidden = true;
		modifiers.FinalDamage *= 0;
		base.ModifyHitNPC(target, ref modifiers);
	}

	public override void OnKill(int timeLeft)
	{
		if (Main.myPlayer == Projectile.owner)
		{
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<ChainGrenadeProjExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			for (int i = 0; i < 3; i++)
			{
				Projectile proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, new Vector2(0, Main.rand.NextFloat(4, 12)).RotatedByRandom(MathHelper.TwoPi), ModContent.ProjectileType<ChainGrenadeSub_Proj>(), (int)(Projectile.damage * 0.6f), Projectile.knockBack, Projectile.owner);
				proj.timeLeft = Main.rand.Next(14, 30);
			}
			SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
		}
		base.OnKill(timeLeft);
	}

	public override bool PreDraw(ref Color lightColor)
	{
		Texture2D tex = (Texture2D)ModContent.Request<Texture2D>(Texture);
		Main.EntitySpriteDraw(tex, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, tex.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
		return false;
	}
}
