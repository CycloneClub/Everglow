using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class CancellableKillNPCQuestTest : PlayerQuestBase
{
	public CancellableKillNPCQuestTest()
	{
		var objective = new KillNPCObjective([NPCID.WallofFlesh], 1, true);
		Objectives.Add(objective);

		RewardItems.Add(new(ItemID.GoldBar, 1000));
	}

	public override bool Cancellable => true;

	public override string DisplayName => nameof(CancellableKillNPCQuestTest);

	public override QuestType Type => QuestType.Daily;
}
