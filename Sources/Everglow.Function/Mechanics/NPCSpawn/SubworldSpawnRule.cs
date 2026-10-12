namespace Everglow.Commons.Mechanics.NPCSpawn;

/// <summary>原生自然生成候选；条件接收 tML 已选好位置的上下文，不负责创建 NPC。</summary>
public sealed class SubworldSpawnRule
{
	public int Type { get; init; }
	public float Weight { get; init; } = 1f;
	public Func<NPCSpawnInfo, bool> Condition { get; init; }
	public int MinGroupSize { get; init; } = 1;
	public int MaxGroupSize { get; init; } = 1;
}
