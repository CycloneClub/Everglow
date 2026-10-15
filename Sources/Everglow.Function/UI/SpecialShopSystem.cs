using Everglow.Commons.UI.UIElements;
using Mono.Cecil.Cil;
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
			IL_ItemSlot.SellOrTrash += IL_ItemSlot_SellOrTrash;
			IL_ItemSlot.OverrideHover_ItemArray_int_int += IL_ItemSlot_OverrideHover_ItemArray_int_int;
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

		Main.LocalPlayer.talkNPC |= -1;

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

	private void IL_ItemSlot_OverrideHover_ItemArray_int_int(ILContext il)
	{
		ILCursor c = new(il);

		// 2 loops process ctrl and shift respectively.
		for (int index = 0; index < 2; index++)
		{
			if (!c.TryGotoNext(
				MoveType.Before,
				i => i.MatchCall(typeof(Main), "get_npcShop"),
				i => i.MatchLdcI4(0),
				i => i.MatchBle(out _)))
			{
				throw new InvalidOperationException(
					$"Can't find shop condition #{index + 1} in ItemSlot.OverrideHover. Check tModLoader version.");
			}

			// Move behind getter before overwriting the value.
			c.Index++;
			c.EmitDelegate<Func<int, int>>(npcShop =>
				CurrentShop is null ? npcShop : 0);
		}
	}

	private void IL_ItemSlot_SellOrTrash(ILContext il)
	{
		ILCursor c = new(il);
		ILLabel elseLabel = null;
		if (!c.TryGotoNext(
			MoveType.After,
			i => i.MatchLdfld<Item>(nameof(Item.favorited)),
			i => i.MatchBrtrue(out elseLabel)))
		{
			throw new InvalidOperationException("Can't find sell condition. Check tmodloader version.");
		}

		c.EmitDelegate(() => currentShop is null);
		c.Emit(OpCodes.Brfalse, elseLabel);
	}

	private void IL_Main_DrawInventory(ILContext il)
	{
		ILCursor restoreCursor = new(il);
		if (!restoreCursor.TryGotoNext(
			MoveType.After,
			x => x.MatchLdindU2(),
			x => x.MatchLdelemU1(),
			x => x.MatchBrtrue(out _),
			x => x.MatchLdsfld(out _),
			x => x.MatchLdsfld(out _),
			x => x.MatchLdelemRef(),
			x => x.MatchLdcI4(-1),
			x => x.MatchStfld(out _),
			x => x.MatchLdcI4(0),
			x => x.MatchCall(out _),
			x => x.MatchLdcI4(0),
			x => x.MatchStloc(out _)))
		{
			throw new InvalidOperationException("Can't find sell condition. Check tmodloader version.");
		}

		restoreCursor.EmitDelegate(CheckSpecialShopEnable_ModifyNpcShop);
	}

	private void On_Main_DrawInventory(On_Main.orig_DrawInventory orig, Main self)
	{
		CheckSpecialShopEnable_ModifyNpcShop();
		orig(self);
		DisposeSpecialShopEnable_ModifyNpcShop();
	}

	private void CheckSpecialShopEnable_ModifyNpcShop()
	{
		if (CurrentShop is not null && Main.npcShop == 0)
		{
			Main.npcShop = SpecialShopWhoAmI;
		}
	}

	private void DisposeSpecialShopEnable_ModifyNpcShop()
	{
		if (Main.npcShop >= SpecialShopWhoAmI)
		{
			Main.npcShop = 0;
		}
	}
}
