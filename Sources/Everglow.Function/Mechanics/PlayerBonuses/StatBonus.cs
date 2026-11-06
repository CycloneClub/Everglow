namespace Everglow.Commons.Mechanics.PlayerBonuses;

/// <summary>
/// 玩家数值加成目标。
/// </summary>
public enum StatBonus
{
	/// <summary>
	/// 最大下落速度上限；取最小值，在原生普通加成之后生效。
	/// </summary>
	MaxFallSpeedCap,

	/// <summary>
	/// 特殊行为接管时的最大下落速度；取最大值，优先于缓降上限。
	/// </summary>
	MaxFallSpeedForSpecialBehavior,
}
