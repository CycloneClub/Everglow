using Everglow.Commons.Coroutines;
using Everglow.Myth.Acytaea.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Bestiary;
using Terraria.GameContent.Personalities;
using Terraria.Localization;
using static Everglow.Commons.Utilities.NPCUtils;

namespace Everglow.Myth.Acytaea.NPCs;

[AutoloadHead]
[Pipeline(typeof(NPPipeline), typeof(AcytaeaPipeline))]
public class Acytaea : VisualNPC
{
	private bool canDespawn = false;
	private int aiMainCount = 0;

	public CoroutineManager _townNPCBehaviorCoroutine = new CoroutineManager();
	public CoroutineManager _townNPCGeneralCoroutine = new CoroutineManager();
	public Queue<Coroutine> AICoroutines = new Queue<Coroutine>();
	public bool Idle = true;
	public bool Talking = false;
	public bool Sit = false;
	public int TextureStyle = 0;
	public int Attack0Cooling = 0;

	public override string HeadTexture => NPC.boss ?
		"Everglow/Myth/Acytaea/NPCs/Acytaea_Head_Boss" :
		"Everglow/Myth/Acytaea/NPCs/Acytaea_Head";

	public override void SetStaticDefaults()
	{
		Main.npcFrameCount[NPC.type] = 25;
		NPCID.Sets.ExtraFramesCount[NPC.type] = 9;
		NPCID.Sets.AttackFrameCount[NPC.type] = 4;
		NPCID.Sets.DangerDetectRange[NPC.type] = 400;
		NPCID.Sets.AttackType[NPC.type] = 2;
		NPCID.Sets.AttackTime[NPC.type] = 60;
		NPCID.Sets.AttackAverageChance[NPC.type] = 15;
		NPCID.Sets.ActsLikeTownNPC[Type] = true;
	}

	public override void SetDefaults()
	{
		NPC.townNPC = true;
		NPC.friendly = true;
		NPC.width = 18;
		NPC.height = 40;
		NPC.aiStyle = -1;
		NPC.damage = 100;
		NPC.defense = 100;
		NPC.lifeMax = 250;
		NPC.HitSound = SoundID.NPCHit1;
		NPC.DeathSound = SoundID.NPCDeath6;
		NPC.knockBackResist = 0.5f;
		NPC.boss = false;
		NPC.friendly = true;
		NPC.knockBackResist = 0;

		NPCHappiness NH = NPC.Happiness;
		NH.SetBiomeAffection<ForestBiome>((AffectionLevel)50);
		NH.SetBiomeAffection<SnowBiome>((AffectionLevel)70);
		NH.SetBiomeAffection<CrimsonBiome>((AffectionLevel)90);
		NH.SetBiomeAffection<CorruptionBiome>((AffectionLevel)90);
		NH.SetBiomeAffection<UndergroundBiome>((AffectionLevel)(-20));
		NH.SetBiomeAffection<DesertBiome>((AffectionLevel)20);
		NH.SetBiomeAffection<DungeonBiome>((AffectionLevel)(-50));
		NH.SetBiomeAffection<OceanBiome>((AffectionLevel)50);
		NH.SetBiomeAffection<JungleBiome>((AffectionLevel)30);
	}

	public override void SetBestiary(BestiaryDatabase database, BestiaryEntry bestiaryEntry)
	{
		// TODO hjson
		string tx1 = "我很喜欢这种鸟语花香的绿色树林";
		string tx2 = "去抓蝴蝶吗?";
		if (Language.ActiveCulture.Name == "en-US")
		{
			tx1 = "I love the forest with birds singing and flowers blooming";
			tx2 = "Catching butterflies?";
		}
		else if (Language.ActiveCulture.Name == "ru-RU")
		{
			tx1 = "Я люблю лес где птички поют и цветочки благоухают";
			tx2 = "Может половить бабочек?";
		}
		else
		{
			tx1 = "I love the forest with birds singing and flowers blooming";
			tx2 = "Catching butterflies?";
		}

		// We can use AddRange instead of calling Add multiple times in order to add multiple items at once
		bestiaryEntry.Info.AddRange(new IBestiaryInfoElement[]
		{
				// Sets the preferred biomes of this town NPC listed in the bestiary.
				// With Town NPCs, you usually set this to what biome it likes the most in regards to NPC happiness.
				BestiaryDatabaseNPCsPopulator.CommonTags.SpawnConditions.Biomes.Surface,

				// Sets your NPC's flavor text in the bestiary.
				new FlavorTextBestiaryInfoElement(tx1),
				new FlavorTextBestiaryInfoElement(tx2),
				new FlavorTextBestiaryInfoElement("Mods.Everglow.Bestiary.Acytaea"),
		});
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		return 0;
	}

	public override void OnSpawn(IEntitySource source)
	{
		_townNPCGeneralCoroutine.StartCoroutine(new Coroutine(AI_Main()));
		base.OnSpawn(source);
	}

	public override bool PreAI()
	{
		aiMainCount = 0;
		return base.PreAI();
	}

	public override void AI()
	{
		Idle = true;
		_townNPCBehaviorCoroutine.Update();
		_townNPCGeneralCoroutine.Update();
		if (!Talking)
		{
			if (Main.rand.NextBool(120) && AICoroutines.Count <= 1)
			{
				AICoroutines.Enqueue(new Coroutine(Walk(Main.rand.Next(60, 900))));
			}
			if (Main.rand.NextBool(120) && AICoroutines.Count <= 1)
			{
				AICoroutines.Enqueue(new Coroutine(Stand(Main.rand.Next(60, 900))));
			}
			if ((int)(Main.time * 0.1f) % 6 == 0)
			{
				if (AICoroutines.Count <= 1)
				{
					if (CanAttack0())
					{
						AICoroutines.Enqueue(new Coroutine(Attack0()));
					}
					if (CanAttack1())
					{
						AICoroutines.Enqueue(new Coroutine(Attack1()));
					}
				}
			}
		}
		else
		{
			if (!CheckTalkingPlayer())
			{
				Talking = false;
			}
			else if (Sit)
			{
				Sit = false;
				Idle = true;
				aiMainCount = 0;
				AICoroutines.Clear();
			}
		}

		if (Sit)
		{
			var tile = Main.tile[NPC.Center.ToTileCoordinates()];
			if (tile == null || !TileID.Sets.CanBeSatOnForNPCs[tile.type])
			{
				Sit = false;
				Idle = true;
				aiMainCount = 0;
				AICoroutines.Clear();
			}
		}

		if (aiMainCount == 0)
		{
			Idle = true;
			_townNPCGeneralCoroutine.StartCoroutine(new Coroutine(AI_Main()));
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

	public bool CheckInSnow()
	{
		for (int j = 0; j < Main.player.Length; j++)
		{
			if (Main.player[j].active && Main.player[j].ZoneSnow)
			{
				if ((Main.player[j].Center - NPC.Center).Length() < 2000)
				{
					return true;
				}
			}
		}
		return false;
	}

	public bool CheckTalkingPlayer()
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

	public bool CanAttack0()
	{
		if (Attack0Cooling > 0)
		{
			return false;
		}
		if (AICoroutines.Count > 1)
		{
			return false;
		}
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && (!npc.CountsAsACritter || CheckInSnow()))
			{
				Vector2 distance = npc.Center - NPC.Center;
				if (MathF.Abs(distance.X) < 240 && distance.Y < 0 && distance.Y > -300)
				{
					if (npc.Center.X > NPC.Center.X)
					{
						NPC.direction = 1;
					}
					else
					{
						NPC.direction = -1;
					}
					NPC.spriteDirection = NPC.direction;
					return true;
				}
			}
		}
		return false;
	}

	public int ChooseAttack0Direction()
	{
		int nearestDir = 1;
		float minDis = 300;
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && (!npc.CountsAsACritter || CheckInSnow()))
			{
				Vector2 distance = npc.Center - NPC.Center;
				if (MathF.Abs(distance.X) < 240 && distance.Y < 0 && distance.Y > -300 && distance.Length() < minDis)
				{
					minDis = distance.Length();
					if (npc.Center.X > NPC.Center.X)
					{
						nearestDir = 1;
					}
					else
					{
						nearestDir = -1;
					}
				}
			}
		}
		return nearestDir;
	}

	public bool CanAttack1()
	{
		foreach (var npc in Main.npc)
		{
			if (!npc.friendly && !npc.dontTakeDamage && npc.active && npc.life > 0 && npc.type != NPCID.TargetDummy && (!npc.CountsAsACritter || CheckInSnow()))
			{
				Vector2 distance = npc.Center - NPC.Center;
				if (MathF.Abs(distance.X) < 120 && MathF.Abs(distance.Y) < 50)
				{
					return true;
				}
			}
		}
		return false;
	}

	public IEnumerator<ICoroutineInstruction> AI_Main()
	{
		while (true)
		{
			aiMainCount++;
			if (AICoroutines.Count > 0 && Idle)
			{
				_townNPCBehaviorCoroutine.StartCoroutine(AICoroutines.First());
				Sit = false;
				Idle = false;
			}
			if (Attack0Cooling > 0)
			{
				Attack0Cooling--;
			}
			else
			{
				Attack0Cooling = 0;
			}
			if (aiMainCount >= 2)
			{
				yield break;
			}
			yield return new SkipThisFrame();
		}
	}

	public IEnumerator<ICoroutineInstruction> Walk(int time)
	{
		NPC.direction = ChooseDirection(NPC);
		for (int t = 0; t < time; t++)
		{
			NPC.spriteDirection = NPC.direction;
			NPC.velocity.X = NPC.direction * 1.2f;
			Idle = false;
			NPC.frameCounter += Math.Abs(NPC.velocity.X);
			if (NPC.frameCounter > 4)
			{
				NPC.frame.Y += 64;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y > 15 * 64)
			{
				NPC.frame.Y = 64;
			}
			if (!CanContinueWalk(NPC))
			{
				break;
			}
			if (Talking)
			{
				break;
			}
			if (CanAttack0())
			{
				AICoroutines.Enqueue(new Coroutine(Attack0()));
				break;
			}
			if (CanAttack1())
			{
				AICoroutines.Enqueue(new Coroutine(Attack1()));
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

	public IEnumerator<ICoroutineInstruction> Attack0()
	{
		Attack0Cooling = 600;
		TextureStyle = 1;
		NPC.frame = new Rectangle(0, 0, 96, 80);
		NPC.direction = ChooseAttack0Direction();
		for (int t = 0; t < 40; t++)
		{
			NPC.spriteDirection = NPC.direction;
			NPC.velocity *= 0;
			if (t == 6)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center + new Vector2(-NPC.direction * 25, -40), Vector2.zeroVector, ModContent.ProjectileType<Acytaea_ShineStar_Town>(), 0, 3, Main.myPlayer, NPC.whoAmI);
			}
			if (t == 12)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<AcytaeaScratch_TownNPC>(), 150, 3, Main.myPlayer, NPC.whoAmI);
			}
			Idle = false;
			NPC.frameCounter += 1;
			if (NPC.frameCounter > 5)
			{
				NPC.frame.Y += 80;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y > 8 * 80)
			{
				NPC.frame.Y = 0;
			}
			yield return new SkipThisFrame();
		}
		TextureStyle = 0;
		NPC.frame = new Rectangle(0, 0, 48, 64);
		yield return new WaitForFrames(16);
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> Attack1()
	{
		TextureStyle = 1;
		NPC.frame = new Rectangle(0, 9 * 80, 96, 80);
		for (int t = 0; t < 60; t++)
		{
			NPC.spriteDirection = NPC.direction;
			NPC.velocity *= 0;
			Idle = false;
			NPC.frameCounter += 1;
			if (t == 6)
			{
				Projectile proj = Projectile.NewProjectileDirect(NPC.GetSource_FromAI(), NPC.Center, Vector2.zeroVector, ModContent.ProjectileType<AcytaeaSword_projectile_TownNPC>(), 150, 3, Main.myPlayer, NPC.whoAmI);
			}
			if (NPC.frameCounter > 6)
			{
				NPC.frame.Y += 80;
				NPC.frameCounter = 0;
			}
			if (NPC.frame.Y >= 18 * 80)
			{
				NPC.frame.Y = 0;
			}
			yield return new SkipThisFrame();
		}
		TextureStyle = 0;
		NPC.direction *= -1;
		NPC.spriteDirection = NPC.direction;
		NPC.frame = new Rectangle(0, 0, 48, 64);
		EndAIPiece();
	}

	public IEnumerator<ICoroutineInstruction> Stand(int time)
	{
		for (int t = 0; t < time; t++)
		{
			NPC.spriteDirection = NPC.direction;
			NPC.velocity.X = 0;
			Idle = false;
			if (CanAttack0())
			{
				AICoroutines.Enqueue(new Coroutine(Attack0()));
				break;
			}
			if (CanAttack1())
			{
				AICoroutines.Enqueue(new Coroutine(Attack1()));
				break;
			}
			yield return new SkipThisFrame();
		}
		EndAIPiece();
	}

	public void EndAIPiece()
	{
		AICoroutines.Dequeue();
		Idle = true;
	}

	public override bool CanChat()
	{
		return true;
	}

	public override string GetChat()
	{
		Talking = true;

		// TODO Hjson 重写
		IList<string> list = new List<string>();
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			if (Main.dayTime)
			{
				list.Add("你在干嘛啊");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("对了,要好好感谢向导");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("其实服装师以前成就厉害的,暗影魔法大师呢");
				}

				list.Add("没事不要烦我");
				list.Add("干嘛!");
				list.Add("额，好吧，我承认我的脾气有点暴躁");
				list.Add("我觉得你是不是喜欢我");
				list.Add("你烦不烦啊!");
				list.Add("不就是砍了你一刀么,没什么大不了");
				list.Add("嘻嘻嘻");
			}
			else
			{
				list.Add("晚上就应该出去浪");
				list.Add("你在干嘛啊");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("对了,要好好感谢向导");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("其实服装师以前成就厉害的,暗影魔法大师呢");
				}

				list.Add("没事不要烦我");
				list.Add("干嘛!");
				list.Add("额,好吧,我承认我的脾气有点暴躁");
				list.Add("我觉得你是不是喜欢我");
				list.Add("你烦不烦啊!");
				list.Add("不就是砍了你一刀么,没什么大不了");
				list.Add("嘻嘻嘻");
			}
		}
		else if (Language.ActiveCulture.Name == "en-US")
		{
			if (Main.dayTime)
			{
				list.Add("What are you doing?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Btw, you should be thankful to the Guide");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("The Clothier was actually a Senior dark Mage");
				}

				list.Add("Don't bother me if you only tell nonsense");
				list.Add("What's up?");
				list.Add("Err, well, I am a little grumpy");
				list.Add("You love me, don't you?");
				list.Add("How boring you are!");
				list.Add("I just cut you open, not a big deal");
				list.Add("*Chunckle");
			}
			else
			{
				list.Add("YOLO, go on adventures at night");
				list.Add("What are you doing?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Btw, you should be thankful to the Guide");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("The Clothier was actually a Senior dark Mage");
				}

				list.Add("Don't bother me if you only tell nonsense");
				list.Add("What's up?");
				list.Add("Err, well, I am a little grumpy");
				list.Add("♥ Wanna eat me? ♥");
				list.Add("How boring you are!");
				list.Add("I just cut you open, not a big deal");
				list.Add("*Chunckle");
			}
		}
		else if (Language.ActiveCulture.Name == "ru-RU")
		{
			if (Main.dayTime)
			{
				list.Add("Что ты делаешь?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Кстати, ты должен быть благодарен Гиду");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("Портной на самом деле был Старшим Тёмным Магом");
				}

				list.Add("Не беспокойся меня если ты говоришь только глупости");
				list.Add("Как дела?");
				list.Add("Эээ, да, я немного сварливая");
				list.Add("Ты любишь меня, не так ли?");
				list.Add("Какой ты скучный!");
				list.Add("Я просто разрезала тебя, ничего страшного");
				list.Add("*Смеётся");
			}
			else
			{
				list.Add("Ты живёшь один раз, иди путешествовать ночью");
				list.Add("Что ты делаешь?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Кстати, ты должен быть благодарен Гиду");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("Портной на самом деле был Старшим Тёмным Магом");
				}

				list.Add("Не беспокойся меня если ты говоришь только глупости");
				list.Add("Как дела?");
				list.Add("Эээ, да, я немного сварливая");
				list.Add("♥ Хочешь съесть меня? ♥");
				list.Add("Какой ты скучный!");
				list.Add("Я просто разрезала тебя, ничего страшного");
				list.Add("*Смеётся");
			}
		}
		else
		{
			if (Main.dayTime)
			{
				list.Add("What are you doing?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Btw, you should be thankful to the Guide");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("The Clothier was actually a Senior dark Mage");
				}

				list.Add("Don't bother me if you only tell nonsense");
				list.Add("What's up?");
				list.Add("Err, well, I am a little grumpy");
				list.Add("You love me, don't you?");
				list.Add("How boring you are!");
				list.Add("I just cut you open, not a big deal");
				list.Add("*Chunckle");
			}
			else
			{
				list.Add("YOLO, go on adventures at night");
				list.Add("What are you doing?");
				if (NPC.CountNPCS(22) != 0)
				{
					list.Add("Btw, you should be thankful to the Guide");
				}

				if (NPC.CountNPCS(54) != 0)
				{
					list.Add("The Clothier was actually a Senior dark Mage");
				}

				list.Add("Don't bother me if you only tell nonsense");
				list.Add("What's up?");
				list.Add("Err, well, I am a little grumpy");
				list.Add("♥ Wanna eat me? ♥");
				list.Add("How boring you are!");
				list.Add("I just cut you open, not a big deal");
				list.Add("*Chunckle");
			}
		}
		return list[Main.rand.Next(list.Count)];
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		// TODO Hjson
		if (Language.ActiveCulture.Name == "zh-Hans")
		{
			button = Language.GetTextValue("挑战");
			button2 = Language.GetTextValue("帮助");
		}
		else
		{
			button = Language.GetTextValue("Challenge");
			button2 = Language.GetTextValue("Help");
		}
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		if (firstButton)
		{
			NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.Center.X, (int)NPC.Center.Y, ModContent.NPCType<Acytaea_Boss>());
			NPC.active = false;
		}
		else
		{
		}
	}

	public override void FindFrame(int frameHeight)
	{
		if (Idle || Talking)
		{
			if (Sit)
			{
				NPC.frame.Y = 1152;
			}
			else
			{
				NPC.frame.Y = 0;
			}
		}
		base.FindFrame(frameHeight);
	}

	public override void Draw()
	{
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		Texture2D texMain = ModAsset.Acytaea.Value;
		if (TextureStyle == 1)
		{
			texMain = ModAsset.Acytaea_Attack.Value;
		}
		Vector2 drawPos = NPC.Center - screenPos + new Vector2(0, NPC.height - NPC.frame.Height + 8) * 0.5f;
		Main.spriteBatch.Draw(texMain, drawPos, NPC.frame, drawColor, NPC.rotation, NPC.frame.Size() * 0.5f, NPC.scale, NPC.spriteDirection == -1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally, 0);

		// Point checkPoint = (NPC.Bottom + new Vector2(8 * NPC.direction, 8)).ToTileCoordinates() + new Point(NPC.direction, -1);
		// Tile tile = Main.tile[checkPoint];
		// Texture2D block = Commons.ModAsset.TileBlock.Value;
		// Main.spriteBatch.Draw(block, checkPoint.ToWorldCoordinates() - Main.screenPosition, null, new Color(1f, 0f, 0f, 0.5f), 0, block.Size() * 0.5f, 1, SpriteEffects.None, 0);
		return false;
	}

	public override bool CheckActive()
	{
		return canDespawn;
	}

	public override bool PreKill()
	{
		return base.PreKill();
	}

	public override void OnKill()
	{
		NPC.SetEventFlagCleared(ref DownedBossSystem.downedAcytaea, -1);
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.WorldData);
		}
	}
}
