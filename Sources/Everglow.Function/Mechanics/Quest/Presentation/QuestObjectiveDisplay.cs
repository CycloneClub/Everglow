using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

/// <summary>
/// Selects the stage shown in quest details without changing objective state or applying Hint masking.
/// </summary>
public static class QuestObjectiveDisplay
{
	public static IReadOnlyList<ObjectiveView> GetObjectives(QuestViewState state, IReadOnlyList<ObjectiveNodeView> nodes)
	{
		if (nodes.Count == 0)
		{
			return [];
		}

		bool preview = state is QuestViewState.Available or QuestViewState.Locked;
		ObjectiveNodeView node = preview ? nodes[0]
			: state == QuestViewState.Completed ? nodes[^1]
			: nodes.FirstOrDefault(node => !IsCompleted(node)) ?? nodes[^1];
		bool finished = state == QuestViewState.Completed || IsCompleted(node);

		IEnumerable<ObjectiveView> objectives = node switch
		{
			LeafObjectiveNodeView leaf => [leaf.Objective],
			ParallelObjectiveNodeView parallel => parallel.Objectives,
			AnyOfObjectiveNodeView anyOf => anyOf.Objectives.Where(objective =>
				preview || !IsCompleted(anyOf) || objective.State == ObjectiveViewState.Completed),
			BranchObjectiveNodeView branch => branch.Branches
				.Where(candidate => candidate.State != ObjectiveBranchState.Skipped)
				.SelectMany(candidate => preview ? candidate.Objectives.Take(1)
					: finished ? candidate.Objectives.TakeLast(1)
					: candidate.Objectives.SkipWhile(objective => objective.State == ObjectiveViewState.Completed).Take(1)),
			_ => [],
		};

		return objectives.Where(objective => objective.State != ObjectiveViewState.Skipped
			&& (preview || finished || objective.State != ObjectiveViewState.Completed)).ToArray();
	}

	private static bool IsCompleted(ObjectiveNodeView node) => node switch
	{
		LeafObjectiveNodeView leaf => leaf.Objective.State == ObjectiveViewState.Completed,
		ParallelObjectiveNodeView parallel => parallel.Objectives.All(objective => objective.State == ObjectiveViewState.Completed),
		AnyOfObjectiveNodeView anyOf => anyOf.Objectives.Any(objective => objective.State == ObjectiveViewState.Completed),
		BranchObjectiveNodeView branch => branch.Branches.Any(candidate =>
			candidate.State == ObjectiveBranchState.Selected
			&& candidate.Objectives.All(objective => objective.State == ObjectiveViewState.Completed)),
		_ => false,
	};
}
