using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class ConsumeItemObjective : PlayerObjectiveBase
{
	public ConsumeItemObjective()
	{
	}

	public ConsumeItemObjective(List<int> itemTypes, int itemCount)
	{
		if (itemTypes.Count == 0 || itemCount <= 0)
		{
			throw new InvalidDataException();
		}

		ItemTypes = itemTypes;
		ItemCount = itemCount;
	}

	public List<int> ItemTypes { get; private set; } = [];

	public int ItemCount { get; private set; }

	public int ConsumedCount { get; private set; }

	public override float Progress => Math.Clamp(ConsumedCount / (float)ItemCount, 0f, 1f);

	public override bool CheckCompletion() => ConsumedCount >= ItemCount;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemQuestIcon.Create(item, new Item(item).Name));
		}
	}

	public override string GetObjectiveText()
	{
		var progress = $"({ConsumedCount}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			return $"消耗{itemString}合计{ItemCount}个 {progress}";
		}

		return $"消耗{ItemDrawer.Create(ItemTypes.First())}{ItemCount}个 {progress}";
	}

	public override void Activate(PlayerQuestBase sourceQuest)
	{
		QuestGlobalItem.PlayerSide_OnConsumeItemEvent += QuestGlobalItem_OnConsumeItem;
	}

	public override void Deactivate()
	{
		QuestGlobalItem.PlayerSide_OnConsumeItemEvent -= QuestGlobalItem_OnConsumeItem;
	}

	private void QuestGlobalItem_OnConsumeItem(Item item)
	{
		if (ItemTypes.Contains(item.type))
		{
			ConsumedCount++;
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		ConsumedCount = 0;
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet<int>(nameof(ConsumedCount), out var consumedCount))
		{
			ConsumedCount = consumedCount;
		}
		else if (tag.TryGet<TagCompound>("Counter", out var counter))
		{
			ConsumedCount = counter.GetInt("Value");
		}
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(ConsumedCount), ConsumedCount);
	}
}
