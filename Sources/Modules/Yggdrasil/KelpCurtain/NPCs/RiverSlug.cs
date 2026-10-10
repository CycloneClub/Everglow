using Everglow.Yggdrasil.KelpCurtain.Dusts;
using Everglow.Yggdrasil.KelpCurtain.Items.Critters;
using Terraria;
using Terraria.ModLoader.Utilities;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs;

public class RiverSlug : ModNPC
{
	private const float BaseSpeed = 0.4f;
	private const float RotationSpeed = 0.1f;

	private const int StuckDetectionTimerMax = 10;
	private const int NotCollideTimerMax = 5;
	private const int FallFromBlockMax = 2;
	private const int FallChance = 6000;

	private enum ActionState
	{
		None,
		Turn,
		Move,
	}

	private enum MovementState
	{
		Horizontally,
		Vertically,
	}

	private ActionState NPCActionState
	{
		get => (ActionState)(int)NPC.ai[0];
		set => NPC.ai[0] = (int)value;
	}

	private MovementState NPCMovementState
	{
		get => (MovementState)(int)NPC.ai[1];
		set => NPC.ai[1] = (int)value;
	}

	public int FallFromBlockCounter
	{
		get => (int)NPC.ai[2];
		set => NPC.ai[2] = value;
	}

	public ref float OldPositionX => ref NPC.localAI[1];

	public int StuckDetectionTimer
	{
		get => (int)NPC.localAI[2];
		set => NPC.localAI[2] = value;
	}

	public int NotCollidedTimer
	{
		get => (int)NPC.localAI[3];
		set => NPC.localAI[3] = value;
	}

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 4;
		Main.npcCatchable[NPC.type] = true;
		NPCID.Sets.CountsAsCritter[NPC.type] = true;
	}

	public override void SetDefaults()
	{
		NPC.CloneDefaults(NPCID.GlowingSnail);

		NPC.width = 20; // 36;
		NPC.height = 20; // 26;

		NPC.life = 5;
		NPC.damage = 10;
		NPC.defense = 0;
		NPC.knockBackResist = 0f;

		NPC.npcSlots = 0.5f;

		NPC.aiStyle = -1;
		NPC.catchItem = ModContent.ItemType<RiverSlugItem>();
		SpawnModBiomes = [ModContent.GetInstance<KelpCurtainBiome>().Type];
	}

	public override void FindFrame(int frameHeight)
	{
		float animationSpeed = 0.1f;
		NPC.frameCounter += animationSpeed;
		NPC.frameCounter %= Main.npcFrameCount[NPC.type];
		NPC.frame.Y = (int)NPC.frameCounter * frameHeight;
	}

	public override void AI()
	{
		// Check if npc is in shimmer, and apply shimmer effect.
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			int centerTileX = (int)MathHelper.Clamp((int)(NPC.Center.X / 16f), 0f, Main.maxTilesX);
			int centerTileY = (int)MathHelper.Clamp((int)(NPC.Center.Y / 16f), 0f, Main.maxTilesY);
			Tile centerTile = Main.tile[centerTileX, centerTileY];
			if (centerTile.shimmer() && centerTile.LiquidAmount > 30)
			{
				NPC.GetShimmered();
				return;
			}
		}

		float velocity = BaseSpeed;
		bool canRotate = true;

		// Look for player as target
		if (NPCActionState == ActionState.None)
		{
			NPC.TargetClosest();
			NPC.directionY = 1;
			NPC.spriteDirection = NPC.direction;

			NPCActionState = ActionState.Turn;
		}

		// Set fall state
		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			// Randomly set fall to 2f
			if (FallFromBlockCounter == 0f && Main.rand.NextBool(FallChance))
			{
				FallFromBlockCounter = FallFromBlockMax;
				NPC.netUpdate = true;
			}

			// Track how long the NPC hasn't collided. If more than a certainTime, set fall to 2f.
			if (!NPC.collideX && !NPC.collideY)
			{
				NotCollidedTimer++;
				if (NotCollidedTimer > NotCollideTimerMax)
				{
					FallFromBlockCounter = FallFromBlockMax;
					NPC.netUpdate = true;
				}
			}
			else
			{
				NotCollidedTimer = 0;
			}
		}

		if (FallFromBlockCounter > 0f)
		{
			NPCMovementState = MovementState.Horizontally;
			NPCActionState = ActionState.Turn;
			NPC.directionY = 1;
			if (NPC.velocity.Y > velocity)
			{
				NPC.rotation += NPC.direction * 0.1f;
			}
			else
			{
				NPC.rotation = 0f;
			}

			NPC.spriteDirection = NPC.direction;
			NPC.velocity.X = velocity * NPC.direction;
			NPC.noGravity = false;

			// Check if the back bottom tile's top slope is true and npc is colliding with it on Y axis,
			// If true, then decrease the fall counter.
			int frontTileX = (int)(NPC.Center.X + NPC.width / 2 * -NPC.direction) / 16;
			int frontTileY = (int)(NPC.Bottom.Y + 8f) / 16; // The 8f offset determines the top edge of block is unter the bottom edge of npc.
			if (Main.tile[frontTileX, frontTileY] != null && !Main.tile[frontTileX, frontTileY].TopSlope && NPC.collideY)
			{
				FallFromBlockCounter--;
			}

			// Check Front bottom tile's bottom slope is false, then reverse direction.
			int backTileX = (int)(NPC.Center.X + NPC.width / 2 * NPC.direction) / 16;
			int backTileY = (int)(NPC.Bottom.Y - 4f) / 16; // The -4f offset determines the top edge of block is over the bottom edge of npc, and the bottom edge is under the bottom edge of npc.
			if (Main.tile[frontTileX, frontTileY] != null && Main.tile[backTileX, backTileY].BottomSlope)
			{
				NPC.direction *= -1;
			}

			if (NPC.collideX && NPC.velocity.Y == 0f)
			{
				canRotate = false;
				FallFromBlockCounter = 0;
				NPC.directionY = -1;
				NPCMovementState = MovementState.Vertically;
			}

			// If npc is stucked within blocks, keep direction to right to prevent npc from shaking in place.
			if (NPC.velocity.Y == 0f)
			{
				if (OldPositionX == NPC.position.X)
				{
					StuckDetectionTimer++;
					if (StuckDetectionTimer > StuckDetectionTimerMax)
					{
						NPC.direction = 1;
						NPC.velocity.X = NPC.direction * velocity;
						StuckDetectionTimer = 0;
					}
				}
				else
				{
					StuckDetectionTimer = 0;
					OldPositionX = NPC.position.X;
				}
			}
		}

		if (FallFromBlockCounter != 0f)
		{
			return;
		}

		NPC.noGravity = true;

		if (NPCMovementState == MovementState.Horizontally)
		{
			if (NPC.collideY)
			{
				NPCActionState = ActionState.Move;
			}

			if (!NPC.collideY && NPCActionState == ActionState.Move)
			{
				NPC.direction = -NPC.direction;
				NPCMovementState = MovementState.Vertically;
				NPCActionState = ActionState.Turn;
			}

			if (NPC.collideX)
			{
				NPC.directionY = -NPC.directionY;
				NPCMovementState = MovementState.Vertically;
			}
		}
		else
		{
			if (NPC.collideX)
			{
				NPCActionState = ActionState.Move;
			}

			if (!NPC.collideX && NPCActionState == ActionState.Move)
			{
				NPC.directionY = -NPC.directionY;
				NPCMovementState = MovementState.Horizontally;
				NPCActionState = ActionState.Turn;
			}

			if (NPC.collideY)
			{
				NPC.direction = -NPC.direction;
				NPCMovementState = MovementState.Horizontally;
			}
		}

		// Update rotation
		if (canRotate)
		{
			float initialRotation = NPC.rotation;
			if (NPC.directionY < 0)
			{
				if (NPC.direction < 0)
				{
					if (NPC.collideX)
					{
						NPC.rotation = MathHelper.PiOver2;
						NPC.spriteDirection = -1;
					}
					else if (NPC.collideY)
					{
						NPC.rotation = MathHelper.Pi;
						NPC.spriteDirection = 1;
					}
				}
				else if (NPC.collideY)
				{
					NPC.rotation = MathHelper.Pi;
					NPC.spriteDirection = -1;
				}
				else if (NPC.collideX)
				{
					NPC.rotation = MathHelper.Pi + MathHelper.PiOver2;
					NPC.spriteDirection = 1;
				}
			}
			else if (NPC.direction < 0)
			{
				if (NPC.collideY)
				{
					NPC.rotation = 0f;
					NPC.spriteDirection = -1;
				}
				else if (NPC.collideX)
				{
					NPC.rotation = MathHelper.PiOver2;
					NPC.spriteDirection = 1;
				}
			}
			else if (NPC.collideX)
			{
				NPC.rotation = MathHelper.Pi + MathHelper.PiOver2;
				NPC.spriteDirection = -1;
			}
			else if (NPC.collideY)
			{
				NPC.rotation = 0f;
				NPC.spriteDirection = 1;
			}

			float updatedRotation = NPC.rotation;
			NPC.rotation = initialRotation;
			if (NPC.rotation > MathHelper.TwoPi)
			{
				NPC.rotation -= MathHelper.TwoPi;
			}
			else if (NPC.rotation < 0f)
			{
				NPC.rotation += MathHelper.TwoPi;
			}

			float rotationDiff = Math.Abs(NPC.rotation - updatedRotation);
			if (NPC.rotation > updatedRotation)
			{
				if (rotationDiff > MathHelper.Pi)
				{
					NPC.rotation += RotationSpeed;
				}
				else
				{
					NPC.rotation -= RotationSpeed;
					if (NPC.rotation < updatedRotation)
					{
						NPC.rotation = updatedRotation;
					}
				}
			}

			if (NPC.rotation < updatedRotation)
			{
				if (rotationDiff > MathHelper.Pi)
				{
					NPC.rotation -= RotationSpeed;
				}
				else
				{
					NPC.rotation += RotationSpeed;
					if (NPC.rotation > updatedRotation)
					{
						NPC.rotation = updatedRotation;
					}
				}
			}
		}

		NPC.velocity.X = velocity * NPC.direction;
		NPC.velocity.Y = velocity * NPC.directionY;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life <= 0)
		{
			for (int i = 0; i < 6; i++)
			{
				int dust = Dust.NewDust(NPC.position, NPC.width, NPC.height, ModContent.DustType<LichenSlime>(), 2 * hit.HitDirection, -2f, newColor: Color.Blue);
				if (Main.rand.NextBool())
				{
					Main.dust[dust].noGravity = true;
					Main.dust[dust].scale = 1.2f * NPC.scale;
				}
				else
				{
					Main.dust[dust].scale = 0.7f * NPC.scale;
				}
			}
		}
	}

	public override float SpawnChance(NPC.Spawner spawner)
	{
		if (spawner.Player.AnyEvent())
		{
			return 0f;
		}

		return SpawnCondition.TownCritter.Chance;
	}

	public override bool CanBeHitByNPC(NPC attacker) => attacker.type != NPC.type; // Prevent attack from other river slugs.

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = ModContent.Request<Texture2D>(Texture).Value;
		var frame = texture.Frame(verticalFrames: 4, frameY: (int)NPC.frameCounter);
		var rotation = NPC.rotation;
		var spriteEffect = NPC.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, frame, drawColor, rotation, frame.Size() / 2, 0.8f, spriteEffect, 0);

		return false;
	}
}
