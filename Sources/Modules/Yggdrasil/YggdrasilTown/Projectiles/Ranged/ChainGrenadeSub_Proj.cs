using Terraria.Audio;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Ranged;

public class ChainGrenadeSub_Proj : ModProjectile
{
	public override string LocalizationCategory => LocalizationUtils.Categories.RangedProjectiles;

	public override void SetDefaults()
	{
		Projectile.timeLeft = 360;
		Projectile.aiStyle = -1;
		Projectile.friendly = false;
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
		if (Timer > 5)
		{
			Projectile.friendly = true;
		}
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
			SoundEngine.PlaySound(SoundID.DD2_GoblinBomb.WithVolume(0.5f), Projectile.Center);
			Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<ChainGrenadeSub_ProjExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
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
