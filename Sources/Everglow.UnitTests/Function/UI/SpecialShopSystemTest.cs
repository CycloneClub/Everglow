using System.Reflection;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace Everglow.UnitTests.Function.UI;

[TestClass]
[DoNotParallelize]
public class SpecialShopSystemTest
{
	private static readonly FieldInfo CurrentShopField = typeof(SpecialShopSystem)
		.GetField("currentShop", BindingFlags.Instance | BindingFlags.NonPublic)!;
	private static readonly MethodInfo OverrideLeftClickMethod = typeof(SpecialShopSystem)
		.GetMethod("On_ItemSlot_OverrideLeftClick", BindingFlags.Instance | BindingFlags.NonPublic)!;

	private int originalCursorOverride;
	private bool originalDedServ;

	private sealed class StubShop : SpecialShopUI
	{
		public bool Visible { get; set; } = true;

		public override bool IsVisible => Visible;
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalCursorOverride = Main.cursorOverride;
		originalDedServ = Main.dedServ;
		Main.dedServ = true;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.cursorOverride = originalCursorOverride;
		Main.dedServ = originalDedServ;
	}

	[TestMethod]
	[DataRow(6)]
	[DataRow(10)]
	public void VisibleSpecialShop_PreventsTrashAndSaleItemLoss(int cursorOverride)
	{
		var system = new SpecialShopSystem();
		CurrentShopField.SetValue(system, new StubShop());
		Main.cursorOverride = cursorOverride;
		Item[] inventory = [new Item { type = ItemID.DirtBlock, stack = 4 }];
		On_ItemSlot.orig_OverrideLeftClick original = (items, context, slot) =>
		{
			items[slot].TurnToAir();
			return true;
		};

		bool handled = (bool)OverrideLeftClickMethod.Invoke(system, [original, inventory, 0, 0])!;

		Assert.IsTrue(handled);
		Assert.AreEqual(ItemID.DirtBlock, inventory[0].type);
		Assert.AreEqual(4, inventory[0].stack);
	}

	[TestMethod]
	[DataRow(false, 10)]
	[DataRow(true, 9)]
	public void HiddenSpecialShopOrOtherAction_PreservesOriginalClick(bool shopVisible, int cursorOverride)
	{
		var system = new SpecialShopSystem();
		CurrentShopField.SetValue(system, new StubShop { Visible = shopVisible });
		Main.cursorOverride = cursorOverride;
		Item[] inventory = [new Item { type = ItemID.DirtBlock, stack = 4 }];
		On_ItemSlot.orig_OverrideLeftClick original = (items, context, slot) =>
		{
			items[slot].TurnToAir();
			return true;
		};

		bool handled = (bool)OverrideLeftClickMethod.Invoke(system, [original, inventory, 0, 0])!;

		Assert.IsTrue(handled);
		Assert.IsTrue(inventory[0].IsAir);
	}
}
