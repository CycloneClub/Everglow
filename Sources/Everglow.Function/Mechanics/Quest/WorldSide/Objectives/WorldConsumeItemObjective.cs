using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldConsumeItemObjective : WorldObjectiveBase
{
	public WorldConsumeItemObjective()
	{
	}

	public WorldConsumeItemObjective(int itemType, int itemCount)
	{
		ItemType = itemType;
		ItemCount = itemCount;
	}

	private int _localConsumedCount = 0;

	public int ItemType { get; private set; }

	public int ItemCount { get; private set; }

	public int ConsumedCount { get; private set; }

	public override bool NeedDeltaSync => _localConsumedCount > 0;

	public override float Progress => Math.Clamp(ConsumedCount / (float)ItemCount, 0, 1);

	public override bool CheckCompletion() => ConsumedCount >= ItemCount;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup) =>
		iconGroup.Add(ItemQuestIcon.Create(ItemType, new Item(ItemType).Name));

	public override string GetObjectiveText() => $"消耗{ItemDrawer.Create(ItemType)}{ItemCount}个 ({ConsumedCount}/{ItemCount})";

	public override void ResetProgress()
	{
		base.ResetProgress();

		ConsumedCount = 0;
		_localConsumedCount = 0;
	}

	public override void Activate(WorldQuestBase sourceQuest)
	{
		QuestGlobalItem.WorldSide_OnItemConsumed += WorldQuestGlobalItem_OnItemConsumed;
	}

	public override void Deactivate()
	{
		QuestGlobalItem.WorldSide_OnItemConsumed -= WorldQuestGlobalItem_OnItemConsumed;
	}

	private void WorldQuestGlobalItem_OnItemConsumed(Item item, Player player)
	{
		if (ItemType == item.type)
		{
			if (NetUtils.IsSingle)
			{
				ConsumedCount++;
				if (ConsumedCount > ItemCount)
				{
					ConsumedCount = ItemCount;
				}

				Console.WriteLine($"{ConsumedCount}/{ItemCount}");
			}

			if (NetUtils.IsClient)
			{
				_localConsumedCount++;
			}
		}
	}

	override public void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(ConsumedCount), ConsumedCount);
	}

	override public void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet<int>(nameof(ConsumedCount), out var cc))
		{
			ConsumedCount = cc;
		}
	}

	override public void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(ConsumedCount);
	}

	override public void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		ConsumedCount = reader.ReadInt32();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(_localConsumedCount);

		Console.WriteLine($"Contributed {_localConsumedCount} times consuming.");
		_localConsumedCount = 0;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		var count = br.ReadInt32();
		ConsumedCount += count;
		if (ConsumedCount > ItemCount)
		{
			ConsumedCount = ItemCount;
		}
		Console.WriteLine($"Received {count} times consuming to {ConsumedCount}.");
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(ConsumedCount);
		Console.WriteLine($"Sync {ConsumedCount} as total.");
	}

	public override void ReceiveMain(BinaryReader br)
	{
		ConsumedCount = br.ReadInt32();
		Console.WriteLine($"Received {ConsumedCount} as total.");
	}
}
