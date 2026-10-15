namespace Everglow.Commons.Mechanics.Quest.Presentation.Views;

public abstract record ObjectiveNodeView;

public sealed record LeafObjectiveNodeView(ObjectiveView Objective)
	: ObjectiveNodeView;

public sealed record ParallelObjectiveNodeView(
	IReadOnlyList<ObjectiveView> Objectives)
	: ObjectiveNodeView;

public sealed record AnyOfObjectiveNodeView(
	IReadOnlyList<ObjectiveView> Objectives)
	: ObjectiveNodeView;

public sealed record ObjectiveBranchView(
	ObjectiveBranchState State,
	IReadOnlyList<ObjectiveView> Objectives);

public sealed record BranchObjectiveNodeView(
	IReadOnlyList<ObjectiveBranchView> Branches)
	: ObjectiveNodeView;
