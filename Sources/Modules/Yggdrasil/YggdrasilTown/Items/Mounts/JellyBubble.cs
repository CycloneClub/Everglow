using Everglow.Yggdrasil.YggdrasilTown.Buffs.Mounts;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.Items.Mounts;

public class JellyBubble : ModMount
{
	public const int Distance = 12;
	public const int MaxMountStamina = 120;
	public const float GravityAcceleration = 5f;
	public const float BuoyancyAcceleration = 8f;
	public int Stamina = 120;
	public float FlyGlowValue = 0f;

	public override void SetStaticDefaults()
	{
		// Movement
		MountData.heightBoost = 20;
		MountData.flightTimeMax = int.MaxValue - 1;
		MountData.fatigueMax = int.MaxValue - 1;
		MountData.fallDamage = 0f;
		MountData.usesHover = true;

		MountData.runSpeed = 11f;
		MountData.dashSpeed = 8f;
		MountData.acceleration = 0.08f;

		MountData.jumpHeight = 5;
		MountData.jumpSpeed = 4f;
		MountData.blockExtraJumps = true;
		MountData.constantJump = false;

		// Misc
		MountData.buff = ModContent.BuffType<JellyBubbleMount>();

		// Visual Effects
		MountData.spawnDust = ModContent.DustType<Dusts.JellyBallGel>();
		MountData.spawnDustNoGravity = true;

		// Frame data and player offsets
		MountData.totalFrames = 4;
		MountData.playerYOffsets = Enumerable.Repeat(20, MountData.totalFrames).ToArray();
		MountData.xOffset = 13;
		MountData.yOffset = -12;
		MountData.playerHeadOffset = 22;
		MountData.bodyFrame = 3;

		// Standing
		MountData.standingFrameCount = 4;
		MountData.standingFrameDelay = 12;
		MountData.standingFrameStart = 0;

		// Running
		MountData.runningFrameCount = 4;
		MountData.runningFrameDelay = 12;
		MountData.runningFrameStart = 0;

		// Flying
		MountData.flyingFrameCount = 4;
		MountData.flyingFrameDelay = 12;
		MountData.flyingFrameStart = 0;

		// In-air
		MountData.inAirFrameCount = 4;
		MountData.inAirFrameDelay = 12;
		MountData.inAirFrameStart = 0;

		// Idle
		MountData.idleFrameCount = 4;
		MountData.idleFrameDelay = 12;
		MountData.idleFrameStart = 0;
		MountData.idleFrameLoop = true;

		// Swim
		MountData.swimFrameCount = MountData.inAirFrameCount;
		MountData.swimFrameDelay = MountData.inAirFrameDelay;
		MountData.swimFrameStart = MountData.inAirFrameStart;

		if (Main.netMode != NetmodeID.Server)
		{
			MountData.textureWidth = MountData.backTexture.Width();
			MountData.textureHeight = MountData.backTexture.Height();
		}
	}

	public override void UpdateEffects(Player player)
	{
		// Update the max speed of mount to player's max speed
		MountData.runSpeed = player.maxRunSpeed;
		MountData.dashSpeed = player.maxRunSpeed;

		// Buoyancy simulation
		var bottomTilePosition = FindSentryRestingSpot(player, player.MountedCenter, out int xPosition, out int yPosition);
		var playerIsNotMovingOnY = !(player.controlUp || player.controlDown);
		if (playerIsNotMovingOnY)
		{
			var distanceToBottomTile = bottomTilePosition.Y - player.MountedCenter.Y;
			var offset = MathF.Abs(MountData.heightBoost) / (16f * 2f);
			var buoyancyFact = 1 - distanceToBottomTile.SmoothStep((Distance - offset) * 16f, (Distance + offset) * 16f);
			var acceleration = (GravityAcceleration - BuoyancyAcceleration * buoyancyFact) * 0.1f;
			player.velocity.Y += acceleration;

			// Lower than 300, regen stamina.
			if (distanceToBottomTile < 300)
			{
				if (Stamina < MaxMountStamina)
				{
					Stamina++;
				}
				else
				{
					Stamina = MaxMountStamina;
					if (FlyGlowValue > 0f)
					{
						FlyGlowValue -= 0.05f;
					}
					else
					{
						FlyGlowValue = 0f;
					}
				}
			}
		}
		if (player.controlUp)
		{
			Stamina--;
			if (Stamina <= 0)
			{
				Stamina = 0;
				player.mount.Dismount(player);
			}
			if (FlyGlowValue < 1f)
			{
				FlyGlowValue += 0.05f;
			}
			else
			{
				FlyGlowValue = 1f;
			}
		}
		Lighting.AddLight(player.Center, new Vector3(0f, 0.4f, 1f));
	}

	public static Vector2 FindSentryRestingSpot(Player player, Vector2 position, out int worldX, out int worldY)
	{
		static void LimitPointToPlayerReachableArea(Player player, ref Vector2 pointPoisition)
		{
			Vector2 center = player.Center;
			Vector2 vector = pointPoisition - center;
			float num = Math.Abs(vector.X);
			float num2 = Math.Abs(vector.Y);
			float num3 = 1f;
			if (num > 960f)
			{
				float num4 = 960f / num;
				if (num3 > num4)
				{
					num3 = num4;
				}
			}

			if (num2 > 600f)
			{
				float num5 = 600f / num2;
				if (num3 > num5)
				{
					num3 = num5;
				}
			}

			Vector2 vector2 = vector * num3;
			pointPoisition = center + vector2;
		}

		Vector2 pointPoisition = position;
		LimitPointToPlayerReachableArea(player, ref pointPoisition);
		int num = (int)pointPoisition.X / 16;
		int i = (int)pointPoisition.Y / 16;
		worldX = num * 16 + 8;

		for (; i < Main.maxTilesY - 10
			 && Main.tile[num, i] != null
			 && !WorldGen.SolidTile2(num, i)
			 && Main.tile[num - 1, i] != null
			 && !WorldGen.SolidTile2(num - 1, i)
			 && Main.tile[num + 1, i] != null
			 && !WorldGen.SolidTile2(num + 1, i);
			i++)
		{
		}

		worldY = i * 16 - 14;

		return new(worldX, worldY);
	}

	public override bool Draw(List<DrawData> playerDrawData, int drawType, Player drawPlayer, ref Texture2D texture, ref Texture2D glowTexture, ref Vector2 drawPosition, ref Rectangle frame, ref Color drawColor, ref Color glowColor, ref float rotation, ref SpriteEffects spriteEffects, ref Vector2 drawOrigin, ref float drawScale, float shadow)
	{
		// Jelly Ball Part
		// ===============
		// Adjust the scale magnitude of jelly ball proportional to the speed of player
		var speedPercentForVFX = drawPlayer.velocity.Length().SmoothStep(0, drawPlayer.maxRunSpeed);
		var scaleMagnitudeFact = 0.1f + speedPercentForVFX * 0.1f;
		var scaleFrequencyFact = 0.11f;

		var jellyBallGlowTexture = ModAsset.JellyBubble_JellyBall_Glow.Value;
		var jellyBallGlowStaminaTexture = ModAsset.JellyBubble_JellyBall_glow2.Value;
		var jellyBallTexture = ModAsset.JellyBubble_JellyBall.Value;
		var jellyBallPosition = drawPlayer.Bottom - Main.screenPosition + new Vector2(0, -21f);
		var jellyBallGlowDrawColor = new Color(0f, 0.04f, 0.3f, 0f);
		var jellyBallDrawColor = drawColor * 3f;
		var jellyBallGlowOrigin = new Vector2(jellyBallGlowTexture.Width / 2, (jellyBallGlowTexture.Height - jellyBallTexture.Height) * 0.5f);
		var jellyBallOrigin = new Vector2(jellyBallTexture.Width / 2, 0);
		var jellyBallScale = new Vector2(
			1.6f * (0.9f + scaleMagnitudeFact) * (scaleMagnitudeFact * MathF.Cos((float)Main.time * scaleFrequencyFact) + (1 - scaleMagnitudeFact)),
			1.2f * (0.9f + scaleMagnitudeFact) * (scaleMagnitudeFact * MathF.Sin((float)Main.time * scaleFrequencyFact) + (1 - scaleMagnitudeFact)));
		var staminaValue = Stamina / (float)MaxMountStamina;
		var staminaColor = new Color(staminaValue, staminaValue + 0.3f, staminaValue * 1.8f + 0.3f, 0) * FlyGlowValue;
		Main.spriteBatch.Draw(jellyBallGlowTexture, jellyBallPosition, null, jellyBallGlowDrawColor, 0, jellyBallGlowOrigin, jellyBallScale, SpriteEffects.None, 0);
		Main.spriteBatch.Draw(jellyBallTexture, jellyBallPosition, null, jellyBallDrawColor, 0, jellyBallOrigin, jellyBallScale, SpriteEffects.None, 0);

		// Use a simple pixel shader to display rest stamina.
		var sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		Effect shader = ModAsset.JellyBallStamina.Value;
		shader.Parameters["duration"].SetValue(1 - staminaValue);
		shader.CurrentTechnique.Passes[0].Apply();
		Main.spriteBatch.Draw(jellyBallGlowStaminaTexture, jellyBallPosition, null, staminaColor, 0, jellyBallOrigin, jellyBallScale, SpriteEffects.None, 0);
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// Saddle Part
		// ===========
		var saddleTexture = ModAsset.JellyBubble_Saddle.Value;
		var saddlePosition = drawPlayer.Bottom - Main.screenPosition + new Vector2(0, -15f);
		var saddleDrawColor = drawColor;
		var saddleOrigin = new Vector2(saddleTexture.Width / 2, saddleTexture.Height);
		var saddleScale = 1.4f;
		Main.spriteBatch.Draw(saddleTexture, saddlePosition, null, saddleDrawColor, 0, saddleOrigin, saddleScale, SpriteEffects.None, 0);

		return true;
	}
}
