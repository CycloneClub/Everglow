using Everglow.Commons.Mechanics.Mission.WorldSide;
using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;

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

	private bool localSubmitted;

	public int NPCType { get; private set; }

	public int ItemType { get; private set; }

	public int ItemCount { get; private set; }

	public bool Given { get; private set; }

	public override float Progress => Given ? 1f : 0f;

	public override bool NeedDeltaSync { get; protected set; }

	public override bool CheckCompletion() => Given;

	public override void Update()
	{
		if (NetUtils.IsSingle)
		{
			var player = Main.LocalPlayer;
			if (player.TalkNPC?.netID == NPCType)
			{
				if (player.CountItem(ItemType, ItemCount) >= ItemCount)
				{
					for (int i = 0; i < ItemCount; i++)
					{
						player.ConsumeItem(ItemType);
					}

					Given = true;
				}
			}
			return;
		}
		else if (NetUtils.IsClient)
		{
			if (localSubmitted)
			{
				if (WorldMissionManager.NetUpdate)
				{
					NeedDeltaSync = true;
				}
				return;
			}

			var player = Main.LocalPlayer;
			if (player.TalkNPC?.netID == NPCType)
			{
				if (player.CountItem(ItemType, ItemCount) >= ItemCount)
				{
					for (int i = 0; i < ItemCount; i++)
					{
						player.ConsumeItem(ItemType);
					}

					localSubmitted = true;
					NeedDeltaSync = true;

					return;
				}
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		Given = false;
		localSubmitted = false;
	}

	public override void GetObjectivesText() => throw new NotImplementedException();

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
