using Everglow.Commons.Utilities;
using Terraria.DataStructures;

namespace Everglow.Commons.Templates.Weapons;

/// <summary>
/// Handheld projectile.
/// </summary>
public abstract class HandheldProjectile : ModProjectile
{
	/// <summary>
	/// The rotation of texture.
	/// default to pi / 4, almost terraria painter like this angle.
	/// </summary>
	public float TextureRotation = 0;

	/// <summary>
	/// Max rotation speed of this projectile.
	/// default to 6.284, projectile will reach the rotation of playerCenter to mouseWorld in a sudden.
	/// </summary>
	public float MaxRotationSpeed = 6.284f;

	/// <summary>
	/// default to 1, projectile will reach the rotation of playerCenter to mouseWorld in a sudden.
	/// </summary>
	public float LerpFactorOfRotation = 1;

	/// <summary>
	/// default to (0, 0).
	/// </summary>
	public Vector2 DrawOffset = Vector2.zeroVector;

	/// <summary>
	/// Length to player arm.default to 50.
	/// </summary>
	public float DepartLength = 50;
	public Vector2 ArmRootPos = Vector2.zeroVector;

	/// <summary>
	/// World-space cursor of the owner. Main.MouseWorld already accounts for local gravity.
	/// </summary>
	protected Vector2 AimPosition
	{
		get
		{
			Player player = Main.player[Projectile.owner];
			player.ListenMouseWorld();
			return Projectile.owner == Main.myPlayer ? Main.MouseWorld : player.MouseWorld();
		}
	}

	protected Vector2 GravityDrawOffset => DrawOffset * new Vector2(1, Main.player[Projectile.owner].gravDir);

	private bool rotationInitialized;

	public override void OnSpawn(IEntitySource source)
	{
		InitializeRotation();
	}

	private void InitializeRotation()
	{
		rotationInitialized = true;
		// Spawn velocity is synchronized by Terraria, unlike the observer's cursor.
		Projectile.rotation = Projectile.velocity.SafeNormalize(Vector2.UnitX * Main.player[Projectile.owner].direction).ToRotation() + MathHelper.PiOver4;
	}

	public override bool PreAI()
	{
		Player player = Main.player[Projectile.owner];
		if (!player.active || player.dead)
		{
			Projectile.Kill();
			return false;
		}
		// Received projectiles do not run OnSpawn.
		if (!rotationInitialized)
		{
			InitializeRotation();
		}
		player.ListenMouseWorld();
		return true;
	}

	public override bool ShouldUpdatePosition() => false;

	public override void SetDefaults()
	{
		Projectile.width = 32;
		Projectile.height = 32;
		Projectile.aiStyle = -1;
		Projectile.timeLeft = 360000;
		Projectile.tileCollide = false;
		Projectile.ignoreWater = true;
		Projectile.friendly = true;
		Projectile.penetrate = -1;
		TextureRotation = MathHelper.PiOver4;
		SetDef();
	}

	public virtual void SetDef()
	{
	}

	public override void AI()
	{
		HeldProjectileAI();
		Player player = Main.player[Projectile.owner];
		RemoveExtraSameProjectiles(player);
	}

	public virtual void HeldProjectileAI()
	{
		Player player = Main.player[Projectile.owner];
		player.heldProj = Projectile.whoAmI;
		ArmRootPos = player.MountedCenter + new Vector2(-4 * player.direction, -2 * player.gravDir);
		player.SetCompositeArmFront(true, Player.CompositeArmStretchAmount.Full, (Projectile.rotation - MathF.PI * 0.25f) * player.gravDir - MathF.PI * 0.5f);
		Vector2 mouseToPlayer = AimPosition - ArmRootPos;
		mouseToPlayer = mouseToPlayer.SafeNormalize(Vector2.UnitX * player.direction);
		if (player.controlUseItem)
		{
			float addRot = MathHelper.WrapAngle(mouseToPlayer.ToRotation() + MathHelper.PiOver4 - Projectile.rotation);
			addRot *= LerpFactorOfRotation;
			if (Math.Abs(addRot) > MaxRotationSpeed)
			{
				addRot *= MaxRotationSpeed / Math.Abs(addRot);
			}
			Projectile.rotation += addRot;
			Projectile.Center = ArmRootPos + new Vector2(0, 1).RotatedBy(Projectile.rotation - Math.PI * 0.75) * DepartLength;

			Projectile.timeLeft = player.itemTimeMax;
		}
		if (!player.controlUseItem)
		{
			Projectile.Kill();
		}
		if (Projectile.Center.X < ArmRootPos.X)
		{
			player.direction = -1;
		}
		else
		{
			player.direction = 1;
		}
	}

	public virtual void RemoveExtraSameProjectiles(Player owner)
	{
		if (Projectile.owner == Main.myPlayer && owner.ownedProjectileCounts[Projectile.type] > 1)
		{
			int count = owner.ownedProjectileCounts[Projectile.type];
			for (int a = Main.projectile.Length - 1; a >= 0; a--)
			{
				if (Main.projectile[a] != null)
				{
					if (Main.projectile[a].active)
					{
						if (Main.projectile[a].type == Projectile.type && Main.projectile[a].owner == owner.whoAmI)
						{
							Main.projectile[a].Kill();
							count--;
							if (count <= 1)
							{
								return;
							}
						}
					}
				}
			}
		}
	}

	public override bool PreDraw(ref Color lightColor)
	{
		DrawBaseTexture(lightColor);
		return false;
	}

	public virtual void DrawBaseTexture(Color lightColor)
	{
		Player player = Main.player[Projectile.owner];
		var texMain = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects se = SpriteEffects.None;
		if (player.direction * player.gravDir == -1)
		{
			se = SpriteEffects.FlipVertically;
		}

		float rot = Projectile.rotation - (float)(Math.PI * 0.25) + TextureRotation * player.direction * player.gravDir;
		Main.spriteBatch.Draw(texMain, Projectile.Center - Main.screenPosition + GravityDrawOffset, null, lightColor, rot, texMain.Size() / 2f, 1f, se, 0);
	}
}
