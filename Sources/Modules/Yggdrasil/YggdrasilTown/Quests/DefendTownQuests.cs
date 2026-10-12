using Everglow.Commons.Mechanics.Events;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Events;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests;

// 四阶段属于同一任务；首阶段接入入侵，后续阶段继续等待内容实现。
public sealed class DefendTownQuest : TownNpcQuest
{
	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new DrunkenMinerInvasionObjective())
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType))
			.Add(new WorldReachObjective(_ => false))
			.Add(new WorldTalkObjective(GiverNpcType));
	}

	public override int GiverNpcType => ModContent.NPCType<Howard_Warden>();

	public override QuestSourceBase Source => Howard;

	public override bool CanOffer(WorldQuestManager manager) => YggdrasilWorldSystem.DownedSquamousShell;

	public bool IsFirstInvasionCompleted() => Objectives.AllNodes[1].Completed;

	// 阶段负责触发与进度映射；通关记录由入侵自身维护。
	private sealed class DrunkenMinerInvasionObjective : WorldObjectiveBase
	{
		private static DrunkenMinerInvasion Invasion => ModContent.GetInstance<DrunkenMinerInvasion>();

		private static bool Downed => YggdrasilWorld.InYggdrasil && Invasion.Downed;

		public bool Reached { get; private set; }

		public override float Progress => Reached || Downed ? 1f
			: YggdrasilWorld.InYggdrasil && Invasion is { Active: true, TargetCount: > 0 } invasion
				? (float)invasion.DefeatedEnemies / invasion.TargetCount : 0f;

		public override bool CheckCompletion() => Reached;

		public override void Update()
		{
			if (Main.netMode == NetmodeID.MultiplayerClient || Completed || IsTimedOut)
			{
				return;
			}
			if (Downed)
			{
				if (NetUtils.IsSingle || NetUtils.IsMainServer)
				{
					Reached = true;
				}
				else
				{
					NeedDeltaSync = !Reached;
				}
			}
			else if (!Reached && YggdrasilWorld.InYggdrasil && !Main.dayTime && !Invasion.Active)
			{
				EventSystem.Activate(Invasion);
			}
		}

		public override void ResetProgress()
		{
			base.ResetProgress();
			Reached = false;
			NeedDeltaSync = false;
		}

		public override void GetObjectivesIcon(QuestIconGroup iconGroup) =>
			iconGroup.Add(NPCQuestIcon.Create(DrunkenMinerInvasion.EnemyType));

		public override string GetObjectiveText() => YggdrasilWorld.InYggdrasil && Invasion is { Active: true } invasion
			? $"{base.GetObjectiveText()} ({invasion.DefeatedEnemies}/{invasion.TargetCount})" : base.GetObjectiveText();

		public override void SaveData(TagCompound tag)
		{
			base.SaveData(tag);
			tag[nameof(Reached)] = Reached;
		}

		public override void LoadData(TagCompound tag)
		{
			base.LoadData(tag);
			Reached = tag.GetBool(nameof(Reached));
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			writer.Write(Reached);
		}

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			Reached = reader.ReadBoolean();
			NeedDeltaSync = false;
		}

		public override void SendDelta(BinaryWriter writer)
		{
			writer.Write(Downed);
			NeedDeltaSync = false;
		}

		public override void ReceiveDelta(BinaryReader reader) => Reached |= reader.ReadBoolean();

		public override void SendMain(BinaryWriter writer) => writer.Write(Reached);

		public override void ReceiveMain(BinaryReader reader)
		{
			Reached = reader.ReadBoolean();
			NeedDeltaSync = false;
		}
	}
}
