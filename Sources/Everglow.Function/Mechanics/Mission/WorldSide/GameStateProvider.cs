namespace Everglow.Commons.Mechanics.Mission.WorldSide;

public interface IGameStateProvider
{
	double TimeForVisualEffects { get; }

	bool GameMenu { get; }

	bool GameInactive { get; }

	bool GamePaused { get; }
}

public class GameStateProvider
{
	public static readonly IGameStateProvider Default = new DefaultProvider();

	private class DefaultProvider : IGameStateProvider
	{
		public double TimeForVisualEffects => Main.timeForVisualEffects;

		public bool GameMenu => Main.gameMenu;

		public bool GameInactive => Main.gameInactive;

		public bool GamePaused => Main.gamePaused;
	}
}
