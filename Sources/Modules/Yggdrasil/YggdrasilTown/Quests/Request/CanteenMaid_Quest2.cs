using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class CanteenMaid_Quest2 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			// TODO: 策划未明确海盗类型、击杀数量及解锁条件。
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));
	}

	private int GiverNpcType => ModContent.NPCType<CanteenMaid>();

	public override QuestSourceBase Source => YggdrasilQuestSources.CanteenMaid;

	public override bool CanUnlock() => false;
}
