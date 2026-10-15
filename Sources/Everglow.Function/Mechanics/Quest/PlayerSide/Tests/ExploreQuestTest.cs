using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Terraria.GameContent.Personalities;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Tests;

public class ExploreQuestTest : PlayerQuestBase
{
	public override string DisplayName => nameof(ExploreQuestTest);

	public ExploreQuestTest()
	{
		Objectives.Add(new ExploreObjective(new ForestBiome(), 2000));
		RewardItems.Add(new Item(ItemID.Zenith, 100));
	}
}
