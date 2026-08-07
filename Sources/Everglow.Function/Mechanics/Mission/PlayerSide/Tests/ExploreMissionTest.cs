using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;
using Terraria.GameContent.Personalities;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Tests;

public class ExploreMissionTest : PlayerMissionBase
{
	public override string DisplayName => nameof(ExploreMissionTest);

	public ExploreMissionTest()
	{
		Objectives.Add(new ExploreObjective(new ForestBiome(), 2000));
		RewardItems.Add(new Item(ItemID.Zenith, 100));
	}
}
