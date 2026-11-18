using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class TeahouseLady_Quest1 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<RockElemental>(), 1))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 2));
	}

	private int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => YggdrasilQuestSources.TeahouseLady;

	public override bool CanUnlock() => true;
}
