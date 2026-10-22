using Everglow.Commons.UI.UIElements;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Everglow.Yggdrasil.YggdrasilTown.Items.Fishing.FishingRods;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons;
using Spine;

namespace Everglow.Yggdrasil.YggdrasilTown.UI;

public class FurnaceScoreMilestoneRewardUI : UIBlock
{
	// ==================== UI elements ==================== //
	public FurnaceScoreMilestoneRewardUI_Itemslot[] FurnaceScoreMilestoneRewardSlots;

	public FurnaceScoreMilestoneRewardUI_VerticalScrollbar FurnaceScoreMilestoneRewardScrollbar;

	public static List<int> MilestoneRewards = new List<int> { 1000, 2000, 5000, 7000, 10000, 14000, 18000, 22000, 26000, 35000, 45000, 60000, 80000, 100000 };

	public float WheelValue => FurnaceScoreMilestoneRewardScrollbar.WheelValue;

	public bool ShouldSetupItemSlots => FurnaceScoreMilestoneRewardSlots.Any(slot => (slot.ContainedItem.type == ItemID.None && slot.GetIndex() >= 0 && !Main.LocalPlayer.GetModPlayer<FurnacePlayer>().ReceivedReward[slot.GetIndex()]));

	public override void OnInitialization()
	{
		base.OnInitialization();
		Info.HiddenOverflow = true;
		FurnaceScoreMilestoneRewardSlots = new FurnaceScoreMilestoneRewardUI_Itemslot[14];
		for (int i = 0; i < FurnaceScoreMilestoneRewardSlots.Length; i++)
		{
			var (rewardType, rewardStack) = GetRewardEntry(i);

			Item item = new Item();
			item.SetDefaults(rewardType);
			item.stack = rewardStack;
			FurnaceScoreMilestoneRewardSlots[i] = new FurnaceScoreMilestoneRewardUI_Itemslot()
			{
				SlotBackTexture = ModAsset.FurnaceShop_Normal.Value,
				ContainedItem = item,
				CanPutInSlot = (item) => false,
				CanTakeOutSlot = (item) => false,
				CornerSize = new Vector2(10, 10),
				DrawColor = Color.White,
				Tooltip = "Furnace Score Milestone Reward Slot",
			};
			Register(FurnaceScoreMilestoneRewardSlots[i]);
		}
		FurnaceScoreMilestoneRewardScrollbar = new FurnaceScoreMilestoneRewardUI_VerticalScrollbar();
		Register(FurnaceScoreMilestoneRewardScrollbar);
	}

	public override void Calculation()
	{
		base.Calculation();
		Info.Left.SetValue(314, 0f);
		Info.Top.SetValue(20, 0f);
		Info.Width.SetValue(180, 0f);
		Info.Height.SetValue(166, 0f);
		for (int i = 0; i < FurnaceScoreMilestoneRewardSlots.Length; i++)
		{
			FurnaceScoreMilestoneRewardSlots[i].Info.Left.SetValue(110, 0f);
			FurnaceScoreMilestoneRewardSlots[i].Info.Top.SetValue(10 + (13 - i) * 60 - FurnaceScoreMilestoneRewardScrollbar.WheelValue * 720, 0f);
			FurnaceScoreMilestoneRewardSlots[i].Info.Width.SetValue(40, 0f);
			FurnaceScoreMilestoneRewardSlots[i].Info.Height.SetValue(40, 0f);
			if (YggdrasilTownFurnaceSystem.CurrentScore >= MilestoneRewards[i])
			{
				FurnaceScoreMilestoneRewardSlots[i].SlotBackTexture = ModAsset.FurnaceScoreMilestoneRewardUI_Enable.Value;
			}
			else
			{
				FurnaceScoreMilestoneRewardSlots[i].SlotBackTexture = ModAsset.FurnaceScoreMilestoneRewardUI_Lock.Value;
			}
		}
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		Calculation();
		if (ShouldSetupItemSlots)
		{
			SetupItemSlots();
		}
	}

	public void SetupItemSlots()
	{
		for (int i = 0; i < FurnaceScoreMilestoneRewardSlots.Length; i++)
		{
			var (rewardType, rewardStack) = GetRewardEntry(i);
			if (Main.LocalPlayer.GetModPlayer<FurnacePlayer>().ReceivedReward[i])
			{
				rewardType = 0;
				rewardStack = 1;
			}
			if (FurnaceScoreMilestoneRewardSlots[i].ContainedItem.type == ItemID.None)
			{
				Item item = new Item();
				item.SetDefaults(rewardType);
				item.stack = rewardStack;
				FurnaceScoreMilestoneRewardSlots[i].ContainedItem = item;
			}
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		Rectangle destRec = Info.HitBox;
		destRec.Y -= 2;
		destRec.Height += 2;
		Draw9Piece_Board(sb, destRec, new Color(0.66f, 0.66f, 0.66f, 0.66f));

		// Texture2D tex = ModAsset.FurnaceScoreMilestoneRewardUI_RewardTree.Value;
		// Rectangle bar_background_frame = new Rectangle(12, 1, 3, 5);
		// Rectangle bar_value_frame = new Rectangle(12, 9, 3, 5);
		// int bar_x = (int)(Info.Left.Pixel + 80);
		// int bar_y = (int)(Info.Top.Pixel + Info.Width.Pixel + 80 - FurnaceScoreMilestoneRewardScrollbar.WheelValue * 720);
		// int value = Math.Clamp((int)((100000 - Main.LocalPlayer.GetModPlayer<FurnacePlayer>().TotalFurnaceScore) * 900f), 0, 900);
		// sb.Draw(tex, new Rectangle(bar_x - 3, bar_y, 6, 900), bar_background_frame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
		// sb.Draw(tex, new Rectangle(bar_x - 3, bar_y + value, 6, 900 - value), bar_value_frame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
	}

	public void Draw9Piece_Board(SpriteBatch spriteBatch, Rectangle destinationBox, Color drawColor)
	{
		Texture2D tex = ModAsset.FurnaceShop_Normal.Value;
		int margin = 4;
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y, margin, margin), new Rectangle(0, 0, margin, margin), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + margin, destinationBox.Y, destinationBox.Width - margin * 2, margin), new Rectangle(margin, 0, 2, margin), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - margin, destinationBox.Y, margin, margin), new Rectangle(tex.Width - margin, 0, margin, margin), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + margin, margin, destinationBox.Height - margin * 2), new Rectangle(0, margin, margin, 2), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + margin, destinationBox.Y + margin, destinationBox.Width - margin * 2, destinationBox.Height - margin * 2), new Rectangle(margin, margin, 2, 2), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - margin, destinationBox.Y + margin, margin, destinationBox.Height - margin * 2), new Rectangle(tex.Width - margin, margin, margin, 2), drawColor);

		spriteBatch.Draw(tex, new Rectangle(destinationBox.X, destinationBox.Y + destinationBox.Height - margin, margin, margin), new Rectangle(0, tex.Height - margin, margin, margin), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + margin, destinationBox.Y + destinationBox.Height - margin, destinationBox.Width - margin * 2, margin), new Rectangle(margin, tex.Height - margin, 2, margin), drawColor);
		spriteBatch.Draw(tex, new Rectangle(destinationBox.X + destinationBox.Width - margin, destinationBox.Y + destinationBox.Height - margin, margin, margin), new Rectangle(tex.Width - margin, tex.Height - margin, margin, margin), drawColor);
	}

	private static (int ItemType, int Stack) GetRewardEntry(int index) => index switch
	{
		0 => (ModContent.ItemType<ThermostatRod>(), 1),
		1 => (ModContent.ItemType<HeatEmblem>(), 1),
		2 => (ModContent.ItemType<MelterGear>(), 1),
		3 => (ModContent.ItemType<ThermalConductor>(), 1),
		4 => (ModContent.ItemType<HotAirBalloon>(), 1),
		5 => (ModContent.ItemType<SmeltingStaff>(), 1),
		6 => (ModContent.ItemType<ThermoprobeStaff>(), 1),
		7 => (ModContent.ItemType<MoltenCore>(), 1),
		8 => (ItemID.PlatinumCoin, 1),
		9 => (ItemID.PlatinumCoin, 2),
		10 => (ItemID.PlatinumCoin, 3),
		11 => (ItemID.PlatinumCoin, 5),
		12 => (ItemID.PlatinumCoin, 10),
		13 => (ItemID.PlatinumCoin, 15),
		_ => (ItemID.None, 0),
	};
}
