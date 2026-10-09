using Terraria.DataStructures;

namespace Everglow.Myth.TheFirefly.NPCs;

public enum WormSegmentType
{
	Head,
	Body,
	Tail,
}

/// <summary>
/// 非分离式蠕虫类敌人的基类。
/// </summary>
public abstract class FireWorm : ModNPC
{
	public abstract WormSegmentType SegmentType { get; }

	public float MoveSpeed { get; set; }

	public float Acceleration { get; set; }

	public NPC HeadSegment => Main.npc[NPC.realLife];

	public NPC FollowingNPC => SegmentType == WormSegmentType.Head ? null : Main.npc[(int)NPC.ai[1]];

	public NPC FollowerNPC => SegmentType == WormSegmentType.Tail ? null : Main.npc[(int)NPC.ai[0]];

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		return SegmentType == WormSegmentType.Head ? null : false;
	}

	private bool startDespawning;

	public override sealed bool PreAI()
	{
		if (NPC.localAI[1] == 0)
		{  // 判断是否初始化了，如果没有则初始化
			NPC.localAI[1] = 1f;
			Init();
		}

		if (SegmentType == WormSegmentType.Head)
		{
			HeadAI();

			if (!NPC.HasValidTarget)
			{
				NPC.TargetClosest(true);

				// If the NPC is a boss and it has no target, force it to fall to the underworld quickly
				// 如果这个NPC是一个BOSS，而且它没有目标，就迫使它迅速坠落到地底消失。
				if (!NPC.HasValidTarget && NPC.boss)
				{
					NPC.velocity.Y += 8f;

					MoveSpeed = 1000f;

					if (!startDespawning)
					{
						startDespawning = true;

						// Despawn after 90 ticks (1.5 seconds) if the NPC gets far enough away
						NPC.timeLeft = 90;
					}
				}
			}
		}
		else
		{
			BodyTailAI();
		}

		return true;
	}

	// Not visible to public API, but is used to indicate what AI to run
	// 对公共API不可见，但用于指示运行什么AI
	internal virtual void HeadAI()
	{ }

	internal virtual void BodyTailAI()
	{ }

	public abstract void Init();
}

/// <summary>
/// 蠕虫敌人的头段NPC的基类
/// </summary>
public abstract class FireWormHead : FireWorm
{
	public override sealed WormSegmentType SegmentType => WormSegmentType.Head;

	public abstract int BodyType { get; }

	public abstract int TailType { get; }

	public int MinSegmentLength { get; set; }

	public int MaxSegmentLength { get; set; }

	/// <summary>
	/// 当NPC试图 "挖掘 "地砖时，是否忽略了地砖的碰撞，就像Wyverns的工作方式。
	/// </summary>
	public bool CanFly { get; set; }

	/// <summary>
	/// 以像素为单位的NPC将发生碰撞的最大距离。
	/// Defaults to 500 pixels, which is equivalent to 62.5 tiles.
	/// </summary>
	public virtual int MaxDistanceForUsingTileCollision => 500;

	/// <summary>
	/// Whether the NPC uses
	/// </summary>
	public virtual bool HasCustomBodySegments => false;

	/// <summary>
	/// If not <see langword="null"/>, 这个NPC将以给定的世界位置为目标，而不是以其玩家目标为目标。
	/// </summary>
	public Vector2? ForcedTargetPosition { get; set; }

	/// <summary>
	/// 覆盖此方法，以使用自定义的身体出生代码
	/// 当HasCustomBodySegments为true时，才会调用该方法
	/// </summary>
	/// <param name="segmentCount">预计将产生多少个身体分段</param>
	/// <returns>The whoAmI of the most-recently spawned NPC, which is the result of calling <see cref="NPC.NewNPC(IEntitySource, int, int, int, int, float, float, float, float, int)"/></returns>
	public virtual int SpawnBodySegments(int segmentCount)
	{
		// Defaults to just returning this NPC's whoAmI, since the tail segment uses the return value as its "following" NPC index
		return NPC.whoAmI;
	}

	/// <summary>
	/// 生成蠕虫的身体或尾巴部分。
	/// </summary>
	/// <param name="source">蠕虫来源</param>
	/// <param name="type">蠕虫NPC的段的ID。T</param>
	/// <param name="latestNPC">The whoAmI of the most-recently spawned segment NPC in the worm, including the head</param>
	/// <returns></returns>
	public int SpawnSegment(IEntitySource source, int type, int latestNPC, int ai2 = 0)
	{
		int oldLatest = latestNPC;

		latestNPC = NPC.NewNPC(source, (int)NPC.Center.X, (int)NPC.Center.Y, type, NPC.whoAmI, 0, latestNPC);

		Main.npc[oldLatest].ai[0] = latestNPC;
		Main.npc[oldLatest].ai[2] = ai2;

		NPC latest = Main.npc[latestNPC];
		// NPC.realLife是产生的NPC的whoAmI，该NPC将与之分享其健康。
		latest.realLife = NPC.whoAmI;

		return latestNPC;
	}

	internal override sealed void HeadAI()
	{
		HeadAI_SpawnSegments();  // 由头部生成身体和尾部

		// bool collision = HeadAI_CheckCollisionForDustSpawns();
		// // 测量与目标的距离
		// HeadAI_CheckTargetDistance(ref collision);

		// HeadAI_Movement(collision);
	}

	private void HeadAI_SpawnSegments()
	{
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			// 因此，我们通过检查NPC.ai[0]（以下NPC的whoAmI）是否为0来开始AI。
			// 由于这是第一次更新，我们可以安全地假设我们需要催生蠕虫的其余部分（身体+尾巴）。
			bool hasFollower = NPC.ai[0] > 0;
			if (!hasFollower)
			{
				// NPC.realLife值主要是用来确定当我们击中这个NPC时，哪个NPC会失去生命。
				NPC.realLife = NPC.whoAmI;
				// latestNPC将在SpawnSegment()中使用，我将在那里解释它。
				int latestNPC = NPC.whoAmI;

				// Here we determine the length of the worm.
				int randomWormLength = Main.rand.Next(MinSegmentLength, MaxSegmentLength + 1);

				int distance = randomWormLength - 2;

				IEntitySource source = NPC.GetSource_FromAI();

				if (HasCustomBodySegments)
					// 调用处理催生体段的方法
					latestNPC = SpawnBodySegments(distance);
				else
				{
					// 像往常一样产生体节
					while (distance > 0)
					{
						int ai2 = distance;
						latestNPC = SpawnSegment(source, BodyType, latestNPC, ai2);
						distance--;
					}
				}

				// 催生尾巴部分
				SpawnSegment(source, TailType, latestNPC);

				NPC.netUpdate = true;

				// 确保所有的片段都能生成。 如果他们不能，就完全取消蠕虫的生成
				int count = 0;
				for (int i = 0; i < Main.maxNPCs; i++)
				{
					NPC n = Main.npc[i];

					if (n.active && (n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
					{
						count++;
					}
				}

				if (count != randomWormLength)
				{
					// Unable to spawn all of the segments... kill the worm
					for (int i = 0; i < Main.maxNPCs; i++)
					{
						NPC n = Main.npc[i];

						if (n.active && (n.type == Type || n.type == BodyType || n.type == TailType) && n.realLife == NPC.whoAmI)
						{
							n.active = false;
							n.netUpdate = true;
						}
					}
				}

				// 设置最近的玩家为目标
				NPC.TargetClosest(true);
			}
		}
	}

	// private bool HeadAI_CheckCollisionForDustSpawns() {
	// 	int minTilePosX = (int)(NPC.Left.X / 16) - 1;
	// 	int maxTilePosX = (int)(NPC.Right.X / 16) + 2;
	// 	int minTilePosY = (int)(NPC.Top.Y / 16) - 1;
	// 	int maxTilePosY = (int)(NPC.Bottom.Y / 16) + 2;

	// 	if (minTilePosX < 0)
	// 		minTilePosX = 0;
	// 	if (maxTilePosX > Main.maxTilesX)
	// 		maxTilePosX = Main.maxTilesX;
	// 	if (minTilePosY < 0)
	// 		minTilePosY = 0;
	// 	if (maxTilePosY > Main.maxTilesY)
	// 		maxTilePosY = Main.maxTilesY;

	// 	bool collision = false;

	// 	// 这是对物体碰撞的初步检查。
	// 	for (int i = minTilePosX; i < maxTilePosX; ++i) {
	// 		for (int j = minTilePosY; j < maxTilePosY; ++j) {
	// 			Tile tile = Main.tile[i, j];

	// 			// 如果物体是实心的或被认为是一个平台，那么就有有效的碰撞。
	// 			if (tile.HasUnactuatedTile && (Main.tileSolid[tile.TileType] || Main.tileSolidTop[tile.TileType] && tile.TileFrameY == 0) || tile.LiquidAmount > 64) {
	// 				Vector2 tileWorld = new Point16(i, j).ToWorldCoordinates(0, 0);

	// 				if (NPC.Right.X > tileWorld.X && NPC.Left.X < tileWorld.X + 16 && NPC.Bottom.Y > tileWorld.Y && NPC.Top.Y < tileWorld.Y + 16) {
	// 					// 发现碰撞
	// 					collision = true;

	// 					if (Main.rand.NextBool(100))
	// 						WorldGen.KillTile(i, j, fail: true, effectOnly: true, noItem: false);
	// 				}
	// 			}
	// 		}
	// 	}

	// 	return collision;
	// }

	// private void HeadAI_CheckTargetDistance(ref bool collision) {
	// 	// 如果没有与tiles发生碰撞，我们会检查这个NPC和它的目标之间的距离是否过大，这样我们仍然可以触发 "碰撞"。
	// 	if (!collision) {
	// 		Rectangle hitbox = NPC.Hitbox;

	// 		int maxDistance = MaxDistanceForUsingTileCollision;

	// 		bool tooFar = true;

	// 		for (int i = 0; i < Main.maxPlayers; i++) {
	// 			Rectangle areaCheck;

	// 			Player player = Main.player[i];

	// 			if (ForcedTargetPosition is Vector2 target)
	// 				areaCheck = new Rectangle((int)target.X - maxDistance, (int)target.Y - maxDistance, maxDistance * 2, maxDistance * 2);
	// 			else if (player.active && !player.dead && !player.ghost)
	// 				areaCheck = new Rectangle((int)player.position.X - maxDistance, (int)player.position.Y - maxDistance, maxDistance * 2, maxDistance * 2);
	// 			else
	// 				continue;  // Not a valid player

	// 			if (hitbox.Intersects(areaCheck)) {
	// 				tooFar = false;
	// 				break;
	// 			}
	// 		}

	// 		if (tooFar)
	// 			collision = true;
	// 	}
	// }

	// private void HeadAI_Movement(bool collision) {
	// 	// 移动速度决定了这个NPC可以移动的最大速度。
	// 	float speed = MoveSpeed;
	// 	// 这个NPC加速的速度。
	// 	float acceleration = Acceleration;

	// 	float targetXPos, targetYPos;

	// 	Player playerTarget = Main.player[NPC.target];

	// 	Vector2 forcedTarget = ForcedTargetPosition ?? playerTarget.Center;
	// 	// 像这样使用一个ValueTuple，可以方便地分配多个值
	// 	(targetXPos, targetYPos) = (forcedTarget.X, forcedTarget.Y);

	// 	// Copy the value, since it will be clobbered later
	// 	Vector2 npcCenter = NPC.Center;

	// 	float targetRoundedPosX = (float)((int)(targetXPos / 16f) * 16);
	// 	float targetRoundedPosY = (float)((int)(targetYPos / 16f) * 16);
	// 	npcCenter.X = (float)((int)(npcCenter.X / 16f) * 16);
	// 	npcCenter.Y = (float)((int)(npcCenter.Y / 16f) * 16);
	// 	// dirX和dirY分别是当前蠕虫到目标的x和y距离
	// 	float dirX = targetRoundedPosX - npcCenter.X;
	// 	float dirY = targetRoundedPosY - npcCenter.Y;
	// 	// 当前蠕虫到目标的直线距离
	// 	float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);

	// 	// 如果我们没有任何类型的碰撞，我们希望NPC倒下并沿X轴减速。
	// 	if (!collision && !CanFly)
	// 		HeadAI_Movement_HandleFallingFromNoCollision(dirX, speed, acceleration);
	// 	else {
	// 		// 否则，我们要播放一些音频（soundDelay）并向我们的目标移动。
	// 		HeadAI_Movement_PlayDigSounds(length);
	// 		// 设置NPC的速度
	// 		HeadAI_Movement_HandleMovement(dirX, dirY, length, speed, acceleration);
	// 	}
	// 	// 设置NPC的转向
	// 	HeadAI_Movement_SetRotation(collision);
	// }

	// 	private void HeadAI_Movement_HandleFallingFromNoCollision(float dirX, float speed, float acceleration) {
	// 		NPC.TargetClosest(true);
	// 		// Constant gravity of 0.11 pixels/tick
	// 		NPC.velocity.Y += 0.11f;

	// 		// Ensure that the NPC does not fall too quickly
	// 		if (NPC.velocity.Y > speed)
	// 			NPC.velocity.Y = speed;

	// 		// 以下行为模仿了香草虫的运动
	// 		if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.4f) {
	// 			// 速度足够快，但不能太快
	// 			if (NPC.velocity.X < 0.0f)
	// 				NPC.velocity.X -= acceleration * 1.1f;
	// 			else
	// 				NPC.velocity.X += acceleration * 1.1f;
	// 		} else if (NPC.velocity.Y == speed) {
	// 			// NPC has reached terminal velocity
	// 			if (NPC.velocity.X < dirX)
	// 				NPC.velocity.X += acceleration;
	// 			else if (NPC.velocity.X > dirX)
	// 				NPC.velocity.X -= acceleration;
	// 		} else if (NPC.velocity.Y > 4) {
	// 			if (NPC.velocity.X < 0)
	// 				NPC.velocity.X += acceleration * 0.9f;
	// 			else
	// 				NPC.velocity.X -= acceleration * 0.9f;
	// 		}
	// 	}

	// 	private void HeadAI_Movement_PlayDigSounds(float length) {
	// 		if (NPC.soundDelay == 0) {
	// 			// NPC离目标地点越近，播放声音越快
	// 			float num1 = length / 40f;

	// 			if (num1 < 10)
	// 				num1 = 10f;

	// 			if (num1 > 20)
	// 				num1 = 20f;

	// 			NPC.soundDelay = (int)num1;

	// 			SoundEngine.PlaySound(SoundID.WormDig, NPC.position);
	// 		}
	// 	}

	// 	private void HeadAI_Movement_HandleMovement(float dirX, float dirY, float length, float speed, float acceleration) {
	// 		float absDirX = Math.Abs(dirX);
	// 		float absDirY = Math.Abs(dirY);
	// 		float newSpeed = speed / length;  // 距离越近，速度越快
	// 		dirX *= newSpeed;
	// 		dirY *= newSpeed;
	// 		// 蠕虫速度方向和目标方向有相同方向的
	// 		if ((NPC.velocity.X > 0 && dirX > 0) || (NPC.velocity.X < 0 && dirX < 0 )|| (NPC.velocity.Y > 0 && dirY > 0) || (NPC.velocity.Y < 0 && dirY < 0)) {
	// 			// 该NPC正在向目标地点移动
	// 			if (NPC.velocity.X < dirX)
	// 				NPC.velocity.X += acceleration;
	// 			else if (NPC.velocity.X > dirX)
	// 				NPC.velocity.X -= acceleration;

	// 			if (NPC.velocity.Y < dirY)
	// 				NPC.velocity.Y += acceleration;
	// 			else if (NPC.velocity.Y > dirY)
	// 				NPC.velocity.Y -= acceleration;

	// 			// 预定的Y-速度很小，而且NPC正在向左移动，目标在NPC的右边，反之亦然。
	// 			if (Math.Abs(dirY) < speed * 0.2 && ((NPC.velocity.X > 0 && dirX < 0) || (NPC.velocity.X < 0 && dirX > 0))) {
	// 				if (NPC.velocity.Y > 0)
	// 					NPC.velocity.Y += acceleration * 2f;
	// 				else
	// 					NPC.velocity.Y -= acceleration * 2f;
	// 			}
	// 			// 预定的X-速度很小，而且NPC正在向上/向下移动，目标在NPC的下方/上方。
	// 			if (Math.Abs(dirX) < speed * 0.2 && ((NPC.velocity.Y > 0 && dirY < 0) || (NPC.velocity.Y < 0 && dirY > 0))) {
	// 				if (NPC.velocity.X > 0)
	// 					NPC.velocity.X = NPC.velocity.X + acceleration * 2f;
	// 				else
	// 					NPC.velocity.X = NPC.velocity.X - acceleration * 2f;
	// 			}
	// 		} else if (absDirX > absDirY) {
	// 			// X距离比Y距离大。 迫使沿X轴的运动更强烈
	// 			if (NPC.velocity.X < dirX)
	// 				NPC.velocity.X += acceleration * 1.1f;
	// 			else if (NPC.velocity.X > dirX)
	// 				NPC.velocity.X -= acceleration * 1.1f;

	// 			if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5) {
	// 				if (NPC.velocity.Y > 0)
	// 					NPC.velocity.Y += acceleration;
	// 				else
	// 					NPC.velocity.Y -= acceleration;
	// 			}
	// 		} else {
	// 			if (NPC.velocity.Y < dirY)
	// 				NPC.velocity.Y += acceleration * 1.1f;
	// 			else if (NPC.velocity.Y > dirY)
	// 				NPC.velocity.Y -= acceleration * 1.1f;

	// 			if (Math.Abs(NPC.velocity.X) + Math.Abs(NPC.velocity.Y) < speed * 0.5) {
	// 				if (NPC.velocity.X > 0)
	// 					NPC.velocity.X += acceleration;
	// 				else
	// 					NPC.velocity.X -= acceleration;
	// 			}
	// 		}
	// 	}

	// 	private void HeadAI_Movement_SetRotation(bool collision) {
	// 		// Set the correct rotation for this NPC.
	// 		// 假设NPC的贴图是朝上的。 你可能需要修改这一行，以适当考虑到你的NPC的方向。
	// 		NPC.rotation = NPC.velocity.ToRotation() + MathHelper.PiOver2;

	// 		// 一些netupdate的东西（多人游戏兼容性）。
	// 		if (collision) {
	// 			if(NPC.localAI[0] != 1)  // 碰撞检测同步
	// 				NPC.netUpdate = true;

	// 			NPC.localAI[0] = 1f;
	// 		} else {
	// 			if(NPC.localAI[0] != 0)
	// 				NPC.netUpdate = true;

	// 			NPC.localAI[0] = 0f;
	// 		}

	// 		// 如果NPC的速度发生变化，并且没有被玩家 "击中"，则强制进行网络更新。
	// 		if (((NPC.velocity.X > 0 && NPC.oldVelocity.X < 0) || (NPC.velocity.X < 0 && NPC.oldVelocity.X > 0) || (NPC.velocity.Y > 0 && NPC.oldVelocity.Y < 0) || (NPC.velocity.Y < 0 && NPC.oldVelocity.Y > 0)) && !NPC.justHit)
	// 			NPC.netUpdate = true;
	// 	}
}

public abstract class FireWormBody : FireWorm
{
	public override sealed WormSegmentType SegmentType => WormSegmentType.Body;

	internal override void BodyTailAI()
	{
		CommonAI_BodyTail(this);
	}

	internal static void CommonAI_BodyTail(FireWorm worm)
	{
		if (!worm.NPC.HasValidTarget)
		{
			worm.NPC.TargetClosest(true);
		}

		if (Main.player[worm.NPC.target].dead && worm.NPC.timeLeft > 30000)
		{
			worm.NPC.timeLeft = 10;
		}

		NPC following = worm.NPC.ai[1] >= Main.maxNPCs ? null : worm.FollowingNPC;
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			// 删除违法生成的身体段
			if (following is null || !following.active || following.friendly || following.townNPC || following.lifeMax <= 5)
			{
				worm.NPC.life = 0;
				worm.NPC.HitEffect(0, 10);
				worm.NPC.active = false;
			}
		}

		if (following is not null)
		{
			// Follow behind the segment "in front" of this NPC
			// 使用当前的NPC.Center来计算对这个NPC的 "父NPC "的方向。
			float dirX = following.Center.X - worm.NPC.Center.X;
			float dirY = following.Center.Y - worm.NPC.Center.Y;
			// 然后我们使用Atan2来获得对该父NPC的正确旋转。
			// 假设NPC的贴图是朝上的。 你可能需要修改这一行，以适当考虑到你的NPC的方向。
			worm.NPC.rotation = (float)Math.Atan2(dirY, dirX) + MathHelper.PiOver2;
			// 我们还得到了方向向量的长度。
			float length = (float)Math.Sqrt(dirX * dirX + dirY * dirY);
			// We calculate a new, correct distance.
			float dist = (length - worm.NPC.width) / length;
			float posX = dirX * dist;
			float posY = dirY * dist;

			// 重置这个NPC的速度，因为我们不希望它自己移动。
			worm.NPC.velocity = Vector2.Zero;
			// 并将这个NPC的位置相应地设置为这个NPC的父NPC的位置。
			worm.NPC.position.X += posX;
			worm.NPC.position.Y += posY;
		}
	}
}

// Since the body and tail segments share the same AI
public abstract class FireWormTail : FireWorm
{
	public override sealed WormSegmentType SegmentType => WormSegmentType.Tail;

	internal override void BodyTailAI()
	{
		FireWormBody.CommonAI_BodyTail(this);
	}
}
