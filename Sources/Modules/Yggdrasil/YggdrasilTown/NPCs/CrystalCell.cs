using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CrystalCell : ModNPC
{
	private const float SurfaceProbeDistance = 25f;
	private const float IdleSpeed = 1.4f;
	private const float AngrySpeed = 5.5f;
	private static readonly float LaunchRange = 140f;
	private static readonly float LaunchSpeed = 10f;

	private Vector2 surfaceNormal = -Vector2.UnitY;
	private Vector2 surfaceTangent = Vector2.UnitX;
	private bool isResting;
	private bool isAngry;
	private bool isLaunched;
	private bool isImpactResting;
	private int stateTimer;
	private int stateDuration;
	private int jumpCooldown;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		TwilightForsetAndRelic twilightBiome = ModContent.GetInstance<TwilightForsetAndRelic>();
		return twilightBiome.IsBiomeActive(spawnInfo.Player) ? 3f : 0f;
	}

	public override void SetDefaults()
	{
		NPC.width = 32;
		NPC.height = 32;
		NPC.lifeMax = 80;
		NPC.damage = 25;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.direction = Main.rand.NextBool() ? 1 : -1;
		NPC.spriteDirection = NPC.direction;
		stateDuration = Main.rand.Next(90, 181);
		TryAttachToSurface(true);
	}

	public override void FindFrame(int frameHeight)
	{
		if (isResting || (!isAngry && NPC.velocity.LengthSquared() < 0.05f))
		{
			NPC.frame.Y = 0;
			NPC.frameCounter = 0;
			return;
		}

		NPC.frameCounter++;
		if (NPC.frameCounter >= (isAngry ? 4 : 7))
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += frameHeight;
			if (NPC.frame.Y >= frameHeight * 8)
			{
				NPC.frame.Y = frameHeight;
			}
		}
	}

	public override void AI()
	{
		if (jumpCooldown > 0)
		{
			jumpCooldown--;
		}

		if (isLaunched)
		{
			UpdateProjectile();
			return;
		}

		if (isImpactResting)
		{
			UpdateImpactRest();
			return;
		}

		if (!TryAttachToSurface(false))
		{
			DetachAndFall();
			return;
		}

		if (isAngry)
		{
			UpdateAngryMovement();
		}
		else
		{
			UpdateNeutralMovement();
		}

		Lighting.AddLight(NPC.Center, new Vector3(0.35f, 0.1f, 0.55f));
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		isAngry = true;
		isResting = false;
		stateTimer = 0;
		stateDuration = 0;
	}

	private void UpdateNeutralMovement()
	{
		stateTimer++;
		if (isResting)
		{
			NPC.velocity *= 0.8f;
			if (stateTimer >= stateDuration)
			{
				isResting = false;
				stateTimer = 0;
				stateDuration = Main.rand.Next(180, 361);
			}
			return;
		}

		NPC.velocity = Vector2.Lerp(NPC.velocity, surfaceTangent * NPC.direction * IdleSpeed, 0.15f);
		if (stateTimer >= stateDuration)
		{
			isResting = true;
			stateTimer = 0;
			stateDuration = Main.rand.Next(45, 121);
		}
	}

	private void UpdateAngryMovement()
	{
		NPC.TargetClosest(faceTarget: false);
		if (!NPC.HasValidTarget)
		{
			isAngry = false;
			return;
		}

		Player target = Main.player[NPC.target];
		Vector2 toTarget = target.Center - NPC.Center;
		if (toTarget.Length() <= LaunchRange)
		{
			LaunchAtTarget(toTarget);
			return;
		}
		if (toTarget.LengthSquared() > 0.01f)
		{
			NPC.direction = Vector2.Dot(toTarget, surfaceTangent) >= 0f ? 1 : -1;
		}

		Vector2 desiredVelocity = surfaceTangent * NPC.direction * AngrySpeed;
		Vector2 nextCenter = NPC.Center + desiredVelocity;
		if (IsBlocked(nextCenter) && jumpCooldown == 0)
		{
			LaunchAtTarget(toTarget);
			return;
		}

		NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 0.25f);
	}

	private void UpdateProjectile()
	{
		NPC.velocity.Y = Math.Min(NPC.velocity.Y + 0.35f, 12f);
		NPC.rotation += 0.22f * Math.Sign(NPC.velocity.X == 0f ? 1f : NPC.velocity.X);
		Vector2 nextCenter = NPC.Center + NPC.velocity;
		if (Collision.SolidCollision(nextCenter - NPC.Size * 0.5f, NPC.width, NPC.height))
		{
			NPC.Center = nextCenter;
			NPC.velocity = Vector2.Zero;
			NPC.noGravity = true;
			NPC.noTileCollide = true;
			isLaunched = false;
			isImpactResting = true;
			stateTimer = 0;
			stateDuration = Main.rand.Next(35, 75);
			TryAttachToSurface(true);
		}
	}

	private void UpdateImpactRest()
	{
		NPC.velocity *= 0.8f;
		NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
		if (++stateTimer < stateDuration)
		{
			return;
		}

		isImpactResting = false;
		jumpCooldown = 45;
		TryAttachToSurface(true);
	}

	private void LaunchAtTarget(Vector2 toTarget)
	{
		isLaunched = true;
		isResting = false;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.velocity = toTarget.SafeNormalize(-Vector2.UnitY) * LaunchSpeed;
	}

	private void DetachAndFall()
	{
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.velocity += Vector2.UnitY * 0.4f;
		NPC.rotation = NPC.rotation.AngleTowards(0f, 0.08f);
	}

	private bool TryAttachToSurface(bool snap)
	{
		Vector2 detectedNormal = FindSurfaceNormal(NPC.Center);
		if (detectedNormal == Vector2.Zero)
		{
			return false;
		}
		if (!snap && MathF.Abs(surfaceNormal.Y) < 0.35f && detectedNormal.Y > 0.65f && Main.rand.NextBool())
		{
			return false;
		}

		surfaceNormal = Vector2.Lerp(surfaceNormal, detectedNormal, snap ? 1f : 0.35f).SafeNormalize(detectedNormal);
		surfaceTangent = surfaceNormal.RotatedBy(MathHelper.PiOver2);
		NPC.rotation = surfaceNormal.ToRotation() + MathHelper.PiOver2;
		NPC.spriteDirection = NPC.direction;

		if (snap)
		{
			MoveCollisionBoxToSurface();
		}
		return true;
	}

	private void MoveCollisionBoxToSurface()
	{
		Vector2 center = NPC.Center;
		Vector2 towardSurface = -surfaceNormal;
		bool startsInside = IsBodySolid(center);
		Vector2 freePoint = center;
		Vector2 solidPoint = center;

		if (startsInside)
		{
			for (int i = 0; i < 64 && IsBodySolid(solidPoint); i++)
			{
				solidPoint += surfaceNormal;
			}
			freePoint = solidPoint;
			solidPoint -= surfaceNormal;
		}
		else
		{
			for (int i = 0; i < 64 && !IsBodySolid(solidPoint); i++)
			{
				freePoint = solidPoint;
				solidPoint += towardSurface;
			}
		}

		if (!IsBodySolid(solidPoint))
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			Vector2 middle = (freePoint + solidPoint) * 0.5f;
			if (IsBodySolid(middle))
			{
				solidPoint = middle;
			}
			else
			{
				freePoint = middle;
			}
		}
		NPC.Center = freePoint;
	}

	private bool IsBodySolid(Vector2 center)
	{
		return Collision.SolidCollision(center - NPC.Size * 0.5f, NPC.width, NPC.height);
	}

	private Vector2 FindSurfaceNormal(Vector2 center)
	{
		Vector2 normal = Vector2.Zero;
		const int sampleCount = 16;
		for (int i = 0; i < sampleCount; i++)
		{
			Vector2 radial = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / sampleCount);
			if (Collision.SolidCollision(center + radial * SurfaceProbeDistance, 2, 2))
			{
				normal -= radial;
			}
		}
		return normal.SafeNormalize(Vector2.Zero);
	}

	private bool IsBlocked(Vector2 center)
	{
		return Collision.SolidCollision(center - surfaceNormal * 8f, NPC.width, NPC.height);
	}
}
