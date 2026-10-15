using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.Presentation.Adapters;

public static class WorldQuestActionAdapter
{
	public static IReadOnlyList<QuestAction> GetActions(WorldQuestBase quest)
	{
		ArgumentNullException.ThrowIfNull(quest);

		if (QuestHintRules.HasContent(quest.Hint))
		{
			return [];
		}

		var identity = new QuestIdentity(QuestSide.World, quest.Name, quest.Name);
		return WorldQuestActions.GetAvailableTypes(quest)
			.Select(type => new QuestAction(identity, type))
			.ToArray();
	}
}
