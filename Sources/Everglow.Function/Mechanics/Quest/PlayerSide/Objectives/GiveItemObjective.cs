using Everglow.Commons.Mechanics.Quest.Core.Dialogue;
using Terraria.ModLoader.IO;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;

public class GiveItemObjective : PlayerObjectiveBase, IQuestDialogueObjective
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

	private bool dialogueSubmitted;

	public ObjectiveDialogue Dialogue { get; set; }

	public int NPCType { get; set; }

	public List<int> ItemTypes { get; private set; } = [];

	public int ItemCount { get; private set; }

	public override float Progress => dialogueSubmitted ? 1f : GetInventoryProgress(Main.LocalPlayer.inventory);

	public override bool CheckCompletion() => dialogueSubmitted;

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

	public virtual string GetChat()
	{
		string items = ItemTypes.Count > 1
			? string.Join(QuestText.Get("Common.ListSeparator"), ItemTypes.Select(type => QuestText.ItemTag(type)))
			: QuestText.ItemTag(ItemTypes[0], ItemCount);
		string progress = $"({Main.LocalPlayer.inventory.Where(i => ItemTypes.Contains(i.type)).Sum(i => i.stack)}/{ItemCount})";
		return QuestText.Get(
			ItemTypes.Count > 1 ? "Objectives.GiveAny" : "Objectives.Give",
			Lang.GetNPCNameValue(NPCType), items, ItemCount, progress);
	}

	public bool TrySubmitObjective(Player player)
	{
		if (!CanProgress || !QuestChat.CanSubmitObjective(player, NPCType))
		{
			return false;
		}
		if (!dialogueSubmitted)
		{
			if (GetInventoryProgress(player.inventory) < 1f)
			{
				return false;
			}
			RemoveItem(player.inventory);
			dialogueSubmitted = true;
		}
		return true;
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		dialogueSubmitted = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag["DialogueSubmitted"] = dialogueSubmitted;
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		dialogueSubmitted = tag.GetBool("DialogueSubmitted");
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

		return QuestText.Get("Objectives.GiveItem", npc.TypeName, ItemDrawer.Create(ItemTypes.First(), ItemCount), progress);
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
