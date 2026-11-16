using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

namespace Everglow.Example.Quest;

[DebugOnlyQuest]
public abstract class ExamplePlayerQuest : PlayerQuestBase
{
	public override string LocalizationKey => $"Mods.Everglow.Examples.{Name}";
}

[DebugOnlyQuest]
public abstract class ExampleWorldQuest : WorldQuestBase
{
	public override string LocalizationKey => $"Mods.Everglow.Examples.{Name}";
}
