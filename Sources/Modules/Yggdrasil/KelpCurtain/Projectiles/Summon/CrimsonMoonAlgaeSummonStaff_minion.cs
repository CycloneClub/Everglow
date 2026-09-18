using Everglow.Yggdrasil.KelpCurtain.Buffs;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class CrimsonMoonAlgaeSummonStaff_minion : ModProjectile
{
	// Minion data
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	// Minion data
	public const float Minion_Knockback = 1f;

	/// <summary>
	/// The distance to teleport back to the owner if the minion is too far away.
	/// </summary>
	private const int TeleportDistance = 1000;

	/// <summary>
	/// The distance to search for target npc.
	/// </summary>
	private const int SearchDistance = 800;

	public int ProjectileAttackCooling = 0;

	public int ProjectileAttackCoolingMax = 60;

	public int ProjectileAttackCount = 6;

	public int BlockedTimer = 0;

	public int BlockedTimeMax = 120;

	public enum AttackState
	{
		None,
		SelfKillingAttack,
		ProjectileAttack,
	}

	private Player Owner => Main.player[Projectile.owner];

	/// <summary>
	/// A timer that is used to control the minion's attack state and cooldown. Has different meanings in different states.
	/// </summary>
	private ref float Timer => ref Projectile.localAI[0];

	/// <summary>
	/// The whoAmI of the target npc that the minion is currently attacking.
	/// <para/>Defaults to -1, and it will be reset to -1 if the target npc is not found or is inactive.
	/// </summary>
	private int TargetWhoAmI { get; set; } = -1;

	public AttackState MinionAttackState { get; private set; } = AttackState.None;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 8;
	}

	public override void SetDefaults()
	{
		Projectile.width = 20;
		Projectile.height = 42;

		Projectile.DamageType = DamageClass.Summon;
		Projectile.netImportant = true;

		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.minionSlots = 1;
		Projectile.minion = true;

		Projectile.timeLeft = 2;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
	}

	/// <summary>
	/// Checks if the minion can deal damage to the target npc.
	/// <para/>Can only do contact damage when the minion is in a normal attack state and charging or decelerating.
	/// </summary>
	/// <returns></returns>
	public override bool? CanDamage() => false;

	public override void AI()
	{
		UpdtateLifeCycle();
		Lighting.AddLight(Projectile.Center, new Vector3(1f, 0.9f, 0.8f));

		// If the minion is too far from the player, reset state and teleport back to owner.
		if (Vector2.Distance(Projectile.Center, Owner.Center) > TeleportDistance)
		{
			ResetTarget();
			Projectile.Center = Owner.Center; // Teleport back to owner
		}

		// Check if the target npc is active.
		if (!ProjectileUtils.MinionCheckTargetActive(TargetWhoAmI))
		{
			ResetTarget();
		}

		if (TargetWhoAmI < 0)
		{
			ChasePlayer(Owner);

			// Search target npc around the owner
			TargetWhoAmI = ProjectileUtils.FindTarget(Owner.Center, SearchDistance);
		}
		else
		{
			Attack();
		}
	}

	private void ResetTarget()
	{
		TargetWhoAmI = -1;
	}

	/// <summary>
	/// Update timeleft and frame of the projectile.
	/// </summary>
	private void UpdtateLifeCycle()
	{
		Timer++;

		// Update timeleft
		if (Owner.HasBuff<CrimsonMoonAlgaeSummonStaff_Buff>())
		{
			Projectile.timeLeft = 2; // Reset time left if the buff is active
		}
		else
		{
			Projectile.Kill(); // Kill the projectile if the buff is not active
		}

		// Update frame
		if (Main.timeForVisualEffects % 5 == 0)
		{
			Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
		}
	}

	private void ChasePlayer(Player player)
	{
		if (!Collision.IsWorldPointSolid(Projectile.Center))
		{
			var destination = player.Center + new Vector2(-90 * player.direction, -60) + new Vector2(MathF.Sin(Timer * 0.0759537f + Projectile.whoAmI) * 160, MathF.Sin(Timer * 0.0324789f - Projectile.whoAmI) * 80);
			var movement = destination - Projectile.Center;
			if (movement.Length() < 1)
			{
				Projectile.velocity = movement;
				return;
			}

			var velocityMax = 5f;
			if (movement.Length() >= 160f)
			{
				velocityMax = MathF.Max(5f, Owner.velocity.Length());
			}

			var toVelocity = movement.NormalizeSafe() * MathF.Min(movement.Length(), velocityMax);
			Projectile.velocity = Vector2.Lerp(Projectile.velocity, toVelocity, 0.2f);
			if (BlockedTimer > 0)
			{
				BlockedTimer--;
			}
		}
		else
		{
			BlockedTimer += 3;
			Vector2 toAir = TileUtils.ToNearestTypeOfTile(Projectile.Center, -1);
			Projectile.velocity = Projectile.velocity * 0.95f + toAir.NormalizeSafe() * 16f * 0.05f;
			if (BlockedTimer >= BlockedTimeMax)
			{
				ResetTarget();
				Projectile.Center = Owner.Center;
				BlockedTimer = 0;
			}
		}
	}

	private void Attack()
	{
		if (TargetWhoAmI <= -1)
		{
			return;
		}
		var target = Main.npc[TargetWhoAmI];
		Vector2 toTarget = target.Center - Projectile.Center;
		if (ProjectileAttackCount > 0)
		{
			if (!Collision.IsWorldPointSolid(Projectile.Center))
			{
				Vector2 chaseAim = target.Center - toTarget.NormalizeSafe() * 150f;
				Vector2 toAim = chaseAim - Projectile.Center;
				if (Collision.IsWorldPointSolid(toAim))
				{
					toAim += TileUtils.ToNearestTypeOfTile(toAim, -1);
				}
				if (toAim.Length() > 120)
				{
					toAim = toAim.NormalizeSafe() * 12f;
				}
				else
				{
					toAim = toAim / 10f;
				}
				Projectile.velocity = Projectile.velocity * 0.95f + toAim * 0.05f;
				foreach (var proj in Main.projectile)
				{
					if (proj is not null && proj.active && proj.type == Projectile.type && proj.owner == Projectile.owner)
					{
						Vector2 toProj = proj.Center - Projectile.Center;
						if (toProj.Length() < 30f)
						{
							Projectile.Center -= toProj.NormalizeSafe() * (30f - toProj.Length()) / 10f;
						}
					}
				}
				if (toTarget.Length() < 300 && Projectile.velocity.Length() < 5f)
				{
					if (ProjectileAttackCooling == 0)
					{
						Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, toTarget.NormalizeSafe() * 8f, ModContent.ProjectileType<CrimsonMoonAlgaeSummonStaff_minion_spore>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
						ProjectileAttackCount--;
						ProjectileAttackCooling = ProjectileAttackCoolingMax;
					}
					else
					{
						ProjectileAttackCooling--;
					}
				}
			}
			else
			{
				Vector2 toAir = TileUtils.ToNearestTypeOfTile(Projectile.Center, -1);
				Projectile.velocity = Projectile.velocity * 0.95f + toAir.NormalizeSafe() * 16f * 0.05f;
			}
		}
		else
		{
			if (ProjectileAttackCooling <= 0)
			{
				Projectile.velocity = Projectile.velocity * 0.95f + toTarget.NormalizeSafe() * 16f * 0.05f;
				if (toTarget.Length() < 20f || Projectile.Hitbox.Intersects(target.Hitbox))
				{
					Projectile.Kill();
					Projectile.NewProjectileDirect(Projectile.GetSource_FromAI(), Projectile.Center, Vector2.zeroVector, ModContent.ProjectileType<CrimsonMoonAlgaeSummonStaff_minion_Explosion>(), Projectile.damage * 3, Projectile.knockBack, Projectile.owner);
				}
			}
			else
			{
				if (ProjectileAttackCooling > 40)
				{
					ProjectileAttackCooling = 40;
				}
				ProjectileAttackCooling--;
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var texture_glow = ModAsset.CrimsonMoonAlgaeSummonStaff_minion_glow.Value;
		var texture_bloom = ModAsset.CrimsonMoonAlgaeSummonStaff_minion_bloom.Value;
		var texture_sub = ModAsset.CrimsonMoonAlgaeSummonStaff_minion_spore.Value;
		var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

		// Fix projectile direction when chasing owner.
		if (MathF.Abs(Projectile.velocity.X) <= 1E-05f && TargetWhoAmI == -1)
		{
			Projectile.direction = Owner.direction;
		}
		var spriteEffect = Projectile.direction < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffect, 0f);
		Main.spriteBatch.Draw(texture_glow, Projectile.Center - Main.screenPosition, frame, Color.White, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffect, 0f);
		Main.spriteBatch.Draw(texture_bloom, Projectile.Center - Main.screenPosition, frame, new Color(1f, 1f, 1f, 0), Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffect, 0f);
		if (ProjectileAttackCount > 0)
		{
			int offsetY = 0;
			if (Projectile.frame == 3 || Projectile.frame == 4)
			{
				offsetY = 1;
			}
			if (Projectile.frame == 6)
			{
				offsetY = -2;
			}
			if (Projectile.frame == 7)
			{
				offsetY = -1;
			}
			for (int k = 0; k < ProjectileAttackCount; k++)
			{
				Main.spriteBatch.Draw(texture_sub, Projectile.Center + new Vector2(1 * Projectile.direction, -1 + offsetY) - Main.screenPosition + new Vector2(6, 0).RotatedBy(k / (float)ProjectileAttackCount * MathHelper.TwoPi + Projectile.whoAmI + Main.GlobalTimeWrappedHourly), null, new Color(1f, 1f, 1f, 0), Projectile.rotation + Projectile.whoAmI, texture_sub.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0f);
			}
		}
		return false;
	}
}
