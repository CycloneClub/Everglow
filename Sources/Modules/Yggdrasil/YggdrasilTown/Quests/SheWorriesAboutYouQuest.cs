using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

public sealed class SheWorriesAboutYouQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldKillNPCObjective(ModContent.NPCType<CrimsonSpell>(), 1))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));
	}

	public override int GiverNpcType => ModContent.NPCType<Guard_of_YggdrasilTown>();

	public override QuestSourceBase Source => Anna;

	public override bool CanOffer(WorldQuestManager manager) =>
		manager.GetQuest<SmuggledAleQuest>()?.State == WorldQuestState.Completed;

	public override string Description => State == WorldQuestState.Completed
		? Text(Name + ".Dialogue.Objective2") : base.Description;
}
