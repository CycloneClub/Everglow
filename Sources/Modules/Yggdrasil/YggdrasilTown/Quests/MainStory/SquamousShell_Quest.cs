using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.SquamousShell;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.MainStory;

public sealed class SquamousShell_Quest : WorldQuestBase
{
	public override QuestType Type => QuestType.MainStory;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldReachObjective(player => YggdrasilWorld.InYggdrasil
				&& TileUtils.ToNearestTypeOfTile(player.Center, ModContent.TileType<SquamousShellSeal>(), 10).Length() < 10 * 16))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<SquamousShell>(), 1));

		RewardItems.Add(new(ModContent.ItemType<GoodNeighborCoin>(), 150));
	}

	public override bool CanUnlock() => WorldQuestManager.Instance?.GetQuest<KingJellyBall_Quest>()?.State == WorldQuestState.Completed;
}
