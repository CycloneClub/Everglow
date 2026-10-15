using Everglow.Commons.UI.UIElements;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;

namespace Everglow.Yggdrasil.YggdrasilTown.UI;

public class FurnaceScoreShop_Itemslot : UIItemSlot
{
	public override void OnInitialization()
	{
		base.OnInitialization();
		Info.Left.SetValue(20, 0f);
		Info.Top.SetValue(20, 0f);
		Info.Width.SetValue(36, 0f);
		Info.Height.SetValue(36, 0f);
		DrawColor = new Color(0.66f, 0.66f, 0.66f, 0.66f);
		CanTakeOutSlot = CanTakeOut;
		CanPutInSlot = CanPutIn;
		OnPickItem += (target) =>
		{
			int value = 0;
			if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(Main.mouseItem.type))
			{
				value = FurnaceScoreShop.SellPricesInFurnaceScore[Main.mouseItem.type];
			}
			Main.LocalPlayer.GetModPlayer<FurnacePlayer>().CurrentFurnaceScore -= value;
		};
		OnPutItem += (target) =>
		{
			int value = 0;
			if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(ContainedItem.type))
			{
				value = FurnaceScoreShop.SellPricesInFurnaceScore[ContainedItem.type];
			}
			Main.LocalPlayer.GetModPlayer<FurnacePlayer>().CurrentFurnaceScore += value;
			ContainedItem = new Item();
			ContainedItem.SetDefaults(ItemID.None, true);
		};
	}

	public int GetIndex(UIItemSlot slot)
	{
		for (int i = 0; i < FurnaceScoreShopUI.Instance.FurnaceScoreShopItemSlots.Length; i++)
		{
			if (FurnaceScoreShopUI.Instance.FurnaceScoreShopItemSlots[i] == slot)
			{
				return i;
			}
		}
		return -1;
	}

	public bool CanTakeOut(Item item)
	{
		int value = 0;
		if (FurnaceScoreShop.SellPricesInFurnaceScore.ContainsKey(item.type))
		{
			value = FurnaceScoreShop.SellPricesInFurnaceScore[item.type];
		}
		return Main.LocalPlayer.GetModPlayer<FurnacePlayer>().CurrentFurnaceScore >= value;
	}

	public bool CanPutIn(Item item)
	{
		return false;
	}

	public override void Update(GameTime gameTime)
	{
		base.Update(gameTime);
	}
}
