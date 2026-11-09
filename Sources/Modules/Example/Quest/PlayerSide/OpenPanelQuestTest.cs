using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class OpenPanelQuestTest : ExamplePlayerQuest
{
	public override string DisplayName => nameof(OpenPanelQuestTest);

	public override QuestType Type => QuestType.Challenge;
}
