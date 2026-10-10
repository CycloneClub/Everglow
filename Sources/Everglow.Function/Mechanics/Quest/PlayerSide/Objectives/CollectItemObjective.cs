using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Hooks;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class CollectItemObjective : PlayerObjectiveBase
{
	public CollectItemObjective()
	{
	}

	public CollectItemObjective(List<int> itemTypes, int itemCount, bool enableIndividualCounter = true)
	{
		if (itemTypes.Count == 0 || itemCount <= 0)
		{
			throw new InvalidDataException();
		}

		ItemTypes = itemTypes;
		ItemCount = itemCount;
		EnableIndividualCounter = enableIndividualCounter;
	}

	public List<int> ItemTypes { get; private set; } = [];

	public int ItemCount { get; private set; }

	public int CollectedCount { get; private set; }

	public bool EnableIndividualCounter { get; set; } = false;

	public override float Progress => CalculateProgress(Main.LocalPlayer);

	/// <summary>
	/// Calculate the progress of the objective.
	/// <para/> This method is created for unit tests, so it is not recommended to use it in other places.
	/// </summary>
	/// <param name="player"></param>
	/// <returns></returns>
	public float CalculateProgress(Player player) => EnableIndividualCounter
		? Math.Clamp(CollectedCount / (float)ItemCount, 0f, 1f)
		: Math.Clamp(player.inventory.Where(x => ItemTypes.Contains(x.type)).Sum(x => x.stack) / (float)ItemCount, 0f, 1f);

	public override bool CheckCompletion() => Progress >= 1f;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemQuestIcon.Create(item, new Item(item).Name));
		}
	}

	public override string GetObjectiveText()
	{
		string progress = EnableIndividualCounter
			? $"({CollectedCount}/{ItemCount})"
			: $"({Main.LocalPlayer.inventory.Where(i => ItemTypes.Contains(i.type)).Sum(i => i.stack)}/{ItemCount})";
		string textKey = EnableIndividualCounter ? "Objectives.Collect" : "Objectives.Have";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			return QuestText.Get(textKey + "Any", itemString, ItemCount, progress);
		}

		return QuestText.Get(textKey, ItemDrawer.Create(ItemTypes.First()), ItemCount, progress);
	}

	/// <summary>
	/// Count pick item.
	/// </summary>
	/// <param name="item"></param>
	public void QuestPlayer_OnPickUp(Item item)
	{
		if (ItemTypes.Contains(item.type) && EnableIndividualCounter)
		{
			CollectedCount += item.stack;
		}
	}

	public override void Activate(PlayerQuestBase sourceQuest)
	{
		QuestPlayer.OnPickupEvent += QuestPlayer_OnPickUp;
	}

	public override void Deactivate()
	{
		QuestPlayer.OnPickupEvent -= QuestPlayer_OnPickUp;
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		CollectedCount = 0;
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);

		if (tag.TryGet<int>(nameof(CollectedCount), out var collectedCount))
		{
			CollectedCount = collectedCount;
		}
		else if (tag.TryGet<TagCompound>("Counter", out var counter))
		{
			CollectedCount = counter.GetInt("Value");
		}
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);

		tag.Add(nameof(CollectedCount), CollectedCount);
	}
}
