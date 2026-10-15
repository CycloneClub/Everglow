using Everglow.Commons.UI.UIElements;
using Terraria.GameContent;
using Terraria.UI.Chat;

namespace Everglow.Yggdrasil.YggdrasilTown.UI;

public class FurnaceScoreMilestoneRewardUI_Itemslot : UIItemSlot
{
	public override void OnInitialization()
	{
		base.OnInitialization();
		CanTakeOutSlot += CanTakeOut;
		OnPickItem += (target) =>
		{
			if (ParentElement is FurnaceScoreMilestoneRewardUI rewardUI)
			{
				int index = GetIndex();
				if (index >= 0 && index < FurnaceScoreMilestoneRewardUI.MilestoneRewards.Count)
				{
					int milestoneScore = FurnaceScoreMilestoneRewardUI.MilestoneRewards[index];
					if (Main.LocalPlayer.GetModPlayer<FurnacePlayer>().TotalFurnaceScore >= milestoneScore)
					{
						Main.LocalPlayer.GetModPlayer<FurnacePlayer>().ReceivedReward[index] = true;
					}
				}
			}
		};
	}

	public bool CanTakeOut(Item item)
	{
		if (ParentElement is FurnaceScoreMilestoneRewardUI rewardUI)
		{
			int index = GetIndex();
			if (index >= 0 && index < FurnaceScoreMilestoneRewardUI.MilestoneRewards.Count)
			{
				int milestoneScore = FurnaceScoreMilestoneRewardUI.MilestoneRewards[index];
				return Main.LocalPlayer.GetModPlayer<FurnacePlayer>().TotalFurnaceScore >= milestoneScore;
			}
		}
		return false;
	}

	public override void Update(GameTime gameTime)
	{
		if (ContainedItem.type > ItemID.None && Main.LocalPlayer.GetModPlayer<FurnacePlayer>().ReceivedReward[GetIndex()])
		{
			ContainedItem = new Item();
			ContainedItem.SetDefaults(ItemID.None, true);
		}
		base.Update(gameTime);
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		int index = GetIndex();
		FurnacePlayer fPlayer = Main.LocalPlayer.GetModPlayer<FurnacePlayer>();
		int score = fPlayer.TotalFurnaceScore;
		int rewardThreod = FurnaceScoreMilestoneRewardUI.MilestoneRewards[index];
		if (score > rewardThreod)
		{
			SlotBackTexture = ModAsset.FurnaceScoreMilestoneRewardUI_Enable.Value;
			if (fPlayer.ReceivedReward[index])
			{
				SlotBackTexture = ModAsset.FurnaceScoreMilestoneRewardUI_Used.Value;
			}
		}
		base.DrawSelf(sb);

		if (ParentElement is FurnaceScoreMilestoneRewardUI rewardUI)
		{
			Texture2D tex = ModAsset.FurnaceScoreMilestoneRewardUI_RewardTree.Value;
			Rectangle bar_background_frame = new Rectangle(12, 1, 3, 5);
			Rectangle bar_value_frame = new Rectangle(12, 9, 3, 5);
			Rectangle bar_top_frame = new Rectangle(12, 7, 3, 1);
			Rectangle joint = new Rectangle(0, 1, 11, 5);
			Rectangle connectLine = new Rectangle(0, 16, 15, 1);
			int revertIndex = 13 - index;
			int bar_x = Info.TotalHitBox.Left - 30;
			int bar_y = Info.TotalHitBox.Top + 15;
			int min = 0;
			if (index >= 1)
			{
				min = FurnaceScoreMilestoneRewardUI.MilestoneRewards[index - 1];
			}
			int max = rewardThreod;
			int range = max - min;
			int value = Math.Clamp((int)((score - min) / (float)range * 60f), 0, 60);
			sb.Draw(tex, new Rectangle(bar_x - 3, bar_y, 6, 56), bar_background_frame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			sb.Draw(tex, new Rectangle(bar_x - 3, bar_y + 56 - value, 6, Math.Max(value, 0)), bar_value_frame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			if (value is > 0 and < 60)
			{
				sb.Draw(tex, new Rectangle(bar_x - 3, bar_y + 56 - value, 6, 2), bar_top_frame, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			}
			if (score >= rewardThreod)
			{
				joint = new Rectangle(0, 9, 11, 5);
				connectLine = new Rectangle(0, 19, 15, 1);
				if (fPlayer.ReceivedReward[index])
				{
					connectLine = new Rectangle(0, 22, 15, 1);
				}
			}
			sb.Draw(tex, new Rectangle(bar_x - 11, bar_y - 5, 22, 10), joint, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
			sb.Draw(tex, new Rectangle(bar_x + 12, bar_y - 1, 18, 2), connectLine, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);

			Texture2D tick = ModAsset.FurnaceScoreMilestoneRewardUI_Used_Icon.Value;
			sb.Draw(tick, Info.TotalHitBox.Center(), null, Color.White, 0f, tick.Size() * 0.5f, 1f, SpriteEffects.None, 0f);
		}

		Color texColor = Color.White;
		Color texBoundColor = Color.Black;
		if (score < rewardThreod)
		{
			Texture2D texLock = ModAsset.FurnaceScoreMilestoneRewardUI_Lock.Value;
			var DrawRectangle = Info.TotalHitBox;
			DrawAdvBox(sb, DrawRectangle.X, DrawRectangle.Y,
				DrawRectangle.Width, DrawRectangle.Height,
				DrawColor * 0.75f, ModAsset.FurnaceScoreMilestoneRewardUI_Lock.Value, CornerSize, 1f);
			texColor = new Color(0.24f, 0.2f, 0.2f, 1);
			texBoundColor = new Color(0.12f, 0.12f, 0.12f, 1f);
		}
		string neededScore = FurnaceScoreMilestoneRewardUI.MilestoneRewards[GetIndex()].ToString();
		Vector2 textSize = ChatManager.GetStringSize(FontAssets.MouseText.Value, neededScore, Vector2.One, -1f);
		Utils.DrawBorderStringFourWay(Main.spriteBatch, FontAssets.MouseText.Value, neededScore, Info.TotalHitBox.X - 40 - textSize.X, Info.TotalHitBox.Center().Y - 15, texColor, texBoundColor, Vector2.Zero, 1f);
	}

	public int GetIndex()
	{
		if (ParentElement is FurnaceScoreMilestoneRewardUI rewardUI)
		{
			for (int i = 0; i < rewardUI.FurnaceScoreMilestoneRewardSlots.Length; i++)
			{
				if (rewardUI.FurnaceScoreMilestoneRewardSlots[i] == this)
				{
					return i;
				}
			}
		}
		return -1;
	}
}
