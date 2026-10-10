using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.VFXs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.KelpCurtain.Projectiles.Summon;

public class WitherbarkMinion : ModProjectile
{
	// Minion data
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	// Minion data
	public const int Minion_ContactDamage = 30;
	public const float Minion_Knockback = 1f;

	/// <summary>
	/// The distance to teleport back to the owner if the minion is too far away.
	/// </summary>
	private const int TeleportDistance = 1000;

	/// <summary>
	/// The distance to search for target npc.
	/// </summary>
	private const int SearchDistance = 800;

	// Blink
	private const int BlinkDistance = 600;
	private const int BlinkTimeMax = 60;

	// Normal attack
	private const int AccelerateTimeMax = 4;
	private const int ChargeTimeMax = 40;
	private const int ChargeMovementDiffMax = 5;

	// Projectile attack
	private const int ProjectileAttack_Cooldown = 240;
	public const int ProjectileAttack_ShootGap = 10;
	public const int ProjectileAttack_ProjectileCount = 8;

	public const int ProjectileAttack_Damage = 10;
	public const float ProjectileAttack_Knockback = 0.2f;

	public enum AttackState
	{
		None,
		Blink,
		NormalAttack,
		ProjectileAttack,
	}

	public enum NormalAttackState
	{
		Preparing,
		Accelerating,
		Charging,
		Decelerating,
	}

	private Player Owner => Main.player[Projectile.owner];

	/// <summary>
	/// A timer that is used to control the minion's attack state and cooldown. Has different meanings in different states.
	/// </summary>
	private ref float Timer => ref Projectile.localAI[0];

	private ref float ProjectileAttackCooldownTimer => ref Projectile.localAI[1];

	/// <summary>
	/// The whoAmI of the target npc that the minion is currently attacking.
	/// <para/>Defaults to -1, and it will be reset to -1 if the target npc is not found or is inactive.
	/// </summary>
	private int TargetWhoAmI { get; set; } = -1;

	public AttackState MinionAttackState { get; private set; } = AttackState.None;

	public NormalAttackState MinionNormalAttackState { get; private set; } = NormalAttackState.Preparing;

	private Vector2 DashStartPos { get; set; } = Vector2.Zero;

	private Vector2 DashEndPos { get; set; } = Vector2.Zero;

	public override void SetStaticDefaults()
	{
		Main.projFrames[Projectile.type] = 3;
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

		Projectile.minion = true;
		Projectile.minionSlots = 0;

		Projectile.timeLeft = 2;

		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 20;
	}

	/// <summary>
	/// Checks if the minion can deal damage to the target npc.
	/// <para/>Can only do contact damage when the minion is in a normal attack state and charging or decelerating.
	/// </summary>
	/// <returns></returns>
	public override bool? CanDamage() => MinionAttackState is AttackState.NormalAttack
		&& MinionNormalAttackState is NormalAttackState.Charging or NormalAttackState.Decelerating;

	public override void AI()
	{
		UpdtateLifeCycle();

		ProjectileAttackCooldownTimer--;

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

		ResetAttackState();
	}

	/// <summary>
	/// Update timeleft and frame of the projectile.
	/// </summary>
	private void UpdtateLifeCycle()
	{
		// Update timeleft
		if (Owner.HasBuff<WitherbarkSetBuff>())
		{
			Projectile.timeLeft = 2; // Reset time left if the buff is active
		}
		else
		{
			Projectile.Kill(); // Kill the projectile if the buff is not active
		}

		// Update frame
		if (Main.timeForVisualEffects % 15 == 0)
		{
			Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
		}
	}

	private void ChasePlayer(Player player)
	{
		var destination = player.Center - new Vector2(40 * player.direction, 30);
		var movement = destination - Projectile.Center;
		if (movement.Length() < 1)
		{
			Projectile.velocity = movement;
			return;
		}

		var velocityMax = 5f;
		if (movement.Length() >= 160f)
		{
			velocityMax = MathF.Max(8f, Owner.velocity.Length());
		}

		var toVelocity = movement.NormalizeSafe() * MathF.Min(movement.Length(), velocityMax);
		Projectile.velocity = Vector2.Lerp(Projectile.velocity, toVelocity, 0.2f);
	}

	private void Attack()
	{
		var target = Main.npc[TargetWhoAmI];
		if (MinionAttackState is not AttackState.Blink
			&& Vector2.Distance(Projectile.Center, target.Center) > BlinkDistance)
		{
			ResetNormalAttackState();
			ResetProjectileAttackState();

			MinionAttackState = AttackState.Blink;
		}

		if (MinionAttackState is AttackState.None)
		{
			ResetNormalAttackState();
			ResetProjectileAttackState();

			// Randomly choose to dash or shoot.
			if (ProjectileAttackCooldownTimer > 0 || Main.rand.NextFloat() >= 0.2f)
			{
				ResetNormalAttackState();
				MinionAttackState = AttackState.NormalAttack;
			}
			else
			{
				ResetProjectileAttackState();
				MinionAttackState = AttackState.ProjectileAttack;
			}
		}

		// Manage attack state
		switch (MinionAttackState)
		{
			case AttackState.Blink:
				Blink(target);
				break;
			case AttackState.NormalAttack:
				NormalAttack(target);
				break;
			case AttackState.ProjectileAttack:
				ProjectileAttack(target);
				break;
			default:
				break;
		}
	}

	private void ResetAttackState()
	{
		MinionAttackState = AttackState.None;
		ResetNormalAttackState();
		ResetProjectileAttackState();
	}

	private void Blink(NPC target)
	{
		Projectile.velocity *= 0.91f;

		if (Main.timeForVisualEffects % 3 == 0)
		{
			Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, newColor: new Color(0f, 0.5f, 0f), Scale: 1.1f);
		}

		Timer++;

		if (Timer >= BlinkTimeMax)
		{
			Timer = 0;
			Projectile.Center = target.Center;
			MinionAttackState = AttackState.None;

			for (int i = 0; i < 20; i++)
			{
				Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, newColor: new Color(0f, 0.5f, 0f), Scale: 1.2f);
			}
		}
	}

	private void NormalAttack(NPC target)
	{
		switch (MinionNormalAttackState)
		{
			case NormalAttackState.Preparing:
				// Create vfx
				GenerateBurstDusts();

				// Set base dash info
				var movementToTarget = target.Center - Projectile.Center + target.velocity;
				var dashLength = Math.Max(movementToTarget.Length(), 64f);
				DashStartPos = Projectile.Center;
				DashEndPos = Projectile.Center + movementToTarget.NormalizeSafe() * dashLength;

				MinionNormalAttackState = NormalAttackState.Accelerating;
				Timer = 0;
				break;
			case NormalAttackState.Accelerating:
				Timer++;
				var movement = DashEndPos - DashStartPos;
				Projectile.velocity = Vector2.Lerp(Projectile.velocity, movement.NormalizeSafe() * 16f, Math.Clamp(MathF.Pow(Timer / AccelerateTimeMax, 2), 0f, 1f));
				if (Projectile.velocity.Length() >= 8f)
				{
					Timer = 0;
					MinionNormalAttackState = NormalAttackState.Charging;
				}
				break;
			case NormalAttackState.Charging:
				Timer++;
				Projectile.velocity *= 0.99f;
				if (Vector2.Distance(Projectile.Center, DashEndPos) <= ChargeMovementDiffMax || Timer >= ChargeTimeMax)
				{
					Timer = 0;
					MinionNormalAttackState = NormalAttackState.Decelerating;
				}
				break;
			case NormalAttackState.Decelerating:
				Projectile.velocity *= 0.92f;
				if (Projectile.velocity.Length() < 0.1f)
				{
					Projectile.velocity = Vector2.Zero;
					ResetAttackState();
				}
				break;
		}
	}

	private void ResetNormalAttackState()
	{
		MinionNormalAttackState = NormalAttackState.Preparing;
		Timer = 0;
	}

	private void GenerateBurstDusts()
	{
		for (int t = 0; t < 25; t++)
		{
			Vector2 phi = new Vector2(0, Main.rand.Next(14, 22)).RotatedBy(t / 25f * MathHelper.TwoPi);
			var dustVFX4 = new MossBlossomDustSide
			{
				Velocity = phi.RotatedBy(0.7f) * 0.2f,
				Active = true,
				Visible = true,
				Position = Projectile.Center + phi * 0.4f,
				MaxTime = Main.rand.Next(15, 40),
				Scale = Main.rand.NextFloat(12, 16),
				Rotation = phi.RotatedBy(0.7f).ToRotationSafe() - MathHelper.PiOver4 * 3,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX4);
		}
	}

	private void ProjectileAttack(NPC target)
	{
		Projectile.velocity = Vector2.Zero;
		Timer++;

		// Shoot leaf projectile
		if (Timer <= ProjectileAttack_ShootGap * ProjectileAttack_ProjectileCount
			&& Timer % ProjectileAttack_ShootGap == 0)
		{
			var movement = target.Center - Projectile.Center;
			var leafVelocity = movement.NormalizeSafe() * WitherbarkMinion_Leaf.GeneralSpeed;
			Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center, leafVelocity, ModContent.ProjectileType<WitherbarkMinion_Leaf>(), ProjectileAttack_Damage, ProjectileAttack_Knockback, Projectile.owner, Projectile.whoAmI, -1);
		}

		if (Timer >= ProjectileAttack_ShootGap * (ProjectileAttack_ProjectileCount + 2))
		{
			Timer = 0;
			ProcAllProjectiles(target);
			ResetAttackState();
			ProjectileAttackCooldownTimer = ProjectileAttack_Cooldown;
		}
	}

	private void ProcAllProjectiles(NPC target = null)
	{
		foreach (var proj in Main.ActiveProjectiles)
		{
			if (proj.owner == Projectile.owner
				&& proj.type == ModContent.ProjectileType<WitherbarkMinion_Leaf>()
				&& proj.ModProjectile is WitherbarkMinion_Leaf leaf)
			{
				leaf.Proc(target);
			}
		}
	}

	private void ResetProjectileAttackState()
	{
		ProcAllProjectiles();
		Timer = 0;
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var frame = texture.Frame(1, Main.projFrames[Projectile.type], 0, Projectile.frame);

		// Fix projectile direction when chasing owner.
		if (MathF.Abs(Projectile.velocity.X) <= 1E-05f && TargetWhoAmI == -1)
		{
			Projectile.direction = player.direction;
		}
		var spriteEffect = Projectile.direction > 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, frame, lightColor, Projectile.rotation, frame.Size() / 2f, Projectile.scale, spriteEffect, 0f);

		// Draw dash shield in normal-charge state
		if (MinionNormalAttackState is NormalAttackState.Charging)
		{
			DrawDashShield();
		}

		return false;
	}

	private void DrawDashShield()
	{
		var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(
			SpriteSortMode.Immediate,
			BlendState.AlphaBlend,
			SamplerState.LinearClamp,
			DepthStencilState.None,
			RasterizerState.CullNone,
			null,
			Main.GameViewMatrix.TransformationMatrix);

		var texture = Commons.ModAsset.Trail_0.Value;
		var drawPos = Projectile.Center - Main.screenPosition + Projectile.velocity;
		var drawColor = new Color(0.2f, 1f, 0.2f, 0f);
		var rotation = Projectile.velocity.ToRotation();

		float size = 80 * (0.6f + 0.4f * Timer / ChargeTimeMax);
		var vertices = new List<Vertex2D>();

		int count = 90;
		for (int i = 0; i <= count; i++)
		{
			var progress = (float)i / count;
			var x = progress + 0.1f;
			var textureCoordYOffset = (1f / x) * (0.01f * MathF.Sin(x * 20) + 1);
			var drawColorStrength = 1 - MathF.Pow(progress, 4);
			var texCoordX = ((progress - (float)Main.timeForVisualEffects * 0.04f) % 1 + 1) % 1;
			vertices.Add(drawPos + new Vector2(size * (1 - 2 * progress), -size * 1.2f).RotatedBy(rotation), drawColor * drawColorStrength, new(texCoordX, 0.5f - textureCoordYOffset, 0f));
			vertices.Add(drawPos + new Vector2(size * (1 - 2 * progress), +size * 1.2f).RotatedBy(rotation), drawColor * drawColorStrength, new(texCoordX, 0.5f + textureCoordYOffset, 0f));
		}
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
	}
}
