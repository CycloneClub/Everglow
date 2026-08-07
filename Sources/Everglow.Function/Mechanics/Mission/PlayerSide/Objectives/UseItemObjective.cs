using Everglow.Commons.Mechanics.Mission.Hooks;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;

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

	public override void OnInitialize()
	{
		base.OnInitialize();
		AssetUtils.LoadVanillaItemTextures(ItemTypes);
	}

	public override bool CheckCompletion() => UsedCount >= ItemCount;

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemMissionIcon.Create(item, new Item(item).Name));
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		var progress = $"({UsedCount}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			lines.Add($"使用{itemString}合计{ItemCount}次 {progress}\n");
		}
		else
		{
			lines.Add($"使用{ItemDrawer.Create(ItemTypes.First())}{ItemCount}次 {progress}\n");
		}
	}

	public override void Activate(PlayerMissionBase sourceMission)
	{
		MissionGlobalItem.PlayerSide_OnUseItemEvent += MissionGlobalItem_OnUseItem;
	}

	public override void Deactivate()
	{
		MissionGlobalItem.PlayerSide_OnUseItemEvent -= MissionGlobalItem_OnUseItem;
	}

	private void MissionGlobalItem_OnUseItem(Item item)
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
