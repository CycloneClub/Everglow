using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.ImageDrawers;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Objectives;

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

		StartText = "请给我一些东西。";

		EndText = "谢谢你！";
	}

	public GiveItemObjective(List<int> itemTypes, int itemCount, int npcType, string startText, string endText)
	{
		InitializeItems(itemTypes, itemCount);
		NPCType = npcType >= NPCID.None
			? npcType
			: throw new InvalidDataException($"NPC type should more than 0.");

		StartText = !string.IsNullOrEmpty(startText)
			? startText
			: throw new ArgumentNullException("Argument 'text' should not be empty!");

		EndText = !string.IsNullOrEmpty(endText)
			? endText
			: throw new ArgumentNullException("Argument 'text' should not be empty!");
	}

	public int NPCType { get; set; }

	public string StartText { get; set; }

	public string EndText { get; set; }

	public List<int> ItemTypes { get; private set; } = [];

	public int ItemCount { get; private set; }

	public override float Progress => GetInventoryProgress(Main.LocalPlayer.inventory);

	public bool IsTalkingToNPC => NPCType == NPCID.None || (NPCType > NPCID.None && Main.LocalPlayer.talkNPC >= NPCID.None && Main.npc[Main.LocalPlayer.talkNPC].type == NPCType);

	public override void OnInitialize()
	{
		base.OnInitialize();
		AssetUtils.LoadVanillaItemTextures(ItemTypes);
		AssetUtils.LoadVanillaNPCTextures([NPCType]);
	}

	public override bool CheckCompletion() => IsTalkingToNPC && GetInventoryProgress(Main.LocalPlayer.inventory) >= 1f;

	public override void Update()
	{
		base.Update();

		if (IsTalkingToNPC)
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
			if (inventoryItem.stack < stackCount)
			{
				stackCount -= inventoryItem.stack;
				inventoryItem.stack = 0;
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

		if (IsTalkingToNPC)
		{
			Main.npcChatText = EndText;
		}

		base.Complete();
	}

	public override void GetObjectivesIcon(MissionIconGroup iconGroup)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);
		iconGroup.Add(NPCMissionIcon.Create(NPCType, npc.TypeName));

		foreach (var item in ItemTypes)
		{
			iconGroup.Add(ItemMissionIcon.Create(item, new Item(item).Name));
		}
	}

	public override void GetObjectivesText(List<string> lines)
	{
		var npc = new NPC();
		npc.SetDefaults(NPCType);

		var progress = $"({Main.LocalPlayer.inventory.Where(i => ItemTypes.Contains(i.type)).Sum(i => i.stack)}/{ItemCount})";
		if (ItemTypes.Count > 1)
		{
			var itemString = string.Join(' ', ItemTypes.ConvertAll(i => ItemDrawer.Create(i)));
			lines.Add($"向{npc.TypeName}提交{itemString}合计{ItemCount}个 {progress}\n");
		}
		else
		{
			lines.Add($"向{npc.TypeName}提交{ItemDrawer.Create(ItemTypes.First())}{ItemCount}个 {progress}\n");
		}
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
