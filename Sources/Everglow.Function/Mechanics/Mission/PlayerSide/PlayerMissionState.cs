namespace Everglow.Commons.Mechanics.Mission.PlayerSide;

/// <summary>
/// 任务池类型
/// <list type="table">
///     <item>Accepted: 已接取</item>
///     <item>Available: 可接取</item>
///     <item>Completed: 已完成</item>
///     <item>Overdue: 已过期</item>
///     <item>Failed: 已失败</item>
/// </list>
/// </summary>
public enum PlayerMissionState
{
	/// <summary>
	/// 已经被接取
	/// </summary>
	Accepted,

	/// <summary>
	/// 可以被接取
	/// </summary>
	Available,

	/// <summary>
	/// 任务失败
	/// </summary>
	Failed,

	/// <summary>
	/// 逾期未完成
	/// </summary>
	Overdue,

	/// <summary>
	/// 任务完成且已领取奖励
	/// </summary>
	Completed,
}
