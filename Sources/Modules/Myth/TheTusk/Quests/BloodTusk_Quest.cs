using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Myth.TheTusk.NPCs.BloodTusk;
using Everglow.Myth.TheTusk.WorldGeneration;
using Everglow.Myth.TheTusk.Tiles;
using SubworldLibrary;

namespace Everglow.Myth.TheTusk.Quests;

public sealed class BloodTusk_Quest : WorldQuestBase
{
	public override QuestType Type => QuestType.SideStory;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldReachObjective(player => !SubworldSystem.IsActive<TuskWorld>()
				&& (TileUtils.ToNearestTypeOfTile(player.Center, ModContent.TileType<BloodyMossWheel>(), 40).Length() < 40 * 16
					|| TileUtils.ToNearestTypeOfTile(player.Center, ModContent.TileType<BloodyMossWheelFinished>(), 40).Length() < 40 * 16)))
			.Add(new WorldReachObjective(_ => SubworldSystem.IsActive<TuskWorld>()))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<BloodTusk>(), 1));

		RewardItems.Add(new(ItemID.GoldCoin, 80));
	}

	public override bool CanUnlock() => true;
}
