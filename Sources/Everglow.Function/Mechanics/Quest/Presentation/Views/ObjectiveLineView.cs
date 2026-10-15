namespace Everglow.Commons.Mechanics.Quest.Presentation.Views;

public sealed record ObjectiveLineView(ObjectiveView Objective, string Text)
{
	public TimerView Timer => Objective.State is ObjectiveViewState.Completed or ObjectiveViewState.Skipped
		? null
		: Objective.Timer;
}
