using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldTalkObjective : WorldObjectiveBase
{
	public WorldTalkObjective()
	{
	}

	public WorldTalkObjective(int npcType)
	{
		NPCType = npcType;
	}

	private bool talking;

	private bool oldTalking;

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
		return $"和{npc.TypeName}对话";
	}

	public override void Update()
	{
		if (NetUtils.IsSingle || NetUtils.IsMainServer)
		{
			foreach (var player in Main.ActivePlayers)
			{
				if (player.TalkNPC?.netID == NPCType)
				{
					Talked = true;
				}
			}
		}
		else if (NetUtils.IsSubServer)
		{
			oldTalking = talking;
			talking = false;
			foreach (var player in Main.ActivePlayers)
			{
				if (player.TalkNPC?.netID == NPCType)
				{
					talking = true;

					if (!oldTalking // First frame sending
						|| Main.timeForVisualEffects % 60 == 0) // Cyclical sending
					{
						NeedDeltaSync = true;
					}

					return;
				}
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		Talked = false;
		talking = false;
		oldTalking = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(Talked), Talked);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
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
	}

	public override void SendDelta(BinaryWriter bw)
	{
		Console.WriteLine("Synced talk npc to main");
		bw.Write(talking);
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
	}
}
