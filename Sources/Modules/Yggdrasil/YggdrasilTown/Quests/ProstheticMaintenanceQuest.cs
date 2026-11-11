using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class ProstheticMaintenanceQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldGiveObjective(GiverNpcType, ModContent.ItemType<CyanVineOre>(), 5)
				.WithDialogue(d => d
					.WithText("Dialogue.Intro")
					.WithCompletionText("Dialogue.Complete")));

		RewardItems.Add(new Item(ModContent.ItemType<GoodNeighborCoin>(), 10));
	}

	public override int GiverNpcType => ModContent.NPCType<InnKeeper>();

	public override QuestSourceBase Source => Georg;

	public override bool CanOffer(WorldQuestManager manager)
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
