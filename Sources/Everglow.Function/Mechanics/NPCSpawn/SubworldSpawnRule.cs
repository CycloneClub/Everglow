namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>原生自然生成候选；条件接收 tML 已选好位置的上下文，不负责创建 NPC。</summary>
public sealed class SubworldSpawnRule
{
	public int Type { get; init; }

	/// <summary>根据本次上下文返回相对权重；0 表示不参与，默认权重为 1。</summary>
	public Func<NPCSpawnInfo, float> SpawnChance { get; init; } = _ => 1f;

	public int MinGroupSize { get; init; } = 1;

	public int MaxGroupSize { get; init; } = 1;
}
