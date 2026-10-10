using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class QuestIconTest : PlayerQuestBase
{
	public override string DisplayName => GetType().Name;

	public override QuestType Type => QuestType.MainStory;
}
