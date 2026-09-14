using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.KelpCurtain.Items.Consumables;
using Everglow.Yggdrasil.KelpCurtain.Items.Misc;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.WorldGeneration;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.KelpCurtain.NPCs.VampireMat;

[AutoloadBossHead]
[NoGameModeScale]
public class VampireMat : ModNPC
{
	public CoroutineManager AICoroutine = new CoroutineManager();

	public Rope BodyRope;

	public MassSpringContainer EularSys = new MassSpringContainer();

	public Vector2 RealCenter;

	public bool DiveAtBackground = false;

	public enum TextureState
	{
		Flat,
		TowardScreen,
		ProjRelease,
	}

	public int NPCTextureState = 0;

	public int Phase = 1;

	public int HitTimer = 0;

	public int CurrentSkillInPhase2 = 0;

	public int VortexStyle = 0;

	public int StickPlayerTimer = 0;

	public int FailToEscapeStickTimer = 0;

	public int TotalDamageIntakeWhenStickPlayer = 0;

	public bool TowardScreenAndAttacking = false;

	public bool HitStuckPlayerAtTheLastMoment = false;

	public bool StickPlayer = false;

	public static List<Vector2> BackgroundWallHoles = [new Vector2(0, 0), new Vector2(-616, -397), new Vector2(-535, 396), new Vector2(371, 610), new Vector2(611, -466),];

	public static List<Vector2> TentacleInBackgroundPoints = [new Vector2(1000, 1000), new Vector2(385, 600), new Vector2(655, 765), new Vector2(767, 890), new Vector2(766, 1131), new Vector2(460, 1394), new Vector2(951, 1282), new Vector2(947, 1590), new Vector2(1109, 1219), new Vector2(1375, 1599), new Vector2(1190, 1393), new Vector2(1088, 1192), new Vector2(1297, 1147), new Vector2(1186, 1014), new Vector2(1497, 962), new Vector2(1316, 848), new Vector2(1451, 722), new Vector2(1675, 471), new Vector2(1874, 945),];

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 11;
		NPCSpawnManager.RegisterNPC(Type);
	}

	public override void SetDefaults()
	{
		NPC.width = 100;
		NPC.height = 100;
		NPC.boss = true;
		NPC.noGravity = true;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath1;
		NPC.value = 32000;

		NPC.lifeMax = 26000;
		NPC.damage = 70;
		NPC.defense = 45;
		NPC.knockBackResist = 0f;

		NPC.noTileCollide = true;
		NPC.aiStyle = -1;
	}

	public override void OnSpawn(IEntitySource source)
	{
		NPC.localAI[0] = 0;
		NPC.TargetClosest();
		AICoroutine.StartCoroutine(new Coroutine(ChasePlayer()));
		RealCenter = NPC.Center;
		VortexStyle = Main.rand.Next(2);
	}

	public override bool CheckActive()
	{
		return false;
	}

	public override void AI()
	{
		AICoroutine.Update();
		RealCenter += NPC.velocity;
		if (BodyRope is null)
		{
			BodyRope = Rope.Create_Vine(NPC.Center, 20, 1, 1, 17.3f);
			EularSys.AddMassSpringMesh(BodyRope);
			GlobalRopeSystem.EulerContainers.Add(EularSys);
		}
		BodyRope.Masses[0].Position = RealCenter;
		BodyRope.ApplyForce_VelocityDecay(0.2f);
		if (NPCTextureState == (int)TextureState.Flat)
		{
			Rectangle hitBox = GetAABBBound(BodyRope);
			NPC.position = hitBox.TopLeft();
			NPC.width = hitBox.Width;
			NPC.height = hitBox.Height;
		}
		if (Phase == 1 && NPC.life < NPC.lifeMax * 0.3f)
		{
			Phase = 2;
		}
		if (HitTimer > 0)
		{
			HitTimer--;
		}
		if (StickPlayerTimer > 0)
		{
			StickPlayerTimer--;
		}
		if (FailToEscapeStickTimer > 0)
		{
			FailToEscapeStickTimer--;
		}
		NPC.dontTakeDamage = DiveAtBackground;
	}

	public Rectangle GetAABBBound(Rope rope)
	{
		int min_x = int.MaxValue;
		int max_x = 0;
		int min_y = int.MaxValue;
		int max_y = 0;
		foreach (var mass in rope.Masses)
		{
			int mass_x = (int)mass.Position.X;
			int mass_y = (int)mass.Position.Y;
			if (mass_x < min_x)
			{
				min_x = mass_x;
			}
			if (mass_x > max_x)
			{
				max_x = mass_x;
			}
			if (mass_y < min_y)
			{
				min_y = mass_y;
			}
			if (mass_y > max_y)
			{
				max_y = mass_y;
			}
		}
		int width = max_x - min_x;
		int height = max_y - min_y;
		return new Rectangle(min_x, min_y, width, height);
	}

	#region skills

	public IEnumerator<ICoroutineInstruction> ChasePlayer()
	{
		yield return new WaitUntil(() => NPC.target >= 0);
		bool reachTarget = false;
		float rot = NPC.rotation;
		int reachTimer = 0;
		Player player = Main.player[NPC.target];
		for (int k = 0; k < 90; k++)
		{
			Vector2 headPos = RealCenter;
			int direction = RealCenter.X > player.Center.X ? 1 : -1;

			NPC.spriteDirection = direction;
			Vector2 toTarget = player.Center - headPos;
			if (!reachTarget && toTarget.Length() < 30)
			{
				reachTarget = true;
				reachTimer = 0;
			}
			if (!reachTarget)
			{
				toTarget = toTarget.SafeNormalize(Vector2.Zero) * 11f;
				rot = toTarget.ToRotation();
				NPC.velocity = toTarget;
			}
			else
			{
				reachTimer++;
			}
			if (reachTimer >= 45)
			{
				reachTarget = false;
			}
			NPC.rotation = rot * 0.05f + NPC.rotation * 0.95f;
			if (!TowardScreenAndAttacking && StickPlayerTimer <= 0)
			{
				if (IntersectWhenFlat(player))
				{
					StickPlayer = true;
					StickPlayerTimer += 120;
					VampireMatStickScreen vMSS = new VampireMatStickScreen();
					vMSS.Owner = player;
					vMSS.Active = true;
					vMSS.Visible = true;
					vMSS.MaxTime = 200;
					vMSS.ParentNPC = NPC;
					Ins.VFXManager.Add(vMSS);
					break;
				}
			}
			yield return new SkipThisFrame();
		}
		for (int k = 0; k < 120; k++)
		{
			NPC.velocity *= 0.96f;
			if (StickPlayer)
			{
				player.Center = NPC.Center;
				if (k == 119)
				{
					HitStuckPlayerAtTheLastMoment = true;
					FailToEscapeStickTimer = 45;
					ReleasePlayer();
				}
			}
			yield return new SkipThisFrame();
		}
		StickPlayer = false;
		TotalDamageIntakeWhenStickPlayer = 0;
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> ShortDash()
	{
		yield return new WaitUntil(() => NPC.target >= 0);
		Player player = Main.player[NPC.target];
		for (int k = 0; k <= 180; k++)
		{
			if (k % 30 == 0 && !StickPlayer)
			{
				Vector2 headPos = RealCenter;
				int direction = RealCenter.X > player.Center.X ? 1 : -1;
				NPC.spriteDirection = direction;
				Vector2 toTarget = player.Center - headPos;
				toTarget = toTarget.SafeNormalize(Vector2.Zero) * 31f;
				NPC.velocity = toTarget;
			}
			else
			{
				NPC.velocity *= 0.96f;
			}
			if (!TowardScreenAndAttacking && StickPlayerTimer <= 0)
			{
				if (IntersectWhenFlat(player))
				{
					StickPlayer = true;
					StickPlayerTimer += 120;
					VampireMatStickScreen vMSS = new VampireMatStickScreen();
					vMSS.Owner = player;
					vMSS.Active = true;
					vMSS.Visible = true;
					vMSS.MaxTime = 200;
					vMSS.ParentNPC = NPC;
					Ins.VFXManager.Add(vMSS);
					break;
				}
			}
			yield return new SkipThisFrame();
		}
		for (int k = 0; k < 120; k++)
		{
			NPC.velocity *= 0.96f;
			if (StickPlayer)
			{
				player.Center = NPC.Center;
				if (k == 119)
				{
					HitStuckPlayerAtTheLastMoment = true;
					FailToEscapeStickTimer = 45;
					ReleasePlayer();
				}
			}
			yield return new SkipThisFrame();
		}
		StickPlayer = false;
		TotalDamageIntakeWhenStickPlayer = 0;
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> Escape()
	{
		int playerDeadTimer = 0;
		for (int k = 0; k < 9999; k++)
		{
			if (NPC.target >= 0)
			{
				Player player = Main.player[NPC.target];
				if (player.active && !player.dead)
				{
					if ((player.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() < 60 * 16)
					{
						AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
						yield break;
					}
					if ((player.Center - NPC.Center).Length() > 240 * 16)
					{
						NPC.active = false;
						yield break;
					}
				}
				else
				{
					playerDeadTimer++;
					if (playerDeadTimer > 120)
					{
						NPC.active = false;
						yield break;
					}
				}
			}
			else
			{
				NPC.active = false;
				yield break;
			}
			NPC.velocity *= 0.95f;
			NPC.velocity.Y += 1;
			yield return new SkipThisFrame();
		}
	}

	public IEnumerator<ICoroutineInstruction> GoBehiveBackground()
	{
		float speed = 0f;
		for (int k = 0; k < 99999; k++)
		{
			speed += 0.2f;
			if (speed >= 30)
			{
				speed = 30;
			}
			Vector2 toCenter = KelpCurtainGeneration.VampireMatCaveCenter - RealCenter;
			NPC.velocity = toCenter.SafeNormalize(Vector2.Zero) * speed * 0.25f + NPC.velocity * 0.75f;
			if (toCenter.Length() < speed * 1.5f)
			{
				DiveAtBackground = true;
				AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
				yield break;
			}
			yield return new SkipThisFrame();
		}
	}

	public IEnumerator<ICoroutineInstruction> TentacleRelease()
	{
		for (int k = 0; k < 120; k++)
		{
			NPC.velocity *= 0.96f;
			NPC.rotation *= 0.96f;
			if (NPCTextureState == (int)TextureState.Flat)
			{
				if (NPC.frame.Y == 1060)
				{
					NPC.width = 220;
					NPC.height = 220;
					NPCTextureState = (int)TextureState.TowardScreen;
					NPC.frame = new Rectangle(0, 0, 400, 400);
					break;
				}
			}
			yield return new SkipThisFrame();
		}
		NPC.velocity *= 0f;
		NPC.rotation *= 0f;
		for (int k = 0; k < 19; k++)
		{
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		TowardScreenAndAttacking = true;
		yield return new WaitForFrames(20);
		List<int> projRots = [0, 1, 2, 3, 4, 5, 6];
		for (int k = 0; k < 48; k++)
		{
			if (k % 7 == 0)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Tentacle>(), 88, 5, NPC.target);
				int number = projRots[Main.rand.Next(projRots.Count)];
				switch (number)
				{
					case 0:
						proj.rotation = -15.8f / 360f * MathHelper.TwoPi;
						break;
					case 1:
						proj.rotation = 25.54f / 360f * MathHelper.TwoPi;
						break;
					case 2:
						proj.rotation = 84.36f / 360f * MathHelper.TwoPi;
						break;
					case 3:
						proj.rotation = 143.07f / 360f * MathHelper.TwoPi;
						break;
					case 4:
						proj.rotation = -162.52f / 360f * MathHelper.TwoPi;
						break;
					case 5:
						proj.rotation = -114.61f / 360f * MathHelper.TwoPi;
						break;
					case 6:
						proj.rotation = -59.09f / 360f * MathHelper.TwoPi;
						break;
				}
				for (int j = 0; j < 2; j++)
				{
					Projectile proj_tusk = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(4, 0).RotatedBy((j - 0.5f) * 0.6f + proj.rotation), ModContent.ProjectileType<VampireMat_Attack_Proj_Tusk>(), 55, 2.5f, NPC.target);
				}
				projRots.Remove(number);
			}
			yield return new SkipThisFrame();
		}
		yield return new WaitForFrames(180);
		TowardScreenAndAttacking = false;
		for (int k = 0; k < 8; k++)
		{
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		NPCTextureState = (int)TextureState.Flat;
		NPC.frame = new Rectangle(0, 0, 346, 106);
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> ProjRelease()
	{
		for (int k = 0; k < 120; k++)
		{
			NPC.velocity *= 0.96f;
			NPC.rotation *= 0.96f;
			if (NPCTextureState == (int)TextureState.Flat)
			{
				if (NPC.frame.Y == 1060)
				{
					NPCTextureState = (int)TextureState.ProjRelease;
					NPC.frame = new Rectangle(0, 0, 400, 400);
					break;
				}
			}
			yield return new SkipThisFrame();
		}
		Player player = Main.player[NPC.target];
		NPC.velocity *= 0f;
		Vector2 toTarget = player.Center - NPC.Center;
		NPC.spriteDirection = 1;
		NPC.rotation = toTarget.ToRotationSafe() + MathHelper.PiOver2;
		for (int k = 0; k < 31; k++)
		{
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			if (k == 25)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, toTarget.NormalizeSafe() * 6f, ModContent.ProjectileType<VampireMat_Attack_Proj_Ball>(), 48, 2.5f, Main.myPlayer);
				NPC.velocity -= toTarget.NormalizeSafe() * 12f;
			}
			NPC.velocity *= 0.9f;
			yield return new SkipThisFrame();
		}
		NPCTextureState = (int)TextureState.Flat;
		NPC.frame = new Rectangle(0, 0, 346, 106);
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> TentacleRelease_Phase2()
	{
		Vector2 destination = KelpCurtainGeneration.VampireMatCaveCenter + BackgroundWallHoles[Main.rand.Next(BackgroundWallHoles.Count)];
		for (int k = 0; k < 300; k++)
		{
			Vector2 toDest = destination - NPC.Center;
			if (toDest.Length() > 200)
			{
				NPC.velocity = NPC.velocity * 0.9f + toDest.NormalizeSafe() * 20 * 0.1f;
			}
			else
			{
				NPC.velocity = NPC.velocity * 0.9f + toDest * 0.1f * 0.1f;
			}
			if (toDest.Length() < 10)
			{
				NPC.width = 220;
				NPC.height = 220;
				NPC.Center = destination;
				NPCTextureState = (int)TextureState.TowardScreen;
				NPC.frame = new Rectangle(0, 0, 400, 400);
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.velocity *= 0f;
		for (int k = 0; k < 19; k++)
		{
			if (k == 8)
			{
				DiveAtBackground = false;
			}
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		TowardScreenAndAttacking = true;
		yield return new WaitForFrames(6);
		List<int> projRots = [0, 1, 2, 3, 4, 5, 6];
		for (int k = 0; k < 34; k++)
		{
			if (k % 5 == 0)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Tentacle>(), 88, 5, NPC.target);
				int number = projRots[Main.rand.Next(projRots.Count)];
				switch (number)
				{
					case 0:
						proj.rotation = -15.8f / 360f * MathHelper.TwoPi;
						break;
					case 1:
						proj.rotation = 25.54f / 360f * MathHelper.TwoPi;
						break;
					case 2:
						proj.rotation = 84.36f / 360f * MathHelper.TwoPi;
						break;
					case 3:
						proj.rotation = 143.07f / 360f * MathHelper.TwoPi;
						break;
					case 4:
						proj.rotation = -162.52f / 360f * MathHelper.TwoPi;
						break;
					case 5:
						proj.rotation = -114.61f / 360f * MathHelper.TwoPi;
						break;
					case 6:
						proj.rotation = -59.09f / 360f * MathHelper.TwoPi;
						break;
				}
				for (int j = 0; j < 2; j++)
				{
					Projectile proj_tusk = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, new Vector2(6, 0).RotatedBy((j - 0.5f) * 0.6f + proj.rotation), ModContent.ProjectileType<VampireMat_Attack_Proj_Tusk>(), 55, 2.5f, NPC.target);
				}
				projRots.Remove(number);
			}
			yield return new SkipThisFrame();
		}
		yield return new WaitForFrames(180);
		TowardScreenAndAttacking = false;
		for (int k = 0; k < 8; k++)
		{
			if (k == 4)
			{
				DiveAtBackground = true;
			}
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		NPCTextureState = (int)TextureState.Flat;
		NPC.frame = new Rectangle(0, 0, 346, 106);
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> VortexAbsorb()
	{
		Vector2 destination = KelpCurtainGeneration.VampireMatCaveCenter + BackgroundWallHoles[0];
		for (int k = 0; k < 300; k++)
		{
			Vector2 toDest = destination - RealCenter;
			if (toDest.Length() > 200)
			{
				NPC.velocity = NPC.velocity * 0.9f + toDest.NormalizeSafe() * 20 * 0.1f;
			}
			else
			{
				NPC.velocity = NPC.velocity * 0.9f + toDest * 0.1f * 0.1f;
			}
			if (toDest.Length() < 10)
			{
				NPC.width = 220;
				NPC.height = 220;
				NPC.Center = destination;
				NPCTextureState = (int)TextureState.TowardScreen;
				NPC.frame = new Rectangle(0, 0, 400, 400);
				break;
			}
			yield return new SkipThisFrame();
		}
		NPC.velocity *= 0f;
		for (int k = 0; k < 19; k++)
		{
			if (k == 8)
			{
				DiveAtBackground = false;
			}
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		TowardScreenAndAttacking = true;
		yield return new WaitForFrames(6);
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Absorb>(), 1, 0, NPC.target);
		switch (VortexStyle)
		{
			case 0:
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Ball_In_AbsorbVortex>(), 88, 5, NPC.target);
				break;
			case 1:
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Attack_Proj_Ball_In_AbsorbVortex2>(), 88, 5, NPC.target);
				break;
		}
		VortexStyle++;
		if (VortexStyle >= 2)
		{
			VortexStyle = 0;
		}
		yield return new WaitForFrames(600);
		TowardScreenAndAttacking = false;
		for (int k = 0; k < 8; k++)
		{
			if (k == 4)
			{
				DiveAtBackground = true;
			}
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		NPCTextureState = (int)TextureState.Flat;
		NPC.frame = new Rectangle(0, 0, 346, 106);
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> TentacleHide()
	{
		Player player = Main.player[NPC.target];
		NPC.width = 220;
		NPC.height = 220;
		NPCTextureState = (int)TextureState.TowardScreen;
		NPC.frame = new Rectangle(0, 0, 400, 400);
		NPC.velocity *= 0f;
		for (int k = 0; k < 19; k++)
		{
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		TowardScreenAndAttacking = true;
		for (int k = 0; k <= 26; k++)
		{
			NPC.alpha += 10;
			if (NPC.alpha > 255)
			{
				NPC.alpha = 255;
			}
			NPC.scale -= 0.05f;
			if (NPC.scale < 0)
			{
				NPC.scale = 0;
			}
			yield return new SkipThisFrame();
		}
		for (int k = 0; k <= 600; k++)
		{
			float minDis = float.MaxValue;
			Vector2 releasePos = Vector2.zeroVector;
			List<Vector2> possiblePos = TentacleInBackgroundPoints;
			if (k % 60 == 0 && k <= 480)
			{
				for (int j = possiblePos.Count - 1; j >= 0; j--)
				{
					Vector2 offsetPos = possiblePos[j] + KelpCurtainGeneration.VampireMatCaveCenter - new Vector2(1000);
					float toPlayer = (offsetPos - player.Center).Length();
					if (toPlayer < minDis)
					{
						releasePos = possiblePos[j];
						minDis = toPlayer;
					}
				}
				possiblePos.Remove(releasePos);
				Vector2 truePos = releasePos + KelpCurtainGeneration.VampireMatCaveCenter - new Vector2(1000);
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), truePos, Vector2.zeroVector, ModContent.ProjectileType<VampireMat_Tentacle_FromBackground>(), 88, 5, NPC.target);
				proj.rotation = (player.Center - truePos).ToRotationSafe();
			}
			yield return new SkipThisFrame();
		}
		yield return new WaitForFrames(150);
		for (int k = 0; k <= 26; k++)
		{
			NPC.alpha -= 10;
			if (NPC.alpha < 0)
			{
				NPC.alpha = 0;
			}
			NPC.scale += 0.05f;
			if (NPC.scale > 1)
			{
				NPC.scale = 1;
			}
			yield return new SkipThisFrame();
		}
		TowardScreenAndAttacking = false;
		for (int k = 0; k < 8; k++)
		{
			if (k % 4 == 3)
			{
				NPC.frame.Y += 400;
			}
			yield return new SkipThisFrame();
		}
		NPCTextureState = (int)TextureState.Flat;
		NPC.frame = new Rectangle(0, 0, 346, 106);

		TentacleInBackgroundPoints = [new Vector2(1000, 1000), new Vector2(385, 600), new Vector2(655, 765), new Vector2(767, 890), new Vector2(766, 1131), new Vector2(460, 1394), new Vector2(951, 1282), new Vector2(947, 1590), new Vector2(1109, 1219), new Vector2(1375, 1599), new Vector2(1190, 1393), new Vector2(1088, 1192), new Vector2(1297, 1147), new Vector2(1186, 1014), new Vector2(1497, 962), new Vector2(1316, 848), new Vector2(1451, 722), new Vector2(1675, 471), new Vector2(1874, 945),];
		AICoroutine.StartCoroutine(new Coroutine(NextAttack()));
	}

	public IEnumerator<ICoroutineInstruction> NextAttack()
	{
		if (!PlayerInCave())
		{
			AICoroutine.StartCoroutine(new Coroutine(Escape()));
			yield break;
		}
		if (NPC.target < 0)
		{
			AICoroutine.StartCoroutine(new Coroutine(Escape()));
			yield break;
		}
		Player player = Main.player[NPC.target];
		if (!player.active || player.dead)
		{
			AICoroutine.StartCoroutine(new Coroutine(Escape()));
			yield break;
		}
		if (NPC.target >= 0 && Phase == 1)
		{
			if ((player.Center - RealCenter).Length() > 600)
			{
				AICoroutine.StartCoroutine(new Coroutine(ChasePlayer()));
				yield break;
			}
		}
		if (Phase == 2 && !DiveAtBackground)
		{
			AICoroutine.StartCoroutine(new Coroutine(GoBehiveBackground()));
			yield break;
		}

		// Main.rand.Next(3)
		switch (Phase)
		{
			case 1:
				switch (Main.rand.Next(3))
				{
					case 0:
						AICoroutine.StartCoroutine(new Coroutine(ShortDash()));
						break;
					case 1:
						AICoroutine.StartCoroutine(new Coroutine(TentacleRelease()));
						break;
					case 2:
						AICoroutine.StartCoroutine(new Coroutine(ProjRelease()));
						break;
				}
				break;

			case 2:
				CurrentSkillInPhase2++;
				if (CurrentSkillInPhase2 >= 3)
				{
					CurrentSkillInPhase2 = 0;
				}
				switch (CurrentSkillInPhase2)
				{
					case 0:
						AICoroutine.StartCoroutine(new Coroutine(VortexAbsorb()));
						break;
					case 1:
						AICoroutine.StartCoroutine(new Coroutine(TentacleHide()));
						break;
					case 2:
						AICoroutine.StartCoroutine(new Coroutine(TentacleRelease_Phase2()));
						break;
				}
				break;
		}

		yield return new SkipThisFrame();
	}

	#endregion

	public override void FindFrame(int frameHeight)
	{
		if (NPCTextureState == (int)TextureState.Flat)
		{
			float animationSpeed = 0.4f;
			NPC.frameCounter += animationSpeed;
			NPC.frameCounter %= Main.npcFrameCount[NPC.type];
			NPC.frame.Y = (int)NPC.frameCounter * 106;
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		HitTimer = 20;
		if (StickPlayer)
		{
			TotalDamageIntakeWhenStickPlayer += hit.Damage;
			if (TotalDamageIntakeWhenStickPlayer >= 100)
			{
				ReleasePlayer();
			}
		}
	}

	public void ReleasePlayer()
	{
		StickPlayer = false;
		Player player = Main.player[NPC.target];
		TotalDamageIntakeWhenStickPlayer = 0;
		Vector2 toCenter = KelpCurtainGeneration.VampireMatCaveCenter - player.Center;
		player.velocity += toCenter.NormalizeSafe() * 12f;
		player.position += toCenter.NormalizeSafe() * 100f;
		if (toCenter.Length() > 800)
		{
			player.Center = KelpCurtainGeneration.VampireMatCaveCenter - toCenter.NormalizeSafe() * 720;
		}
	}

	public override bool CanHitPlayer(Player target, ref int cooldownSlot)
	{
		if (!DiveAtBackground)
		{
			if (!TowardScreenAndAttacking)
			{
				if (HitStuckPlayerAtTheLastMoment)
				{
					HitStuckPlayerAtTheLastMoment = false;
					return true;
				}
				return false;
			}
			else
			{
				return base.CanHitPlayer(target, ref cooldownSlot);
			}
		}
		return false;
	}

	public bool IntersectWhenFlat(Player target)
	{
		if (!TowardScreenAndAttacking)
		{
			for (int k = 1; k < BodyRope.Masses.Length; k++)
			{
				Vector2 old_pos = BodyRope.Masses[k - 1].Position;
				Vector2 pos = BodyRope.Masses[k].Position;
				if (CollisionUtils.Intersect(old_pos, pos, 10, target.Top, target.Bottom, target.width))
				{
					return true;
				}
			}
		}
		return false;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo info)
	{
		OnVampireMatHitPlayer(target, info.Damage);
		base.OnHitPlayer(target, info);
	}

	public static void OnVampireMatHitPlayer(Player target, int damage)
	{
		if (NetUtils.IsSingle)
		{
			VampireHitHealEffect(target, damage);
		}
		else if (NetUtils.IsClient)
		{
			ModIns.PacketResolver.Send(new VampireMatHitPacket(damage));

			if (target.HasBuff(BuffID.Gills))
			{
				target.ClearBuff(BuffID.Gills);
			}
		}

		// Add screen effect
		var screenEffectVFX = new ScreenScaringEffect()
		{
			Active = true,
			Visible = true,
			Timer = 0,
			MaxTime = 120,
		};
		Ins.VFXManager.Add(screenEffectVFX);
	}

	public static void VampireHitHealEffect(Player target, int damage)
	{
		// Sync the buff removal on the server side
		if (target.HasBuff(BuffID.Gills))
		{
			target.ClearBuff(BuffID.Gills);
		}

		NPC owner = NPCUtils.FindNearest(target.Center, ModContent.NPCType<VampireMat>());
		if (owner is not null)
		{
			owner.netUpdate = true;
			int amount = damage * 2 + 98;
			owner.HealEffect(amount, true);
			owner.life += amount;
			if (owner.life > owner.lifeMax)
			{
				owner.life = owner.lifeMax;
			}
		}
	}

	public override bool SpecialOnKill()
	{
		if (!NetUtils.IsServer)
		{
			for (int i = 0; i < 8; i++)
			{
				Vector2 v0 = new Vector2(0, Main.rand.NextFloat(0, 6f)).RotatedByRandom(MathHelper.TwoPi);
				int type = ModContent.Find<ModGore>("Everglow/VampireMat_Gore_" + i).Type;
				Gore.NewGore(NPC.GetSource_Death(), NPC.position + new Vector2(Main.rand.Next(NPC.width), Main.rand.Next(NPC.height)), v0, type, NPC.scale);
			}
		}

		return base.SpecialOnKill();
	}

	public override void OnKill()
	{
		VampireMat_Summon.VampireMatSummonCD = 600;
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<VampireMatTreasureBag>()));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<FleshOfVampireMat>(), 1, 1, 1));
		base.ModifyNPCLoot(npcLoot);
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		return 0;
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (!DiveAtBackground)
		{
			DrawSelf(NPC, this, spriteBatch, drawColor);
		}
		return false;
	}

	public static void DrawSelf(NPC npc, VampireMat vampireMat, SpriteBatch spriteBatch, Color drawColor)
	{
		float fade = (255 - npc.alpha) / 255f;
		Effect effect = ModAsset.VampireMat_HitEffect.Value;
		SpriteBatchState sBS = GraphicsUtils.GetState(Main.spriteBatch).Value;
		var texture = ModContent.Request<Texture2D>(npc.ModNPC.Texture).Value;
		var drawPos = vampireMat.RealCenter - Main.screenPosition;
		if (vampireMat.NPCTextureState == (int)TextureState.TowardScreen)
		{
			drawPos = npc.Center - Main.screenPosition;
			texture = ModAsset.VampireMat_Attack.Value;
			if (vampireMat.TowardScreenAndAttacking)
			{
				List<Vertex2D> boss_bottom_bars = new List<Vertex2D>();
				for (int k = 0; k < 20; k++)
				{
					Vector2 offset0 = new Vector2(200, 0).RotatedBy(k / 20f * MathHelper.TwoPi);
					Vector2 offset1 = new Vector2(200, 0).RotatedBy((k + 1) / 20f * MathHelper.TwoPi);
					Vector2 pos0 = drawPos + offset0 * (1 + MathF.Sin(k / 4f * MathHelper.TwoPi + (float)Main.time * 0.09f) * 0.1f) * npc.scale;
					Vector2 pos1 = drawPos + offset1 * (1 + MathF.Sin((k + 1) / 4f * MathHelper.TwoPi + (float)Main.time * 0.09f) * 0.1f) * npc.scale;
					SpriteBatchUtils.AddVertexWithEnv_Light(boss_bottom_bars, pos0, new Vector3((new Vector2(200, 1800) + offset0) / texture.Size(), 0), false, fade);
					SpriteBatchUtils.AddVertexWithEnv_Light(boss_bottom_bars, pos1, new Vector3((new Vector2(200, 1800) + offset1) / texture.Size(), 0), false, fade);
					SpriteBatchUtils.AddVertexWithEnv_Light(boss_bottom_bars, drawPos, new Vector3(new Vector2(200, 1800) / texture.Size(), 0), false, fade);
				}
				if (boss_bottom_bars.Count > 2)
				{
					Main.spriteBatch.End();
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

					Main.graphics.GraphicsDevice.Textures[0] = texture;
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, boss_bottom_bars.ToArray(), 0, boss_bottom_bars.Count / 3);

					Main.spriteBatch.End();
					Main.spriteBatch.Begin(sBS);

					if (vampireMat.HitTimer > 0)
					{
						Main.spriteBatch.End();
						Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

						effect.Parameters["hitTimer"].SetValue(vampireMat.HitTimer);
						effect.CurrentTechnique.Passes[0].Apply();

						Main.graphics.GraphicsDevice.Textures[0] = texture;
						Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, boss_bottom_bars.ToArray(), 0, boss_bottom_bars.Count / 3);

						Main.spriteBatch.End();
						Main.spriteBatch.Begin(sBS);
					}
				}
				return;
			}
		}
		if (vampireMat.NPCTextureState == (int)TextureState.ProjRelease)
		{
			drawPos = npc.Center - Main.screenPosition;
			texture = ModAsset.VampireMat_ReleaseProj.Value;
		}

		var frame = npc.frame;
		if (vampireMat.NPCTextureState == (int)TextureState.Flat)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointWrap, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
			if (vampireMat.BodyRope is null)
			{
				spriteBatch.End();
				spriteBatch.Begin(sBS);
				return;
			}
			List<Vertex2D> bars = new List<Vertex2D>();
			for (int k = 1; k < vampireMat.BodyRope.Masses.Length; k++)
			{
				Vector2 dir = vampireMat.BodyRope.Masses[k].Position - vampireMat.BodyRope.Masses[k - 1].Position;
				dir = dir.SafeNormalize(Vector2.Zero);
				Vector2 normal = new Vector2(-dir.Y, dir.X) * 16;
				Vector2 ropePos = vampireMat.BodyRope.Masses[k - 1].Position - Main.screenPosition;
				float value = k / (float)vampireMat.BodyRope.Masses.Length;
				if (npc.velocity.X > 0)
				{
					vampireMat.AddVertex(bars, ropePos + normal, new Vector3(value, (frame.Y + frame.Height * 0.25f) / texture.Height, 0), fade);
					vampireMat.AddVertex(bars, ropePos - normal, new Vector3(value, (frame.Y + frame.Height * 0.75f) / texture.Height, 0), fade);
				}
				else
				{
					vampireMat.AddVertex(bars, ropePos + normal, new Vector3(value, (frame.Y + frame.Height * 0.75f) / texture.Height, 0), fade);
					vampireMat.AddVertex(bars, ropePos - normal, new Vector3(value, (frame.Y + frame.Height * 0.25f) / texture.Height, 0), fade);
				}
			}
			if (bars.Count > 0)
			{
				Main.graphics.graphicsDevice.Textures[0] = texture;
				Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

				if (vampireMat.HitTimer > 0)
				{
					Main.spriteBatch.End();
					Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

					effect.Parameters["hitTimer"].SetValue(vampireMat.HitTimer);
					effect.CurrentTechnique.Passes[0].Apply();

					Main.graphics.graphicsDevice.Textures[0] = texture;
					Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, bars.Count - 2);

					Main.spriteBatch.End();
					Main.spriteBatch.Begin(sBS);
				}
			}
		}
		else
		{
			var rotation = npc.rotation;
			var spriteEffect = npc.spriteDirection < 0 ? SpriteEffects.None : SpriteEffects.FlipVertically;
			spriteBatch.Draw(texture, drawPos, frame, drawColor, rotation, frame.Size() / 2, 0.8f, spriteEffect, 0);

			if (vampireMat.HitTimer > 0)
			{
				Main.spriteBatch.End();
				Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.Default, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

				effect.Parameters["hitTimer"].SetValue(vampireMat.HitTimer);
				effect.CurrentTechnique.Passes[0].Apply();

				spriteBatch.Draw(texture, drawPos, frame, drawColor, rotation, frame.Size() / 2, 0.8f, spriteEffect, 0);

				Main.spriteBatch.End();
				Main.spriteBatch.Begin(sBS);
			}
		}
		spriteBatch.End();
		spriteBatch.Begin(sBS);
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 screenPos, Vector3 coord, float fade = 1f)
	{
		bars.Add(screenPos, Lighting.GetColor((screenPos + Main.screenPosition).ToTileCoordinates()) * fade, coord);
	}

	public bool PlayerInCave()
	{
		if (NPC.target < 0)
		{
			return false;
		}
		Player player = Main.player[NPC.target];
		if ((player.Center - KelpCurtainGeneration.VampireMatCaveCenter).Length() <= 60 * 16)
		{
			return true;
		}
		return false;
	}
}
