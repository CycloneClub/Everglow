namespace Everglow.Commons.Mechanics.Quest.Core;

public sealed class QuestTimer
{
	public QuestTimer(int timeLimit)
	{
		if (timeLimit <= 0)
		{
			throw new ArgumentOutOfRangeException(nameof(timeLimit), timeLimit, "Time limit must be positive.");
		}

		TimeLimit = timeLimit;
	}

	public int TimeLimit { get; }

	public int ElapsedTime { get; private set; }

	public int RemainingTime => Math.Max(0, TimeLimit - ElapsedTime);

	public bool IsExpired => ElapsedTime >= TimeLimit;

	public bool Update(int elapsedFrames)
	{
		if (elapsedFrames < 0)
		{
			throw new ArgumentOutOfRangeException(nameof(elapsedFrames), elapsedFrames, "Elapsed frames must not be negative.");
		}

		if (IsExpired)
		{
			return false;
		}

		ElapsedTime = (int)Math.Min((long)ElapsedTime + elapsedFrames, TimeLimit);
		return IsExpired;
	}

	public void Reset() => ElapsedTime = 0;

	internal void RestoreElapsedTime(int elapsedTime) => ElapsedTime = Math.Clamp(elapsedTime, 0, TimeLimit);
}
