using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class GiveItemObjective : PlayerObjectiveBase
{
	public GiveItemObjective()
	{
	}

	public GiveItemObjective(List<int> itemTypes, int itemCount, int npcType)
	{
		InitializeItems(itemTypes, itemCount);
		NPCType = npcType >= NPCID.None
			? npcType
			: throw new InvalidDataException($"NPC type should more than 0.");
	}

	public int NPCType { get; set; }

	public string StartText => GetText("StartText");

	public string EndText => GetText("EndText");

	public List<int> ItemTypes { get; private set; } = [];

	public int ItemCount { get; private set; }

	public override float Progress => GetInventoryProgress(Main.LocalPlayer.inventory);

	public bool IsTalkingToNPC => NPCType == NPCID.None || (NPCType > NPCID.None && Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType);

	public override bool CheckCompletion() => IsTalkingToNPC && GetInventoryProgress(Main.LocalPlayer.inventory) >= 1f;

	public override void Update()
	{
		base.Update();

		if (IsTalkingToNPC && !string.IsNullOrEmpty(StartText))
		{
			Main.npcChatText = StartText;
		}
	}

	/// <summary>
	/// Remove required items from player inventory.
	/// </summary>
	/// <param name="inventory"></param>
	public void RemoveItem(IEnumerable<Item> inventory)
	{
		var stackCount = ItemCount;
		foreach (var inventoryItem in inventory.Where(x => ItemTypes.Contains(x.type)))
		{
			if (inventoryItem.stack <= stackCount)
			{
				stackCount -= inventoryItem.stack;
				inventoryItem.TurnToAir();
				if (stackCount == 0)
				{
					break;
				}
			}
			else
			{
				inventoryItem.stack -= stackCount;
				break;
			}
		}
	}

	public override void Complete()
	{
		// Make sure the items can only be removed once.
		if (!Completed)
		{
			RemoveItem(Main.LocalPlayer.inventory);
		}

		if (IsTalkingToNPC && !string.IsNullOrEmpty(EndText))
		{
			Main.npcChatText = EndText;
		}

		base.Complete();
	}

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		iconGroup.Add(NPCQuestIcon.Create(NPCType, npc.TypeName));

		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemQuestIcon.Create(item, new Item(item).Name));
		}
	}

	public override string GetObjectiveText()
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);

		var progress = $"({Main.LocalPlayer.inventory.Where(i => ItemTypes.Contains(i.type)).Sum(i => i.stack)}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			return QuestText.Get("Objectives.GiveAny", npc.TypeName, itemString, ItemCount, progress);
		}

		return QuestText.Get("Objectives.Give", npc.TypeName, ItemDrawer.Create(ItemTypes.First()), ItemCount, progress);
	}

	private float GetInventoryProgress(IEnumerable<Item> inventory) => Math.Clamp(inventory.Where(x => ItemTypes.Contains(x.type)).Sum(x => x.stack) / (float)ItemCount, 0f, 1f);

	private void InitializeItems(List<int> itemTypes, int itemCount)
	{
		if (itemTypes.Count == 0 || itemCount <= 0)
		{
			throw new InvalidDataException();
		}

		ItemTypes = itemTypes;
		ItemCount = itemCount;
	}
}
