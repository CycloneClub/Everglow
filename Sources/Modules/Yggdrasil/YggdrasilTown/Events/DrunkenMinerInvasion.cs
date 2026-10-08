using Everglow.Commons.Mechanics.Events;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;
using Terraria.DataStructures;
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

	public override bool IsBackground => true;

	// 暂用原版僵尸验证流程；刷怪配置由该入侵自身维护。
	public const int EnemyType = NPCID.Zombie;
	private const int EnemyCount = 12;
	private const int MaxActiveEnemies = 4;
	private const int SpawnInterval = 60;

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

	/// <summary>Whether this invasion has been defeated in the current world.</summary>
	public bool Downed;

	public int DefeatedEnemies { get; private set; }

	public int TargetCount { get; private set; }

	public bool Completed => TargetCount > 0 && DefeatedEnemies >= TargetCount;

	public override bool CanActivate(params object[] args) => YggdrasilWorld.InYggdrasil && base.CanActivate(args);

	public override void OnActivate(params object[] args)
	{
		DefeatedEnemies = 0;
		TargetCount = EnemyCount;
		spawnTimer = 0;
	}

	public override void PostUpdateEverything()
	{
		if (!YggdrasilWorld.InYggdrasil)
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
		if (enemies.Count >= Math.Min(MaxActiveEnemies, TargetCount - DefeatedEnemies))
		{
			return;
		}

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
				var bounds = new Rectangle(x - 16, y - 48, 32, 48);
				if (!area.Contains(bounds) || !WorldGen.InWorld(x / 16, y / 16, 10)
					|| Collision.SolidCollision(bounds.TopLeft(), bounds.Width, bounds.Height)
					|| !Collision.SolidCollision(new Vector2(x - 16, y), 32, 16))
				{
					continue;
				}
				int index = NPC.NewNPC(new EntitySource_Misc(FullName), x, y, EnemyType);
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
		DefeatedEnemies++;
		if (Completed)
		{
			Downed = true;
			EventSystem.Deactivate(this);
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
		DefeatedEnemies = 0;
		TargetCount = 0;
		Downed = false;
	}

	public override void SaveWorldData(TagCompound tag)
	{
		tag[nameof(DefeatedEnemies)] = DefeatedEnemies;
		tag[nameof(TargetCount)] = TargetCount;
		tag[nameof(Downed)] = (byte)(Downed ? 1 : 0);
	}

	public override void LoadWorldData(string defName, TagCompound tag)
	{
		TargetCount = Math.Max(0, tag.GetInt(nameof(TargetCount)));
		DefeatedEnemies = Math.Clamp(tag.GetInt(nameof(DefeatedEnemies)), 0, TargetCount);
		Downed = tag.ContainsKey(nameof(Downed)) ? tag.GetByte(nameof(Downed)) != 0 : Completed;
	}

	public override void NetSend(BinaryWriter writer)
	{
		writer.Write(TargetCount);
		writer.Write(DefeatedEnemies);
		writer.Write(Downed);
	}

	public override void NetReceive(BinaryReader reader)
	{
		TargetCount = Math.Max(0, reader.ReadInt32());
		DefeatedEnemies = Math.Clamp(reader.ReadInt32(), 0, TargetCount);
		Downed = reader.ReadBoolean();
	}
}
