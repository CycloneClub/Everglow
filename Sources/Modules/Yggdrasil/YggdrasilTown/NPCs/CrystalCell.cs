using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CrystalCell : ModNPC
{
	private const float IdleSpeed = 1.4f;
	private const float AngrySpeed = 2.5f;
	private static readonly float LaunchRange = 360f;
	private static readonly float LaunchSpeed = 10f;
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
	private Vector2 routeTarget;

	private const float RouteStep = 16f;
	private const float RouteSearchDistance = 720f;

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
		Lighting.AddLight(NPC.Center, new Vector3(0.05f, 0.35f, 0.85f));
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
		isAngry = true;
		isResting = false;
		stateTimer = 0;
		stateDuration = 0;
		routeTimer = 0;
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
		//if (toTarget.Length() <= LaunchRange)
		//{
		//	LaunchAtTarget(toTarget);
		//	return;
		//}

		if (routeTimer <= 0 || Vector2.DistanceSquared(routeTarget, target.Center) > 64f * 64f)
		{
			SurfaceRoute forwardRoute = EvaluateSurfaceRoute(target.Center, 1);
			SurfaceRoute backwardRoute = EvaluateSurfaceRoute(target.Center, -1);
			SurfaceRoute selectedRoute = SelectSurfaceRoute(forwardRoute, backwardRoute);
			//if (!selectedRoute.CanContinue)
			//{
			//	LaunchAtTarget(toTarget);
			//	return;
			//}

			routeDirection = selectedRoute.Direction;
			routeTarget = target.Center;
			routeTimer = 15;
		}
		routeTimer--;
		NPC.direction = routeDirection;
		NPC.spriteDirection = NPC.direction;

		Vector2 desiredVelocity = surfaceTangent * NPC.direction * AngrySpeed;
		if (isInTheAir)
		{
			NPC.velocity.Y = Math.Min(NPC.velocity.Y + Gravity, 12f);
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
			CanContinue = true,
		};
		List<Vector2> visited = new List<Vector2>();
		Vector2 position = NPC.Center;
		Vector2 normal = surfaceNormal;
		Vector2 travelTangent = normal.RotatedBy(MathHelper.PiOver2) * direction;

		for (float distance = 0f; distance < RouteSearchDistance; distance += RouteStep)
		{
			Vector2 nextPosition = position + travelTangent * RouteStep;
			Vector2 nextNormal = ProbeSurfaceNormal(nextPosition, normal.ToRotation() + MathHelper.PiOver2, out bool inAir);
			if (inAir)
			{
				route.CanContinue = false;
				route.Ended = true;
				break;
			}

			Vector2 nextTangent = nextNormal.RotatedBy(MathHelper.PiOver2);
			if (Vector2.Dot(nextTangent, travelTangent) < 0f)
			{
				nextTangent *= -1f;
			}

			position = nextPosition;
			normal = nextNormal;
			travelTangent = nextTangent;
			visited.Add(position);

			float targetDistance = Vector2.Distance(position, target);
			if (targetDistance < route.BestDistance)
			{
				route.BestDistance = targetDistance;
			}

			for (int i = 0; i < visited.Count - 1; i++)
			{
				if (Vector2.DistanceSquared(position, visited[i]) < RouteStep * RouteStep * 0.75f)
				{
					route.IsLoop = true;
					route.CanContinue = false;
					return route;
				}
			}
		}

		route.SearchExhausted = route.CanContinue;
		if (route.SearchExhausted && route.BestDistance > LaunchRange)
		{
			route.CanContinue = false;
		}
		return route;
	}

	private SurfaceRoute SelectSurfaceRoute(SurfaceRoute first, SurfaceRoute second)
	{
		if (first.CanContinue && !second.CanContinue)
		{
			return first;
		}
		if (second.CanContinue && !first.CanContinue)
		{
			return second;
		}
		if (!first.CanContinue && !second.CanContinue)
		{
			return first;
		}
		return first.BestDistance <= second.BestDistance ? first : second;
	}

	private struct SurfaceRoute
	{
		public int Direction;
		public float BestDistance;
		public bool CanContinue;
		public bool Ended;
		public bool IsLoop;
		public bool SearchExhausted;
	}

	private void UpdateProjectile()
	{
		NPC.velocity.Y = Math.Min(NPC.velocity.Y + Gravity * 0.25f, 12f);
		NPC.rotation += 0.22f * Math.Sign(NPC.velocity.X == 0f ? 1f : NPC.velocity.X);
		Vector2 nextCenter = NPC.Center + NPC.velocity;
		if (ProjectileHitsSolid(NPC.Center, nextCenter))
		{
			NPC.Center = nextCenter;
			NPC.velocity = Vector2.Zero;
			NPC.noGravity = true;
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
		Vector2 surfaceNormal = FindSurfaceNormal(NPC.Center);
		Vector2 launchDirection = toTarget.SafeNormalize(-Vector2.UnitY);
		if (Vector2.Dot(surfaceNormal, launchDirection) <= MathF.Sqrt(2))
		{
			launchDirection = launchDirection * 0.5f + surfaceNormal * 0.5f;
		}
		NPC.velocity = launchDirection * LaunchSpeed;
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
			for (int i = 0; i < 4 && !IsSupportLineSolid(solidPoint, surfaceNormal); i++)
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
			Vector2 arrow = new Vector2(0, 16).RotatedBy(i / (float)normalCount * MathHelper.TwoPi);
			if (Collision.IsWorldPointSolid(point + arrow))
			{
				bestNormal -= arrow;
			}
			else
			{
				bestNormal += arrow;
				airCount++;
			}
			// Vector2 normal = Vector2.UnitX.RotatedBy(MathHelper.TwoPi * i / normalCount);
			// Vector2 tangent = normal.RotatedBy(MathHelper.PiOver2);
			// int score = 0;
			// for (int j = 0; j <= 8; j++)
			// {
			// Vector2 point = center - normal * 16f + tangent * (j / 8f * 32f - 16f);
			// if (Collision.IsWorldPointSolid(point))
			// {
			// score++;
			// }
			// }
			// if (score > bestScore)
			// {
			// bestScore = score;
			// bestNormal = normal;
			// }
		}
		// Main.NewText(bestNormal);
		inAir = airCount >= normalCount;
		return bestNormal.Length() < 0.001f ? new Vector2(0, -1) : bestNormal.NormalizeSafe();
	}

	private bool IsBlocked(Vector2 center)
	{
		return !IsSupportLineSolid(center, surfaceNormal);
	}

	private bool ProjectileHitsSolid(Vector2 start, Vector2 end)
	{
		Vector2 velocity = end - start;
		Vector2 forward = velocity.SafeNormalize(Vector2.UnitY);
		Vector2 side = forward.RotatedBy(MathHelper.PiOver2);
		int steps = Math.Max(1, (int)(velocity.Length() / 2f));
		for (int i = 1; i <= steps; i++)
		{
			Vector2 center = Vector2.Lerp(start, end, i / (float)steps);
			for (int axis = -1; axis <= 1; axis++)
			{
				for (int sideOffset = -1; sideOffset <= 1; sideOffset++)
				{
					Vector2 point = center + forward * (axis * 16f) + side * (sideOffset * 16f);
					if (Collision.IsWorldPointSolid(point))
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		// Texture2D tex = Commons.ModAsset.SparkLight.Value;
		// Vector2 mousePos = Vector2.Transform(Main.MouseWorld, Matrix.Invert(spriteBatch.transformMatrix) * Main.GameViewMatrix.TransformationMatrix);
		// float rotation = FindSurfaceNormal(mousePos).ToRotationSafe();
		// spriteBatch.Draw(tex, mousePos - Main.screenPosition, null, new Color(1f, 1f, 1f, 0), rotation, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
