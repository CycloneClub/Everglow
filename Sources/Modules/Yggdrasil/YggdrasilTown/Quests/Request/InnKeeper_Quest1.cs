using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class InnKeeper_Quest1 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldGiveObjective(GiverNpcType, ModContent.ItemType<CyanVineOre>(), 5)
				.WithDialogue(d => d
					.WithText("Dialogue.Intro")
					.WithCompletionText("Dialogue.Complete")));

		RewardItems.Add(new Item(ModContent.ItemType<GoodNeighborCoin>(), 10));
	}

	private int GiverNpcType => ModContent.NPCType<InnKeeper>();

	public override QuestSourceBase Source => YggdrasilQuestSources.InnKeeper;

	public override bool CanUnlock()
	{
		foreach (var player in Main.ActivePlayers)
		{
			if (player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<CyanVineOre>())
				|| player.HasItemInInventoryOrOpenVoidBag(ModContent.ItemType<CyanVineBar>()))
			{
				return true;
			}
		}

		return false;
	}
}
