using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Ores;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class ProstheticMaintenanceQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldReachObjective(player => player.TalkNPC?.netID == GiverNpcType && HasCyanVineMaterialOrArmor(player)))
			.Add(new WorldTalkObjective(GiverNpcType))
			// TODO: 矿石交付数量、报酬未确定。数量明确后改为 WorldGiveObjective。
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType));
	}

	public override int GiverNpcType => ModContent.NPCType<InnKeeper>();

	public override QuestSourceBase Source => Georg;

	public override bool CanOffer(WorldQuestManager manager) => true;

	private static bool HasCyanVineMaterialOrArmor(Player player) =>
		player.HasItem(ModContent.ItemType<CyanVineOre>())
		|| player.HasItem(ModContent.ItemType<CyanVineBar>())
		|| player.armor.Take(3).Any(item => item.type == ModContent.ItemType<CyanWarhelm>()
			|| item.type == ModContent.ItemType<CyanHeavylet>()
			|| item.type == ModContent.ItemType<CyanBreastplate>()
			|| item.type == ModContent.ItemType<CyanLeggings>());
}
