using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class KillNPCQuestTest : PlayerQuestBase
{
	public KillNPCQuestTest()
	{
		var objective = new KillNPCObjective([NPCID.CursedSkull, NPCID.DemonEye], 10, true);
		Objectives.Add(objective);

		RewardItems.Add(new(ItemID.DirtBlock, 10));
	}

	public override QuestSourceBase Source => QuestSourceTest1.Instance;

	public override QuestSourceBase SubSource => QuestSourceTest2.Instance;

	public override string DisplayName => nameof(KillNPCQuestTest);

	public override QuestType Type => QuestType.MainStory;
}
