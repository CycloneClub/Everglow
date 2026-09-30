using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// 四阶段属于同一任务；Talk 表达引导、回报与过渡，入侵目标等待事件内容实现。
public sealed class DefendTownQuest : TownNpcQuest
{
	private WorldReachObjective firstInvasion;

	public override void Initialize()
	{
		firstInvasion = new WorldReachObjective(_ => false, Text("DefendTownOneQuest.Objective"));
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType).WithDescription(Text(Name + ".IntroObjective")))
			.Add(firstInvasion.WithDescription(Text("DefendTownOneQuest.InvasionDescription")))
			.Add(new WorldTalkObjective(GiverNpcType).WithDescription(Text("DefendTownOneQuest.ReportDescription")))
			.Add(new WorldReachObjective(_ => false, Text("DefendTownTwoQuest.Objective"))
				.WithDescription(Text("DefendTownTwoQuest.InvasionDescription")))
			.Add(new WorldTalkObjective(GiverNpcType).WithDescription(Text("DefendTownTwoQuest.ReportDescription")))
			.Add(new WorldReachObjective(_ => false, Text("DefendTownThreeQuest.Objective"))
				.WithDescription(Text("DefendTownThreeQuest.InvasionDescription")))
			.Add(new WorldTalkObjective(GiverNpcType).WithDescription(Text("DefendTownThreeQuest.ReportDescription")))
			.Add(new WorldReachObjective(_ => false, Text("DefendTownFourQuest.Objective"))
				.WithDescription(Text("DefendTownFourQuest.InvasionDescription")))
			.Add(new WorldTalkObjective(GiverNpcType).WithDescription(Text("DefendTownFourQuest.ReportDescription")));
	}

	public bool FirstStageCompleted => firstInvasion?.Completed == true;

	public override int GiverNpcType => ModContent.NPCType<Howard_Warden>();

	public override QuestSourceBase Source => Howard;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.DownedSquamousShell;
}
