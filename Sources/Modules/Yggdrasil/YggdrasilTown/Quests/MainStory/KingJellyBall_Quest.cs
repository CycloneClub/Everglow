using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;
using Everglow.Yggdrasil.YggdrasilTown.Items.SummonItems;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Biomes;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.MainStory;

public sealed class KingJellyBall_Quest : WorldQuestBase
{
	public override QuestType Type => QuestType.MainStory;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldReachObjective(p => p.InModBiome<JellyBallHotbedBiome>()))
			.Add(new WorldCollectItemObjective(ModContent.ItemType<JellyBallCube>(), 60))
			.Add(new WorldCollectItemObjective(ModContent.ItemType<JellyBallDaifuku>(), 1))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<KingJellyBall>(), 1));

		RewardItems.Add(new(ModContent.ItemType<GoodNeighborCoin>(), 100));
	}

	public override bool CanUnlock() => true;
}
