namespace Everglow.Commons.Mechanics.Quest.PlayerSide;

/// <summary>
/// 任务状态
/// <list type="table">
///     <item>Accepted: 已接取</item>
///     <item>Available: 可接取</item>
///     <item>Completed: 已完成</item>
///     <item>Failed: 已失败</item>
/// </list>
/// </summary>
public enum PlayerQuestState
{
	/// <summary>
	/// 已经被接取
	/// </summary>
	Accepted = 0,

	/// <summary>
	/// 可以被接取
	/// </summary>
	Available = 1,

	/// <summary>
	/// 任务失败
	/// </summary>
	Failed = 2,

	/// <summary>
	/// 任务完成且已领取奖励
	/// </summary>
	// Value 3 belonged to the removed Overdue state and must not be reused.
	Completed = 4,
}
