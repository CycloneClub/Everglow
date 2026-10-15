namespace Everglow.Commons.Mechanics.Quest.Presentation.Views;

public sealed class TimerView
{
	public int TimeLimit { get; init; }

	public int ElapsedTime { get; init; }

	public int RemainingTime => Math.Max(0, TimeLimit - ElapsedTime);
}
