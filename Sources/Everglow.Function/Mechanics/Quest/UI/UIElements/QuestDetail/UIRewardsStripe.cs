using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;
using ReLogic.Graphics;
using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIRewardsStripe : UIBlock
{
	private const int Padding = 144;

	private float globalMotionOffset = 0;

	private UIQuestRewardHorizontalScrollbar _scrollBar;

	private List<UIRewardsStripeItem> rewardUIElements;
	private IReadOnlyList<RewardView> rewards = [];

	public IReadOnlyList<RewardView> Rewards
	{
		get => rewards;
		set
		{
			rewards = value ?? [];

			ChildrenElements.Clear();
			rewardUIElements = rewards.Select(reward => new UIRewardsStripeItem(reward)).ToList();
			foreach (UIRewardsStripeItem rewardElement in rewardUIElements)
			{
				Register(rewardElement);
			}
			if (rewardUIElements.Count * Padding > Info.HitBox.Width)
			{
				_scrollBar = new UIQuestRewardHorizontalScrollbar();
				Register(_scrollBar);
				_scrollBar.WheelValue = 0f;
			}
		}
	}

	public void SetRewards(IReadOnlyList<RewardView> rewards)
	{
		Rewards = rewards;
	}

	public override void OnInitialization()
	{
		base.OnInitialization();
		PanelColor = Color.Transparent;
		BorderColor = Color.Transparent;
		Info.SetMargin(0);
		Info.IsSensitive = true;
		Info.HiddenOverflow = true;
		_scrollBar = new UIQuestRewardHorizontalScrollbar();
		Register(_scrollBar);
	}

	public override void Calculation()
	{
		if (_scrollBar is not null && ChildrenElements.Contains(_scrollBar))
		{
			_scrollBar.Info.Width.SetValue(-28, 1f);
			_scrollBar.Info.Height.SetValue(20);
			_scrollBar.Info.Top.SetValue(-60);
			_scrollBar.Info.Left.SetValue(14f);
		}
		base.Calculation();
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (rewardUIElements == null || rewardUIElements.Count == 0)
		{
			return;
		}

		// Update global offset moving to current icon
		float wheelValue = _scrollBar?.WheelValue ?? 0f;
		float targetOffset = -Padding * wheelValue * rewardUIElements.Count + (Info.HitBox.Width + Padding - 128) * wheelValue;

		globalMotionOffset = targetOffset;

		if (Math.Abs(globalMotionOffset - targetOffset) < 0.5f)
		{
			globalMotionOffset = targetOffset;
		}
		for (int i = 0; i < rewardUIElements.Count; i++)
		{
			var scale = QuestContainer.Scale;
			var offsetX = (int)((Padding * i + globalMotionOffset) * QuestContainer.Scale);
			var offsetY = 0;

			var lerpValue = Math.Clamp(Math.Abs(offsetX) / (Padding * scale) - 2, 0, 1);

			var width = 64;

			offsetY += HitBox.Height / 2 - width;

			var icon = rewardUIElements[i];
			icon.BorderColor = Color.Gray;
			icon.Info.HiddenOverflow = true;
			icon.Info.Width.SetValue(width * 2);
			icon.Info.Height.SetValue(width * 2);
			icon.Info.Left.SetValue(offsetX);
			icon.Info.Top.SetValue(offsetY);
			icon.Color = Color.White;
			icon.PanelColor = Color.White;
			icon.Scale = 1;
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);
	}

	protected override void DrawChildren(SpriteBatch sb)
	{
		if (rewardUIElements == null || rewardUIElements.Count == 0)
		{
			return;
		}
		for (int i = 0; i < rewardUIElements.Count; i++)
		{
			rewardUIElements[i].Draw(sb);
		}
		if (ChildrenElements.Contains(_scrollBar))
		{
			_scrollBar.Draw(sb);
		}
	}
}

public class UIRewardsStripeItem : UIBlock
{
	public RewardView Reward { get; }

	public QuestIconBase Icon { get; }

	public Color Color { get; set; } = Color.White;

	public float Scale { get; set; } = 1f;

	public UIRewardsStripeItem(RewardView reward)
	{
		ArgumentNullException.ThrowIfNull(reward);

		Reward = reward;
		Icon = Reward.Item is null
			? TextureQuestIcon.Create(ModAsset.Point.Value)
			: ItemQuestIcon.Create(Reward.Item.type);
		Events.OnMouseHover += Events_OnMouseHover;
	}

	private void Events_OnMouseHover(BaseElement baseElement)
	{
		if (Reward.Item is not null)
		{
			Main.hoverItemName = Reward.Item.Name;
			Main.HoverItem = Reward.Item.Clone();
		}
		else
		{
			QuestContainer.Instance.MouseText = Reward.Description;
		}
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		UIQuestBlock.DrawQuestPanel(sb, Info.TotalHitBox, Color.White * (PanelColor.A / 255f));
		int x = Info.TotalHitBox.X;
		int y = Info.TotalHitBox.Y;
		int w = Info.TotalHitBox.Width;
		int h = Info.TotalHitBox.Height;
		Texture2D tex_side = ModAsset.QuestTopicFrame_bottom_side.Value;
		sb.Draw(tex_side, new Rectangle(x + (w - 51) / 2, y + h - 7, 51, 7), null, Color.White * (PanelColor.A / 255f));
	}

	protected override void DrawChildren(SpriteBatch sb)
	{
		Icon.Draw(sb, HitBox, Color, Scale * 2.5f);

		if (Reward.Item is not null && Reward.Item.stack > 1)
		{
			string stack = Reward.Item.stack.ToString();
			var font = FontAssets.ItemStack.Value;
			var position = new Vector2(HitBox.Right - 8f, HitBox.Bottom - 8f);
			var origin = font.MeasureString(stack);
			sb.DrawString(font, stack, position, Color.White, 0f, origin, Scale, SpriteEffects.None, 0f);
		}
	}
}
