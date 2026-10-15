using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Adapters;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.Presentation;

public sealed class QuestPresentationService
{
	private readonly PlayerQuestManager _playerManager;
	private readonly PlayerQuestActions _playerActions;
	private readonly WorldQuestManager _worldManager;
	private readonly WorldQuestActions _worldActions;

	public QuestPresentationService(
		PlayerQuestManager playerManager,
		PlayerQuestActions playerActions,
		WorldQuestManager worldManager,
		WorldQuestActions worldActions)
	{
		ArgumentNullException.ThrowIfNull(playerManager);
		ArgumentNullException.ThrowIfNull(playerActions);
		ArgumentNullException.ThrowIfNull(worldManager);
		ArgumentNullException.ThrowIfNull(worldActions);

		_playerManager = playerManager;
		_playerActions = playerActions;
		_worldManager = worldManager;
		_worldActions = worldActions;
	}

	public IReadOnlyList<QuestPresentationEntry> GetAll()
	{
		var entries = new List<QuestPresentationEntry>(_playerManager.Quests.Count + _worldManager.Quests.Count);
		foreach (PlayerQuestBase quest in _playerManager.Quests)
		{
			entries.Add(CreateEntry(quest));
		}
		foreach (WorldQuestBase quest in _worldManager.Quests)
		{
			entries.Add(CreateEntry(quest));
		}
		return entries.ToArray();
	}

	public bool TryGet(QuestIdentity identity, out QuestPresentationEntry entry)
	{
		entry = null;
		return identity.Side switch
		{
			QuestSide.Player => TryGetPlayer(identity, out entry),
			QuestSide.World => TryGetWorld(identity, out entry),
			_ => false,
		};
	}

	public bool TryExecute(QuestAction action) => action.Quest.Side switch
	{
		QuestSide.Player => _playerActions.TryExecute(action),
		QuestSide.World => _worldActions.TryExecute(action),
		_ => false,
	};

	private static QuestPresentationEntry CreateEntry(PlayerQuestBase quest) => new(
		PlayerQuestViewAdapter.Create(quest),
		PlayerQuestActionAdapter.GetActions(quest).ToArray());

	private static QuestPresentationEntry CreateEntry(WorldQuestBase quest) => new(
		WorldQuestViewAdapter.Create(quest),
		WorldQuestActionAdapter.GetActions(quest).ToArray());

	private bool TryGetPlayer(QuestIdentity identity, out QuestPresentationEntry entry)
	{
		entry = null;
		PlayerQuestBase quest = _playerManager.GetQuest(identity.DefinitionId);
		if (quest is null
			|| !string.Equals(quest.InstanceId, identity.InstanceId, StringComparison.Ordinal))
		{
			return false;
		}

		entry = CreateEntry(quest);
		return true;
	}

	private bool TryGetWorld(QuestIdentity identity, out QuestPresentationEntry entry)
	{
		entry = null;
		if (!string.Equals(identity.DefinitionId, identity.InstanceId, StringComparison.Ordinal))
		{
			return false;
		}

		WorldQuestBase quest = _worldManager.GetQuest(identity.DefinitionId);
		if (quest is null)
		{
			return false;
		}

		entry = CreateEntry(quest);
		return true;
	}
}
