using Everglow.Commons.Utilities;

namespace Everglow.Commons.Templates.Enemies;

public abstract class Fighter : ModNPC
{
	public bool CanOpenTheDoor = true;

	public bool TendToDespawn = false;

	public float MaxSpeedX = 1f;

	public float AggroThreshold = 60f;

	public override void SetDefaults()
	{
		NPC.width = 30;
		NPC.height = 40;
		NPC.lifeMax = 100;
		NPC.damage = 10;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.aiStyle = -1;
	}

	/// <summary>
	/// Walk 0^max-3, Jump max-2, Stand max-1
	/// </summary>
	/// <param name="frameHeight"></param>
	public override void FindFrame(int frameHeight)
	{
		if (NPC.velocity.Y < 0f)
		{
			NPC.frameCounter = 0;
		}

		NPC.frameCounter += 1;
		int frameNumber = (int)(NPC.frameCounter / 3);
		if (frameNumber >= Main.npcFrameCount[NPC.type] - 2)
		{
			NPC.frameCounter = 0;
			frameNumber = 0;
		}
		if (!NPC.collideY)
		{
			frameNumber = Main.npcFrameCount[NPC.type] - 1;
		}
		NPC.frame.Y = frameNumber * frameHeight;
	}

	public override void AI()
	{
		if (NPC.target < 0)
		{
			NPC.velocity.X *= 0;
			return;
		}
		Player player = Main.player[NPC.target];
		if (player.position.Y + player.height == NPC.position.Y + NPC.height)
		{
			NPC.directionY = -1;
		}

		// Accumulate aggro.
		Fighters_AccumulateAggro();

		// Find target and change direction, check despawn.
		Fighters_FindTarget();
		NPC.spriteDirection = NPC.direction;

		bool canJump = false;
		if (NPC.velocity.X == 0f)
		{
			canJump = true;
		}
		if (NPC.justHit)
		{
			canJump = false;
		}
		Fighters_Accelerate();

		bool noBlockOverTop = false;
		Fighters_Check_Collision_Top(out noBlockOverTop);
		Fighters_Step();
		Fighters_Obstruction(canJump, noBlockOverTop);
	}

	public virtual void Fighters_AccumulateAggro()
	{
		Player player = Main.player[NPC.target];
		bool ceasedOrMovingInWrongDirection = false;
		bool hittingDoor = false;
		if (NPC.ai[2] > 0f)
		{
			hittingDoor = true;
		}
		if (!hittingDoor)
		{
			if (NPC.velocity.Y == 0f && ((NPC.velocity.X > 0f && NPC.direction < 0) || (NPC.velocity.X < 0f && NPC.direction > 0)))
			{
				ceasedOrMovingInWrongDirection = true;
			}
			if (NPC.position.X == NPC.oldPosition.X || NPC.ai[3] >= AggroThreshold || ceasedOrMovingInWrongDirection)
			{
				NPC.ai[3] += 1f;
			}
			else if (Math.Abs(NPC.velocity.X) > 0.9f && NPC.ai[3] > 0f)
			{
				NPC.ai[3] -= 1f;
			}
			if (NPC.ai[3] > AggroThreshold * 10)
			{
				NPC.ai[3] = 0f;
			}
			if (NPC.justHit)
			{
				NPC.ai[3] = 0f;
			}
			if (NPC.ai[3] == AggroThreshold)
			{
				NPC.netUpdate = true;
			}
			if (player.Hitbox.Intersects(NPC.Hitbox))
			{
				NPC.ai[3] = 0f;
			}
		}
	}

	public virtual void Fighters_FindTarget()
	{
		Player player = Main.player[NPC.target];
		if (NPC.ai[3] < AggroThreshold)
		{
			NPC.TargetClosest(true);
			if (NPC.target < 0)
			{
				NPC.velocity.X *= 0;
				return;
			}
			if (NPC.directionY > 0 && player.Center.Y <= NPC.Bottom.Y)
			{
				NPC.directionY = -1;
			}
		}
		else if (NPC.ai[2] <= 0f)
		{
			if (TendToDespawn)
			{
				NPC.EncourageDespawn(10);
			}
			if (NPC.velocity.X == 0f)
			{
				if (NPC.velocity.Y == 0f)
				{
					NPC.ai[0] += 1f;
					if (NPC.ai[0] >= 2f)
					{
						NPC.direction *= -1;
						NPC.spriteDirection = NPC.direction;
						NPC.ai[0] = 0f;
					}
				}
			}
			else
			{
				NPC.ai[0] = 0f;
			}
			if (NPC.direction == 0)
			{
				NPC.direction = 1;
			}
		}
	}

	public virtual void Fighters_Accelerate()
	{
		if (NPC.velocity.X < -MaxSpeedX || NPC.velocity.X > MaxSpeedX)
		{
			if (NPC.velocity.Y == 0f)
			{
				NPC.velocity *= 0.8f;
			}
		}
		else if (NPC.velocity.X < MaxSpeedX && NPC.direction == 1)
		{
			NPC.velocity.X += 0.07f;
			if (NPC.velocity.X > MaxSpeedX)
			{
				NPC.velocity.X = MaxSpeedX;
			}
		}
		else if (NPC.velocity.X > -MaxSpeedX && NPC.direction == -1)
		{
			NPC.velocity.X -= 0.07f;
			if (NPC.velocity.X < -MaxSpeedX)
			{
				NPC.velocity.X = -MaxSpeedX;
			}
		}
	}

	public virtual void Fighters_Check_Collision_Top(out bool noBlockOverTop)
	{
		// Check if hit the ceiling;
		noBlockOverTop = false;
		if (NPC.velocity.Y == 0f)
		{
			int bottomTileY = (int)(NPC.position.Y + NPC.height + 7f) / 16;
			int topTileY = (int)(NPC.position.Y - 9f) / 16;
			int leftTileX = (int)NPC.position.X / 16;
			int rightTileX = (int)(NPC.position.X + NPC.width) / 16;
			int left_inside_TileX = (int)(NPC.position.X + 8f) / 16;
			int right_inside_TileX = (int)(NPC.position.X + NPC.width - 8f) / 16;
			for (int tile_x = left_inside_TileX; tile_x <= right_inside_TileX; tile_x++)
			{
				if (Main.tile[tile_x, topTileY] != null && Main.tile[tile_x, topTileY].HasUnactuatedTile && Main.tileSolid[Main.tile[tile_x, topTileY].TileType])
				{
					noBlockOverTop = false;
					break;
				}
				if (tile_x >= leftTileX && tile_x <= rightTileX && Main.tile[tile_x, bottomTileY].HasUnactuatedTile && Main.tileSolid[Main.tile[tile_x, bottomTileY].TileType])
				{
					noBlockOverTop = true;
				}
			}
			if (!noBlockOverTop && NPC.velocity.Y < 0f)
			{
				NPC.velocity.Y = 0f;
			}
		}
	}

	public virtual void Fighters_Step()
	{
		// Step tile
		if (NPC.velocity.Y >= 0f && NPC.directionY != 1)
		{
			int velocityDirection = Math.Sign(NPC.velocity.X);
			Vector2 nextPos = NPC.position;
			nextPos.X += NPC.velocity.X;
			int collisionBoundX = (int)((nextPos.X + NPC.width / 2 + (NPC.width / 2 + 1) * velocityDirection) / 16f);
			int collisionBottomY = (int)((nextPos.Y + NPC.height - 1f) / 16f);
			if (WorldGen.InWorld(collisionBoundX, collisionBottomY, 4))
			{
				if (collisionBoundX * 16 < nextPos.X + NPC.width && collisionBoundX * 16 + 16 > nextPos.X && ((Main.tile[collisionBoundX, collisionBottomY].HasUnactuatedTile && !Main.tile[collisionBoundX, collisionBottomY].topSlope() && !Main.tile[collisionBoundX, collisionBottomY - 1].topSlope() && Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY].TileType] && !Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY].TileType]) || (Main.tile[collisionBoundX, collisionBottomY - 1].halfBrick() && Main.tile[collisionBoundX, collisionBottomY - 1].HasUnactuatedTile)) && (!Main.tile[collisionBoundX, collisionBottomY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 1].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 1].TileType] || (Main.tile[collisionBoundX, collisionBottomY - 1].halfBrick() && (!Main.tile[collisionBoundX, collisionBottomY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 4].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 4].TileType]))) && (!Main.tile[collisionBoundX, collisionBottomY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 2].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 2].TileType]) && (!Main.tile[collisionBoundX, collisionBottomY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 3].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 3].TileType]) && (!Main.tile[collisionBoundX - velocityDirection, collisionBottomY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX - velocityDirection, collisionBottomY - 3].TileType]))
				{
					float bottomWorldY = collisionBottomY * 16;
					if (Main.tile[collisionBoundX, collisionBottomY].halfBrick())
					{
						bottomWorldY += 8f;
					}
					if (Main.tile[collisionBoundX, collisionBottomY - 1].halfBrick())
					{
						bottomWorldY -= 8f;
					}
					if (bottomWorldY < nextPos.Y + NPC.height)
					{
						float num188 = nextPos.Y + NPC.height - bottomWorldY;
						float num189 = 16.1f;
						if (num188 <= num189)
						{
							NPC.gfxOffY += NPC.position.Y + NPC.height - bottomWorldY;
							NPC.position.Y = bottomWorldY - NPC.height;
							NPC.stepSpeed = num188 < 9f ? 1f : 2f;
						}
					}
				}
			}
		}
	}

	public virtual void Fighters_Obstruction(bool canJump, bool noBlockOverTop)
	{
		if (noBlockOverTop)
		{
			int pushTileX = (int)((NPC.position.X + NPC.width / 2 + (NPC.width / 2f + 4) * NPC.direction) / 16f);
			int pushTileY = (int)((NPC.position.Y + NPC.height - 15f) / 16f);
			var pushTile = TileUtils.SafeGetTile(pushTileX, pushTileY - 1);
			if (pushTile.HasUnactuatedTile && (TileLoader.IsClosedDoor(pushTile) || pushTile.TileType == TileID.TallGateClosed))
			{
				Fighters_BreakDoor(pushTileX, pushTileY, CanOpenTheDoor);
			}
			else
			{
				Fighters_Jump(pushTileX, pushTileY, canJump);
			}
		}
	}

	public virtual void Fighters_BreakDoor(int pushTileX, int pushTileY, bool CanOpenTheDoor)
	{
		int AggroThreshold = 60;
		NPC.ai[2] += 1f;
		NPC.ai[3] = 0f;
		if (NPC.ai[2] >= 60f)
		{
			NPC.velocity.X = 0.5f * (float)(-(float)NPC.direction);
			WorldGen.KillTile(pushTileX, pushTileY - 1, true, false, false);
			if ((Main.netMode != NetmodeID.MultiplayerClient || !CanOpenTheDoor) && CanOpenTheDoor && Main.netMode != NetmodeID.MultiplayerClient)
			{
				NPC.ai[2] = 0;
				if (TileLoader.IsClosedDoor(Main.tile[pushTileX, pushTileY - 1]))
				{
					bool openSuccess = WorldGen.OpenDoor(pushTileX, pushTileY - 1, NPC.direction);
					if (!openSuccess)
					{
						NPC.ai[3] = AggroThreshold;
						NPC.netUpdate = true;
					}
					if (Main.netMode == NetmodeID.Server && openSuccess)
					{
						NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, pushTileX, pushTileY - 1, NPC.direction, 0, 0, 0);
					}
				}
				if (Main.tile[pushTileX, pushTileY - 1].TileType == TileID.TallGateClosed)
				{
					bool openSuccess = WorldGen.ShiftTallGate(pushTileX, pushTileY - 1, false, false);
					if (!openSuccess)
					{
						NPC.ai[3] = AggroThreshold;
						NPC.netUpdate = true;
					}
					if (Main.netMode == NetmodeID.Server && openSuccess)
					{
						NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, pushTileX, pushTileY - 1, 0f, 0, 0, 0);
					}
				}
			}
		}
	}

	public virtual void Fighters_Jump(int pushTileX, int pushTileY, bool canJump)
	{
		Player player = Main.player[NPC.target];
		if ((NPC.velocity.X < 0f && NPC.spriteDirection == -1) || (NPC.velocity.X > 0f && NPC.spriteDirection == 1))
		{
			// Jump over obstacles.
			if (NPC.height >= 32 && Main.tile[pushTileX, pushTileY - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 2].TileType])
			{
				if (Main.tile[pushTileX, pushTileY - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 3].TileType])
				{
					NPC.velocity.Y = -8f;
					NPC.netUpdate = true;
				}
				else
				{
					NPC.velocity.Y = -7f;
					NPC.netUpdate = true;
				}
			}
			else if (Main.tile[pushTileX, pushTileY - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 1].TileType])
			{
				NPC.velocity.Y = -6f;
				NPC.netUpdate = true;
			}
			else if (NPC.position.Y + NPC.height - pushTileY * 16 > 20f && Main.tile[pushTileX, pushTileY].HasUnactuatedTile && !Main.tile[pushTileX, pushTileY].TopSlope && Main.tileSolid[Main.tile[pushTileX, pushTileY].TileType])
			{
				NPC.velocity.Y = -5f;
				NPC.netUpdate = true;
			}
			else if (NPC.directionY < 0 && (!Main.tile[pushTileX, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX, pushTileY + 1].TileType]) && (!Main.tile[pushTileX + NPC.direction, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX + NPC.direction, pushTileY + 1].TileType]))
			{
				NPC.velocity.Y = -8f;
				NPC.velocity.X *= 1.5f;
				NPC.netUpdate = true;
			}

			// Jump when starting to accumulate aggro. "I found you! Look at me!"
			if (NPC.velocity.Y == 0f && canJump && NPC.ai[3] == 1f)
			{
				NPC.velocity.Y = -5f;
			}

			// Jump and attack target.
			if (NPC.velocity.Y == 0f && Main.expertMode && player.Bottom.Y < NPC.Top.Y && Math.Abs(NPC.Center.X - player.Center.X) < player.width * 3 && Collision.CanHit(NPC, player))
			{
				int maxTileOverHead = 6;
				if (player.Bottom.Y > NPC.Top.Y - maxTileOverHead * 16)
				{
					NPC.velocity.Y = -7.9f;
				}
				else
				{
					int centerTileX = (int)(NPC.Center.X / 16f);
					int bottomTileY = (int)(NPC.Bottom.Y / 16f) - 1;
					for (int y = bottomTileY; y > bottomTileY - maxTileOverHead; y--)
					{
						if (Main.tile[centerTileX, y].HasUnactuatedTile && TileID.Sets.Platforms[Main.tile[centerTileX, y].TileType])
						{
							NPC.velocity.Y = -7.9f;
							break;
						}
					}
				}
			}
		}
	}
}
