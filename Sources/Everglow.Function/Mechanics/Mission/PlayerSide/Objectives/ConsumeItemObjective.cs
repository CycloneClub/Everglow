using Everglow.Commons.Mechanics.Mission.Hooks;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;

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

	public override void OnInitialize()
	{
		base.OnInitialize();
		AssetUtils.LoadVanillaItemTextures(ItemTypes);
	}

	public override bool CheckCompletion() => ConsumedCount >= ItemCount;

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemMissionIcon.Create(item, new Item(item).Name));
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		var progress = $"({ConsumedCount}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			lines.Add($"消耗{itemString}合计{ItemCount}个 {progress}\n");
		}
		else
		{
			lines.Add($"消耗{ItemDrawer.Create(ItemTypes.First())}{ItemCount}个 {progress}\n");
		}
	}

	public override void Activate(PlayerMissionBase sourceMission)
	{
		MissionGlobalItem.PlayerSide_OnConsumeItemEvent += MissionGlobalItem_OnConsumeItem;
	}

	public override void Deactivate()
	{
		MissionGlobalItem.PlayerSide_OnConsumeItemEvent -= MissionGlobalItem_OnConsumeItem;
	}

	private void MissionGlobalItem_OnConsumeItem(Item item)
	{
		if (ItemTypes.Contains(item.type))
		{
			ConsumedCount++;
		}
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
