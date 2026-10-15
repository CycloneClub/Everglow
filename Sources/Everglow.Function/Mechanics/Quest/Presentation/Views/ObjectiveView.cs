namespace Everglow.Commons.Mechanics.Quest.Presentation.Views;

public sealed class ObjectiveView
{
	public int Id { get; init; }

	public string Description { get; init; } = string.Empty;

	public string ObjectiveText { get; init; } = string.Empty;

	public float Progress { get; init; }

	public ObjectiveViewState State { get; init; }

	public bool CanRetry { get; init; }

	public TimerView Timer { get; init; }
}
