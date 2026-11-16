using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Myth.TheFirefly.NPCs.Bosses;
using Everglow.Myth.TheFirefly.WorldGeneration;
using Everglow.Myth.TheFirefly.Tiles;
using Everglow.Myth.TheFirefly.Pylon;
using SubworldLibrary;

namespace Everglow.Myth.TheFirefly.Quests;

public sealed class CorruptMoth_Quest : WorldQuestBase
{
	public override QuestType Type => QuestType.SideStory;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldReachObjective(player => SubworldSystem.Current is null
				&& TileUtils.ToNearestTypeOfTile(player.Center, ModContent.TileType<ShabbyPylon>(), 25).Length() < 25 * 16))
			.Add(new WorldReachObjective(_ => SubworldSystem.IsActive<MothWorld>()))
			.Add(new WorldReachObjective(player => SubworldSystem.IsActive<MothWorld>()
				&& TileUtils.ToNearestTypeOfTile(player.Center, ModContent.TileType<DarkCocoonSpecial>(), 25).Length() < 25 * 16))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<CorruptMoth>(), 1));

		RewardItems.Add(new(ItemID.GoldCoin, 100));
	}

	public override bool CanUnlock() => NPC.downedBoss2;
}
