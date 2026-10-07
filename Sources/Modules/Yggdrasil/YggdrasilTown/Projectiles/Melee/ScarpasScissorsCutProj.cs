using Everglow.Commons.VFX.CommonVFXDusts;
using Terraria.Audio;
using Terraria.DataStructures;
using static Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.ScarpasScissors;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public abstract class ScarpasScissorsCutProj : ScarpasScissorsProjBase
{
	private const float BaseDuration = 80f;
	private const float HitBoxWidth = 30f;

	// Arm position
	private const float RotationOffSetFrontArm = 0.4f;
	private const float RotationOffsetBackArm = 0.8f;

	// Draw
	private const float DistanceToArmPosition = 15f;
	private const float AnimationRotationMax = 0.5f;
	private const float AnimationRotationOffset = 0.04f;
	private const float AnimationTimePowerClosing = 16f;
	private const float AnimationTimePowerOpening = 2f;

	/// <summary>
	/// Determines if this projectile represents the left blade.
	/// </summary>
	protected abstract bool IsLeftBlade { get; }

	protected abstract Vector2 TextureOrigin { get; }

	/// <summary>
	/// Timer tracking projectile lifetime.
	/// </summary>
	private ref float Timer => ref Projectile.ai[0];

	/// <summary>
	/// Flag indicating if cut has been processed.
	/// </summary>
	public bool HasCut
	{
		get => Projectile.ai[1] == 1f;
		set => Projectile.ai[1] = value ? 1f : 0f;
	}

	/// <summary>
	/// Duration adjusted by player attack speed.
	/// </summary>
	private float Duration => BaseDuration / Owner.GetTotalAttackSpeed(Projectile.DamageType);

	/// <summary>
	/// Current animation progress (0 to 1).
	/// </summary>
	private float Progress => Timer / Duration;

	/// <summary>
	/// Current animation time value.
	/// </summary>
	private float TimeForAnimation => MathHelper.TwoPi * 4 * MathF.Pow(Progress, 1.6f);

	/// <summary>
	/// Indicates if blades are in closing phase.
	/// </summary>
	private bool IsClosing => TimeForAnimation % MathHelper.TwoPi > MathHelper.Pi;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;

		Projectile.timeLeft = 10000;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.DamageType = DamageClass.Melee;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 12;
		Projectile.ownerHitCheck = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;
		Projectile.rotation = (Main.MouseWorld - Owner.Center).ToRotation();
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 center = Owner.MountedCenter;
		Vector2 start = center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * 0.4f * Projectile.scale);
		Vector2 end = center + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
		float collisionPoint = 0f;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, HitBoxWidth * Projectile.scale, ref collisionPoint);
	}

	public override bool? CanHitNPC(NPC target) => IsClosing && !HasCut;

	public override void AI()
	{
		if (!HeldProjAI())
		{
			return;
		}

		AttackAI();

		Timer++;
	}

	private bool HeldProjAI()
	{
		Owner.itemAnimation = 2;
		Owner.itemTime = 2;
		Owner.heldProj = Projectile.whoAmI;

		if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed)
		{
			Projectile.Kill();
			return false;
		}

		return true;
	}

	private void AttackAI()
	{
		if (Timer > Duration)
		{
			Projectile.Kill();
			return;
		}

		SetWeaponPosition();
		ProcessCutEffects();
	}

	public void SetWeaponPosition()
	{
		// Set arm position
		float rotationOffset = Projectile.spriteDirection > 0 ? MathHelper.ToRadians(45f) : MathHelper.ToRadians(135f);
		rotationOffset += 0.6f * Owner.direction;

		var rotation = Projectile.rotation - rotationOffset;
		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, rotation + AnimationRotation() + RotationOffSetFrontArm);
		Owner.SetCompositeArmBack(true, Player.CompositeArmStretchAmount.Full, rotation - AnimationRotation() - RotationOffsetBackArm);

		// Set weapon position
		Vector2 weaponPosition = Owner.Center;
		weaponPosition.Y += Owner.gfxOffY;

		Projectile.Center = weaponPosition;
		Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers
	}

	private void ProcessCutEffects()
	{
		if (AnimationRotation() <= 0f && IsLeftBlade && !HasCut)
		{
			HasCut = true;
			Dust.NewDust(Projectile.Center + Projectile.rotation.ToRotationVector2() * 40f, 1, 1, DustID.Iron, Scale: 0.4f);
			SoundEngine.PlaySound(SoundID.Item1, Projectile.Center);
		}
		else if (AnimationRotation() > 0f)
		{
			HasCut = false;
		}
	}

	public float AnimationRotation()
	{
		float t = TimeForAnimation;
		float animationTime = IsClosing
			? MathF.Pow((-MathF.Cos(t) + 1f) / 2f, AnimationTimePowerClosing) - AnimationRotationOffset // Closing quickly
			: MathF.Pow((-MathF.Cos(t) + 1f) / 2f, AnimationTimePowerOpening) - AnimationRotationOffset; // Opening slowly
		return animationTime * AnimationRotationMax;
	}

	public override bool PreDraw(ref Color lightColor)
	{
		var drawColor = lightColor * Projectile.Opacity;
		var distanceToArmCenter = DistanceToArmPosition * Projectile.scale;
		var position = Owner.Center - Main.screenPosition + Projectile.rotation.ToRotationVector2() * distanceToArmCenter;

		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var origin = TextureOrigin;
		float rotationOffset = MathHelper.ToRadians(45f);
		SpriteEffects effects = SpriteEffects.None;
		if (Projectile.spriteDirection <= 0)
		{
			origin.X = texture.Width - origin.X;
			rotationOffset = MathHelper.ToRadians(135f);
			effects = SpriteEffects.FlipHorizontally;
		}

		var rotation = IsLeftBlade
			? rotationOffset + Projectile.spriteDirection * AnimationRotation()
			: rotationOffset - Projectile.spriteDirection * AnimationRotation();

		Main.spriteBatch.Draw(texture, position, default, drawColor, Projectile.rotation + rotation, origin, Projectile.scale, effects, 0);

		return false;
	}

	public sealed override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
	{
		base.OnHitNPC(target, hit, damageDone);

		if (hit.Crit)
		{
			for (int i = 0; i < 10; i++)
			{
				Ins.VFXManager.Add(new BloodDrop()
				{
					Active = true,
					Visible = true,
					Position = target.Center,
					Velocity = Main.rand.NextFloat(0, MathHelper.TwoPi).ToRotationVector2() * 2f,
					MaxTime = Main.rand.Next(82, 164),
					Scale = Main.rand.NextFloat(12f, Main.rand.NextFloat(12f, 28.0f)),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				});
			}
		}
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write((sbyte)Projectile.spriteDirection);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		Projectile.spriteDirection = reader.ReadSByte();
	}
}
