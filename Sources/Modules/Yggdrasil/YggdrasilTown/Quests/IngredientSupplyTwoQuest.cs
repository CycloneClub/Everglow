using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.KelpCurtain.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class IngredientSupplyTwoQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<MossyThornTurtle>(), 1))
			.Add(new WorldTalkObjective(GiverNpcType));
	}

	public override int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => Rolle;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.EnteredKelpCurtain;
}
