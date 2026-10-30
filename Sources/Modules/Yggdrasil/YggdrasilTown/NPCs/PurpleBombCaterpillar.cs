using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.NPCEffects;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class PurpleBombCaterpillar : ModNPC
{
	private const int BodyLength = 98;
	private const int BodyHeight = 36;
	private const int BodySegments = 40;
	private const int BodyPathSamples = 128;
	private const int MaximumGapWidth = 18;
	private const float CrawlPeriod = 60f;
	private const float MaximumStepHeight = 18f;
	private const float BodyInset = 2f;

	public float MoveTimer;
	public float MoveScale = 1f;
	public float GlowValue = 1f;
	public bool Angry;

	private float surfaceRadius = BodyHeight * 0.5f + 1;
	private bool attached;
	private Vector2 surfacePosition;
	private Vector2 surfaceNormal = -Vector2.UnitY;
	private float crawlSpeed;
	private int detachTimer;
	private bool obstacleTurnPending;

	private readonly List<Vector2> bodyFeet = new();
	private Vector2[] bodyTops = Array.Empty<Vector2>();
	private float[] bellySag = Array.Empty<float>();
	private Vector2[] previousBellyFeet = Array.Empty<Vector2>();

	public IReadOnlyList<Vector2> BodyFeet => bodyFeet;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 10;
	}

	public override void SetDefaults()
	{
		// 脱落时使用紧凑碰撞箱；贴附时由曲线身体计算受击范围。
		NPC.width = BodyHeight;
		NPC.height = BodyHeight;
		NPC.lifeMax = 80;
		NPC.damage = 25;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		MoveTimer = 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
		NPC.direction = Main.rand.NextBool() ? 1 : -1;
	}

	public override void AI()
	{
		if (Angry)
		{
			NPC.TargetClosest();
			if (NPC.HasValidTarget)
			{
				NPC.FaceTarget();
			}
		}
		GlowValue = Angry ? 1f : 0.5f;
		MoveScale = Angry ? 1f : 0.3f;
		MoveTimer++;
		UpdateMoveAndLight();
	}

	public void UpdateMoveAndLight()
	{
		TryAttachToGround();
		UpdateCrawlCycle();
		if (attached)
		{
			MoveAlongGround();
		}
		else
		{
			NPC.velocity *= 0.999f;
		}

		UpdateRotation();
		// 形变后的头部也要检查，确保伸展动作停在真实墙面。
		if (attached && MathF.Abs(crawlSpeed) > 0.01f && Math.Sign(crawlSpeed) == NPC.spriteDirection && HeadMeetsHighObstacle())
		{
			StopAtObstacle();
		}
		if (!Main.dedServ)
		{
			Lighting.AddLight(NPC.Center, new Vector3(1f, 0.05f, 0.6f) * GlowValue);
		}
	}

	private void UpdateCrawlCycle()
	{
		if (MoveTimer >= CrawlPeriod)
		{
			MoveTimer %= CrawlPeriod;
			NPC.netUpdate = true;
		}
		// 每帧推进一刻度；一刻度宽的触发窗口兼容小数相位。
		if (!attached || MoveTimer is < 15 or >= 16)
		{
			return;
		}
		if (obstacleTurnPending)
		{
			NPC.direction = -NPC.spriteDirection;
			obstacleTurnPending = false;
			NPC.netUpdate = true;
		}
		else if (Main.netMode != NetmodeID.MultiplayerClient && NPC.target < 0 && Main.rand.NextBool(15))
		{
			NPC.direction *= -1;
			NPC.netUpdate = true;
		}
		NPC.spriteDirection = NPC.direction;
	}

	private void MoveAlongGround()
	{
		if (MoveTimer is >= 30 and < 31)
		{
			crawlSpeed += NPC.spriteDirection * 6 * MoveScale * NPC.scale;
		}
		else
		{
			crawlSpeed *= 0.95f;
		}

		float radius = GetBodyRadius();
		// 蠕动改变厚度时同步移动支点，保持腹部贴面。
		surfacePosition.Y -= radius - surfaceRadius;
		surfaceRadius = radius;
		if (bodyFeet.Count > 0 && Math.Sign(crawlSpeed) == NPC.spriteDirection && HeadMeetsHighObstacle())
		{
			StopAtObstacle();
		}
		GroundMoveResult result = AdvanceSurface(ref surfacePosition, ref surfaceNormal, crawlSpeed, surfaceRadius);
		if (result == GroundMoveResult.Unsupported)
		{
			DetachFromSurface();
			return;
		}
		if (result == GroundMoveResult.Blocked)
		{
			StopAtObstacle();
		}
		NPC.velocity = Vector2.Zero;
	}

	private void TryAttachToGround()
	{
		if (attached)
		{
			return;
		}
		if (detachTimer > 0)
		{
			detachTimer--;
			return;
		}
		if (NPC.velocity.Y < 0)
		{
			return;
		}
		float radius = GetBodyRadius();
		// 先从脚底找支撑，避免大体型靠墙落地时将更近的墙误当作唯一接触面。
		Vector2 position = NPC.Bottom - new Vector2(0, 2);
		Vector3 sample = SampleSurface(position);
		if (sample.X < 0 || sample.X > 5 || sample.Z > -0.5f)
		{
			return;
		}
		surfaceNormal = new Vector2(sample.Y, sample.Z);
		surfacePosition = position + surfaceNormal * (radius - sample.X);
		KeepOutsideTerrain(ref surfacePosition, radius);
		surfaceRadius = radius;
		crawlSpeed = NPC.velocity.X;
		NPC.spriteDirection = NPC.direction == 0 ? 1 : NPC.direction;
		attached = NPC.noGravity = NPC.noTileCollide = true;
		NPC.netUpdate = true;
	}

	private void DetachFromSurface()
	{
		// 恢复轴对齐的下落碰撞箱时，补足其沿接触法线的半宽，避免小体型嵌墙。
		float clearance = 18 * (MathF.Abs(surfaceNormal.X) + MathF.Abs(surfaceNormal.Y));
		Vector2 center = surfacePosition + surfaceNormal * Math.Max(0, clearance - surfaceRadius);
		NPC.width = NPC.height = BodyHeight;
		NPC.Center = center;
		NPC.velocity = new Vector2(-surfaceNormal.Y, surfaceNormal.X) * crawlSpeed + surfaceNormal * 3;
		NPC.velocity.Y = Math.Max(2, NPC.velocity.Y);
		attached = NPC.noGravity = NPC.noTileCollide = false;
		obstacleTurnPending = false;
		detachTimer = 45;
		NPC.netUpdate = true;
	}

	private enum GroundMoveResult
	{
		Moved,
		Blocked,
		Unsupported,
	}

	// 只沿地面上边界推进。无法容纳身体的高障碍返回 Blocked，不再绕到墙面或顶面。
	private static GroundMoveResult AdvanceSurface(ref Vector2 position, ref Vector2 normal, float distance, float radius)
	{
		int steps = Math.Max(1, (int)MathF.Ceiling(MathF.Abs(distance) / 0.75f));
		float step = distance / steps;
		for (int i = 0; i < steps; i++)
		{
			Vector2 candidate = position + new Vector2(step, 0);
			int range = Math.Max(1, (int)MathF.Ceiling((radius + 3) / 16));
			if (step != 0 && SampleSurface(candidate, range).X >= radius - 0.05f && CanBridgeGap(candidate, Math.Sign(step), radius))
			{
				position = candidate;
				normal = -Vector2.UnitY;
				continue;
			}
			GroundMoveResult result = FindGround(candidate.X, position.Y, radius, out Vector2 ground, out Vector2 groundNormal);
			if (result != GroundMoveResult.Moved)
			{
				return result;
			}
			Vector2 oldSupport = position - normal * radius;
			Vector2 newSupport = ground - groundNormal * radius;
			// 看真实支撑面的落差，而非圆心落差。两格直坎应自然掉落，
			// 不能因为探针仍挨着上沿，就把下层地面当成连续贴地路径。
			if (newSupport.Y - oldSupport.Y > MaximumStepHeight
				&& !HasWalkableStepConnection(oldSupport.X, oldSupport.Y, newSupport.X, newSupport.Y))
			{
				return GroundMoveResult.Unsupported;
			}
			position = ground;
			normal = groundNormal;
		}
		return GroundMoveResult.Moved;
	}

	private static GroundMoveResult FindGround(float x, float previousY, float radius, out Vector2 position, out Vector2 normal, float horizontalRadius = 0)
	{
		horizontalRadius = horizontalRadius > 0 ? horizontalRadius : radius;
		position = new Vector2(x, previousY);
		normal = -Vector2.UnitY;
		float bestY = float.MaxValue;
		float highestSurface = previousY + radius - MaximumStepHeight; // 一格台阶，加两像素的圆角过渡余量。
		int left = Math.Max(0, (int)MathF.Floor((x - horizontalRadius) / 16));
		int right = Math.Min(Main.maxTilesX - 1, (int)MathF.Floor((x + horizontalRadius) / 16));
		// 细探针也必须向上覆盖可迈的一格，否则会漏掉斜面顶端的下一层物块。
		int top = Math.Max(0, (int)MathF.Floor(Math.Min(previousY - radius, highestSurface) / 16));
		int bottom = Math.Min(Main.maxTilesY - 1, (int)MathF.Floor((previousY + radius + 32) / 16));
		for (int tx = left; tx <= right; tx++)
		{
			for (int ty = top; ty <= bottom; ty++)
			{
				Tile tile = Main.tile[tx, ty];
				if (!tile.HasTile || tile.IsActuated || (!Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
				{
					continue;
				}
				float surfaceY = ty * 16 + (tile.IsHalfBlock ? 8 : 0);
				if (Main.tileSolidTop[tile.TileType] && (tile.TileFrameY != 0 || surfaceY < previousY))
				{
					continue;
				}
				float slope = tile.Slope == SlopeType.SlopeDownRight ? -1 : tile.Slope == SlopeType.SlopeDownLeft ? 1 : 0;
				float contactX = MathHelper.Clamp(x - slope * horizontalRadius * horizontalRadius / MathF.Sqrt(radius * radius + slope * slope * horizontalRadius * horizontalRadius), tx * 16, tx * 16 + 16);
				if (slope != 0)
				{
					surfaceY += slope > 0 ? contactX - tx * 16 : 16 - (contactX - tx * 16);
				}
				float dx = x - contactX;
				if (MathF.Abs(dx) >= horizontalRadius)
				{
					continue;
				}
				float dy = radius * MathF.Sqrt(1 - dx * dx / (horizontalRadius * horizontalRadius));
				float candidateY = surfaceY - dy;
				if (candidateY >= bestY)
				{
					continue;
				}
				if (surfaceY < highestSurface && candidateY < previousY - 0.05f
					&& (horizontalRadius <= 1 || !HasWalkableStepConnection(x, previousY + radius, contactX, surfaceY)))
				{
					return GroundMoveResult.Blocked;
				}
				bestY = candidateY;
				normal = new Vector2(dx / (horizontalRadius * horizontalRadius), -dy / (radius * radius)).SafeNormalize(-Vector2.UnitY);
			}
		}
		if (bestY == float.MaxValue || bestY > previousY + MaximumStepHeight)
		{
			return GroundMoveResult.Unsupported;
		}
		position.Y = bestY;
		return GroundMoveResult.Moved;
	}

	private static bool HasWalkableStepConnection(float startX, float feetY, float endX, float surfaceY = float.PositiveInfinity)
	{
		// 圆探针会越过斜坡提前碰到下一格。按中间的真实地表确认每次抬升，
		// 避免把“斜坡 + 一格台阶”的累计高度当成同一面墙。
		const float probe = 0.25f;
		if (FindGround(startX, feetY - probe, probe, out Vector2 point, out _) != GroundMoveResult.Moved)
		{
			return false;
		}
		int steps = Math.Max(1, (int)MathF.Ceiling(MathF.Abs(endX - startX) / 2));
		for (int i = 1; i <= steps; i++)
		{
			float x = MathHelper.Lerp(startX, endX, i / (float)steps);
			if (FindGround(x, point.Y, probe, out point, out _) != GroundMoveResult.Moved)
			{
				return false;
			}
		}
		return point.Y + probe <= surfaceY + 0.5f;
	}

	private bool HeadMeetsHighObstacle()
	{
		int head = NPC.spriteDirection > 0 ? bodyFeet.Count - 1 : 0;
		Vector2 foot = bodyFeet[head];
		Vector2 top = bodyTops[head];
		int steps = Math.Max(1, (int)MathF.Ceiling(Vector2.Distance(top, foot) / 8));
		for (int i = 0; i <= steps; i++)
		{
			Vector2 point = Vector2.Lerp(top, foot, i / (float)steps);
			// 下缘仍可登上一格台阶；更高的障碍允许两像素浅嵌，然后停止。
			if (point.Y > foot.Y - MaximumStepHeight)
			{
				continue;
			}
			if (SampleSurface(point).X <= -BodyInset + 0.02f)
			{
				return true;
			}
		}
		return false;
	}

	private void StopAtObstacle()
	{
		if (crawlSpeed != 0)
		{
			obstacleTurnPending = true;
			NPC.netUpdate = true;
		}
		crawlSpeed = 0;
		NPC.velocity = Vector2.Zero;
	}

	private static bool CanBridgeGap(Vector2 position, int direction, float radius)
	{
		// 仅跨越一格以内的小缺口；没有落脚面时由地面采样决定继续下坡或掉落。
		Vector2 tangent = Vector2.UnitX * direction;
		for (int ahead = 2; ahead <= MaximumGapWidth; ahead += 2)
		{
			Vector3 sample = SampleSurface(position + tangent * ahead, Math.Max(1, (int)MathF.Ceiling((radius + 3) / 16)));
			if (sample.X < radius - 1)
			{
				return false;
			}
			if (sample.X <= radius + 0.02f && -sample.Z > 0.99f)
			{
				return true;
			}
		}
		return false;
	}

	private static Vector3 SampleSurface(Vector2 position, int range = 1)
	{
		Vector3 nearest = new Vector3(32, 0, -1);
		int tileX = (int)MathF.Floor(position.X / 16);
		int tileY = (int)MathF.Floor(position.Y / 16);
		for (int x = Math.Max(0, tileX - range); x <= Math.Min(Main.maxTilesX - 1, tileX + range); x++)
		{
			for (int y = Math.Max(0, tileY - range); y <= Math.Min(Main.maxTilesY - 1, tileY + range); y++)
			{
				Tile tile = Main.tile[x, y];
				if (!tile.HasTile || tile.IsActuated || (!Main.tileSolid[tile.TileType] && !Main.tileSolidTop[tile.TileType]))
				{
					continue;
				}
				Vector2 local = position - new Vector2(x * 16 + 8, y * 16 + 8);
				Vector3 sample;
				if (Main.tileSolidTop[tile.TileType])
				{
					// 平台仍是单向地面，不允许吸附平台的底面。
					if (tile.TileFrameY != 0 || local.Y > -8)
					{
						continue;
					}
					sample = SDFUtils.SdgBox(local + new Vector2(0, 8), new Vector2(8, 0));
				}
				else
				{
					sample = tile.IsHalfBlock
						? SDFUtils.SdgBox(local - new Vector2(0, 4), new Vector2(8, 4))
						: SDFUtils.SdgBox(local, new Vector2(8));
				}
				if (tile.Slope != SlopeType.Solid)
				{
					Vector2 slopeNormal = tile.Slope switch
					{
						SlopeType.SlopeDownRight => new Vector2(-1, -1),
						SlopeType.SlopeDownLeft => new Vector2(1, -1),
						SlopeType.SlopeUpRight => new Vector2(-1, 1),
						_ => new Vector2(1, 1),
					};
					slopeNormal *= MathF.Sqrt(0.5f);
					float slopeDistance = Vector2.Dot(local, slopeNormal);
					if (slopeDistance > sample.X)
					{
						sample = new Vector3(slopeDistance, slopeNormal.X, slopeNormal.Y);
					}
				}
				if (sample.X < nearest.X)
				{
					nearest = sample;
				}
			}
		}
		return nearest;
	}

	public void UpdateRotation()
	{
		float squeeze = GetSqueeze();
		float length = squeeze * BodyLength * NPC.scale;
		float height = BodyHeight * (2 - squeeze) * NPC.scale;
		Vector2[] spine = BuildBodySpine(length, height);
		var edges = BuildBodyEdges(spine, height, attached);
		bodyFeet.Clear();
		bodyFeet.AddRange(edges.Feet);
		bodyTops = edges.Tops;
		if (attached)
		{
			RelaxBelly();
		}
		else
		{
			Array.Clear(bellySag);
			previousBellyFeet = Array.Empty<Vector2>();
		}
		if (attached)
		{
			int middle = spine.Length / 2;
			NPC.rotation = (spine[middle + 1] - spine[middle - 1]).ToRotation();
			Vector2 min = bodyFeet[0];
			Vector2 max = min;
			for (int i = 0; i < spine.Length; i++)
			{
				min = Vector2.Min(min, Vector2.Min(bodyFeet[i], bodyTops[i]));
				max = Vector2.Max(max, Vector2.Max(bodyFeet[i], bodyTops[i]));
			}
			NPC.position = min;
			NPC.width = Math.Max(1, (int)MathF.Ceiling(max.X - min.X));
			NPC.height = Math.Max(1, (int)MathF.Ceiling(max.Y - min.Y));
		}
	}

	private void RelaxBelly()
	{
		if (bellySag.Length != bodyFeet.Count)
		{
			bellySag = new float[bodyFeet.Count];
		}
		bool initialize = previousBellyFeet.Length != bodyFeet.Count;
		if (initialize)
		{
			previousBellyFeet = new Vector2[bodyFeet.Count];
		}
		for (int i = 0; i < bodyFeet.Count; i++)
		{
			Vector2 foot = bodyFeet[i];
			float target = 0;
			GroundMoveResult result = FindGround(foot.X, foot.Y, 0.25f, out Vector2 ground, out _);
			// 无地面不代表净空为零：跨出边缘后仍按速度限制收回腹部。
			float clearance = result == GroundMoveResult.Blocked ? 0 : 8;
			if (result == GroundMoveResult.Moved)
			{
				clearance = Math.Max(0, ground.Y + 0.25f + BodyInset - foot.Y);
				// 提前接触阶缘，使下垂的腹部在越过阶缘前逐渐收回，不瞬间弹起。
				if (clearance < 16 && FindGround(foot.X, foot.Y - 16, 16, out Vector2 support, out _, 6) == GroundMoveResult.Moved)
				{
					float gap = support.Y + 16 + BodyInset - foot.Y;
					if (gap > 0 && gap < 16)
					{
						float fade = 1 - MathHelper.SmoothStep(0, 1, MathHelper.Clamp((gap - 8) / 8, 0, 1));
						target = Math.Min(8, gap * fade);
					}
				}
			}
			// 按该材质列的实际移动提前收腹，避免到阶缘才被净空限制瞬间顶回。
			Vector2 movement = initialize ? Vector2.Zero : foot - previousBellyFeet[i];
			previousBellyFeet[i] = foot;
			if (target > 0 && movement.LengthSquared() > 0.0001f)
			{
				for (int ahead = 1; ahead <= 16; ahead++)
				{
					Vector2 future = foot + movement * ahead;
					GroundMoveResult futureResult = FindGround(future.X, future.Y, 0.25f, out Vector2 futureGround, out _);
					if (futureResult == GroundMoveResult.Unsupported)
					{
						continue;
					}
					float futureClearance = futureResult == GroundMoveResult.Blocked ? 0 : Math.Max(0, futureGround.Y + 0.25f + BodyInset - future.Y);
					target = Math.Min(target, futureClearance + ahead * 0.5f);
				}
			}
			bellySag[i] = Math.Min(clearance, bellySag[i] + MathHelper.Clamp(target - bellySag[i], -0.5f, 0.5f));
			foot.Y += bellySag[i];
			bodyFeet[i] = foot;
		}
	}

	private Vector2[] BuildBodySpine(float length, float height)
	{
		const int columns = BodySegments;
		var spine = new Vector2[columns + 1];
		if (!attached)
		{
			NPC.rotation = MathHelper.WrapAngle(NPC.rotation) * 0.9f;
			for (int i = 0; i <= columns; i++)
			{
				spine[i] = NPC.Bottom + new Vector2((i / (float)columns - 0.5f) * length, -height * 0.5f).RotatedBy(NPC.rotation);
			}
			return spine;
		}

		float radius = height * 0.5f + 1;
		float clearanceRadius = height * 0.5f - BodyInset;
		float sampleSpacing = (length + height * 6) / BodyPathSamples;
		Vector2[] path = TraceBodyPath(height, radius, sampleSpacing);
		Vector2[] smoothPath = SmoothBodyPath(path, sampleSpacing, radius * 0.35f);
		ResampleBodyPath(smoothPath, length, spine);
		SmoothBodyBends(spine, length, radius, clearanceRadius);
		return spine;
	}

	private Vector2[] TraceBodyPath(float height, float radius, float sampleSpacing)
	{
		const int samples = BodyPathSamples;
		// 绘制探针跟随连续地表，腹部下缘负责台阶间的局部贴合。
		float probeRadius = Math.Min(radius, 16f);
		Vector2 probeOffset = new Vector2(0, radius - probeRadius);
		float supportY = surfacePosition.Y - surfaceNormal.Y * radius;
		if (FindGround(surfacePosition.X, supportY - 0.25f, 0.25f, out _, out _) != GroundMoveResult.Moved
			&& !HasWalkableStepConnection(surfacePosition.X - surfaceNormal.X * radius, supportY, surfacePosition.X))
		{
			// 身体中心已越过陡坎，仍由上沿支撑。小探针沿接触法线靠近上沿，
			// 避免提前丢失支撑而把弯曲身体重建成下方的平直身体。
			probeOffset = -surfaceNormal * (radius - probeRadius);
		}
		var path = new Vector2[samples + 1];
		Vector2 center = surfacePosition + probeOffset;
		if (FindGround(center.X, center.Y, probeRadius, out Vector2 groundCenter, out _) == GroundMoveResult.Moved)
		{
			center = groundCenter;
		}
		path[samples / 2] = center - probeOffset;
		for (int direction = -1; direction <= 1; direction += 2)
		{
			Vector2 point = center;
			Vector2 normal = surfaceNormal;
			for (int i = 1; i <= samples / 2; i++)
			{
				GroundMoveResult result = AdvanceSurface(ref point, ref normal, sampleSpacing * direction, probeRadius);
				if (result == GroundMoveResult.Unsupported)
				{
					point += new Vector2(sampleSpacing * direction, 0);
				}
				else if (result == GroundMoveResult.Blocked)
				{
					// 运动探测圆会提前碰墙；绘制的端面没有圆形端帽，应一直贴近实际墙面。
					int steps = Math.Max(1, (int)MathF.Ceiling(sampleSpacing));
					for (int step = 0; step < steps; step++)
					{
						Vector2 next = point + new Vector2(sampleSpacing * direction / steps, 0);
						if (!BodyColumnFits(next - probeOffset, height * 0.5f))
						{
							// 逼近允许的浅嵌边界，避免采样步长留下新的空隙。
							Vector2 low = point;
							Vector2 high = next;
							for (int refine = 0; refine < 6; refine++)
							{
								Vector2 middlePoint = (low + high) * 0.5f;
								if (BodyColumnFits(middlePoint - probeOffset, height * 0.5f))
								{
									low = middlePoint;
								}
								else
								{
									high = middlePoint;
								}
							}
							point = low;
							break;
						}
						point = next;
					}
				}
				path[samples / 2 + i * direction] = point - probeOffset;
			}
		}

		return path;
	}

	private static Vector2[] SmoothBodyPath(Vector2[] path, float sampleSpacing, float smoothingRadius)
	{
		var smoothPath = new Vector2[path.Length];
		int window = (int)MathF.Ceiling(smoothingRadius * 2 / sampleSpacing);
		for (int i = 0; i < path.Length; i++)
		{
			Vector2 sum = Vector2.Zero;
			float weights = 0;
			for (int j = Math.Max(0, i - window); j <= Math.Min(path.Length - 1, i + window); j++)
			{
				float distance = (j - i) * sampleSpacing / smoothingRadius;
				float weight = MathF.Exp(-distance * distance * 0.5f);
				sum += path[j] * weight;
				weights += weight;
			}
			smoothPath[i] = sum / weights;
		}
		return smoothPath;
	}

	private static void ResampleBodyPath(Vector2[] path, float length, Vector2[] spine)
	{
		// 按弧长等距采样，保持贴图各列连续且长度分配均匀。
		var arc = new float[path.Length];
		for (int i = 1; i < arc.Length; i++)
		{
			arc[i] = arc[i - 1] + Vector2.Distance(path[i - 1], path[i]);
		}
		float start = MathHelper.Clamp(arc[path.Length / 2] - length * 0.5f, 0, Math.Max(0, arc[^1] - length));
		int segment = 0;
		for (int i = 0; i < spine.Length; i++)
		{
			float distance = start + length * i / (spine.Length - 1);
			while (segment < path.Length - 2 && arc[segment + 1] < distance)
			{
				segment++;
			}
			float span = Math.Max(0.001f, arc[segment + 1] - arc[segment]);
			spine[i] = Vector2.Lerp(path[segment], path[segment + 1], (distance - arc[segment]) / span);
		}
	}

	private static void SmoothBodyBends(Vector2[] spine, float length, float radius, float clearanceRadius)
	{
		int segments = spine.Length - 1;
		float spacing = length / segments;
		var angles = new float[segments];
		for (int i = 0; i < segments; i++)
		{
			angles[i] = (spine[i + 1] - spine[i]).ToRotation();
			if (i > 0)
			{
				angles[i] = angles[i - 1] + MathHelper.WrapAngle(angles[i] - angles[i - 1]);
			}
		}
		// 弯曲越明显，越允许身体稍微回缩；平地保持原长，额外收缩不超过 10%。
		float totalBend = 0;
		for (int i = 1; i < segments; i++)
		{
			totalBend += MathF.Abs(angles[i] - angles[i - 1]);
		}
		float compression = MathHelper.SmoothStep(0, 0.1f, MathHelper.Clamp(totalBend / MathHelper.PiOver2, 0, 1));
		spacing *= 1 - compression;
		var smooth = new float[segments];
		for (int i = 0; i < segments; i++)
		{
			float weight = 0;
			for (int j = Math.Max(0, i - 1); j <= Math.Min(segments - 1, i + 1); j++)
			{
				float w = 2 - Math.Abs(j - i);
				smooth[i] += angles[j] * w;
				weight += w;
			}
			smooth[i] /= weight;
		}
		// 将允许的收缩均匀分配到整条身体，保留弯曲半径限制，避免局部纹理挤压。
		float maxTurn = spacing / (radius * 0.5f);
		for (int i = 1; i < segments; i++)
		{
			smooth[i] = MathHelper.Clamp(smooth[i], smooth[i - 1] - maxTurn, smooth[i - 1] + maxTurn);
		}
		for (int i = segments - 2; i >= 0; i--)
		{
			smooth[i] = MathHelper.Clamp(smooth[i], smooth[i + 1] - maxTurn, smooth[i + 1] + maxTurn);
		}
		int middle = segments / 2;
		for (int i = middle; i < segments; i++)
		{
			spine[i + 1] = spine[i] + smooth[i].ToRotationVector2() * spacing;
		}
		for (int i = middle - 1; i >= 0; i--)
		{
			spine[i] = spine[i + 1] - smooth[i].ToRotationVector2() * spacing;
		}

		// 沿身体中部的平滑法线贴近表面，不缩小寻路半径或重新挤压各段。
		Vector2 tangent = (spine[middle + 1] - spine[middle - 1]).SafeNormalize(Vector2.UnitX);
		Vector2 inset = GetBodyOffset(tangent, radius - clearanceRadius, true);
		for (int i = 0; i < spine.Length; i++)
		{
			spine[i] -= inset;
		}

		// 直接检查最终网格横截面，避免给每列额外套一个圆，造成头尾空气墙和台阶悬空。
		float halfHeight = radius - 1;
		float allowedInset = halfHeight - clearanceRadius;
		int crossSections = Math.Max(1, (int)MathF.Ceiling(halfHeight * 2 / 8));
		var offsets = new Vector2[spine.Length];
		for (int i = 0; i < spine.Length; i++)
		{
			Vector2 localTangent = spine[Math.Min(i + 1, spine.Length - 1)] - spine[Math.Max(0, i - 1)];
			offsets[i] = GetBodyOffset(localTangent, halfHeight, true);
		}
		for (int pass = 0; pass < 32; pass++)
		{
			Vector2 offset = Vector2.Zero;
			float penetrationWeight = 0;
			float maximumDepth = 0;
			for (int i = 0; i < spine.Length; i++)
			{
				for (int j = 0; j <= crossSections; j++)
				{
					Vector3 sample = SampleSurface(spine[i] + offsets[i] * (2f * j / crossSections - 1));
					float depth = -allowedInset - sample.X;
					if (depth <= 0)
					{
						continue;
					}
					offset += new Vector2(sample.Y, sample.Z) * depth * depth;
					penetrationWeight += depth;
					maximumDepth = Math.Max(maximumDepth, depth);
				}
			}
			if (penetrationWeight == 0 || maximumDepth < 0.01f)
			{
				break;
			}
			offset /= penetrationWeight;
			// 凹槽两侧的水平修正可能互相抵消；地面身体轻微上提即可释放这种夹持。
			if (offset.LengthSquared() < maximumDepth * maximumDepth * 0.0025f)
			{
				offset = -Vector2.UnitY * maximumDepth;
			}
			for (int i = 0; i < spine.Length; i++)
			{
				spine[i] += offset;
			}
		}
	}

	private static bool BodyColumnFits(Vector2 center, float halfHeight)
	{
		int steps = Math.Max(1, (int)MathF.Ceiling(halfHeight * 2 / 8));
		for (int i = 0; i <= steps; i++)
		{
			Vector2 point = center + new Vector2(0, halfHeight * (2f * i / steps - 1));
			if (SampleSurface(point).X < -BodyInset)
			{
				return false;
			}
		}
		return true;
	}

	private static void KeepOutsideTerrain(ref Vector2 point, float radius)
	{
		int range = Math.Max(1, (int)MathF.Ceiling((radius + 3) / 16));
		for (int i = 0; i < 8; i++)
		{
			Vector3 sample = SampleSurface(point, range);
			if (sample.X >= radius)
			{
				break;
			}
			point += new Vector2(sample.Y, sample.Z) * (radius - sample.X);
		}
	}

	internal static (Vector2[] Feet, Vector2[] Tops) BuildBodyEdges(IReadOnlyList<Vector2> spine, float height, bool softGround = false)
	{
		var feet = new Vector2[spine.Count];
		var tops = new Vector2[spine.Count];
		for (int i = 0; i < spine.Count; i++)
		{
			Vector2 tangent = spine[Math.Min(i + 1, spine.Count - 1)] - spine[Math.Max(0, i - 1)];
			Vector2 offset = GetBodyOffset(tangent, height * 0.5f, softGround);
			feet[i] = spine[i] - offset;
			tops[i] = spine[i] + offset;
		}
		return (feet, tops);
	}

	private static Vector2 GetBodyOffset(Vector2 tangent, float halfHeight, bool softGround)
	{
		Vector2 normal = new Vector2(tangent.Y, -tangent.X).SafeNormalize(-Vector2.UnitY);
		// 地面身体允许纵向形变，不再强制每列都垂直中心线而形成硬拱。
		if (softGround)
		{
			normal = Vector2.Lerp(-Vector2.UnitY, normal, 0.25f).SafeNormalize(-Vector2.UnitY);
		}
		return normal * halfHeight;
	}

	private float GetBodyRadius() => (BodyHeight * 0.5f) * (2 - GetSqueeze()) * NPC.scale + 1;

	private float GetSqueeze()
	{
		float phase = MoveTimer % CrawlPeriod;
		if (phase is >= 15 and < 30)
		{
			return 1 - (phase - 15) / 15 * 0.3f * MoveScale;
		}
		if (phase is >= 30 and <= 45)
		{
			return 1 - MathF.Sin((phase - 30) * 0.5f - MathHelper.PiOver2) / (phase - 27) * MoveScale;
		}
		return 1;
	}

	internal static (Vector2 Position, float Rotation, Vector2 Origin) GetEyePose(IReadOnlyList<Vector2> feet, IReadOnlyList<Vector2> tops, int direction, int frameWidth)
	{
		// 旧绘制以 NPC.Bottom 为锚点，眼睛 origin 为 (帧宽 / 2, 54 * 0.95)。
		// 将眼柄根部 (21, 41) 的原偏移映射到身体：未变形时距前端 3、距顶部 25.7 像素。
		float headInset = ((BodyLength - frameWidth) * 0.5f + 21) / BodyLength;
		float column = (direction == 1 ? 1 - headInset : headInset) * (feet.Count - 1);
		int index = Math.Min((int)column, feet.Count - 2);
		float u = column - index;
		const float v = (BodyHeight + 41 - 54 * 0.95f) / BodyHeight;
		// 与 TriangleStrip 的两个三角形使用同一插值，弯曲时也不离开身体表面。
		Vector2 root = v >= u
			? tops[index] * (1 - v) + feet[index] * (v - u) + feet[index + 1] * u
			: tops[index] * (1 - u) + tops[index + 1] * (u - v) + feet[index + 1] * v;
		Vector2 normal = Vector2.Lerp(tops[index] - feet[index], tops[index + 1] - feet[index + 1], u).SafeNormalize(-Vector2.UnitY);
		return (root, new Vector2(-normal.Y, normal.X).ToRotation(), new Vector2(direction == 1 ? frameWidth - 21 : 21, 41));
	}

	public override void SendExtraAI(BinaryWriter writer)
	{
		writer.Write(attached);
		writer.Write(surfacePosition.X);
		writer.Write(surfacePosition.Y);
		writer.Write(surfaceNormal.X);
		writer.Write(surfaceNormal.Y);
		writer.Write(surfaceRadius);
		writer.Write(crawlSpeed);
		writer.Write(detachTimer);
		writer.Write(obstacleTurnPending);
		writer.Write(MoveTimer);
		writer.Write(Angry);
		writer.Write(NPC.scale);
	}

	public override void ReceiveExtraAI(BinaryReader reader)
	{
		attached = reader.ReadBoolean();
		surfacePosition = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		surfaceNormal = new Vector2(reader.ReadSingle(), reader.ReadSingle());
		surfaceRadius = reader.ReadSingle();
		crawlSpeed = reader.ReadSingle();
		detachTimer = reader.ReadInt32();
		obstacleTurnPending = reader.ReadBoolean();
		MoveTimer = reader.ReadSingle();
		Angry = reader.ReadBoolean();
		NPC.scale = reader.ReadSingle();
		NPC.noGravity = NPC.noTileCollide = attached;
		if (!attached)
		{
			// SyncNPC 的 position 已是脱落后的左上角，恢复尺寸时不要再次平移。
			NPC.width = NPC.height = BodyHeight;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		Angry = true;

		NPC.netUpdate = true;
		if (Main.dedServ)
		{
			return;
		}
		for (int g = 0; g < 11; g++)
		{
			Vector2 vel = new Vector2(0, Main.rand.NextFloat(24, 72)).RotatedByRandom(MathHelper.TwoPi);
			float dropScale = Main.rand.NextFloat(6f, 12f);
			var blood = new PurpleBombCaterpillarBloodDrop
			{
				Velocity = vel / dropScale,
				Active = true,
				Visible = true,
				Position = NPC.position + new Vector2(Main.rand.NextFloat(0, NPC.width), Main.rand.NextFloat(0, NPC.height)) + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				MaxTime = Main.rand.Next(42, 84),
				Scale = dropScale,
				Rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
	}

	public override void OnKill()
	{
		Projectile corpse = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<PurpleBombCaterpillarDeath>(), NPC.damage, 3.5f);
		corpse.Center = NPC.Center;
		corpse.rotation = NPC.rotation;
		corpse.scale = NPC.scale;
		corpse.direction = NPC.direction;
		corpse.spriteDirection = NPC.spriteDirection;
		if (corpse.ModProjectile is PurpleBombCaterpillarDeath death)
		{
			death.SetBodyShape(bodyFeet, bodyTops);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (Main.dedServ || bodyFeet.Count < 2)
		{
			return false;
		}
		Color glowColor = new Color(1f, 0.65f, 0.25f, 0) * GlowValue;
		DrawBodyLayer(ModAsset.PurpleBombCaterpillar_Body.Value, bodyFeet, bodyTops, drawColor, NPC.spriteDirection);
		DrawBodyLayer(ModAsset.PurpleBombCaterpillar_Body_glow.Value, bodyFeet, bodyTops, glowColor, NPC.spriteDirection);

		SpriteEffects flip = NPC.spriteDirection == 1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		var eyes = GetEyePose(bodyFeet, bodyTops, NPC.spriteDirection, NPC.frame.Width);
		SpriteBatchUtils.SpriteBatch_Draw_To_Vertex_Draw(ModAsset.PurpleBombCaterpillar_Eye.Value, eyes.Position - Main.screenPosition, NPC.frame, drawColor, eyes.Rotation, eyes.Origin, NPC.scale, flip);
		SpriteBatchUtils.SpriteBatch_Draw_To_Vertex_Draw(ModAsset.PurpleBombCaterpillar_Eye_glow.Value, eyes.Position - Main.screenPosition, NPC.frame, glowColor, eyes.Rotation, eyes.Origin, NPC.scale, flip);
		return false;
	}

	internal static void DrawBodyLayer(Texture2D texture, IReadOnlyList<Vector2> feet, IReadOnlyList<Vector2> tops, Color color, int direction)
	{
		if (Main.dedServ || feet.Count < 2)
		{
			return;
		}
		var vertices = new Vertex2D[feet.Count * 2];
		for (int i = 0; i < feet.Count; i++)
		{
			float u = i / (float)(feet.Count - 1);
			if (direction == -1)
			{
				u = 1 - u;
			}
			vertices[i * 2] = new Vertex2D(feet[i] - Main.screenPosition, color, new Vector3(u, 1f, 0f));
			vertices[i * 2 + 1] = new Vertex2D(tops[i] - Main.screenPosition, color, new Vector3(u, 0f, 0f));
		}
		Main.graphics.GraphicsDevice.Textures[0] = texture;
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, vertices.Length - 2);
	}
}
