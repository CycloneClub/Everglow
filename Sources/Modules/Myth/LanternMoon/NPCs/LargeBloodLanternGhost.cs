using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Projectiles.PerWave15;
using Everglow.Myth.LanternMoon.VFX;

namespace Everglow.Myth.LanternMoon.NPCs;

public class LargeBloodLanternGhost : LanternMoonNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 8;
	}

	public enum BehaviorState
	{
		ClosePlayer,
		Summon,
		TentacleRelease,
	}

	public int State;

	public int Timer;

	public int ClosingTimeMax = 300;

	public Vector2 SummonPos = Vector2.zeroVector;

	public override void SetDefaults()
	{
		NPC.damage = 75;
		NPC.lifeMax = 1260;
		NPC.npcSlots = 2.5f;
		NPC.width = 80;
		NPC.height = 108;
		NPC.defense = 8;
		NPC.value = 200;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.05f;
		NPC.dontTakeDamage = false;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.HitSound = SoundID.NPCHit3;
		LanternMoonScore = 35f;
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frameCounter++;
		frameHeight = 142;
		if (NPC.frameCounter >= 6)
		{
			NPC.frameCounter = 0;
			NPC.frame.Y += frameHeight;
			if (NPC.frame.Y >= frameHeight * Main.npcFrameCount[NPC.type])
			{
				NPC.frame.Y = 0;
			}
		}
	}

	public override void AI()
	{
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			return;
		}
		NPC.TargetClosest(false);
		Player player = Main.player[NPC.target];
		Timer++;
		if (NPC.velocity.Length() > 1.5f)
		{
			NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
		}
		switch (State)
		{
			case (int)BehaviorState.ClosePlayer:
				{
					NPC.velocity *= 0.99f;
					var toTarget = player.Center - NPC.Center - NPC.velocity;
					if (toTarget.Length() > 130)
					{
						NPC.velocity = toTarget.SafeNormalize(Vector2.Zero) * 4 * 0.1f + NPC.velocity * 0.9f;
					}
					else
					{
						if (NPC.velocity.Length() > 5f)
						{
							NPC.velocity *= 0.95f;
						}
						SwitchState();
					}
					if (toTarget.Length() > 300)
					{
						if (NPC.velocity.Length() < 6)
						{
							NPC.velocity *= 1.3f;
						}
					}
					if (toTarget.Length() > 1000)
					{
						if (NPC.velocity.Length() < 15)
						{
							NPC.velocity *= 1.3f;
						}
					}
					if (Timer > ClosingTimeMax)
					{
						SwitchState();
					}
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
			case (int)BehaviorState.Summon:
				{
					if (Timer < 35)
					{
						if (SummonPos != Vector2.zeroVector)
						{
							NPC.Center = Vector2.Lerp(NPC.Center, player.Center + SummonPos, 0.05f);
						}
						NPC.velocity *= 0.95f;
					}
					else
					{
						NPC.velocity *= 0;
						if (Timer == 40)
						{
							Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<LargeBloodLanternGhost_Matrix_Summon>(), 0, 0f, -1);
							LargeBloodLanternGhost_Matrix_Summon lBLGMS = p1.ModProjectile as LargeBloodLanternGhost_Matrix_Summon;
							if (lBLGMS is not null)
							{
								lBLGMS.OwnerNPC = NPC;
							}
						}
						if (Timer > 200)
						{
							SwitchState();
						}
					}
					break;
				}
			case (int)BehaviorState.TentacleRelease:
				{
					NPC.velocity *= 0.95f;
					var toTarget = player.Center - NPC.Center - NPC.velocity;
					NPC.velocity = toTarget.SafeNormalize(Vector2.Zero) * 0.1f + NPC.velocity * 0.9f;
					if (NPC.velocity.Length() > 0.5f)
					{
						NPC.direction = NPC.velocity.X > 0 ? 1 : -1;
					}
					if (Timer < 60)
					{
						if (toTarget.Length() > 200)
						{
							NPC.velocity = toTarget.SafeNormalize(Vector2.Zero) * 0.8f + NPC.velocity * 0.9f;
						}
					}
					if (Timer == 60)
					{
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<LargeBloodLanternGhost_Tentacles>(), 60, 0f, -1);
						LargeBloodLanternGhost_Tentacles lBLGT = p0.ModProjectile as LargeBloodLanternGhost_Tentacles;
						if (lBLGT is not null)
						{
							lBLGT.OwnerNPC = NPC;
						}
					}
					if (Timer > 300)
					{
						SwitchState();
					}
					break;
				}
		}
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc != NPC)
			{
				if (npc.type == ModContent.NPCType<NPCs.LanternGhostKing.LanternGhostKing>())
				{
					Vector2 v0 = NPC.Center - npc.Center;
					if (v0.Length() < 400)
					{
						NPC.velocity += Vector2.Normalize(v0) * 2.5f;
						break;
					}
				}
			}
		}
		Lighting.AddLight(NPC.Center, new Vector3(1f, 0.3f * MathF.Sin(Timer * 0.03f) + 0.3f, 0.3f * MathF.Cos(Timer * 0.03f) + 0.3f));
	}

	public void SwitchState()
	{
		int newState = Main.rand.Next(3);
		if (State == newState)
		{
			State = newState + 1;
			if (State > 2)
			{
				State = 0;
			}
		}
		else
		{
			State = newState;
		}
		if (State == 1)
		{
			SummonPos = new Vector2(0, -Main.rand.NextFloat(480, 540)).RotatedBy(Main.rand.NextFloat(-1.7f, 1.7f));
		}
		ClosingTimeMax = Main.rand.Next(150, 300);
		Timer = 0;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		if (Main.expertMode)
		{
			target.AddBuff(BuffID.ChaosState, 60);
		}
	}

	public override void OnKill()
	{
		for (int f = 0; f < 15; f++)
		{
			Vector2 v3 = new Vector2(0, Main.rand.NextFloat(0, 12f)).RotatedByRandom(MathHelper.TwoPi);
			int r = Dust.NewDust(NPC.Center - new Vector2(4, 4) - new Vector2(4, 4), 8, 8, ModContent.DustType<Dusts.Flame4>(), v3.X, v3.Y, 0, default, Main.rand.NextFloat(0.6f, 1.8f));
			Main.dust[r].noGravity = true;
			Main.dust[r].velocity = v3;
		}

		for (int g = 0; g < 32; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			vel += new Vector2(0, 3);
			float mulScale = Main.rand.NextFloat(26f, 42f);
			var blood = new LargeBloodLanternGhost_BloodDrop
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = NPC.Center + vel,
				maxTime = Main.rand.Next(42, 84),
				scale = mulScale,
				rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 8; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 4f, 0).RotatedByRandom(MathHelper.TwoPi);
			vel += new Vector2(0, 3);
			var blood = new LargeBloodLanternGhost_BloodSplash
			{
				velocity = vel,
				Active = true,
				Visible = true,
				position = NPC.Center - vel,
				maxTime = Main.rand.Next(122, 194),
				scale = Main.rand.NextFloat(26f, 34f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(blood);
		}

		for (int g = 0; g < 8; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 8f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.LargeBloodLanternGhost_Gore_0_Mod;
			if (texturePath is not null)
			{
				texturePath = texturePath.Remove(texturePath.Length - 1, 1);
				texturePath += g;
			}
			var gore = new NormalGore
			{
				Velocity = vel,
				Position = NPC.Center + vel,
				Texture = ModContent.Request<Texture2D>(texturePath).Value,
				RotateSpeed = Main.rand.NextFloat(-0.2f, 0.2f),
				Scale = Main.rand.NextFloat(0.8f, 1.2f),
				MaxTime = Main.rand.Next(300, 340),
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
			};
			Ins.VFXManager.Add(gore);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var texture = (Texture2D)ModContent.Request<Texture2D>(Texture);
		SpriteEffects effects = SpriteEffects.None;
		NPC.spriteDirection = NPC.direction;
		if (NPC.spriteDirection == 1)
		{
			effects = SpriteEffects.FlipHorizontally;
		}
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, effects, 0);
	}
}
