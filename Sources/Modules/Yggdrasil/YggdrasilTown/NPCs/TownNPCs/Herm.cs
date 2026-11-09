using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.NPCSpawn;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.Netcode;
using Everglow.Yggdrasil.YggdrasilTown.NPCSpawn;
using Everglow.Yggdrasil.YggdrasilTown.Quests;
using Terraria.GameContent.UI;
using Terraria.Localization;
using static Everglow.Yggdrasil.YggdrasilTown.Quests.LostMinerQuest;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

public sealed class Herm : ModNPC
{
	private enum AiSlot
	{
		InMine,
		HomeX,
		WalkTime,
	}

	private ref float HomeX => ref NPC.ai[(int)AiSlot.HomeX];

	private ref float WalkTime => ref NPC.ai[(int)AiSlot.WalkTime];

	public bool IsInMine => NPC.ai[(int)AiSlot.InMine] != 0;

	public override string LocalizationCategory => LocalizationUtils.Categories.TownNPCs;

	public override void SetStaticDefaults() => Main.npcFrameCount[Type] = 1;

	public override void SetDefaults()
	{
		NPC.width = 32;
		NPC.height = 42;
		NPC.lifeMax = 250;
		NPC.rarity = 1;
		NPC.friendly = true;
		NPC.dontTakeDamage = true;
		NPC.damage = 0;
		NPC.knockBackResist = 0;
		NPC.aiStyle = -1;
	}

	public override bool CanChat() => true;

	public override bool CheckActive() => false;

	public override bool CanGoToStatue(bool toKingStatue) => false;

	public override void AI()
	{
		var progress = LostMinerQuest.Instance;
		if (Main.netMode != NetmodeID.MultiplayerClient && (!YggdrasilWorld.InYggdrasil || progress is null
			|| (!IsInMine && progress.Stage >= HermStage.Searching)))
		{
			NPC.active = false;
			NPC.netUpdate = true;
			return;
		}
		if (IsInMine)
		{
			NPC.velocity.X = 0;
			return;
		}
		if (Main.netMode == NetmodeID.MultiplayerClient)
		{
			return;
		}
		if (HomeX == 0)
		{
			HomeX = NPC.Center.X;
		}
		WalkTime++;
		bool talking = false;
		foreach (Player player in Main.ActivePlayers)
		{
			talking |= !player.dead && player.talkNPC == NPC.whoAmI
				&& Vector2.DistanceSquared(player.Center, NPC.Center) <= 240 * 240;
		}
		if (talking && !progress.HasMet && WalkTime % 60 == 0)
		{
			// 原生交谈消息已同步到当前服务端；等待主服快照期间最多每秒通知一次。
			if (NetUtils.IsSingle)
			{
				LostMinerQuest.ApplyValidatedMeeting();
			}
			else
			{
				ModIns.PacketResolver.Route(new HermValidatedInteractionPacket(), RouteDestination.MainServer);
			}
		}
		if (talking || WalkTime % 360 < 180)
		{
			NPC.velocity.X = 0;
		}
		else
		{
			if (Math.Abs(NPC.Center.X - HomeX) > 64)
			{
				NPC.direction = NPC.Center.X > HomeX ? -1 : 1;
			}
			if (NPC.collideX)
			{
				NPC.direction *= -1;
			}
			NPC.velocity.X = NPC.direction * 0.4f;
			NPC.spriteDirection = NPC.direction;
		}
		if (WalkTime % 240 == 0)
		{
			NPC.netUpdate = true;
			if (progress is { HasMet: false } && !talking)
			{
				EmoteBubble.NewBubble(EmoteID.EmoteConfused, new WorldUIAnchor(NPC), 180);
			}
		}
	}

	public override float SpawnChance(NPCSpawnInfo spawnInfo)
	{
		if (!YggdrasilWorld.InYggdrasil || LostMinerQuest.Instance is not { } quest || NPC.AnyNPCs(Type)
			|| !SpawnConditions.OnGround(spawnInfo))
		{
			return 0;
		}
		bool inMine = quest.Stage < HermStage.Searching
			? YggdrasilTownSpawnConditions.InShallowMine(spawnInfo)
			: YggdrasilTownSpawnConditions.InDeepMine(spawnInfo);
		return inMine ? 0.05f : 0;
	}

	public override int SpawnNPC(int tileX, int tileY)
	{
		bool injured = LostMinerQuest.Instance?.Stage >= HermStage.Searching;
		return NPC.NewNPC(new Terraria.DataStructures.EntitySource_SpawnNPC(), tileX * 16 + 8, tileY * 16, Type,
			ai0: injured ? 1 : 0, ai1: tileX * 16 + 8);
	}

	public override string GetChat()
	{
		var progress = LostMinerQuest.Instance;
		// 单机 Auto Pause 会暂停 NPC AI，首次交谈必须在这里直接解锁。
		if (NetUtils.IsSingle && !IsInMine && progress is { HasMet: false })
		{
			LostMinerQuest.ApplyValidatedMeeting();
		}
		return this.GetLocalizedValue(IsInMine ? "Silence"
			: progress?.Stage == HermStage.CardIssued ? "Waiting" : "Greeting");
	}

	public override void SetChatButtons(ref string button, ref string button2)
	{
		QuestChat.SetChatButton(NPC, ref button);
	}

	public override void OnChatButtonClicked(bool firstButton, ref string shopName)
	{
		QuestChat.OnChatButtonClicked(NPC, firstButton, useSecondButton: false);
	}

	public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
	{
		var progress = LostMinerQuest.Instance;
		var stage = progress?.Stage;
		bool foundAlive = progress?.FoundAlive == true;
		bool dead = stage >= HermStage.LampRecovered || (stage == HermStage.Found && !foundAlive)
			|| (stage == HermStage.Searching && progress.SearchExpired);
		bool sitting = IsInMine && !dead;
		Texture2D texture = !IsInMine ? ModAsset.Herm.Value
			: sitting ? ModAsset.Herm_Sit.Value
			: foundAlive ? ModAsset.Herm_Down.Value
			: ModAsset.Herm_Corrupted.Value;
		var effects = NPC.spriteDirection < 0 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
		if (sitting)
		{
			Texture2D bucket = ModAsset.Herm_Bucket.Value;
			// 矿桶先画，放在坐姿身后，并随人物一起翻转。
			var offset = new Vector2(NPC.spriteDirection < 0 ? -12 : 12, 0);
			spriteBatch.Draw(bucket, NPC.Bottom + offset - screenPos, null, drawColor, 0,
				new Vector2(bucket.Width / 2f, bucket.Height), 1f, effects, 0);
		}
		spriteBatch.Draw(texture, NPC.Bottom - screenPos, null, drawColor, 0,
			new Vector2(texture.Width / 2f, texture.Height), 1f, effects, 0);
		return false;
	}
}
