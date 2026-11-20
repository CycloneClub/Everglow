using Everglow.Commons.Mechanics.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

[NoGameModeScale]
public class LightBulb : ModNPC
{
	public Vector2 RootPos = Vector2.zeroVector;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 12;
	}

	public override void SetDefaults()
	{
		NPC.width = 42;
		NPC.height = 42;
		NPC.lifeMax = Main.masterMode ? 90 : Main.expertMode ? 65 : 40;
		NPC.damage = Main.masterMode ? 16 : Main.expertMode ? 12 : 8;
		NPC.defense = 2;
		NPC.friendly = false;
		NPC.aiStyle = -1;
		NPC.knockBackResist = 0;
		NPC.noGravity = false;
		NPC.noTileCollide = false;
		NPC.value = 100;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Poisoned, 600);
	}

	public override void FindFrame(int frameHeight)
	{
		switch (state)
		{
			case (int)NPCState.Sleep:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 8 % 4 + 8) * frameHeight;
					break;
				}
			case (int)NPCState.Charge:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 6 % 12) * frameHeight;
					break;
				}
			case (int)NPCState.Attack:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 6 % 12) * frameHeight;
					break;
				}
			case (int)NPCState.Cooldown:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 8 % 4 + 8) * frameHeight;
					break;
				}
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		state = (int)NPCState.Sleep;
	}

	private int state;

	private enum NPCState
	{
		Sleep,
		Charge,
		Attack,
		Cooldown,
	}

	public override void AI()
	{
		Lighting.AddLight(NPC.Center, 0.6f, 0.6f, 0);
		float timeValue = (float)Main.time * 0.024f + TileUtils.GetFixedRandomNumber(NPC.whoAmI, (int)NPC.position.X) / 1024f * MathHelper.TwoPi;
		if (NPC.frame.Y == 270)
		{
			Lighting.AddLight(NPC.Center, 2.6f, 2.6f, 0.6f);
		}
		if (RootPos == Vector2.zeroVector && NPC.collideY)
		{
			RootPos = NPC.Bottom;
			NPC.noGravity = true;
		}
		NPC.TargetClosest();
		if (!NPC.HasValidTarget)
		{
			return;
		}
		Player player = Main.player[NPC.target];
		switch (state)
		{
			case (int)NPCState.Sleep:
				{
					NPC.frameCounter++;
					NPC.ai[0] = 0;
					if (player.Distance(NPC.Center) <= 950)
					{
						if (player.Distance(NPC.Center) <= 350)
						{
							Vector2 npcToward = new Vector2(0, -1).RotatedBy(NPC.rotation);
							Vector2 toPlayer = (player.Center - NPC.Center).NormalizeSafe();
							float deltaRotation = MathF.Asin(Vector3.Cross(new Vector3(npcToward, 0), new Vector3(toPlayer, 0)).Z);
							NPC.rotation += deltaRotation * 0.1f;
						}
						else if (player.Distance(NPC.Center) <= 550)
						{
							state = (int)NPCState.Charge;
							NPC.ai[0] = 0;
							NPC.frameCounter = 0;
						}
						else
						{
							if (RootPos != Vector2.zeroVector)
							{
								Vector2 target = RootPos + new Vector2(MathF.Sin(timeValue), MathF.Sin(timeValue * 2)) * new Vector2(120, 60) + new Vector2(0, -150);
								Vector2 toTarget = (target - NPC.Center - NPC.velocity).NormalizeSafe() * 2.5f;
								NPC.velocity = toTarget * 0.25f + NPC.velocity * 0.75f;
								NPC.rotation = NPC.velocity.X * 0.05f;
							}
						}
					}
					break;
				}
			case (int)NPCState.Charge:
				{
					NPC.frameCounter++;
					NPC.ai[0]++;
					if (player.Distance(NPC.Center) <= 350)
					{
						Vector2 npcToward = new Vector2(0, -1).RotatedBy(NPC.rotation);
						Vector2 toPlayer = (player.Center - NPC.Center).NormalizeSafe();
						float deltaRotation = MathF.Asin(Vector3.Cross(new Vector3(npcToward, 0), new Vector3(toPlayer, 0)).Z);
						NPC.rotation += deltaRotation * 0.1f;
					}
					if (RootPos != Vector2.zeroVector)
					{
						Vector2 stemBase = RootPos + new Vector2(0, -80);
						Vector2 target = stemBase + (player.Center - stemBase).NormalizeSafe() * 65f;
						Vector2 toTarget = (target - NPC.Center - NPC.velocity).NormalizeSafe() * 2.5f;
						NPC.velocity = toTarget * 0.25f + NPC.velocity * 0.75f;
					}
					if ((NPC.ai[0] % 18) == 0)
					{
						state = (int)NPCState.Attack;
						NPC.ai[0] = 0;
					}
					break;
				}
			case (int)NPCState.Attack:
				{
					if (NPC.ai[0] == 0)
					{
						for (int i = 0; i < 6; i++)
						{
							Vector2 v = new Vector2(5f, 0);
							v = v.RotatedBy(-Math.PI * i / 5 + NPC.rotation).RotatedByRandom(Math.PI / 10);
							if (v.Y > 0)
							{
								v.Y = -v.Y;
							}
							Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center - new Vector2(0, 40), v, ModContent.ProjectileType<LightSeed>(), 4, 0, Owner: -1);
						}
						for (int x = 0; x < 12; x++)
						{
							Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi) + new Vector2(0, -3f);
							Dust dust = Dust.NewDustDirect(NPC.Center - new Vector2(0, 20), 0, 0, ModContent.DustType<LampWood_Dust_Fluorescent_Appear>());
							dust.velocity = newVelocity;
							dust.scale = Main.rand.NextFloat(1.2f, 2f);
							dust.rotation = Main.rand.NextFloat(0.4f, 0.8f);
						}
						SoundEngine.PlaySound(SoundID.NPCDeath3.WithPitchOffset(1), NPC.Center - new Vector2(0, 40));
					}
					NPC.ai[0]++;
					NPC.frameCounter++;
					NPC.TargetClosest();

					if ((NPC.ai[0] % 30) == 0)
					{
						state = (int)NPCState.Cooldown;
					}
					break;
				}
			case (int)NPCState.Cooldown:
				{
					NPC.frameCounter++;
					NPC.ai[0]++;

					NPC.TargetClosest();
					if ((NPC.HasValidTarget && Main.player[NPC.target].Distance(NPC.Center) > 750) || (NPC.ai[0] % 160) == 0)
					{
						state = (int)NPCState.Sleep;
						NPC.ai[0] = 0;
					}
					break;
				}
		}
		// RestrictFromRootPos();
	}

	public void RestrictFromRootPos()
	{
		if (RootPos != Vector2.zeroVector)
		{
			Vector2 toRoot = RootPos - NPC.Center;
			float length = toRoot.Length();
			if (toRoot.Length() > 160)
			{
				NPC.velocity -= toRoot.NormalizeSafe() * (length - 160) * 0.2f;
			}
		}
	}

	public override void OnKill()
	{
		for (int i = 0; i < 5; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/GlowingBlossom_Gore" + Main.rand.Next(4)).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/GlowingBlossom_Gore" + Main.rand.Next(4, 7)).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, NPC.scale);
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<UnstablePollen>(), 1, 1, 2));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<LightBulbOvule>(), 24, 1));
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.LightBulb.Value;
		if (RootPos != Vector2.zeroVector)
		{
			Texture2D root = ModAsset.LightBulb_Root.Value;
			spriteBatch.Draw(root, RootPos - Main.screenPosition, null, drawColor, NPC.rotation, root.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
		}
		Vector2 stemPos = RootPos;
		Vector2 stemVel = new Vector2(0, -16);
		Vector2 blossomToward = new Vector2(0, -16).RotatedBy(NPC.rotation);
		for (int k = 0; k < 160; k++)
		{
			Texture2D stem = ModAsset.LightBulb_Stem.Value;
			Vector2 toBlossom = NPC.Center + new Vector2(0, 18).RotatedBy(NPC.rotation) - stemPos - stemVel;
			toBlossom -= Vector2.Dot(toBlossom, blossomToward) * blossomToward.NormalizeSafe() / 32f;
			stemVel += toBlossom.NormalizeSafe() * 8f;
			stemVel = stemVel.NormalizeSafe() * 16f;
			Rectangle frame = new Rectangle(0, (k % 2) * 19, 16, 19);
			spriteBatch.Draw(stem, stemPos - Main.screenPosition, frame, drawColor, stemVel.ToRotationSafe() + MathHelper.PiOver2, frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
			if ((NPC.Center + new Vector2(0, 18).RotatedBy(NPC.rotation) - stemPos).Length() <= 16)
			{
				break;
			}
			stemPos += stemVel;
		}
		spriteBatch.Draw(tex, NPC.Bottom - Main.screenPosition, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D glow = ModAsset.LightBulb_Glow.Value;
		spriteBatch.Draw(glow, NPC.Bottom - Main.screenPosition, NPC.frame, new Color(1f, 1f, 1f, 0) * 0.5f, NPC.rotation, NPC.frame.Size() * new Vector2(0.5f, 1f), NPC.scale, SpriteEffects.None, 0);
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
