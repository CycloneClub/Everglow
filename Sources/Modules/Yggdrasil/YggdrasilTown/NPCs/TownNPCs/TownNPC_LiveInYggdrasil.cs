using Everglow.Commons.Coroutines;
using Everglow.Commons.DataStructures;
using Everglow.Yggdrasil.WorldGeneration;
using Everglow.Yggdrasil.YggdrasilTown.VFXs.Arena;
using SubworldLibrary;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using static Everglow.Commons.Utilities.NPCUtils;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

public abstract class TownNPC_LiveInYggdrasil : ModNPC
{
	/// <summary>
	/// Control specific behaviors; Control by _townNPCGeneralCoroutine.
	/// </summary>
	public CoroutineManager _townNPCBehaviorCoroutine = new CoroutineManager();

	/// <summary>
	/// Manage behavior pool.
	/// </summary>
	public CoroutineManager _townNPCGeneralCoroutine = new CoroutineManager();

	/// <summary>
	/// Behaviors pool, waiting for _townNPCBehaviorCoroutine to execute.
	/// </summary>
	public Queue<Coroutine> BehaviorsCoroutines = new Queue<Coroutine>();
	public bool Talking = false;
	public bool Sit = false;
	public bool Idle = true;
	public bool ChallengeClicked = false;

	/// <summary>
	/// True when fighting in arena, and behave as boss if true.
	/// </summary>
	public bool ArenaFighting = false;
	public int ThreatenTarget = -1;
	public int FrameHeight = 56;

	/// <summary>
	/// A scalars for the total threaten value;
	/// </summary>
	public float TotalThreaten = 0f;

	/// <summary>
	/// A vector2 for total threaten direction;
	/// </summary>
	public Vector2 TotalThreatenDirection = Vector2.zeroVector;

	public Rectangle StandFrame = new Rectangle(0, 0, 10, 10);

	public Rectangle SitFrame = new Rectangle(0, 0, 10, 10);

	/// <summary>
	/// HomePos, force attach competitor or her home to this coordinate.
	/// </summary>
	public Point AnchorForBehaviorPos = YggdrasilTownCentralSystem.TownTopLeftWorldCoord.ToTileCoordinates();

	private int aiMainCount = 0;

	public override string LocalizationCategory => LocalizationUtils.Categories.TownNPCs;

	public override void SetDefaults()
	{
		NPC.townNPC = true;
		NPC.width = 18;
		NPC.height = 40;
		NPC.aiStyle = -1;
		NPC.damage = 100;
		NPC.defense = 100;
		NPC.lifeMax = 250;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath6;
		NPC.friendly = true;
		NPC.knockBackResist = 0;
	}

	public override float SpawnChance(NPC.Spawner spawner)
	{
		return 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		_townNPCGeneralCoroutine.StartCoroutine(new Coroutine(BehaviorControl()));
		CheckInArena();
		if (ArenaFighting)
		{
			SetDefaultsToArena();
		}
		base.OnSpawn(source);
	}

	public override bool PreAI()
	{
		aiMainCount = 0;
		return base.PreAI();
	}

	public override void AI()
	{
		_townNPCBehaviorCoroutine.Update();
		_townNPCGeneralCoroutine.Update();
		if (ArenaFighting && StartedFight)
		{
			BossAI();
			return;
		}
		if (ArenaFighting && !StartedFight)
		{
			NPC.velocity *= 0;
			NPC.frame = StandFrame;
			return;
		}
		if (!Talking)
		{
			CheckDangers();
			if (BehaviorsCoroutines.Count <= 0)
			{
				Idle = true;
				if (Peace())
				{
					if (Sit)
					{
						BehaviorsCoroutines.Enqueue(new Coroutine(SitDown(Main.rand.Next(60, 90))));
					}
					else
					{
						if (Main.rand.NextBool())
						{
							BehaviorsCoroutines.Enqueue(new Coroutine(Stand(Main.rand.Next(60, 90))));
						}
						else
						{
							BehaviorsCoroutines.Enqueue(new Coroutine(Walk(Main.rand.Next(60, 900))));
						}
					}
				}
				else if (CanEscape())
				{
					Sit = false;
					if (TotalThreaten < 1 && Main.rand.NextBool(2))
					{
						TryAttack();
					}
					else
					{
						BehaviorsCoroutines.Enqueue(new Coroutine(Walk(Main.rand.Next(60, 90))));
					}
				}
				else
				{
					Sit = false;
					TryAttack();
				}
			}
		}
		else
		{
			if (!CheckTalkingPlayer())
			{
				Talking = false;
			}
		}

		if (aiMainCount == 0)
		{
			_townNPCGeneralCoroutine.StartCoroutine(new Coroutine(BehaviorControl()));
		}

		// ai[0] = 5 is a magic number represent to a sitting town npc.
		// so we should prevent other npc from sitting the same chair.
		if (Sit)
		{
			NPC.aiStyle = NPCAIStyleID.Passive;
			NPC.ai[0] = 5;
		}
		else
		{
			NPC.aiStyle = -1;
			NPC.ai[0] = 0;
		}
	}

	public virtual bool Peace()
	{
		if (TotalThreaten > 0.005)
		{
			return false;
		}
		return true;
	}

	public virtual bool CanEscape()
	{
		// Main.NewText(TotalThreaten);
		// Main.NewText(MathF.Abs(TotalThreatenDirection.X), Color.Red);
		if (!CanContinueWalk(NPC))
		{
			NPC.frame = StandFrame;
			return false;
		}
		if (TotalThreaten > MathF.Abs(TotalThreatenDirection.X) * 3f)
		{
			NPC.frame = StandFrame;
			return false;
		}
		if (!SafeAndInSuitable())
		{
			return false;
		}
		return true;
	}

	public virtual bool SafeAndInSuitable()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return true;
		}
		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X < AnchorForBehaviorPos.X - 90 && TotalThreatenDirection.X < 0)
		{
			NPC.frame = StandFrame;
			return false;
		}
		if (npcTilePos.X > AnchorForBehaviorPos.X + 90 && TotalThreatenDirection.X > 0)
		{
			NPC.frame = StandFrame;
			return false;
		}
		return true;
	}

	public virtual bool CheckTalkingPlayer()
	{
		for (int j = 0; j < 255; j++)
		{
			if (Main.player[j].active && Main.player[j].talkNPC == NPC.whoAmI)
			{
				if (Main.player[j].position.X + Main.player[j].width / 2 < NPC.position.X + NPC.width / 2)
				{
					NPC.direction = -1;
				}
				else
				{
					NPC.direction = 1;
				}
				return true;
			}
		}
		return false;
	}

	public virtual void CheckDuplicatedSelf()
	{
		foreach (NPC npc in Main.npc)
		{
			if (npc != null && npc.type == Type && npc.active && npc != NPC)
			{
				npc.active = false;
			}
			if (NPC.CountNPCS(Type) <= 1)
			{
				break;
			}
		}
	}

	public virtual void TryAttack()
	{
	}

	/// <summary>
	/// Analysis surrounding threaten sources.
	/// </summary>
	public virtual void CheckDangers()
	{
		ThreatenTarget = -1;
		TotalThreatenDirection = Vector2.zeroVector;
		TotalThreaten = 0f;
		float minDis = 300;
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && !npc.CountsAsACritter && npc != NPC)
			{
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
				if (distance.Length() < 300 && npc.damage > 0)
				{
					TotalThreatenDirection += (-distance.NormalizeSafe() / (distance.Length() + 1)) * 1f * npc.damage;
					TotalThreaten += (-distance.NormalizeSafe() / (distance.Length() + 1)).Length() * 1f * npc.damage;
					if (distance.Length() < minDis)
					{
						minDis = distance.Length();
						ThreatenTarget = npc.whoAmI;
					}
				}
			}
		}
	}

	public virtual void CheckInSuitableArea()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return;
		}
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

	public virtual void CheckInArena()
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			ArenaFighting = true;
			return;
		}
	}

	public virtual void TeleportHome()
	{
		NPC.Center = AnchorForBehaviorPos.ToWorldCoordinates() + new Vector2(0, 48);
	}

	public virtual void EndAIPiece()
	{
		Idle = true;
		BehaviorsCoroutines.Dequeue();
	}

	/// <summary>
	/// Inheritable
	/// </summary>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> BehaviorControl()
	{
		while (true)
		{
			CheckInSuitableArea();
			CheckDuplicatedSelf();
			aiMainCount++;
			if (BehaviorsCoroutines.Count > 0 && Idle)
			{
				Idle = false;
				_townNPCBehaviorCoroutine.StartCoroutine(BehaviorsCoroutines.First());
			}
			if (aiMainCount >= 2)
			{
				yield break;
			}
			yield return new SkipThisFrame();
		}
	}

	/// <summary>
	/// Sitting silently; Inheritable
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> SitDown(int time)
	{
		Sit = true;
		for (int t = 0; t < time; t++)
		{
			NPC.frame = SitFrame;
			NPC.velocity.X = 0;
			NPC.spriteDirection = NPC.direction;
			if (!Peace())
			{
				if (TotalThreatenDirection.X > 0)
				{
					NPC.direction = 1;
				}
				else
				{
					NPC.direction = -1;
				}
				break;
			}
			yield return new SkipThisFrame();
		}
		Sit = false;
		EndAIPiece();
	}

	/// <summary>
	/// Standing silently; Inheritable
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Stand(int time)
	{
		for (int t = 0; t < time; t++)
		{
			NPC.frame = StandFrame;
			NPC.velocity.X = 0;
			NPC.spriteDirection = NPC.direction;
			if (!Peace())
			{
				if (TotalThreatenDirection.X > 0)
				{
					NPC.direction = 1;
				}
				else
				{
					NPC.direction = -1;
				}
				break;
			}
			yield return new SkipThisFrame();
		}
		EndAIPiece();
	}

	/// <summary>
	/// Walking, include escape under duress, jump in front of a protruding tile, door opening(closing), check chair for sitting; Inheritable
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> Walk(int time)
	{
		NPC.direction = ChooseDirection(NPC);
		for (int t = 0; t < time; t++)
		{
			// Too far from home will trigger teleportation.
			CheckWalkBound();
			if (TotalThreatenDirection.X > 0.32f)
			{
				NPC.direction = 1;
			}
			if (TotalThreatenDirection.X < -0.32f)
			{
				NPC.direction = -1;
			}
			NPC.spriteDirection = NPC.direction;
			float speed = 1.2f;
			if (TotalThreatenDirection.Length() > 0.5f)
			{
				speed += TotalThreatenDirection.Length() - 0.5f;
			}
			if (!CanEscape())
			{
				break;
			}
			WalkFrame();
			NPC.velocity.X = NPC.direction * speed;
			if (!CanContinueWalk(NPC))
			{
				break;
			}
			if (Talking)
			{
				break;
			}

			if (Main.rand.NextBool(24))
			{
				if (CheckSit(NPC))
				{
					Sit = true;
					break;
				}
			}
			TryOpenDoor(NPC);
			TryCloseDoor(NPC);
			yield return new SkipThisFrame();
		}
		NPC.velocity.X *= 0;
		EndAIPiece();
	}

	public virtual void CheckWalkBound()
	{
		if (SubworldSystem.Current is not YggdrasilWorld)
		{
			return;
		}
		var npcTilePos = NPC.Center.ToTileCoordinates();
		if (npcTilePos.X < AnchorForBehaviorPos.X - 90)
		{
			NPC.direction = 1;
		}
		if (npcTilePos.X > AnchorForBehaviorPos.X + 90)
		{
			NPC.direction = -1;
		}
	}

	/// <summary>
	/// Only trigger when walking
	/// </summary>
	public virtual void WalkFrame()
	{
		NPC.frameCounter += Math.Abs(NPC.velocity.X);
		if (NPC.frameCounter > 4)
		{
			NPC.frame.Y += FrameHeight;
			NPC.frameCounter = 0;
		}
		if (NPC.frame.Y > 9 * FrameHeight)
		{
			NPC.frame.Y = 0;
		}
	}

	public override bool CheckActive()
	{
		return false;
	}

	public override bool CanChat()
	{
		return !YggdrasilTownCentralSystem.InArena_YggdrasilTown();
	}

	public override string GetChat()
	{
		Talking = true;
		ChallengeClicked = false;
		YggdrasilTownCentralSystem.FightingRequestPlayerNPCType = new Point(-1, -1);
		return base.GetChat();
	}

	public override void RegisterChatButtons(NPCInteractionList interactions)
	{
		interactions.InsertBefore(new ChallengeButton(), NPCInteractionDatabase.CloseButton);
		interactions.InsertBefore(new HelpButton(), NPCInteractionDatabase.CloseButton);
	}

	private sealed class ChallengeButton : NPCInteraction
	{
		public override bool Condition() => true;

		public override string GetText() => Language.GetTextValue(
			((TownNPC_LiveInYggdrasil)TalkNPC.ModNPC).ChallengeClicked ? "Fight Now" : "Challenge");

		public override void Interact()
		{
			var npc = (TownNPC_LiveInYggdrasil)TalkNPC.ModNPC;
			if (npc.ChallengeClicked)
			{
				YggdrasilTownCentralSystem.TryEnterArena();
			}
			if (!npc.ChallengeClicked)
			{
				npc.ChallengeClicked = true;
				YggdrasilTownCentralSystem.FightingRequestPlayerNPCType = new Point(Main.myPlayer, npc.Type);
			}
		}
	}

	private sealed class HelpButton : NPCInteraction
	{
		public override bool Condition() => true;

		public override string GetText() => Language.GetTextValue("Help");

		public override void Interact()
		{
		}
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texMain = ModAsset.Restauranteur.Value;
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height + 8) * 0.5f;
		Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);
		return false;
	}

	// -------Below is Boss Part-----------Below is Boss Part-----------Below is Boss Part-----------Below is Boss Part-----------Below is Boss Part-----------Below is Boss Part-----------Below is Boss Part-----------
	public struct BossTag(string name = default, int value = default, string display = default)
	{
		public bool Enable = false;
		public string Name = name;
		public string DisplayContents = display;
		public int Value = value;
		public List<string> FatherTags;
		public List<string> ConflictTags;
		public int IconType = -1;
		public Color IconColor = Color.White;
	}

	public List<BossTag> MyBossTags = new List<BossTag>();

	public float BossMovingSpeed = 1f;

	public float Resilience = 0;

	public float ResilienceMax = 100;

	public float SkillPower = 0;

	public float SkillPowerMax = 100;

	public float AttackSpeed = 1f;

	public bool StartedFight = false;

	public int LifeRegenPerS = 0;

	public int LifeTimer = 0;

	public int MaxHitPlayerCount = -1;

	public int HitPlayerCount;

	public int BossTimer;

	public int FailTime = -1;

	public int FinishTime = -1;

	/// <summary>
	/// Timer for hit effect(White or Red light).
	/// </summary>
	public int HitEffectTimer = 0;

	/// <summary>
	/// Timer for knock out animation.
	/// </summary>
	public int KnockOutTimer = 0;

	public bool Fail = false;

	public bool KnockOut = false;

	public bool ImmuneLower300 = false;

	public bool ImmuneUpper300 = false;

	public bool DisableKnockBack = false;

	public bool ContentTag(string name)
	{
		foreach (var tag in MyBossTags)
		{
			if (tag.Name == name)
			{
				return true;
			}
		}
		return false;
	}

	public virtual void SetDefaultsToArena()
	{
		NPC.townNPC = false;
		NPC.friendly = true;
		NPC.boss = true;
		NPC.dontTakeDamage = true;
		NPC.width = 18;
		NPC.height = 40;
		NPC.aiStyle = -1;
		NPC.damage = 100;
		NPC.defense = 100;
		NPC.lifeMax = 5000;
		NPC.life = NPC.lifeMax;
		NPC.damage = 50;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath6;
		Music = Common.YggdrasilContent.QuickMusic(ModAsset.Arena_BGM_Path);

		NPC.knockBackResist = 1f;

		FailTime = -1;
		LifeRegenPerS = 0;
		LifeTimer = 0;
		HitPlayerCount = 0;
		HitEffectTimer = 0;
		KnockOutTimer = 0;

		SkillPower = SkillPowerMax;
		ResilienceMax = 100;
		BossMovingSpeed = 1f;
		AttackSpeed = 1f;
		Resilience = ResilienceMax;

		StartedFight = false;
		ImmuneLower300 = false;
		ImmuneUpper300 = false;
		KnockOut = false;
		Fail = false;
		InitializeBossTags();
	}

	public virtual void InitializeBossTags()
	{
		MyBossTags = new List<BossTag>();
		var bossTags = new List<BossTag>
		{
			new BossTag("PlayerDamageReduce10", 10, "Decrease your damage by 10%, same effects stacked.") { IconType = 1 },
			new BossTag("PlayerDamageReduce20", 10, "Decrease your damage by 20%, same effects stacked.") { IconType = 1 },
			new BossTag("PlayerDamageReduce30", 30, "Decrease your damage by 30%, same effects stacked.") { IconType = 1 },

			new BossTag("BossDamagePlus30", 10, "Increase competitor damage by 30%, same effects stacked.") { IconType = 0 },
			new BossTag("BossDamagePlus50", 10, "Increase competitor damage by 50%, same effects stacked.") { IconType = 0 },
			new BossTag("BossDamagePlus120", 30, "Increase competitor damage by 120%, same effects stacked.") { IconType = 0 },

			new BossTag("FastMoving20", 10, "Increase competitor moving speed by 20%, same effects stacked.") { IconType = 3 },
			new BossTag("FastMoving50", 20, "Increase competitor moving speed by 50%, same effects stacked.") { IconType = 3 },
			new BossTag("FastMoving100", 30, "Increase competitor moving speed by 100%, same effects stacked.") { IconType = 3 },

			new BossTag("BanHealthPotion", 30, "Prohibit the use of life-restoring potions.") { IconType = 4, ConflictTags = new List<string>() { "HalfHealthPotion" } },
			new BossTag("HalfHealthPotion", 10, "Halves the effect of life-restoring potions.") { IconType = 5, ConflictTags = new List<string>() { "BanHealthPotion" } },

			new BossTag("BossDefIncrease20", 10, "Increase competitor defense by 20, same effects stacked.") { IconType = 6 },
			new BossTag("BossDefIncrease40", 20, "Increase competitor defense by 40, same effects stacked.") { IconType = 6 },
			new BossTag("BossDefIncrease60", 30, "Increase competitor defense by 60, same effects stacked.") { IconType = 6 },

			new BossTag("PlayerDefenseReduce20", 10, "Decrease your defense by 20, same effects stacked.") { IconType = 7 },
			new BossTag("PlayerDefenseReduce30", 20, "Decrease your defense by 30, same effects stacked.") { IconType = 7 },

			new BossTag("BossLifeIncrease50", 10, "Increase competitor max life by 50%, same effects stacked.") { IconType = 8 },
			new BossTag("BossLifeIncrease150", 20, "Increase competitor max life by 150%, same effects stacked.") { IconType = 8 },
			new BossTag("BossLifeIncrease250", 30, "Increase competitor max life by 250%, same effects stacked.") { IconType = 8 },

			new BossTag("LifeRegeneration20", 10, "Competitor life will regenerate by 20 per second, same effects stacked.") { IconType = 9 },
			new BossTag("LifeRegeneration30", 20, "Competitor life will regenerate by 30 per second, same effects stacked.") { IconType = 9 },

			new BossTag("DisableCreate", 10, "Prohibition of the creation and destruction of tiles.") { IconType = 10 },
			new BossTag("DisableKnockBack", 10, "He immunes knockback.") { IconType = 11 },

			new BossTag("PlayerLifeReduce25", 10, "Decrease your max life by 25%, same effects stacked.") { IconType = 12 },
			new BossTag("PlayerLifeReduce50", 20, "Decrease your max life by 50%, same effects stacked.") { IconType = 12 },

			new BossTag("BossImmuneIn300Distance", 30, "He will immune all damage at a distance of less than 300") { IconType = 13, ConflictTags = new List<string>() { "BossImmuneOff300Distance" } },
			new BossTag("BossImmuneOff300Distance", 30, "He will immune all damage at a distance of more than 300") { IconType = 14, ConflictTags = new List<string>() { "BossImmuneIn300Distance" } },

			new BossTag("3Inventory", 20, "You can only carry a maximum of 3 items in your backpack.") { IconType = 15, ConflictTags = new List<string>() { "4Accessories" } },
			new BossTag("4Accessories", 20, "You can only carry a maximum of 4 items in your accessories slots.") { IconType = 38, ConflictTags = new List<string>() { "3Inventory" } },

			new BossTag("15Hits", 10, "Forced death from 15 injuries") { IconType = 16, ConflictTags = new List<string>() { "5Hits" } },
			new BossTag("5Hits", 30, "Forced death from 5 injuries.") { IconType = 17, ConflictTags = new List<string>() { "15Hits" } },

			new BossTag("180Seconds", 10, "Limit 180 seconds.") { IconType = 18, ConflictTags = new List<string>() { "90Seconds" } },
			new BossTag("90Seconds", 30, "Limit 90 seconds.") { IconType = 18, ConflictTags = new List<string>() { "180Seconds" } },

			new BossTag("FasterAttack100", 20, "Increase competitor attack speed by 100%, same effects stacked.") { IconType = 21 },
			new BossTag("FasterAttack200", 30, "Increase competitor attack speed by 200%, same effects stacked.") { IconType = 21 },

			new BossTag("RemoveTopPlatform", 20, "Remove the top layer of platforms.") { IconType = 33, ConflictTags = new List<string>() { "RemoveBottomPlatform", "RemoveLeftPlatform", "RemoveRightPlatform" } },
			new BossTag("RemoveBottomPlatform", 20, "Remove the bottom layer of platforms.") { IconType = 34, ConflictTags = new List<string>() { "RemoveTopPlatform", "RemoveLeftPlatform", "RemoveRightPlatform" } },
			new BossTag("RemoveLeftPlatform", 20, "Remove the left side of both layers of platforms.") { IconType = 35, ConflictTags = new List<string>() { "RemoveTopPlatform", "RemoveBottomPlatform", "RemoveRightPlatform" } },
			new BossTag("RemoveRightPlatform", 20, "Remove the right side of both layers of platforms.") { IconType = 36, ConflictTags = new List<string>() { "RemoveTopPlatform", "RemoveLeftPlatform", "RemoveBottomPlatform" } },
		};
		MyBossTags.AddRange(bossTags);
	}

	public virtual void EnableBossTag(int index)
	{
		if (index >= MyBossTags.Count)
		{
			return;
		}
		BossTag[] oldTags = MyBossTags.ToArray();
		MyBossTags.Clear();
		BossTag targetTag = oldTags[index];
		targetTag.Enable = true;
		for (int i = 0; i < oldTags.Length; i++)
		{
			if (targetTag.ConflictTags is not null && targetTag.ConflictTags.Contains(oldTags[i].Name))
			{
				oldTags[i].Enable = false;
			}
		}
		for (int i = 0; i < oldTags.Length; i++)
		{
			if (i == index)
			{
				MyBossTags.Add(targetTag);
			}
			else
			{
				MyBossTags.Add(oldTags[i]);
			}
		}
	}

	public virtual void DisableBossTag(int index)
	{
		if (index >= MyBossTags.Count)
		{
			return;
		}
		BossTag[] oldTags = MyBossTags.ToArray();
		MyBossTags.Clear();
		BossTag targetTag = oldTags[index];
		targetTag.Enable = false;
		for (int i = 0; i < oldTags.Length; i++)
		{
			if (i == index)
			{
				MyBossTags.Add(targetTag);
			}
			else
			{
				MyBossTags.Add(oldTags[i]);
			}
		}
	}

	public virtual void ApplyBossTags()
	{
		foreach (var tag in MyBossTags)
		{
			int origDamage = NPC.damage;
			if (tag.Name == "BossDamagePlus30" && tag.Enable)
			{
				NPC.damage += (int)(origDamage * 0.3);
			}
			if (tag.Name == "BossDamagePlus50" && tag.Enable)
			{
				NPC.damage += (int)(origDamage * 0.5);
			}
			if (tag.Name == "BossDamagePlus120" && tag.Enable)
			{
				NPC.damage += (int)(origDamage * 0.5);
			}
			if (tag.Name == "FastMoving20" && tag.Enable)
			{
				BossMovingSpeed += 0.2f;
			}
			if (tag.Name == "FastMoving50" && tag.Enable)
			{
				BossMovingSpeed += 0.5f;
			}
			if (tag.Name == "FastMoving100" && tag.Enable)
			{
				BossMovingSpeed += 1.0f;
			}
			if (tag.Name == "BossDefIncrease20" && tag.Enable)
			{
				NPC.defense += 20;
			}
			if (tag.Name == "BossDefIncrease40" && tag.Enable)
			{
				NPC.defense += 40;
			}
			if (tag.Name == "BossDefIncrease60" && tag.Enable)
			{
				NPC.defense += 60;
			}
			int origLifeMax = NPC.lifeMax;
			if (tag.Name == "BossLifeIncrease50" && tag.Enable)
			{
				NPC.lifeMax += (int)(origLifeMax * 0.5f);
			}
			if (tag.Name == "BossLifeIncrease150" && tag.Enable)
			{
				NPC.lifeMax += (int)(origLifeMax * 1.5f);
			}
			if (tag.Name == "BossLifeIncrease250" && tag.Enable)
			{
				NPC.lifeMax += (int)(origLifeMax * 2.5f);
			}
			if (tag.Name == "LifeRegeneration20" && tag.Enable)
			{
				LifeRegenPerS += 20;
			}
			if (tag.Name == "LifeRegeneration30" && tag.Enable)
			{
				LifeRegenPerS += 30;
			}
			if (tag.Name == "BossImmuneIn300Distance" && tag.Enable)
			{
				ImmuneLower300 = true;
			}
			if (tag.Name == "BossImmuneOff300Distance" && tag.Enable)
			{
				ImmuneUpper300 = true;
			}
			if (tag.Name == "DisableKnockBack" && tag.Enable)
			{
				DisableKnockBack = true;
			}
			if (tag.Name == "FasterAttack100" && tag.Enable)
			{
				AttackSpeed += 1;
			}
			if (tag.Name == "FasterAttack200" && tag.Enable)
			{
				AttackSpeed += 2;
			}
			if (tag.Name == "RemoveTopPlatform" && tag.Enable)
			{
				int y = 170;
				for (int x = 20; x < Main.maxTilesX - 20; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
			}
			if (tag.Name == "RemoveBottomPlatform" && tag.Enable)
			{
				int y = 185;
				for (int x = 20; x < Main.maxTilesX - 20; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
			}
			if (tag.Name == "RemoveLeftPlatform" && tag.Enable)
			{
				int y = 170;
				for (int x = 20; x < Main.maxTilesX / 2; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
				y = 185;
				for (int x = 20; x < Main.maxTilesX / 2; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
			}
			if (tag.Name == "RemoveRightPlatform" && tag.Enable)
			{
				int y = 170;
				for (int x = Main.maxTilesX / 2; x < Main.maxTilesX - 20; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
				y = 185;
				for (int x = Main.maxTilesX / 2; x < Main.maxTilesX - 20; x++)
				{
					var tile = TileUtils.SafeGetTile(x, y);
					tile.TileType = TileID.Dirt;
					tile.HasTile = false;
				}
			}
			if (tag.Name == "180Seconds" && tag.Enable)
			{
				FailTime = 10800;
			}
			if (tag.Name == "90Seconds" && tag.Enable)
			{
				FailTime = 5400;
			}
			if (tag.Name == "15Hits" && tag.Enable)
			{
				MaxHitPlayerCount = 15;
			}
			if (tag.Name == "5Hits" && tag.Enable)
			{
				MaxHitPlayerCount = 5;
			}
		}
	}

	public virtual void StartFighting()
	{
		ApplyBossTags();
		NPC.life = NPC.lifeMax;
		NPC.dontTakeDamage = false;
		NPC.friendly = false;
		BossTimer = 0;
		StartedFight = true;
		NPCBossValueBar nBVB = new NPCBossValueBar();
		nBVB.Active = true;
		nBVB.Visible = true;
		nBVB.TargetBoss = NPC;
		Ins.VFXManager.Add(nBVB);
	}

	public virtual void BossAI()
	{
		NPC.TargetClosest(false);
		if (StartedFight)
		{
			if (!Fail)
			{
				BossTimer++;
			}
			LifeTimer++;
			if (Fail)
			{
				NPC.velocity *= 0;
				NPC.dontTakeDamage = true;
				NPC.damage = 0;
				return;
			}
			if (((FailTime > 0 && BossTimer > FailTime) || (HitPlayerCount >= MaxHitPlayerCount && MaxHitPlayerCount > 0)) && !Fail)
			{
				PopFailVFX();
			}
			if (HitEffectTimer > 0)
			{
				HitEffectTimer--;
			}
			else
			{
				HitEffectTimer = 0;
			}
			if (LifeTimer % 6 == 0)
			{
				if (NPC.life < NPC.lifeMax)
				{
					NPC.life += LifeRegenPerS / 10;
				}
				else
				{
					NPC.life = NPC.lifeMax;
				}
			}
			Player target = Main.player[NPC.target];
			Vector2 distance = target.Center - NPC.Center;
			NPC.dontTakeDamage = false;
			if (!KnockOut)
			{
				if (ImmuneLower300)
				{
					if (distance.Length() < 300)
					{
						NPC.dontTakeDamage = true;
					}
				}

				if (ImmuneUpper300)
				{
					if (distance.Length() > 300)
					{
						NPC.dontTakeDamage = true;
					}
				}
			}
			if (BehaviorsCoroutines.Count <= 0)
			{
				Idle = true;
				if (Resilience <= 0)
				{
					Resilience = 0;
					BehaviorsCoroutines.Enqueue(new Coroutine(StopAndKnockOut(600)));
				}
			}
		}
		if (NPC.life != NPC.lifeMax && !NPC.dontTakeDamage)
		{
			ShouldDrawHealthBar = true;
		}
		else
		{
			ShouldDrawHealthBar = false;
		}
	}

	/// <summary>
	/// Walk, chase target player at most condition.
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> BossWalk(int time)
	{
		for (int t = 0; t < time; t++)
		{
			// Too far from home will trigger teleportation.
			bool safe = false;
			if (NPC.target >= 0)
			{
				safe = true;
			}
			if (!safe)
			{
				EndAIPiece();
				yield break;
			}
			Player player = Main.player[NPC.target];
			if (NPC.Center.X < player.Center.X - 10)
			{
				NPC.direction = 1;
			}
			if (NPC.Center.X > player.Center.X + 10)
			{
				NPC.direction = -1;
			}
			NPC.spriteDirection = NPC.direction;
			float speed = 1.8f * BossMovingSpeed;
			WalkFrame();
			NPC.velocity.X = NPC.direction * speed;
			if (!Collision.SolidCollision(NPC.Bottom - new Vector2(20, 0), 40, 20))
			{
				NPC.velocity.Y += 0.5f;
			}
			if (BossToPlayerDistanceLowerThan(200))
			{
				break;
			}
			if (Resilience <= 0)
			{
				break;
			}
			TryOpenDoor(NPC);
			TryCloseDoor(NPC);
			yield return new SkipThisFrame();
		}
		NPC.velocity.X *= 0;
		EndAIPiece();
	}

	/// <summary>
	/// When resilience lower or equal to 0, boss will be knocked out and start a recovery period. During this period, boss can not attack or chase any player.
	/// </summary>
	/// <param name="time"></param>
	/// <returns></returns>
	public virtual IEnumerator<ICoroutineInstruction> StopAndKnockOut(int time)
	{
		KnockOut = true;
		NPC.knockBackResist = 1;
		NPC.velocity *= 0f;
		int oldDamage = NPC.damage;
		KnockOutTimer = 0;
		for (int t = 0; t < time; t++)
		{
			if (t <= 60)
			{
				KnockOutTimer++;
			}
			KnockOutFrame();
			if (!Collision.SolidCollision(NPC.Bottom - new Vector2(20, 0), 40, 20))
			{
				NPC.velocity.Y += 0.5f;
			}
			NPC.velocity.X *= 0.9f;
			float value = t / (float)time;
			Resilience = (MathF.Sin((value - 0.5f) * MathHelper.Pi) + 1) * ResilienceMax * 0.5f;
			NPC.damage = 0;
			if (t >= time - 60)
			{
				KnockOutTimer--;
			}
			yield return new SkipThisFrame();
		}
		KnockOutTimer = 0;
		Resilience = ResilienceMax;
		NPC.velocity.X *= 0;
		NPC.knockBackResist = 1;
		NPC.damage = oldDamage;
		KnockOut = false;
		EndAIPiece();
	}

	public virtual void KnockOutFrame()
	{
	}

	public override void ModifyHitByItem(Player player, Item item, ref NPC.HitModifiers modifiers)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (!KnockOut)
			{
				Resilience -= item.knockBack;
			}
			if (DisableKnockBack && !KnockOut)
			{
				modifiers.Knockback *= 0;
			}
		}
		base.ModifyHitByItem(player, item, ref modifiers);
	}

	public override void ModifyHitByProjectile(Projectile projectile, ref NPC.HitModifiers modifiers)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			if (!KnockOut)
			{
				Resilience -= projectile.knockBack;
			}
			if (DisableKnockBack && !KnockOut)
			{
				modifiers.Knockback *= 0;
			}
		}
		base.ModifyHitByProjectile(projectile, ref modifiers);
	}

	public override void HitEffect(NPC.HitInfo hit)
	{
		HitEffectTimer = 10;
		base.HitEffect(hit);
	}

	public override void OnHitPlayer(Player target, Player.HurtInfo hurtInfo)
	{
		HitPlayerCount++;
		base.OnHitPlayer(target, hurtInfo);
	}

	public virtual bool BossToPlayerDistanceLowerThan(float value)
	{
		if (NPC.target < 0)
		{
			return false;
		}
		Player player = Main.player[NPC.target];
		if ((NPC.Center - player.Center).Length() < value)
		{
			return true;
		}
		return false;
	}

	public override void OnKill()
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			PopSuccessVFX();
		}
		base.OnKill();
	}

	public void PopSuccessVFX()
	{
		var sIB = new SettlementBackground
		{
			Active = true,
			Visible = true,
			Timer = 0,
			State = 0,
			BossNPC = NPC,
		};
		Ins.VFXManager.Add(sIB);
		var aSet = new ArenaSettlement
		{
			Active = true,
			Visible = true,
			Timer = 0,
			State = 0,
			BossNPC = NPC,
		};
		Ins.VFXManager.Add(aSet);
	}

	public void PopFailVFX()
	{
		if (!Fail)
		{
			var fIB = new SettlementBackground
			{
				Active = true,
				Visible = true,
				Timer = 0,
				State = 1,
				BossNPC = NPC,
			};
			Ins.VFXManager.Add(fIB);
			var aSet = new ArenaSettlement
			{
				Active = true,
				Visible = true,
				Timer = 0,
				State = 1,
				BossNPC = NPC,
			};
			Ins.VFXManager.Add(aSet);
			Fail = true;
		}
	}

	public Vector2 HealthBarPos = Vector2.Zero;

	public float HealthBarScale = 1f;

	public bool ShouldDrawHealthBar = false;

	public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			HealthBarPos = position;
			HealthBarScale = scale;
		}
		return true;
	}

	public override void PostDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		if (YggdrasilTownCentralSystem.InArena_YggdrasilTown())
		{
			SpriteBatchState sBS = GraphicsUtils.GetState(spriteBatch).Value;
			spriteBatch.End();
			spriteBatch.Begin(sBS);
			spriteBatch.sortMode = SpriteSortMode.Immediate;
			spriteBatch.blendState = BlendState.AlphaBlend;
			Effect hitShader = ModAsset.TownNPCHitEffect.Value;
			hitShader.CurrentTechnique.Passes[0].Apply();
			var hitColor = Color.White;
			if (KnockOut)
			{
				hitColor = Color.Red;
			}
			hitColor *= HitEffectTimer / 10f;

			PreDraw(spriteBatch, screenPos, hitColor);

			spriteBatch.End();
			spriteBatch.Begin(sBS);
		}
		base.PostDraw(spriteBatch, screenPos, drawColor);
	}
}
