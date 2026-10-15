using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Adapters;

public static class PlayerQuestActionAdapter
{
	public static IReadOnlyList<QuestAction> GetActions(PlayerQuestBase quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		if (QuestHintRules.HasContent(quest.Hint))
		{
			return [];
		}

		var identity = new QuestIdentity(QuestSide.Player, quest.Name, quest.InstanceId);
		return PlayerQuestActions.GetAvailableTypes(quest)
			.Select(type => new QuestAction(identity, type))
			.ToArray();
	}
}
