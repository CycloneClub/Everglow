using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldGiveObjective : WorldObjectiveBase, IQuestDialogueObjective
{
	public WorldGiveObjective()
	{
	}

	public WorldGiveObjective(int npcType, int itemType, int itemCount)
	{
		NPCType = npcType;
		ItemType = itemType;
		ItemCount = itemCount;
	}

	private bool localSubmitted;

	public ObjectiveDialogue Dialogue { get; set; }

	public int NPCType { get; private set; }

	public int ItemType { get; private set; }

	public int ItemCount { get; private set; }

	public bool Given { get; private set; }

	public override float Progress => Given ? 1f : 0f;

	public override bool NeedDeltaSync { get; protected set; }

	public override bool CheckCompletion() => Given;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		iconGroup.Add(NPCQuestIcon.Create(NPCType, npc.TypeName));
		iconGroup.Add(ItemQuestIcon.Create(ItemType, new Item(ItemType).Name));
	}

	public override string GetObjectiveText()
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		return QuestText.Get("Objectives.Give", npc.TypeName, ItemDrawer.Create(ItemType), ItemCount, string.Empty).TrimEnd();
	}

	public override void Update()
	{
		if (CanProgress && !Given && Main.netMode == NetmodeID.MultiplayerClient
			&& localSubmitted && WorldQuestManager.NetUpdate)
		{
			NeedDeltaSync = true;
		}
	}

	public virtual string GetChat() => QuestText.Get(
		"Objectives.Give",
		Lang.GetNPCNameValue(NPCType), Lang.GetItemNameValue(ItemType), ItemCount, string.Empty).TrimEnd();

	public bool TrySubmitObjective(Player player)
	{
		if (!CanProgress || !QuestChat.CanSubmitObjective(player, NPCType))
		{
			return false;
		}
		if (Given || localSubmitted)
		{
			return true;
		}
		if (player.TalkNPC?.netID != NPCType)
		{
			return false;
		}
		if (player.CountItem(ItemType, ItemCount) < ItemCount)
		{
			return false;
		}
		for (int i = 0; i < ItemCount; i++)
		{
			player.ConsumeItem(ItemType);
		}
		if (NetUtils.IsSingle)
		{
			Given = true;
		}
		else
		{
			localSubmitted = true;
			NeedDeltaSync = true;
		}
		return true;
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		NeedDeltaSync = false;
		Given = false;
		localSubmitted = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(Given), Given);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		localSubmitted = false;
		NeedDeltaSync = false;
		if (tag.TryGet(nameof(Given), out bool g))
		{
			Given = g;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(Given);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		Given = reader.ReadBoolean();
		ClearResolvedSubmission();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(localSubmitted);
		NeedDeltaSync = false;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		Given |= br.ReadBoolean();
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(Given);
	}

	public override void ReceiveMain(BinaryReader br)
	{
		Given = br.ReadBoolean();
		ClearResolvedSubmission();
	}

	private void ClearResolvedSubmission()
	{
		if (Given || Completed || IsTimedOut)
		{
			localSubmitted = false;
			NeedDeltaSync = false;
		}
	}
}
