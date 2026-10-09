using Everglow.Commons.Coroutines;
using Everglow.Yggdrasil.YggdrasilTown.Projectiles.TownNPCs.Fevens;
using Everglow.Yggdrasil.YggdrasilTown.VFXs;
using SubworldLibrary;
using Terraria.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

[AutoloadHead]
public class Fevens : TownNPC_LiveInYggdrasil
{
	/// <summary>
	/// 0 default; 1 hold magic array; 2 attack1; 3 attack2; 4 stand; 5 attack3; 6 attack4(Raise blade);7 attack5(Raise blade, no wing);8 defense; 9(Raise blade 2)
	/// </summary>
	public int State;

	public int Phase;

	public override string Texture => ModAsset.Fevens_Walk_Mod;

	public override string HeadTexture => ModAsset.Fevens_Head_Boss_Mod;

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 10;
	}

	public override void SetDefaults()
	{
		base.SetDefaults();
		StandFrame = new Rectangle(0, 0, 48, 70);
		SitFrame = new Rectangle(0, 630, 48, 70);
		NPC.frame = StandFrame;
		FrameHeight = 70;
	}

	public override void OnSpawn(IEntitySource source) => base.OnSpawn(source);

	public override void TryAttack()
	{
		float minDis = 100;
		float minDis2 = 400;
		if (ThreatenTarget >= 0 && ThreatenTarget < 200)
		{
			NPC npc = Main.npc[ThreatenTarget];
			Vector2 distance = npc.Center - NPC.Center;
			for (int i = 0; i < 8; i++)
			{
				Vector2 checkPos = npc.Center;
				switch (i)
				{
					case 0:
						checkPos = npc.TopLeft;
						break;
					case 1:
						checkPos = npc.Top;
						break;
					case 2:
						checkPos = npc.TopRight;
						break;
					case 3:
						checkPos = npc.Left;
						break;
					case 4:
						checkPos = npc.Right;
						break;
					case 5:
						checkPos = npc.BottomLeft;
						break;
					case 6:
						checkPos = npc.Bottom;
						break;
					case 7:
						checkPos = npc.BottomRight;
						break;
				}
				if ((checkPos - NPC.Center).Length() < distance.Length())
				{
					distance = checkPos - NPC.Center;
				}
			}
			if (Main.rand.NextFloat(TotalThreaten) > 0.3f)
			{
				if (distance.Length() < minDis2)
				{
					BehaviorsCoroutines.Enqueue(new Coroutine(Attack2()));
					return;
				}
			}
			if (distance.Length() < minDis)
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack()));
				return;
			}
			else if (distance.Length() < minDis2)
			{
				BehaviorsCoroutines.Enqueue(new Coroutine(Attack2()));
				return;
			}
		}
	}

	public IEnumerator<ICoroutineInstruction> Attack()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;

		// Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(-NPC.direction, 0), ModContent.ProjectileType<Georg_Hammer>(), 150, 4, Main.myPlayer, NPC.whoAmI);
		for (int t = 0; t < 60; t++)
		{
			NPC.velocity.X *= 0;

			// Attacking0 = true;
			yield return new SkipThisFrame();
		}

		// Attacking0 = false;
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> Attack2()
	{
		bool safe = false;
		if (ThreatenTarget is >= 0 and < 200)
		{
			safe = true;
		}
		if (!safe)
		{
			EndAIPiece();
			yield break;
		}
		NPC target = Main.npc[ThreatenTarget];
		if (target.Center.X > NPC.Center.X)
		{
			NPC.direction = 1;
		}
		if (target.Center.X < NPC.Center.X)
		{
			NPC.direction = -1;
		}
		NPC.spriteDirection = NPC.direction;

		// Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, (target.Center - NPC.Center).NormalizeSafe(), ModContent.ProjectileType<Georg_Hook>(), 100, 4, Main.myPlayer, NPC.whoAmI);
		for (int t = 0; t < 100; t++)
		{
			NPC.velocity.X *= 0;

			// Attacking0 = true;
			yield return new SkipThisFrame();
		}

		// Attacking0 = false;
		EndAIPiece();
	}

	public override void WalkFrame()
	{
		NPC.frame.Width = 48;
		NPC.frame.Height = FrameHeight;
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 8 * FrameHeight)
		{
			NPC.frame.Y = FrameHeight;
		}
	}

	public override void CheckInSuitableArea()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return;
		}
		AnchorForBehaviorPos = YggdrasilTownCentralSystem.TownTopLeftWorldCoord.ToTileCoordinates() + new Point(116, 158);
		bool safe = false;
		var homePoint = AnchorForBehaviorPos;
		NPC.homeless = false;
		NPC.homeTileX = homePoint.X;
		NPC.homeTileY = homePoint.Y;

		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X > AnchorForBehaviorPos.X - 100 && npcTilePos.X < AnchorForBehaviorPos.X + 100)
		{
			if (npcTilePos.Y > AnchorForBehaviorPos.Y - 20 && npcTilePos.Y < AnchorForBehaviorPos.Y + 20)
			{
				safe = true;
			}
		}
		if (!safe)
		{
			TeleportHome();
		}
	}

	public override void AI()
	{
		base.AI();
	}

	public override IEnumerator<ICoroutineInstruction> BehaviorControl() => base.BehaviorControl();

	public override void BossAI()
	{
		base.BossAI();

		// NPC.TargetClosest();
		// Player player = Main.player[NPC.target];
		// _fevensCoroutine.Update();
		// if (!player.active || player.dead)
		// {
		// NPC.active = false;
		// }
		// NPC.damage = 40;
		_fevensCoroutine.Update();
		if (Fail)
		{
			NPC.velocity *= 0;
			NPC.dontTakeDamage = true;
			NPC.damage = 0;
			return;
		}
		if (KnockOut)
		{
			return;
		}
		bool safe = false;
		if (NPC.target >= 0)
		{
			safe = true;
		}
		if (!safe)
		{
			return;
		}
		Player player = Main.player[NPC.target];
		if (BehaviorsCoroutines.Count <= 0)
		{
			Idle = true;

			ChooseAttack();
		}
	}

	public override void SetDefaultsToArena()
	{
		base.SetDefaultsToArena();
		NPC.width = 26;
		NPC.height = 36;
		NPC.aiStyle = NPCAIStyleID.Passive;
		NPC.damage = 80;
		NPC.defense = 100;
		NPC.lifeMax = 250000;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath6;
		NPC.knockBackResist = 0.5f;
		NPC.boss = true;
		NPC.knockBackResist = 0;
		NPC.noTileCollide = true;
		Phase = 0;
		NPC.friendly = false;
		NPC.aiStyle = -1;

		NPC.lifeMax = 88000;
		NPC.life = 88000;
		if (Main.expertMode)
		{
			NPC.lifeMax = 112000;
			NPC.life = 112000;
		}
		if (Main.masterMode)
		{
			NPC.lifeMax = 144000;
			NPC.life = 144000;
		}
		NPC.boss = true;
		NPC.localAI[0] = 0;
		NPC.aiStyle = -1;
		NPC.width = 40;
		NPC.height = 56;
		StartToBeABoss();
	}

	public override void ApplyBossTags()
	{
		base.ApplyBossTags();
	}

	private CoroutineManager _fevensCoroutine = new CoroutineManager();

	public void StartToBeABoss()
	{
		NPC.TargetClosest(true);

		// Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<fevensSword_following>(), 0, 0f, -1, NPC.whoAmI);
	}

	public override void StartFighting()
	{
		base.StartFighting();
		_fevensCoroutine.StartCoroutine(new Coroutine(StartFighting2()));
	}

	private IEnumerator<ICoroutineInstruction> StartFighting2()
	{
		Player player = Main.player[NPC.target];
		int dirChoose = 1;
		if (Main.rand.NextBool())
		{
			dirChoose = -1;
		}
		NPC.direction = dirChoose;
		NPC.spriteDirection = NPC.direction;
		NPC.noGravity = true;
		State = 1;
		Phase = 1;
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_MagicArray>(), 0, 0f, -1, NPC.whoAmI);
		for (int t = 0; t < 60; t++)
		{
			Vector2 targetPos = player.Center + new Vector2(200 * NPC.direction, -200);
			NPC.position = targetPos * 0.05f + NPC.position * 0.95f;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 4; i++)
		{
			Vector2 vel = new Vector2(0, 4).RotatedByRandom(MathHelper.TwoPi);
			Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<FevensChasingProj>(), 0, 1f, -1, 60);
			FevensChasingProj fCP = proj.ModProjectile as FevensChasingProj;
			if (fCP != null)
			{
				fCP.TargetPosition = player.Center;
			}
		}
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_AttackTarget>(), 0, 0f, -1, NPC.whoAmI);
		_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack0()));
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack0()
	{
		Player player = Main.player[NPC.target];
		GoToMagicAttackPosition();
		for (int t = 0; t < 60; t++)
		{
			var oldPos = NPC.Center;
			Vector2 targetPos = player.Center + new Vector2(600 * NPC.direction, -300);
			NPC.position = targetPos * 0.05f + NPC.position * 0.95f;
			GenerateVFXBatShortTime((int)(oldPos - NPC.Center).Length() / 4);
			yield return new SkipThisFrame();
		}

		for (int j = 0; j < 6; j++)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 vel = new Vector2(0, 9).RotatedByRandom(MathHelper.TwoPi);
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<FevensChasingProj>(), (int)(NPC.damage * 1.1f), 1f, -1, 60);
				FevensChasingProj fCP = proj.ModProjectile as FevensChasingProj;
				if (fCP != null)
				{
					fCP.TargetPosition = player.Center;
				}
			}
			Projectile targetP = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_AttackTarget>(), 0, 0f, -1, NPC.whoAmI);
			targetP.spriteDirection = player.direction * -1;
			State = 2;
			for (int t = 0; t < 12; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
			State = 1;
			for (int t = 0; t < 8; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
		}
		State = 3;
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = new Vector2(0, 9).RotatedBy(MathHelper.TwoPi / 12f * i);
			Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<Fevens_AttackProj0>(), (int)(NPC.damage * 1.1f), 1f, -1, 60);
		}
		State = 1;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack1()
	{
		Player player = Main.player[NPC.target];
		for (int i = 0; i < 3; i++)
		{
			NPC.velocity *= 0;
			NPC.Center = player.Center + new Vector2(0, Main.rand.Next(-220, -90)).RotatedBy(Main.rand.NextFloat(-1.1f, 1.1f));
			if (NPC.Center.X > player.Center.X)
			{
				NPC.direction = 1;
			}
			else
			{
				NPC.direction = -1;
			}
			NPC.spriteDirection = NPC.direction;

			for (int v = 0; v < 25; v++)
			{
				Vector2 startPos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
				var dustVFX = new Fevens_Bat_Small
				{
					Velocity = new Vector2(0, 3).RotatedByRandom(Math.PI * 2),
					Active = true,
					Visible = true,
					Position = startPos,
					MaxTime = Main.rand.Next(70, 120),
					Scale = Main.rand.Next(20, 40),
					ai = new float[] { Main.rand.NextFloat(1f, 8f), NPC.Center.X, NPC.Center.Y },
				};
				Ins.VFXManager.Add(dustVFX);
			}
			int lightingCount = 8;
			float offsetX = Main.rand.NextFloat(-100, 100);
			for (int j = 0; j < lightingCount; j++)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center + new Vector2((j - lightingCount / 2 + 0.5f) * 360 + offsetX + Main.rand.NextFloat(-10, 10), Main.rand.NextFloat(-30, 30) + 200), Vector2.zeroVector, ModContent.ProjectileType<Fevens_ThunderMark>(), (int)(NPC.damage * 1.1f), 1f, -1, 0);
			}
			for (int t = 0; t < 60; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack2()
	{
		Player player = Main.player[NPC.target];
		GoToMagicAttackPosition();
		for (int t = 0; t < 60; t++)
		{
			var oldPos = NPC.Center;
			Vector2 targetPos = player.Center + new Vector2(600 * NPC.direction, -300);
			NPC.position = targetPos * 0.05f + NPC.position * 0.95f;
			GenerateVFXBatShortTime((int)(oldPos - NPC.Center).Length() / 4);
			yield return new SkipThisFrame();
		}

		for (int j = 0; j < 3; j++)
		{
			Vector2 vel = new Vector2(0, 9).RotatedByRandom(MathHelper.TwoPi);
			for (int i = 0; i < 5; i++)
			{
				for (int k = 0; k < 3; k++)
				{
					Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel.RotatedBy(k * 0.15f + MathHelper.TwoPi * i / 5f), ModContent.ProjectileType<FevensChasingProj1>(), (int)(NPC.damage * 1.1f), 1f, -1, 60);
					FevensChasingProj1 fCP = proj.ModProjectile as FevensChasingProj1;
					if (fCP != null)
					{
						fCP.TargetPosition = player.Center;
					}
				}
			}
			Projectile targetP = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_AttackTarget>(), 0, 0f, -1, NPC.whoAmI);
			targetP.spriteDirection = player.direction * -1;
			targetP.timeLeft += 120;
			for (int t = 0; t < 12; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
			State = 1;
			for (int t = 0; t < 8; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
		}
		State = 3;
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel = new Vector2(0, 9).RotatedBy(MathHelper.TwoPi / 12f * i);
			Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel, ModContent.ProjectileType<Fevens_AttackProj0>(), (int)(NPC.damage * 1.1f), 1f, -1, 60);
		}
		State = 1;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack3()
	{
		Player player = Main.player[NPC.target];
		GoToMagicAttackPosition();
		for (int t = 0; t < 60; t++)
		{
			var oldPos = NPC.Center;
			Vector2 targetPos = player.Center + new Vector2(600 * NPC.direction, -300);
			NPC.position = targetPos * 0.05f + NPC.position * 0.95f;
			GenerateVFXBatShortTime((int)(oldPos - NPC.Center).Length() / 4);
			yield return new SkipThisFrame();
		}

		Projectile targetP = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_AttackTarget>(), 0, 0f, -1, NPC.whoAmI);
		targetP.spriteDirection = player.direction * -1;
		targetP.timeLeft += 120;
		var posTarget = targetP.Center;
		Vector2 vel = Vector2.Normalize(targetP.Center - NPC.Center) * 8;
		for (int j = 0; j < 60; j++)
		{
			for (int i = 0; i < 2; i++)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel.RotatedBy(j * Math.Sign(i - 0.5f) * 0.1f), ModContent.ProjectileType<FevensChasingProj1>(), (int)(NPC.damage * 1.1f), 1f, -1, 36);
				FevensChasingProj1 fCP = proj.ModProjectile as FevensChasingProj1;
				if (fCP != null)
				{
					fCP.TargetPosition = posTarget;
				}
			}
			yield return new WaitForFrames(1);
		}
		State = 3;
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 12; i++)
		{
			Vector2 vel2 = new Vector2(0, 9).RotatedBy(MathHelper.TwoPi / 12f * i);
			Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel2, ModContent.ProjectileType<Fevens_AttackProj0>(), (int)(NPC.damage * 1.1f), 1f, -1, 60);
		}
		State = 1;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack4()
	{
		Player player = Main.player[NPC.target];
		Vector2 startPlayerPos = player.Center - new Vector2(0, 380);
		Vector2 startPos = new Vector2(0, 180).RotatedByRandom(MathHelper.TwoPi);
		for (int i = 0; i < 6; i++)
		{
			NPC.velocity *= 0;
			Vector2 addPos = startPos.RotatedBy(i * MathHelper.TwoPi / 6f);
			NPC.Center = startPlayerPos + addPos;
			if (NPC.Center.X > player.Center.X)
			{
				NPC.direction = 1;
			}
			else
			{
				NPC.direction = -1;
			}
			NPC.spriteDirection = NPC.direction;

			GenerateVFXBat(25);
			int projCount = 10;
			Vector2 vel = Vector2.Normalize(addPos);
			for (int j = 0; j < projCount; j++)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, vel.RotatedBy((j - 5f) * 0.1f) * 6, ModContent.ProjectileType<FevensChasingProj1>(), (int)(NPC.damage * 1.1f), 1f, -1, 120);
				FevensChasingProj1 fCP = proj.ModProjectile as FevensChasingProj1;
				if (fCP != null)
				{
					fCP.TargetPosition = player.Center;
				}

				Projectile targetP = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_AttackTarget>(), 0, 0f, -1, NPC.whoAmI);
				targetP.spriteDirection = player.direction * -1;
				targetP.timeLeft += 120;
			}
			for (int t = 0; t < 30; t++)
			{
				NPC.velocity *= 0;
				yield return new SkipThisFrame();
			}
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> SwitchToPhase2()
	{
		NPC.velocity *= 0;
		NPC.velocity.Y = 20;
		for (int i = 0; i < 1000; i++)
		{
			if (NPC.velocity.Y < 30)
			{
				NPC.velocity.Y += 1f;
			}
			if (TileUtils.PlatformCollision(NPC.Bottom))
			{
				NPC.velocity *= 0;
				break;
			}
			yield return new SkipThisFrame();
		}
		Phase = 2;
		State = 4;
		NPC.localAI[0] = 60;
		for (int t = 0; t < 60; t++)
		{
			NPC.localAI[0]--;
			yield return new SkipThisFrame();
		}
		NPC.localAI[0] = 0;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));
		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> SlashAttack0()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		Vector2 attackPos = player.Center;
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), attackPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash_Target>(), 0, 2, default, 2);
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash_Shine>(), 0, 2, default, 2);
		for (int t = 0; t < 10; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		State = 5;
		for (int t = 0; t < 20; t++)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), attackPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash>(), 60, 2, default, 2);
			NPC.position = attackPos + new Vector2(Main.rand.NextFloat(-270, 270), Main.rand.NextFloat(-90, 90));
			NPC.spriteDirection *= -1;
			NPC.rotation = Main.rand.NextFloat(MathHelper.TwoPi);
			if (t % 2 == 0)
			{
				Vector2 startPos = new Vector2(0, Main.rand.NextFloat(45, 200)).RotatedByRandom(MathHelper.TwoPi);
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), attackPos + startPos * 3, -startPos.RotatedByRandom(0.4f), ModContent.ProjectileType<Fevens_Wing_Slash>(), 60, 2, default, 2);
			}

			yield return new SkipThisFrame();
		}
		NPC.alpha = 255;
		Vector2 nextPos = player.Center + new Vector2(0, -320).RotatedByRandom(0.5f);
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), nextPos, Vector2.zeroVector, ModContent.ProjectileType<Fevens_WingTarget>(), 0, 2, default, 2);
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		for (int i = 0; i < 8; i++)
		{
			Vector2 startPos = new Vector2(0, -120).RotatedBy(Main.time + i / 8d * MathHelper.TwoPi);
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), nextPos + startPos, -startPos.RotatedBy(MathHelper.PiOver4 / 2) * 0.4f, ModContent.ProjectileType<Fevens_Wing_Slash>(), 60, 2, default, 2);
		}
		yield return new WaitForFrames(4);
		for (int i = 0; i < 4; i++)
		{
			Vector2 startPos = new Vector2(-720, -675).RotatedBy(-i * 0.5f);
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), nextPos + startPos, -startPos * 0.2f, ModContent.ProjectileType<Fevens_Wing_Slash>(), 60, 2, default, 2);

			startPos = new Vector2(720, -675).RotatedBy(i * 0.5f);
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), nextPos + startPos, -startPos * 0.2f, ModContent.ProjectileType<Fevens_Wing_Slash>(), 60, 2, default, 2);
			yield return new WaitForFrames(2);
		}
		NPC.Center = nextPos;
		NPC.rotation = 0;
		NPC.alpha = 0;
		if (Main.rand.NextBool(2))
		{
			NPC.spriteDirection *= -1;
		}
		State = 4;

		for (int t = 0; t < 30; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> SlashAttack1()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		Vector2 attackPos = player.Center;
		int attackDirection = 1;
		if (Main.rand.NextBool())
		{
			attackDirection = -1;
		}
		NPC.direction = attackDirection;
		NPC.spriteDirection = attackDirection;
		NPC.Center = attackPos + new Vector2(attackDirection * 220, 0);

		State = 9;
		for (int t = 0; t < 20; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_TaijutsuSlash>(), 130, 2, default, 1.25f, MathHelper.PiOver4 * -attackDirection, 0.45f);
		p0.direction = -attackDirection;
		p0.extraUpdates = 6;

		State = 4;
		for (int t = 0; t < 10; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		State = 6;
		for (int t = 0; t < 10; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		NPC.position.X += -attackDirection * 120;
		NPC.position.Y += 48;
		Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_TaijutsuSlash>(), 144, 2, default, 1.25f, MathHelper.PiOver4 * -attackDirection, -0.45f);
		p1.direction = -attackDirection;
		p1.extraUpdates = 6;

		State = 4;
		for (int t = 0; t < 10; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		if (attackDirection == -1)
		{
			State = 6;
		}
		else
		{
			State = 9;
		}
		for (int t = 0; t < 10; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}

		Vector2 towardPlayer = Vector2.Normalize(player.Center - NPC.Center) * 4.3f;
		Projectile p2 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, towardPlayer, ModContent.ProjectileType<Fevens_TaijutsuSlash_Round>(), 24, 20, default, 0.75f, MathHelper.PiOver4 * -attackDirection, -0.45f);
		p2.spriteDirection = -attackDirection;
		p2.extraUpdates = 5;
		NPC.dontTakeDamage = true;
		NPC.alpha = 255;
		State = 4;
		for (int t = 0; t < 120; t++)
		{
			NPC.rotation += 0.7f;
			NPC.Center = p2.Center;
			if (p2.active && p2.timeLeft < 250)
			{
				NPC.alpha = p2.timeLeft;
			}
			if (p2.active && p2.timeLeft < 20)
			{
				NPC.alpha = 0;
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.dontTakeDamage = false;
		NPC.rotation = 0;
		State = 4;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> MagicAttack5()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		Vector2 attackPos = player.Center;
		int attackDirection = 1;
		if (Main.rand.NextBool())
		{
			attackDirection = -1;
		}
		NPC.direction = attackDirection;
		NPC.spriteDirection = attackDirection;
		NPC.Center = attackPos + new Vector2(attackDirection * 400, -340);
		float dir = (-new Vector2(attackDirection * 400, -340)).ToRotation();
		Projectile array = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_MagicLaserArray>(), 80, 20, default, 0.08f, dir);
		for (int t = 0; t < 50; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		Vector2 firstPos = NPC.Center;
		Vector2 firstDirection = Vector2.Normalize(player.Center - NPC.Center);
		Vector2 firstNormal = firstDirection.RotatedBy(MathHelper.PiOver2);
		State = 6;
		for (int t = 0; t < 16; t++)
		{
			Vector2 destination = firstPos + firstDirection * 200 * t;
			if (t % 2 == 0)
			{
				destination += firstNormal * 80;
			}
			else
			{
				destination -= firstNormal * 80;
			}
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), destination, Vector2.zeroVector, ModContent.ProjectileType<Fevens_TaijutsuSlash_Target>(), 0, 2, default, 2);
			yield return new WaitForFrames(4);
		}
		for (int t = 0; t < 50; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		State = 7;
		for (int t = 0; t < 16; t++)
		{
			Vector2 destination = firstPos + firstDirection * 200 * t;
			if (t % 2 == 0)
			{
				destination += firstNormal * 80;
			}
			else
			{
				destination -= firstNormal * 80;
			}
			if (t == 0)
			{
				Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_TaijutsuSlash>(), 144, 2, default, 1.25f, MathHelper.PiOver4 * -attackDirection, -0.45f);
				p0.direction = -attackDirection;
				p0.extraUpdates = 6;
				NPC.Center = destination;
				NPC.alpha = 255;
				NPC.dontTakeDamage = true;
			}

			for (int h = 0; h < 4; h++)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), destination, Vector2.zeroVector, ModContent.ProjectileType<Fevens_Slash>(), 30, 2, default, 2);
				yield return new SkipThisFrame();
			}
		}
		NPC.alpha = 0;
		NPC.dontTakeDamage = false;
		State = 4;
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> SlashAttack2_Down()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		Vector2 attackPos = player.Center;
		int attackDirection = 1;
		if (Main.rand.NextBool())
		{
			attackDirection = -1;
		}
		NPC.direction = attackDirection;
		NPC.spriteDirection = attackDirection;
		NPC.Center = attackPos + new Vector2(attackDirection * 220, -200);
		State = 7;
		for (int t = 0; t < 15; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_TaijutsuSlash_Down>(), 130, 2, default, 1.25f, 3.5f * attackDirection);
		p0.spriteDirection = -attackDirection;
		p0.extraUpdates = 6;

		for (int t = -4; t <= 4; t++)
		{
			if (t != 0)
			{
				Vector2 startPos = NPC.Center + new Vector2(t * 230 + Math.Sign(t) * 100 + attackDirection * -100, -600);
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), startPos, new Vector2(0, 100), ModContent.ProjectileType<Fevens_Wing_Slash_Down>(), 60, 2, default, MathF.Abs(t) * 20 + 40);
			}
		}
		State = 3;
		yield return new WaitForFrames(3);
		for (int t = 0; t < 3; t++)
		{
			NPC.rotation += -0.15f * attackDirection;
			yield return new SkipThisFrame();
		}
		Vector2 attackOldPos = NPC.Center;
		Vector2 playerOldPos = player.Center;
		for (int t = 0; t < 30; t++)
		{
			if (p0 != null && p0.active)
			{
				if (p0.type == ModContent.ProjectileType<Fevens_TaijutsuSlash_Down>())
				{
					Fevens_TaijutsuSlash_Down fTSD = p0.ModProjectile as Fevens_TaijutsuSlash_Down;
					if (fTSD != null)
					{
						NPC.Center = attackOldPos + fTSD.FallingMove;
						if (fTSD.HitPlayers.Contains(player.whoAmI))
						{
							player.velocity *= 0;
							player.Center = GetPlayerHitByFallingTaijutsuPos(playerOldPos + fTSD.FallingMove, player);
						}
						if (fTSD.FallingMove.Y > 1.5f && fTSD.FallingVel.Y < 1.5f)
						{
							NPC.rotation = 0;
						}
					}
				}
			}
			yield return new SkipThisFrame();
		}
		for (int t = 0; t < 30; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		State = 4;
		NPC.rotation = 0;
		for (int t = 0; t < 30; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> AimmingAndSmash()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		NPC.Center = player.Center + new Vector2(Main.rand.NextFloat(-200, 200), -300);

		Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_EnchantingSmash>(), 130, 2, default);

		NPC.rotation = 0;
		for (int t = 0; t < 150; t++)
		{
			NPC.velocity *= 0;
			if (NPC.Center.X > player.Center.X)
			{
				NPC.spriteDirection = 1;
			}
			else
			{
				NPC.spriteDirection = -1;
			}
			Vector2 toTarget = player.Center - NPC.Center;
			NPC.rotation = toTarget.ToRotation();
			if (NPC.spriteDirection == 1)
			{
				NPC.rotation = toTarget.ToRotation() + MathHelper.Pi;
			}

			if (p0 != null && p0.active)
			{
				if (p0.type == ModContent.ProjectileType<Fevens_EnchantingSmash>())
				{
					Fevens_EnchantingSmash fES = p0.ModProjectile as Fevens_EnchantingSmash;
					if (fES != null)
					{
						if (p0.timeLeft <= 5)
						{
							NPC.Center = fES.EndPosition;
						}
					}
				}
			}

			yield return new SkipThisFrame();
		}
		NPC.rotation = 0;
		for (int t = 0; t < 60; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	private IEnumerator<ICoroutineInstruction> PretentToDefense()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;
		Vector2 attackPos = player.Center;
		int attackDirection = 1;
		int oldDefense = NPC.defense;
		NPC.defense = 880;
		if (Main.rand.NextBool())
		{
			attackDirection = -1;
		}
		NPC.direction = attackDirection;
		NPC.spriteDirection = attackDirection;
		NPC.Center = attackPos + new Vector2(attackDirection * 220, 0);
		State = 8;
		for (int t = 0; t < 4; t++)
		{
			Vector2 shootVel = new Vector2(12, -6).RotatedBy(t / 4f);
			Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, shootVel, ModContent.ProjectileType<Fevens_Wing_Fly>(), 10, 2, default, t);
			p0.spriteDirection = -1;
		}
		for (int t = 0; t < 4; t++)
		{
			Vector2 shootVel = new Vector2(-12, -6).RotatedBy(-t / 4f);
			Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, shootVel, ModContent.ProjectileType<Fevens_Wing_Fly>(), 10, 2, default, t + 4);
		}
		for (int t = 0; t < 60; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		int lightingCount = 8;
		for (int j = 0; j < 8; j++)
		{
			Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2((j - lightingCount / 2 + 0.5f) * 360, Main.rand.NextFloat(-30, 30) + 240), Vector2.zeroVector, ModContent.ProjectileType<Fevens_ThunderMarkShortTiming>(), 80, 1f, -1, 0);
		}
		for (int t = 0; t < 110; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		NPC.defense = oldDefense;
		State = 4;
		for (int t = 0; t < 30; t++)
		{
			NPC.velocity *= 0;
			yield return new SkipThisFrame();
		}
		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	public Vector2 GetPlayerHitByFallingTaijutsuPos(Vector2 pos, Player player)
	{
		int count = 0;
		while (Collision.SolidCollision(pos, player.Hitbox.Width, player.Hitbox.Height))
		{
			count++;
			if (count > 100)
			{
				break;
			}
			pos.Y -= 10;
		}
		return pos;
	}

	private IEnumerator<ICoroutineInstruction> FallingSwordAttack0()
	{
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0;

		_fevensCoroutine.StartCoroutine(new Coroutine(ChooseAttack()));

		yield return new SkipThisFrame();
	}

	public void GenerateVFXBat(int count)
	{
		for (int v = 0; v < count; v++)
		{
			Vector2 startVfxPos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
			var dustVFX = new Fevens_Bat_Small
			{
				Velocity = new Vector2(0, 3).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				Position = startVfxPos,
				MaxTime = Main.rand.Next(70, 120),
				Scale = Main.rand.Next(20, 40),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), NPC.Center.X, NPC.Center.Y },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public void GenerateVFXBatShortTime(int count)
	{
		for (int v = 0; v < count; v++)
		{
			Vector2 startVfxPos = NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height));
			var dustVFX = new Fevens_Bat_Small
			{
				Velocity = new Vector2(0, 2).RotatedByRandom(Math.PI * 2),
				Active = true,
				Visible = true,
				Position = startVfxPos,
				MaxTime = Main.rand.Next(40, 50),
				Scale = Main.rand.Next(20, 40),
				ai = new float[] { Main.rand.NextFloat(1f, 8f), NPC.Center.X, NPC.Center.Y },
			};
			Ins.VFXManager.Add(dustVFX);
		}
	}

	public void GoToMagicAttackPosition()
	{
		Player player = Main.player[NPC.target];
		int dirChoose = 1;
		if (Main.rand.NextBool())
		{
			dirChoose = -1;
		}
		NPC.direction = dirChoose;
		NPC.spriteDirection = NPC.direction;
		NPC.noGravity = true;
		State = 1;
		bool lackArray = true;
		foreach (var proj in Main.projectile)
		{
			if (proj.active && proj.type == ModContent.ProjectileType<Fevens_MagicArray>())
			{
				if ((proj.Center - NPC.Center).Length() < 200)
				{
					lackArray = false;
					break;
				}
			}
		}
		if (lackArray)
		{
			Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, ModContent.ProjectileType<Fevens_MagicArray>(), 0, 0f, -1, NPC.whoAmI);
		}
	}

	private IEnumerator<ICoroutineInstruction> ChooseAttack()
	{
		if (Phase == 1)
		{
			if (NPC.life > NPC.lifeMax * 0.5f)
			{
				switch (Main.rand.Next(5))
				{
					case 0:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack0()));
						break;
					case 1:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack1()));
						break;
					case 2:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack2()));
						break;
					case 3:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack3()));
						break;
					case 4:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack4()));
						break;
					default:
						_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack1()));
						break;
				}
			}
			else
			{
				_fevensCoroutine.StartCoroutine(new Coroutine(SwitchToPhase2()));
			}
		}
		if (Phase == 2)
		{
			switch (Main.rand.Next(6))
			{
				case 0:
					_fevensCoroutine.StartCoroutine(new Coroutine(SlashAttack2_Down()));
					break;
				case 1:
					_fevensCoroutine.StartCoroutine(new Coroutine(AimmingAndSmash()));
					break;
				case 2:
					_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack5()));
					break;
				case 3:
					_fevensCoroutine.StartCoroutine(new Coroutine(SlashAttack1()));
					break;
				case 4:
					_fevensCoroutine.StartCoroutine(new Coroutine(SlashAttack0()));
					break;
				case 5:
					_fevensCoroutine.StartCoroutine(new Coroutine(PretentToDefense()));
					break;
				default:
					_fevensCoroutine.StartCoroutine(new Coroutine(MagicAttack1()));
					break;
			}
		}
		yield return new SkipThisFrame();
	}

	public override void FindFrame(int frameHeight)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (State == 1)
			{
				if (NPC.frame.Width != 132)
				{
					NPC.frame.Y = 0;
					NPC.frame.X = 0;
					NPC.frame.Width = 132;
					NPC.frame.Height = 120;
				}
				NPC.frameCounter++;
				if (NPC.frameCounter >= 5)
				{
					NPC.frame.Y += 120;
					NPC.frame.X = 0;
					NPC.frame.Width = 132;
					NPC.frame.Height = 120;
					if (NPC.frame.Y > 480)
					{
						NPC.frame.Y = 0;
					}
					NPC.frameCounter = 0;
				}
			}

			if (State == 2)
			{
				if (NPC.frame.Width != 38)
				{
					NPC.frame.Y = 0;
					NPC.frame.X = 0;
					NPC.frame.Width = 38;
					NPC.frame.Height = 58;
				}
				NPC.frameCounter++;
				if (NPC.frameCounter >= 3)
				{
					NPC.frame.Y += 58;
					NPC.frame.X = 0;
					NPC.frame.Width = 38;
					NPC.frame.Height = 58;
					if (NPC.frame.Y > 174)
					{
						NPC.frame.Y = 0;
					}
					NPC.frameCounter = 0;
				}
			}

			if (State == 3)
			{
				if (NPC.frame.Width != 38)
				{
					NPC.frame.Y = 0;
					NPC.frame.X = 0;
					NPC.frame.Width = 38;
					NPC.frame.Height = 58;
				}
				NPC.frameCounter++;
				if (NPC.frameCounter >= 2)
				{
					NPC.frame.Y += 58;
					NPC.frame.X = 0;
					NPC.frame.Width = 38;
					NPC.frame.Height = 58;
					if (NPC.frame.Y > 522)
					{
						NPC.frame.Y = 0;
					}
					NPC.frameCounter = 0;
				}
			}
		}
		base.FindFrame(frameHeight);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			drawColor *= (255 - NPC.alpha) / 255f;
			if (State == 0)
			{
				return true;
			}
			if (Phase == 2)
			{
				if (State == 4 || State == 6)
				{
					Texture2D wing = ModAsset.Fevens_Wing.Value;
					Texture2D wingFlame = ModAsset.Fevens_Wing_Flame.Value;
					float standardRot = NPC.rotation - MathHelper.PiOver4 * 3;
					for (int i = 0; i < 4; i++)
					{
						Vector2 offset = new Vector2(0, 0);
						float duration = MathF.Pow(1 - NPC.localAI[0] / 60f, 0.3f);
						float rotationWing = standardRot + duration * (1 - i * 0.24f) * 2.2f;
						float wingSize = 0.8f;
						if (NPC.spriteDirection == -1)
						{
							offset = new Vector2(-10, 0).RotatedBy(NPC.rotation);
							wingSize = 1f;
						}
						int frameIndex = (int)(Main.time * 0.17 + i * 5) % 7;
						Rectangle frameWing = new Rectangle(0, frameIndex * 40, 40, 40);

						spriteBatch.Draw(wing, NPC.Center - Main.screenPosition + offset, frameWing, drawColor, rotationWing, new Vector2(40), NPC.scale * (1 - i * 0.1f) * wingSize, SpriteEffects.None, 0);
						spriteBatch.Draw(wingFlame, NPC.Center - Main.screenPosition + offset, frameWing, new Color(0f, 1f, 1f, 0), rotationWing, new Vector2(40), NPC.scale * (1 - i * 0.1f) * wingSize, SpriteEffects.None, 0);
					}

					standardRot = NPC.rotation - MathHelper.PiOver4;
					for (int i = 0; i < 4; i++)
					{
						Vector2 offset = new Vector2(10, 0).RotatedBy(NPC.rotation);
						float duration = MathF.Pow(1 - NPC.localAI[0] / 60f, 0.3f);
						float rotationWing = standardRot - duration * (1 - i * 0.24f) * 2f - 0.2f;
						float wingSize = 1f;
						if (NPC.spriteDirection == -1)
						{
							offset = new Vector2(0);
							wingSize = 0.8f;
						}
						int frameIndex = (int)(Main.time * 0.17 + i * 5 + 3) % 7;
						Rectangle frameWing = new Rectangle(0, frameIndex * 40, 40, 40);
						spriteBatch.Draw(wing, NPC.Center - Main.screenPosition + offset, frameWing, drawColor, rotationWing, new Vector2(40, 0), NPC.scale * (1 - i * 0.1f) * wingSize, SpriteEffects.FlipVertically, 0);
						spriteBatch.Draw(wingFlame, NPC.Center - Main.screenPosition + offset, frameWing, new Color(0f, 1f, 1f, 0), rotationWing, new Vector2(40, 0), NPC.scale * (1 - i * 0.1f) * wingSize, SpriteEffects.FlipVertically, 0);
					}
				}
			}
			if (State == 1)
			{
				Rectangle frame = NPC.frame;
				Texture2D sorcererFevens = ModAsset.Fevens_Boss_MagicArray.Value;
				spriteBatch.Draw(sorcererFevens, NPC.Center - Main.screenPosition + new Vector2(0, -30), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 2 || State == 3)
			{
				Rectangle frame = NPC.frame;
				Texture2D sorcererFevens = ModAsset.Fevens_Attack0.Value;
				if (State == 3)
				{
					sorcererFevens = ModAsset.Fevens_Attack1.Value;
				}
				spriteBatch.Draw(sorcererFevens, NPC.Center - Main.screenPosition + new Vector2(0, 0), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 4)
			{
				Texture2D standFevens = ModAsset.Fevens.Value;
				Rectangle frame = new Rectangle(0, 0, standFevens.Width, standFevens.Height);
				spriteBatch.Draw(standFevens, NPC.Center - Main.screenPosition + new Vector2(0, 0), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 5)
			{
				Texture2D knifeFevens = ModAsset.Fevens_Attack2.Value;
				Rectangle frame = new Rectangle(0, 0, knifeFevens.Width, knifeFevens.Height);
				spriteBatch.Draw(knifeFevens, NPC.Center - Main.screenPosition + new Vector2(0, 0), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 6 || State == 7)
			{
				Texture2D knifeFevens = ModAsset.Fevens_Attack4.Value;
				Texture2D knifeFevensGlow = ModAsset.Fevens_Attack4_glow.Value;
				Rectangle frame = new Rectangle(0, 0, knifeFevens.Width, knifeFevens.Height);
				spriteBatch.Draw(knifeFevens, NPC.Center - Main.screenPosition + new Vector2(30 * NPC.spriteDirection, -4), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
				spriteBatch.Draw(knifeFevensGlow, NPC.Center - Main.screenPosition + new Vector2(30 * NPC.spriteDirection, -4), frame, new Color(0.4f, 0, 0.6f, 0), NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 8)
			{
				Texture2D shieldFevens = ModAsset.Fevens_Defence.Value;
				Rectangle frame = new Rectangle(0, 0, shieldFevens.Width, shieldFevens.Height);
				spriteBatch.Draw(shieldFevens, NPC.Center - Main.screenPosition + new Vector2(0, -4), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}

			if (State == 9 || State == 10)
			{
				Texture2D knifeFevens = ModAsset.Fevens_Attack3.Value;
				Texture2D knifeFevensGlow = ModAsset.Fevens_Attack3_glow.Value;
				Rectangle frame = new Rectangle(0, 0, knifeFevens.Width, knifeFevens.Height);
				spriteBatch.Draw(knifeFevens, NPC.Center - Main.screenPosition + new Vector2(30 * NPC.spriteDirection, -4), frame, drawColor, NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
				spriteBatch.Draw(knifeFevensGlow, NPC.Center - Main.screenPosition + new Vector2(30 * NPC.spriteDirection, -4), frame, new Color(255, 255, 255, 0), NPC.rotation, frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
			}
		}
		else
		{
			Texture2D texMain = ModAsset.Fevens_Walk.Value;
			Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, -13);
			Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		}
		return false;
	}
}
