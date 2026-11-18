using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public sealed class TeahouseLady_Quest2 : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new WorldReachObjective(player => KelpCurtainBiome.IsKelpCurtainLayer(player)
				&& player.Center.X > Main.maxTilesX * 0.05f * 16
				&& player.Center.X < Main.maxTilesX * 0.75f * 16
				&& player.Center.Y > Main.maxTilesY * 0.87f * 16
				&& player.Center.Y < Main.maxTilesY * 0.9f * 16))
			.Add(new WorldTalkObjective(GiverNpcType)
				.WithDialogue(d => d.WithText("Dialogue.Objective2")));

		RewardItems.Add(new Item(ItemID.GoldCoin, 3));
	}

	private int GiverNpcType => ModContent.NPCType<TeahouseLady>();

	public override QuestSourceBase Source => YggdrasilQuestSources.TeahouseLady;

	public override bool CanUnlock() => YggdrasilWorldSystem.EnteredKelpCurtain
		&& WorldQuestManager.Instance.GetQuest<TeahouseLady_Quest1>()?.State == WorldQuestState.Completed;
}
