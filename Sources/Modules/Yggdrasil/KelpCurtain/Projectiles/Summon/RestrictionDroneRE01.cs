using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class RestrictionDroneRE01 : ModProjectile
{
	private enum AIState
	{
		Target,
		ShotCooldown,
	}

	// Implementation tuning; the design only specifies a fast tracking laser drone.
	private const float SearchRange = 1000f;
	private const float FlightSpeed = 24f;
	private const int ShotInterval = 30;

	private int TargetIndex
	{
		get => (int)Projectile.ai[(int)AIState.Target] - 1;
		set => Projectile.ai[(int)AIState.Target] = value + 1;
	}

	private float ShotCooldown
	{
		get => Projectile.ai[(int)AIState.ShotCooldown];
		set => Projectile.ai[(int)AIState.ShotCooldown] = value;
	}

	public override string LocalizationCategory => LocalizationUtils.Categories.SummonProjectiles;

	public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.UFOMinion}";

	public override void SetStaticDefaults()
	{
		Main.projFrames[Type] = Main.projFrames[ProjectileID.UFOMinion];
		Main.projPet[Type] = true;
		ProjectileID.Sets.MinionSacrificable[Type] = true;
		ProjectileID.Sets.MinionTargettingFeature[Type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 24;
		Projectile.friendly = true;
		Projectile.minion = true;
		Projectile.minionSlots = 1f;
		Projectile.DamageType = DamageClass.Summon;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.timeLeft = 2;
	}

	public override bool MinionContactDamage() => false;

	public override void AI()
	{
		Player owner = Main.player[Projectile.owner];
		int buffType = ModContent.BuffType<RestrictionDroneRE01Buff>();
		if (!owner.active || owner.dead || !owner.HasBuff(buffType))
		{
			Projectile.Kill();
			return;
		}
		Projectile.timeLeft = 2;

		if (Projectile.owner == Main.myPlayer)
		{
			int previousTarget = TargetIndex;
			if (owner.HasMinionAttackTargetNPC && IsValidTarget(owner.MinionAttackTargetNPC, owner))
			{
				TargetIndex = owner.MinionAttackTargetNPC;
			}
			else if (!IsValidTarget(TargetIndex, owner))
			{
				TargetIndex = -1;
				float nearest = SearchRange * SearchRange;
				foreach (NPC npc in Main.ActiveNPCs)
				{
					float distance = Vector2.DistanceSquared(npc.Center, Projectile.Center);
					if (IsValidTarget(npc.whoAmI, owner) && distance < nearest)
					{
						nearest = distance;
						TargetIndex = npc.whoAmI;
					}
				}
			}
			if (TargetIndex != previousTarget)
			{
				Projectile.netUpdate = true;
			}
		}

		NPC target = IsValidTarget(TargetIndex, owner) ? Main.npc[TargetIndex] : null;
		Vector2 destination = target == null
			? owner.Center + new Vector2(-owner.direction * (48 + 32 * Projectile.minionPos), -70)
			: target.Center + new Vector2(0, -120);
		if (Vector2.DistanceSquared(Projectile.Center, owner.Center) > 2000f * 2000f)
		{
			Projectile.Center = owner.Center;
			Projectile.velocity = Vector2.Zero;
			Projectile.netUpdate = true;
		}
		Vector2 offset = destination - Projectile.Center;
		Vector2 desiredVelocity = offset.SafeNormalize(Vector2.Zero) * Math.Min(FlightSpeed, offset.Length());
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, desiredVelocity, 0.2f);
		Projectile.rotation = Projectile.velocity.X * 0.015f;

		if (ShotCooldown > 0)
		{
			ShotCooldown--;
		}
		if (Projectile.owner == Main.myPlayer && target != null && ShotCooldown <= 0
			&& Collision.CanHitLine(Projectile.position, Projectile.width, Projectile.height, target.position, target.width, target.height))
		{
			Vector2 velocity = (target.Center - Projectile.Center).SafeNormalize(Vector2.UnitY) * 18f;
			Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, velocity,
				ModContent.ProjectileType<RestrictionDroneRE01Laser>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
			ShotCooldown = ShotInterval;
			Projectile.netUpdate = true;
		}
		if (!Main.dedServ && ++Projectile.frameCounter >= 6)
		{
			Projectile.frameCounter = 0;
			Projectile.frame = (Projectile.frame + 1) % Main.projFrames[Type];
		}
	}

	private bool IsValidTarget(int index, Player owner)
	{
		return index >= 0 && index < Main.maxNPCs && Main.npc[index].CanBeChasedBy(this)
			&& Vector2.DistanceSquared(Main.npc[index].Center, owner.Center) <= SearchRange * SearchRange;
	}
}
