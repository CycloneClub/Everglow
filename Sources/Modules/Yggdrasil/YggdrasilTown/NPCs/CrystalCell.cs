using Everglow.Yggdrasil.YggdrasilTown.Dusts.TwilightForest;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CrystalCell : ModNPC
{
	private const float IdleSpeed = 1.4f;
	private const float AngrySpeed = 2.5f;
	private const float Gravity = 0.35f;

	private Vector2 surfaceNormal = -Vector2.UnitY;
	private Vector2 surfaceTangent = Vector2.UnitX;
	private bool isResting;
	private bool isAngry;
	private bool isLaunched;
	private bool isImpactResting;
	private bool isInTheAir;
	private int stateTimer;
	private int stateDuration;
	private int jumpCooldown;
	private int routeTimer;
	private int routeDirection;
	private int hitTimer;
	private Vector2 routeTarget;
	private Vector2 bestPointInRoute;

	private float routeStep = 8f;
	private const float RouteSearchDistance = 720f;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
		NPCID.Sets.SpecificDebuffImmunity[Type][BuffID.Confused] = true;
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
		NPC.noGravity = false;
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
		bool walking = !isLaunched && !isImpactResting && NPC.noGravity && NPC.velocity.LengthSquared() >= 0.05f;
		if (!walking)
		{
			NPC.frame.Y = 0;
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
		Vector3 lightColor = new Vector3(0.05f, 0.35f, 0.85f);
		if (hitTimer > 0)
		{
			hitTimer--;
			lightColor = Vector3.Lerp(lightColor, new Vector3(1f, 1f, 1f), hitTimer / 30f);
		}
		Lighting.AddLight(NPC.Center, lightColor);
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
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (!isAngry)
		{
			isImpactResting = true;
			stateDuration = 60;
		}
		else
		{
			stateDuration = 0;
		}
		isAngry = true;
		isResting = false;
		stateTimer = 0;
		routeTimer = 0;
		hitTimer = 30;
	}

	private void UpdateNeutralMovement()
	{
		stateTimer++;
		if (isResting)
		{
			NPC.velocity.X *= 0.8f;
			if (stateTimer >= stateDuration)
			{
				isResting = false;
				stateTimer = 0;
				stateDuration = Main.rand.Next(180, 361);
			}
			return;
		}

		if (isInTheAir)
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + Gravity, 12f);
		}
		else
		{
			NPC.velocity = Vector2.Lerp(NPC.velocity, surfaceTangent * NPC.direction * IdleSpeed, 0.15f);
		}
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
		if (toTarget.Length() <= 240 && Collision.CanHit(NPC, target) && CanLaunch(NPC.Center, toTarget.NormalizeSafe() * 8))
		{
			LaunchAtTarget(toTarget);
			return;
		}

		if (routeTimer <= 0 || Vector2.Distance(routeTarget, target.Center) > 128f)
		{
			SurfaceRoute forwardRoute = EvaluateSurfaceRoute(target.Center, 1);
			SurfaceRoute backwardRoute = EvaluateSurfaceRoute(target.Center, -1);
			SurfaceRoute selectedRoute = SelectSurfaceRoute(forwardRoute, backwardRoute);
			if (Math.Abs(forwardRoute.BestDistance - backwardRoute.BestDistance) < 3 && CanLaunch(NPC.Center, surfaceNormal * 8))
			{
				LaunchTowardNormal();
				return;
			}
			// if (!selectedRoute.CanContinue)
			// {
			// LaunchAtTarget(toTarget);
			// return;
			// }

			routeDirection = selectedRoute.Direction;
			routeTarget = target.Center;
			routeTimer = 60;
			bestPointInRoute = selectedRoute.BestDistancePoint;
		}
		routeTimer--;
		NPC.direction = routeDirection;
		NPC.spriteDirection = NPC.direction;
		Vector2 desiredVelocity = surfaceTangent * NPC.direction * AngrySpeed;
		if (isInTheAir)
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + Gravity, 12f);
		}
		else if ((NPC.Center - bestPointInRoute).Length() < 15f && CanLaunch(NPC.Center, surfaceNormal * 8))
		{
			LaunchTowardNormal();
			isInTheAir = true;
		}
		else
		{
			NPC.velocity = Vector2.Lerp(NPC.velocity, desiredVelocity, 0.25f);
		}
	}

	private SurfaceRoute EvaluateSurfaceRoute(Vector2 target, int direction)
	{
		SurfaceRoute route = new SurfaceRoute
		{
			Direction = direction,
			BestDistance = Vector2.Distance(NPC.Center, target),
		};
		List<Vector2> visited = new List<Vector2>();
		Vector2 position = NPC.Center;
		Vector2 normal = surfaceNormal;
		Vector2 travelTangent = normal.RotatedBy(MathHelper.PiOver2) * direction;

		for (float distance = 0f; distance < RouteSearchDistance; distance += routeStep)
		{
			Vector2 nextPosition = position + travelTangent * routeStep;
			nextPosition = MovePositionToSurface(nextPosition);
			Vector2 nextNormal = ProbeSurfaceNormal(nextPosition, normal.ToRotation() + MathHelper.PiOver2, out bool inAir);
			if (inAir)
			{
				break;
			}

			Vector2 nextTangent = nextNormal.RotatedBy(MathHelper.PiOver2) * direction;
			// if (Vector2.Dot(nextTangent, travelTangent) < 0f)
			// {
			// nextTangent *= -1f;
			// }

			position = nextPosition;
			normal = nextNormal;
			travelTangent = nextTangent;
			visited.Add(position);

			float targetDistance = Vector2.Distance(position, target);
			if (targetDistance < route.BestDistance)
			{
				route.BestDistance = targetDistance;
				route.BestDistancePoint = position;
			}

			// if (direction == -1)
			// {
			// Dust d0 = Dust.NewDustDirect(position, 0, 0, DustID.RedTorch);
			// d0.noGravity = true;
			// d0.scale = 3f;
			// d0.velocity *= 0;
			// }
			// else
			// {
			// Dust d0 = Dust.NewDustDirect(position, 0, 0, DustID.BlueTorch);
			// d0.noGravity = true;
			// d0.scale = 3f;
			// d0.velocity *= 0;
			// }
		}
		// if (direction == -1)
		// {
		// Main.NewText(route.BestDistance, Color.Red);
		// }
		// else
		// {
		// Main.NewText(route.BestDistance, Color.Blue);
		// }
		return route;
	}

	private SurfaceRoute SelectSurfaceRoute(SurfaceRoute first, SurfaceRoute second)
	{
		return first.BestDistance <= second.BestDistance ? first : second;
	}

	private struct SurfaceRoute
	{
		public int Direction;
		public float BestDistance;

		public Vector2 BestDistancePoint;
	}

	private void UpdateProjectile()
	{
		NPC.velocity.Y = Math.Min(NPC.velocity.Y + Gravity * 0.25f, 12f);
		NPC.rotation += 0.22f * Math.Sign(NPC.velocity.X == 0f ? 1f : NPC.velocity.X);
		Vector2 nextCenter = NPC.Center + NPC.velocity;
		if (ProjectileHitsSolid(NPC.Center, nextCenter))
		{
			NPC.velocity = Vector2.Zero;
			NPC.noGravity = true;
			isLaunched = false;
			isImpactResting = true;
			stateTimer = 0;
			stateDuration = Main.rand.Next(35, 75);
			TryAttachToSurfaceFar();
		}
	}

	private void UpdateImpactRest()
	{
		NPC.velocity *= 0.8f;
		if (stateTimer < 5)
		{
			TryAttachToSurface(true);
		}
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
		Vector2 surfaceNormal = FindSurfaceNormal(NPC.Center);
		toTarget += new Vector2(0, Main.rand.NextFloat(-40, -10));
		Vector2 launchDirection = toTarget.SafeNormalize(-Vector2.UnitY);
		if (Vector2.Dot(surfaceNormal, launchDirection) <= MathF.Sqrt(2))
		{
			launchDirection = launchDirection * 0.5f + surfaceNormal * 0.5f;
		}
		NPC.velocity = launchDirection * 15;
	}

	private void LaunchTowardNormal()
	{
		isLaunched = true;
		isResting = false;
		NPC.noGravity = false;
		Vector2 surfaceNormal = FindSurfaceNormal(NPC.Center);
		Vector2 launchDirection = surfaceNormal.SafeNormalize(-Vector2.UnitY);
		if (Vector2.Dot(surfaceNormal, launchDirection) <= MathF.Sqrt(2))
		{
			launchDirection = launchDirection * 0.5f + surfaceNormal * 0.5f;
		}
		NPC.velocity = launchDirection * 9;
	}

	private void DetachAndFall()
	{
		NPC.noGravity = false;
		NPC.velocity += Vector2.UnitY * Gravity;
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
		NPC.noGravity = true;

		NPC.rotation = surfaceNormal.ToRotation() + MathHelper.PiOver2;
		NPC.spriteDirection = NPC.direction;

		MoveCollisionBoxToSurface();
		return true;
	}

	private bool TryAttachToSurfaceFar()
	{
		Vector2 detectedNormal = ProbeSurfaceNormalRough(NPC.Center);
		if (detectedNormal == Vector2.Zero)
		{
			return false;
		}

		surfaceNormal = detectedNormal.SafeNormalize(detectedNormal);
		surfaceTangent = surfaceNormal.RotatedBy(MathHelper.PiOver2);
		NPC.noGravity = true;

		NPC.rotation = surfaceNormal.ToRotation() + MathHelper.PiOver2;
		NPC.spriteDirection = NPC.direction;

		MoveCollisionBoxToSurface();
		return true;
	}

	private void MoveCollisionBoxToSurface()
	{
		Vector2 center = NPC.Center;
		Vector2 towardSurface = -surfaceNormal;
		bool startsInside = IsSupportLineSolid(center, surfaceNormal);
		Vector2 freePoint = center;
		Vector2 solidPoint = center;

		if (startsInside)
		{
			for (int i = 0; i < 4 && IsSupportLineSolid(solidPoint, surfaceNormal); i++)
			{
				solidPoint += surfaceNormal;
			}
			freePoint = solidPoint;
			solidPoint -= surfaceNormal;
		}
		else
		{
			for (int i = 0; i < 8 && !IsSupportLineSolid(solidPoint, surfaceNormal); i++)
			{
				freePoint = solidPoint;
				solidPoint += towardSurface;
			}
		}

		if (!IsSupportLineSolid(solidPoint, surfaceNormal))
		{
			return;
		}

		for (int i = 0; i < 6; i++)
		{
			Vector2 middle = (freePoint + solidPoint) * 0.5f;
			if (IsSupportLineSolid(middle, surfaceNormal))
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

	private Vector2 MovePositionToSurface(Vector2 position)
	{
		Vector2 center = position;
		Vector2 normal = ProbeSurfaceNormalRough(center);
		Vector2 towardSurface = -normal;
		bool startsInside = IsSupportLineSolid(center, normal);
		Vector2 freePoint = center;
		Vector2 solidPoint = center;

		if (startsInside)
		{
			for (int i = 0; i < 4 && IsSupportLineSolid(solidPoint, normal); i++)
			{
				solidPoint += normal;
			}
			freePoint = solidPoint;
			solidPoint -= normal;
		}
		else
		{
			for (int i = 0; i < 4 && !IsSupportLineSolid(solidPoint, normal); i++)
			{
				freePoint = solidPoint;
				solidPoint += towardSurface;
			}
		}

		if (!IsSupportLineSolid(solidPoint, normal))
		{
			return position;
		}

		for (int i = 0; i < 6; i++)
		{
			Vector2 middle = (freePoint + solidPoint) * 0.5f;
			if (IsSupportLineSolid(middle, normal))
			{
				solidPoint = middle;
			}
			else
			{
				freePoint = middle;
			}
		}
		return freePoint;
	}

	private bool IsSupportLineSolid(Vector2 center, Vector2 normal)
	{
		Vector2 tangent = normal.RotatedBy(MathHelper.PiOver2);
		for (int i = 0; i <= 3; i++)
		{
			Vector2 point = center - normal * 16f + tangent * (i / 3f * 32f - 16f);
			if (Collision.IsWorldPointSolid(point))
			{
				return true;
			}
		}
		return false;
	}

	private Vector2 FindSurfaceNormal(Vector2 center)
	{
		return ProbeSurfaceNormal(center, NPC.rotation, out isInTheAir);
	}

	private Vector2 ProbeSurfaceNormal(Vector2 center, float rotation, out bool inAir)
	{
		Vector2 bestNormal = Vector2.zeroVector;
		const int normalCount = 16;
		int airCount = 0;
		for (int i = 0; i < normalCount; i++)
		{
			Vector2 point = center + new Vector2(0, 16).RotatedBy(rotation);
			Vector2 arrow = new Vector2(0, 16).RotatedBy((i + 0.5f) / normalCount * MathHelper.TwoPi);
			if (Collision.IsWorldPointSolid(point + arrow))
			{
				bestNormal -= arrow;
			}
			else
			{
				bestNormal += arrow;
				airCount++;
			}
		}
		inAir = airCount >= normalCount;
		return bestNormal.Length() < 0.001f ? new Vector2(0, -1) : bestNormal.NormalizeSafe();
	}

	private Vector2 ProbeSurfaceNormalRough(Vector2 center)
	{
		Vector2 bestNormal = Vector2.zeroVector;
		const int normalCount = 16;
		int airCount = 0;
		for (int i = 0; i < normalCount; i++)
		{
			Vector2 point = center;
			Vector2 arrow = new Vector2(0, 24).RotatedBy((i + 0.5f) / normalCount * MathHelper.TwoPi);
			if (Collision.IsWorldPointSolid(point + arrow))
			{
				bestNormal -= arrow;
			}
			else
			{
				bestNormal += arrow;
				airCount++;
			}
		}
		return bestNormal.Length() < 0.001f ? new Vector2(0, -1) : bestNormal.NormalizeSafe();
	}

	private bool CanLaunch(Vector2 pos, Vector2 vel)
	{
		return !ProjectileHitsSolid(pos + vel, pos + vel * 2);
	}

	private bool ProjectileHitsSolid(Vector2 start, Vector2 end)
	{
		Vector2 velocity = end - start;
		Vector2 forward = velocity.SafeNormalize(Vector2.UnitY);
		int steps = Math.Max(1, (int)(velocity.Length() / 2f));
		for (int i = 1; i <= steps; i++)
		{
			Vector2 center = Vector2.Lerp(start, end, i / (float)steps);
			if (Collision.SolidCollision(center - new Vector2(8), 16, 16))
			{
				return true;
			}
		}
		return false;
	}

	public override void OnKill()
	{
		for (int j = 0; j < 4; j++)
		{
			int type = ModContent.Find<ModGore>("Everglow/CrystalCell_Gore_" + j).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, new Vector2(0, 3).RotatedByRandom(MathHelper.TwoPi), type, NPC.scale);
		}
		for (int j = 0; j < 3; j++)
		{
			NPC.NewNPCDirect(NPC.GetSource_Death(), NPC.Center, ModContent.NPCType<CrystalCell_small>());
		}
		for (int h = 0; h < 30; h++)
		{
			Dust dust = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<TwilightCrystalDust>());
			dust.velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
		}
		for (int h = 0; h < 12; h++)
		{
			Vector2 velocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat()) * 6).RotatedByRandom(MathHelper.TwoPi);
			var dustVFX = new TwilightCrystalVFXDust
			{
				Velocity = velocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + velocity * 6,
				MaxTime = Main.rand.Next(24, 42),
				Scale = Main.rand.NextFloat(3, 12),
				Rotation = velocity.ToRotation() - MathHelper.PiOver4,
				ai = new float[] { 0, 0, 0 },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D glow = ModAsset.CrystalCell_glow.Value;
		spriteBatch.Draw(glow, NPC.Center + new Vector2(0, NPC.gfxOffY) - screenPos, NPC.frame, new Color(1f, 1f, 1f, 0) * (hitTimer / 30f), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
