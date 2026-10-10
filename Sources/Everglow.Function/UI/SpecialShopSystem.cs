using Everglow.Commons.UI.UIElements;
using MonoMod.Cil;
using Terraria.UI;

namespace Everglow.Commons.UI;

/// <summary>
/// Reuses registered UI instances and keeps at most one special shop open.
/// </summary>
public class SpecialShopSystem : ModSystem
{
	private const int SpecialShopWhoAmI = 65536;

	private readonly Dictionary<Type, SpecialShopUI> shops = [];
	private SpecialShopUI currentShop;

	public static SpecialShopSystem Instance => ModContent.GetInstance<SpecialShopSystem>();

	public SpecialShopUI CurrentShop => currentShop is { IsVisible: true } ? currentShop : null;

	public override void Load()
	{
		if (!Main.dedServ)
		{
			On_Main.DrawInventory += On_Main_DrawInventory;
			IL_Main.DrawInventory += IL_Main_DrawInventory;
			On_ItemSlot.GetAlternateClickAction += On_ItemSlot_GetAlternateClickAction;
			On_ItemSlot.OverrideLeftClick += On_ItemSlot_OverrideLeftClick;
		}
	}

	public override void Unload()
	{
		Close();
		shops.Clear();
	}

	public override void PostSetupContent()
	{
		if (Main.dedServ)
		{
			return;
		}

		foreach (var shop in UISystem.EverglowUISystem.Elements.Values.OfType<SpecialShopUI>())
		{
			shops.Add(shop.GetType(), shop);
		}
	}

	public override void UpdateUI(GameTime gameTime)
	{
		if (currentShop is not null
			&& !Main.playerInventory)
		{
			Close();
		}
	}

	public override void OnWorldUnload()
	{
		Close();
	}

	public T Open<T>()
		where T : SpecialShopUI
	{
		T shop = (T)shops[typeof(T)];
		Main.playerInventory = true;
		if (CurrentShop != shop)
		{
			Close();
			currentShop = shop;
			shop.Show();
		}

		Main.LocalPlayer.SetTalkNPC(-1);

		return shop;
	}

	public void Close()
	{
		currentShop?.Close();
		currentShop = null;
	}

	public T Get<T>()
		where T : SpecialShopUI
	{
		return (T)shops[typeof(T)];
	}

	private ItemSlot.AlternateClickAction? On_ItemSlot_GetAlternateClickAction(On_ItemSlot.orig_GetAlternateClickAction orig, Item[] inv, int context, int slot)
	{
		var action = orig(inv, context, slot);
		if (CurrentShop is not null && action is { cursorOverride: 6 or 10 })
		{
			return null;
		}

		return action;
	}

	private bool On_ItemSlot_OverrideLeftClick(On_ItemSlot.orig_OverrideLeftClick orig, Item[] inv, int context, int slot)
	{
		if (CurrentShop is not null && Main.cursorOverride is 6 or 10)
		{
			return true;
		}

		return orig(inv, context, slot);
	}

	private void IL_Main_DrawInventory(ILContext il)
	{
		ILCursor restoreCursor = new(il);
		// Vanilla clears the custom shop index before drawing NPC shop slots. Restore it
		// after that section, before the quick-stack, sort and crafting controls.
		if (!restoreCursor.TryGotoNext(
			MoveType.After,
			x => x.MatchCall(typeof(ChestUI), nameof(ChestUI.Draw))))
		{
			throw new InvalidOperationException("Can't find ChestUI.Draw in Main.DrawInventory. Check tModLoader version.");
		}

		restoreCursor.EmitDelegate(CheckSpecialShopEnable_ModifyNpcShop);
	}

	private void On_Main_DrawInventory(On_Main.orig_DrawInventory orig, Main self)
	{
		CheckSpecialShopEnable_ModifyNpcShop();
		try
		{
			orig(self);
		}
		finally
		{
			DisposeSpecialShopEnable_ModifyNpcShop();
		}
	}

	private void CheckSpecialShopEnable_ModifyNpcShop()
	{
		if (CurrentShop is not null && Main.npcShop == 0)
		{
			Main.SetNPCShopIndex(SpecialShopWhoAmI);
		}
	}

	private void DisposeSpecialShopEnable_ModifyNpcShop()
	{
		if (Main.npcShop >= SpecialShopWhoAmI)
		{
			Main.SetNPCShopIndex(0);
		}
	}
}
