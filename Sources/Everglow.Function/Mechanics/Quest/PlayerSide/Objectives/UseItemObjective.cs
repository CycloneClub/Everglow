using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

[Obsolete("This class is yet unfinished, don't use it.", true)]
public class UseItemObjective : PlayerObjectiveBase
{
	public UseItemObjective()
	{
	}

	public UseItemObjective(List<int> itemTypes, int itemCount)
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

	public int UsedCount { get; private set; }

	public override float Progress => Math.Clamp(UsedCount / (float)ItemCount, 0f, 1f);

	public override bool CheckCompletion() => UsedCount >= ItemCount;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemQuestIcon.Create(item, new Item(item).Name));
		}
	}

	public override string GetObjectiveText()
	{
		var progress = $"({UsedCount}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			return $"使用{itemString}合计{ItemCount}次 {progress}";
		}

		return $"使用{ItemDrawer.Create(ItemTypes.First())}{ItemCount}次 {progress}";
	}

	public override void Activate(PlayerQuestBase sourceQuest)
	{
		QuestGlobalItem.PlayerSide_OnUseItemEvent += QuestGlobalItem_OnUseItem;
	}

	public override void Deactivate()
	{
		QuestGlobalItem.PlayerSide_OnUseItemEvent -= QuestGlobalItem_OnUseItem;
	}

	private void QuestGlobalItem_OnUseItem(Item item)
	{
		if (ItemTypes.Contains(item.type))
		{
			UsedCount++;
		}
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet<int>(nameof(UsedCount), out var usedCount))
		{
			UsedCount = usedCount;
		}
		else if (tag.TryGet<TagCompound>("Counter", out var counter))
		{
			UsedCount = counter.GetInt("Value");
		}
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(UsedCount), UsedCount);
	}
}
