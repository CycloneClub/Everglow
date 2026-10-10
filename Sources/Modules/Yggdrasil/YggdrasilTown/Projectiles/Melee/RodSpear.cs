using Terraria.Audio;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class RodSpear : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.MeleeProjectiles;

	private const int NoTarget = -1;

	public override void SetDefaults()
	{
		Projectile.melee = true;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.width = 20;
		Projectile.height = 20;
		Projectile.tileCollide = false;
		Projectile.timeLeft = 600;
		Projectile.aiStyle = -1;
		Projectile.penetrate = -1;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 15;
	}

	private bool HasNotShot { get; set; } = true;

	private int StickTarWho { get; set; } = NoTarget;

	private float RelativeAngle { get; set; } = 0;

	private float HitTargetAngle { get; set; } = 0;

	private float HitTargetScale { get; set; } = 1;

	private Vector2 RelativePos { get; set; } = Vector2.zeroVector;

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		if (Projectile.timeLeft % 4 == 0)
		{
			Projectile.frame++;
		}
		if (Projectile.frame >= 3)
		{
			Projectile.frame = 0;
		}
		if (HasNotShot)
		{
			HasNotShot = false;
			SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
		}
		if (Projectile.wet)
		{
			Projectile.timeLeft -= 2;
		}
		if (Projectile.lavaWet)
		{
			Projectile.timeLeft -= 24;
		}

		if (StickTarWho == NoTarget)
		{
			Projectile.tileCollide = true;

			if (NPCCollision())
			{
				StickNPC(Main.npc[StickTarWho]);
			}
			else if (Collision.SolidCollision(Projectile.Center, 0, 0))
			{
				Projectile.Kill();
			}
			else
			{
				Projectile.velocity.Y += 0.05f;
				Projectile.rotation = (float)(Math.Atan2(Projectile.velocity.Y, Projectile.velocity.X) + Math.PI * 0.25);
			}
		}
		else
		{
			NPC target = Main.npc[StickTarWho];
			if (target != null && target.active)
			{
				Projectile.rotation = target.rotation + RelativeAngle;
				Projectile.Center = target.Center + RelativePos.RotatedBy(target.rotation + RelativeAngle - HitTargetAngle) * target.scale / HitTargetScale;
			}
			else
			{
				Projectile.Kill();
			}
		}
	}

	private bool NPCCollision()
	{
		foreach (NPC npc in Main.npc)
		{
			if (npc.active && !npc.dontTakeDamage && !npc.friendly && !npc.townNPC)
			{
				if (new Rectangle((int)Projectile.Center.X, (int)Projectile.Center.Y, 1, 1).Intersects(npc.Hitbox))
				{
					StickTarWho = npc.whoAmI;
					return true;
				}
			}
		}
		return false;
	}

	private void StickNPC(NPC npc)
	{
		Projectile.damage = (int)(Projectile.damage * 0.1f);
		if (Projectile.damage == 0)
		{
			Projectile.damage = 1;
		}
		Projectile.knockBack = 0;
		Projectile.velocity *= 0;
		RelativeAngle = Projectile.rotation - npc.rotation;
		HitTargetAngle = Projectile.rotation;
		RelativePos = Projectile.Center - npc.Center;
		HitTargetScale = npc.scale;
	}

	public override bool OnTileCollide(Vector2 oldVelocity)
	{
		return true;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texStick = (Texture2D)ModContent.Request<Texture2D>(Texture);
		var frame = new Rectangle(0, Projectile.frame * 52, 52, 52);
		Main.spriteBatch.Draw(
			texStick,
			Projectile.Center - Main.screenPosition,
			frame,
			lightColor,
			Projectile.rotation,
			frame.Size() / 2f,
			Projectile.scale,
			SpriteEffects.None,
			0);
		return false;
	}

	public override void OnKill(int timeLeft)
	{
		if (timeLeft > 60)
		{
			for (int x = 0; x < 16; x++)
			{
				var d = Dust.NewDustDirect(Projectile.position, 40, 40, DustID.Dirt);
				d.velocity *= Projectile.velocity.Length() / 10f;
			}
			SoundEngine.PlaySound(SoundID.Shatter, Projectile.Center);
		}
	}
}
