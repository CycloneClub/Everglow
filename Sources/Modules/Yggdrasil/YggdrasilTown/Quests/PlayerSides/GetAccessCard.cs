using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.PlayerSides;

public class GetAccessCard : PlayerQuestBase
{
	public override string DisplayName => GetType().Name;

	public GetAccessCard()
	{
		Objectives.Add(new CollectItemObjective([ModContent.ItemType<YggdrasilTownAccessCard>()], 1));
	}
}
