using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Example.Quest;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class QuestIconTest : ExamplePlayerQuest
{
	public override string DisplayName => GetType().Name;

	public override QuestType Type => QuestType.MainStory;
}
