using static Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon.WitherbarkMinion;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class WitherbarkMinion_Leaf : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	private const float OrbitRadius = 80f;
	public const float GeneralSpeed = 6f;
	private const float ChaseSpeed = 12f;

	private int OwnerMinion => (int)Projectile.ai[0];

	private int TargetWhoAmI
	{
		get => Projectile.ai[1] == -1 ? -1 : (int)Projectile.ai[1];
		set => Projectile.ai[1] = value;
	}

	/// <summary>
	/// Indicates whether the projectile is in orbit around the owner minion or not.
	/// </summary>
	private bool HasFixed
	{
		get => Projectile.localAI[0] == 1;
		set
		{
			Projectile.localAI[0] = value ? 1 : 0;
		}
	}

	/// <summary>
	/// Indicates whether the projectile has proc'd a target or not.
	/// </summary>
	private bool HasProc
	{
		get => Projectile.localAI[1] == 1;
		set
		{
			Projectile.localAI[1] = value ? 1 : 0;
		}
	}

	/// <summary>
	/// Has different meaning in different states (<see cref="HasProc"/>):
	/// <para/>1. In proc state, it is used to count the time since proc.
	/// <para/>2. In non-proc state, it is used to record the previous rotation angle of the projectile.
	/// </summary>
	private ref float Timer => ref Projectile.localAI[2];

	private Vector2 TargetLatesetPosition { get; set; }

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 5;
	}

	public override void SetDefaults()
	{
		Projectile.width = 22;
		Projectile.height = 14;

		Projectile.DamageType = DamageClass.Summon;
		Projectile.friendly = true;
		Projectile.ignoreWater = true;
		Projectile.tileCollide = false;
		Projectile.penetrate = -1;
	}

	public override bool? CanDamage() => HasProc;

	public override void AI()
	{
		if (Main.timeForVisualEffects % 5 == 0)
		{
			Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
		}

		if (HasProc)
		{
			var target = Main.npc[TargetWhoAmI];
			if (ProjectileUtils.MinionCheckTargetActive(TargetWhoAmI))
			{
				TargetLatesetPosition = target.Center;
			}
			else
			{
				if (Projectile.velocity == Vector2.Zero)
				{
					Projectile.Kill();
				}

				Projectile.rotation = Projectile.velocity.ToRotation();
				return; // If target is unavailable during lifetime, preserve motion state until killed.
			}

			var movementToTarget = TargetLatesetPosition - Projectile.Center;
			if (Timer++ < 15) // Rotate to target
			{
				Projectile.rotation = MathHelper.Lerp(Projectile.rotation, movementToTarget.ToRotation(), 0.2f);
			}
			else // Chase target
			{
				Projectile.velocity = MathUtils.Lerp(0.2f, Projectile.velocity, movementToTarget.NormalizeSafe() * ChaseSpeed);
				Projectile.rotation = Projectile.velocity.ToRotation();
			}
		}
		else
		{
			Projectile ownerMinion = Main.projectile[OwnerMinion];
			if (ownerMinion.active)
			{
				var movementFromMinion = Projectile.Center - ownerMinion.Center;
				if (HasFixed) // Orbit around owner minion.
				{
					Timer += MathHelper.TwoPi / (ProjectileAttack_ShootGap * ProjectileAttack_ProjectileCount);
					Projectile.Center = ownerMinion.Center + Vector2.UnitX.RotatedBy(Timer) * OrbitRadius;
					Projectile.rotation = Timer;
				}
				else // Try moving to the motion orbit from create position.
				{
					if (movementFromMinion.Length() > 80)
					{
						Projectile.velocity = Vector2.Zero;
						HasFixed = true;
						Timer = movementFromMinion.ToRotation();
					}

					Projectile.velocity = movementFromMinion.NormalizeSafe() * GeneralSpeed;
					Projectile.rotation = Projectile.velocity.ToRotation();
				}
			}
			else
			{
				Projectile.Kill();
			}
		}
	}

	public void Proc(NPC target)
	{
		if (HasProc)
		{
			return;
		}

		if (target is null)
		{
			TargetWhoAmI = ProjectileUtils.FindTarget(Projectile.Center, 300);
			if (TargetWhoAmI < 0)
			{
				Projectile.Kill();
				return;
			}

			target = Main.npc[TargetWhoAmI];
		}
		else
		{
			TargetWhoAmI = target.whoAmI;
		}

		HasProc = true;
		TargetLatesetPosition = target.Center;
		Timer = 0;
		Projectile.tileCollide = true;
		Projectile.penetrate = 1;
		Projectile.timeLeft = 300;

		// Set an initial speed on projectile's direction
		Projectile.velocity = Vector2.UnitX.RotatedBy(Projectile.rotation) * GeneralSpeed;

		Projectile.netUpdate = true;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);
		var rotation = Projectile.rotation;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, Color.White, rotation, frame.Size() / 2, 1f, SpriteEffects.None, 0);

		return false;
	}

	public override void OnKill(int timeLeft)
	{
		for (int i = 0; i < 20; i++)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GrassBlades);
		}
	}
}
