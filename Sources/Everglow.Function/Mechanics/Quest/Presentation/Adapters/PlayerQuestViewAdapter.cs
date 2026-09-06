using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure.Nodes;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Adapters;

public static class PlayerQuestViewAdapter
{
	public static QuestView Create(PlayerQuestBase quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

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
			Identity = new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId),
			Source = quest.Source ?? QuestSourceBase.Default,
			SubSource = quest.SubSource,
			Type = quest.Type,
			DisplayName = quest.DisplayName ?? string.Empty,
			Description = quest.Description ?? string.Empty,
			Hint = hint,
			Visible = quest.IsVisible,
			Icons = icons,
			State = MapState(quest.State),
			Progress = progress,
			ElapsedTime = elapsedTime,
			TimeLimit = timeLimit,
			ObjectiveNodes = objectiveNodes,
			Rewards = rewards,
		};
	}

	private static QuestIconBase[] CreateIcons(PlayerQuestBase quest)
	{
		var iconGroup = new QuestIconGroup();
		iconGroup.Add(QuestSourceIcon.Create(quest.Source ?? QuestSourceBase.Default, quest.SubSource));
		quest.Objectives.GetObjectivesIcon(iconGroup);
		return iconGroup.Icons.ToArray();
	}

	private static ObjectiveNodeView[] CreateObjectiveNodes(PlayerQuestBase quest)
	{
		var activeObjectives = new HashSet<PlayerObjectiveBase>(ReferenceEqualityComparer.Instance);
		if (quest.State == PlayerQuestState.Accepted)
		{
			activeObjectives.UnionWith(quest.Objectives.FindCurrentObjectives());
		}

		return quest.Objectives.AllNodes
			.Select(node => CreateObjectiveNode(quest, node, activeObjectives))
			.ToArray();
	}

	private static ObjectiveNodeView CreateObjectiveNode(
		PlayerQuestBase quest,
		PlayerObjectiveNodeBase node,
		IReadOnlySet<PlayerObjectiveBase> activeObjectives)
	{
		return node switch
		{
			PlayerLeafNode leaf => new LeafObjectiveNodeView(
				CreateObjective(quest, leaf.Objective, activeObjectives, skipped: false)),
			PlayerParallelNode parallel => new ParallelObjectiveNodeView(
				parallel.Objectives
					.Select(objective => CreateObjective(quest, objective, activeObjectives, skipped: false))
					.ToArray()),
			PlayerOptionalNode optional => new AnyOfObjectiveNodeView(
				optional.Objectives
					.Select(objective => CreateObjective(quest, objective, activeObjectives, skipped: false))
					.ToArray()),
			PlayerBranchNode branch => CreateBranchNode(quest, branch, activeObjectives),
			_ => throw new InvalidDataException($"Unknown player objective node type {node.GetType().FullName}."),
		};
	}

	private static BranchObjectiveNodeView CreateBranchNode(
		PlayerQuestBase quest,
		PlayerBranchNode node,
		IReadOnlySet<PlayerObjectiveBase> activeObjectives)
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
		PlayerQuestBase quest,
		PlayerObjectiveBase objective,
		IReadOnlySet<PlayerObjectiveBase> activeObjectives,
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
			CanRetry = !QuestHintRules.HasContent(quest.Hint) && quest.CanRetryObjective(objective.ObjectiveID),
			Timer = objective.Timer is null
				? null
				: new TimerView
				{
					TimeLimit = objective.Timer.TimeLimit,
					ElapsedTime = objective.Timer.ElapsedTime,
				},
		};
	}

	private static RewardView[] CreateRewards(PlayerQuestBase quest)
	{
		List<Item> rewardItems = quest.RewardItems;
		return rewardItems is null
			? []
			: rewardItems.Select(item => new RewardView { Item = item }).ToArray();
	}

	private static QuestViewState MapState(PlayerQuestState state)
	{
		return state switch
		{
			PlayerQuestState.Available => QuestViewState.Available,
			PlayerQuestState.Accepted => QuestViewState.Active,
			PlayerQuestState.Completed => QuestViewState.Completed,
			PlayerQuestState.Failed => QuestViewState.Failed,
			_ => throw new InvalidDataException($"Unknown player quest state {state}."),
		};
	}

	private static float ClampProgress(float progress) => float.IsNaN(progress)
		? 0f
		: Math.Clamp(progress, 0f, 1f);
}
