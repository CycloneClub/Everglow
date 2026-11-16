using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class Restauranteur_Quest2 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldCollectItemObjective(ModContent.ItemType<LampFruit>(), 12))
			// TODO: 灯果汁腌草鱼及配方尚未实现；灯果只检查持有，不消耗。
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));
	}

	private int GiverNpcType => ModContent.NPCType<Restauranteur>();

	public override QuestSourceBase Source => YggdrasilQuestSources.Restauranteur;

	public override bool CanUnlock() => YggdrasilWorldSystem.DownedKingJellyBall || YggdrasilWorldSystem.DownedSquamousShell;
}
