using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using SubworldLibrary;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldCollectItemObjective : WorldObjectiveBase
{
	public WorldCollectItemObjective()
	{
	}

	public WorldCollectItemObjective(int itemType, int itemCount)
	{
		ItemType = itemType;
		ItemCount = itemCount;
	}

	private Dictionary<string, int> globalCount = [];

	private int localMax;
	private int globalMax;

	public int ItemType { get; private set; }

	public int ItemCount { get; private set; }

	public bool Reached { get; private set; }

	public override float Progress => Math.Clamp((float)globalMax / ItemCount, 0f, 1f);

	public override bool NeedDeltaSync { get; protected set; }

	public override bool CheckCompletion() => Reached;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup) =>
		iconGroup.Add(ItemQuestIcon.Create(ItemType, new Item(ItemType).Name));

	public override string GetObjectiveText() => $"拥有{ItemDrawer.Create(ItemType)}{ItemCount}个 ({globalMax}/{ItemCount})";

	public override void Update()
	{
		if (NetUtils.IsSingle)
		{
			globalMax = Main.LocalPlayer.CountItem(ItemType);
			if (globalMax >= ItemCount)
			{
				globalMax = ItemCount;
				Reached = true;
			}
		}
		else if (NetUtils.IsMainServer)
		{
			int mainWorldMax = 0;
			foreach (var player in Main.ActivePlayers)
			{
				int count = player.CountItem(ItemType, ItemCount);
				if (count > mainWorldMax)
				{
					mainWorldMax = count;
				}
			}

			var mainWorldName = SubworldSystem.Current?.Name ?? "MainWorld";
			if (!globalCount.TryAdd(mainWorldName, mainWorldMax))
			{
				globalCount[mainWorldName] = mainWorldMax;
			}

			globalMax = globalCount.Max(x => x.Value);
			if (globalMax >= ItemCount)
			{
				globalMax = ItemCount;
				Reached = true;
			}
		}
		else if (NetUtils.IsSubServer)
		{
			int maxCount = 0;

			foreach (var player in Main.ActivePlayers)
			{
				int count = player.CountItem(ItemType, ItemCount);
				if (count > maxCount)
				{
					maxCount = count;
				}
			}

			if (maxCount > localMax)
			{
				NeedDeltaSync = true;
			}

			localMax = maxCount;

			if (WorldQuestManager.NetUpdate)
			{
				NeedDeltaSync = true;
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		globalCount = [];
		Reached = false;
		localMax = 0;
		globalMax = 0;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(Reached), Reached);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet(nameof(Reached), out bool r))
		{
			Reached = r;
		}
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
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(SubworldSystem.Current.Name);
		bw.Write(localMax);
		NeedDeltaSync = false;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		string subWorldName = br.ReadString();
		int subWorldMax = br.ReadInt32();
		if (!globalCount.TryAdd(subWorldName, subWorldMax))
		{
			globalCount[subWorldName] = subWorldMax;
		}
		globalMax = globalCount.Max(x => x.Value);
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(globalMax);
		bw.Write(Reached);
	}

	public override void ReceiveMain(BinaryReader br)
	{
		globalMax = br.ReadInt32();
		Reached = br.ReadBoolean();
	}
}
