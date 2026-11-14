using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.KelpCurtain.NPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Restauranteur_Quest3 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<MossyThornTurtle>(), 1))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));
	}

	private int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => YggdrasilQuestSources.Restauranteur;

	public override bool CanUnlock() => YggdrasilWorldSystem.EnteredKelpCurtain;
}
