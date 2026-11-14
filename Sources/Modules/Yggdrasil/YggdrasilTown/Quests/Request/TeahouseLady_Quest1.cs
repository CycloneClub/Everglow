using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class TeahouseLady_Quest1 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			// TODO: 无主肉傀尚无 NPC 实现，不能用无关怪物完成这个目标。
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));
	}

	private int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => YggdrasilQuestSources.TeahouseLady;

	public override bool CanUnlock() => true;
}
