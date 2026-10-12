using Everglow.Commons.Mechanics.Events;

namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>世界公共策略；Density 接收格坐标，只调整原生生成频率和容量。</summary>
public sealed record SpawnWorldPolicy(string Id, Func<bool> IsActive, Func<Vector2, float> Density = null);

/// <summary>向 tML 自然生成钩子提供区域规则和公共策略；不独立调度，群体在原生自然生成成功后集中追加。</summary>
public sealed class SubworldSpawnSystem : ModSystem
{
	private readonly Dictionary<string, SpawnWorldPolicy> worlds = new(StringComparer.Ordinal);
	private readonly List<SpawnTable> tables = new();
	private string groupWorldId;
	private NPCSpawnInfo groupSpawnInfo;
	private List<SpawnGroupRule> groupRules;

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
			if (rule == null || rule.Type == 0 || !float.IsFinite(rule.Weight) || rule.Weight < 0 ||
				rule.MinGroupSize < 1 || rule.MaxGroupSize < rule.MinGroupSize || rule.MaxGroupSize > Main.maxNPCs)
			{
				throw new ArgumentException($"Invalid NPC type, weight or group size in {worldId}/{tableName}.", nameof(rules));
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
				if (rule.Weight > 0 && (rule.Condition?.Invoke(spawnInfo) ?? true))
				{
					pool.TryGetValue(rule.Type, out float weight);
					pool[rule.Type] = weight + rule.Weight;
					groupRules.Add(new(rule, table.InRegion));
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
		float density = world.Density?.Invoke(player.Center / 16f) ?? 1f;
		if (!float.IsFinite(density) || density <= 0)
		{
			spawnRate = int.MaxValue;
			maxSpawns = 0;
			return;
		}
		density = Math.Min(density, 1f);
		spawnRate = (int)Math.Clamp(Math.Ceiling(spawnRate / (double)density), 1, int.MaxValue);
		maxSpawns = Math.Max(0, (int)(maxSpawns * (double)density));
	}

	// 只保存同一次原生选怪所需的信息，不跨帧排队，也不附加到 NPC 或网络包。
	private void ClearGroupContext()
	{
		groupWorldId = null;
		groupSpawnInfo = default;
		groupRules = null;
	}

	internal SpawnGroupContext TakeSpawnGroup(int type, int tileX, int tileY, Func<double> weightRoll)
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
		var matches = candidates.Where(entry => entry.Rule.Type == type).ToArray();
		if (matches.Length == 0 || matches.All(entry => entry.Rule.MaxGroupSize == 1))
		{
			return null;
		}
		// tML 已按合并权重选定类型；这里只确定同类型多条有效规则对应的群体配置。
		double choice = matches.Length == 1 ? 0 : weightRoll() * matches.Sum(entry => (double)entry.Rule.Weight);
		foreach (var entry in matches)
		{
			if (choice < entry.Rule.Weight)
			{
				return entry.Rule.MaxGroupSize > 1 ? new(worldId, info, entry) : null;
			}
			choice -= entry.Rule.Weight;
		}
		return null;
	}
}
