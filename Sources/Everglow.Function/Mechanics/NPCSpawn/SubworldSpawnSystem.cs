using Everglow.Commons.Mechanics.Events;

namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>世界公共策略；CanSpawn 接收玩家中心的格坐标，仅区分禁刷与野外。</summary>
public sealed record SpawnWorldPolicy(string Id, Func<bool> IsActive, Func<Vector2, bool> CanSpawn = null);

/// <summary>向 tML 自然生成钩子提供区域规则和公共策略；不独立调度，群体在原生自然生成成功后集中追加。</summary>
public sealed class SubworldSpawnSystem : ModSystem
{
	// 参考肉前洞穴的补怪节奏：平均 2 秒一次机会，9 个 NPC 槽位。
	// 固定覆盖原版输入，不随昼夜、深度、城镇 NPC、蜡烛或药水改变。
	private const int WildernessSpawnRate = 120;
	private const int WildernessMaxSpawns = 9;

	private readonly Dictionary<string, SpawnWorldPolicy> worlds = new(StringComparer.Ordinal);
	private readonly List<SpawnTable> tables = new();
	private string groupWorldId;
	private NPCSpawnInfo groupSpawnInfo;
	private Dictionary<int, SpawnGroupRule> groupRules;

	private sealed record SpawnTable(string WorldId, string Name, SubworldSpawnRule[] Rules, Func<Vector2, bool> InRegion);

	public override void PostUpdateNPCs() => ClearGroupContext();

	public override void OnWorldUnload() => ClearGroupContext();

	public override void Unload()
	{
		worlds.Clear();
		tables.Clear();
		ClearGroupContext();
	}

	public static void RegisterWorld(SpawnWorldPolicy policy) => ModContent.GetInstance<SubworldSpawnSystem>().AddWorld(policy);

	public static void RegisterRules(string worldId, string tableName, IReadOnlyList<SubworldSpawnRule> rules, Func<Vector2, bool> inRegion) =>
		ModContent.GetInstance<SubworldSpawnSystem>().AddRules(worldId, tableName, rules, inRegion);

	internal void AddWorld(SpawnWorldPolicy policy)
	{
		ArgumentNullException.ThrowIfNull(policy);
		ArgumentException.ThrowIfNullOrWhiteSpace(policy.Id);
		ArgumentNullException.ThrowIfNull(policy.IsActive);
		if (!worlds.TryAdd(policy.Id, policy))
		{
			throw new InvalidOperationException($"Duplicate spawn world: {policy.Id}");
		}
	}

	internal void AddRules(string worldId, string tableName, IReadOnlyList<SubworldSpawnRule> rules, Func<Vector2, bool> inRegion)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(tableName);
		ArgumentNullException.ThrowIfNull(rules);
		ArgumentNullException.ThrowIfNull(inRegion);
		if (!worlds.ContainsKey(worldId) || tables.Any(table => table.WorldId == worldId && table.Name == tableName))
		{
			throw new InvalidOperationException($"Unknown world or duplicate spawn table: {worldId}/{tableName}");
		}
		foreach (var rule in rules)
		{
			if (rule == null || rule.Type == 0 || rule.SpawnChance == null ||
				rule.MinGroupSize < 1 || rule.MaxGroupSize < rule.MinGroupSize || rule.MaxGroupSize > Main.maxNPCs)
			{
				throw new ArgumentException($"Invalid NPC type, spawn chance or group size in {worldId}/{tableName}.", nameof(rules));
			}
		}
		tables.Add(new(worldId, tableName, rules.ToArray(), inRegion));
	}

	internal SpawnWorldPolicy FindActiveWorld()
	{
		SpawnWorldPolicy active = null;
		foreach (var world in worlds.Values)
		{
			if (!world.IsActive())
			{
				continue;
			}
			if (active != null)
			{
				throw new InvalidOperationException("Multiple spawn policies claim the active world.");
			}
			active = world;
		}
		return active;
	}

	internal void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
	{
		ClearGroupContext();
		var world = FindActiveWorld();
		if (world == null || EventSystem.HasActiveSpawnOverride)
		{
			return;
		}
		// 清除本次已有的原版入口和模组候选；后续其他模组钩子仍可修改池。
		pool.Clear();
		groupWorldId = world.Id;
		groupSpawnInfo = spawnInfo;
		groupRules = new();
		var position = new Vector2(spawnInfo.SpawnTileX * 16 + 8, spawnInfo.SpawnTileY * 16);
		foreach (var table in tables)
		{
			if (table.WorldId != world.Id || !table.InRegion(position))
			{
				continue;
			}
			foreach (var rule in table.Rules)
			{
				float weight = rule.SpawnChance(spawnInfo);
				if (!float.IsFinite(weight) || weight < 0)
				{
					throw new InvalidOperationException($"Invalid spawn weight for NPC {rule.Type} in {world.Id}/{table.Name}.");
				}
				// 同类型只保留最高权重及其完整规则；同权重时保留先注册的规则。
				if (weight > 0 && (!pool.TryGetValue(rule.Type, out float existingWeight) || weight > existingWeight))
				{
					pool[rule.Type] = weight;
					groupRules[rule.Type] = new(rule, table.InRegion);
				}
			}
		}
	}

	internal void EditSpawnRate(Player player, ref int spawnRate, ref int maxSpawns)
	{
		var world = FindActiveWorld();
		if (world == null || EventSystem.HasActiveSpawnOverride)
		{
			return;
		}
		if (!(world.CanSpawn?.Invoke(player.Center / 16f) ?? true))
		{
			spawnRate = int.MaxValue;
			maxSpawns = 0;
			return;
		}
		spawnRate = WildernessSpawnRate;
		maxSpawns = WildernessMaxSpawns;
	}

	// 只保存同一次原生选怪所需的信息，不跨帧排队，也不附加到 NPC 或网络包。
	private void ClearGroupContext()
	{
		groupWorldId = null;
		groupSpawnInfo = default;
		groupRules = null;
	}

	internal SpawnGroupContext TakeSpawnGroup(int type, int tileX, int tileY)
	{
		string worldId = groupWorldId;
		NPCSpawnInfo info = groupSpawnInfo;
		var candidates = groupRules;
		ClearGroupContext();
		var world = FindActiveWorld();
		if (EventSystem.HasActiveSpawnOverride || world == null || world.Id != worldId ||
			info.SpawnTileX != tileX || info.SpawnTileY != tileY)
		{
			return null;
		}
		if (!candidates.TryGetValue(type, out var entry) || entry.Rule.MaxGroupSize == 1)
		{
			return null;
		}
		return new(worldId, info, entry);
	}
}
