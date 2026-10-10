using Terraria.DataStructures;
using Terraria.GameContent;
using static Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.ScarpasScissors;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Melee;

public class ScarpasScissorsProj : ScarpasScissorsProjBase
{
	private ref float Timer => ref Projectile.ai[1];

	private ref float InitialAngle => ref Projectile.ai[2];

	private ref float Progress => ref Projectile.localAI[1];

	private float SwingTime => 16f / Owner.GetTotalAttackSpeed(Projectile.DamageType);

	public override void SetDefaults()
	{
		Projectile.width = 52;
		Projectile.height = 52;

		Projectile.timeLeft = 10000;
		Projectile.penetrate = -1;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;

		Projectile.DamageType = DamageClass.Melee;
		Projectile.friendly = true;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = -1;
		Projectile.ownerHitCheck = true;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Projectile.spriteDirection = Main.MouseWorld.X > Owner.MountedCenter.X ? 1 : -1;

		if (Projectile.spriteDirection == 1)
		{
			InitialAngle = MathHelper.Pi;
		}
		else
		{
			InitialAngle = 0;
		}
	}

	public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
	{
		Vector2 start = Owner.MountedCenter;
		Vector2 end = start + Projectile.rotation.ToRotationVector2() * (Projectile.Size.Length() * Projectile.scale);
		float collisionPoint = 0f;
		return Collision.CheckAABBvLineCollision(targetHitbox.TopLeft(), targetHitbox.Size(), start, end, 15f * Projectile.scale, ref collisionPoint);
	}

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
		Progress = Timer / SwingTime;

		if (Timer > SwingTime)
		{
			Projectile.Kill();
		}

		SetWeaponPosition_Swing();
	}

	// Function to easily set projectile and arm position
	public void SetWeaponPosition_Swing()
	{
		Projectile.rotation = InitialAngle + Projectile.spriteDirection * Progress * 3; // Set projectile rotation

		Owner.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, Projectile.rotation - MathHelper.ToRadians(90f));
		Vector2 armPosition = Owner.GetFrontHandPosition(Player.CompositeArmStretchAmount.Full, Projectile.rotation - (float)Math.PI / 2);
		armPosition.Y += Owner.gfxOffY;

		Projectile.Center = armPosition; // Set projectile to arm position
		Projectile.scale = Owner.GetAdjustedItemScale(Owner.HeldItem); // Slightly scale up the projectile and also take into account melee size modifiers
	}

	public override bool PreDraw(Player player, ref Color lightColor)
	{
		Vector2 origin;
		float rotationOffset;
		SpriteEffects effects;

		if (Projectile.spriteDirection > 0)
		{
			origin = new Vector2(0, Projectile.height);
			rotationOffset = MathHelper.ToRadians(45f);
			effects = SpriteEffects.None;
		}
		else
		{
			origin = new Vector2(Projectile.width, Projectile.height);
			rotationOffset = MathHelper.ToRadians(135f);
			effects = SpriteEffects.FlipHorizontally;
		}

		Texture2D texture = TextureAssets.Projectile[Type].Value;
		Main.spriteBatch.Draw(texture, Projectile.Center - Main.screenPosition, default, lightColor * Projectile.Opacity, Projectile.rotation + rotationOffset, origin, Projectile.scale, effects, 0);

		return false;
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
