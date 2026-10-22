using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Miscs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables.Furniture.Furnace;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.FurnaceTiles;
using Terraria.GameContent;

namespace Everglow.Yggdrasil.YggdrasilTown.UI;

public class FurnaceScoreShopUI : SpecialShopUI
{
	public static FurnaceScoreShopUI Instance => SpecialShopSystem.Instance.Get<FurnaceScoreShopUI>();

	// ==================== UI elements ==================== //
	public FurnaceScoreShop_Itemslot[] FurnaceScoreShopItemSlots;

	public FurnaceScoreMilestoneRewardUI _mileStoneRewardUI;

	public bool ShouldSetupShopItems = true;

	public override void OnInitialization()
	{
		base.OnInitialization();
		FurnaceScoreShopItemSlots = new FurnaceScoreShop_Itemslot[GetShopEntries().Count];
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
		Info.Height.SetValue(Math.Max(300, 130 + (FurnaceScoreShopItemSlots.Length + 6) / 7 * 42), 0f);
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
			Close();
		}
		Calculation();
		if (ShouldSetupShopItems)
		{
			SetupShopItems();
		}
	}

	public void SetupShopItems()
	{
		var entries = GetShopEntries();
		FurnaceScoreShop.SellPricesInFurnaceScore.Clear();
		for (int i = 0; i < entries.Count; i++)
		{
			var entry = entries[i];
			FurnaceScoreShop.SellPricesInFurnaceScore.Add(entry.ItemType, entry.Price);
			FurnaceScoreShopItemSlots[i].ShopSlot = true;
			Item item = new Item(entry.ItemType);

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

	private static IReadOnlyList<(int ItemType, int Price)> GetShopEntries() =>
	[
		(ModContent.ItemType<MagicalBoomerang>(), 14500),
		(ModContent.ItemType<PearShapedNeedle>(), 15500),
		(ModContent.ItemType<ChainGrenade>(), 17000),
		(ModContent.ItemType<ArmorPiercingBlaster>(), 14000),
		(ModContent.ItemType<Caltrop>(), 12800),
		(ModContent.ItemType<AnemoShell>(), 26200),
		(ModContent.ItemType<TelescopicPlatformAlpha>(), 75000),
		(ModContent.ItemType<TelescopicPlatformBeta>(), 900000),
		// Furniture prices are provisional; each purchase delivers one piece.
		(ModContent.ItemType<HeatproofBathtub_item>(), 800),
		(ModContent.ItemType<HeatproofBed_item>(), 800),
		(ModContent.ItemType<HeatproofBookcase_item>(), 800),
		(ModContent.ItemType<HeatproofCandelabra_item>(), 300),
		(ModContent.ItemType<HeatproofCandle_item>(), 100),
		(ModContent.ItemType<HeatproofChair_item>(), 200),
		(ModContent.ItemType<HeatproofChandelier_item>(), 500),
		(ModContent.ItemType<HeatproofChest_item>(), 500),
		(ModContent.ItemType<HeatproofClock_item>(), 1000),
		(ModContent.ItemType<HeatproofDoor_item>(), 300),
		(ModContent.ItemType<HeatproofDresser_item>(), 800),
		(ModContent.ItemType<HeatproofLamp_item>(), 300),
		(ModContent.ItemType<HeatproofLantern_item>(), 300),
		(ModContent.ItemType<HeatproofPiano_item>(), 1000),
		(ModContent.ItemType<HeatproofSink_item>(), 500),
		(ModContent.ItemType<HeatproofSofa_item>(), 500),
		(ModContent.ItemType<HeatproofTable_item>(), 500),
		(ModContent.ItemType<HeatproofToilet_item>(), 300),
		(ModContent.ItemType<HeatproofWorkbench_item>(), 200),
		(ModContent.ItemType<ElectroProbe>(), 5000),
		// Match the existing furnace weapon price band until balance testing.
		(ModContent.ItemType<IntroductiontoThermalElectricity>(), 14000),
		(ModContent.ItemType<MeltingSideGyroscope>(), 17000),
		(ModContent.ItemType<QuenchingBlade>(), 15000),
		(ModContent.ItemType<ResistanceWireBayonet>(), 15500),
	];
}
