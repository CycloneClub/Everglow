using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldGiveObjective : WorldObjectiveBase
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

	public WorldGiveObjective(int npcType, int itemType, int itemCount, string startText, string endText)
		: this(npcType, itemType, itemCount)
	{
		StartText = startText;
		EndText = endText;
	}

	private bool localSubmitted;

	public int NPCType { get; private set; }

	public int ItemType { get; private set; }

	public int ItemCount { get; private set; }

	public bool Given { get; private set; }

	public string StartText { get; set; } = string.Empty;

	public string EndText { get; set; } = string.Empty;

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
		if (!CanProgress || Given || NetUtils.IsServer)
		{
			return;
		}
		if (localSubmitted)
		{
			if (WorldQuestManager.NetUpdate)
			{
				NeedDeltaSync = true;
			}
			return;
		}

		var player = Main.LocalPlayer;
		if (player.TalkNPC?.netID != NPCType)
		{
			return;
		}
		if (!string.IsNullOrEmpty(StartText))
		{
			Main.npcChatText = StartText;
		}
		if (player.CountItem(ItemType, ItemCount) < ItemCount)
		{
			return;
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
		if (!string.IsNullOrEmpty(EndText))
		{
			Main.npcChatText = EndText;
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
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
	}
}
