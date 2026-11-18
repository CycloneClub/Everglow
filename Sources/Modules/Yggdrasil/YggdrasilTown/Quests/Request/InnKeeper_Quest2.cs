using Everglow.Yggdrasil.KelpCurtain.Items.Placeables;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class InnKeeper_Quest2 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldGiveObjective(GiverNpcType, ModContent.ItemType<JadeLakeRedAlgae_Item>(), 5))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 2));
	}

	private int GiverNpcType => ModContent.NPCType<InnKeeper>();

	public override QuestSourceBase Source => YggdrasilQuestSources.InnKeeper;

	public override bool CanUnlock() => YggdrasilWorldSystem.EnteredKelpCurtain
		&& WorldQuestManager.Instance.GetQuest<InnKeeper_Quest1>()?.State == WorldQuestState.Completed;
}
