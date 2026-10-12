using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class QuestTimerTest : PlayerQuestBase
{
	public QuestTimerTest()
	{
		Objectives.Add(new KillNPCObjective([NPCID.BlueSlime], 3, true).WithTimeLimit(10 * 60));
	}

	public override string DisplayName => GetType().Name;

	public override bool Cancellable => true;
}
