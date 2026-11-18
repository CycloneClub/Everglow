using Everglow.Commons.Mechanics.Events;
using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Mechanics.Quest.WorldSide.Packets;
using Everglow.Commons.Netcode;
using Everglow.Commons.Utilities;
using Everglow.Yggdrasil.YggdrasilTown.Events;
using Everglow.Yggdrasil.YggdrasilTown.NPCs.TownNPCs;
using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown.Quests.Request;

public abstract class WardenInvasionQuest : WorldQuestBase
{
	public override QuestType Type => QuestType.Request;

	public override void Initialize()
	{
		Objectives
			.Add(new WorldTalkObjective(GiverNpcType).WithDialogue(d => d.WithText("Dialogue.Objective0")))
			.Add(new DrunkenMinerInvasionObjective(Stage))
			.Add(new WorldTalkObjective(GiverNpcType).WithDialogue(d => d.WithText("Dialogue.Objective2")));

		RewardItems.Add(new Item(ItemID.GoldCoin, DrunkenMinerInvasion.GetStage(Stage).Gold));
	}

	private int GiverNpcType => ModContent.NPCType<Howard_Warden>();

	public override QuestSourceBase Source => YggdrasilQuestSources.Warden;

	protected abstract int Stage { get; }

	internal static void RecordInvasionProgress(int stage, int primary, int secondary)
	{
		var manager = ModContent.GetInstance<WorldQuestSystem>()?.Manager;
		var quest = manager?.ActiveQuests.OfType<WardenInvasionQuest>().FirstOrDefault(candidate => candidate.Stage == stage);
		if (quest?.ActiveObjectives.SingleOrDefault() is not DrunkenMinerInvasionObjective objective
			|| !objective.RecordProgress(primary, secondary))
		{
			return;
		}
		if (NetUtils.IsSubServer)
		{
			ModIns.PacketResolver.Route(new ObjectiveDeltaSyncPacket_SubProgress(quest.Name, objective), RouteDestination.MainServer);
		}
	}

	private sealed class DrunkenMinerInvasionObjective : WorldObjectiveBase
	{
		private static DrunkenMinerInvasion Invasion => ModContent.GetInstance<DrunkenMinerInvasion>();

		private int defeatedPrimary;
		private int defeatedSecondary;

		public DrunkenMinerInvasionObjective(int stage)
		{
			Stage = stage;
		}

		public int Stage { get; }

		private DrunkenMinerInvasion.StageDefinition Definition => DrunkenMinerInvasion.GetStage(Stage);

		private (int Primary, int Secondary) DisplayProgress => YggdrasilWorld.InYggdrasil && Invasion.Stage == Stage
			? (Math.Max(defeatedPrimary, Invasion.DefeatedPrimary), Math.Max(defeatedSecondary, Invasion.DefeatedSecondary))
			: (defeatedPrimary, defeatedSecondary);

		public override float Progress
		{
			get
			{
				var (primary, secondary) = DisplayProgress;
				return Completed ? 1f : (float)(primary + secondary) / Definition.TargetCount;
			}
		}

		public override bool CheckCompletion() => defeatedPrimary + defeatedSecondary >= Definition.TargetCount;

		public override void Update()
		{
			if (NetUtils.IsClient || Completed || IsTimedOut || !YggdrasilWorld.InYggdrasil)
			{
				return;
			}
			if (Invasion.Stage == Stage)
			{
				if (NetUtils.IsSingle)
				{
					RecordProgress(Invasion.DefeatedPrimary, Invasion.DefeatedSecondary);
				}
				else
				{
					NeedDeltaSync = Invasion.DefeatedPrimary > defeatedPrimary || Invasion.DefeatedSecondary > defeatedSecondary;
				}
			}
			if (!CheckCompletion() && !Invasion.Active && !NeedDeltaSync)
			{
				EventSystem.Activate(Invasion, Stage, defeatedPrimary, defeatedSecondary);
			}
		}

		public bool RecordProgress(int primary, int secondary)
		{
			var definition = Definition;
			if ((uint)primary > definition.PrimaryCount || (uint)secondary > definition.SecondaryCount
				|| (primary <= defeatedPrimary && secondary <= defeatedSecondary))
			{
				return false;
			}
			if (NetUtils.IsSingle || NetUtils.IsMainServer)
			{
				defeatedPrimary = Math.Max(defeatedPrimary, primary);
				defeatedSecondary = Math.Max(defeatedSecondary, secondary);
			}
			else
			{
				NeedDeltaSync = true;
			}
			return true;
		}

		public override void ResetProgress()
		{
			base.ResetProgress();
			defeatedPrimary = 0;
			defeatedSecondary = 0;
			NeedDeltaSync = false;
		}

		public override void GetObjectivesIcon(QuestIconGroup iconGroup)
		{
			var definition = Definition;
			iconGroup.Add(NPCQuestIcon.Create(definition.PrimaryType));
			if (definition.SecondaryCount > 0)
			{
				iconGroup.Add(NPCQuestIcon.Create(definition.SecondaryType));
			}
		}

		public override string GetObjectiveText()
		{
			var (primary, secondary) = DisplayProgress;
			var definition = Definition;
			string progress = definition.SecondaryCount == 0
				? $"{primary}/{definition.PrimaryCount}"
				: $"{primary}/{definition.PrimaryCount}, {secondary}/{definition.SecondaryCount}";
			return $"{base.GetObjectiveText()} ({progress})";
		}

		public override void SaveData(TagCompound tag)
		{
			base.SaveData(tag);
			tag[nameof(defeatedPrimary)] = defeatedPrimary;
			tag[nameof(defeatedSecondary)] = defeatedSecondary;
		}

		public override void LoadData(TagCompound tag)
		{
			base.LoadData(tag);
			var definition = Definition;
			bool previouslyCompleted = Completed || (Stage == 0 && tag.GetBool("Reached"));
			defeatedPrimary = previouslyCompleted ? definition.PrimaryCount : Math.Clamp(tag.GetInt(nameof(defeatedPrimary)), 0, definition.PrimaryCount);
			defeatedSecondary = previouslyCompleted ? definition.SecondaryCount : Math.Clamp(tag.GetInt(nameof(defeatedSecondary)), 0, definition.SecondaryCount);
		}

		public override void NetSend(BinaryWriter writer)
		{
			base.NetSend(writer);
			SendMain(writer);
		}

		public override void NetReceive(BinaryReader reader)
		{
			base.NetReceive(reader);
			ReceiveMain(reader);
		}

		public override void SendDelta(BinaryWriter writer)
		{
			writer.Write(Invasion.Stage == Stage ? Invasion.DefeatedPrimary : 0);
			writer.Write(Invasion.Stage == Stage ? Invasion.DefeatedSecondary : 0);
			NeedDeltaSync = false;
		}

		public override void ReceiveDelta(BinaryReader reader) => RecordProgress(reader.ReadInt32(), reader.ReadInt32());

		public override void SendMain(BinaryWriter writer)
		{
			writer.Write(defeatedPrimary);
			writer.Write(defeatedSecondary);
		}

		public override void ReceiveMain(BinaryReader reader)
		{
			defeatedPrimary = reader.ReadInt32();
			defeatedSecondary = reader.ReadInt32();
			NeedDeltaSync = false;
		}
	}
}
