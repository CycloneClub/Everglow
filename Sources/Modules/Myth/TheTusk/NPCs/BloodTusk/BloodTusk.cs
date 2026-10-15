using Everglow.Commons.Coroutines;
using Everglow.Commons.CustomTiles;
using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Miscs;
using Everglow.Commons.VFX.CommonVFXDusts;
using Everglow.Myth.TheTusk.Items;
using Everglow.Myth.TheTusk.Items.Accessories;
using Everglow.Myth.TheTusk.Items.BossDrops;
using Everglow.Myth.TheTusk.Items.Weapons;
using Everglow.Myth.TheTusk.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.ItemDropRules;
using Terraria.Localization;
using static Everglow.Myth.TheTusk.NPCs.BloodTusk.BloodTuskAtlas;

namespace Everglow.Myth.TheTusk.NPCs.BloodTusk;

[AutoloadBossHead]
[NoGameModeScale]
public class BloodTusk : ModNPC
{
	/// <summary>
	/// MapIcon Slot.
	/// </summary>
	public int SecondStageHeadSlot = -1;

	/// <summary>
	/// NPC State.
	/// </summary>
	public int State = -1;

	/// <summary>
	/// Draw offset added when being hit.
	/// </summary>
	public Vector2 BeatOffset = Vector2.Zero;

	/// <summary>
	/// Draw offset for bottom when stand.
	/// </summary>
	public Vector2 StandBottomOffset = Vector2.Zero;

	/// <summary>
	/// X refer to TuskWall Left, Y refer to TuskWall Right.
	/// </summary>
	public Vector2 TuskWallClampRange = Vector2.Zero;

	/// <summary>
	/// Part below this point will has a dissolve shader.
	/// </summary>
	public Vector2 BottomDissolvePoint = Vector2.Zero;

	/// <summary>
	/// 0~1,0 is confront (normal) while 1 is cowered.
	/// </summary>
	public float CowerValue;

	/// <summary>
	/// 0~1,0 is confront (normal) while 1 is cowered, only for main tusk.
	/// </summary>
	public float CowerValueSpecial_MainTusk;

	/// <summary>
	/// 0~1,0 for phase1 1 for phase2.
	/// </summary>
	public float FadeValue_ToPhase2;

	/// <summary>
	/// Drawing sequence.
	/// </summary>
	public List<DrawPiece> DrawPieceSequence = new List<DrawPiece>();

	/// <summary>
	/// Shader drawing sequence when switch to phase2.
	/// </summary>
	public List<DrawPiece> ShaderDrawPieceSequence = new List<DrawPiece>();

	/// <summary>
	/// A coroutine to run the state pattern.
	/// </summary>
	public CoroutineManager BloodTuskCoroutine = new CoroutineManager();

	/// <summary>
	/// Coroutine to be implemented, time.
	/// </summary>
	public List<IEnumerator<ICoroutineInstruction>> CoroutineList = new List<IEnumerator<ICoroutineInstruction>>();

	/// <summary>
	/// AIPool, choose action for coroutine.
	/// </summary>
	public List<AIAction> AIPool = new List<AIAction>();

	public struct AIAction
	{
		public List<IEnumerator<ICoroutineInstruction>> AICoroutine;
		public float Weight;

		public AIAction(List<IEnumerator<ICoroutineInstruction>> aICoroutine, float weight)
		{
			AICoroutine = aICoroutine;
			Weight = weight;
		}
	}

	public enum States
	{
		Sleep,
		Phase1,
		Phase2,
		Phase3,
		Death,
	}

	public override void Load()
	{
		string texture = BossHeadTexture + "_Void";
		SecondStageHeadSlot = Mod.AddBossHeadTexture(texture, -1);
	}

	public override void SetStaticDefaults()
	{
		var drawModifier = new NPCID.Sets.NPCBestiaryDrawModifiers()
		{
			CustomTexturePath = Texture,
			Position = new Vector2(40f, 24f),
			PortraitPositionXOverride = 0f,
			PortraitPositionYOverride = 12f,
			Scale = 0.4f,
		};
		NPCID.Sets.NPCBestiaryDrawOffset.Add(NPC.type, drawModifier);
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		string tex = "It was just a wolf tooth, dropped to the Crimson when its owner was defeated by a hero, gradually corrupted by the power of Cthulhu and granted mentality.";
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			tex = "原本只是一颗狼牙,在它的主人被勇士讨伐时掉落至猩红之地,逐渐为克苏鲁的力量所沾染,有了自己的意识";
		}
		bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
		{
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,
			BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.TheCrimson,
			new FlavorTextBestiaryInfoElement(tex),
		});
	}

	public override void SetDefaults()
	{
		NPC.behindTiles = true;
		NPC.damage = 0;
		NPC.width = 40;
		NPC.height = 40;
		NPC.defense = 15;
		NPC.lifeMax = 7800;
		if (Main.expertMode)
		{
			NPC.lifeMax = 10200;
			NPC.defense = 20;
		}
		if (Main.masterMode)
		{
			NPC.lifeMax = 13400;
			NPC.defense = 25;
		}
		for (int i = 0; i < NPC.buffImmune.Length; i++)
		{
			NPC.buffImmune[i] = true;
		}
		NPC.knockBackResist = 0f;
		NPC.value = Item.buyPrice(0, 2, 0, 0);
		NPC.color = new Color(0, 0, 0, 0);
		NPC.alpha = 0;
		NPC.aiStyle = -1;
		NPC.boss = true;
		NPC.lavaImmune = true;
		NPC.noGravity = true;
		NPC.noTileCollide = false;
		NPC.HitSound = SoundID.DD2_SkeletonHurt;
		NPC.DeathSound = SoundID.DD2_SkeletonDeath;
		NPC.dontTakeDamage = false;
		Music = Common.MythContent.QuickMusic("TuskBiome");
	}

	public override void OnSpawn(IEntitySource source)
	{
		int counts = 0;
		while (!Collision.SolidCollision(NPC.Bottom, 0, 0))
		{
			NPC.position += new Vector2(0, 10);
			counts++;
			if (counts > 100)
			{
				NPC.active = false;
				break;
			}
		}
		CowerValue = 1;
		CowerValueSpecial_MainTusk = -1;
		DrawPieceSequence = new List<DrawPiece>
		{
			 Gum_Middle, SubTusk3, SubTusk5, Tusk0, SubTusk0, SubTusk1,
			 SubTusk6, SubTusk7, Gum_Bottom, Gum_Surface, SubTusk2, SubTusk4, Gum_Surface_Center,
		};
		State = (int)States.Sleep;
		Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<BloodTusk_Sleep_Crack>(), 0, 0);
		BloodTusk_Sleep_Crack crack = proj.ModProjectile as BloodTusk_Sleep_Crack;
		crack.Tusk = NPC;
		BottomDissolvePoint = NPC.Bottom;
	}

	public override void BossHeadSlot(ref int index)
	{
		// if (firstIcon == 9999999)
		// {
		// firstIcon = index;
		// }
		// int slot = SecondStageHeadSlot;
		// if (NPC.alpha == 0 && slot != -1)
		// {
		// index = slot;
		// }
		// if (NPC.alpha > 0)
		// {
		// index = firstIcon;
		// }
	}

	public override void AI()
	{
		if (State != (int)States.Sleep)
		{
			NPC.TargetClosest();
			BloodTuskCoroutine.Update();
			if (CoroutineList.Count <= 1)
			{
				SwitchAction();
			}
		}
		if (State == (int)States.Phase2)
		{
			foreach (Player player in Main.player)
			{
				if (player.active)
				{
					if (player.Center.Y < NPC.Top.Y - 400)
					{
						if (Main.rand.NextBool(135))
						{
							Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2((Main.maxTilesX * 0.94f) * 16, (Main.maxTilesY * 0.9f) * 16), new Vector2(-21, -Main.rand.NextFloat(25, 30)), ModContent.ProjectileType<Living_Jawbone_Huge>(), 400, 10, NPC.target);
						}
						if (Main.rand.NextBool(135))
						{
							Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), new Vector2((Main.maxTilesX * 0.1f) * 16, (Main.maxTilesY * 0.9f) * 16), new Vector2(21, -Main.rand.NextFloat(25, 30)), ModContent.ProjectileType<Living_Jawbone_Huge>(), 400, 10, NPC.target);
						}
						break;
					}
				}
			}
		}
	}

	public void SetAIPool()
	{
		Player player = Main.player[NPC.target];
		AIPool = new List<AIAction>();
		if (State == (int)States.Phase1)
		{
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(), SimultaneouslyGroundSpike(1), Rise(), Wait(GetWatingTime(150)) }, 1f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(), SimultaneouslyGroundSpike(3), Rise(), Wait(GetWatingTime(150)) }, 0.7f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(), Rise(), Wait(GetWatingTime(60)) }, 1f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(), SlowWaveGroundSpike(), Rise(), Wait(GetWatingTime(150)) }, 1f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(), FastWaveGroundSpike(), Rise(), Wait(GetWatingTime(150)) }, 1f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { SprayTuskSpice(300), Wait(GetWatingTime(60)), Cower(), Rise() }, player.Center.Y > NPC.Top.Y ? 0.2f : 10f));
		}
		if (State == (int)States.Phase2)
		{
			float spiceAroundWeight = 0;
			if ((player.Center - NPC.Bottom).Length() < 250)
			{
				spiceAroundWeight = 1;
			}
			if ((player.Center - NPC.Bottom).Length() < 180)
			{
				spiceAroundWeight = 4;
			}
			if ((player.Center - NPC.Bottom).Length() < 120)
			{
				spiceAroundWeight = 20;
			}
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { SpikeAround(), Wait(GetWatingTime(30)) }, spiceAroundWeight));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { CowerToCenter(), Rise(), SprayTuskSpice_Style2(300), Wait(GetWatingTime(60)) }, player.Center.Y > NPC.Top.Y ? 0.2f : 4f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { CowerToCenter(), Rise(), SprayTuskSpice_Style3(300), Wait(GetWatingTime(60)) }, player.Center.Y > NPC.Top.Y ? 0.2f : 4f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { CowerToCenter(), Rise(), ShootBloodCursed(), Wait(GetWatingTime(600)) }, 0.5f));

			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { SpicyBloodTentacle(), Wait(GetWatingTime(30)) }, 1f));
			AIPool.Add(new AIAction(new List<IEnumerator<ICoroutineInstruction>> { Cower(true), GiantJaw(), Rise(), Wait(GetWatingTime(60)) }, Math.Clamp((NPC.Center - player.Center).Length() - 150f, 0, 1000f) / 100f));
		}
	}

	public void SwitchAction()
	{
		SetAIPool();
		float value = AIPool.Sum(action => action.Weight);
		float check = 0;

		value = Main.rand.NextFloat(value);

		// CoroutineList.AddRange(AIPool.Aggregate((selectedAction, nextAction) =>
		// {
		// if (value >= check && value < check + selectedAction.Weight)
		// {
		// return selectedAction;
		// }
		// check += selectedAction.Weight;
		// return selectedAction;
		// }).AICoroutine);
		for (int i = 0; i < AIPool.Count; i++)
		{
			if (value >= check && value < check + AIPool[i].Weight)
			{
				CoroutineList.AddRange(AIPool[i].AICoroutine);
				return;
			}
			check += AIPool[i].Weight;
		}
	}

	/// <summary>
	/// Allow to control the next action.
	/// </summary>
	/// <returns></returns>
	protected IEnumerator<ICoroutineInstruction> DoCoroutineList()
	{
		while (true)
		{
			while (CoroutineList.Count == 0)
			{
				yield return new SkipThisFrame();
			}
			yield return new AwaitForTask(CoroutineList[0]);
			CoroutineList.RemoveAt(0);
		}
	}

	public IEnumerator<ICoroutineInstruction> Wait(uint waitTime)
	{
		yield return new WaitForFrames(waitTime);
	}

	public IEnumerator<ICoroutineInstruction> Rise()
	{
		Vector2 vector = NPC.Bottom;
		NPC.dontTakeDamage = false;
		for (int i = 0; i <= 60; i++)
		{
			CowerValue *= 0.9f;
			if (i == 60)
			{
				CowerValue = 0;
			}
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			if (NPC.alpha > 25)
			{
				NPC.alpha -= 25;
			}
			else
			{
				NPC.alpha = 0;
			}
			yield return new SkipThisFrame();
		}
		NPC.damage = GetDamage(40);
		NPC.height = 184;
		NPC.Bottom = vector;
	}

	public IEnumerator<ICoroutineInstruction> Cower(bool nearPlayer = false)
	{
		Vector2 vector = NPC.Bottom;
		for (int i = 0; i <= 60; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 1f, 0.1f);
			if (i == 60)
			{
				CowerValue = 1;
			}
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			if (i > 13)
			{
				NPC.alpha += 10;
				NPC.dontTakeDamage = true;
			}
			yield return new SkipThisFrame();
		}
		NPC.damage = 0;
		NPC.height = 10;
		NPC.alpha = 255;
		NPC.Bottom = vector;
		Relocation(nearPlayer);
	}

	public IEnumerator<ICoroutineInstruction> CowerToCenter()
	{
		Vector2 vector = NPC.Bottom;
		for (int i = 0; i <= 60; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 1f, 0.1f);
			if (i == 60)
			{
				CowerValue = 1;
			}
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			if (i > 13)
			{
				NPC.alpha += 10;
				NPC.dontTakeDamage = true;
			}
			yield return new SkipThisFrame();
		}
		NPC.damage = 0;
		NPC.height = 10;
		NPC.alpha = 255;
		NPC.Bottom = vector;
		RelocationToCenter();
	}

	/// <summary>
	/// Simultaneously generate spikes on the entire ground.
	/// </summary>
	/// <param name="times"></param>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SimultaneouslyGroundSpike(int times)
	{
		for (int i = 0; i < times; i++)
		{
			Vector2 leftSpikePos = NPC.Bottom + new Vector2(-40 - i * 30, 0);
			Vector2 rightSpikePos = NPC.Bottom + new Vector2(40 - i * 30, 0);
			int distance = 25;
			if (Main.expertMode)
			{
				distance = 20;
			}
			if (Main.masterMode)
			{
				distance = 15;
			}
			for (int j = 0; j < 25 * 25 / distance; j++)
			{
				Vector2 leftNormal = -TileUtils.GetTopographicGradient(leftSpikePos, 8);
				for (int k = 0; k < 15; k++)
				{
					if (!Collision.SolidCollision(leftSpikePos, 0, 0))
					{
						leftSpikePos += leftNormal * 20;
					}
					else if (Collision.SolidCollision(leftSpikePos - leftNormal * 20, 0, 0))
					{
						leftSpikePos -= leftNormal * 20;
					}
					else
					{
						break;
					}
				}
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), leftSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
				leftSpikePos += TileUtils.GetTopographicGradient(leftSpikePos, 8).RotatedBy(-MathHelper.PiOver2) * distance * 4;

				Vector2 rightNormal = -TileUtils.GetTopographicGradient(rightSpikePos, 8);
				for (int k = 0; k < 15; k++)
				{
					if (!Collision.SolidCollision(rightSpikePos, 0, 0))
					{
						rightSpikePos += rightNormal * 20;
					}
					else if (Collision.SolidCollision(rightSpikePos - rightNormal * 20, 0, 0))
					{
						rightSpikePos -= rightNormal * 20;
					}
					else
					{
						break;
					}
				}
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), rightSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
				rightSpikePos += TileUtils.GetTopographicGradient(rightSpikePos, 8).RotatedBy(MathHelper.PiOver2) * distance * 4;
			}
			uint waitTime = 110;
			if (Main.expertMode)
			{
				waitTime = 100;
			}
			if (Main.masterMode)
			{
				waitTime = 90;
			}
			yield return new WaitForFrames(waitTime);
		}
	}

	/// <summary>
	/// A slow wave of spike.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SlowWaveGroundSpike()
	{
		Vector2 leftSpikePos = NPC.Bottom + new Vector2(-40, 0);
		Vector2 rightSpikePos = NPC.Bottom + new Vector2(40, 0);
		uint waitTime = 4;
		if (Main.expertMode)
		{
			waitTime = 3;
		}
		if (Main.masterMode)
		{
			waitTime = 1;
		}
		for (int i = 0; i < 240 / waitTime; i++)
		{
			Vector2 leftNormal = -TileUtils.GetTopographicGradient(leftSpikePos, 8);
			for (int k = 0; k < 15; k++)
			{
				if (!Collision.SolidCollision(leftSpikePos, 0, 0))
				{
					leftSpikePos += leftNormal * 10;
				}
				else if (Collision.SolidCollision(leftSpikePos - leftNormal * 10, 0, 0))
				{
					leftSpikePos -= leftNormal * 10;
				}
				else
				{
					break;
				}
			}
			Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), leftSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
			p0.timeLeft = Main.rand.Next(101, 110);
			leftSpikePos += TileUtils.GetTopographicGradient(leftSpikePos, 8).RotatedBy(-MathHelper.PiOver2) * 30;

			Vector2 rightNormal = -TileUtils.GetTopographicGradient(rightSpikePos, 8);
			for (int k = 0; k < 15; k++)
			{
				if (!Collision.SolidCollision(rightSpikePos, 0, 0))
				{
					rightSpikePos += rightNormal * 10;
				}
				else if (Collision.SolidCollision(rightSpikePos - rightNormal * 10, 0, 0))
				{
					rightSpikePos -= rightNormal * 10;
				}
				else
				{
					break;
				}
			}
			Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), rightSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
			p1.timeLeft = Main.rand.Next(101, 110);
			rightSpikePos += TileUtils.GetTopographicGradient(rightSpikePos, 8).RotatedBy(MathHelper.PiOver2) * 30;
			yield return new WaitForFrames(waitTime);
		}
	}

	/// <summary>
	/// A fast wave of spike, but sparser.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> FastWaveGroundSpike()
	{
		Vector2 leftSpikePos = NPC.Bottom + new Vector2(-40, 0);
		Vector2 rightSpikePos = NPC.Bottom + new Vector2(40, 0);
		float distance = 160;
		if (Main.expertMode)
		{
			distance = 100;
		}
		if (Main.masterMode)
		{
			distance = 70;
		}
		for (int i = 0; i < 60 * 160 / distance; i++)
		{
			Vector2 leftNormal = -TileUtils.GetTopographicGradient(leftSpikePos, 8);
			for (int k = 0; k < 15; k++)
			{
				if (!Collision.SolidCollision(leftSpikePos, 0, 0))
				{
					leftSpikePos += leftNormal * 20;
				}
				else if (Collision.SolidCollision(leftSpikePos - leftNormal * 20, 0, 0))
				{
					leftSpikePos -= leftNormal * 20;
				}
				else
				{
					break;
				}
			}
			Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), leftSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
			p0.timeLeft = Main.rand.Next(101, 110);
			leftSpikePos += TileUtils.GetTopographicGradient(leftSpikePos, 8).RotatedBy(-MathHelper.PiOver2) * distance;

			Vector2 rightNormal = -TileUtils.GetTopographicGradient(rightSpikePos, 8);
			for (int k = 0; k < 15; k++)
			{
				if (!Collision.SolidCollision(rightSpikePos, 0, 0))
				{
					rightSpikePos += rightNormal * 20;
				}
				else if (Collision.SolidCollision(rightSpikePos - rightNormal * 20, 0, 0))
				{
					rightSpikePos -= rightNormal * 20;
				}
				else
				{
					break;
				}
			}
			Projectile p1 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), rightSpikePos, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
			p1.timeLeft = Main.rand.Next(101, 110);
			rightSpikePos += TileUtils.GetTopographicGradient(rightSpikePos, 8).RotatedBy(MathHelper.PiOver2) * distance;
			yield return new WaitForFrames(2);
		}
	}

	/// <summary>
	/// Spray blood and tusk.
	/// When player fly, chance increased.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SprayTuskSpice(int maxTime)
	{
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk = MathHelper.Lerp(CowerValueSpecial_MainTusk, 1f, 0.1f);
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = 1;
			}
			if (i > 13)
			{
				NPC.dontTakeDamage = true;
			}
			if (i == 40)
			{
				Projectile fountain = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), Vector2.zeroVector, ModContent.ProjectileType<BloodFountain>(), GetDamage(14), 1, NPC.target, maxTime + 60);
				fountain.scale = 1.5f;
			}
			yield return new SkipThisFrame();
		}
		for (int i = 0; i <= maxTime; i++)
		{
			float speed = Math.Min(maxTime / 2f - Math.Abs(i - maxTime / 2f), 100) / 100f;
			speed = MathF.Sqrt(speed);
			for (int g = 0; g < 3; g++)
			{
				var blood = new BloodDrop
				{
					Velocity = new Vector2(0, -speed * 25).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
					Active = true,
					Visible = true,
					Position = NPC.Bottom + new Vector2(4, -34),
					MaxTime = Main.rand.Next(54, 360),
					Scale = Main.rand.NextFloat(6f, 55f) * (speed + 0.01f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			var bloodSplash = new BloodSplash
			{
				Velocity = new Vector2(0, -speed * 9).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
				Active = true,
				Visible = true,
				Position = NPC.Bottom + new Vector2(4, -34),
				MaxTime = Main.rand.Next(54, 75),
				Scale = Main.rand.NextFloat(6f, 18f) * (speed + 0.01f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(bloodSplash);
			int timeInterval = 15;
			if (Main.expertMode)
			{
				timeInterval = 12;
			}
			if (Main.masterMode)
			{
				timeInterval = 8;
			}
			float mulSpeed = 1f;
			if (Main.expertMode)
			{
				mulSpeed = 1.1f;
			}
			if (Main.masterMode)
			{
				mulSpeed = 1.2f;
			}
			speed *= mulSpeed;
			if (i % timeInterval == 0 && speed > 0.5f)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 35).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f)) * Main.rand.NextFloat(0.8f, 1.2f), ModContent.ProjectileType<TuskSpice_WithGravity>(), GetDamage(23), 1, NPC.target);
			}
			yield return new SkipThisFrame();
		}
		NPC.dontTakeDamage = false;
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk *= 0.9f;
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = -1;
			}
			yield return new SkipThisFrame();
		}
	}

	/// <summary>
	/// Generate 2 spicy blood tentacles.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SpicyBloodTentacle()
	{
		Vector2 startBottom = NPC.Bottom;
		Vector2 targetBottom = NPC.Bottom - new Vector2(0, 240);
		for (int i = 0; i <= 60; i++)
		{
			NPC.Bottom = NPC.Bottom * 0.94f + targetBottom * 0.06f;
			if (i == 15)
			{
				float speed = 0.34f;
				if (Main.expertMode)
				{
					speed = 0.38f;
				}
				if (Main.masterMode)
				{
					speed = 0.4f;
				}
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(speed, 0), ModContent.ProjectileType<BloodTusk_Tentacle>(), GetDamage(90), 2, NPC.target, 0, 1);
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom, new Vector2(-speed, 0), ModContent.ProjectileType<BloodTusk_Tentacle>(), GetDamage(90), 2, NPC.target, 0, -1);
			}
			StandBottomOffset = startBottom - NPC.Bottom;
			yield return new SkipThisFrame();
		}
		targetBottom = NPC.Bottom + new Vector2(0, 160);
		for (int k = 0; k < 100; k++)
		{
			if (Collision.SolidCollision(targetBottom, 0, 0))
			{
				break;
			}
			targetBottom += new Vector2(0, 2);
		}
		yield return new WaitForFrames(240);
		for (int i = 0; i <= 60; i++)
		{
			StandBottomOffset = startBottom - NPC.Bottom;
			NPC.Bottom = NPC.Bottom * 0.8f + targetBottom * 0.2f;
			yield return new SkipThisFrame();
		}
		StandBottomOffset = Vector2.zeroVector;
	}

	/// <summary>
	/// A animation to next phase.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SwitchToPhase2()
	{
		NPC.dontTakeDamage = true;
		NPC.defense = 55;
		if (Main.expertMode)
		{
			NPC.defense = 64;
		}
		if (Main.masterMode)
		{
			NPC.defense = 70;
		}
		ShaderDrawPieceSequence = [.. DrawPieceSequence];
		DrawPieceSequence.Clear();
		DrawPieceSequence = new List<DrawPiece>
		{
			 Gum_Middle, SubTusk3_2, SubTusk5_2, Tusk_Black, Tusk1, SubTusk0_2, SubTusk1_2,
			 SubTusk6_2, SubTusk7_2, Gum_Bottom, Gum_Surface, SubTusk2_2, SubTusk4_2, Gum_Surface_Center,
		};

		var rightWall = ColliderManager.Instance.Add<TuskWall>(new(1000 + Main.maxTilesX * 8, NPC.Center.Y - 760));
		rightWall.Flip = false;
		rightWall.Tusk = NPC;

		var leftWall = ColliderManager.Instance.Add<TuskWall>(new(-900 + Main.maxTilesX * 8, NPC.Center.Y - 760));
		leftWall.Flip = true;
		leftWall.Tusk = NPC;

		for (int i = 0; i < 300; i++)
		{
			FadeValue_ToPhase2 = i / 300f;

			yield return new SkipThisFrame();
		}
		FadeValue_ToPhase2 = 1;
		NPC.dontTakeDamage = false;
	}

	/// <summary>
	/// Generate spike around to hit near players.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SpikeAround()
	{
		Vector2 vector = NPC.Bottom;
		float cowerTime = 30;
		if (Main.expertMode)
		{
			cowerTime = 20;
		}
		if (Main.masterMode)
		{
			cowerTime = 10;
		}
		for (int i = 0; i <= cowerTime; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 1f, 0.2f / MathF.Log10(cowerTime));
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			yield return new SkipThisFrame();
		}
		NPC.height = 10;
		NPC.Bottom = vector;
		CowerValue = 1;
		DrawPieceSequence.Clear();
		DrawPieceSequence = new List<DrawPiece>
		{
			 Gum_Middle, SubTusk0_3, SubTusk1_3, SubTusk5_3, SubTusk6_3, SubTusk8_3, SubTusk11_3, SubTusk12_3, Tusk_Black, Tusk1, SubTusk2_3, SubTusk3_3,
			 SubTusk9_3, SubTusk10_3, Gum_Bottom, Gum_Surface, SubTusk4_3, SubTusk7_3, Gum_Surface_Center,
		};
		GenerateLongSpice(SubTusk0_3);
		GenerateLongSpice(SubTusk1_3);
		GenerateLongSpice(SubTusk2_3);
		GenerateLongSpice(SubTusk3_3);
		GenerateLongSpice(SubTusk4_3);
		GenerateLongSpice(SubTusk5_3);
		GenerateLongSpice(SubTusk6_3);
		GenerateLongSpice(SubTusk7_3);
		GenerateLongSpice(SubTusk8_3);
		GenerateLongSpice(SubTusk9_3);
		GenerateLongSpice(SubTusk10_3);
		GenerateLongSpice(SubTusk11_3);
		GenerateLongSpice(SubTusk12_3);
		float riseTime = 15;
		if (Main.expertMode)
		{
			riseTime = 10;
		}
		if (Main.masterMode)
		{
			riseTime = 5;
		}
		for (int i = 0; i <= riseTime; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 0f, 0.4f);
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue / MathF.Log10(riseTime));
			NPC.Bottom = vector;
			yield return new SkipThisFrame();
		}
		CowerValue = 0;
		NPC.height = 184;
		NPC.Bottom = vector;
		yield return new WaitForFrames(60);
		for (int i = 0; i < 180; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 1f, MathF.Pow(i / 10000f, 2) * 200);
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			yield return new SkipThisFrame();
		}
		NPC.height = 10;
		NPC.Bottom = vector;
		CowerValue = 1;
		DrawPieceSequence.Clear();
		DrawPieceSequence = new List<DrawPiece>
		{
			 Gum_Middle, SubTusk3_2, SubTusk5_2, Tusk_Black, Tusk1, SubTusk0_2, SubTusk1_2,
			 SubTusk6_2, SubTusk7_2, Gum_Bottom, Gum_Surface, SubTusk2_2, SubTusk4_2, Gum_Surface_Center,
		};
		for (int i = 0; i <= 30; i++)
		{
			CowerValue = MathHelper.Lerp(CowerValue, 0f, 0.14f);
			NPC.height = (int)MathHelper.Lerp(184, 10, CowerValue);
			NPC.Bottom = vector;
			yield return new SkipThisFrame();
		}
		NPC.height = 184;
		NPC.Bottom = vector;
		CowerValue = 0;
	}

	/// <summary>
	/// Tusk will generate surround player.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> ChasingPlayerSpike(int time)
	{
		for (int t = 0; t < time; t++)
		{
			foreach (Player player in Main.player)
			{
				if (player.active && !player.dead)
				{
					if ((player.Center - NPC.Center).Length() < 1500)
					{
						Projectile p0 = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), player.Center, Vector2.zeroVector, GetTusk_groundType(), GetDamage(30), 1, NPC.target);
						p0.timeLeft = Main.rand.Next(101, 110);
					}
				}
			}
			yield return new WaitForFrames(5);
		}
	}

	/// <summary>
	/// Spray blood and tusk in a 2nd style.
	/// When player fly, chance increased.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SprayTuskSpice_Style2(int maxTime)
	{
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk = MathHelper.Lerp(CowerValueSpecial_MainTusk, 1f, 0.1f);
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = 1;
			}
			if (i > 13)
			{
				NPC.dontTakeDamage = true;
			}
			if (i == 40)
			{
				Projectile fountain = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), Vector2.zeroVector, ModContent.ProjectileType<BloodFountain>(), GetDamage(14), 1, NPC.target, maxTime + 60);
				fountain.scale = 1.5f;
			}
			yield return new SkipThisFrame();
		}
		for (int i = 0; i <= maxTime; i++)
		{
			float speed = Math.Min(maxTime / 2f - Math.Abs(i - maxTime / 2f), 100) / 100f;
			speed = MathF.Sqrt(speed);
			for (int g = 0; g < 3; g++)
			{
				var blood = new BloodDrop
				{
					Velocity = new Vector2(0, -speed * 25).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
					Active = true,
					Visible = true,
					Position = NPC.Bottom + new Vector2(4, -34),
					MaxTime = Main.rand.Next(54, 360),
					Scale = Main.rand.NextFloat(6f, 55f) * (speed + 0.01f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			var bloodSplash = new BloodSplash
			{
				Velocity = new Vector2(0, -speed * 9).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
				Active = true,
				Visible = true,
				Position = NPC.Bottom + new Vector2(4, -34),
				MaxTime = Main.rand.Next(54, 75),
				Scale = Main.rand.NextFloat(6f, 18f) * (speed + 0.01f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(bloodSplash);
			int timeInterval = 40;
			if (Main.expertMode)
			{
				timeInterval = 35;
			}
			if (Main.masterMode)
			{
				timeInterval = 30;
			}
			float mulSpeed = 1f;
			if (Main.expertMode)
			{
				mulSpeed = 1.1f;
			}
			if (Main.masterMode)
			{
				mulSpeed = 1.2f;
			}
			speed *= mulSpeed;
			int maxCount = 4;
			if (Main.expertMode)
			{
				maxCount = 5;
			}
			if (Main.masterMode)
			{
				maxCount = 6;
			}
			if (i % timeInterval == 0 && speed >= 1)
			{
				float rotation = Main.rand.NextFloat(-0.6f, 0.6f);
				for (int t = -maxCount; t < maxCount + 1; t++)
				{
					Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 24).RotatedBy(rotation + t / (float)maxCount * 0.6f), ModContent.ProjectileType<TuskSpice_WithGravity>(), GetDamage(23), 1, NPC.target);
				}
			}
			yield return new SkipThisFrame();
		}
		NPC.dontTakeDamage = false;
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk *= 0.9f;
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = -1;
			}
			yield return new SkipThisFrame();
		}
	}

	/// <summary>
	/// Spray blood and tusk in a 3rd style.
	/// When player fly, chance increased.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> SprayTuskSpice_Style3(int maxTime)
	{
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk = MathHelper.Lerp(CowerValueSpecial_MainTusk, 1f, 0.1f);
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = 1;
			}
			if (i > 13)
			{
				NPC.dontTakeDamage = true;
			}
			if (i == 40)
			{
				Projectile fountain = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), Vector2.zeroVector, ModContent.ProjectileType<BloodFountain>(), GetDamage(14), 1, NPC.target, maxTime + 60);
				fountain.scale = 1.5f;
			}
			yield return new SkipThisFrame();
		}
		for (int i = 0; i <= maxTime; i++)
		{
			float speed = Math.Min(maxTime / 2f - Math.Abs(i - maxTime / 2f), 100) / 100f;
			speed = MathF.Sqrt(speed);
			for (int g = 0; g < 3; g++)
			{
				var blood = new BloodDrop
				{
					Velocity = new Vector2(0, -speed * 25).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
					Active = true,
					Visible = true,
					Position = NPC.Bottom + new Vector2(4, -34),
					MaxTime = Main.rand.Next(54, 360),
					Scale = Main.rand.NextFloat(6f, 55f) * (speed + 0.01f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			var bloodSplash = new BloodSplash
			{
				Velocity = new Vector2(0, -speed * 9).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
				Active = true,
				Visible = true,
				Position = NPC.Bottom + new Vector2(4, -34),
				MaxTime = Main.rand.Next(54, 75),
				Scale = Main.rand.NextFloat(6f, 18f) * (speed + 0.01f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(bloodSplash);
			int timeInterval = 8;
			if (Main.expertMode)
			{
				timeInterval = 6;
			}
			if (Main.masterMode)
			{
				timeInterval = 2;
			}
			float mulSpeed = 1f;
			if (Main.expertMode)
			{
				mulSpeed = 1.1f;
			}
			if (Main.masterMode)
			{
				mulSpeed = 1.2f;
			}
			speed *= mulSpeed;
			if (i % timeInterval == 0 && speed >= 0.6f)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 28).RotatedBy(MathF.Sin((float)Main.time * 0.1f) * 0.7f), ModContent.ProjectileType<TuskSpice_WithGravity>(), GetDamage(23), 1, NPC.target);
			}
			yield return new SkipThisFrame();
		}
		NPC.dontTakeDamage = false;
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk *= 0.9f;
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = -1;
			}
			yield return new SkipThisFrame();
		}
	}

	/// <summary>
	/// Shoot some "blood cursed", just like a kind of bomb.Create a jaw fish pool after explosion.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> ShootBloodCursed()
	{
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk = MathHelper.Lerp(CowerValueSpecial_MainTusk, 1f, 0.1f);
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = 1;
			}
			if (i > 13)
			{
				NPC.dontTakeDamage = true;
			}
			yield return new SkipThisFrame();
		}
		for (int i = 0; i <= 90; i++)
		{
			float speed = Math.Min(45 - Math.Abs(i - 45), 100) / 100f;
			speed = MathF.Sqrt(speed);
			for (int g = 0; g < 3; g++)
			{
				var blood = new BloodDrop
				{
					Velocity = new Vector2(0, -speed * 25).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
					Active = true,
					Visible = true,
					Position = NPC.Bottom + new Vector2(4, -34),
					MaxTime = Main.rand.Next(54, 360),
					Scale = Main.rand.NextFloat(6f, 55f) * (speed + 0.01f),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { 0f, Main.rand.NextFloat(0.0f, 4.93f) },
				};
				Ins.VFXManager.Add(blood);
			}
			var bloodSplash = new BloodSplash
			{
				Velocity = new Vector2(0, -speed * 9).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * Main.rand.NextFloat(0.8f, 1.5f),
				Active = true,
				Visible = true,
				Position = NPC.Bottom + new Vector2(4, -34),
				MaxTime = Main.rand.Next(54, 75),
				Scale = Main.rand.NextFloat(6f, 18f) * (speed + 0.01f),
				ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), 0, Main.rand.NextFloat(20.0f, 40.0f) },
			};
			Ins.VFXManager.Add(bloodSplash);
			if (i == 15)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 65).RotatedBy(-0.8f), ModContent.ProjectileType<TuskCurse>(), GetDamage(23), 1, NPC.target);
			}
			if (i == 35)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 44).RotatedBy(0.7f), ModContent.ProjectileType<TuskCurse>(), GetDamage(23), 1, NPC.target);
			}
			if (i == 50 && Main.expertMode)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 40).RotatedBy(-0.3f), ModContent.ProjectileType<TuskCurse>(), GetDamage(23), 1, NPC.target);
			}
			if (i == 65 && Main.masterMode)
			{
				Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom + new Vector2(4, -34), new Vector2(0, -speed * 72).RotatedBy(0.2f), ModContent.ProjectileType<TuskCurse>(), GetDamage(23), 1, NPC.target);
			}
			yield return new SkipThisFrame();
		}
		NPC.dontTakeDamage = false;
		for (int i = 0; i <= 60; i++)
		{
			CowerValueSpecial_MainTusk *= 0.9f;
			if (i == 60)
			{
				CowerValueSpecial_MainTusk = -1;
			}
			yield return new SkipThisFrame();
		}
	}

	/// <summary>
	/// Add a jaw under target.
	/// </summary>
	/// <returns></returns>
	public IEnumerator<ICoroutineInstruction> GiantJaw()
	{
		Player player = Main.player[NPC.target];
		Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Bottom - TileUtils.GetTopographicGradient(NPC.Bottom, 6) * 10, Vector2.zeroVector, ModContent.ProjectileType<Living_Jawbone_Huge_ground_Wave>(), GetDamage(150), 1, NPC.target);
		yield return new WaitForFrames(30);
	}

	public void GenerateLongSpice(DrawPiece drawPiece)
	{
		Vector2 offset = drawPiece.Offset1 - drawPiece.Offset0;
		Projectile projectile = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + drawPiece.Offset0 + new Vector2(0, -50) - offset * 0.5f, Vector2.zeroVector, ModContent.ProjectileType<BloodTuskLongSpice>(), GetDamage(120), 0.2f, NPC.target, offset.X, offset.Y);
		BloodTuskLongSpice bTLS = projectile.ModProjectile as BloodTuskLongSpice;
		bTLS.Tusk = NPC;
	}

	/// <summary>
	/// When invisible, teleport to a random point.
	/// </summary>
	public void Relocation(bool nearPlayer = false)
	{
		Player player = Main.player[NPC.target];
		for (int i = 0; i < 100; i++)
		{
			Vector2 randomPoint = (NPC.Bottom + player.Bottom) * 0.5f + new Vector2(Main.rand.NextFloat(-800, 800), -300);
			if (nearPlayer)
			{
				randomPoint = player.Bottom + new Vector2(Main.rand.NextFloat(-300, 300), -300);
			}
			if (State == (int)States.Phase2)
			{
				if (TuskWallClampRange.Y < TuskWallClampRange.X)
				{
					randomPoint.X = TuskWallClampRange.Y + TuskWallClampRange.X;
					randomPoint.X *= 0.5f;
				}
				else
				{
					randomPoint.X = Math.Clamp(randomPoint.X, TuskWallClampRange.X, TuskWallClampRange.Y);
				}
			}
			for (int j = 0; j < 60; j++)
			{
				if (Collision.SolidCollision(randomPoint, 0, 0))
				{
					NPC.Bottom = randomPoint;
					BottomDissolvePoint = NPC.Bottom;
					return;
				}
				randomPoint.Y += 10;
			}
		}
		BottomDissolvePoint = NPC.Bottom;
	}

	/// <summary>
	/// Teleport to a Center.
	/// </summary>
	public void RelocationToCenter()
	{
		Vector2 randomPoint = new Vector2(Main.maxTilesX * 8 + 150, NPC.Bottom.Y - 300);
		for (int j = 0; j < 60; j++)
		{
			if (Collision.SolidCollision(randomPoint, 0, 0))
			{
				NPC.Bottom = randomPoint;
				BottomDissolvePoint = NPC.Bottom;
				return;
			}
			randomPoint.Y += 10;
		}
		BottomDissolvePoint = NPC.Bottom;
	}

	public uint GetWatingTime(int time_orig)
	{
		uint waitTime = (uint)time_orig;
		if (Main.expertMode)
		{
			waitTime = (uint)(waitTime * 0.8f);
		}
		if (Main.masterMode)
		{
			waitTime = (uint)(waitTime * 0.5f);
		}
		return waitTime;
	}

	public int GetTusk_groundType()
	{
		int type = ModContent.ProjectileType<Tusk_ground_little>();
		if (Main.rand.NextBool(3))
		{
			type = ModContent.ProjectileType<Tusk_ground>();
		}
		return type;
	}

	public int GetDamage(int origValue)
	{
		if (Main.expertMode)
		{
			return (int)(origValue * 1.6f);
		}
		if (Main.masterMode)
		{
			return (int)(origValue * 2.4f);
		}
		return origValue;
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		target.AddBuff(BuffID.Bleeding, 120);
	}

	public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone)
	{
		if (NPC.Center - player.Center != Vector2.Zero)
		{
			BeatOffset += Vector2.Normalize(NPC.Center - player.Center) * Math.Clamp(item.knockBack, 0, 20f);
		}
	}

	public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone)
	{
		if (projectile.velocity != Vector2.Zero)
		{
			BeatOffset += Vector2.Normalize(projectile.velocity) * Math.Clamp(projectile.knockBack, 0, 20f);
		}
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		if (State == (int)States.Sleep)
		{
			SetAIPool();
			CoroutineList.Add(Rise());
			BloodTuskCoroutine.StartCoroutine(new Coroutine(DoCoroutineList()));
			State = (int)States.Phase1;
		}
		if (State == (int)States.Phase1 && NPC.life < NPC.lifeMax * 0.5)
		{
			State = (int)States.Phase2;
			CoroutineList.Add(SwitchToPhase2());
		}
		if (NPC.life <= 0)
		{
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
		Texture2D tuskAtlas = ModAsset.BloodTusk_Atlas.Value;
		spriteBatch.End();
		spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
		Effect effect = ModAsset.BloodTuskShader.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoiseSize"].SetValue(0.4f);

		Vector2 result1 = Vector2.Transform(BottomDissolvePoint, model * projection);
		result1 += Vector2.One;
		result1.Y = 2 - result1.Y;
		result1 *= new Vector2(Main.screenWidth, Main.screenHeight) / 2f;
		effect.Parameters["drawOrigin"].SetValue(result1);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_rgb.Value);
		effect.Parameters["noiseCoord"].SetValue(new Vector2(0, (float)Main.time * 0.0005f));

		effect.CurrentTechnique.Passes[0].Apply();

		List<Vertex2D> bars = new List<Vertex2D>();
		if (DrawMyNPC(bars, DrawPieceSequence))
		{
			Main.graphics.graphicsDevice.Textures[0] = tuskAtlas;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		}

		bars = new List<Vertex2D>();
		if (DrawDissolveNPC(bars, spriteBatch))
		{
			Main.graphics.graphicsDevice.Textures[0] = tuskAtlas;
			Main.graphics.graphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleList, bars.ToArray(), 0, bars.Count / 3);
		}

		spriteBatch.End();
		spriteBatch.Begin(sBS);
		return false;
	}

	public bool DrawMyNPC(List<Vertex2D> bars, List<DrawPiece> pieces)
	{
		if (pieces.Count <= 0)
		{
			return false;
		}
		if (CowerValueSpecial_MainTusk is >= 0 and <= 1)
		{
			pieces.ForEach(drawPiece =>
			{
				if (drawPiece.Equals(Tusk0) || drawPiece.Equals(Tusk1) || drawPiece.Equals(Tusk_Black))
				{
					drawPiece.Draw(NPC, bars, CowerValueSpecial_MainTusk);
				}
				else
				{
					drawPiece.Draw(NPC, bars);
				}
			});
		}
		else if (StandBottomOffset != Vector2.zeroVector)
		{
			pieces.ForEach(drawPiece =>
			{
				if (drawPiece.Equals(Gum_Bottom))
				{
					drawPiece.Draw(NPC, bars, StandBottomOffset);
					DrawStickyBlood(bars);
				}
				else
				{
					drawPiece.Draw(NPC, bars);
				}
			});
		}
		else
		{
			pieces.ForEach(drawPiece => drawPiece.Draw(NPC, bars));
		}
		return true;
	}

	public void DrawStickyBlood(List<Vertex2D> bars)
	{
		Texture2D texture = ModAsset.BloodTusk_Atlas.Value;
		Vector2 drawPos = NPC.Bottom + new Vector2(-10, -20);
		Rectangle rectangle = StickyBlood.DrawRectangle;
		float alpha = (255 - NPC.alpha) / 255f;
		float rotation = NPC.rotation;
		Vector2 topLeft = rectangle.TopLeft() / texture.Size();
		Vector2 topRight = rectangle.TopRight() / texture.Size();
		Vector2 bottomLeft = rectangle.BottomLeft() / texture.Size();
		Vector2 bottomRight = rectangle.BottomRight() / texture.Size();
		AddVertex(bars, drawPos + new Vector2(-rectangle.Width, 0).RotatedBy(rotation) * 0.5f, new Vector3(topLeft, 0), alpha);
		AddVertex(bars, drawPos + new Vector2(rectangle.Width, 0).RotatedBy(rotation) * 0.5f, new Vector3(topRight, 0), alpha);
		AddVertex(bars, drawPos + new Vector2(-rectangle.Width, StandBottomOffset.Y * 2 + 60).RotatedBy(rotation) * 0.5f, new Vector3(bottomLeft, 0), alpha);

		AddVertex(bars, drawPos + new Vector2(rectangle.Width, 0).RotatedBy(rotation) * 0.5f, new Vector3(topRight, 0), alpha);
		AddVertex(bars, drawPos + new Vector2(rectangle.Width, StandBottomOffset.Y * 2 + 60).RotatedBy(rotation) * 0.5f, new Vector3(bottomRight, 0), alpha);
		AddVertex(bars, drawPos + new Vector2(-rectangle.Width, StandBottomOffset.Y * 2 + 60).RotatedBy(rotation) * 0.5f, new Vector3(bottomLeft, 0), alpha);
	}

	public bool DrawDissolveNPC(List<Vertex2D> bars, SpriteBatch spriteBatch)
	{
		if (FadeValue_ToPhase2 is >= 0 and < 1)
		{
			spriteBatch.End();
			spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, default, RasterizerState.CullNone, null);
			Effect dissolve = ModAsset.BloodTusk_dissolve.Value;
			float dissolveDuration = FadeValue_ToPhase2;
			var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
			var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
			dissolve.Parameters["uTransform"].SetValue(model * projection);
			dissolve.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_spiderNet.Value);
			dissolve.Parameters["duration"].SetValue(1 - dissolveDuration);
			dissolve.Parameters["uNoiseSize"].SetValue(1f);
			dissolve.Parameters["uNoiseXY"].SetValue(new Vector2(0.5f, 0.5f));
			dissolve.CurrentTechnique.Passes[0].Apply();

			DrawMyNPC(bars, ShaderDrawPieceSequence);
			return true;
		}
		return false;
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 position, Vector3 coord, float alpha)
	{
		bars.Add(position, Lighting.GetColor(position.ToTileCoordinates()) * alpha, coord);
	}

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		return base.DrawHealthBar(hbPosition, ref scale, ref position);
	}

	public override void ModifyHoverBoundingBox(ref Rectangle boundingBox)
	{
		Rectangle rectangle = NPC.Hitbox;
		if (NPC.dontTakeDamage)
		{
			rectangle = Rectangle.emptyRectangle;
		}
		boundingBox = rectangle;
	}

	public override void OnKill()
	{
		NPC.SetEventFlagCleared(ref DownedBossSystem.downedTusk, -1);
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.WorldData);
		}
		for (int i = 0; i < 20; i++)
		{
			Gore.NewGore(NPC.GetSource_FromAI(), NPC.position + new Vector2(Main.rand.NextFloat(NPC.width), Main.rand.NextFloat(NPC.height)), new Vector2(0, -Main.rand.NextFloat(3, 16)).RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f)), ModContent.Find<ModGore>("Everglow/BloodTusk_gore" + i).Type);
		}
	}

	public override void ModifyNPCLoot(NPCLoot npcLoot)
	{
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<TuskMirror>()));
		npcLoot.Add(ItemDropRule.Common(ModContent.ItemType<BloodyTuskTrophy>(), 10, 1));

		npcLoot.Add(ItemDropRule.BossBag(ModContent.ItemType<TuskTreasureBag>()));
		npcLoot.Add(ItemDropRule.MasterModeCommonDrop(ModContent.ItemType<TuskRelic>()));

		var rule = new LeadingConditionRule(new Conditions.NotExpert());
		rule.OnSuccess(ItemDropRule.OneFromOptions(1, ModContent.ItemType<ToothStaff>(), ModContent.ItemType<ToothBow>(), ModContent.ItemType<ToothSpear>(), ModContent.ItemType<TuskLace>(), ModContent.ItemType<ToothMagicBall>(), ModContent.ItemType<BloodyBoneYoyo>(), ModContent.ItemType<SpineGun>(), ModContent.ItemType<ToothKnife>()));

		npcLoot.Add(rule);
		base.ModifyNPCLoot(npcLoot);
	}
}
