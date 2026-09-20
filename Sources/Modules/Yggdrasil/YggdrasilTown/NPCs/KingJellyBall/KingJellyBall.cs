using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.KingJellyBall;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;

[AutoloadBossHead]
[NoGameModeScale]
public class KingJellyBall : ModNPC
{
	public override string Texture => ModAsset.KingJellyBall_Core_Mod;

	public float HealLightValue = 0f;

	public int TotalDamageTakeIn = 0;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 1;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 160;
		NPC.height = 200;
		NPC.aiStyle = -1;
		NPC.damage = 30;
		NPC.defense = 6;
		NPC.lifeMax = 2200;
		if (Main.expertMode)
		{
			NPC.damage = 40;
			NPC.defense = 8;
			NPC.lifeMax = 2600;
		}
		if (Main.masterMode)
		{
			NPC.damage = 55;
			NPC.defense = 10;
			NPC.lifeMax = 3000;
		}
		NPC.HitSound = SoundID.NPCHit3;
		NPC.DeathSound = SoundID.NPCDeath3;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.knockBackResist = 0.02f;
		NPC.value = 10960;
		NPC.boss = true;
		if (!Main.dedServ)
		{
			Mod everglow = ModLoader.GetMod("Everglow");
			if (everglow != null)
			{
				Music = MusicLoader.GetMusicSlot(everglow, ModAsset.KingJellyBallBGM_Path);
			}
		}
	}

	public override void OnSpawn(IEntitySource source)
	{
		Anger = false;
		BloomLightColor = new Vector3(0f, 0.5f, 1f);
		State = (int)NPCState.Sleep;

		var reflection = new KingJellyBall_Reflection
		{
			Active = true,
			Visible = true,
			MyKingJellyBallOwner = NPC,
			Position = NPC.Center,
			Rotation = NPC.rotation,
			Scale = NPC.scale,
		};
		Ins.VFXManager.Add(reflection);
	}

	public int State;
	public bool Anger = false;
	public Vector3 BloomLightColor;
	public int NoReflectionTime = 0;
	public List<int> InteractableJellyBalls = new List<int>();

	public enum NPCState
	{
		Sleep,
		Approach,
		ShootGelStream,
		ShootCrystal,
		AbsorbJellyBall,
		KillJellyBall,
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		return 0f;
	}

	public override void FindFrame(int frameHeight)
	{
	}

	public override void AI()
	{
		NoReflectionTime++;
		if (NoReflectionTime > 15)
		{
			var reflection = new KingJellyBall_Reflection
			{
				Active = true,
				Visible = true,
				MyKingJellyBallOwner = NPC,
				Position = NPC.Center,
				Rotation = NPC.rotation,
				Scale = NPC.scale,
			};
			Ins.VFXManager.Add(reflection);
		}
		Lighting.AddLight(NPC.Center, BloomLightColor);

		ControlScale();
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		if (State == (int)NPCState.Approach)
		{
			ApproachTarget();
			Vector2 toPlayer = player.Center - NPC.Center;
			if (GetInteractableTargetCount(300, 1200) > 2 && Main.rand.NextBool((int)Math.Max(NPC.life / (float)NPC.lifeMax * 120, 10)) && NPC.life < NPC.lifeMax * 0.8f)
			{
				State = (int)NPCState.AbsorbJellyBall;
				NPC.localAI[0] = 0;
				return;
			}
			if (GetInteractableTargetCount(500, 2000) > 2 && Main.rand.NextBool(Math.Max(120 - GetInteractableTargetCount(500, 2000) * 10, 1)))
			{
				State = (int)NPCState.KillJellyBall;
				NPC.localAI[0] = 0;
				return;
			}
			if (Main.rand.NextBool(120) && toPlayer.Length() > 150 && toPlayer.Length() < 600)
			{
				State = (int)NPCState.ShootGelStream;
				NPC.localAI[0] = 0;
				return;
			}
		}

		if (State == (int)NPCState.ShootGelStream)
		{
			ApproachTarget();
			ShootGelStream();
			NPC.localAI[0]++;
			if (NPC.localAI[0] >= 600)
			{
				State = (int)NPCState.Approach;
			}
		}

		if (State == (int)NPCState.AbsorbJellyBall)
		{
			AbsorbJellyBall();
			NPC.localAI[0]++;
			int maxTime = 1440;
			if (Main.expertMode)
			{
				maxTime = 1230;
			}
			if (Main.masterMode)
			{
				maxTime = 1200;
			}
			maxTime += 60;
			if (NPC.localAI[0] >= maxTime)
			{
				State = (int)NPCState.Approach;
			}
		}

		if (State == (int)NPCState.KillJellyBall)
		{
			KillJellyBall();
			NPC.localAI[0]++;
			if (NPC.localAI[0] >= 300)
			{
				State = (int)NPCState.Approach;
			}
		}

		if (State == (int)NPCState.Sleep)
		{
			NPC.velocity.Y = MathF.Sin((float)Main.time * 0.006f) * 0.05f;
			NPC.velocity *= 0.996f;
		}

		if (HealLightValue > 0)
		{
			HealLightValue -= 0.01f;
		}
		else
		{
			HealLightValue = 0;
		}

		// Escape when player dead
		if (!player.active || player.dead)
		{
			NPC.velocity.Y += 0.25f;
			NPC.noTileCollide = true;
		}
		else
		{
			NPC.noTileCollide = false;
		}
	}

	public override bool CheckActive()
	{
		if (NPC.target > 0)
		{
			Player player = Main.player[NPC.target];
			if (player.active && !player.dead)
			{
				if ((player.Center - NPC.Center).Length() < 8000)
				{
					return false;
				}
			}
		}
		return base.CheckActive();
	}

	public void KillJellyBall()
	{
		NPC.velocity *= 0.96f;
		int maxCount = 8;
		if (Main.expertMode)
		{
			maxCount = 12;
		}
		if (Main.masterMode)
		{
			maxCount = 16;
		}
		if (NPC.localAI[0] < maxCount * 10 && GetInteractableTargetCount(500, 2000) > 0)
		{
			if (NPC.localAI[0] % 10 == 0)
			{
				Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<JellyBallElectricKill>(), (int)(NPC.damage * 0.5f * 1.1f), 0, NPC.target);
				JellyBallElectricKill jellyBallElectricKill = projectile.ModProjectile as JellyBallElectricKill;
				if (jellyBallElectricKill != null)
				{
					jellyBallElectricKill.OwnerBoss = NPC;
					jellyBallElectricKill.Killee = Main.npc[InteractableJellyBalls[Main.rand.Next(InteractableJellyBalls.Count)]];
				}
			}
		}
		else
		{
			if (KillProjCount() <= 0)
			{
				State = (int)NPCState.Approach;
			}
		}
	}

	public int KillProjCount()
	{
		int count = 0;
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile != null && projectile.active)
			{
				if (projectile.type == ModContent.ProjectileType<JellyBallElectricKill>())
				{
					count++;
				}
			}
		}
		return count;
	}

	public void AbsorbJellyBall()
	{
		NPC.velocity *= 0.96f;
		int maxCount = 4;
		if (Main.expertMode)
		{
			maxCount = 5;
		}
		if (Main.masterMode)
		{
			maxCount = 6;
		}
		if (NPC.localAI[0] < maxCount * 10 && GetInteractableTargetCount(300, 1200) > 0)
		{
			if (NPC.localAI[0] % 10 == 0)
			{
				Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<JellyBallLifeAbsorb>(), 0, 0, NPC.target);
				JellyBallLifeAbsorb jellyBallLifeAbsorb = projectile.ModProjectile as JellyBallLifeAbsorb;
				if (jellyBallLifeAbsorb != null)
				{
					jellyBallLifeAbsorb.OwnerBoss = NPC;
					jellyBallLifeAbsorb.Absorbee = Main.npc[InteractableJellyBalls[Main.rand.Next(InteractableJellyBalls.Count)]];
				}
			}
		}
		else
		{
			if (AbsorbProjCount() <= 0)
			{
				State = (int)NPCState.Approach;
			}
		}
	}

	public int AbsorbProjCount()
	{
		int count = 0;
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile != null && projectile.active)
			{
				if (projectile.type == ModContent.ProjectileType<JellyBallLifeAbsorb>())
				{
					count++;
				}
			}
		}
		return count;
	}

	public int GetInteractableTargetCount(float minDis = 600, float maxDis = 1800)
	{
		InteractableJellyBalls = new List<int>();
		int count = 0;
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.active && npc.life > 0)
			{
				if (npc.type == ModContent.NPCType<JellyBall>() || npc.type == ModContent.NPCType<GiantJellyBall>())
				{
					if (Collision.CanHit(npc, NPC) && (NPC.Center - npc.Center).Length() < maxDis && (NPC.Center - npc.Center).Length() > minDis)
					{
						bool canCount = true;
						foreach (Projectile projectile in Main.projectile)
						{
							if (projectile != null && projectile.active)
							{
								if (projectile.type == ModContent.ProjectileType<JellyBallLifeAbsorb>())
								{
									JellyBallLifeAbsorb jellyBallLifeAbsorb = projectile.ModProjectile as JellyBallLifeAbsorb;
									if (jellyBallLifeAbsorb != null)
									{
										if (jellyBallLifeAbsorb.Absorbee == npc)
										{
											canCount = false;
											break;
										}
									}
								}
							}
						}
						if (canCount)
						{
							count++;
							InteractableJellyBalls.Add(npc.whoAmI);
						}
					}
				}
			}
		}
		return count;
	}

	public void ShootGelStream()
	{
		NPC.TargetClosest();
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0.95f;
		int interval = 72;
		if (Main.expertMode)
		{
			interval = 60;
		}
		if (Main.masterMode)
		{
			interval = 48;
		}
		if (NPC.localAI[0] % interval == 0)
		{
			for (int i = 0; i < 1080; i++)
			{
				Vector2 shootVel = new Vector2(0, -16).RotatedBy(i / 1080f * MathHelper.TwoPi);
				Vector2 origVel = shootVel;
				Vector2 checkPos = NPC.Center + new Vector2(0, -120 * (NPC.scale - 0.3f));
				Vector2 startPos = checkPos;
				for (int t = 0; t < 120; t++)
				{
					checkPos += shootVel;
					if (shootVel.Y <= 12)
					{
						shootVel.Y += 0.2f;
					}
					if ((checkPos - player.Center).Length() < 24)
					{
						Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), startPos, origVel, ModContent.ProjectileType<JellyBallGelStream>(), (int)(NPC.damage * 0.375), 2, NPC.target);
						return;
					}
				}
			}
		}
	}

	public void ApproachTarget()
	{
		Player player = Main.player[NPC.target];
		Vector2 toPlayer = player.Center - NPC.Center;
		float speedLimit = 4f;
		if (NPC.life < NPC.lifeMax * 0.5f)
		{
			speedLimit = 15f;
		}
		if (toPlayer.Length() > 240)
		{
			NPC.velocity += Vector2.Normalize(player.Center - NPC.Center) * 0.03f / NPC.scale;
			if (NPC.velocity.Length() > speedLimit / NPC.scale)
			{
				NPC.velocity = Vector2.Normalize(NPC.velocity) * speedLimit / NPC.scale;
			}
			NPC.velocity *= 0.96f;
		}
		else if (toPlayer.Length() > 90)
		{
			NPC.velocity *= 0.96f;
			NPC.velocity += Vector2.Normalize(player.Center - NPC.Center) * 0.02f / NPC.scale;
		}
		else
		{
			NPC.velocity *= 0.96f;
		}
	}

	public void ControlScale()
	{
		if (NPC.life > NPC.lifeMax * 0.5f)
		{
			float value = (NPC.life - NPC.lifeMax * 0.5f) / (NPC.lifeMax * 0.5f);
			value = MathF.Pow(value, 1 / 2f);
			NPC.scale = (float)Utils.Lerp(0.3f, 1f, value);
		}
		else
		{
			NPC.scale = 0.3f;
		}
		Vector2 oldCenter = NPC.Center;
		NPC.width = (int)(NPC.scale * 350);
		NPC.height = (int)(NPC.scale * 420);
		NPC.Center = oldCenter;
	}

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		position = NPC.Center + new Vector2(0, 150 * NPC.scale);
		scale = 1.3f;
		return true;
	}

	public bool CanHitBody(Vector2 worldCoordPos)
	{
		Vector2 toCenter = NPC.Center + new Vector2(0, -60) - worldCoordPos;
		float thetaValue = MathF.Atan2(toCenter.Y, toCenter.X);
		float timeValue = (float)Main.time * 0.03f;
		float a = 200;
		float b = 110 + 10 * MathF.Sin(timeValue);
		float r = a - b * MathF.Sin(thetaValue);

		// noise wave
		for (int k = 0; k < 6; k++)
		{
			r += MathF.Sin((thetaValue + MathF.Sin(timeValue * MathF.Pow(2, k * 0.2f)) * 0.22f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
			r += MathF.Sin((thetaValue - timeValue * 0.33f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 2;
			r += MathF.Sin((thetaValue + timeValue * 0.65f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
		}
		r *= NPC.scale;
		Vector2 toDistance = new Vector2(-r, 0).RotatedBy(thetaValue);
		toDistance.Y *= 1.3f;
		toCenter.Y /= 1.3f;
		if (toCenter.Length() < r)
		{
			return true;
		}
		return false;
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (State == (int)NPCState.Sleep)
		{
			State = (int)NPCState.Approach;
		}
		TotalDamageTakeIn += hit.Damage;
		int thresholdDamage = 60;
		if (Main.expertMode)
		{
			thresholdDamage = 40;
		}
		if (Main.masterMode)
		{
			thresholdDamage = 30;
		}
		while (TotalDamageTakeIn > thresholdDamage)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 60f).RotatedByRandom(MathHelper.TwoPi), new Vector2(0, -Main.rand.NextFloat(4f, 8f)).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)), ModContent.ProjectileType<JellyBallGelStream>(), (int)(NPC.damage * 0.375), 2, NPC.target);
			TotalDamageTakeIn -= thresholdDamage;
		}
		if (NPC.life < NPC.lifeMax * 0.5 && NPC.life + hit.Damage >= NPC.lifeMax * 0.5)
		{
			int maxJellyBall = 2;
			if (Main.expertMode)
			{
				maxJellyBall = 4;
			}
			if (Main.masterMode)
			{
				maxJellyBall = 8;
			}
			for (int i = 0; i < maxJellyBall; i++)
			{
				NPC largeJelly = NPC.NewNPCDirect(NPC.GetSource_FromAI(), NPC.Center, ModContent.NPCType<GiantJellyBall>(), default, default, 127);
				largeJelly.velocity = new Vector2(16, 0).RotatedBy((i / (float)maxJellyBall) * MathHelper.TwoPi);
			}
		}
		for (int g = 0; g < 3; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 24)).RotatedByRandom(MathHelper.TwoPi);
			float mulScale = Main.rand.NextFloat(6f, 12f);
			var blood = new JellyBallGelDrop
			{
				velocity = afterVelocity / mulScale,
				Active = true,
				Visible = true,
				position = NPC.Center + new Vector2(Main.rand.NextFloat(-200f, 200f) * NPC.scale, 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(42, 84),
				scale = mulScale,
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
			};
			Ins.VFXManager.Add(blood);
		}
		for (int g = 0; g < 1; g++)
		{
			Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 4)).RotatedByRandom(MathHelper.TwoPi);
			var blood = new JellyBallGelSplash
			{
				velocity = afterVelocity,
				Active = true,
				Visible = true,
				position = NPC.Center + new Vector2(Main.rand.NextFloat(-200f, 200f) * NPC.scale, 0).RotatedByRandom(6.283) - afterVelocity,
				maxTime = Main.rand.Next(32, 94),
				scale = Main.rand.NextFloat(6f, 24f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.4f), 0 },
			};
			Ins.VFXManager.Add(blood);
		}

		if (NPC.life <= 0)
		{
			for (int i = 0; i < 120; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallGel>());
				d.scale *= Main.rand.NextFloat(0.7f, 1.4f);
				d.velocity = new Vector2(Main.rand.NextFloat(2, 36f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int i = 0; i < 40; i++)
			{
				Dust d = Dust.NewDustDirect(NPC.position, NPC.width, NPC.height, ModContent.DustType<JellyBallSpark>());
				d.velocity = new Vector2(Main.rand.NextFloat(2, 36f), 0).RotatedByRandom(MathHelper.TwoPi);
			}
			for (int g = 0; g < 320; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 240)).RotatedByRandom(MathHelper.TwoPi);
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
			for (int g = 0; g < 80; g++)
			{
				Vector2 afterVelocity = new Vector2(0, Main.rand.NextFloat(2, 16)).RotatedByRandom(MathHelper.TwoPi);
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
			float scaleGore = 1;
			for (int i = 0; i < 4; i++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(6, 12f)).RotatedByRandom(MathHelper.TwoPi);
				int type = ModContent.Find<ModGore>("Everglow/KingJellyBall_gore" + i).Type;
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, scaleGore);
			}
			for (int i = 0; i < 4; i++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(6, 18f)).RotatedByRandom(MathHelper.TwoPi);
				int type = ModContent.Find<ModGore>("Everglow/KingJellyBall_gore4").Type;
				Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, scaleGore);
				for (int j = 0; j < 3; j++)
				{
					v0 = new Vector2(0, Main.rand.NextFloat(2, 7f + j * 3)).RotatedByRandom(MathHelper.TwoPi);
					type = ModContent.Find<ModGore>("Everglow/KingJellyBall_gore5").Type;
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, scaleGore);
					v0 = new Vector2(0, Main.rand.NextFloat(2, 7f + j * 3)).RotatedByRandom(MathHelper.TwoPi);
					type = ModContent.Find<ModGore>("Everglow/KingJellyBall_gore6").Type;
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, scaleGore);
					v0 = new Vector2(0, Main.rand.NextFloat(2, 7f + j * 3)).RotatedByRandom(MathHelper.TwoPi);
					type = ModContent.Find<ModGore>("Everglow/KingJellyBall_gore7").Type;
					Gore.NewGore(NPC.GetSource_Death(), NPC.Center, v0, type, scaleGore);
				}
			}
			for (int i = 0; i < 35; i++)
			{
				var dustVFX = new JellyBallSparkElectricity
				{
					velocity = new Vector2(0, Main.rand.NextFloat(3, 44)).RotatedBy(Main.rand.NextFloat(MathHelper.TwoPi)),
					Active = true,
					Visible = true,
					position = NPC.Center,
					maxTime = Main.rand.Next(20, 1190),
					scale = Main.rand.Next(4, 16),
					ai = new float[] { Main.rand.NextFloat(1f, 8f), 0 },
				};
				Ins.VFXManager.Add(dustVFX);
			}
		}
		base.HitEffect(hit);
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		if (NPC.life > NPC.lifeMax * 0.5f)
		{
			NPC.StrikeNPC((int)(NPC.life * 0.012f), 0, Main.rand.NextFloat(-2, 2) > 0 ? -1 : 1);
		}
		else
		{
			modifiers.FinalDamage *= 2.1f;
		}
		base.ModifyHitPlayer(target, ref modifiers);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		float timeValue = (float)Main.time * 0.03f;

		// back
		Vector2 drawCenter = NPC.Center - Main.screenPosition + new Vector2(0, -120 * (NPC.scale - 0.3f));
		Vector2 offset = new Vector2(0, -60);
		Vector2 offsetedCenter = drawCenter + offset;
		int step = 150;
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
		List<Vertex2D> jellyBallBodyBound = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyBound_highLight = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyBound_highLight_side = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyInner_front = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyInner_back = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyInner_shallowReflect = new List<Vertex2D>();
		List<Vertex2D> jellyBallBodyInner_healGlow = new List<Vertex2D>();
		Color healGlow = Color.Lerp(new Color(0.6f, 1f, 1f, 0f), Color.Transparent, MathF.Pow(1 - HealLightValue, 3));

		// adjust center base on the polar funtion graph.
		for (int theta = 0; theta <= step; theta++)
		{
			float a = 200;
			float b = 110 + 10 * MathF.Sin(timeValue);
			float r = a - b * MathF.Sin(theta / (float)step * MathHelper.TwoPi);

			// noise wave
			for (int k = 0; k < 6; k++)
			{
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi + MathF.Sin(timeValue * MathF.Pow(2, k * 0.2f)) * 0.22f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi - timeValue * 0.33f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 2;
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi + timeValue * 0.65f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
			}
			r *= NPC.scale;
			Vector2 toDistance = new Vector2(-r, 0).RotatedBy(theta / (float)step * MathHelper.TwoPi);
			toDistance.Y *= 1.3f;
			Vector2 width = Vector2.Normalize(toDistance) * 8f;
			Color sideColor = Lighting.GetColor((offsetedCenter + toDistance + Main.screenPosition).ToTileCoordinates());
			jellyBallBodyBound.Add(offsetedCenter + toDistance - width, sideColor * 0.8f, new Vector3(theta / (float)step, 0.2f, 0));
			jellyBallBodyBound.Add(offsetedCenter + toDistance, sideColor, new Vector3(theta / (float)step, 0, 0));

			Color highlightColor = sideColor * 0.25f;
			highlightColor.A = 0;
			jellyBallBodyBound_highLight.Add(offsetedCenter + toDistance - width * 4, Color.Transparent, new Vector3(theta / (float)step * 3, 0.08f + timeValue * 0.05f, 0));
			jellyBallBodyBound_highLight.Add(offsetedCenter + toDistance - width * 0.3f, highlightColor, new Vector3(theta / (float)step * 3, 0 + timeValue * 0.05f, 0));

			jellyBallBodyBound_highLight_side.Add(offsetedCenter + toDistance - width * 1.2f, Color.Transparent, new Vector3(theta / (float)step * 3, 0.08f + timeValue * 0.05f, 0));
			jellyBallBodyBound_highLight_side.Add(offsetedCenter + toDistance - width * 0.3f, highlightColor * 2, new Vector3(theta / (float)step * 3, 0 + timeValue * 0.05f, 0));

			jellyBallBodyInner_front.Add(offsetedCenter + toDistance - width, sideColor * .8f, new Vector3(theta / (float)step, 0.2f, 0));
			jellyBallBodyInner_front.Add(offsetedCenter, drawColor * 0.2f, new Vector3(theta / (float)step, 1, 0));

			jellyBallBodyInner_back.Add(offsetedCenter + toDistance - width, Color.Lerp(sideColor, new Color(0f, 0f, 0.2f, 1f), 0.4f), new Vector3(theta / (float)step, 0.2f, 0));
			jellyBallBodyInner_back.Add(offsetedCenter, Color.Lerp(drawColor, new Color(0f, 0f, 0.2f, 1f), 0.8f), new Vector3(theta / (float)step, 1, 0));

			jellyBallBodyInner_shallowReflect.Add(offsetedCenter + toDistance - width, sideColor * 0.1f, new Vector3(offsetedCenter, 0));
			jellyBallBodyInner_shallowReflect.Add(offsetedCenter, drawColor * 0.1f, new Vector3(offsetedCenter, 0));

			jellyBallBodyInner_healGlow.Add(offsetedCenter + toDistance - width, healGlow * 0.8f, new Vector3(theta / (float)step, 0.2f, 0));
			jellyBallBodyInner_healGlow.Add(offsetedCenter, healGlow, new Vector3(theta / (float)step, 1, 0));
		}
		if (jellyBallBodyInner_back.Count >= 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.KingJellyBall_Color.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyInner_back.ToArray(), 0, jellyBallBodyInner_back.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);

		// core
		Texture2D texture = ModAsset.KingJellyBall_Core.Value;
		spriteBatch.Draw(texture, drawCenter, null, Color.Lerp(drawColor * 0.7f, new Color(0.6f, 1f, 1f, 1f), 0.4f), NPC.rotation, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
		if (HealLightValue > 0)
		{
			Texture2D texture_White = ModAsset.KingJellyBall_Core.Value;
			Texture2D texture_Black = ModAsset.KingJellyBall_Core.Value;
			spriteBatch.Draw(texture_White, drawCenter, null, Color.Lerp(Color.Cyan, Color.Transparent, HealLightValue), NPC.rotation, texture.Size() * 0.5f, 1, SpriteEffects.None, 0);
		}

		// front
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		// adjust center base on the polar funtion graph.
		// draw front (over core) structures.
		if (jellyBallBodyBound.Count >= 2 && jellyBallBodyInner_front.Count >= 2 && jellyBallBodyBound_highLight.Count > 2 && jellyBallBodyBound_highLight_side.Count > 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = ModAsset.KingJellyBall_Color.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyBound.ToArray(), 0, jellyBallBodyBound.Count - 2);
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyInner_front.ToArray(), 0, jellyBallBodyInner_front.Count - 2);
			if (HealLightValue > 0)
			{
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyInner_healGlow.ToArray(), 0, jellyBallBodyInner_healGlow.Count - 2);
			}
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_cell_rgb.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyBound_highLight.ToArray(), 0, jellyBallBodyBound_highLight.Count - 2);

			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_1.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyBound_highLight_side.ToArray(), 0, jellyBallBodyBound_highLight_side.Count - 2);
		}
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect effect = ModAsset.KingJellyBall_ShallowReflection.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uTime"].SetValue(timeValue * 0.21f);
		effect.Parameters["uSize"].SetValue(0.002f);
		effect.Parameters["uThredshold"].SetValue(0.2f);
		effect.CurrentTechnique.Passes["Test"].Apply();
		if (jellyBallBodyInner_shallowReflect.Count >= 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = Commons.ModAsset.Noise_flame_2.Value;
			Main.graphics.graphicsDevice.Textures[1] = Commons.ModAsset.Trail.Value;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyInner_shallowReflect.ToArray(), 0, jellyBallBodyInner_shallowReflect.Count - 2);
		}

		// Draw crystal that over surface while scale descend.
		Main.spriteBatch.End();
		Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

		Effect crystalOverBody = ModAsset.KingJellyBall_CoreCrystalOverSurface.Value;
		crystalOverBody.Parameters["uTransform"].SetValue(model * projection);
		crystalOverBody.Parameters["uHeatMap"].SetValue(ModAsset.KingJellyBall_Zaxis.Value);
		crystalOverBody.CurrentTechnique.Passes["Test"].Apply();
		float sizeX = texture.Size().X * 0.5f;
		float sizeY = texture.Size().Y * 0.5f;
		float scaleZ = NPC.scale + 0.3f + 0.05f * MathF.Sin(timeValue * 0.6f);
		Color crystalColor = drawColor * 1.6f;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(drawCenter + new Vector2(-sizeX, -sizeY), crystalColor, new Vector3(0, 0, scaleZ)),
			new Vertex2D(drawCenter + new Vector2(sizeX, -sizeY), crystalColor, new Vector3(1, 0, scaleZ)),

			new Vertex2D(drawCenter + new Vector2(-sizeX, sizeY), crystalColor, new Vector3(0, 1, scaleZ)),
			new Vertex2D(drawCenter + new Vector2(sizeX, sizeY), crystalColor, new Vector3(1, 1, scaleZ)),
		};
		if (jellyBallBodyInner_shallowReflect.Count >= 2)
		{
			Main.graphics.graphicsDevice.Textures[0] = texture;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		Main.spriteBatch.End();
		Main.spriteBatch.Begin(sBS);
		return false;
	}

	public override void OnKill()
	{
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<CrystalNucleusOfJellyKing>(), 1, 4, 6));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<JellyBallCube>(), 1, 24, 40));
	}
}
