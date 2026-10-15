using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Quest.WorldSide;

public sealed class WorldQuestActions
{
	private readonly WorldQuestManager _manager;

	public WorldQuestActions(WorldQuestManager manager)
	{
		_manager = manager ?? throw new ArgumentNullException(nameof(manager));
	}

	public static IReadOnlyList<QuestActionType> GetAvailableTypes(WorldQuestBase quest)
	{
		return GetAvailableTypesForPlayer(quest, Main.LocalPlayer.name);
	}

	private static IReadOnlyList<QuestActionType> GetAvailableTypesForPlayer(WorldQuestBase quest, string playerName)
	{
		if (quest.State == WorldQuestState.Failed && quest.Retriable && NetUtils.IsSingle)
		{
			return [QuestActionType.Retry];
		}

		if (quest.CanClaimReward(playerName))
		{
			return [QuestActionType.ClaimReward];
		}

		return [];
	}

	public bool TryExecute(QuestAction action)
	{
		QuestIdentity identity = action.Quest;
		if (identity.Side != QuestSide.World
			|| !string.Equals(identity.DefinitionId, identity.InstanceId, StringComparison.Ordinal))
		{
			return false;
		}

		WorldQuestBase quest = _manager.GetQuest(identity.DefinitionId);
		if (quest is null
			|| !string.Equals(quest.Name, identity.InstanceId, StringComparison.Ordinal))
		{
			return false;
		}

		if (action is { Type: QuestActionType.Retry, Args: int objectiveId })
		{
			return TryRetryObjective(quest, objectiveId);
		}

		if (action.Args is not null)
		{
			return false;
		}

		string playerName = Main.LocalPlayer.name;
		if (QuestHintRules.HasContent(quest.Hint)
			|| !GetAvailableTypesForPlayer(quest, playerName).Contains(action.Type))
		{
			return false;
		}

		return action.Type switch
		{
			QuestActionType.Retry => TryRetry(quest),
			QuestActionType.ClaimReward => TryClaimReward(quest, playerName),
			_ => false,
		};
	}

	private bool TryRetryObjective(WorldQuestBase quest, int objectiveId)
	{
		if (!quest.CanRetryObjective(objectiveId))
		{
			return false;
		}

		if (NetUtils.IsSingle)
		{
			return _manager.TryRetryObjective(quest.Name, objectiveId);
		}

		if (NetUtils.IsClient)
		{
			ModIns.PacketResolver.Route(
				new ObjectiveRetryRequestPacket(quest.Name, objectiveId),
				RouteDestination.MainServer);
			return true;
		}

		return false;
	}

	private bool TryRetry(WorldQuestBase quest)
	{
		if (!quest.RetryCore())
		{
			return false;
		}

		_manager.OnQuestObjectiveUpdated(quest);
		_manager.OnQuestStatusUpdated(quest);
		return true;
	}

	private bool TryClaimReward(WorldQuestBase quest, string playerName)
	{
		if (NetUtils.IsSingle)
		{
			Player player = Main.LocalPlayer;
			if (!string.Equals(player.name, playerName, StringComparison.Ordinal)
				|| !quest.TryRecordRewardClaim(playerName))
			{
				return false;
			}

			quest.GiveRewards(player);
			_manager.OnQuestStatusUpdated(quest);
			return true;
		}
		else if (NetUtils.IsClient)
		{
			ModIns.PacketResolver.Route(
				new QuestClaimRewardPacket(quest.Name),
				RouteDestination.MainServer);

			return true;
		}

		return false;
	}
}
