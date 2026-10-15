using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure.Nodes;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Adapters;

public static class WorldQuestViewAdapter
{
	public static QuestView Create(WorldQuestBase quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		// Use the quest name as the definition ID for world quests, since they don't have a separate definition ID.
		string definitionId = quest.Name;

		string hint = quest.Hint ?? string.Empty;
		float progress = ClampProgress(quest.Progress);
		int elapsedTime = quest.Time;
		int questTimeLimit = quest.TimeLimit;
		int? timeLimit = questTimeLimit <= 0 ? null : questTimeLimit;
		ObjectiveNodeView[] objectiveNodes = CreateObjectiveNodes(quest);
		RewardView[] rewards = CreateRewards(quest);

		QuestIconBase[] icons = CreateIcons(quest);

		return new QuestView
		{
			Identity = new QuestIdentity(QuestSide.World, definitionId, definitionId),
			Source = quest.Source ?? QuestSourceBase.Default,
			SubSource = null,
			Type = quest.Type,
			DisplayName = quest.DisplayName ?? string.Empty,
			Description = quest.Description ?? string.Empty,
			Hint = hint,
			Visible = quest.Visible,
			Icons = icons,
			State = MapState(quest.State),
			Progress = progress,
			ElapsedTime = elapsedTime,
			TimeLimit = timeLimit,
			ObjectiveNodes = objectiveNodes,
			Rewards = rewards,
		};
	}

	private static QuestIconBase[] CreateIcons(WorldQuestBase quest)
	{
		var iconGroup = new QuestIconGroup();
		iconGroup.Add(QuestSourceIcon.Create(quest.Source ?? QuestSourceBase.Default, null));
		quest.Objectives.GetObjectivesIcon(iconGroup);
		return iconGroup.Icons.ToArray();
	}

	private static ObjectiveNodeView[] CreateObjectiveNodes(WorldQuestBase quest)
	{
		var activeObjectives = new HashSet<WorldObjectiveBase>(ReferenceEqualityComparer.Instance);
		if (quest.State == WorldQuestState.Active)
		{
			activeObjectives.UnionWith(quest.Objectives.FindCurrentObjectives());
		}

		return quest.Objectives.AllNodes
			.Select(node => CreateObjectiveNode(quest, node, activeObjectives))
			.ToArray();
	}

	private static ObjectiveNodeView CreateObjectiveNode(
		WorldQuestBase quest,
		WorldObjectiveNodeBase node,
		IReadOnlySet<WorldObjectiveBase> activeObjectives)
	{
		return node switch
		{
			WorldLeafNode leaf => new LeafObjectiveNodeView(
				CreateObjective(quest, leaf.Objective, activeObjectives, skipped: false)),
			WorldParallelNode parallel => new ParallelObjectiveNodeView(
				parallel.Objectives
					.Select(objective => CreateObjective(quest, objective, activeObjectives, skipped: false))
					.ToArray()),
			WorldOptionalNode optional => new AnyOfObjectiveNodeView(
				optional.Objectives
					.Select(objective => CreateObjective(quest, objective, activeObjectives, skipped: false))
					.ToArray()),
			WorldBranchNode branch => CreateBranchNode(quest, branch, activeObjectives),
			_ => throw new InvalidDataException($"Unknown world objective node type {node.GetType().FullName}."),
		};
	}

	private static BranchObjectiveNodeView CreateBranchNode(
		WorldQuestBase quest,
		WorldBranchNode node,
		IReadOnlySet<WorldObjectiveBase> activeObjectives)
	{
		int? selectedBranchIndex = node.SelectedBranchIndex;
		var branches = new ObjectiveBranchView[node.Branches.Count];
		for (int branchIndex = 0; branchIndex < node.Branches.Count; branchIndex++)
		{
			ObjectiveBranchState state = selectedBranchIndex switch
			{
				null => ObjectiveBranchState.Candidate,
				int selected when selected == branchIndex => ObjectiveBranchState.Selected,
				_ => ObjectiveBranchState.Skipped,
			};
			bool skipped = state == ObjectiveBranchState.Skipped;
			ObjectiveView[] objectives = node.Branches[branchIndex]
				.Select(objective => CreateObjective(quest, objective, activeObjectives, skipped))
				.ToArray();
			branches[branchIndex] = new ObjectiveBranchView(state, objectives);
		}

		return new BranchObjectiveNodeView(branches);
	}

	private static ObjectiveView CreateObjective(
		WorldQuestBase quest,
		WorldObjectiveBase objective,
		IReadOnlySet<WorldObjectiveBase> activeObjectives,
		bool skipped)
	{
		ObjectiveViewState state;
		float progress;
		if (skipped)
		{
			state = ObjectiveViewState.Skipped;
			progress = 0f;
		}
		else if (objective.Completed)
		{
			state = ObjectiveViewState.Completed;
			progress = 1f;
		}
		else if (objective.IsTimedOut)
		{
			state = ObjectiveViewState.TimedOut;
			progress = ClampProgress(objective.Progress);
		}
		else
		{
			state = activeObjectives.Contains(objective)
				? ObjectiveViewState.Active
				: ObjectiveViewState.Pending;
			progress = ClampProgress(objective.Progress);
		}

		return new ObjectiveView
		{
			Id = objective.ObjectiveID,
			Description = objective.Description,
			ObjectiveText = objective.GetObjectiveText(),
			Progress = progress,
			State = state,
			CanRetry = quest.CanRetryObjective(objective.ObjectiveID),
			Timer = objective.Timer is null
				? null
				: new TimerView
				{
					TimeLimit = objective.Timer.TimeLimit,
					ElapsedTime = objective.Timer.ElapsedTime,
				},
		};
	}

	private static RewardView[] CreateRewards(WorldQuestBase quest)
	{
		List<Item> rewardItems = quest.RewardItems;
		return rewardItems is null
			? []
			: rewardItems.Select(item => new RewardView { Item = item }).ToArray();
	}

	private static QuestViewState MapState(WorldQuestState state)
	{
		return state switch
		{
			WorldQuestState.Locked => QuestViewState.Locked,
			WorldQuestState.Active => QuestViewState.Active,
			WorldQuestState.Completed => QuestViewState.Completed,
			WorldQuestState.Failed => QuestViewState.Failed,
			_ => throw new InvalidDataException($"Unknown world quest state {state}."),
		};
	}

	private static float ClampProgress(float progress) => float.IsNaN(progress)
		? 0f
		: Math.Clamp(progress, 0f, 1f);
}
