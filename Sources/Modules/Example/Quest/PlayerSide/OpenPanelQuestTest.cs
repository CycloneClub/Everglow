using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class OpenPanelQuestTest : PlayerQuestBase
{
	public override string DisplayName => nameof(OpenPanelQuestTest);

	public override QuestType Type => QuestType.Challenge;
}
