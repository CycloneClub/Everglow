using Everglow.Commons.Mechanics.ElementalDebuff;
using Everglow.Commons.Netcode.Packets;

namespace Everglow.Commons.Utilities;

public static class NPCUtils
{
	#region Town NPC Behavior

	public static void TryCloseDoor(NPC npc)
	{
		if (npc.closeDoor && ((npc.position.X + npc.width / 2) / 16f > npc.doorX + 2 || (npc.position.X + npc.width / 2) / 16f < npc.doorX - 2))
		{
			Tile tileSafely = Framing.GetTileSafely(npc.doorX, npc.doorY);

			if (TileLoader.CloseDoorID(tileSafely) >= 0)
			{
				if (WorldGen.CloseDoor(npc.doorX, npc.doorY))
				{
					npc.closeDoor = false;
					NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 1, npc.doorX, npc.doorY, npc.direction);
				}

				if ((npc.position.X + npc.width / 2) / 16f > npc.doorX + 4 || (npc.position.X + npc.width / 2) / 16f < npc.doorX - 4 || (npc.position.Y + npc.height / 2) / 16f > npc.doorY + 4 || (npc.position.Y + npc.height / 2) / 16f < npc.doorY - 4)
				{
					npc.closeDoor = false;
				}
			}
			else if (tileSafely.type == 389)
			{
				if (WorldGen.ShiftTallGate(npc.doorX, npc.doorY, closing: true))
				{
					npc.closeDoor = false;
					NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 5, npc.doorX, npc.doorY);
				}

				if ((npc.position.X + npc.width / 2) / 16f > npc.doorX + 4 || (npc.position.X + npc.width / 2) / 16f < npc.doorX - 4 || (npc.position.Y + npc.height / 2) / 16f > npc.doorY + 4 || (npc.position.Y + npc.height / 2) / 16f < npc.doorY - 4)
				{
					npc.closeDoor = false;
				}
			}
			else
			{
				npc.closeDoor = false;
			}
		}
	}

	public static void TryOpenDoor(NPC npc)
	{
		int touchDoorX = (int)((npc.position.X + npc.width / 2 + 15 * npc.direction) / 16f);
		int touchDoorY = (int)((npc.position.Y + npc.height - 16f) / 16f);
		Tile tileSafely5 = Framing.GetTileSafely(touchDoorX, touchDoorY - 2);
		if ((npc.townNPC || NPCID.Sets.AllowDoorInteraction[npc.type]) && tileSafely5.HasUnactuatedTile && (TileLoader.IsClosedDoor(tileSafely5) || tileSafely5.type == 388))
		{
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				if (WorldGen.OpenDoor(touchDoorX, touchDoorY - 2, npc.direction))
				{
					npc.closeDoor = true;
					npc.doorX = touchDoorX;
					npc.doorY = touchDoorY - 2;
					NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, touchDoorX, touchDoorY - 2, npc.direction);
					npc.netUpdate = true;
					npc.ai[1] += 80f;
				}
				else if (WorldGen.OpenDoor(touchDoorX, touchDoorY - 2, -npc.direction))
				{
					npc.closeDoor = true;
					npc.doorX = touchDoorX;
					npc.doorY = touchDoorY - 2;
					NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, touchDoorX, touchDoorY - 2, -npc.direction);
					npc.netUpdate = true;
					npc.ai[1] += 80f;
				}
				else if (WorldGen.ShiftTallGate(touchDoorX, touchDoorY - 2, closing: false))
				{
					npc.closeDoor = true;
					npc.doorX = touchDoorX;
					npc.doorY = touchDoorY - 2;
					NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, touchDoorX, touchDoorY - 2);
					npc.netUpdate = true;
					npc.ai[1] += 80f;
				}
				else
				{
					npc.direction *= -1;
					npc.netUpdate = true;
				}
			}
		}
	}

	public static int ChooseDirection(NPC npc)
	{
		return Collision.SolidCollision(npc.Right + new Vector2(8, 0), 2, 2)
			? -1
			: Collision.SolidCollision(npc.Left + new Vector2(-10, 0), 2, 2) ? 1 : Main.rand.NextBool() ? -1 : 1;
	}

	public static bool CheckSit(NPC npc)
	{
		if (npc.Center.X > 320 && npc.Center.X < Main.maxTilesX * 16 - 320 && npc.Center.Y > 320 && npc.Center.Y < Main.maxTilesY * 16 - 320)
		{
			var tile = Main.tile[npc.Center.ToTileCoordinates()];
			int tileType = tile.TileType;
			bool flag = !NPCID.Sets.CannotSitOnFurniture[npc.type] && !NPCID.Sets.IsTownSlime[npc.type];
			if (flag)
			{
				flag &= tile != null && tile.active() && TileID.Sets.CanBeSatOnForNPCs[tile.type];
			}
			if (flag)
			{
				Point point = (npc.Bottom + Vector2.UnitY * -2f).ToTileCoordinates();
				for (int i = 0; i < 200; i++)
				{
					if (Main.npc[i].active && Main.npc[i].aiStyle == NPCAIStyleID.Passive && Main.npc[i].townNPC && Main.npc[i].ai[0] == 5f && (Main.npc[i].Bottom + Vector2.UnitY * -2f).ToTileCoordinates() == point)
					{
						flag = false;
						break;
					}
				}
			}
			if (flag)
			{
				Vector2 bottom = default;
				npc.SitDown(npc.Center.ToTileCoordinates(), out npc.direction, out bottom);
				npc.spriteDirection = npc.direction;
				npc.velocity *= 0;
				return true;
			}
		}
		return false;
	}

	public static bool CanContinueWalk(NPC npc)
	{
		if (npc.Center.X < 320 || npc.Center.X > Main.maxTilesX * 16 - 320 || npc.Center.Y < 320 || npc.Center.Y > Main.maxTilesY * 16 - 320)
		{
			return false;
		}
		Point checkPoint = (npc.Bottom + new Vector2(8 * npc.direction, 8)).ToTileCoordinates() + new Point(npc.direction, -1);
		Tile checkTile = Main.tile[checkPoint];
		if (TileLoader.IsClosedDoor(checkTile.TileType) || checkTile.TileType == TileID.TallGateClosed)
		{
			return true;
		}
		int empty = 0;

		// This check was from 2 tile over NPC's bottom to 3 tiles below.
		for (int y = -2; y < 4; y++)
		{
			if (!TileUtils.PlatformCollision(npc.Bottom + new Vector2(npc.direction * 15, y * 16)) && !Collision.SolidCollision(npc.BottomLeft + new Vector2(npc.direction * 15, y * 16), npc.width, npc.height))
			{
				empty++;
			}
			else
			{
				break;
			}
		}
		if (empty >= 5)
		{
			empty = 0;

			// To stop a walking NPC, a groove at lease 2 tiles is necessary.
			for (int y = -2; y < 4; y++)
			{
				if (!TileUtils.PlatformCollision(npc.Bottom + new Vector2(npc.direction * 30, y * 16)) && !Collision.SolidCollision(npc.BottomLeft + new Vector2(npc.direction * 30, y * 16), npc.width, npc.height))
				{
					empty++;
				}
				else
				{
					break;
				}
			}
			if (empty >= 5)
			{
				return false;
			}
		}

		// Jumping limit was 6 tiles.(Only for normal NPC)
		int obstructionHeight = 0;
		for (int y = 1; y < 6; y++)
		{
			if (Collision.SolidCollision(npc.Bottom + new Vector2(16 * npc.direction, 16 * -y + 8), 2, 2))
			{
				obstructionHeight++;
			}
		}
		if (obstructionHeight >= 1 && npc.collideX)
		{
			npc.velocity.Y = -2.5f * obstructionHeight;
		}
		else if (checkTile.IsHalfBlock)
		{
			npc.velocity.Y = -0.5f;
		}
		return obstructionHeight < 5;
	}

	#endregion

	#region Vanilla Stats

	public static int GetVanillaDotDamage(this NPC npc, IEnumerable<int> buffTypes) =>
		buffTypes
			.Where(npc.HasBuff)
			.Where(type => BuffUtils.VanillaDotDebuffDamageOnNPC.TryGetValue(type, out int _))
			.Select(type => npc.buffTime[npc.FindBuffIndex(type)] * BuffUtils.VanillaDotDebuffDamageOnNPC[type])
			.Sum();

	/// <summary>
	/// Set <see cref="NPC.lifeRegenExpectedLossPerSecond"/> to the max of current value and given value,
	/// avoiding multiple debuffs stacking incorrectly.
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="value"></param>
	public static void SetLifeRegenExpectedLossPerSecond(this NPC npc, int value) =>
		npc.lifeRegenExpectedLossPerSecond = Math.Max(npc.lifeRegenExpectedLossPerSecond, value);

	public static void StrikeNPCWithCustomCombatText(this NPC npc, NPC.HitInfo hit, Color textColor, bool dot = false)
	{
		npc.HideStrikeDamage = true;
		npc.StrikeNPC(hit);
		npc.HideStrikeDamage = false;
		CombatText.NewText(npc.Hitbox, textColor, hit.Damage, hit.Crit, dot);
	}

	#endregion

	#region Elemental Debuff

	/// <summary>
	/// Add build-up to the specific elemental debuff instance of this NPC.
	/// <br/> This accounts for if NPC has resistance to this type of elemental debuff.
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="type"></param>
	/// <param name="owner"></param>
	/// <param name="buildUp"></param>
	/// <param name="penentration"></param>
	/// <returns></returns>
	internal static bool AddElementalDebuffBuildUp(this NPC npc, string type, int owner, int buildUp, float penentration = 0)
	{
		// Add to real target of the npc
		if (npc.realLife == -1 || npc.realLife == npc.whoAmI)
		{
			return npc.GetGlobalNPC<ElementalDebuffGlobalNPC>().ElementalDebuffs[type].AddBuildUp(buildUp, owner, penentration);
		}
		else
		{
			var realLife = Main.npc[npc.realLife];
			return realLife.active
				? realLife.GetGlobalNPC<ElementalDebuffGlobalNPC>().ElementalDebuffs[type].AddBuildUp(buildUp, owner, penentration)
				: false;
		}
	}

	/// <summary>
	/// Add build-up to the specific elemental debuff instance of this NPC with the source of player.
	/// <br/> This accounts for if NPC has resistance to this type of elemental debuff, also player's elemental penetration.
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="source"></param>
	/// <param name="type"></param>
	/// <param name="buildUp"></param>
	/// <param name="additionalPenentration"></param>
	public static bool AddElementalDebuffBuildUp(this NPC npc, Player source, string type, int buildUp, float additionalPenentration = 0)
	{
		if (NetUtils.IsClient && source.whoAmI == Main.myPlayer)
		{
			ModIns.PacketResolver.Send(new ElementalBuildUpPacket(npc.whoAmI, ElementalDebuffRegistry.NameToNetID[type], buildUp), false, source);
		}

		npc.PlayerInteraction(source.whoAmI);

		// Calculate player's elemental penetration
		if (source != null)
		{
			var typePene = source.GetElementalPenetration(type).ApplyTo(1f) - 1f;
			if (typePene > 0)
			{
				additionalPenentration += typePene;
			}

			var genericPene = source.GetElementalPenetration().Generic.ApplyTo(1f) - 1f;
			if (genericPene > 0)
			{
				additionalPenentration += genericPene;
			}
		}

		return npc.AddElementalDebuffBuildUp(type, source.whoAmI, buildUp, additionalPenentration);
	}

	/// <summary>
	/// Add build-up to the specific elemental debuff instance of this NPC with source of world/server.
	/// <br/> This accounts for if NPC has resistance to this type of elemental debuff.
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="type"></param>
	/// <param name="buildUp"></param>
	/// <param name="additionalPenentration"></param>
	/// <returns></returns>
	public static bool AddElementalDebuffBuildUp_World(this NPC npc, string type, int buildUp, float additionalPenentration = 0)
	{
		if (NetUtils.IsServer)
		{
			ModIns.PacketResolver.Send(new ElementalBuildUpPacket(npc.whoAmI, ElementalDebuffRegistry.NameToNetID[type], buildUp));
		}

		return npc.AddElementalDebuffBuildUp(type, 255, buildUp, additionalPenentration);
	}

	/// <summary>
	/// Get the specific elemental debuff instance of this NPC.
	/// </summary>
	/// <param name="npc"></param>
	/// <param name="type"></param>
	/// <returns></returns>
	public static ElementalDebuffInstance GetElementalDebuff(this NPC npc, string type) =>
		npc.GetGlobalNPC<ElementalDebuffGlobalNPC>().ElementalDebuffs[type];

	public static ref StatModifier GetElementalResistance(this NPC npc, string type) =>
		ref npc.GetElementalDebuff(type).ElementalResistanceModifier;

	#endregion

	#region AI Utils

	/// <summary>
	/// Find the nearet npc from a given position.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="type"></param>
	/// <param name="maxDistance"></param>
	/// <returns></returns>
	public static NPC FindNearest(Vector2 position, int type, float maxDistance = 2048)
	{
		NPC target = null;
		float distanceMin = maxDistance;
		foreach (var npc in Main.npc)
		{
			if (npc is not null && npc.active && npc.type == type)
			{
				float distanceNPC = (npc.Center - position).Length();
				if (distanceNPC < distanceMin)
				{
					distanceMin = distanceNPC;
					target = npc;
				}
			}
		}
		return target;
	}

	/// <summary>
	/// The vanilla fighter AI.<br/>
	/// npc.ai[0] is a timer for change direction.<br/>
	/// npc.ai[2] is the duration of opening a door, >=60 the door will open.<br/>
	/// npc.ai[3] is a value of aggro.<br/>
	/// canOpenTheDoor only valid no tile over the top.<br/>
	/// </summary>
	/// <param name="npc"></param>
	public static void Vanilla_NPC_AI_003_Fighters(NPC npc, float maxSpeedX, bool canOpenTheDoor = true, int aggroThreshold = 60, bool tendToDespawn = false)
	{
		if (npc.target < 0)
		{
			npc.velocity.X *= 0;
			return;
		}
		Player player = Main.player[npc.target];
		if (player.position.Y + player.height == npc.position.Y + npc.height)
		{
			npc.directionY = -1;
		}

		// Accumulate aggro.
		Fighters_AccumulateAggro(npc, aggroThreshold);

		// Find target and change direction, check despawn.
		Fighters_FindTarget(npc, aggroThreshold, tendToDespawn);

		bool canJump = false;
		if (npc.velocity.X == 0f)
		{
			canJump = true;
		}
		if (npc.justHit)
		{
			canJump = false;
		}
		Fighters_Move(npc, maxSpeedX, canOpenTheDoor, canJump);
	}

	public static void Fighters_AccumulateAggro(NPC npc, int aggroThreshold)
	{
		Player player = Main.player[npc.target];
		bool ceasedOrMovingInWrongDirection = false;
		bool hittingDoor = false;
		if (npc.ai[2] > 0f)
		{
			hittingDoor = true;
		}
		if (!hittingDoor)
		{
			if (npc.velocity.Y == 0f && ((npc.velocity.X > 0f && npc.direction < 0) || (npc.velocity.X < 0f && npc.direction > 0)))
			{
				ceasedOrMovingInWrongDirection = true;
			}
			if (npc.position.X == npc.oldPosition.X || npc.ai[3] >= aggroThreshold || ceasedOrMovingInWrongDirection)
			{
				npc.ai[3] += 1f;
			}
			else if (Math.Abs(npc.velocity.X) > 0.9f && npc.ai[3] > 0f)
			{
				npc.ai[3] -= 1f;
			}
			if (npc.ai[3] > aggroThreshold * 10)
			{
				npc.ai[3] = 0f;
			}
			if (npc.justHit)
			{
				npc.ai[3] = 0f;
			}
			if (npc.ai[3] == aggroThreshold)
			{
				npc.netUpdate = true;
			}
			if (player.Hitbox.Intersects(npc.Hitbox))
			{
				npc.ai[3] = 0f;
			}
		}
	}

	public static void Fighters_FindTarget(NPC npc, int aggroThreshold, bool tendToDespawn)
	{
		Player player = Main.player[npc.target];
		if (npc.ai[3] < aggroThreshold)
		{
			npc.TargetClosest(true);
			if (npc.target < 0)
			{
				npc.velocity.X *= 0;
				return;
			}
			if (npc.directionY > 0 && player.Center.Y <= npc.Bottom.Y)
			{
				npc.directionY = -1;
			}
		}
		else if (npc.ai[2] <= 0f)
		{
			if (tendToDespawn)
			{
				npc.EncourageDespawn(10);
			}
			if (npc.velocity.X == 0f)
			{
				if (npc.velocity.Y == 0f)
				{
					npc.ai[0] += 1f;
					if (npc.ai[0] >= 2f)
					{
						npc.direction *= -1;
						npc.spriteDirection = npc.direction;
						npc.ai[0] = 0f;
					}
				}
			}
			else
			{
				npc.ai[0] = 0f;
			}
			if (npc.direction == 0)
			{
				npc.direction = 1;
			}
		}
	}

	public static void Fighters_Move(NPC npc, float maxSpeedX, bool canOpenTheDoor, bool canJump)
	{
		// Calculate velocity (Accelerate).
		if (npc.velocity.X < -maxSpeedX || npc.velocity.X > maxSpeedX)
		{
			if (npc.velocity.Y == 0f)
			{
				npc.velocity *= 0.8f;
			}
		}
		else if (npc.velocity.X < maxSpeedX && npc.direction == 1)
		{
			npc.velocity.X += 0.07f;
			if (npc.velocity.X > maxSpeedX)
			{
				npc.velocity.X = maxSpeedX;
			}
		}
		else if (npc.velocity.X > -maxSpeedX && npc.direction == -1)
		{
			npc.velocity.X -= 0.07f;
			if (npc.velocity.X < -maxSpeedX)
			{
				npc.velocity.X = -maxSpeedX;
			}
		}

		// Check if hit the ceiling;
		bool noBlockOverTop = false;
		if (npc.velocity.Y == 0f)
		{
			int bottomTileY = (int)(npc.position.Y + npc.height + 7f) / 16;
			int topTileY = (int)(npc.position.Y - 9f) / 16;
			int leftTileX = (int)npc.position.X / 16;
			int rightTileX = (int)(npc.position.X + npc.width) / 16;
			int left_inside_TileX = (int)(npc.position.X + 8f) / 16;
			int right_inside_TileX = (int)(npc.position.X + npc.width - 8f) / 16;
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
			if (!noBlockOverTop && npc.velocity.Y < 0f)
			{
				npc.velocity.Y = 0f;
			}
		}

		// Step tile
		if (npc.velocity.Y >= 0f && npc.directionY != 1)
		{
			int velocityDirection = Math.Sign(npc.velocity.X);
			Vector2 nextPos = npc.position;
			nextPos.X += npc.velocity.X;
			int collisionBoundX = (int)((nextPos.X + npc.width / 2 + (npc.width / 2 + 1) * velocityDirection) / 16f);
			int collisionBottomY = (int)((nextPos.Y + npc.height - 1f) / 16f);
			if (WorldGen.InWorld(collisionBoundX, collisionBottomY, 4))
			{
				if (collisionBoundX * 16 < nextPos.X + npc.width && collisionBoundX * 16 + 16 > nextPos.X && ((Main.tile[collisionBoundX, collisionBottomY].HasUnactuatedTile && !Main.tile[collisionBoundX, collisionBottomY].topSlope() && !Main.tile[collisionBoundX, collisionBottomY - 1].topSlope() && Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY].TileType] && !Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY].TileType]) || (Main.tile[collisionBoundX, collisionBottomY - 1].halfBrick() && Main.tile[collisionBoundX, collisionBottomY - 1].HasUnactuatedTile)) && (!Main.tile[collisionBoundX, collisionBottomY - 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 1].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 1].TileType] || (Main.tile[collisionBoundX, collisionBottomY - 1].halfBrick() && (!Main.tile[collisionBoundX, collisionBottomY - 4].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 4].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 4].TileType]))) && (!Main.tile[collisionBoundX, collisionBottomY - 2].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 2].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 2].TileType]) && (!Main.tile[collisionBoundX, collisionBottomY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX, collisionBottomY - 3].TileType] || Main.tileSolidTop[Main.tile[collisionBoundX, collisionBottomY - 3].TileType]) && (!Main.tile[collisionBoundX - velocityDirection, collisionBottomY - 3].HasUnactuatedTile || !Main.tileSolid[Main.tile[collisionBoundX - velocityDirection, collisionBottomY - 3].TileType]))
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
					if (bottomWorldY < nextPos.Y + npc.height)
					{
						float num188 = nextPos.Y + npc.height - bottomWorldY;
						float num189 = 16.1f;
						if (num188 <= num189)
						{
							npc.gfxOffY += npc.position.Y + npc.height - bottomWorldY;
							npc.position.Y = bottomWorldY - npc.height;
							npc.stepSpeed = num188 < 9f ? 1f : 2f;
						}
					}
				}
			}
		}

		// Obstruction
		if (noBlockOverTop)
		{
			int pushTileX = (int)((npc.position.X + npc.width / 2 + (npc.width / 2f + 4) * npc.direction) / 16f);
			int pushTileY = (int)((npc.position.Y + npc.height - 15f) / 16f);
			var pushTile = TileUtils.SafeGetTile(pushTileX, pushTileY - 1);
			if (pushTile.HasUnactuatedTile && (TileLoader.IsClosedDoor(pushTile) || pushTile.TileType == TileID.TallGateClosed))
			{
				Fighters_BreakDoor(npc, pushTileX, pushTileY, canOpenTheDoor);
			}
			else
			{
				Fighters_Jump(npc, pushTileX, pushTileY, canJump);
			}
		}
	}

	public static void Fighters_BreakDoor(NPC npc, int pushTileX, int pushTileY, bool canOpenTheDoor)
	{
		int aggroThreshold = 60;
		npc.ai[2] += 1f;
		npc.ai[3] = 0f;
		if (npc.ai[2] >= 60f)
		{
			npc.velocity.X = 0.5f * (float)(-(float)npc.direction);
			WorldGen.KillTile(pushTileX, pushTileY - 1, true, false, false);
			if ((Main.netMode != NetmodeID.MultiplayerClient || !canOpenTheDoor) && canOpenTheDoor && Main.netMode != NetmodeID.MultiplayerClient)
			{
				npc.ai[2] = 0;
				if (TileLoader.IsClosedDoor(Main.tile[pushTileX, pushTileY - 1]))
				{
					bool openSuccess = WorldGen.OpenDoor(pushTileX, pushTileY - 1, npc.direction);
					if (!openSuccess)
					{
						npc.ai[3] = aggroThreshold;
						npc.netUpdate = true;
					}
					if (Main.netMode == NetmodeID.Server && openSuccess)
					{
						NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 0, pushTileX, pushTileY - 1, npc.direction, 0, 0, 0);
					}
				}
				if (Main.tile[pushTileX, pushTileY - 1].TileType == TileID.TallGateClosed)
				{
					bool openSuccess = WorldGen.ShiftTallGate(pushTileX, pushTileY - 1, false, false);
					if (!openSuccess)
					{
						npc.ai[3] = aggroThreshold;
						npc.netUpdate = true;
					}
					if (Main.netMode == NetmodeID.Server && openSuccess)
					{
						NetMessage.SendData(MessageID.ToggleDoorState, -1, -1, null, 4, pushTileX, pushTileY - 1, 0f, 0, 0, 0);
					}
				}
			}
		}
	}

	public static void Fighters_Jump(NPC npc, int pushTileX, int pushTileY, bool canJump)
	{
		Player player = Main.player[npc.target];
		if ((npc.velocity.X < 0f && npc.spriteDirection == -1) || (npc.velocity.X > 0f && npc.spriteDirection == 1))
		{
			// Jump over obstacles.
			if (npc.height >= 32 && Main.tile[pushTileX, pushTileY - 2].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 2].TileType])
			{
				if (Main.tile[pushTileX, pushTileY - 3].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 3].TileType])
				{
					npc.velocity.Y = -8f;
					npc.netUpdate = true;
				}
				else
				{
					npc.velocity.Y = -7f;
					npc.netUpdate = true;
				}
			}
			else if (Main.tile[pushTileX, pushTileY - 1].HasUnactuatedTile && Main.tileSolid[Main.tile[pushTileX, pushTileY - 1].TileType])
			{
				npc.velocity.Y = -6f;
				npc.netUpdate = true;
			}
			else if (npc.position.Y + npc.height - pushTileY * 16 > 20f && Main.tile[pushTileX, pushTileY].HasUnactuatedTile && !Main.tile[pushTileX, pushTileY].TopSlope && Main.tileSolid[Main.tile[pushTileX, pushTileY].TileType])
			{
				npc.velocity.Y = -5f;
				npc.netUpdate = true;
			}
			else if (npc.directionY < 0 && (!Main.tile[pushTileX, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX, pushTileY + 1].TileType]) && (!Main.tile[pushTileX + npc.direction, pushTileY + 1].HasUnactuatedTile || !Main.tileSolid[Main.tile[pushTileX + npc.direction, pushTileY + 1].TileType]))
			{
				npc.velocity.Y = -8f;
				npc.velocity.X *= 1.5f;
				npc.netUpdate = true;
			}

			// Jump when starting to accumulate aggro.
			if (npc.velocity.Y == 0f && canJump && npc.ai[3] == 1f)
			{
				npc.velocity.Y = -5f;
			}
			if (npc.velocity.Y == 0f && Main.expertMode && player.Bottom.Y < npc.Top.Y && Math.Abs(npc.Center.X - player.Center.X) < player.width * 3 && Collision.CanHit(npc, player))
			{
				int maxTileOverHead = 6;
				if (player.Bottom.Y > npc.Top.Y - maxTileOverHead * 16)
				{
					npc.velocity.Y = -7.9f;
				}
				else
				{
					int centerTileX = (int)(npc.Center.X / 16f);
					int bottomTileY = (int)(npc.Bottom.Y / 16f) - 1;
					for (int y = bottomTileY; y > bottomTileY - maxTileOverHead; y--)
					{
						if (Main.tile[centerTileX, y].HasUnactuatedTile && TileID.Sets.Platforms[Main.tile[centerTileX, y].TileType])
						{
							npc.velocity.Y = -7.9f;
							break;
						}
					}
				}
			}
		}
	}
	#endregion
}
