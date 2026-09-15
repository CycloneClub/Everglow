using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.UI;

/// <summary>
/// Reuses registered UI instances and keeps at most one special shop open.
/// </summary>
public class SpecialShopSystem : ModSystem
{
	private readonly Dictionary<Type, SpecialShopUI> shops = [];
	private SpecialShopUI currentShop;

	public static SpecialShopSystem Instance => ModContent.GetInstance<SpecialShopSystem>();

	public SpecialShopUI CurrentShop => currentShop is { IsVisible: true } ? currentShop : null;

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
}
