using Everglow.Commons.Coroutines;
using Everglow.Commons.CustomTiles;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.Netcode;
using Everglow.Commons.Skeleton2D;
using Everglow.Commons.Skeleton2D.Reader;
using Everglow.Commons.Skeleton2D.Renderer;
using Everglow.Commons.Skeleton2D.Renderer.DrawCommands;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock;
using Everglow.Yggdrasil.YggdrasilTown.Items.BossDrops;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Bosses.SquamousShell;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;

[AutoloadBossHead]
[NoGameModeScale]
public class SquamousShell : ModNPC
{
	public override void SetDefaults()
	{
		NPC.aiStyle = -1;
		NPC.width = 140;
		NPC.height = 150;
		NPC.knockBackResist = 0;
		NPC.lavaImmune = true;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.boss = true;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.value = 32000;
		NPC.localAI[0] = 0;
		NPC.damage = 48;
		NPC.defense = 15;
		NPC.lifeMax = 3300;
		if (Main.expertMode)
		{
			NPC.damage = 65;
			NPC.defense = 21;
			NPC.lifeMax = 4650;
			NPC.value = 54000;
		}
		if (Main.masterMode)
		{
			NPC.damage = 71;
			NPC.defense = 24;
			NPC.lifeMax = 6130;
			NPC.value = 81000;
		}

		if (!Main.dedServ)
		{
			Mod everglow = ModLoader.GetMod("Everglow");
			if (everglow != null)
			{
				Music = MusicLoader.GetMusicSlot(everglow, ModAsset.SquamousShellBGM_Path);
			}
		}
		NPCID.Sets.MustAlwaysDraw[Type] = true;
	}

	public bool Flying = false;
	public bool Smashing = false;
	public bool Dashing = false;
	public int TrapedInTiles = 0;
	public float PhantomValue = 0f;
	public float SmashValue = 0f;
	public Vector2 SpawnPos = Vector2.zeroVector;
	public Skeleton2D SquamousShellSkeleton;
	public SkeletonRenderer skeletonRenderer = new SkeletonRenderer();
	public SkeletonDebugRenderer skeletonDebugRenderer = new SkeletonDebugRenderer();

	public override void Load()
	{
	}

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 1;
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		return 0f;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.velocity.Y = 2f;
		_coroutineManager.StartCoroutine(new Coroutine(Landing()));
		var data = Mod.GetFileBytes(ModAsset.monsterj_Path);
		if (!Main.dedServ)
		{
			if (SquamousShellSkeleton == null)
			{
				var json = Mod.GetFileBytes(ModAsset.monsterj_Path);
				var atlas = Mod.GetFileBytes(ModAsset.monstera_Path);
				SquamousShellSkeleton = Skeleton2DReader.ReadSkeleton(atlas, json, ModAsset.monster.Value);
				SquamousShellSkeleton.AnimationState.SetAnimation(0, "walk4", true);
			}
		}
	}

	public override bool CheckActive() => false;

	public override void AI()
	{
		if (!Main.dedServ)
		{
			SquamousShellSkeleton.AnimationState.Apply(SquamousShellSkeleton.Skeleton);
		}

		NPC.localAI[0] += 1;
		NPC.TargetClosest(false);
		Player player = Main.player[NPC.target];
		_coroutineManager.Update();
		SquamousShellSkeleton.Position = NPC.Bottom;
		SquamousShellSkeleton.Skeleton.FindBone("root").Rotation = NPC.rotation / Spine.MathUtils.DegRad;
		SquamousShellSkeleton.Skeleton.FindBone("root").ScaleY = NPC.spriteDirection;
		SquamousShellSkeleton.Skeleton.FindBone("root").ScaleX = -1;

		if (Flying)
		{
			NPC.frameCounter++;
			if (NPC.frameCounter > 3)
			{
				NPC.frameCounter = 0;
				if (NPC.frame.Y >= 900)
				{
					NPC.frame.Y = 0;
				}
				else
				{
					NPC.frame.Y += 300;
				}
				string origStr = ModAsset.SquamousShell_Buzz_0_Mod;
				if (origStr.Length > 0)
				{
					origStr = origStr.Substring(0, origStr.Length - 1);
				}
				int soundType = NPC.frame.Y / 300;
				string soundStr = origStr + soundType;
				SoundEngine.PlaySound(new SoundStyle(soundStr), NPC.Center);
			}
			Vector2 pos0 = new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldY);
			Vector2 addPos = new Vector2(-30 * NPC.spriteDirection, -120).RotatedBy(Main.rand.NextFloat(-2.4f, 0) * NPC.spriteDirection) * Main.rand.NextFloat(1);
			Dust d0 = Dust.NewDustDirect(pos0 + addPos, 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.velocity.Y = -addPos.Y * 0.1f;
			d0.velocity += NPC.velocity * 0.3f;
			d0.noGravity = true;
		}
	}

	public void DiswingEffect()
	{
		for (int i = 0; i < 75; i++)
		{
			Vector2 pos0 = new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldY);
			Vector2 addPos = new Vector2(-30 * NPC.spriteDirection, -120).RotatedBy(Main.rand.NextFloat(-2.4f, 0) * NPC.spriteDirection) * Main.rand.NextFloat(1);
			Dust d0 = Dust.NewDustDirect(pos0 + addPos, 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.scale = Main.rand.NextFloat(0.5f, 2f);
			d0.velocity.Y = -addPos.Y * 0.1f;
			d0.velocity += new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(1f)) * 35f).RotatedByRandom(MathHelper.TwoPi);
			d0.velocity += NPC.velocity * 0.3f;
			d0.noGravity = true;
		}
	}

	/// <summary>
	/// 落地
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> Landing()
	{
		while (NPC.velocity.Y != 0)
		{
			NPC.velocity.Y += 0.2f;
			NPC.spriteDirection = 1;
			yield return new SkipThisFrame();
		}

		// 这里需要一个震屏
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
		_coroutineManager.StartCoroutine(new Coroutine(Wake()));
	}

	/// <summary>
	/// 苏醒
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> Wake()
	{
		for (int x = 0; x <= 180; x++)
		{
			wakingTimer++;
			yield return new SkipThisFrame();
		}
		int direction = 1;
		if (Main.player[NPC.target].Center.X < NPC.Center.X)
		{
			direction = -1;
		}
		SpawnPos = NPC.Center;
		_coroutineManager.StartCoroutine(new Coroutine(Rush(direction)));
	}

	/// <summary>
	/// 冲撞
	/// </summary>
	/// <param name="direction"></param>
	/// <param name="acceleration"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> Rush(int direction, float acceleration = 0.2f)
	{
		Player target = Main.player[NPC.target];

		// if target no stand in solid top.
		if (!TileUtils.PlatformCollision(target.Bottom) && !TileUtils.PlatformCollision(target.Bottom + new Vector2(0, 8)) && !Collision.SolidCollision(target.position, target.width, target.height + 48))
		{
			_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
			yield break;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "walk4", true);
		int deathTimer = 0;
		float soundTimer = 0;
		int soundType = 0;
		if (Main.expertMode)
		{
			acceleration = 0.3f;
		}
		if (Main.masterMode)
		{
			acceleration = 0.6f;
		}

		// Stand on ground
		if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && !TileUtils.PlatformCollision(NPC.Bottom))
		{
			for (int t = 0; t < 300; t++)
			{
				NPC.position.Y += 10;
				if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || TileUtils.PlatformCollision(NPC.Bottom))
				{
					NPC.position.Y -= 10;
					NPC.velocity *= 0;
					break;
				}
			}
		}

		Projectile dashEffect = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, NPC.velocity, ModContent.ProjectileType<SquamousDashEffect>(), 0, 0, NPC.target, NPC.whoAmI);
		while (!NPC.collideX || NPC.velocity.Y != 0)
		{
			Dashing = true;
			deathTimer++;
			if (deathTimer > 60 && NPC.velocity.Length() < 1f)
			{
				break;
			}

			// Walk sound
			soundTimer += Math.Clamp(Math.Abs(NPC.velocity.X) / 60f, 0f, 1f);
			if (soundTimer > 1)
			{
				soundType++;
				if (soundType > 3)
				{
					soundType = 0;
				}
				string origStr = ModAsset.SquamousShell_Step_0_Mod;
				if (origStr.Length > 0)
				{
					origStr = origStr.Substring(0, origStr.Length - 1);
				}
				string soundStr = origStr + soundType;
				SoundEngine.PlaySound(new SoundStyle(soundStr), NPC.Bottom);
				soundTimer = 0;
			}

			// Different speed scale in different mode.
			float maxSpeed = 20f;
			if (Main.expertMode)
			{
				maxSpeed = 24f;
			}
			if (Main.masterMode)
			{
				maxSpeed = 36f;
			}

			// accleration
			if (Math.Abs(NPC.velocity.X) < maxSpeed)
			{
				if (Collision.SolidCollision(NPC.Center + new Vector2(50, 80), 0, 0) || TileUtils.PlatformCollision(NPC.Bottom))
				{
					NPC.velocity.X += direction * acceleration / 3f;
				}
				if (Collision.SolidCollision(NPC.Center + new Vector2(0, 80), 0, 0) || TileUtils.PlatformCollision(NPC.Bottom))
				{
					NPC.velocity.X += direction * acceleration / 3f;
				}
				if (Collision.SolidCollision(NPC.Center + new Vector2(-50, 80), 0, 0) || TileUtils.PlatformCollision(NPC.Bottom))
				{
					NPC.velocity.X += direction * acceleration / 3f;
				}
				NPC.velocity.X += direction * acceleration / 2f;
			}
			if (NPC.collideX)
			{
				NPC.velocity.Y -= 16;
			}
			if (!NPC.collideY)
			{
				NPC.velocity.Y += 0.6f;
			}

			// 动画表现
			NPC.spriteDirection = direction;
			SquamousShellSkeleton.AnimationState.Update(Math.Clamp(Math.Abs(NPC.velocity.X) / 300f, 0f, 0.08f));
			if ((Main.player[NPC.target].Center.X - NPC.Center.X < -1000 && direction == 1) || (Main.player[NPC.target].Center.X - NPC.Center.X > 1000 && direction == -1))
			{
				direction *= -1;
				NPC.velocity *= 0;
				NPC.position.X += direction * 20;
				foreach (Projectile projectile in Main.projectile)
				{
					if (projectile != null && projectile.active && projectile.type == ModContent.ProjectileType<SquamousDashEffect>())
					{
						if (projectile.ai[0] == NPC.whoAmI)
						{
							projectile.active = false;
						}
					}
				}
				break;
			}
			if ((Main.player[NPC.target].Center.X - NPC.Center.X < -800 && direction == 1) || (Main.player[NPC.target].Center.X - NPC.Center.X > 800 && direction == -1))
			{
				NPC.velocity *= 0.98f;
			}

			NPC.rotation = (float)GetVector2Rot(GetRotationVec());
			yield return new SkipThisFrame();
		}
		foreach (Projectile projectile in Main.projectile)
		{
			if (projectile != null && projectile.active && projectile.type == ModContent.ProjectileType<SquamousDashEffect>())
			{
				if (projectile.ai[0] == NPC.whoAmI)
				{
					projectile.active = false;
				}
			}
		}
		Dashing = false;

		// 这里需要一个震屏
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.6f), NPC.Center);
		yield return new WaitForFrames(30);
		int newDirection = 1;
		if (Main.player[NPC.target].Center.X < NPC.Center.X)
		{
			newDirection = -1;
		}
		if (newDirection == direction)
		{
			NPC.velocity.Y -= 16f;
			NPC.position.Y -= 40f;
		}
		NPC.position.X += newDirection * 20;
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	private IEnumerator<ICoroutineInstruction> RushHitPlayerFly(Player player, Vector2 velocityHit)
	{
		player.velocity += velocityHit;
		yield return new SkipThisFrame();
	}

	/// <summary>
	/// 坐地飞石雨
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> ShootSmallRocks(int direction)
	{
		Player player = Main.player[NPC.target];
		Vector2 vel = new Vector2((player.Center.X - NPC.Center.X) / 70f, -25);
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		for (int k = 0; k < 9; k++)
		{
			NPC.position += new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat(1f)) * 4).RotatedByRandom(6.283);
			Vector2 vel2 = vel.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
			Vector2 pos = NPC.Center + new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat(1f)) * 40).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f));
			SoundEngine.PlaySound(SoundID.Item152, NPC.Center);
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), pos, vel2, ModContent.ProjectileType<SquamousRockProj_small>(), (int)(NPC.damage * 0.35f), 3, NPC.target);
			yield return new SkipThisFrame();
		}
		for (int k = 0; k < 30; k++)
		{
			if (!Collision.SolidCollision(NPC.Bottom + new Vector2(0, 2), 2, 2) || !TileUtils.PlatformCollision(NPC.Bottom))
			{
				NPC.position.Y += 6;
			}
			else
			{
				break;
			}
		}
		for (int k = 0; k < 30; k++)
		{
			if (Collision.SolidCollision(NPC.Bottom + new Vector2(0, -2), 2, 2) || !TileUtils.PlatformCollision(NPC.Bottom))
			{
				NPC.position.Y -= 6;
			}
			else
			{
				break;
			}
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 飞石雨2
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> ShootSmallRocks2(int direction)
	{
		Player player = Main.player[NPC.target];
		Vector2 vel = new Vector2((player.Center.X - NPC.Center.X) / 70f, -25);
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		for (int k = 0; k < 3; k++)
		{
			NPC.position += new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat(1f)) * 4).RotatedByRandom(6.283);
			Vector2 vel2 = vel.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f));
			Vector2 pos = NPC.Center + new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat(1f)) * 40).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f));
			SoundEngine.PlaySound(SoundID.DD2_JavelinThrowersAttack, NPC.Center);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), pos, vel2, ModContent.ProjectileType<SquamousRockProj>(), (int)(NPC.damage * 0.54f), 3, 0);
			yield return new WaitForFrames(5);
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 滚石
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> RollingARock(int direction)
	{
		int waitCount = 0;
		int count = 0;

		// 确保位置正确
		while (Collision.SolidCollision(NPC.BottomLeft - new Vector2(0, 4), NPC.width, 8) || TileUtils.PlatformCollision(NPC.Bottom))
		{
			count++;
			NPC.position.Y -= 2f;
			if (count > 100)
			{
				break;
			}
		}
		while (NPC.velocity != NPC.oldVelocity)
		{
			waitCount++;
			if (waitCount > 60)
			{
				break;
			}
			NPC.velocity *= 0.95f;
			NPC.velocity.Y += 0.6f;
			yield return new SkipThisFrame();
		}

		// 动画设置
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "pushBall", true);
		NPC.velocity *= 0;
		int damage = 55;
		if (Main.expertMode)
		{
			damage = 80;
		}
		if (Main.masterMode)
		{
			damage = 133;
		}
		Projectile rock = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(direction * 4, 0), ModContent.ProjectileType<SquamousRollingStone>(), damage, 6, 0, direction);
		rock.scale = 0;
		rock.width = 0;
		rock.height = 0;
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		float maxScale = 1f;
		if (Main.expertMode)
		{
			maxScale = 1.2f;
		}
		if (Main.masterMode)
		{
			maxScale = 1.8f;
		}
		for (int x = 0; x < 61; x++)
		{
			float startTime = 30;
			if (x > startTime)
			{
				rock.scale += maxScale / (61f - startTime);
				rock.width = (int)(rock.scale * 100f);
				rock.height = (int)(rock.scale * 100f);
				rock.Center = NPC.Center + Vector2.Lerp(new Vector2(70 * direction, 60), new Vector2(140 * direction, 20), (x - startTime) / (61f - startTime));
			}
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			yield return new SkipThisFrame();
		}

		// 震击,球浮空
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(100 * direction, 70), Vector2.Zero, ModContent.ProjectileType<Squamous_HitTile>(), 0, 0, 0, 3, Main.rand.NextFloat(6.283f));
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), NPC.Center);
		for (int x = -12; x <= 12; x++)
		{
			Dust dust = Dust.NewDustDirect(rock.Center + new Vector2(x * 5, 20), 0, 0, ModContent.DustType<SquamousShellStone>(), Main.rand.NextFloat(-0.1f, 0.1f), 27 * MathF.Cos(x / 12f * MathHelper.PiOver2) * Main.rand.NextFloat(0.2f, 1.6f), 0, default, Main.rand.NextFloat(0.9f, 1.6f));
			dust.noGravity = false;
		}
		for (int x = 0; x < 5; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			rock.Center = NPC.Center + Vector2.Lerp(new Vector2(140 * direction, 20), new Vector2(170 * direction, -20), x / 5f);
			yield return new SkipThisFrame();
		}

		for (int x = 0; x < 20; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			if (x < 5)
			{
				rock.Center = NPC.Center + new Vector2(170 * direction, -20);
			}

			// 撞击,球被击飞
			if (x == 5)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(170 * direction, -20), Vector2.Zero, ModContent.ProjectileType<Squamous_HitTile>(), 0, 0, 0, 6, Main.rand.NextFloat(6.283f));
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), NPC.Center);
				for (int a = 0; a <= 30; a++)
				{
					Vector2 v0 = new Vector2(0, Main.rand.NextFloat(7f, 12f)).RotatedByRandom(MathHelper.TwoPi);
					Dust dust = Dust.NewDustDirect(rock.position, rock.width, rock.height, ModContent.DustType<SquamousShellStone>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.9f, 1.6f));
					dust.noGravity = false;
				}
				float speed = 8;
				if (Main.expertMode)
				{
					speed = 10;
				}
				if (Main.masterMode)
				{
					speed = 15;
				}
				rock.velocity = new Vector2(speed * direction, -3);
				ShakerManager.AddShaker(rock.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 10 * MathF.Pow(rock.scale / 1.2f, 2f), 20f, 120, 0.9f, 0.8f, 150);
			}
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 滚石打飞
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> RollingARockFly(int direction)
	{
		int waitCount = 0;

		// Stand on ground
		if (!Collision.SolidCollision(NPC.position, NPC.width, NPC.height) && !TileUtils.PlatformCollision(NPC.Bottom))
		{
			for (int t = 0; t < 300; t++)
			{
				NPC.position.Y += 10;
				if (Collision.SolidCollision(NPC.position, NPC.width, NPC.height) || TileUtils.PlatformCollision(NPC.Bottom))
				{
					NPC.position.Y -= 10;
					NPC.velocity *= 0;
					break;
				}
			}
		}
		while (NPC.velocity != NPC.oldVelocity)
		{
			waitCount++;
			if (waitCount > 60)
			{
				break;
			}
			NPC.velocity *= 0.95f;
			NPC.velocity.Y += 0.6f;
			yield return new SkipThisFrame();
		}

		// 动画设置
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "pushBall", true);
		NPC.velocity *= 0;
		int damage = 55;
		if (Main.expertMode)
		{
			damage = 80;
		}
		if (Main.masterMode)
		{
			damage = 133;
		}
		Projectile rock = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(direction * 4, -24), ModContent.ProjectileType<SquamousRollingStone_fly>(), damage, 6, 0, direction);
		rock.scale = 0;
		rock.width = 0;
		rock.height = 0;
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		float maxScale = 1f;
		if (Main.expertMode)
		{
			maxScale = 1.2f;
		}
		if (Main.masterMode)
		{
			maxScale = 1.8f;
		}
		for (int x = 0; x < 61; x++)
		{
			float startTime = 30;
			if (x > startTime)
			{
				rock.scale += maxScale / (61f - startTime);
				rock.width = (int)(rock.scale * 100f);
				rock.height = (int)(rock.scale * 100f);
				rock.Center = NPC.Center + Vector2.Lerp(new Vector2(70 * direction, 60), new Vector2(140 * direction, 20), (x - startTime) / (61f - startTime));
			}
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			yield return new SkipThisFrame();
		}

		// 震击,球浮空
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(100 * direction, 70), Vector2.Zero, ModContent.ProjectileType<Squamous_HitTile>(), 0, 0, 0, 3, Main.rand.NextFloat(6.283f));
		SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), NPC.Center);
		for (int x = -12; x <= 12; x++)
		{
			Dust dust = Dust.NewDustDirect(rock.Center + new Vector2(x * 5, 20), 0, 0, ModContent.DustType<SquamousShellStone>(), Main.rand.NextFloat(-0.1f, 0.1f), 27 * MathF.Cos(x / 12f * MathHelper.PiOver2) * Main.rand.NextFloat(0.2f, 1.6f), 0, default, Main.rand.NextFloat(0.9f, 1.6f));
			dust.noGravity = false;
		}
		for (int x = 0; x < 5; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			rock.Center = NPC.Center + Vector2.Lerp(new Vector2(140 * direction, 20), new Vector2(170 * direction, -20), x / 5f);
			yield return new SkipThisFrame();
		}

		for (int x = 0; x < 20; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1f / 30f);
			if (x < 5)
			{
				rock.Center = NPC.Center + new Vector2(170 * direction, -20);
			}

			// 撞击,球被击飞
			if (x == 5)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(170 * direction, -20), Vector2.Zero, ModContent.ProjectileType<Squamous_HitTile>(), 0, 0, 0, 6, Main.rand.NextFloat(6.283f));
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact.WithVolume(0.4f), NPC.Center);
				for (int a = 0; a <= 30; a++)
				{
					Vector2 v0 = new Vector2(0, Main.rand.NextFloat(7f, 12f)).RotatedByRandom(MathHelper.TwoPi);
					Dust dust = Dust.NewDustDirect(rock.position, rock.width, rock.height, ModContent.DustType<SquamousShellStone>(), v0.X, v0.Y, 0, default, Main.rand.NextFloat(0.9f, 1.6f));
					dust.noGravity = false;
				}
				float speed = 8;
				if (Main.expertMode)
				{
					speed = 10;
				}
				if (Main.masterMode)
				{
					speed = 15;
				}
				rock.velocity = new Vector2(speed * direction, -10) * Main.rand.NextFloat(0.65f, 1f);
				ShakerManager.AddShaker(rock.Center, Vector2.One.RotatedByRandom(MathHelper.Pi), 30 * MathF.Pow(rock.scale / 1.2f, 2f), 20f, 120, 0.9f, 0.8f, 150);
			}
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 撼地跳
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> JumpAndShake(int direction)
	{
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "takeOff", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		NPC.velocity = new Vector2(direction * 4, -7);
		Player player = Main.player[NPC.target];
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", false);
		Flying = true;
		float accleration = 0.2f;
		if (Main.expertMode)
		{
			accleration = 0.3f;
		}
		if (Main.masterMode)
		{
			accleration = 0.5f;
		}
		for (int x = 0; x < 120; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			if ((player.Center - NPC.Center).X * direction < 0)
			{
				NPC.velocity.Y += accleration * 2;
				if (x > 30)
				{
					break;
				}
			}
			if (NPC.Center.Y - player.Center.Y < -300)
			{
				if ((player.Center - NPC.Center).X * direction > 0)
				{
					if (Math.Abs(NPC.velocity.X) < 16)
					{
						NPC.velocity.X += direction * accleration;
					}
				}
				else
				{
					NPC.velocity.X *= 0;
					break;
				}
				if (!Flying)
				{
					Flying = true;
					NPC.frame = new Rectangle(0, 0, 244, 130);
				}
				if (NPC.velocity.Y < (player.Center.Y - NPC.Center.Y) / 60f)
				{
					NPC.velocity.Y += accleration * 5;
				}
			}
			else
			{
				NPC.velocity.Y -= accleration * 2;
			}
			yield return new SkipThisFrame();
		}

		// 短暂悬停
		DiswingEffect();
		Flying = false;
		NPC.velocity *= 0;
		NPC.velocity.Y = -2;
		PhantomValue = 1;
		int waitintTime = 20;
		if (Main.expertMode)
		{
			waitintTime = 16;
		}
		if (Main.masterMode)
		{
			waitintTime = 10;
		}
		for (int x = 0; x < waitintTime; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			PhantomValue = 1 - x / (float)waitintTime;
			yield return new SkipThisFrame();
		}
		PhantomValue = 0;
		for (int x = 0; x < 100; x++)
		{
			if (SmashValue < 1f)
			{
				SmashValue += 0.15f;
			}
			else
			{
				SmashValue = 1f;
			}
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			NPC.velocity.X = 0;
			NPC.velocity.Y = 40 + x;
			Smashing = true;
			NPC.noTileCollide = true;
			if (Collision.SolidCollision(NPC.BottomLeft - new Vector2(0, 4), NPC.width, 8))
			{
				int count = 0;
				while (Collision.SolidCollision(NPC.BottomLeft - new Vector2(0, 4), NPC.width, 8))
				{
					count++;
					NPC.position.Y -= 2f;
					if (count > 100)
					{
						break;
					}
				}
				SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, NPC.Center);
				GenerateSmogAtBottom(120);
				NPC.velocity *= 0;
				NPC.noTileCollide = false;
				break;
			}

			yield return new SkipThisFrame();
		}
		ShakerManager.AddShaker(NPC.Bottom, Vector2.One.RotatedByRandom(MathHelper.Pi), 60, 20f, 60, 0.9f, 0.8f, 150);
		for (int g = 0; g < 35; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(12f, 37f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y = -Math.Abs(newVelocity.Y);
			newVelocity.Y *= Main.rand.NextFloat(0.1f, 1.4f);
			newVelocity.X *= 0.01f;
			var somg = new RockSmog_Cone_FallingSandDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = NPC.Bottom + new Vector2(newVelocity.X * 600, 8),
				maxTime = Main.rand.NextFloat(90, Math.Max(newVelocity.Y * 6, 163)),
				scale = Main.rand.NextFloat(12f, 13f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(6f, 33f)).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y = -Math.Abs(newVelocity.Y);
			newVelocity.X *= 0.01f;
			var somg = new RockSmog_Cone_FallingSandDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = NPC.Bottom + new Vector2(newVelocity.X * 600, 8),
				maxTime = Main.rand.NextFloat(90, Math.Max(newVelocity.Y * 6, 163)),
				scale = Main.rand.NextFloat(6f, 7f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int x = 0; x < 20; x++)
		{
			Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(-4 + (x - 10) * 10, -4), 0, 0, ModContent.DustType<SquamousShellStone>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.7f, 1.7f));
			dust.velocity = new Vector2((x - 10) * 0.15f, -Main.rand.NextFloat(0f, 7f) * (MathF.Sin(x / 20f * MathF.PI) + 1.5f));
		}
		for (int x = 0; x < 20; x++)
		{
			Dust dust = Dust.NewDustDirect(NPC.Center + new Vector2(-4 + (x - 10) * 10, -4), 0, 0, ModContent.DustType<SquamousShellStone_dark>(), 0f, 0f, 0, default, Main.rand.NextFloat(0.5f, 1.3f));
			dust.velocity = new Vector2((x - 10) * 0.15f, -Main.rand.NextFloat(0f, 5f) * (MathF.Sin(x / 20f * MathF.PI) + 1.5f));
		}
		if (Main.expertMode)
		{
			for (int x = 0; x < 10; x++)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2((x - 4.5f) * 400f + Main.rand.NextFloat(-100, 100), -100), new Vector2(0, 1), ModContent.ProjectileType<SquamousRockSpike_Falling_Predict>(), 60, 3f, player.whoAmI);
			}
		}
		if (Main.masterMode)
		{
			for (int x = 0; x < 3; x++)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2((x - 1.5f) * 700f + Main.rand.NextFloat(-100, 100), -100), new Vector2(0, 1), ModContent.ProjectileType<SquamousRockSpike_Falling_Predict>(), 60, 3f, player.whoAmI);
			}
		}
		for (int x = 0; x <= 150; x++)
		{
			Dust d0 = Dust.NewDustDirect(NPC.Bottom + new Vector2((x - 75) * 4 - 4, -4), 0, 0, ModContent.DustType<SquamousShellWingDust>());
			d0.velocity.Y = -(MathF.Cos(((x - 75) / 75f) * MathHelper.Pi) + 1) * 16;
			d0.velocity.Y *= Main.rand.NextFloat(0.05f, 1.2f);
			d0.noGravity = false;
			d0.scale *= Math.Min(MathF.Cos(((x - 75) / 75f) * MathHelper.Pi) + 1, 1f);
		}
		Smashing = false;
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "drop", false);
		for (int i = 0; i < 10; i++)
		{
			if (SmashValue > 0f)
			{
				SmashValue -= 0.1f;
			}
			else
			{
				SmashValue = 0f;
			}
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		SmashValue = 0f;
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "idle", false);
		for (int i = 0; i < 60; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 飞天月刃Plus+乱舞
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> FlyingSlashAndMoonBlade(int direction)
	{
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "takeOff", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		NPC.velocity = new Vector2(direction * 4, -7);
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", false);
		Player player = Main.player[NPC.target];
		Flying = true;
		if (Main.rand.NextBool(2))
		{
			direction *= -1;
		}
		for (int x = 0; x < 100; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			if (NPC.Center.Y - player.Center.Y < -200)
			{
				Vector2 aimPos = player.Center + new Vector2(-400 * direction, -300);
				Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
				NPC.velocity = NPC.velocity * 0.8f + toTarget * 0.2f * 0.1f;
				if ((NPC.Center - aimPos).Length() < 150)
				{
					break;
				}
			}
			else
			{
				NPC.velocity *= 0.96f;
				NPC.velocity.Y -= 0.5f;
			}
			yield return new SkipThisFrame();
		}

		// 减速
		for (int x = 0; x < 12; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			NPC.velocity *= 0.8f;
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "slash", false);

		// 出刀
		var startRot = NPC.rotation;
		var startDir = NPC.spriteDirection;
		var attackPos = NPC.Center;
		NPC.hide = true;
		NPC.dontTakeDamage = true;
		for (int t = 0; t < 24; t++)
		{
			int damage = 60;
			if (Main.expertMode)
			{
				damage = 110;
			}
			if (Main.masterMode)
			{
				damage = 150;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Squamous_Slash>(), damage, 2, default, 2.5f);
			Vector2 shadeVel = new Vector2(0, Main.rand.NextFloat(4, 7)).RotateRandom(MathHelper.TwoPi);
			Projectile shade = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center - shadeVel * 60, shadeVel, ModContent.ProjectileType<SquamousShell_shade>(), 0, 2, default);
			shade.rotation = shadeVel.ToRotation();
			NPC.Center = attackPos + new Vector2(Main.rand.NextFloat(-140, 140), Main.rand.NextFloat(-50, 50));
			NPC.spriteDirection *= -1;
			NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			NPC.alpha = 150;
			yield return new WaitForFrames(2);
		}
		NPC.dontTakeDamage = false;
		NPC.hide = false;
		NPC.alpha = 0;
		NPC.rotation = startRot;
		NPC.spriteDirection = startDir;
		Vector2 playerPos = player.Center;
		if (playerPos.X < NPC.Center.X)
		{
			direction = -1;
		}
		else
		{
			direction = 1;
		}
		if (NPC.spriteDirection > direction)
		{
			NPC.rotation += MathF.PI;
			NPC.spriteDirection = direction;
		}
		if (NPC.spriteDirection < direction)
		{
			NPC.rotation -= MathF.PI;
			NPC.spriteDirection = direction;
		}
		for (int x = 0; x < 20; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);

			Vector2 aimPos = playerPos + new Vector2(-400 * direction, -300);
			Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
			Vector2 toPlayer = playerPos - NPC.Center;
			NPC.velocity = NPC.velocity * 0.995f + toTarget * 0.005f * 0.1f;
			if (x == 0)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(-0.05f) * 4f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI);
			}
			if (x == 1)
			{
				for (int i = 0; i < 4; i++)
				{
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(MathHelper.TwoPi * (i + 0.5f) / 4f + 0.05f) * 4f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(MathHelper.TwoPi * (i + 0.5f) / 4f - 0.05f) * 4f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI);
				}
			}
			if (x == 2)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(0.03f) * 4.1f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2f, player.whoAmI);
			}
			if (x == 16)
			{
				for (int t = 0; t < 8; t++)
				{
					int damage = 60;
					if (Main.expertMode)
					{
						damage = 110;
					}
					if (Main.masterMode)
					{
						damage = 150;
					}
					Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Squamous_Slash>(), damage, 2, default, 2.5f);
				}
			}
			if (x == 11)
			{
				if (direction == -1)
				{
					NPC.rotation = MathF.PI;
				}

				int projDir = 1;
				if (Main.rand.NextBool(2))
				{
					projDir = -1;
				}
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(-0.05f) * 4f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI, 10, projDir, 60);
			}
			if (x == 12)
			{
				for (int i = 0; i < 4; i++)
				{
					int projDir = 1;
					if (Main.rand.NextBool(2))
					{
						projDir = -1;
					}
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(MathHelper.TwoPi * (i + 0.5f) / 4f + 0.05f) * 4f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI, 10, projDir, 45);
					Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(MathHelper.TwoPi * (i + 0.5f) / 4f - 0.05f) * 4f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI, 10, projDir, 40);
				}
			}
			if (x == 13)
			{
				if (direction == -1)
				{
					NPC.rotation = MathF.PI;
				}
				int projDir = 1;
				if (Main.rand.NextBool(2))
				{
					projDir = -1;
				}
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(0.03f) * 4.1f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2f, player.whoAmI, 0, projDir, 60);
			}
			yield return new SkipThisFrame();
		}
		for (int x = 0; x < 10; x++)
		{
			NPC.velocity *= 0.74f;
		}

		// 落地
		NPC.rotation = 0;
		if (direction == -1)
		{
			NPC.rotation = MathF.PI;
		}
		DiswingEffect();
		Flying = false;
		for (int x = 0; x < 300; x++)
		{
			if (!NPC.collideY)
			{
				NPC.velocity.Y += 1f;
			}
			else
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "drop", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 飞天月刃
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> FlyingMoonBlade(int direction)
	{
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "takeOff", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		NPC.velocity = new Vector2(direction * 4, -7);
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", false);
		Player player = Main.player[NPC.target];
		Flying = true;
		if (Main.rand.NextBool(2))
		{
			direction *= -1;
		}
		for (int x = 0; x < 100; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			if (NPC.Center.Y - player.Center.Y < -200)
			{
				Vector2 aimPos = player.Center + new Vector2(-400 * direction, -300);
				Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
				NPC.velocity = NPC.velocity * 0.8f + toTarget * 0.2f * 0.1f;
				if ((NPC.Center - aimPos).Length() < 150)
				{
					break;
				}
			}
			else
			{
				NPC.velocity *= 0.96f;
				NPC.velocity.Y -= 0.5f;
			}
			yield return new SkipThisFrame();
		}

		// 减速
		for (int x = 0; x < 12; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			NPC.velocity *= 0.8f;
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "slash", false);

		// 出刀
		Vector2 playerPos = player.Center;
		Vector2 toPlayer = playerPos - NPC.Center;
		for (int x = 0; x < 30; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			if (x % 20 == 0)
			{
				playerPos = player.Center;
			}
			if (playerPos.X < NPC.Center.X)
			{
				direction = -1;
			}
			else
			{
				direction = 1;
			}
			if (NPC.spriteDirection > direction)
			{
				NPC.rotation += MathF.PI;
				NPC.spriteDirection = direction;
			}
			if (NPC.spriteDirection < direction)
			{
				NPC.rotation -= MathF.PI;
				NPC.spriteDirection = direction;
			}
			Vector2 aimPos = playerPos + new Vector2(-400 * direction, -300);
			Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
			NPC.velocity = NPC.velocity * 0.995f + toTarget * 0.005f * 0.1f;
			if (x == 0)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(-0.05f) * 4f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI);
			}
			if (x == 2)
			{
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(0.03f) * 4.1f, ModContent.ProjectileType<SquamousMoonBladePredict>(), (int)(NPC.damage * 1.2f), 2f, player.whoAmI);
			}
			if (x == 16)
			{
				for (int t = 0; t < 8; t++)
				{
					int damage = 60;
					if (Main.expertMode)
					{
						damage = 110;
					}
					if (Main.masterMode)
					{
						damage = 150;
					}
					Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Squamous_Slash>(), damage, 2, default, 2.5f);
				}
			}
			if (x == 11)
			{
				if (direction == -1)
				{
					NPC.rotation = MathF.PI;
				}

				int projDir = 1;
				if (Main.rand.NextBool(2))
				{
					projDir = -1;
				}
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(-0.05f) * 4f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2, player.whoAmI, 10, projDir, 22);
			}
			if (x == 13)
			{
				if (direction == -1)
				{
					NPC.rotation = MathF.PI;
				}
				int projDir = 1;
				if (Main.rand.NextBool(2))
				{
					projDir = -1;
				}
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, Vector2.Normalize(toPlayer).RotatedBy(0.03f) * 4.1f, ModContent.ProjectileType<YggdrasilMoonBlade>(), (int)(NPC.damage * 1.2f), 2f, player.whoAmI, 0, projDir, 22.5f);
			}
			yield return new SkipThisFrame();
		}
		for (int x = 0; x < 10; x++)
		{
			NPC.velocity *= 0.74f;
		}

		// 落地
		NPC.rotation = 0;
		if (direction == -1)
		{
			NPC.rotation = MathF.PI;
		}
		DiswingEffect();
		Flying = false;
		for (int x = 0; x < 300; x++)
		{
			if (!NPC.collideY)
			{
				NPC.velocity.Y += 1f;
			}
			else
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "drop", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 飞天风球
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> FlyingAirProjectiles()
	{
		Player player = Main.player[NPC.target];
		int direction;
		if (NPC.Center.X > player.Center.X)
		{
			direction = -1;
		}
		else
		{
			direction = 1;
		}
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "takeOff", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		NPC.velocity = new Vector2(direction * 4, -7);

		SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", false);
		Flying = true;
		for (int x = 0; x < 100; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			Vector2 aimPos = player.Center + new Vector2(-300 * direction, -300);
			Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
			NPC.velocity = NPC.velocity * 0.8f + toTarget * 0.2f * 0.1f;
			if ((NPC.Center - aimPos).Length() < 50)
			{
				break;
			}
			yield return new SkipThisFrame();
		}

		// 减速
		for (int x = 0; x < 12; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			NPC.velocity *= 0.8f;
			yield return new SkipThisFrame();
		}

		// 放球
		Vector2 playerPos = player.Center;
		for (int x = 0; x < 30; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center, new Vector2(0, -15 * Main.rand.NextFloat(0.75f, 1.25f)).RotatedBy(Main.rand.NextFloat(-1.5f, 1.5f)), ModContent.ProjectileType<SquamousAirProj>(), (int)(NPC.damage * 0.45f), 0, 0, player.whoAmI);
			if (Main.masterMode && x % 12 == 0)
			{
				float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
				for (int s = 0; s < 4; s++)
				{
					for (int t = -2; t < 2; t++)
					{
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<SquamousAirProj>(), (int)(NPC.damage * 0.45f), 0, 0, player.whoAmI);
						p0.velocity = new Vector2(-2, t).RotatedBy(s / 4f * MathHelper.TwoPi + randomRot) * 4;
					}
				}
			}
			if (x % 20 == 0)
			{
				playerPos = player.Center;
			}
			NPC.velocity = NPC.velocity * 0.9f + new Vector2(25 * direction, 0) * 0.1f;
			NPC.position += new Vector2(0, -MathF.Sqrt(Main.rand.NextFloat(1f)) * 4).RotatedByRandom(6.283);
			yield return new SkipThisFrame();
		}
		if (Main.expertMode)
		{
			float randomRot = Main.rand.NextFloat(MathHelper.TwoPi);
			int times = 1;
			if (Main.masterMode)
			{
				times = 3;
			}
			for (int t = 0; t < times; t++)
			{
				for (int s = 0; s < 4; s++)
				{
					for (int x = -2; x < 2; x++)
					{
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<SquamousAirProj>(), (int)(NPC.damage * 0.45f), 0, 0, player.whoAmI);
						p0.velocity = new Vector2(-2, x).RotatedBy(s / 4f * MathHelper.TwoPi + randomRot + t) * (t * 4 + 4);
					}
				}
			}
		}

		// 落地
		NPC.rotation = 0;
		if (direction == -1)
		{
			NPC.rotation = MathF.PI;
		}
		DiswingEffect();
		Flying = false;
		for (int x = 0; x < 300; x++)
		{
			if (!NPC.collideY)
			{
				NPC.velocity.Y += 1f;
			}
			else
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "drop", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 飞天岩牙
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> FlyingRockTusk()
	{
		Player player = Main.player[NPC.target];
		int direction;
		if (NPC.Center.X > player.Center.X)
		{
			direction = -1;
		}
		else
		{
			direction = 1;
		}
		NPC.spriteDirection = direction;
		NPC.rotation = 0;
		if (NPC.spriteDirection == -1)
		{
			NPC.rotation = MathF.PI;
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "takeOff", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		NPC.velocity = new Vector2(direction * 4, -7);
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "fly", true);
		Flying = true;
		for (int x = 0; x < 100; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			Vector2 aimPos = player.Center + new Vector2(-400 * direction, -300);
			Vector2 toTarget = aimPos - NPC.Center - NPC.velocity;
			NPC.velocity = NPC.velocity * 0.8f + toTarget * 0.2f * 0.1f;
			if ((NPC.Center - aimPos).Length() < 50)
			{
				break;
			}
			yield return new SkipThisFrame();
		}

		// 减速
		for (int x = 0; x < 12; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			NPC.velocity *= 0.8f;
			yield return new SkipThisFrame();
		}

		// 放锥体
		Vector2 playerPos = player.Center;
		for (int x = 0; x < 140; x++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 100f);
			playerPos = player.Center;
			NPC.velocity = NPC.velocity * 0.9f + new Vector2(5 * direction, 0) * 0.1f;
			int freq = 20;
			if (Main.expertMode)
			{
				freq = 14;
			}
			if (Main.masterMode)
			{
				freq = 8;
			}
			if (x % freq == 0)
			{
				Vector2 toPlayer = playerPos - NPC.Center;
				Projectile.NewProjectile(NPC.GetSource_FromAI(), NPC.Center + new Vector2(0, -50).RotatedBy(x / 7f * Math.PI), Vector2.Normalize(toPlayer), ModContent.ProjectileType<SquamousRockSpike>(), (int)(NPC.damage * 0.7f), 2, 0, player.whoAmI);
			}
			yield return new SkipThisFrame();
		}

		// 落地
		NPC.rotation = 0;
		if (direction == -1)
		{
			NPC.rotation = MathF.PI;
		}
		DiswingEffect();
		Flying = false;
		for (int x = 0; x < 300; x++)
		{
			if (!NPC.collideY)
			{
				NPC.velocity.Y += 1f;
			}
			else
			{
				break;
			}
			yield return new SkipThisFrame();
		}
		SquamousShellSkeleton.AnimationState.SetAnimation(0, "drop", false);
		for (int i = 0; i < 10; i++)
		{
			SquamousShellSkeleton.AnimationState.Update(1 / 30f);
			yield return new SkipThisFrame();
		}
		_coroutineManager.StartCoroutine(new Coroutine(NextAttack()));
	}

	/// <summary>
	/// 下一个技能
	/// </summary>
	/// <param name="direction"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> NextAttack()
	{
		if (SpawnPos != Vector2.zeroVector)
		{
			if (NPC.position.Y > SpawnPos.Y + 120)
			{
				NPC.Center = SpawnPos;
			}
			if (NPC.position.Y < SpawnPos.Y - 1200)
			{
				NPC.Center = SpawnPos;
			}
			if (Collision.SolidCollision(NPC.Center - new Vector2(8), 16, 16))
			{
				TrapedInTiles++;
			}
			else
			{
				TrapedInTiles = 0;
			}
			if (TrapedInTiles >= 2)
			{
				TrapedInTiles = 0;
				NPC.Center = SpawnPos;
				NPC.direction = 1;
				NPC.spriteDirection = 1;
				NPC.rotation = 0;
			}
		}
		Player target = Main.player[NPC.target];
		float randAttack = Main.rand.NextFloat(0, 20);
		if (!TileUtils.PlatformCollision(target.Bottom) && !TileUtils.PlatformCollision(target.Bottom + new Vector2(0, 8)) && !Collision.SolidCollision(target.position, target.width, target.height + 48))
		{
			if (MathF.Abs(target.Center.X - NPC.Center.X) > 700)
			{
				randAttack = 2;
			}
		}
		int newDirection = 1;
		if (target.Center.X < NPC.Center.X)
		{
			newDirection = -1;
		}
		if (randAttack <= 4)
		{
			if ((target.Center - NPC.Center).Length() > 300f)
			{
				_coroutineManager.StartCoroutine(new Coroutine(Rush(newDirection)));
			}
			else
			{
				_coroutineManager.StartCoroutine(new Coroutine(FlyingMoonBlade(newDirection)));
			}
		}
		if (randAttack > 4 && randAttack <= 5)
		{
			_coroutineManager.StartCoroutine(new Coroutine(RollingARock(newDirection)));
		}
		if (randAttack > 5 && randAttack <= 6)
		{
			_coroutineManager.StartCoroutine(new Coroutine(RollingARockFly(newDirection)));
		}
		if (randAttack > 6 && randAttack <= 8)
		{
			_coroutineManager.StartCoroutine(new Coroutine(JumpAndShake(newDirection)));
		}
		if (randAttack > 8 && randAttack <= 10)
		{
			_coroutineManager.StartCoroutine(new Coroutine(FlyingMoonBlade(newDirection)));
		}
		if (randAttack > 10 && randAttack <= 12)
		{
			if (Main.expertMode)
			{
				_coroutineManager.StartCoroutine(new Coroutine(ShootSmallRocks(newDirection)));
			}
			else
			{
				if ((target.Center - NPC.Center).Length() > 300f)
				{
					_coroutineManager.StartCoroutine(new Coroutine(Rush(newDirection)));
				}
				else
				{
					_coroutineManager.StartCoroutine(new Coroutine(FlyingMoonBlade(newDirection)));
				}
			}
		}
		if (randAttack > 12 && randAttack <= 14)
		{
			if (Main.expertMode)
			{
				_coroutineManager.StartCoroutine(new Coroutine(ShootSmallRocks2(newDirection)));
			}
			else
			{
				if ((target.Center - NPC.Center).Length() > 300f)
				{
					_coroutineManager.StartCoroutine(new Coroutine(Rush(newDirection)));
				}
				else
				{
					_coroutineManager.StartCoroutine(new Coroutine(FlyingMoonBlade(newDirection)));
				}
			}
		}
		if (randAttack > 14 && randAttack <= 16)
		{
			_coroutineManager.StartCoroutine(new Coroutine(FlyingAirProjectiles()));
		}
		if (randAttack > 16 && randAttack <= 18)
		{
			_coroutineManager.StartCoroutine(new Coroutine(FlyingRockTusk()));
		}

		// master mode only
		if (randAttack > 18 && randAttack <= 20)
		{
			if (Main.expertMode)
			{
				_coroutineManager.StartCoroutine(new Coroutine(FlyingSlashAndMoonBlade(newDirection)));
			}
			else
			{
				_coroutineManager.StartCoroutine(new Coroutine(FlyingMoonBlade(newDirection)));
			}
		}
		yield break;
	}

	public override void ModifyHitPlayer(Player target, ref Player.HurtModifiers modifiers)
	{
		if (Dashing)
		{
			modifiers.FinalDamage *= 1.5f;
			target.position += new Vector2(NPC.velocity.X * 0.7f, -6f) * 5;
			_coroutineManager.StartCoroutine(new Coroutine(RushHitPlayerFly(target, new Vector2(NPC.velocity.X * 0.7f, -6f) * 1.5f)));
		}
		if (Smashing)
		{
			modifiers.FinalDamage *= 2.1f;
		}
		base.ModifyHitPlayer(target, ref modifiers);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (PhantomValue > 0)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			SpriteEffects spriteEffects = SpriteEffects.FlipVertically;
			Texture2D phantom = ModAsset.SquamousShell_light.Value;
			if (NPC.spriteDirection == 1)
			{
				spriteEffects = SpriteEffects.None;
			}
			for (int i = 0; i < 6; i++)
			{
				float scale = MathF.Pow(PhantomValue + 1, 2);
				spriteBatch.Draw(phantom, NPC.Center - Main.screenPosition + new Vector2(0, 100 * PhantomValue).RotatedBy(i / 6f * MathHelper.TwoPi), null, new Color(0.1f, 0.6f, 0.8f, 0) * (0.2f / scale), NPC.rotation, phantom.Size() * 0.5f, scale, spriteEffects, 0);
			}
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}

		if (SquamousShellSkeleton == null)
		{
			var json = Mod.GetFileBytes(ModAsset.monsterj_Path);
			var atlas = Mod.GetFileBytes(ModAsset.monstera_Path);
			SquamousShellSkeleton = Skeleton2DReader.ReadSkeleton(atlas, json, ModAsset.monster.Value);
			SquamousShellSkeleton.AnimationState.SetAnimation(0, "walk4", true);
		}

		skeletonDebugRenderer.DisableAll();
		skeletonDebugRenderer.DrawBones = true;

		SquamousShellSkeleton.Skeleton.UpdateWorldTransform();

		skeletonRenderer.UseEnvironmentLight = true;
		skeletonRenderer.DrawOffset = -Main.screenPosition;
		if (NPC.hide)
		{
			return false;
		}
		var cmdList = skeletonRenderer.Draw(SquamousShellSkeleton);
		skeletonRenderer.UseEnvironmentLight = false;
		cmdList.AddRange(skeletonRenderer.DrawWithOtherTexture(SquamousShellSkeleton, ModAsset.monster_glow.Value));
		if (Skeleton2D.ShowSkeleton)
		{
			cmdList.AddRange(skeletonDebugRenderer.Draw(SquamousShellSkeleton));
		}
		NaiveExecuter executer = new NaiveExecuter();
		executer.Execute(cmdList, Main.graphics.graphicsDevice);
		if (SmashValue > 0)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			SpriteEffects spriteEffects = SpriteEffects.FlipVertically;
			Texture2D smashTex = ModAsset.SquamousShell_smashing.Value;
			Texture2D smashGlow = ModAsset.SquamousShell_smashing_glow.Value;
			Texture2D smashShade = ModAsset.SquamousShell_smashing_shadow.Value;
			var offset = new Vector2(0, -520);
			if (NPC.spriteDirection == 1)
			{
				offset = new Vector2(0, -10);
				spriteEffects = SpriteEffects.None;
			}
			Color light = Lighting.GetColor(NPC.Center.ToTileCoordinates());
			light.A = 0;
			var drawPos = NPC.Center - Main.screenPosition + offset;
			spriteBatch.Draw(smashShade, drawPos, null, Color.White * SmashValue, NPC.rotation, new Vector2(171, 524), NPC.scale, spriteEffects, 0);
			spriteBatch.Draw(smashTex, drawPos, null, light * SmashValue, NPC.rotation, new Vector2(171, 524), NPC.scale, spriteEffects, 0);
			spriteBatch.Draw(smashGlow, drawPos, null, new Color(1f, 1f, 1f, 0) * SmashValue, NPC.rotation, new Vector2(300, 524), NPC.scale, spriteEffects, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		if (Dashing)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			SpriteEffects spriteEffects = SpriteEffects.FlipVertically;
			Texture2D dashTex = ModAsset.SquamousShell_Dash.Value;
			Texture2D dashGlow = ModAsset.SquamousShell_Dash_glow.Value;
			Texture2D dashShade = ModAsset.SquamousShell_Dash_Shadow.Value;
			var offset = new Vector2(0, 0);
			if (NPC.spriteDirection == 1)
			{
				offset = new Vector2(0, 0);
				spriteEffects = SpriteEffects.None;
			}
			Color light = Lighting.GetColor(NPC.Center.ToTileCoordinates());
			light.A = 0;
			var drawPos = NPC.Center - Main.screenPosition + offset;
			float dashValue = MathF.Abs(NPC.velocity.X / 40f);
			dashValue = Math.Clamp(dashValue, 0, 1);
			spriteBatch.Draw(dashShade, drawPos, null, Color.White * dashValue, NPC.rotation, new Vector2(504, 101), NPC.scale, spriteEffects, 0);
			spriteBatch.Draw(dashTex, drawPos, null, light * dashValue, NPC.rotation, new Vector2(504, 101), NPC.scale, spriteEffects, 0);
			spriteBatch.Draw(dashGlow, drawPos, null, new Color(1f, 1f, 1f, 0) * dashValue, NPC.rotation, new Vector2(504, 101), NPC.scale, spriteEffects, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		if (TrapedInTiles >= 1)
		{
			SpriteBatchState sBS = Main.spriteBatch.GetState().Value;
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

			Texture2D spawnGlow = ModAsset.SquamousShell_spawnGlow.Value;
			var drawPos = SpawnPos - Main.screenPosition;
			float timeValue = MathF.Sin((float)Main.timeForVisualEffects * 0.09f) * 0.6f + 0.4f;
			spriteBatch.Draw(spawnGlow, drawPos, null, new Color(1f, 1f, 1f, 0) * timeValue, 0, spawnGlow.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
			Main.spriteBatch.End();
			Main.spriteBatch.Begin(sBS);
		}
		if (Flying)
		{
			Texture2D wing0 = ModAsset.SquamousShell_wing_front_0.Value;
			Texture2D wing1 = ModAsset.SquamousShell_wing_front_1.Value;
			Texture2D wing2 = ModAsset.SquamousShell_wing_front_2.Value;
			Texture2D wing3 = ModAsset.SquamousShell_wing_front_3.Value;
			SpriteEffects spriteEffects = SpriteEffects.FlipHorizontally;
			Vector2 offset = new Vector2(0);
			Vector2 orig0 = new Vector2(wing0.Width - 5, 170);
			Vector2 orig1 = new Vector2(wing1.Width - 0, 170);
			Vector2 orig2 = new Vector2(wing2.Width - 8, 203);
			Vector2 orig3 = new Vector2(wing3.Width - 13, 231);
			float addRotation = 0;
			if (NPC.spriteDirection == -1)
			{
				orig0 = new Vector2(5, 170);
				orig1 = new Vector2(0, 170);
				orig2 = new Vector2(8, 203);
				orig3 = new Vector2(13, 231);
				spriteEffects = SpriteEffects.None;
				addRotation = MathHelper.Pi;
			}
			spriteBatch.Draw(
				wing0,
				new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor1").WorldY) - Main.screenPosition + offset,
				new Rectangle(0, NPC.frame.Y, wing0.Width, 300), drawColor, NPC.rotation * NPC.spriteDirection + addRotation, orig0, 1, spriteEffects, 0);
			spriteBatch.Draw(
				wing1,
				new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor2").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor2").WorldY) - Main.screenPosition + offset,
				new Rectangle(0, NPC.frame.Y, wing1.Width, 300), drawColor, NPC.rotation * NPC.spriteDirection + addRotation, orig1, 1, spriteEffects, 0);
			spriteBatch.Draw(
				wing2,
				new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor3").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor3").WorldY) - Main.screenPosition + offset,
				new Rectangle(0, NPC.frame.Y, wing2.Width, 300), drawColor, NPC.rotation * NPC.spriteDirection + addRotation, orig2, 1, spriteEffects, 0);
			spriteBatch.Draw(
				wing3,
				new Vector2(SquamousShellSkeleton.Skeleton.FindBone("Armor4").WorldX, SquamousShellSkeleton.Skeleton.FindBone("Armor4").WorldY) - Main.screenPosition + offset,
				new Rectangle(0, NPC.frame.Y, wing3.Width, 300), drawColor, NPC.rotation * NPC.spriteDirection + addRotation, orig3, 1, spriteEffects, 0);
		}
		return false;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
	}

	public override void OnKill()
	{
		if (YggdrasilWorld.InYggdrasil && !NetUtils.IsClient && !YggdrasilWorldSystem.DownedSquamousShell)
		{
			if (NetUtils.IsSingle)
			{
				YggdrasilWorldSystem.DownedSquamousShell = true;
			}
			else
			{
				ModIns.PacketResolver.Route(new YggdrasilProgressSyncPacket(true), RouteDestination.MainServer);
			}
		}
		// 全局击败进度由主服同步；专用服务器不执行视觉效果。
		if (Main.dedServ)
		{
			return;
		}

		for (int i = 0; i < 14; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/SquamousShell_gore" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), v0, type, NPC.scale);
		}
		for (int i = 5; i < 14; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/SquamousShell_gore" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), v0, type, NPC.scale);
		}
		for (int i = 5; i < 14; i++)
		{
			Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
			int type = ModContent.Find<ModGore>("Everglow/SquamousShell_gore" + i).Type;
			Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), v0, type, NPC.scale);
		}
		for (int g = 0; g < 20; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 4f)).RotatedByRandom(MathHelper.TwoPi);
			var somg = new RockSmogDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = NPC.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(37, 45),
				scale = Main.rand.NextFloat(40f, 55f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
		for (int g = 0; g < 400; g++)
		{
			Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(1.0f, 34f)).RotatedByRandom(MathHelper.TwoPi);
			var spark = new Spark_MoonBladeDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = NPC.Center + new Vector2(Main.rand.NextFloat(-6f, 6f), 0).RotatedByRandom(6.283),
				maxTime = Main.rand.Next(70, 125),
				scale = Main.rand.NextFloat(0.1f, Main.rand.NextFloat(9f, 47.0f)),
				rotation = Main.rand.NextFloat(6.283f),
				noGravity = true,
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.03f, 0.03f) },
			};
			Ins.VFXManager.Add(spark);
		}
	}

	//--------------------------以下为辅助功能模块-------------------------------------------------以下为辅助功能模块-------------------------------------------------以下为辅助功能模块-------------------------------------------------以下为辅助功能模块-------------------------------------------------以下为辅助功能模块-------------------------------------------------
	public void GenerateSmogAtBottom(int Frequency)
	{
		for (int g = 0; g < Frequency / 2 + 1; g++)
		{
			Vector2 newVelocity = new Vector2(0, MathF.Sqrt(Main.rand.NextFloat(0f, 1f)) * 12).RotatedByRandom(MathHelper.TwoPi);
			newVelocity.Y *= 0.05f;
			newVelocity.Y -= 2f;
			var somg = new RockSmogDust
			{
				velocity = newVelocity,
				Active = true,
				Visible = true,
				position = NPC.Bottom + new Vector2(Main.rand.NextFloat(-6f, 6f), -20).RotatedByRandom(6.283) + new Vector2(newVelocity.X * 10, 0),
				maxTime = Main.rand.Next(47, 145),
				scale = Main.rand.NextFloat(20f, 75f),
				rotation = Main.rand.NextFloat(6.283f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0 },
			};
			Ins.VFXManager.Add(somg);
		}
	}

	public double GetVector2Rot(Vector2 value)
	{
		if (value == Vector2.zeroVector)
		{
			double value1 = 0;
			if (NPC.spriteDirection == -1)
			{
				value1 = Math.PI;
			}
			return value1;
		}
		return Math.Atan2(value.Y, value.X) * NPC.spriteDirection + Math.PI / 2d;
	}

	public double CheckRotDir(double OldRot)
	{
		return -OldRot;
	}

	public void Exchange(ref float value1, ref float value2)
	{
		(value1, value2) = (value2, value1);
	}

	public Vector2 GetRotationVec()
	{
		Vector2 outValue = Vector2.Zero;
		for (int rad = 16; rad < 256; rad += 32)
		{
			for (int rot = 0; rot < rad / 4; rot++)
			{
				float value = rot / (float)rad * 4f;
				if (!Collision.SolidCollision(NPC.Center + new Vector2(0, rad).RotatedBy(value * MathHelper.TwoPi), 0, 0))
				{
					outValue += Vector2.Normalize(new Vector2(0, rad).RotatedBy(value * MathHelper.TwoPi)) / rad;
				}
				else
				{
					outValue -= Vector2.Normalize(new Vector2(0, rad).RotatedBy(value * MathHelper.TwoPi)) / rad;
				}
			}
		}
		if (outValue.Length() < 0.005)
		{
			outValue = Vector2.Zero;
		}
		return Utils.SafeNormalize(outValue, Vector2.Zero);
	}

	public void CheckSpriteDir()
	{
		if (NPC.velocity.X > 0)
		{
			NPC.spriteDirection = -1;
		}

		if (NPC.velocity.X < 0)
		{
			NPC.spriteDirection = 1;
		}
	}

	public void MoveTo(Vector2 aimPosition, float Speed = 1)
	{
		Vector2 v0 = Utils.SafeNormalize(aimPosition - NPC.Center, Vector2.Zero);
		NPC.velocity.X = v0.X * Speed;
	}

	public void DashTo(Vector2 aim, float Speed)
	{
		NPC.noTileCollide = true;
		Vector2 v0 = Utils.SafeNormalize(aim - NPC.Center, Vector2.Zero);
		NPC.velocity = v0 * Speed;
		NPC.noTileCollide = false;
	}

	public CoroutineManager _coroutineManager = new CoroutineManager();
	public int wakingTimer = 0;

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<SquamousShellTreasureBag>()));
		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<Items.BossDrops.SquamousShellRelic>()));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<Items.BossDrops.SquamousShellRelic>(), 10));

		var rule = new LeadingConditionRule(new Conditions.NotExpert());
		rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<DragonScaleHammer>(), ModContent.ItemType<EyeOfAnabiosis>(), ModContent.ItemType<FlurryingBlades>(), ModContent.ItemType<RockSpikeBallista>(), ModContent.ItemType<DeadBeetleEgg>(), ModContent.ItemType<BlueyWings>()));
		rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<RockGreaves>(), ModContent.ItemType<RockHelmet>(), ModContent.ItemType<RockPlateMail>()));
		npcLoot.Add(rule);
		base.ModifyNPCLoot(npcLoot);
	}
}
