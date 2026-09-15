using Everglow.Commons.CustomTiles;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs;

public class JellyBall : ModNPC
{
	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 5;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 40;
		NPC.height = 40;
		NPC.aiStyle = -1;
		NPC.damage = 13;
		NPC.defense = 2;
		NPC.lifeMax = 24;
		NPC.HitSound = SoundID.NPCHit3;
		NPC.DeathSound = SoundID.NPCDeath3;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.knockBackResist = 1f;
		NPC.value = 90;
	}

	public override void OnSpawn(IEntitySource source)
	{
		Anger = false;
		BloomLightColor = new Vector3(0f, 0.5f, 1f);
		state = (int)NPCState.Sleep;
		NPC.scale = Main.rand.NextFloat(0.75f, 1.18f);
	}

	private int state;
	public bool Anger = false;
	public Vector3 BloomLightColor;

	private enum NPCState
	{
		Sleep,
		Dash,
		Rest,
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

		return 3f;
	}

	public override void FindFrame(int frameHeight)
	{
		NPC.frame.Width = 48;
		NPC.frame.Height = 58;
		if (Anger)
		{
			NPC.frame.X = 0;
		}
		else
		{
			NPC.frame.X = 48;
		}
		switch (state)
		{
			case (int)NPCState.Dash:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 5 % 5) * 58;
					break;
				}
			case (int)NPCState.Sleep:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 5 % 5) * 58;
					break;
				}
			case (int)NPCState.Rest:
				{
					NPC.frame.Y = (int)(NPC.frameCounter / 5 % 5) * 58;
					break;
				}
		}
	}

	public override void AI()
	{
		switch (state)
		{
			case (int)NPCState.Sleep:
				{
					NPC.frameCounter++;
					NPC.TargetClosest();
					Vector2 targetVel = new Vector2(MathF.Sin((float)Main.time * 0.03f + NPC.whoAmI), MathF.Sin((float)Main.time * 0.06f + MathF.PI + NPC.whoAmI)) * NPC.scale;
					NPC.velocity = Vector2.Lerp(NPC.velocity, targetVel, 0.4f);
					NPC.rotation = Math.Clamp(NPC.velocity.X * 0.3f, -1f, 1f);
					if (NPC.HasValidTarget && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1)
							 && Anger)
					{
						state = (int)NPCState.Dash;
						NPC.ai[0] = 0;
						NPC.frameCounter = 0;
					}

					// 逆集群化
					foreach (NPC npc in Main.npc)
					{
						if (npc != null && npc.active && npc != NPC)
						{
							if (npc.type == Type)
							{
								Vector2 v0 = NPC.Center - npc.Center;
								if (v0.Length() < 60)
								{
									NPC.velocity += Vector2.Normalize(v0) * 0.25f;
								}
							}
							if (npc.type == ModContent.NPCType<GiantJellyBall>())
							{
								Vector2 v0 = NPC.Center - npc.Center;
								if (v0.Length() < 120)
								{
									NPC.velocity += Vector2.Normalize(v0) * 0.25f;
								}
							}
							if (npc.type == ModContent.NPCType<KingJellyBall.KingJellyBall>())
							{
								Vector2 v0 = NPC.Center - npc.Center;
								if (v0.Length() < 450)
								{
									NPC.velocity += Vector2.Normalize(v0) * 0.45f;
								}
							}
						}
					}
					break;
				}
			case (int)NPCState.Dash:
				{
					NPC.localAI[0] += 0.08f;
					NPC.TargetClosest();
					Player target = Main.player[NPC.target];
					Vector2 toAim = target.Center - NPC.Center;

					// 差异化
					if (MathF.Sin((float)Main.time * 0.004f + NPC.whoAmI) > -0.8)
					{
						toAim = target.Center + new Vector2(0, MathF.Sin((float)Main.time * 0.0005f + NPC.whoAmI * 7 + 2.1f)).RotatedBy(NPC.localAI[0] + NPC.whoAmI * 4.4f) * 142 * NPC.scale - NPC.Center;
					}
					NPC.velocity = Vector2.Normalize(toAim) * 3 * NPC.scale;
					NPC.rotation = NPC.velocity.ToRotation() + 1.57f;

					NPC.frameCounter++;
					if (NPC.frameCounter >= 5)
					{
						state = (int)NPCState.Rest;
						NPC.frameCounter = 0;
					}
					break;
				}
			case (int)NPCState.Rest:
				{
					NPC.velocity *= 0.92f;
					NPC.frameCounter++;
					if (NPC.velocity.Length() <= 0.5f)
					{
						NPC.velocity *= 0;
						state = (int)NPCState.Dash;
						NPC.frameCounter = 0;
						NPC.TargetClosest();
						if (NPC.HasValidTarget && Collision.CanHit(NPC.Center, 1, 1, Main.player[NPC.target].Center, 1, 1)
										   && Main.player[NPC.target].Distance(NPC.Center) > 1000)
						{
							Anger = false;
							state = (int)NPCState.Sleep;
							NPC.frameCounter = 0;
						}

						// 第二层差异化处理,400距离以上,1/15的概率解除仇恨
						if (Main.rand.NextBool(15) && Main.player[NPC.target].Distance(NPC.Center) > 400)
						{
							Anger = false;
							state = (int)NPCState.Sleep;
							NPC.frameCounter = 0;
						}
					}
					NPC.rotation = NPC.velocity.ToRotation() + 1.57f;
					break;
				}
		}
		float glowStrength = 0.4f;
		if (state == (int)NPCState.Sleep)
		{
			glowStrength = 0.1f;
		}
		if (Anger)
		{
			if (NPC.frame.Y == 58)
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0.5f, 2f, 4f) * glowStrength, 0.3f);
			}
			else if (NPC.frame.Y == 116)
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 1f, 2f) * glowStrength, 0.3f);
			}
			else
			{
				BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 0.3f, 0.6f) * glowStrength, 0.3f);
			}
		}
		else
		{
			BloomLightColor = Vector3.Lerp(BloomLightColor, new Vector3(0f, 0.5f, 1f) * glowStrength, 0.3f);
		}
		Lighting.AddLight(NPC.Center, BloomLightColor);
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (!Anger)
		{
			Anger = true;
		}
		if (NPC.life <= 0)
		{
			for (int i = 0; i < 12; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallGel>());
				d.scale *= Main.rand.NextFloat(0.7f, 1.4f);
				d.velocity = new Vector2(Main.rand.NextFloat(2, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int i = 0; i < 4; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallSpark>());
				d.velocity = new Vector2(Main.rand.NextFloat(2, 6f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int g = 0; g < 32; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 24)).RotatedByRandom(MathHelper.TwoPi);
				float mulScale = Main.rand.NextFloat(6f, 12f);
				var blood = new JellyBallGelDrop
				{
					velocity = afterVelocity / mulScale,
					Active = true,
					Visible = true,
					position = NPC.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
					maxTime = Main.rand.Next(42, 84),
					scale = mulScale,
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			for (int g = 0; g < 8; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 4)).RotatedByRandom(MathHelper.TwoPi);
				var blood = new JellyBallGelSplash
				{
					velocity = afterVelocity,
					Active = true,
					Visible = true,
					position = NPC.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283) - afterVelocity,
					maxTime = Main.rand.Next(32, 94),
					scale = Main.rand.NextFloat(6f, 24f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
				};
				Ins.VFXManager.Add(blood);
			}
		}
		else
		{
			for (int i = 0; i < 4; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallGel>());
				d.scale *= Main.rand.NextFloat(0.6f, 1.3f);
				d.velocity = new Vector2(Main.rand.NextFloat(1, 3f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int i = 0; i < 1; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallSpark>());
				d.velocity = new Vector2(Main.rand.NextFloat(2, 4f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
		}
		base.HitEffect(hit);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		float glowStrength = 1f;
		if (state == (int)NPCState.Sleep)
		{
			glowStrength = 0.4f;
		}
		Texture2D texture = ModAsset.JellyBall.Value;
		Texture2D textureG = ModAsset.JellyBall_glow.Value;
		Texture2D textureB = ModAsset.JellyBall_bloom.Value;
		spriteBatch.Draw(textureB, NPC.Center - Main.screenPosition, new Rectangle(NPC.frame.X / 48 * 160, NPC.frame.Y / 58 * 160, 160, 160), new Color(1f, 1f, 1f, 0f) * glowStrength, NPC.rotation, new Vector2(80), NPC.scale, SpriteEffects.None, 0);
		spriteBatch.Draw(texture, NPC.Center - Main.screenPosition, NPC.frame, Color.Lerp(drawColor * 0.7f, new Color(0.6f, 1f, 1f, 1f), 0.4f), NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

		spriteBatch.Draw(textureG, NPC.Center - Main.screenPosition, NPC.frame, new Color(1f, 1f, 1f, 0f) * glowStrength, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

		return false;
	}

	public override void OnKill()
	{
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.ByCondition(new JellyBallNoneDerivativeCondition(), ModContent.ItemType<JellyBallCube>(), 1, 1, 2));
	}
}

internal class JellyBallNoneDerivativeCondition : IItemDropRuleCondition
{
	public bool CanDrop(DropAttemptInfo info)
	{
		if (info.npc.type == ModContent.NPCType<JellyBall>() || info.npc.type == ModContent.NPCType<GiantJellyBall>())
		{
			if (info.npc.ai[1] != 127)
			{
				return true;
			}
		}
		return false;
	}

	public bool CanShowItemDropInUI()
	{
		return false;
	}

	public string GetConditionDescription()
	{
		string desc = "None boss summoned jelly ball";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			desc = "非boss召唤的球冻";
		}

		return desc;
	}
}
