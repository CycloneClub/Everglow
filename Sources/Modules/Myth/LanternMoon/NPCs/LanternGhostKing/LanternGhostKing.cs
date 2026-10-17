using Everglow.Commons.DataStructures;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Myth.LanternMoon.Gores;
using Everglow.Myth.LanternMoon.Items;
using Everglow.Myth.LanternMoon.Items.Weapons;
using Everglow.Myth.LanternMoon.LanternCommon;
using Everglow.Myth.LanternMoon.Projectiles.LanternKing;
using Everglow.Myth.LanternMoon.VFX;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Myth.LanternMoon.NPCs.LanternGhostKing;

[AutoloadBossHead]
public class LanternGhostKing : LanternMoonNPC
{
	public Vector2 RingCenterTrend;
	public Vector2 RingCenter;
	public Vector2 Lantern3RingCenter;

	public float RingRadius = 0;
	public float RingRadiusTrend = 1800;
	public float RingFade = 0;
	public float FrameworkRotation = 0;
	public float ShakeStrength = 2;
	public float EffectValueZ = 0f;
	public float GoldenShieldLerp = 0f;
	public float GoldenShieldBreakEffectTimer = 0;
	public float GoldenShieldCrackResistInYAxis = 0.14f;
	public float GoldenShieldBreakBloomValue = 0;
	public float RushDirectionPridiction = 0f;

	public int Phase = 1;
	public int Timer = 0;
	public List<int> OldSkillInPhase2 = new List<int>();

	public Rectangle BodyFrame = new Rectangle(0, 82, 270, 174);
	public Rectangle ExteriorFrameworkFrame = new Rectangle(272, 2, 538, 298);
	public Rectangle CoreAndTailFrame = new Rectangle(912, 2, 100, 182);
	public string ShaderType = "Normal";

	public Rope LanternTail = null;
	public static MassSpringContainer LanternGhostKingMassSpringSystem = new MassSpringContainer();
	public static PBDSolver LanternGhostKingPBDSolver = new PBDSolver(8);

	public override void SetDefaults()
	{
		NPC.damage = 100;
		if (Main.expertMode)
		{
			NPC.lifeMax = 20000;
		}
		else
		{
			NPC.lifeMax = 30000;
		}
		NPC.width = 250;
		NPC.height = 150;
		NPC.defense = 50;
		NPC.value = 20000;
		NPC.aiStyle = -1;
		NPC.boss = true;
		NPC.knockBackResist = 0f;
		NPC.noGravity = true;
		NPC.noTileCollide = true;
		NPC.dontTakeDamage = true;
		NPC.HitSound = SoundID.NPCHit3;
	}

	public override bool CheckActive()
	{
		return Main.dayTime;
	}

	public override void OnSpawn(IEntitySource source)
	{
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		if (musicSystem is not null)
		{
			musicSystem.Wave15StartTimer = 0;
			LanternMoon.StartWave15Music();
		}
		Timer = 0;
		Phase = 1;
		RingFade = 240;
		var spark = new LanternFlameRingDust
		{
			OwnerLanternKing = NPC,
			Active = true,
			Visible = true,
			MaxFade = 240,
		};
		Ins.VFXManager.Add(spark);
		var warp = new LanternFlameRing_warpDust
		{
			OwnerLanternKing = NPC,
			Active = true,
			Visible = true,
			MaxFade = 240,
		};
		Ins.VFXManager.Add(warp);
		RingCenter = NPC.Center;
		for (int t = 0; t < 2; t++)
		{
			float value = t - 0.5f;
			value *= 2;
			Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-200 * value, 0), Vector2.zeroVector, ModContent.ProjectileType<LanternGhostKingPower>(), 0, 0, Main.myPlayer, value);
			LanternGhostKingPower l0 = p0.ModProjectile as LanternGhostKingPower;
			if (l0 is not null)
			{
				l0.OwnerNPC = NPC;
			}
		}
		LanternMoon.Boss15Started = false;
	}

	public void UpdateDrawParameter()
	{
		FrameworkRotation = (float)Utils.Lerp(FrameworkRotation, NPC.rotation, 0.02f);
	}

	public void CheckPlayerTouchRing()
	{
		if (RingFade > 0)
		{
			return;
		}
		foreach (var player in Main.player)
		{
			if (player != null && player.active && !player.dead)
			{
				float distance = (player.Center - RingCenter).Length();
				int dir = 1;
				if ((player.Center - RingCenter).X > 0)
				{
					dir = -1;
				}
				if (Math.Abs(distance - RingRadius - 50) < 50)
				{
					player.Hurt(PlayerDeathReason.ByNPC(NPC.whoAmI), 400, dir, false, true, false, player.immuneTime);
					player.AddBuff(BuffID.OnFire3, 240);
				}
			}
		}
	}

	/// <summary>
	/// Return true when cleared all the enemies before wave 14.
	/// </summary>
	/// <returns></returns>
	public bool CanBeginAI()
	{
		int lanternMoonNPCCount = 0;
		foreach (var npc in Main.npc)
		{
			if (npc != null && npc.active && npc.ModNPC is LanternMoonNPC)
			{
				lanternMoonNPCCount++;
			}
		}
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		return lanternMoonNPCCount <= 1 && musicSystem.Wave15StartTimer >= 15 * 60;
	}

	public float GoldenShieldBreakBloomValueFunction()
	{
		float value0 = 1.01f - GoldenShieldBreakBloomValue;
		return (MathF.Sin(10 / value0) * 0.5f + 0.5f) * GoldenShieldBreakBloomValue;
	}

	public Vector4 BloomEffectColorV4()
	{
		Vector4 envC = Lighting.GetColor(NPC.Center.ToTileCoordinates()).ToVector4();
		return Vector4.Lerp(envC, Vector4.One, GoldenShieldBreakBloomValueFunction());
	}

	public override void AI()
	{
		Music = LanternMoon.SwitchMusic();
		Timer += 1;
		UpdateDrawParameter();
		NPC.TargetClosest(false);
		Player player = Main.player[NPC.target];
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		if (musicSystem.Wave15StartTimer == 14 * 60 + 10)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 150), Vector2.zeroVector, ModContent.ProjectileType<KillLanternMoonMobs>(), 75000, 0, Main.myPlayer);
			NPC.alpha = 180;
		}
		if (musicSystem.Wave15StartTimer == 15 * 60)
		{
			NPC.alpha = 0;
		}
		if (GoldenShieldBreakEffectTimer > 0)
		{
			GoldenShieldBreakEffectTimer--;
			if (GoldenShieldCrackResistInYAxis > 0)
			{
				GoldenShieldCrackResistInYAxis -= 0.002f;
			}
			if (GoldenShieldBreakEffectTimer > 30)
			{
				if (GoldenShieldBreakEffectTimer % 4 == 1)
				{
					ShakerManager.AddShaker(NPC.Center, new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi), 6, 0.8f, 16, 0.9f, 0.8f, 30);
				}
				GoldenShieldBreakBloomValue += 0.014286f;
			}
			EffectValueZ = 0.9f;
			if (GoldenShieldBreakEffectTimer == 30)
			{
				BrakeGoldenShieldEffect();
				ShakerManager.AddShaker(NPC.Center, new Vector2(0, 1).RotatedByRandom(MathHelper.TwoPi), 100, 0.8f, 16, 0.9f, 0.8f, 300);
				SoundEngine.PlaySound(SoundID.Shatter, NPC.Center);
				NPC.HitSound = SoundID.NPCHit3;
				ShaderType = "Normal";
				NPC.defense = 30;
				GoldenShieldLerp = 1;
				EffectValueZ = 0;
				GoldenShieldBreakBloomValue = 0;
			}
		}
		else
		{
			GoldenShieldBreakEffectTimer = 0;
		}
		UpdateTailRope();
		PlayerDeadHealthRecovery();
		Lighting.AddLight(NPC.Center, new Vector3(1.5f, 0.65f, 0.55f) * ((255 - NPC.alpha) / 255f) * 3);
		if (Timer <= 1)
		{
			NPC.rotation = NPC.velocity.X / 120f;
			Vector2 v = player.Center + new Vector2((float)Math.Sin(Timer / 40f) * 500f, (float)Math.Sin((Timer + 200) / 40f) * 50f - 350) - NPC.Center;
			if (musicSystem.Wave15StartTimer < 4 * 60 || musicSystem.Wave15StartTimer > 22 * 60)
			{
				if (NPC.velocity.Length() < 9f)
				{
					NPC.velocity += v.NormalizeSafe() * 0.35f;
				}
			}
			else
			{
				NPC.velocity *= 0.8f;
			}

			if (musicSystem.Wave15StartTimer == 7 * 60)
			{
				var redWave = new LanternGhostKingPowerAbsorbWave
				{
					Position = NPC.Center + new Vector2(0, 150),
					Timer = 0,
					MaxTime = 60 * 8,
					Active = true,
					Visible = true,
				};
				Ins.VFXManager.Add(redWave);
			}

			NPC.velocity *= 0.96f;
			RingCenterTrend = NPC.Center;
			RingRadiusTrend = 900;
			if (Phase == 1 && NPC.life == NPC.lifeMax)
			{
				if (!CanBeginAI())
				{
					Timer = 0;
					NPC.dontTakeDamage = true;
					if (musicSystem.Wave15StartTimer < 14 * 60)
					{
						NPC.alpha = 200;
					}
				}
				else
				{
					if (!LanternMoon.Boss15Started)
					{
						FormalStartEffect();
					}
				}
			}
		}
		else
		{
			if (CanBeginAI())
			{
				if (RingFade > 0)
				{
					RingFade -= 10;
				}
			}
			CheckPlayerTouchRing();
		}
		if (Phase == 1)
		{
			// 闪烁金线 ai1借用为晃动力
			if (Timer < 700 && Timer > 0)
			{
				GoldenLine(player);
			}

			// 灯火炸环
			if (Timer >= 700 && Timer < 1500)
			{
				Layer3LanternRing(player);
			}

			// 灯火炸弹阵法
			if (Timer >= 1500 && Timer < 1700)
			{
				LanternBombMatrix(player);
			}

			// 游离灯火
			if (Timer >= 1700 && Timer < 2400)
			{
				SmallLanternRelease(player);
			}

			// 短距离冲撞
			if (Timer >= 2400 && Timer < 3000)
			{
				RotateAndRush(player);
			}
			if (Timer >= 2998)
			{
				Timer = 0;
			}
		}
		if (Phase == 2)
		{
			// 召唤小弟
			if (Timer < 400)
			{
				Phase2_SummonMinion(player);
			}

			// 流灯河 ai1借用为位置
			if (Timer > 400 && Timer < 1200)
			{
				Phase2_LanterRiver(player);
			}

			// 千灯夜雨
			if (Timer >= 1200 && Timer < 1800)
			{
				Phase2_LanterRain(player);
			}

			// 甩出金丝 ai3借用存方向  临时向量0借为旋转中心,临时向量1借为转动半径
			if (Timer >= 1800 && Timer < 2000)
			{
				Phase2_SwingGoldenLines(player);
			}

			// 闪烁金线2 ai1借用为晃动力
			if (Timer >= 2000 && Timer < 2300)
			{
				Phase2_GoldenLine(player);
			}

			// 灯火炸环2
			if (Timer >= 2400 && Timer < 3100)
			{
				Phase2_Layer3LanternRing(player);
			}

			// 落灯柱,临时向量0借为位移目标
			if (Timer >= 3100 && Timer < 3600)
			{
				Phase2_FallingColumn(player);
			}

			// 长距离冲刺(附加火墙)
			if (Timer >= 3600 && Timer < 4200)
			{
				Phase2_Rush(player);
			}
			if (Timer >= 4200)
			{
				SwitchAttackTypeInPhase2();
			}
		}
		if (Main.dayTime)
		{
			NPC.velocity.Y += 1;
			if ((NPC.Center - player.Center).Length() > 2500)
			{
				NPC.active = false;
			}
		}

		RingRadius = RingRadius * 0.99f + RingRadiusTrend * 0.01f;
		RingCenter = RingCenter * 0.99f + RingCenterTrend * 0.01f;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TreasureBag_LanternGhostKing>()));
		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Relic_LanternGhostKing>()));
		var rule = new LeadingConditionRule(new Conditions.NotExpert());
		rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<GildingRevolver>(), ModContent.ItemType<LanternYoyo>(), ModContent.ItemType<MillionLightStaff>(), ModContent.ItemType<GoldenLotusStaff>(), ModContent.ItemType<KeroseneLanternFlameThrower>(), ModContent.ItemType<LanternSword>()));
		npcLoot.Add(rule);
	}

	public void FormalStartEffect()
	{
		LanternMoon.Boss15Started = true;
		NPC.dontTakeDamage = false;
	}

	public void BrakeGoldenShieldEffect()
	{
		for (int g = 0; g < 150; g++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 135).RotatedByRandom(2.6f);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2f;
			var sparkFlame = new LanternGoldenShieldFragiles
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + offsetPos,
				RotateSpeed = Main.rand.NextFloat(-0.3f, 0.3f),
				Rotate2Speed = Main.rand.NextFloat(-0.5f, 0.5f),
				VelocityRotateSpeed = Main.rand.NextFloat(0.15f, 0.45f) * (g % 2 - 0.5f) * 0.2f,
				Rotation = Main.rand.NextFloat(MathHelper.TwoPi),
				Rotation2 = Main.rand.NextFloat(MathHelper.TwoPi),
				MaxTime = Main.rand.Next(155, 200),
				Scale = Main.rand.NextFloat(0.6f, 1.5f),
				Frame = Main.rand.Next(0, 4),
			};
			Ins.VFXManager.Add(sparkFlame);
		}
		for (int x = 0; x < 72; x++)
		{
			float value = MathF.Pow(Main.rand.NextFloat(), 0.3f);
			Vector2 offsetPos = new Vector2(0, -value * 135).RotatedByRandom(2.6f);
			offsetPos.Y *= 85f / 135f;
			Vector2 newVelocity = offsetPos / 2f;
			var spark = new LanternGoldenShieldStar()
			{
				Velocity = newVelocity,
				Active = true,
				Visible = true,
				Position = NPC.Center + offsetPos,
				RotateSpeed = 0,
				Rotation = 0,
				MaxTime = Main.rand.Next(50, 100),
				Scale = Main.rand.NextFloat(0.5f, 1f),
			};
			Ins.VFXManager.Add(spark);
		}
	}

	/// <summary>
	/// Release golden line proj, ai[1] lent as a para for swing force.<br/>
	/// 0-700
	/// </summary>
	/// <param name="target"></param>
	public void GoldenLine(Player target)
	{
		float duration = Timer % 100;
		if (duration < 50)
		{
			NPC.rotation = NPC.velocity.X / 120f;
			Vector2 v = target.Center + new Vector2((float)Math.Sin(Main.time * 0.045f) * 500f, (float)Math.Sin(Main.time * 0.075f + 20) * 50f - 350) - NPC.Center;
			NPC.velocity += v.NormalizeSafe() * 1f;
			NPC.velocity *= 0.96f;
		}
		else if (duration >= 50 && duration < 60)
		{
			NPC.rotation = NPC.velocity.X / 120f;
			NPC.velocity *= 0.8f;
		}
		else
		{
			if (duration == 60)
			{
				NPC.rotation = 0.6f;
				NPC.ai[1] = -0.2f;
			}
			if (duration % 2 == 1 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 v0 = new Vector2(0, 24 * MathF.Abs(NPC.ai[1]) + 12).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 20) + v0 * 6, v0, ModContent.ProjectileType<GoldLanternLine>(), 40, 0f, target.whoAmI, 0, 0);
				float myDamage = 40;
				if (Main.expertMode)
				{
					myDamage = 55;
				}
				if (Main.masterMode)
				{
					myDamage = 70;
				}
				p0.damage = (int)myDamage / 2;
			}
			NPC.rotation += NPC.ai[1];
			NPC.ai[1] -= NPC.rotation * 0.07f;
			NPC.rotation *= 0.9f;
		}

		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
	}

	/// <summary>
	/// Spawn 3 layers of lantern ring and release with a high speed then descenting gradually.<br/>
	/// 700-1500
	/// </summary>
	/// <param name="target"></param>
	public void Layer3LanternRing(Player target)
	{
		if (Timer % 250 == 0)
		{
			Lantern3RingCenter = new Vector2(0, -300).RotatedBy(Main.rand.NextFloat(-0.25f, 0.25f) * Math.PI) + target.Center;
			var matrix = new LanternGhostKing_Wheel3Mark()
			{
				Active = true,
				Visible = true,
				Position = Lantern3RingCenter,
				Rotation = 0.585f,
				MaxTime = 1500,
				ExtraUpdate = 6,
				Scale = 0.25f,
			};
			Ins.VFXManager.Add(matrix);
		}

		if (Timer % 250 < 20)
		{
			NPC.velocity *= 0.95f;
			NPC.rotation *= 0.95f;
		}
		if (Timer % 250 < 30 && Timer % 250 < 20)
		{
			NPC.velocity *= 0;
			NPC.rotation *= 0.95f;
		}
		if (Timer % 250 == 1 && Main.netMode != NetmodeID.MultiplayerClient)
		{
			int myDamage = 50;
			if (Main.expertMode)
			{
				myDamage = 75;
			}
			if (Main.masterMode)
			{
				myDamage = 100;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<WheelShapeLantern3Layer>(), myDamage, 0f, target.whoAmI, 0, 0);
		}
		if (Timer % 250 > 120)
		{
			Vector2 v = Lantern3RingCenter - NPC.velocity * 30f;
			NPC.velocity += (v - NPC.Center) / (v - NPC.Center).Length() * 0.25f;
			if (NPC.velocity.Length() > 20f)
			{
				NPC.velocity *= 0.96f;
			}
		}
		if ((Timer is 999 or 1249) && Main.rand.NextBool(3))
		{
			Timer = 1500;
		}
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
	}

	/// <summary>
	/// Spawn a series of lantern bomb around.<br/>
	/// 1500-1700
	/// </summary>
	/// <param name="target"></param>
	public void LanternBombMatrix(Player target)
	{
		NPC.rotation *= 0.95f;
		NPC.velocity *= 0.95f;
		if (Timer == 1600 && Main.netMode != NetmodeID.MultiplayerClient)
		{
			for (int i = 0; i < 10; i++)
			{
				for (int j = 0; j < i * 3; j++)
				{
					Vector2 v2 = new Vector2(0, i * 70 + 400).RotatedBy(j / (i * 3f) * MathHelper.TwoPi + i * 4);
					var p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + v2, Vector2.zeroVector, ModContent.ProjectileType<LanternBomb>(), 50, 0f, target.whoAmI, 0, 0);
					float myDamage = 50;
					if (Main.expertMode)
					{
						myDamage = 75;
					}
					if (Main.masterMode)
					{
						myDamage = 100;
					}
					p0.damage = (int)myDamage / 2;
				}
			}
		}
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1000;
	}

	/// <summary>
	/// Release a series of small lantern regularly.<br/>
	/// 1700-2300
	/// </summary>
	/// <param name="target"></param>
	public void SmallLanternRelease(Player target)
	{
		NPC.rotation *= 0.95f;
		NPC.velocity *= 0.95f;
		if (Timer == 1820 && Main.netMode != NetmodeID.MultiplayerClient)
		{
			int myDamage = 20;
			if (Main.expertMode)
			{
				myDamage = 25;
			}
			if (Main.masterMode)
			{
				myDamage = 40;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 150), Vector2.zeroVector, ModContent.ProjectileType<SmallLanternGroup>(), myDamage, 0f, target.whoAmI, Main.rand.Next(4), 0);
		}
		RingCenterTrend = NPC.Center + new Vector2(0, 150);
		RingRadiusTrend = 1000;
	}

	/// <summary>
	/// Spawn a series of lantern bomb around.<br/>
	/// 2300-2900
	/// </summary>
	/// <param name="target"></param>
	public void RotateAndRush(Player target)
	{
		int offsetTimer = Timer - 2400;
		int dashTimer = offsetTimer % 60;
		if (dashTimer < 10)
		{
			NPC.velocity *= 0.9f;
		}
		if (dashTimer == 10 && Timer > 2410)
		{
			Vector2 dir = new Vector2(1, 0).RotatedBy(RushDirectionPridiction).NormalizeSafe() * 16f;
			NPC.velocity += dir;
		}
		if (dashTimer == 30 && Timer < 2970)
		{
			var matrix = new LanternGhostKing_Matrix()
			{
				Active = true,
				Visible = true,
				Position = Main.MouseWorld,
				Rotation = 0.585f,
				MaxTime = 300,
				ExtraUpdate = 6,
				Scale = 0.45f,
				OwnerLanternGhostKing = NPC,
			};
			Ins.VFXManager.Add(matrix);
		}
		if (dashTimer >= 30)
		{
			RushDirectionPridiction = (target.Center - NPC.Center).ToRotation();
		}
		if (dashTimer >= 50)
		{
			NPC.velocity *= 0.9f;
		}

		NPC.rotation = NPC.velocity.X * 0.01f;
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1000;
	}

	/// <summary>
	/// Summon little lanterns.<br/>
	/// 0-400
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_SummonMinion(Player target)
	{
		int maxCount = 20;
		if (Main.expertMode)
		{
			maxCount = 30;
		}
		if (Main.masterMode)
		{
			maxCount = 45;
		}
		if (Timer <= maxCount * 4)
		{
			NPC.HitSound = SoundID.NPCHit4;
			if (Timer % 20 == 0)
			{
				for (int k = 0; k < 5; k++)
				{
					NPC npc = NPC.NewNPCDirect(null, NPC.Center + new Vector2(0, 100), ModContent.NPCType<EvilLantern>(), 0, 0, 0, 0, 0, 255);
					npc.velocity = new Vector2(0, 16).RotatedBy(k / 5f * MathHelper.TwoPi + Main.time);
				}
			}
			ShaderType = "GoldenSheild";
			if (GoldenShieldLerp < 0.6f)
			{
				GoldenShieldLerp += 0.012f;
			}
			if (EffectValueZ < 1.2f)
			{
				EffectValueZ = 1.2f;
			}
		}

		NPC.rotation *= 0.95f;
		Vector2 v = target.Center + new Vector2(0, -350) - NPC.Center;
		if (NPC.velocity.Length() < 9f && GoldenShieldBreakEffectTimer <= 0)
		{
			NPC.velocity += v.NormalizeSafe() * 0.35f;
			NPC.velocity.X += target.velocity.X * 0.07f;
		}
		NPC.velocity *= 0.96f;
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
		if (Timer >= 260)
		{
			int minionNumber = NPC.CountNPCS(ModContent.NPCType<EvilLantern>());
			if (minionNumber >= 1)
			{
				Timer = 260;
				if (NPC.CountNPCS(ModContent.NPCType<EvilLantern>()) < 36)
				{
					EffectValueZ = minionNumber / 36f * 0.3f + 0.9f;
				}
			}
			else
			{
				if (Timer < 280)
				{
					Timer = 280;
					EffectValueZ = 0.9f;
					GoldenShieldBreakEffectTimer = 100;
				}
			}
		}
		if (Timer >= 399)
		{
			Timer = 405;
		}

		NPC.rotation = NPC.velocity.X * 0.01f;
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
	}

	/// <summary>
	/// 5 Main flows of lantern river.<br/>
	/// 400-1200
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_LanterRiver(Player target)
	{
		NPC.rotation *= 0.96f;
		if (Timer == 406)
		{
			NPC.ai[1] = 1;
			if (Main.rand.NextBool(2))
			{
				NPC.ai[1] = -1;
			}
		}
		if (Timer < 430)
		{
			NPC.velocity *= 0;
			NPC.alpha += 10;
			NPC.dontTakeDamage = true;
			ShaderType = "DecayAndFade";
		}
		if (Timer == 430)
		{
			Vector2 testPos = target.Center + new Vector2(600 * NPC.ai[1], 0);
			int count = 0;
			while (Collision.SolidCollision(testPos - new Vector2(50), 100, 100))
			{
				testPos.Y -= 100;
				count++;
				if (count > 10 || testPos.Y < 1000)
				{
					break;
				}
			}
			TeleportTo(testPos);
		}
		if (Timer > 430 && Timer < 450)
		{
			NPC.velocity *= 0;
			NPC.alpha -= 20;
			if (NPC.alpha < 0)
			{
				NPC.alpha = 0;
			}
		}
		if (Timer == 450)
		{
			ShaderType = "Normal";
			NPC.velocity *= 0;
			NPC.alpha = 0;
			RingCenterTrend = NPC.Center;
		}
		if (Timer == 455)
		{
			int count = 3;
			int damage = 45;
			if (Main.expertMode)
			{
				count = 4;
				damage = 67;
			}
			if (Main.masterMode)
			{
				count = 5;
				damage = 85;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				float addValue = Main.rand.NextFloat(6.283f);
				for (int x = 0; x < count; x++)
				{
					Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 50) + new Vector2(2000, 0).RotatedBy(x / (float)count * MathHelper.TwoPi + addValue), new Vector2(-11, 0).RotatedBy(x / (float)count * MathHelper.TwoPi + addValue), ModContent.ProjectileType<LanternFlow>(), damage, 0f, target.whoAmI, 0.02f, 0);
					LanternFlow lanternF = p0.ModProjectile as LanternFlow;
					lanternF.OwnerNPC = NPC;
					lanternF.MinDisToNPC = 500;
					lanternF.VelDecay = 0.995f;
					lanternF.RotateSpeed = -0.0598575436f;
					lanternF.BestRotateSpeed = 0;
					lanternF.BestVelDecay = 0;
				}
			}
		}
	}

	/// <summary>
	/// Lantern Rain.<br/>
	/// 1200-1800
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_LanterRain(Player target)
	{
		NPC.rotation *= 0.96f;
		if (Timer == 1206)
		{
			NPC.ai[1] = 1;
			if (Main.rand.NextBool(2))
			{
				NPC.ai[1] = -1;
			}
		}
		if (Timer < 1230)
		{
			NPC.velocity *= 0;
			NPC.alpha += 10;
			NPC.dontTakeDamage = true;
			ShaderType = "DecayAndFade";
		}
		if (Timer == 1230)
		{
			Vector2 testPos = target.Center + new Vector2(200 * NPC.ai[1], 0);

			// int count = 0;
			// while (Collision.SolidCollision(testPos - new Vector2(50), 100, 100))
			// {
			// testPos.Y -= 100;
			// count++;
			// if (count > 10 || testPos.Y < 1000)
			// {
			// break;
			// }
			// }
			TeleportTo(testPos);
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<SmallLanternGroup_LanternRain>(), 50, 0f, target.whoAmI, Main.rand.Next(4), 0);
			}
		}
		if (Timer > 1230 && Timer < 1250)
		{
			NPC.velocity *= 0;
			NPC.alpha -= 20;
			if (NPC.alpha < 0)
			{
				NPC.dontTakeDamage = false;
				NPC.alpha = 0;
			}
		}
		if (Timer == 1250)
		{
			NPC.velocity *= 0;
			NPC.alpha = 0;
			RingCenterTrend = NPC.Center;
			ShaderType = "Normal";
		}

		// 释放一波灯雨
		if (Timer >= 1250 && Timer % 67 == 0)
		{
			for (int x = -3; x < 4; x++)
			{
				float deltaY = MathF.Sin(x + (float)Main.time * 0.2f) * 60;
				float deltaY2 = MathF.Cos(x + (float)Main.time * 0.2f) * 60;
				NPC npc0 = NPC.NewNPCDirect(NPC.GetSource_FromAI(), target.Center - new Vector2(0, 1000), ModContent.NPCType<ExplosiveLantern_growing>(), 0, 0f, 0, 0, 0);

				npc0.Center = target.Center + new Vector2(x * 600 + Timer % 135 - 70, -700 + deltaY);
				npc0.velocity = new Vector2(0, -deltaY2 * 0.005f);
				npc0.damage = NPC.damage / 2;
			}
		}
		if (Timer == 1799)
		{
			SwitchAttackTypeInPhase2();
		}
	}

	/// <summary>
	/// Swing, rush and release a series of star.<br/>
	/// 1800-2000
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_SwingGoldenLines(Player target)
	{
		if (Timer == 1801)
		{
			NPC.ai[3] = 1;
			if (Main.rand.NextBool(2))
			{
				NPC.ai[3] = -1;
			}
		}
		if (Timer < 1830)
		{
			NPC.velocity *= 0;
			NPC.alpha += 10;
			NPC.dontTakeDamage = true;
			ShaderType = "DecayAndFade";
		}
		if (Timer == 1830)
		{
			Vector2 testPos = target.Center + new Vector2(600 * NPC.ai[3], 200);

			// int count = 0;
			// while (Collision.SolidCollision(testPos - new Vector2(50), 100, 100))
			// {
			// testPos.Y -= 100;
			// count++;
			// if (count > 10 || testPos.Y < 1000)
			// {
			// break;
			// }
			// }
			TeleportTo(testPos);
			RingCenterTrend = NPC.Center;
		}
		if (Timer > 1830 && Timer < 1850)
		{
			NPC.velocity *= 0;
			NPC.alpha -= 20;
			if (NPC.alpha < 0)
			{
				NPC.dontTakeDamage = false;
				NPC.alpha = 0;
			}
		}
		if (Timer == 1850)
		{
			NPC.velocity *= 0;
			NPC.alpha = 0;
			ShaderType = "Normal";
			innerVector0 = NPC.Center + new Vector2(-NPC.ai[3] * 600, -200);
			innerVector1 = new Vector2(NPC.ai[3] * 600, 200);
		}
		if (Timer > 1880 && Timer <= 1910)
		{
			float rotValue = (1910 - Timer) / 30f;
			rotValue = MathF.Pow(rotValue, 3);
			rotValue = 1 - rotValue;
			rotValue *= 3.0f;
			Vector2 addValue = innerVector1.RotatedBy(rotValue * NPC.ai[3]);
			NPC.Center = innerVector0 + addValue;
			NPC.rotation = MathF.Atan2(addValue.Y, addValue.X) - MathHelper.PiOver2;
			if (Timer % 2 == 0 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				Vector2 v0 = -Vector2.Normalize(addValue) * 10f;
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + v0 * 6, v0, ModContent.ProjectileType<GoldLanternLine3>(), 40, 0f, target.whoAmI, 0, 0);
				float myDamage = 40;
				if (Main.expertMode)
				{
					myDamage = 55;
				}
				if (Main.masterMode)
				{
					myDamage = 70;
				}
				p0.damage = (int)myDamage / 2;
			}
		}
		if (Timer > 1910)
		{
			FrameworkRotation *= 0.9f;
			NPC.rotation *= 0.7f;
		}
		if (Timer == 1939)
		{
			SwitchAttackTypeInPhase2();
		}
	}

	/// <summary>
	/// Release golden line proj, ai[1] lent as a para for swing force.<br/>
	/// 2000-2300
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_GoldenLine(Player target)
	{
		float duration = Timer % 60;
		if (duration < 30)
		{
			NPC.rotation = NPC.velocity.X / 120f;
			Vector2 v = target.Center + new Vector2((float)Math.Sin(Main.time * 0.045f) * 500f, (float)Math.Sin(Main.time * 0.075f + 20) * 50f - 350) - NPC.Center;
			NPC.velocity += Vector2.Normalize(v) * 1.3f;
			NPC.velocity *= 0.96f;
		}
		else if (duration >= 30 && duration < 35)
		{
			NPC.rotation = NPC.velocity.X / 120f;
			NPC.velocity *= 0.8f;
		}
		else
		{
			if (duration == 40)
			{
				NPC.ai[2] = 1;
				if (Main.rand.NextBool(2))
				{
					NPC.ai[2] = -1;
				}
				NPC.rotation = 0.9f * NPC.ai[2];
				NPC.ai[1] = -0.3f;
			}
			Vector2 v0 = new Vector2(0, 24 * MathF.Abs(NPC.ai[1]) + 12).RotatedBy(Main.rand.NextFloat(-0.4f, 0.4f));
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 20) + v0 * 6, v0, ModContent.ProjectileType<GoldLanternLine>(), 40, 0f, target.whoAmI, 0, 0);
				float myDamage = 40;
				if (Main.expertMode)
				{
					myDamage = 55;
				}
				if (Main.masterMode)
				{
					myDamage = 70;
				}
				p0.damage = (int)myDamage / 2;
			}
			NPC.rotation += NPC.ai[1];
			NPC.ai[1] -= NPC.rotation * 0.07f;
			NPC.rotation *= 0.9f;
		}

		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
		if (Timer == 2299)
		{
			SwitchAttackTypeInPhase2();
		}
	}

	/// <summary>
	/// Spawn 3 layers of lantern ring and release with a high speed then descenting gradually.<br/>
	/// 2400-3100
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_Layer3LanternRing(Player target)
	{
		if (Timer < 3000)
		{
			if (Timer % 250 == 0)
			{
				Lantern3RingCenter = new Vector2(0, -300).RotatedBy(Main.rand.NextFloat(-1.3f, 1.3f)) + target.Center;
				var matrix = new LanternGhostKing_Wheel3Mark()
				{
					Active = true,
					Visible = true,
					Position = Lantern3RingCenter,
					Rotation = 0.585f,
					MaxTime = 1500,
					ExtraUpdate = 6,
					Scale = 0.25f,
				};
				Ins.VFXManager.Add(matrix);
			}

			if (Timer % 250 < 20)
			{
				NPC.velocity *= 0.95f;
				NPC.rotation *= 0.85f;
			}
			if (Timer % 250 < 30 && Timer % 250 < 20)
			{
				NPC.velocity *= 0;
				NPC.rotation *= 0.95f;
			}
			if (Timer % 250 == 1 && Main.netMode != NetmodeID.MultiplayerClient)
			{
				int myDamage = 50;
				if (Main.expertMode)
				{
					myDamage = 75;
				}
				if (Main.masterMode)
				{
					myDamage = 100;
				}
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<WheelShapeLantern3Layer>(), myDamage, 0f, target.whoAmI, 0, 0);
				if (Main.expertMode)
				{
					for (int i = 0; i < 8; i++)
					{
						Vector2 vel = new Vector2(0, 12).RotatedBy(i / 8f * MathHelper.TwoPi);
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<GoldLanternLine_NoTarget>(), myDamage, 0f, target.whoAmI, (i + 0.22f) / 8f * MathHelper.TwoPi + MathHelper.PiOver2, 0);
						GoldLanternLine_NoTarget gLLNT0 = p0.ModProjectile as GoldLanternLine_NoTarget;
						if (gLLNT0 is not null)
						{
							gLLNT0.Timer = Main.rand.Next(-5, 5);
						}

						Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<GoldLanternLine_NoTarget>(), myDamage, 0f, target.whoAmI, (i - 0.22f) / 8f * MathHelper.TwoPi + MathHelper.PiOver2, 0);
						GoldLanternLine_NoTarget gLLNT1 = p1.ModProjectile as GoldLanternLine_NoTarget;
						if (gLLNT1 is not null)
						{
							gLLNT1.Timer = Main.rand.Next(-5, 5);
						}
					}
				}
			}
			if (Timer % 250 > 120 && Timer > 2500)
			{
				Vector2 v = Lantern3RingCenter - NPC.velocity * 30f;
				NPC.velocity += (v - NPC.Center - NPC.velocity) * 0.015f;
				NPC.velocity *= 0.96f;
				NPC.rotation = NPC.velocity.X * 0.02f;
			}
			RingCenterTrend = NPC.Center;
			RingRadiusTrend = 1200;
		}
		else
		{
			NPC.velocity *= 0.95f;
			NPC.rotation *= 0.85f;
			if (Timer >= 3029)
			{
				SwitchAttackTypeInPhase2();
			}
		}
	}

	/// <summary>
	/// Summon random verticle lantern flow column and release golden ray particles.
	/// 3100-3600
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_FallingColumn(Player target)
	{
		if (Timer == 3103)
		{
			float distance = 600;
			if (Main.expertMode)
			{
				distance = 400;
			}
			if (Main.masterMode)
			{
				distance = 360;
			}
			for (int i = 0; i < 8; i++)
			{
				if (Main.netMode != NetmodeID.MultiplayerClient)
				{
					Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2((i - 3.5f) * distance, -600), Vector2.Zero, ModContent.ProjectileType<LanternFlowLine>(), 40, 0f, target.whoAmI, 0, 0);
					p0.timeLeft = Main.rand.Next(580, 620);
				}
			}
		}
		NPC.rotation *= 0.9f;
		NPC.velocity *= 0.9f;

		if (Timer % 100 < 30)
		{
			NPC.velocity *= 0;
			NPC.alpha += 10;
			NPC.dontTakeDamage = true;
			ShaderType = "DecayAndFade";
		}
		if (Timer % 100 == 30)
		{
			Vector2 testPos = target.Center + new Vector2(0, -700).RotatedBy((Main.rand.NextFloat(-0.3f, 0.3f) + MathHelper.PiOver2) * (Main.rand.NextBool(2) ? 1 : -1));

			// while (Collision.SolidCollision(testPos - new Vector2(50), 100, 100))
			// {
			// testPos.Y -= 100;
			// count++;
			// if (count > 10 || testPos.Y < 1000)
			// {
			// break;
			// }
			// }
			TeleportTo(testPos);
			RingCenterTrend = NPC.Center;
		}
		if (Timer % 100 > 30 && Timer % 100 < 50)
		{
			NPC.velocity *= 0;
			NPC.alpha -= 20;
			if (NPC.alpha < 0)
			{
				NPC.dontTakeDamage = false;
				NPC.alpha = 0;
			}
		}
		if (Timer % 100 == 50)
		{
			NPC.velocity *= 0;
			NPC.alpha = 0;
			ShaderType = "Normal";
			int count = 9;
			if (Main.expertMode)
			{
				count = 15;
			}
			if (Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < count; i++)
				{
					Vector2 v0 = new Vector2(0, count).RotatedBy(i / (float)count * MathHelper.TwoPi);
					Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 20) + v0, v0, ModContent.ProjectileType<GoldLanternLine>(), 40, 0f, target.whoAmI, 0, 0);
					float myDamage = 40;
					if (Main.expertMode)
					{
						myDamage = 55;
					}
					if (Main.masterMode)
					{
						myDamage = 70;
					}
					p0.damage = (int)myDamage / 2;
					GoldLanternLine gLL = p0.ModProjectile as GoldLanternLine;
					if (gLL is not null)
					{
						gLL.Timer = Main.rand.Next(-10, 10);
					}
				}
			}
			if (Main.masterMode && Main.netMode != NetmodeID.MultiplayerClient)
			{
				for (int i = 0; i < 15; i++)
				{
					Vector2 v0 = new Vector2(0, 8).RotatedBy((i + 0.5f) / 15f * MathHelper.TwoPi);
					Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, 20) + v0, v0, ModContent.ProjectileType<GoldLanternLine>(), 40, 0f, target.whoAmI, 0, 0);
					float myDamage = 40;
					if (Main.expertMode)
					{
						myDamage = 55;
					}
					if (Main.masterMode)
					{
						myDamage = 70;
					}
					p0.damage = (int)myDamage / 2;
					GoldLanternLine gLL = p0.ModProjectile as GoldLanternLine;
					if (gLL is not null)
					{
						gLL.Timer = Main.rand.Next(-10, 10);
					}
				}
			}
		}
		if (Timer == 3599)
		{
			SwitchAttackTypeInPhase2();
		}
	}

	/// <summary>
	/// Long distance rush.<br/>
	/// 3600-4200
	/// </summary>
	/// <param name="target"></param>
	public void Phase2_Rush(Player target)
	{
		int offsetTimer = Timer - 3600;
		int dashTimer = offsetTimer % 100;
		if (dashTimer < 10)
		{
			NPC.velocity *= 0.9f;
		}
		if (dashTimer == 10 && Timer > 3610)
		{
			Vector2 dir = (target.Center - NPC.Center).NormalizeSafe() * 25f;
			NPC.velocity += dir;
		}
		if (dashTimer == 10 && Timer > 3610 && Main.netMode != NetmodeID.MultiplayerClient && Main.expertMode)
		{
			var dir = (NPC.Center - target.Center).NormalizeSafe() * (RingRadius - 20);
			float myDamage = 55;
			float speed = 8f;
			if (Main.masterMode)
			{
				speed = 10f;
				myDamage = 70;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), RingCenter + dir, -dir.NormalizeSafe() * speed, ModContent.ProjectileType<LanternFlameWall>(), (int)myDamage, 1, target.whoAmI);
		}
		if (dashTimer == 40 && Timer < 4140)
		{
			var matrix = new LanternGhostKing_Matrix()
			{
				Active = true,
				Visible = true,
				Position = Main.MouseWorld,
				Rotation = 0.585f,
				MaxTime = 300,
				ExtraUpdate = 4,
				Scale = 0.8f,
				OwnerLanternGhostKing = NPC,
			};
			Ins.VFXManager.Add(matrix);
		}
		if (dashTimer >= 40)
		{
			RushDirectionPridiction = (target.Center - NPC.Center).ToRotation();
		}
		if (dashTimer >= 55)
		{
			NPC.velocity *= 0.95f;
		}
		NPC.rotation = NPC.velocity.X * 0.01f;
		RingCenterTrend = NPC.Center;
		RingRadiusTrend = 1200;
		if (Timer == 4199)
		{
			SwitchAttackTypeInPhase2();
		}
	}

	public void TeleportTo(Vector2 destination)
	{
		NPC.Center = destination;

		for (int i = 0; i < LanternTail.Masses.Length; i++)
		{
			LanternTail.Masses[i].Position += destination - NPC.Center;
		}
	}

	public void PlayerDeadHealthRecovery()
	{
		bool allDie = true;
		foreach (var player in Main.player)
		{
			if (player != null && player.active && !player.dead)
			{
				allDie = false;
				return;
			}
		}
		if (allDie)
		{
			if (NPC.life < NPC.lifeMax)
			{
				NPC.life += 10;
			}
			else
			{
				NPC.life = NPC.lifeMax;
			}
		}
	}

	/// <summary>
	/// 内部临时向量，用来存各种技能位置
	/// </summary>
	private Vector2 innerVector0 = Vector2.zeroVector;
	private Vector2 innerVector1 = Vector2.zeroVector;

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (NPC.life < NPC.lifeMax / 2)
		{
			if (Phase == 1)
			{
				Phase = 2;
				Timer = 0;
				NPC.defense = 1000;
				var crackVFX = new LanternCrackingRay()
				{
					Active = true,
					Visible = true,
					Timer = 0,
					MaxTime = 90,
					OwnerLanternKing = NPC,
				};
				Ins.VFXManager.Add(crackVFX);
			}
		}
	}

	public override void OnKill()
	{
		for (int g = 0; g < 24; g++)
		{
			Vector2 vel = new Vector2(MathF.Sqrt(Main.rand.NextFloat()) * 36f, 0).RotatedByRandom(MathHelper.TwoPi);
			string texturePath = ModAsset.LanternGhostKing_Gore_0_Mod;
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

		if (Main.netMode != NetmodeID.MultiplayerClient)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<LanternGhostKingExplosion>(), 150, 0f, Main.myPlayer);
		}
		SoundStyle sound;
		switch (Main.rand.Next(3))
		{
			case 0:
				sound = new SoundStyle(ModAsset.LanternYoyo_Explode0_Mod);
				break;
			case 1:
				sound = new SoundStyle(ModAsset.LanternYoyo_Explode1_Mod);
				break;
			case 2:
				sound = new SoundStyle(ModAsset.LanternYoyo_Explode2_Mod);
				break;
			default:
				sound = new SoundStyle(ModAsset.LanternYoyo_Explode0_Mod);
				break;
		}
		SoundEngine.PlaySound(sound, NPC.Center);
		LanternMoonMusicManager musicSystem = ModContent.GetInstance<LanternMoonMusicManager>();
		if (musicSystem is not null)
		{
			musicSystem.FadeAllTheCurrentMusic(120);
		}
		if (LanternMoon.ScoreRequireOfWave is not null && LanternMoon.ScoreRequireOfWave.Length > 15 && LanternMoon.Active)
		{
			LanternMoon.Boss15Ended = true;
			LanternMoon.NewWave();
			LanternMoon.AccumulatedScore = LanternMoon.ScoreRequireOfWave.Take(15).Sum();
		}
		base.OnKill();
	}

	public void SwitchAttackTypeInPhase2()
	{
		// Main.rand.Next(6)
		int value = Main.rand.Next(6);
		int skillCount = 0;
		foreach (var skill in OldSkillInPhase2)
		{
			if (skill == value)
			{
				skillCount++;
			}
		}
		int safeTime = 0;
		while (skillCount > OldSkillInPhase2.Count / 6f)
		{
			safeTime++;
			value = Main.rand.Next(6);
			skillCount = 0;
			foreach (var skill in OldSkillInPhase2)
			{
				if (skill == value)
				{
					skillCount++;
				}
			}
			if (safeTime > 10)
			{
				break;
			}
		}
		switch (value)
		{
			case 0:
				Timer = 1200;
				break;
			case 1:
				Timer = 1800;
				break;
			case 2:
				Timer = 2000;
				break;
			case 3:
				Timer = 2400;
				break;
			case 4:
				Timer = 3100;
				break;
			case 5:
				// Long distance rush
				Timer = 3600;
				break;
		}
		OldSkillInPhase2.Add(value);
	}

	public override bool SpecialOnKill()
	{
		return false;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D tex = ModAsset.LanternGhostKing_Atlas.Value;
		Vector2 drawPos = NPC.Center;
		DrawTail(spriteBatch);

		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		Effect ef = ModAsset.LanternGhostKing_Shader.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.ZoomMatrix;
		ef.Parameters["uTransform"].SetValue(model * projection);
		ef.Parameters["size1"].SetValue(new Vector2(1f, 1 / 3f) * 4f);
		ef.Parameters["size2"].SetValue(Vector2.One);
		ef.Parameters["size3"].SetValue(Vector2.One);
		ef.Parameters["bloomEffectColor"].SetValue(BloomEffectColorV4());
		ef.Parameters["npcAlpha"].SetValue(255 - NPC.alpha);
		ef.Parameters["lerpGolden"].SetValue(1 - GoldenShieldLerp);
		ef.Parameters["uTime"].SetValue((float)Main.timeForVisualEffects * 0.0005f);
		ef.Parameters["warpScale"].SetValue(0f);
		ef.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_RGB_Large.Value);
		ef.CurrentTechnique.Passes[ShaderType].Apply();

		DrawVertexTexture(tex, drawPos, BodyFrame, NPC.rotation, BodyFrame.Size() * 0.5f);

		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);
		ef.CurrentTechnique.Passes["Normal"].Apply();
		DrawVertexTexture(tex, drawPos, ExteriorFrameworkFrame, FrameworkRotation, new Vector2(ExteriorFrameworkFrame.Width * 0.5f, 170));
		spriteBatch.End();
		spriteBatch.Begin(sBS);
		//// 第二阶段十字光辉
		if (Phase == 2)
		{
			if (Timer >= 700)
			{
				float value = (Timer - 700) / 100f;
				value = Math.Clamp(value, 0, 1);
				value *= value;
				Texture2D star = Commons.ModAsset.StarSlash.Value;
				Vector2 orig = star.Size() / 2f;
				Vector2 offset = new Vector2(0, 50);
				float value2 = (255 - NPC.alpha) / 255f;
				float mulSize = MathF.Sin((float)Main.timeForVisualEffects * 0.4f) * 0.05f + 1;
				spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.7f, 0.5f, 0), 0, orig, new Vector2(value, 0.5f * mulSize) * mulSize * value2, SpriteEffects.None, 0f);
				spriteBatch.Draw(star, NPC.Center - Main.screenPosition + offset, null, new Color(1f, 0.7f, 0.5f, 0), MathHelper.PiOver2, orig, new Vector2(value, 2.5f * mulSize) * mulSize * value2, SpriteEffects.None, 0f);
			}
		}
		if (GoldenShieldBreakBloomValue > 0)
		{
			var bloom = ModAsset.LanternGhostKing_BodyBloom.Value;
			spriteBatch.Draw(bloom, NPC.Center - Main.screenPosition, null, new Color(1f, 0.8f, 0.2f, 0) * GoldenShieldBreakBloomValueFunction(), 0, bloom.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
		}
		if (GoldenShieldBreakEffectTimer is >= 40 and < 50)
		{
			if (GoldenShieldBreakEffectTimer % 10 > 4)
			{
				var crack = ModAsset.LanternGhostKing_Body_CrackVFXEffect.Value;
				spriteBatch.Draw(crack, NPC.Center - Main.screenPosition, null, new Color(1f, 1f, 0.6f, 0.5f), 0, crack.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
			}
			else
			{
				var crack = ModAsset.LanternGhostKing_Body_CrackVFXEffect.Value;
				spriteBatch.Draw(crack, NPC.Center - Main.screenPosition, null, new Color(0f, 0f, 0f, 1f), 0, crack.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
			}
		}
		if (GoldenShieldBreakEffectTimer is >= 30 and < 40)
		{
			if (GoldenShieldBreakEffectTimer % 6 > 2)
			{
				var crack = ModAsset.LanternGhostKing_Body_CrackVFXEffect.Value;
				spriteBatch.Draw(crack, NPC.Center - Main.screenPosition, null, new Color(0f, 0f, 0f, 1f), 0, crack.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
			}
			else
			{
				var crack = ModAsset.LanternGhostKing_Body_CrackVFXEffect.Value;
				spriteBatch.Draw(crack, NPC.Center - Main.screenPosition, null, new Color(1f, 1f, 1f, 1f), 0, crack.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0f);
			}
		}
	}

	public void UpdateTailRope()
	{
		if (LanternTail == null)
		{
			LanternTail = Rope.Create_Fixed_StartPos(Main.MouseWorld, 8, 2f, 0.4f, 4f);
			LanternGhostKingMassSpringSystem.AddMassSpringMesh(LanternTail);
		}
		LanternTail.Masses[0].Position = NPC.Center + new Vector2(0, 50).RotatedBy(NPC.rotation);
		for (int i = 1; i < LanternTail.Masses.Length; i++)
		{
			float damping = 0.3f;
			float toEnd = LanternTail.Masses.Length - i - 1;
			if (toEnd < 3)
			{
				damping += (1 - toEnd / 3f) * 0.1f;
			}
			float toStart = i - 1;
			if (toStart < 3)
			{
				damping *= toStart / 3f;
			}
			LanternTail.ApplyForceSpecial(i, -LanternTail.Masses[i].Velocity * damping);
			LanternTail.ApplyForceSpecial(i, new Vector2(0, 13 * LanternTail.Masses[i].Value));
		}
		LanternGhostKingPBDSolver.Step(LanternGhostKingMassSpringSystem, 1f);
	}

	public void DrawTail(SpriteBatch spriteBatch)
	{
		if (LanternTail == null || LanternTail.Masses.Length <= 0)
		{
			return;
		}
		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone, null, Main.Transform);

		Texture2D tex = ModAsset.LanternGhostKing_Atlas.Value;
		float effectWidth = 100f;
		float alphaFade = (255 - NPC.alpha) / 255f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 1; i < LanternTail.Masses.Length; i++)
		{
			float value = i - 1;
			value /= LanternTail.Masses.Length - 2;
			Vector2 drawPos = LanternTail.Masses[i].Position - Main.screenPosition;
			Vector2 directionVec = LanternTail.Masses[i].Position - LanternTail.Masses[i - 1].Position;
			directionVec = directionVec.NormalizeSafe();
			Vector2 directionLeft = directionVec.RotatedBy(MathHelper.PiOver2) * effectWidth / 2f;
			bars.Add(drawPos + directionLeft, Lighting.GetColor(LanternTail.Masses[i].Position.ToTileCoordinates()) * alphaFade, new Vector3(812f / tex.Width, (100f + value * 202f) / tex.Height, 0));
			bars.Add(drawPos - directionLeft, Lighting.GetColor(LanternTail.Masses[i].Position.ToTileCoordinates()) * alphaFade, new Vector3(912f / tex.Width, (100f + value * 202f) / tex.Height, 0));
		}
		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
		Lighting.AddLight(LanternTail.Masses[1].Position, new Vector3(1.25f, 1.05f, 1f));
		spriteBatch.Draw(tex, LanternTail.Masses[1].Position - Main.screenPosition, CoreAndTailFrame, Lighting.GetColor(LanternTail.Masses[1].Position.ToTileCoordinates()) * alphaFade, (LanternTail.Masses[1].Position - LanternTail.Masses[0].Position).ToRotationSafe() - MathHelper.PiOver2, CoreAndTailFrame.Size() * 0.5f, 1f, SpriteEffects.None, 0);
	}

	public void DrawVertexTexture(Texture2D tex, Vector2 position, Rectangle frame, float rotation, Vector2 origin, bool flipH = false)
	{
		Vector2 vertex0 = new Vector2(frame.X, frame.Y);
		Vector2 vertex1 = new Vector2(frame.X + frame.Width, frame.Y);
		Vector2 vertex2 = new Vector2(frame.X, frame.Y + frame.Height);
		Vector2 vertex3 = new Vector2(frame.X + frame.Width, frame.Y + frame.Height);

		Vector3 coord0 = new Vector3(vertex0 / tex.Size(), EffectValueZ + GoldenShieldCrackResistInYAxis);
		Vector3 coord1 = new Vector3(vertex1 / tex.Size(), EffectValueZ + GoldenShieldCrackResistInYAxis);
		Vector3 coord2 = new Vector3(vertex2 / tex.Size(), EffectValueZ);
		Vector3 coord3 = new Vector3(vertex3 / tex.Size(), EffectValueZ);

		origin += new Vector2(frame.X, frame.Y);
		Vector2 origintToVertex0 = vertex0 - origin;
		Vector2 origintToVertex1 = vertex1 - origin;
		Vector2 origintToVertex2 = vertex2 - origin;
		Vector2 origintToVertex3 = vertex3 - origin;

		Vector2 v0 = position + origintToVertex0.RotatedBy(rotation) * NPC.scale;
		Vector2 v1 = position + origintToVertex1.RotatedBy(rotation) * NPC.scale;
		Vector2 v2 = position + origintToVertex2.RotatedBy(rotation) * NPC.scale;
		Vector2 v3 = position + origintToVertex3.RotatedBy(rotation) * NPC.scale;

		float alpha = (255 - NPC.alpha) / 255f;

		Color c0 = Lighting.GetColor(v0.ToTileCoordinates()) * alpha;
		Color c1 = Lighting.GetColor(v1.ToTileCoordinates()) * alpha;
		Color c2 = Lighting.GetColor(v2.ToTileCoordinates()) * alpha;
		Color c3 = Lighting.GetColor(v3.ToTileCoordinates()) * alpha;

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, coord0),
			new Vertex2D(v1, c1, coord1),

			new Vertex2D(v2, c2, coord2),
			new Vertex2D(v3, c3, coord3),
		};
		if (flipH)
		{
			bars = new List<Vertex2D>()
			{
				new Vertex2D(v0, c0, coord1),
				new Vertex2D(v1, c1, coord0),

				new Vertex2D(v2, c2, coord3),
				new Vertex2D(v3, c3, coord2),
			};
		}
		if (bars.Count > 0)
		{
			Main.graphics.GraphicsDevice.Textures[0] = tex;
			Main.graphics.GraphicsDevice.Textures[1] = Commons.ModAsset.Noise_Turtle_Crack_Inverse.Value;
			Main.graphics.GraphicsDevice.Textures[2] = ModAsset.LanternGhostKing_Atlas_GoldenShield.Value;
			Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);
		}
	}

	public Color GetColorRed(Color c0)
	{
		Vector4 v0 = c0.ToVector4();
		float value = (255 - NPC.alpha) / 255f;
		value = Math.Clamp(value, 0, 1);
		v0.X *= MathF.Sqrt(value);
		v0.Y *= MathF.Pow(value, 2);
		v0.Z *= MathF.Pow(value, 3.3f);
		return new Color(v0.X, v0.Y, v0.Z, v0.W);
	}

	public Vector2 RandomVector2(float maxLength, float minLength = 0)
	{
		if (maxLength <= minLength)
		{
			maxLength = minLength + 0.001f;
		}
		return new Vector2(Main.rand.NextFloat(minLength, maxLength), 0).RotatedByRandom(6.283);
	}
}
