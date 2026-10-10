using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class UmbrellaTailThief : ModNPC
{
	public bool Flying = false;

	public int FlyTimer = 0;

	public int SwitchStateCooling = 0;

	public int JumpTimer = 0;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 6;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 100;
		NPC.height = 60;
		NPC.lifeMax = 80;
		NPC.damage = 25;
		NPC.defense = 10;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0.5f;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		AIType = NPCID.WalkingAntlion;
	}

	public override void OnSpawn(IEntitySource source)
	{
		JumpTimer = Main.rand.Next(120, 1200);
		NPC.scale = Main.rand.NextFloat(0.85f, 1.15f);
	}

	public override void FindFrame(int frameHeight)
	{
		// Vanilla AI
		if (FlyTimer <= 0)
		{
			if (FlyTimer > 0)
			{
				FlyTimer--;
			}
			NPC.frame.Width = 114;
			NPC.frame.Height = 70;
			Vector2 oldBottom = NPC.Bottom;
			NPC.width = 100;
			NPC.height = 60;
			NPC.Bottom = oldBottom;
			switch (NPC.ai[0])
			{
				case 0:
					NPC.rotation = 0f;
					if (NPC.velocity.Y == 0f)
					{
						NPC.spriteDirection = NPC.direction;
					}
					else if (NPC.velocity.Y < 0f)
					{
						NPC.frameCounter = 0;
					}

					NPC.frameCounter += Math.Abs(NPC.velocity.X) * 1.1f;
					if (NPC.frameCounter < 6)
					{
						NPC.frame.Y = 0;
					}
					else if (NPC.frameCounter < 12)
					{
						NPC.frame.Y = frameHeight;
					}
					else if (NPC.frameCounter < 18)
					{
						NPC.frame.Y = frameHeight * 2;
					}
					else if (NPC.frameCounter < 24)
					{
						NPC.frame.Y = frameHeight * 3;
					}
					else if (NPC.frameCounter < 32)
					{
						NPC.frame.Y = frameHeight * 4;
					}
					else
					{
						NPC.frameCounter = 0;
					}
					break;
				case 1:
					NPC.frameCounter = 0;
					NPC.frame.Y = NPC.ai[1] < 10f ? frameHeight * 5 : NPC.ai[1] < 20f ? frameHeight * 6 : frameHeight * 7;
					break;
				case 5:
					NPC.frameCounter = 0;
					NPC.frame.Y = NPC.ai[1] < 10f ? frameHeight * 7 : NPC.ai[1] < 20 ? frameHeight * 6 : frameHeight * 5;
					break;
				default:
					NPC.frameCounter = 0;
					NPC.frame.Y = frameHeight * 7;
					break;
			}
		}
		else
		{
			NPC.frame.Width = 146;
			NPC.frame.Height = 134;
			NPC.width = 60;
			NPC.height = 100;
			NPC.frame.Y = 134 * (FlyTimer / 5);
		}
	}

	public override void AI()
	{
		UpdateFlying();
		int distanceOverTiles = CheckSpaceDown(NPC.Bottom);
		if (SwitchStateCooling <= 0)
		{
			if (Flying != distanceOverTiles >= 64)
			{
				SwitchStateCooling = 60;
			}
			Flying = distanceOverTiles >= 64;
		}
		if (SwitchStateCooling > 0)
		{
			SwitchStateCooling--;
		}

		if (FlyTimer <= 0)
		{
			if (JumpTimer <= 0 && NPC.collideY)
			{
				JumpTimer = Main.rand.Next(120, 1200);
				NPC.velocity.Y = -Main.rand.NextFloat(12f, 17f);
				if (NPC.target >= 0)
				{
					Player target = Main.player[NPC.target];
					NPC.FaceTarget();
					NPC.spriteDirection = NPC.direction;
					NPC.velocity.X = (target.Center.X - NPC.Center.X) * 0.02f;
					if (MathF.Abs(NPC.velocity.X) > 8f)
					{
						NPC.velocity.X = MathF.Sign(NPC.velocity.X) * 8f;
					}
				}
			}
			else
			{
				NPC.noGravity = false;
				NPC.type = NPCID.WalkingAntlion;
				NPC.AI_003_Fighters();
				NPC.type = ModContent.NPCType<UmbrellaTailThief>();
				JumpTimer--;
			}
		}
		else
		{
			NPC.noGravity = true;
			NPC.velocity.X = MathF.Sin((float)Main.time * 0.03f + NPC.whoAmI) * 0.1f + NPC.velocity.X * 0.9f;
			if (NPC.velocity.X > 0)
			{
				NPC.spriteDirection = 1;
			}
			if (NPC.velocity.X < 0)
			{
				NPC.spriteDirection = -1;
			}
			if (NPC.velocity.Y < 1f)
			{
				NPC.velocity.Y += 0.1f;
			}
			else
			{
				NPC.velocity.Y -= 0.1f;
			}
			Lighting.AddLight(NPC.Top, new Vector3(1f, 0.75f, 0.1f) * 0.5f);
		}
	}

	public int CheckSpaceDown(Vector2 worldPos)
	{
		for (int h = 0; h < 1600; h += 8)
		{
			if (Collision.IsWorldPointSolid(worldPos + new Vector2(0, h)))
			{
				return h;
			}
		}
		return 0;
	}

	public void UpdateFlying()
	{
		if (!Flying)
		{
			if (FlyTimer > 0)
			{
				FlyTimer--;
			}
		}
		else
		{
			if (FlyTimer < 20)
			{
				FlyTimer++;
			}
		}
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}

	public override float SpawnChance(NPC.Spawner spawnInfo)
	{
		LampWoodForest lampBiome = ModContent.GetInstance<LampWoodForest>();
		return !lampBiome.IsBiomeActive(Main.LocalPlayer) ? 0f : 3f;
	}

	public override void OnKill()
	{
		for (int i = 0; i < 13; i++)
		{
			if (i == 5 && Flying)
			{
				continue;
			}
			if (i is >= 1 and <= 3 && !Flying)
			{
				continue;
			}
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/UmbrellaTailThief_gore_" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (FlyTimer <= 0)
		{
			spriteBatch.Draw(ModAsset.UmbrellaTailThief.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		else
		{
			spriteBatch.Draw(ModAsset.UmbrellaTailThief_Fly.Value, NPC.Center - screenPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			spriteBatch.Draw(ModAsset.UmbrellaTailThief_Fly_glow.Value, NPC.Center - screenPos, NPC.frame, new Color(1f, 1f, 1f, 0), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		// TODO 掉落物
	}
}
