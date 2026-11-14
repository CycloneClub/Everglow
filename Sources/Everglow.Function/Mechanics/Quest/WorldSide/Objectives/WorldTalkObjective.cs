using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldTalkObjective : WorldObjectiveBase, IQuestDialogueObjective
{
	public WorldTalkObjective()
	{
	}

	public WorldTalkObjective(int npcType)
	{
		NPCType = npcType;
	}

	private bool localSubmitted;

	public ObjectiveDialogue Dialogue { get; set; }

	public bool Talked { get; private set; }

	public int NPCType { get; private set; }

	public override float Progress => Talked ? 1f : 0f;

	public override bool NeedDeltaSync { get; protected set; } = false;

	public override bool CheckCompletion() => Talked;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		iconGroup.Add(NPCQuestIcon.Create(NPCType, npc.TypeName));
	}

	public override string GetObjectiveText()
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		return QuestText.Get("Objectives.Talk", npc.TypeName);
	}

	public virtual string GetChat() => QuestText.Get("Objectives.Talk", Lang.GetNPCNameValue(NPCType));

	public bool TrySubmitObjective(Player player)
	{
		if (!CanProgress || !QuestChat.CanSubmitObjective(player, NPCType))
		{
			return false;
		}
		if (Main.netMode == NetmodeID.SinglePlayer)
		{
			Talked = true;
		}
		else
		{
			localSubmitted = true;
			NeedDeltaSync = true;
		}
		return true;
	}

	public override void Update()
	{
		if (CanProgress && !Talked && Main.netMode == NetmodeID.MultiplayerClient
			&& localSubmitted && WorldQuestManager.NetUpdate)
		{
			NeedDeltaSync = true;
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		NeedDeltaSync = false;
		Talked = false;
		localSubmitted = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(Talked), Talked);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		localSubmitted = false;
		NeedDeltaSync = false;
		if (tag.TryGet(nameof(Talked), out bool talked))
		{
			Talked = talked;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(Talked);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		Talked = reader.ReadBoolean();
		ClearResolvedSubmission();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(localSubmitted);
		NeedDeltaSync = false;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		Talked |= br.ReadBoolean();
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(Talked);
	}

	public override void ReceiveMain(BinaryReader br)
	{
		Talked = br.ReadBoolean();
		ClearResolvedSubmission();
	}

	private void ClearResolvedSubmission()
	{
		if (Talked || Completed || IsTimedOut)
		{
			localSubmitted = false;
			NeedDeltaSync = false;
		}
	}
}
