using Everglow.Commons.DataStructures;
using Everglow.Commons.Utilities;
using Everglow.Commons.Mechanics.Events;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.Quests.Request;
using Terraria.Chat;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.Localization;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown.Events;

/// <summary>
/// A small, world-local invasion. Only kills of enemies spawned by this run count.
/// Spawn area uses world pixels and only limits spawning. Missing enemies are replaced rather than credited.
/// </summary>
public sealed class DrunkenMinerInvasion : ModEvent
{
	private readonly HashSet<NPC> enemies = [];
	private int spawnTimer;

	public override bool Networked => true;

	internal const int StageCount = 4;
	private const int SpawnInterval = 60;
	private const string LocalizationKey = "Mods.Everglow.Events.DrunkenMinerInvasion.";

	internal sealed class SpawnSource(string context) : EntitySource_Misc(context)
	{
	}

	internal readonly record struct StageDefinition(int PrimaryType, int PrimaryCount, int SecondaryType, int SecondaryCount, int MaxActiveEnemies, int Gold)
	{
		public int TargetCount => PrimaryCount + SecondaryCount;
	}

	internal static StageDefinition GetStage(int stage) => stage switch
	{
		0 => new(NPCID.Zombie, 12, 0, 0, 4, 2),
		1 => new(ModContent.NPCType<UmbrellaTailThief>(), 10, 0, 0, 4, 2),
		2 => new(ModContent.NPCType<AncientPhantom_Spear>(), 4, ModContent.NPCType<AncientPhantom_Bow>(), 4, 3, 3),
		3 => new(ModContent.NPCType<CrimsonSpell>(), 3, 0, 0, 1, 3),
		_ => throw new ArgumentOutOfRangeException(nameof(stage)),
	};

	private static Rectangle SpawnArea
	{
		get
		{
			if (YggdrasilTownBiome.BiomeCenter == Vector2.Zero)
			{
				YggdrasilTownBiome.BiomeCenter = YggdrasilTownBiome.GetBiomeCenter();
			}
			Rectangle town = YggdrasilTownCentralSystem.TownArea;
			return new Rectangle(town.X * 16, town.Y * 16, town.Width * 16, town.Height * 16);
		}
	}

	public int Stage { get; private set; } = -1;

	public int DefeatedPrimary { get; private set; }

	public int DefeatedSecondary { get; private set; }

	public int DefeatedEnemies => DefeatedPrimary + DefeatedSecondary;

	public int TargetCount => Stage < 0 ? 0 : GetStage(Stage).TargetCount;

	public bool Completed => TargetCount > 0 && DefeatedEnemies >= TargetCount;

	public bool Downed => Stage > 0 || (Stage == 0 && Completed);

	private string StageNameKey => LocalizationKey + $"Stage{Stage + 1}Name";

	public override bool CanActivate(params object[] args)
	{
		int stage = args.Length > 0 && args[0] is int requested ? requested : 0;
		return YggdrasilWorld.InYggdrasil && !Main.dayTime && base.CanActivate(args)
			&& (uint)stage < StageCount && (stage > Stage || (stage == Stage && !Completed));
	}

	public override void OnActivate(params object[] args)
	{
		int stage = args.Length > 0 && args[0] is int requested ? requested : 0;
		if (Stage != stage)
		{
			Stage = stage;
			DefeatedPrimary = 0;
			DefeatedSecondary = 0;
		}
		StageDefinition definition = GetStage(stage);
		if (args.Length == 3 && args[1] is int primary && args[2] is int secondary)
		{
			DefeatedPrimary = Math.Clamp(primary, DefeatedPrimary, definition.PrimaryCount);
			DefeatedSecondary = Math.Clamp(secondary, DefeatedSecondary, definition.SecondaryCount);
		}
		spawnTimer = 0;
		Announce("StartMessage");
	}

	private void Announce(string message)
	{
		if (Main.netMode == NetmodeID.SinglePlayer)
		{
			Main.NewText(Language.GetTextValue(LocalizationKey + message, Language.GetTextValue(StageNameKey)), 175, 75, 255);
		}
		else if (Main.netMode == NetmodeID.Server)
		{
			ChatHelper.BroadcastChatMessage(NetworkText.FromKey(LocalizationKey + message, NetworkText.FromKey(StageNameKey)), new Color(175, 75, 255));
		}
	}

	public override void Draw(SpriteBatch sprite)
	{
		if (Main.dedServ || !Active || TargetCount <= 0)
		{
			return;
		}

		// Use the vanilla invasion layout without changing event state during drawing.
		using var scope = (SpriteBatchState.Immediate with { TransformMatrix = Main.UIScaleMatrix }).BeginScope(sprite);
		Vector2 center = new(Main.screenWidth - 120f, Main.screenHeight - 40f);
		Utils.DrawInvBG(sprite, Utils.CenteredRectangle(center, new Vector2(200f, 45f)), new Color(63, 65, 151) * 0.785f);

		Texture2D colorBar = TextureAssets.ColorBar.Value;
		sprite.Draw(colorBar, center, null, Color.White, 0f, new Vector2(colorBar.Width / 2f, 0f), 1f, SpriteEffects.None, 0f);
		float progress = MathHelper.Clamp((float)DefeatedEnemies / TargetCount, 0f, 1f);
		const float barWidth = 169f;
		const float barHeight = 8f;
		Vector2 edge = center + new Vector2(1f + (progress - 0.5f) * barWidth, barHeight);
		Texture2D pixel = TextureAssets.MagicPixel.Value;
		Rectangle pixelSource = new(0, 0, 1, 1);
		sprite.Draw(pixel, edge, pixelSource, new Color(255, 241, 51), 0f, new Vector2(1f, 0.5f), new Vector2(barWidth * progress, barHeight), SpriteEffects.None, 0f);
		sprite.Draw(pixel, edge, pixelSource, Color.Black, 0f, new Vector2(0f, 0.5f), new Vector2(barWidth * (1f - progress), barHeight), SpriteEffects.None, 0f);
		sprite.Draw(pixel, edge, pixelSource, new Color(255, 165, 0, 127), 0f, new Vector2(1f, 0.5f), new Vector2(2f, barHeight), SpriteEffects.None, 0f);
		Utils.DrawBorderString(sprite, $"{DefeatedEnemies} / {TargetCount}", center + new Vector2(0f, 4f), Color.White, 1f, 0.5f, 1f);

		string name = Language.GetTextValue(StageNameKey);
		Vector2 nameSize = FontAssets.MouseText.Value.MeasureString(name);
		float nameScale = Math.Min(0.9f, 180f / Math.Max(1f, nameSize.X));
		Vector2 nameCenter = center - new Vector2(0f, 40f);
		Utils.DrawInvBG(sprite, Utils.CenteredRectangle(nameCenter, new Vector2(200f, 30f)), new Color(148, 122, 72) * 0.5f);
		Utils.DrawBorderString(sprite, name, nameCenter, Color.White, nameScale, 0.5f, 0.5f);
	}

	public override void PostUpdateEverything()
	{
		if (!YggdrasilWorld.InYggdrasil || Main.dayTime || Completed || Stage < 0)
		{
			EventSystem.Deactivate(this);
			return;
		}
		foreach (NPC npc in enemies.ToArray())
		{
			if (!npc.active || npc.life <= 0)
			{
				enemies.Remove(npc);
			}
		}
		if (++spawnTimer < SpawnInterval)
		{
			return;
		}
		spawnTimer = 0;
		StageDefinition definition = GetStage(Stage);
		if (enemies.Count >= definition.MaxActiveEnemies)
		{
			return;
		}
		int primaryRemaining = definition.PrimaryCount - DefeatedPrimary - enemies.Count(npc => npc.type == definition.PrimaryType);
		int secondaryRemaining = definition.SecondaryCount - DefeatedSecondary - enemies.Count(npc => npc.type == definition.SecondaryType);
		if (primaryRemaining <= 0 && secondaryRemaining <= 0)
		{
			return;
		}
		int enemyType = primaryRemaining >= secondaryRemaining ? definition.PrimaryType : definition.SecondaryType;

		Rectangle area = SpawnArea;
		Player player = null;
		foreach (Player candidate in Main.ActivePlayers)
		{
			if (!candidate.dead && area.Contains(candidate.Center.ToPoint()))
			{
				player = candidate;
				break;
			}
		}
		if (player is null)
		{
			return;
		}

		// Find an empty, grounded position near a player, confined to the event area.
		for (int attempt = 0; attempt < 12; attempt++)
		{
			int x = (int)player.Center.X + Main.rand.Next(320, 641) * (Main.rand.NextBool() ? 1 : -1);
			int y = (int)player.Bottom.Y - 160;
			for (int step = 0; step < 40; step++, y += 16)
			{
				int height = Stage == 3 ? 56 : 48;
				var bounds = new Rectangle(x - 16, y - height, 32, height);
				if (!area.Contains(bounds) || !WorldGen.InWorld(x / 16, y / 16, 10)
					|| Collision.SolidCollision(bounds.TopLeft(), bounds.Width, bounds.Height)
					|| !Collision.SolidCollision(new Vector2(x - 16, y), 32, 16))
				{
					continue;
				}
				int index = NPC.NewNPC(new SpawnSource(FullName), x, y, enemyType);
				if (index < Main.maxNPCs)
				{
					TrackEnemy(Main.npc[index]);
					Main.npc[index].netUpdate = true;
				}
				return;
			}
		}
	}

	private void TrackEnemy(NPC npc)
	{
		if (Active && Main.netMode != NetmodeID.MultiplayerClient)
		{
			enemies.Add(npc);
		}
	}

	public override void OnNPCKilled(NPC npc)
	{
		if (!Active || Main.netMode == NetmodeID.MultiplayerClient || !enemies.Remove(npc))
		{
			return;
		}
		StageDefinition definition = GetStage(Stage);
		if (npc.type == definition.PrimaryType && DefeatedPrimary < definition.PrimaryCount)
		{
			DefeatedPrimary++;
		}
		else if (npc.type == definition.SecondaryType && DefeatedSecondary < definition.SecondaryCount)
		{
			DefeatedSecondary++;
		}
		else
		{
			return;
		}

		WardenInvasionQuest.RecordInvasionProgress(Stage, DefeatedPrimary, DefeatedSecondary);
		if (Completed)
		{
			EventSystem.Deactivate(this);
			Announce("EndMessage");
		}
		else if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.WorldData);
		}
	}

	private void RemoveEnemy(NPC npc)
	{
		enemies.Remove(npc);
		npc.active = false;
		if (Main.netMode == NetmodeID.Server)
		{
			NetMessage.SendData(MessageID.SyncNPC, number: npc.whoAmI);
		}
	}

	public override void OnDeactivate(params object[] args)
	{
		foreach (NPC npc in enemies.ToArray())
		{
			RemoveEnemy(npc);
		}
	}

	public override void ClearWorld()
	{
		base.ClearWorld();
		enemies.Clear();
		spawnTimer = 0;
		Stage = -1;
		DefeatedPrimary = 0;
		DefeatedSecondary = 0;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag[nameof(Stage)] = Stage;
		tag[nameof(DefeatedPrimary)] = DefeatedPrimary;
		tag[nameof(DefeatedSecondary)] = DefeatedSecondary;
	}

	public override void LoadWorldData(string defName, TagCompound tag)
	{
		if (tag.ContainsKey(nameof(Stage)))
		{
			RestoreProgress(tag.GetInt(nameof(Stage)), tag.GetInt(nameof(DefeatedPrimary)), tag.GetInt(nameof(DefeatedSecondary)));
		}
		else if (tag.GetInt(nameof(TargetCount)) > 0)
		{
			int oldKills = tag.GetByte(nameof(Downed)) != 0 ? GetStage(0).PrimaryCount : tag.GetInt(nameof(DefeatedEnemies));
			RestoreProgress(0, oldKills, 0);
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(Stage);
		writer.Write(DefeatedPrimary);
		writer.Write(DefeatedSecondary);
	}

	public override void NetReceive(BinaryReader reader)
	{
		RestoreProgress(reader.ReadInt32(), reader.ReadInt32(), reader.ReadInt32());
	}

	private void RestoreProgress(int stage, int primary, int secondary)
	{
		Stage = (uint)stage < StageCount ? stage : -1;
		DefeatedPrimary = Stage < 0 ? 0 : Math.Clamp(primary, 0, GetStage(Stage).PrimaryCount);
		DefeatedSecondary = Stage < 0 ? 0 : Math.Clamp(secondary, 0, GetStage(Stage).SecondaryCount);
	}
}
