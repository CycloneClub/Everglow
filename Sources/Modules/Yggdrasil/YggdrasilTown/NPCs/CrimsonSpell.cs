using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class CrimsonSpell : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 11;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 32;
		NPC.height = 54;
		NPC.aiStyle = -1;
		NPC.damage = 30;
		NPC.defense = 0;
		NPC.lifeMax = 450;
		NPC.HitSound = SoundID.NPCHit3;
		NPC.DeathSound = SoundID.NPCDeath3;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.knockBackResist = 0f;
		NPC.alpha = 0;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		YggdrasilTownBiome YggdrasilTownBiome = ModContent.GetInstance<YggdrasilTownBiome>();
		if (!YggdrasilTownBiome.IsBiomeActive(Main.LocalPlayer))
		{
			return 0f;
		}
		return 0.05f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		base.OnSpawn(source);
		NPC.ai[0] = 0;
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		Teleport(player);
	}

	public void FacePlayer(Player player)
	{
		if (player.Center.X < NPC.Center.X)
		{
			NPC.spriteDirection = 1;
		}
		else
		{
			NPC.spriteDirection = -1;
		}
	}

	public override void AI()
	{
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		NPC.ai[0]++;

		// Blood spell
		if (NPC.ai[0] < 500)
		{
			if (NPC.ai[0] == 30)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-5 * NPC.spriteDirection, 0), Vector2.zeroVector, ModContent.ProjectileType<BloodSpell>(), 10, 1, NPC.target);
			}
			if (NPC.ai[0] < 80)
			{
				NPC.frame.Y = 64 * ((int)NPC.ai[0] / 10);
			}
			else if (NPC.ai[0] < 240)
			{
				if (NPC.ai[0] % 10 > 5)
				{
					NPC.frame.Y = 64 * 7;
				}
				else
				{
					NPC.frame.Y = 64 * 6;
				}
			}
			if (NPC.ai[0] >= 240)
			{
				FacePlayer(player);
				NPC.frame.Y = 64 * ((int)(NPC.ai[0] - 240) / 10 + 8);
			}
			if (NPC.frame.Y >= 704)
			{
				NPC.frame.Y = 0;
				NPC.ai[1]++;
			}
			if (NPC.ai[1] > 180)
			{
				NPC.ai[1] = 0;
				if (Main.rand.NextBool(2))
				{
					NPC.ai[0] = 500;
				}
				else if ((NPC.Center - player.Center).Length() > 600)
				{
					NPC.ai[0] = 500;
				}
				else
				{
					NPC.ai[0] = 0;
				}
			}
		}

		// Teleport
		else if (NPC.ai[0] < 750)
		{
			if (NPC.ai[0] < 610)
			{
				NPC.alpha += 5;
			}
			else if (NPC.ai[0] == 610)
			{
				Teleport(player);
				NPC.alpha = 255;
			}
			else if (NPC.ai[0] < 670)
			{
				NPC.alpha -= 5;
				if (NPC.alpha < 0)
				{
					NPC.alpha = 0;
				}
			}
			else
			{
				NPC.alpha = 0;
				NPC.ai[0] = 0;
			}
		}
	}

	public void Teleport(Player player)
	{
		for (int i = 0; i < 1000; i++)
		{
			Vector2 checkPoint = player.Center + new Vector2(0, i + 300).RotatedBy(i * 0.5f + NPC.whoAmI + Main.time);
			if (!Collision.SolidCollision(checkPoint - NPC.Size * 0.5f, NPC.width, NPC.height))
			{
				for (int j = 0; j < 50; j++)
				{
					checkPoint += new Vector2(0, 10);
					if (Collision.SolidCollision(checkPoint - NPC.Size * 0.5f, NPC.width, NPC.height))
					{
						if ((checkPoint - player.Center).Length() >= 100)
						{
							NPC.Center = checkPoint + new Vector2(0, 0);
							return;
						}
					}
				}
			}
		}
		NPC.active = false;
	}

	public override void FindFrame(int frameHeight)
	{
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (NPC.alpha >= 250)
		{
			return false;
		}
		Texture2D texture = ModAsset.CrimsonSpell.Value;
		Vector2 offset = new Vector2(0, -10f);
		float alphaValue = NPC.alpha / 255f;
		drawColor = Color.Lerp(drawColor, new Color(alphaValue, alphaValue * 0.05f, alphaValue * 0.05f, 0), alphaValue);
		spriteBatch.Draw(texture, NPC.Center + offset - Main.screenPosition, NPC.frame, drawColor, 0, NPC.frame.Size() * 0.5f, 1f, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		Texture2D textureglow = ModAsset.CrimsonSpell_glow.Value;
		spriteBatch.Draw(textureglow, NPC.Center + offset - Main.screenPosition, NPC.frame, Color.White * alphaValue, 0, NPC.frame.Size() * 0.5f, 1f, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		return false;
	}

	public override void OnKill()
	{
		for (int i = 0; i < 5; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/CrimsonProtestantGore" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		// TODO 掉落物
	}
}
