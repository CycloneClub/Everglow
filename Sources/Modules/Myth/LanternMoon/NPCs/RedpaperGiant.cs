using Everglow.Commons.DataStructures;
using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Spine;

namespace Everglow.Myth.LanternMoon.NPCs;

public class RedpaperGiant : LanternMoonNPC
{
	public int State;

	public int Timer;

	public int FlipTimer;

	public int FlyTimer = 0;

	public int FlyCooling = 0;

	public int AttackTimer = 0;

	public int MaxAttackTime = 45;

	public int TeleportTimer = 0;

	public int TeleportCooling = 0;

	public int MaxTeleportTime = 36;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 23;
	}

	public enum BehaviorState
	{
		CloseTarget,
		Attack,
		Teleporting,
	}

	public override void SetDefaults()
	{
		NPC.damage = 55;
		NPC.lifeMax = 450;
		NPC.npcSlots = 1f;
		NPC.width = 44;
		NPC.height = 120;
		NPC.defense = 0;
		NPC.value = 200;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 1.5f;
		NPC.dontTakeDamage = false;
		NPC.HitSound = SoundID.NPCHit3;
		NPC.noTileCollide = true;
		NPC.noGravity = true;
		LanternMoonScore = 8f;
		State = (int)BehaviorState.CloseTarget;
	}

	public override void FindFrame(int frameHeight)
	{
		frameHeight = 160;
		NPC.frameCounter++;
		if (State == (int)BehaviorState.CloseTarget)
		{
			int frameNumber = (int)(NPC.frameCounter / 8f) % 3;
			NPC.frame = new Rectangle(0, frameHeight * frameNumber, frameHeight, frameHeight);
		}
		if (State == (int)BehaviorState.Attack)
		{
			int frameNumber = (int)((MaxAttackTime - AttackTimer) / (float)MaxAttackTime * 13);
			if (frameNumber > 12)
			{
				frameNumber = 12;
			}
			frameNumber += 2;
			NPC.frame = new Rectangle(0, frameHeight * frameNumber, frameHeight, frameHeight);
		}
		if (State == (int)BehaviorState.Teleporting)
		{
			int frameNumber = (int)((MaxTeleportTime - TeleportTimer) / (float)MaxTeleportTime * 7);
			if (frameNumber > 6)
			{
				frameNumber = 6;
			}
			frameNumber += 15;
			NPC.frame = new Rectangle(0, frameHeight * frameNumber, frameHeight, frameHeight);
		}
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			return;
		}
		Timer++;
		NPC.TargetClosest(false);
		if (NPC.target == -1)
		{
			return;
		}
		if (State == (int)BehaviorState.Teleporting)
		{
		}
		else
		{
		}

		Player player = Main.player[NPC.target];
		switch (State)
		{
			case (int)BehaviorState.CloseTarget:
				{
					Lighting.AddLight(NPC.Center, 0.4f, 0.05f, 0.05f);
					int oldDir = NPC.direction;
					if (NPC.velocity.X > 0)
					{
						NPC.direction = 1;
					}
					else
					{
						NPC.direction = -1;
					}

					// Turn paper effect
					if (NPC.direction != oldDir)
					{
						FlipTimer = 15;
					}
					if (FlipTimer > 0)
					{
						FlipTimer--;
					}

					// Approach player
					Vector2 toTarget = player.Center - NPC.Center;
					if (toTarget.Length() < 75)
					{
						State = (int)BehaviorState.Attack;
						AttackTimer = MaxAttackTime;
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<RedpaperGiantAttackProj>(), 27, 0f, Main.myPlayer);
						RedpaperGiantAttackProj rGAP = p0.ModProjectile as RedpaperGiantAttackProj;
						if (rGAP is not null)
						{
							rGAP.OwnerNPC = NPC;
						}
					}

					// Fly
					if (MathF.Abs(toTarget.X) < 100 && MathF.Abs(toTarget.Y) > 200 && FlyCooling <= 0)
					{
						FlyTimer = Main.rand.Next(120, 240);
						FlyCooling = Main.rand.Next(260, 500);
					}

					// Teleport
					if ((MathF.Abs(toTarget.X) > 200 || toTarget.Length() > 300 || (toTarget.Length() < 100 && FlyCooling > 0)) && TeleportCooling <= 0)
					{
						TeleportTimer = MaxTeleportTime;
						TeleportCooling = Main.rand.Next(260, 300);
						State = (int)BehaviorState.Teleporting;
					}
					if (TeleportCooling > 0)
					{
						TeleportCooling--;
					}
					toTarget = toTarget.SafeNormalize(Vector2.Zero);
					Vector2 aimVel = toTarget * 6;
					if (FlyTimer <= 0)
					{
						toTarget = player.Center - NPC.Center + new Vector2(300 * MathF.Cos(Timer * 0.01f + NPC.whoAmI), 0);
						toTarget = toTarget.SafeNormalize(Vector2.Zero);
						aimVel = toTarget * 6;
						float aimFloatHeight = 30 + MathF.Sin(Timer * 0.006f + NPC.whoAmI) * 15;
						float disparity = OverTileHeight(NPC.Center) - aimFloatHeight;
						disparity *= 0.15f;
						disparity = disparity.Clamp(-8, 8);
						aimVel += new Vector2(0, disparity);
						if (FlyCooling > 0)
						{
							FlyCooling--;
						}
					}
					else
					{
						FlyTimer--;
					}
					NPC.velocity = aimVel * 0.05f + NPC.velocity * 0.95f;
					foreach (NPC npc in Main.npc)
					{
						if (npc != null && npc.active && npc != NPC)
						{
							if (npc.type == Type)
							{
								Vector2 v0 = NPC.Center - npc.Center;
								if (v0.Length() < 60)
								{
									NPC.velocity += Vector2.Normalize(v0) * 2.5f;
								}
							}
						}
					}
					break;
				}
			case (int)BehaviorState.Attack:
				{
					Lighting.AddLight(NPC.Center, 0.4f, 0.05f, 0.05f);
					FlipTimer = 0;
					AttackTimer--;
					if (AttackTimer <= 0)
					{
						State = (int)BehaviorState.CloseTarget;
					}
					NPC.velocity *= 0.96f;
					int frameNumber = (int)((MaxAttackTime - AttackTimer) / (float)MaxAttackTime * 13);
					if (frameNumber > 12)
					{
						frameNumber = 12;
					}
					frameNumber += 3;
					if (frameNumber is 7 or 10 or 11 or 13)
					{
						Lighting.AddLight(NPC.Center, new Vector3(1f, 0f, 0f));
					}
					else
					{
						Lighting.AddLight(NPC.Center, new Vector3(0.3f, 0f, 0f));
					}
					break;
				}

			case (int)BehaviorState.Teleporting:
				{
					FlipTimer = 0;
					TeleportTimer--;
					if (TeleportTimer <= 0)
					{
						State = (int)BehaviorState.CloseTarget;
					}
					NPC.velocity *= 0;
					if (TeleportTimer == 18)
					{
						float dis = Main.rand.NextFloat(120, 180);
						if (FlyCooling > 0)
						{
							dis = Main.rand.NextFloat(320, 380);
						}
						Vector2 teleportPos = player.Center + new Vector2(dis * (Main.rand.NextBool() ? -1 : 1), 0);
						teleportPos.Y += OverTileHeight(teleportPos) - 30;
						NPC.Center = teleportPos;
					}
					Vector3 lightColor = new Vector3(0.4f, 0.05f, 0.05f) * ((float)Math.Abs(TeleportTimer - MaxTeleportTime / 2) / (MaxTeleportTime / 2));
					Lighting.AddLight(NPC.Center, lightColor);
					break;
				}
		}
	}

	public float OverTileHeight(Vector2 pos)
	{
		float value = 0f;
		int checkCount = 0;
		while (Collision.IsWorldPointSolid(pos + new Vector2(0, value)))
		{
			value -= 8;
			checkCount++;
			if (checkCount > 100)
			{
				return -9999999;
			}
		}
		if (checkCount > 0)
		{
			return value + 8;
		}
		while (!Collision.IsWorldPointSolid(pos + new Vector2(0, value)))
		{
			value += 8;
			checkCount++;
			if (checkCount > 100)
			{
				return 9999999;
			}
		}
		return value - 8;
	}

	public override void OnKill()
	{
		for (int g = 0; g < 9; g++)
		{
			if (Main.rand.NextBool(2))
			{
				Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 6f, 0).RotatedByRandom(MathHelper.TwoPi);
				string texturePath = ModAsset.RedpaperGiantGore_0_Mod;
				if (texturePath is not null)
				{
					texturePath = texturePath.Remove(texturePath.Length - 1, 1);
					texturePath += g;
				}
				var gore = new PaperGore
				{
					LightValue = 0.15f,
					Velocity = vel,
					Position = NPC.Center + vel,
					Texture = ModContent.Request<Texture2D>(texturePath).Value,
					RotateSpeed = vel.X / 8f,
					Scale = Main.rand.NextFloat(1f, 1.25f),
					MaxTime = Main.rand.Next(120, 360),
					Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
					ai = new float[] { Main.rand.NextFloat(-2, 2), Main.rand.NextFloat(0, MathF.PI) },
				};
				Ins.VFXManager.Add(gore);
			}
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		NPC.spriteDirection = NPC.direction;
		Texture2D texture = ModAsset.RedpaperGiant.Value;
		Texture2D texture_attack = ModAsset.RedpaperGiant_Attack.Value;
		SpriteBatchState sBS = spriteBatch.GetState().Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, new Vector2(-NPC.spriteDirection * (7.5f - FlipTimer) / 7.5f, NPC.scale), SpriteEffects.None, 0);
		spriteBatch.Draw(texture_attack, NPC.Center - Main.screenPosition, NPC.frame, Color.Lerp(Color.White, drawColor, 0.7f), NPC.rotation, NPC.frame.Size() * 0.5f, new Vector2(-NPC.spriteDirection * (7.5f - FlipTimer) / 7.5f, NPC.scale), SpriteEffects.None, 0);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		spriteBatch.End();
		spriteBatch.Begin(sBS);
		return false;
	}
}
