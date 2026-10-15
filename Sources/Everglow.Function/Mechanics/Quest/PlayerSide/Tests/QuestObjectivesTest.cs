using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class QuestObjectivesTest : PlayerQuestBase
{
	public QuestObjectivesTest()
	{
		var objective1 = new KillNPCObjective(
			[
				NPCID.BlueSlime,
				NPCID.IceSlime,
				NPCID.SpikedJungleSlime,
				NPCID.MotherSlime,
			], 10, true);

		var objective2 = new ConsumeItemObjective([ItemID.SpikyBall], 10);

		Objectives.Add(objective1);
		Objectives.Add(objective2);

		RewardItems.Add(new Item(ItemID.Zenith, 1000));
	}

	public override string DisplayName => nameof(QuestObjectivesTest);

	public override QuestType Type => QuestType.SideStory;

	public override bool Cancellable => true;
}
