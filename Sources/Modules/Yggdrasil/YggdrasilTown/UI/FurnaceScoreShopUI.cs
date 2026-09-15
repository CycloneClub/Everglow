using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.UI;

public class FurnaceScoreShopUI : SpecialShopUI
{
	public static FurnaceScoreShopUI Instance => (FurnaceScoreShopUI)UISystem.EverglowUISystem.Elements[typeof(FurnaceScoreShopUI).FullName];

	// ==================== UI elements ==================== //
	public FurnaceScoreShop_Itemslot[] FurnaceScoreShopItemSlots;

	public FurnaceScoreMilestoneRewardUI _mileStoneRewardUI;

	public bool ShouldSetupShopItems = true;

	public override void OnInitialization()
	{
		base.OnInitialization();
		FurnaceScoreShopItemSlots = new FurnaceScoreShop_Itemslot[28];
		for (int i = 0; i < FurnaceScoreShopItemSlots.Length; i++)
		{
			FurnaceScoreShopItemSlots[i] = new FurnaceScoreShop_Itemslot()
			{
				SlotBackTexture = ModAsset.FurnaceShop_Normal.Value,
				ContainedItem = new Item(0, 1),
				CanPutInSlot = (item) => false,
				CanTakeOutSlot = (item) => false,
				CornerSize = new Vector2(10, 10),
				DrawColor = Color.White,
				Tooltip = "Furnace Score Shop Item Slot",
			};
			Register(FurnaceScoreShopItemSlots[i]);
		}
		_mileStoneRewardUI = new FurnaceScoreMilestoneRewardUI()
		{
			CanDrag = false,
		};
		Register(_mileStoneRewardUI);
	}

	public override void Calculation()
	{
		base.Calculation();
		Info.Left.SetValue(0, 0f);
		Info.Top.SetValue(240, 0f);
		Info.Width.SetValue(480, 0f);
		Info.Height.SetValue(300, 0f);
		for (int i = 0; i < FurnaceScoreShopItemSlots.Length; i++)
		{
			FurnaceScoreShopItemSlots[i].Info.Left.SetValue(20 + i % 7 * 42, 0f);
			FurnaceScoreShopItemSlots[i].Info.Top.SetValue(20 + i / 7 * 42, 0f);
			FurnaceScoreShopItemSlots[i].Info.Width.SetValue(38, 0f);
			FurnaceScoreShopItemSlots[i].Info.Height.SetValue(38, 0f);
		}
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		if (Main.LocalPlayer.chest > -1 || Main.LocalPlayer.talkNPC > -1)
		{
			Instance.Close();
		}
		Calculation();
		if (ShouldSetupShopItems)
		{
			SetupShopItems();
		}
	}

	public void SetupShopItems()
	{
		FurnaceScoreShop.SellPricesInFurnaceScore = new Dictionary<int, int>();
		for (int i = 0; i < FurnaceScoreShopItemSlots.Length; i++)
		{
			int rewardType = 0;
			switch (i)
			{
				case 0:
					rewardType = ModContent.ItemType<MagicalBoomerang>();
					FurnaceScoreShop.SellPricesInFurnaceScore[rewardType] = 14500;
					FurnaceScoreShopItemSlots[i].ShopSlot = true;
					break;
				case 1:
					rewardType = ModContent.ItemType<PearShapedNeedle>();
					FurnaceScoreShop.SellPricesInFurnaceScore[rewardType] = 15500;
					FurnaceScoreShopItemSlots[i].ShopSlot = true;
					break;
				case 2:
					rewardType = ModContent.ItemType<ChainGrenade>();
					FurnaceScoreShop.SellPricesInFurnaceScore[rewardType] = 17000;
					FurnaceScoreShopItemSlots[i].ShopSlot = true;
					break;
				case 3:
					rewardType = ModContent.ItemType<ArmorPiercingBlaster>();
					FurnaceScoreShop.SellPricesInFurnaceScore[rewardType] = 14000;
					FurnaceScoreShopItemSlots[i].ShopSlot = true;
					break;
			}
			Item item = new Item();
			item.SetDefaults(rewardType);

			// TODO: Need a modded itemTooltip allocating feature. Now use 36 for Furnace Score Shop Itemslot.
			item.tooltipContext = 36;
			FurnaceScoreShopItemSlots[i].ContainedItem = item;
		}
		ShouldSetupShopItems = false;
	}

	public override void Draw(SpriteBatch sb)
	{
		int score = Main.LocalPlayer.GetModPlayer<FurnacePlayer>().CurrentFurnaceScore;
		Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, "Furnace Score Shop & Milestone Reward", 504f, Info.Top.Pixel + 18, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
		Texture2D tex = ModAsset.FurnaceScoreIcon.Value;
		sb.Draw(tex, new Vector2(30f, Info.TotalHitBox.Bottom - 100), null, Color.White, 0, tex.Size() * 0.5f, 1f, SpriteEffects.None, 0);
		Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, "   : " + score, 20f, Info.TotalHitBox.Bottom - 110, Color.White * (Main.mouseTextColor / 255f), Color.Black, Vector2.Zero, 1f);
		base.Draw(sb);
	}
}
