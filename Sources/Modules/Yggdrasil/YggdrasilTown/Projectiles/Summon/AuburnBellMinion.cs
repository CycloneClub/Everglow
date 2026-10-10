using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Microsoft.Xna.Framework;
using Terraria;

namespace Everglow.Yggdrasil.YggdrasilTown.Projectiles.Summon;

public class AuburnBellMinion : ModProjectile
{
	public override string LocalizationCategory => Everglow.Commons.Utilities.LocalizationUtils.Categories.SummonProjectiles;

	private enum AttackPhaseEnum
	{
		/// <summary>
		/// 1. The minion try to move to a position where pitch to the position of target is less than 45°
		/// </summary>
		Aim,

		/// <summary>
		/// 2. The minion dash to the enemy, adjust its rotation to towarding the position of target, until puncturing target or dashing for a certain amount of time
		/// </summary>
		Dash,

		/// <summary>
		/// 3. The minion try to slow down and restore balance
		/// </summary>
		Decelerate,
	}

	private const int BodyLength = 12;
	private const float NotMovingVelocity = 1E-05f;
	private const float MaxDistanceToOwner = 1000f;
	private const int MaxTeleportCooldown = 60;
	private const int SearchDistance = 1000;
	private const int DashDistance = 300;
	private const int DashPhaseDistanceMin = 150;
	private const int DashPhaseDistanceMax = 400;

	private Vector2 dashStartPos;
	private Vector2 dashEndPos;

	private int targetWhoAmI = -1;

	private int TargetWhoAmI
	{
		get => targetWhoAmI;
		set => targetWhoAmI = value;
	}

	private Player Owner => Main.player[Projectile.owner];

	private int MinionIndex => (int)Projectile.ai[0];

	private int Timer
	{
		get { return (int)Projectile.ai[1]; }
		set { Projectile.ai[1] = value; }
	}

	private AttackPhaseEnum AttackPhase { get; set; }

	private int TeleportCooldown { get; set; }

	public override bool? CanCutTiles() => false;

	public override bool MinionContactDamage() => true;

	public override void SetStaticDefaults()
	{
		Main.projPet[Projectile.type] = true;
		Main.projFrames[Projectile.type] = 8;
		ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
		ProjectileID.Sets.TrailCacheLength[Projectile.type] = 5;
		ProjectileID.Sets.CultistIsResistantTo[Projectile.type] = true;
		ProjectileID.Sets.MinionSacrificable[Projectile.type] = true;
	}

	public override void SetDefaults()
	{
		Projectile.tileCollide = false;
		Projectile.friendly = true;
		Projectile.hostile = false;
		Projectile.usesLocalNPCImmunity = true;
		Projectile.localNPCHitCooldown = 30;
		Projectile.timeLeft = 18000;

		Projectile.penetrate = -1;
		Projectile.aiStyle = -1;
		Projectile.width = 70;
		Projectile.height = 10;

		Projectile.DamageType = DamageClass.Summon;
		Projectile.minion = true;
		Projectile.minionSlots = 1;

		Projectile.netImportant = true;

		TargetWhoAmI = -1;
		TeleportCooldown = 0;
		AttackPhase = AttackPhaseEnum.Aim;
		Projectile.drawLayer = ProjectileDrawLayerID.BehindNPCsAndTiles;
	}

	public override void AI()
	{
		UpdateLifeCycle();

		LimitDistanceFromOwner();

		if (TargetWhoAmI == -1 || !CheckTargetActive())
		{
			Idle();
			SearchTarget();
		}
		else
		{
			Attack();
		}
	}

	#region AI

	/// <summary>
	/// 1. Reset timeleft and buff duration to keep projectile alive.
	/// 2. Update projectile frame.
	/// </summary>
	private void UpdateLifeCycle()
	{
		if (CheckOwnerActive())
		{
			Owner.AddBuff(ModContent.BuffType<AuburnBell>(), 2);
			Projectile.timeLeft = 2;
		}

		Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
	}

	/// <summary>
	/// Keep the distance between minion and owner within a certain amount
	/// </summary>
	private void LimitDistanceFromOwner()
	{
		if (TeleportCooldown > 0)
		{
			TeleportCooldown--;
		}
		else if (Projectile.Center.Distance(Owner.Center) > MaxDistanceToOwner)
		{
			ResetTarget();

			// Teleport to
			TeleportCooldown = MaxTeleportCooldown;
			Projectile.Center = Owner.MountedCenter + new Vector2((10 - MinionIndex * 30) * Owner.direction, -40 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f - MinionIndex) * 35f);
		}
	}

	private void MoveTo(Vector2 aim)
	{
		float timeValue = (float)(Main.timeForVisualEffects * 0.014f);

		Projectile.velocity *= 0.97f;

		var newRotation = MathF.Log(MathF.Abs(Projectile.velocity.X) + 1) * 0.2f * Projectile.direction;
		Projectile.rotation = Projectile.rotation * 0.95f + newRotation * 0.05f;
		int dirY = Projectile.velocity.Y >= 0 ? 1 : -1;
		Vector2 aimPosition =
			aim +
			new Vector2(
				210f * MathF.Sin(timeValue * 2 + Projectile.whoAmI) * Projectile.direction + MinionIndex,
				(-50 + MinionIndex + 30f * MathF.Sin(timeValue * 0.15f + Projectile.whoAmI)) * dirY)
			* Projectile.scale;
		Vector2 toAim = aimPosition - Projectile.Center - Projectile.velocity;
		if (toAim.Length() > 50)
		{
			Projectile.velocity += Vector2.Normalize(toAim) * 0.15f * Projectile.scale;
		}
	}

	/// <summary>
	/// Check if owner is active
	/// </summary>
	/// <returns>
	/// active: true | inactive: false
	/// </returns>
	private bool CheckOwnerActive()
	{
		if (Owner.dead || Owner.active is false)
		{
			Owner.ClearBuff(ModContent.BuffType<AuburnBell>());
			return false;
		}

		if (!Owner.HasBuff(ModContent.BuffType<AuburnBell>()))
		{
			return false;
		}

		return true;
	}

	/// <summary>
	/// Search target within attack distance
	/// </summary>
	private void SearchTarget()
	{
		Projectile.Minion_FindTargetInRange(SearchDistance, ref targetWhoAmI, false);
	}

	private void ResetTarget()
	{
		TargetWhoAmI = -1;
		AttackPhase = AttackPhaseEnum.Aim;
	}

	/// <summary>
	/// Check if target is active
	/// </summary>
	/// <returns>
	/// active: true | inactive: false
	/// </returns>
	private bool CheckTargetActive()
	{
		if (TargetWhoAmI == -1)
		{
			return false;
		}

		NPC target = Main.npc[TargetWhoAmI];
		if (!target.active || target.dontTakeDamage || !target.CanBeChasedBy() || target.friendly)
		{
			ResetTarget();
			return false;
		}

		return true;
	}

	/// <summary>
	/// Move around owner and patrol
	/// </summary>
	private void Idle()
	{
		Vector2 aim;

		if (Owner.velocity.Length() > NotMovingVelocity) // Player is moving
		{
			aim = Owner.MountedCenter
				+ new Vector2(
					x: (10 - MinionIndex * 30) * Owner.direction,
					y: -40 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f - MinionIndex) * 35f);
		}
		else
		{
			aim = Owner.MountedCenter
				+ new Vector2(
					x: Owner.direction * (MathF.Sin((float)Main.timeForVisualEffects * 0.02f) * 40f - MinionIndex * 30),
					y: -50 + MathF.Sin((float)Main.timeForVisualEffects * 0.04f) * 20f);
		}

		MoveTo(aim);
		Projectile.rotation = Projectile.rotation * 0.95f + Projectile.velocity.X * 0.002f;
	}

	/// <summary>
	/// Attack target
	/// </summary>
	private void Attack()
	{
		switch (AttackPhase)
		{
			case AttackPhaseEnum.Aim:
				{
					Attack_Aim();
					break;
				}
			case AttackPhaseEnum.Dash:
				{
					Attack_Dash();
					break;
				}
			case AttackPhaseEnum.Decelerate:
				{
					Attack_Decelerate();
					break;
				}
		}
	}

	private void Attack_Aim()
	{
		var target = Main.npc[TargetWhoAmI];
		var distance = target.Center - Projectile.Center;
		var angleToTarget = distance.ToRotation();
		var sinOfAngleToTarget = MathF.Sin(angleToTarget);

		if (MathF.Abs(sinOfAngleToTarget) > MathF.Sqrt(2f) / 2f)
		{
			MoveTo(target.Center + new Vector2(-1, 1) * distance.NormalizeSafe() * 200);
		}
		else
		{
			if (distance.Length() < DashPhaseDistanceMin)
			{
				MoveTo(target.Center - distance.NormalizeSafe() * 300);
			}
			else if (distance.Length() > DashPhaseDistanceMax)
			{
				MoveTo(target.Center + distance.NormalizeSafe() * 300);
			}
			else
			{
				MoveTo(target.Center);
			}
		}

		Timer++;
		if (Timer >= 60
			&& MathF.Abs(sinOfAngleToTarget) <= MathF.Sqrt(2f) / 2f
			&& distance.Length() >= DashPhaseDistanceMin
			&& distance.Length() <= DashPhaseDistanceMax)
		{
			Timer = 0;
			dashStartPos = Projectile.Center;
			dashEndPos = target.Center + distance.NormalizeSafe() * (DashDistance - distance.Length() + MinionIndex);
			AttackPhase = AttackPhaseEnum.Dash;
		}
	}

	private void Attack_Dash()
	{
		var dashProgress = Timer / 30f;
		var pos = dashStartPos + (dashEndPos - dashStartPos) * dashProgress;
		Projectile.velocity = pos - Projectile.Center;
		if (Projectile.velocity.X >= 0)
		{
			Projectile.rotation = MathHelper.Lerp(Projectile.rotation, Projectile.velocity.ToRotation(), dashProgress);
		}
		else
		{
			Projectile.rotation = MathHelper.Lerp(Projectile.rotation, -MathF.Sign(Projectile.velocity.ToRotation()) * (MathHelper.Pi - MathF.Abs(Projectile.velocity.ToRotation())), dashProgress);
		}

		Timer++;
		if (Timer == 30)
		{
			Timer = 0;
			AttackPhase = AttackPhaseEnum.Decelerate;
		}
	}

	private void Attack_Decelerate()
	{
		Projectile.velocity *= 0.96f;
		MoveTo(Projectile.Center);
		if (Projectile.velocity.Length() < 2f)
		{
			AttackPhase = AttackPhaseEnum.Aim;
		}
	}

	#endregion

	public override bool PreDraw(Player player, ref Color lightColor)/* tModPorter Replace 'Main.player[Projectile.owner]' with 'player'. */
	{
		// Draw Minion Body
		// ================
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var vertices = new List<Vertex2D>();
		var drawCenter = Projectile.Center - Main.screenPosition;
		var segmentDrawCenter = drawCenter + new Vector2(BodyLength * 5f * 0.75f, 0).RotatedBy(Projectile.rotation) * Projectile.scale;
		for (int i = 0; i < BodyLength; i++)
		{
			Color drawColor = Lighting.GetColor(Projectile.Center.ToTileCoordinates());
			float segmentIndex = i / (float)BodyLength;
			float segmentScale = (MathF.Sin(segmentIndex * MathHelper.Pi) * 0.5f + 0.5f) * Projectile.scale;

			// Calculate texCoord
			int frameY = (Projectile.frame + i) % Main.projFrames[Projectile.type];
			float texCoordOffsetY = frameY / (float)Main.projFrames[Projectile.type];
			float texCoordHeight = 1 / (float)Main.projFrames[Projectile.type];

			// Upper triangle
			vertices.Add(segmentDrawCenter + new Vector2(-20, -20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(0, texCoordOffsetY, 0));
			vertices.Add(segmentDrawCenter + new Vector2(-20, 20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(0, texCoordOffsetY + texCoordHeight, 0));
			vertices.Add(segmentDrawCenter + new Vector2(20, -20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(1, texCoordOffsetY, 0));

			// Lower triangle
			vertices.Add(segmentDrawCenter + new Vector2(20, -20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(1, texCoordOffsetY, 0));
			vertices.Add(segmentDrawCenter + new Vector2(-20, 20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(0, texCoordOffsetY + texCoordHeight, 0));
			vertices.Add(segmentDrawCenter + new Vector2(20, 20).RotatedBy(Projectile.rotation) * segmentScale, drawColor, new Vector3(1, texCoordOffsetY + texCoordHeight, 0));

			segmentDrawCenter -= new Vector2(10, 0).RotatedBy(Projectile.rotation) * segmentScale;
		}

		SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.NonPremultiplied, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Main.graphics.GraphicsDevice.RasterizerState = RasterizerState.CullNone;
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		if (vertices.Count > 3)
		{
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, vertices.ToArray(), 0, vertices.Count / 3);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		return false;
	}
}
