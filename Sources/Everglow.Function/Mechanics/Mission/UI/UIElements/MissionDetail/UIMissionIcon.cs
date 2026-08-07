using Everglow.Commons.Mechanics.Mission.Presentation.Icons;
using Everglow.Commons.UI.UIElements;
using static Everglow.Commons.Mechanics.Mission.UI.MissionContainer;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;

public class UIMissionIcon : UIBlock
{
	private const int ButtonSize = 50;
	private const int ButtonTop = 79;
	private const int ButtonLeftRight = 10;

	private UIMissionCarousel carousel;
	private UIBlock prevBtn;
	private UIBlock nextBtn;
	private MissionIconGroup iconGroup;

	public MissionIconGroup IconGroup => iconGroup;

	public UIMissionIcon(MissionIconGroup iconGroup)
	{
		this.iconGroup = iconGroup;
	}

	public void SetIconGroup(MissionIconGroup iconGroup)
	{
		this.iconGroup = iconGroup;
		carousel.IconGroup = iconGroup;

		if (iconGroup != null && IconGroup.IconCount > 1)
		{
			prevBtn.Info.CanBeInteract = true;
			prevBtn.Info.IsHidden = false;
			prevBtn.Info.IsVisible = true;

			nextBtn.Info.CanBeInteract = true;
			nextBtn.Info.IsHidden = false;
			nextBtn.Info.IsVisible = true;
		}
		else
		{
			prevBtn.Info.CanBeInteract = false;
			prevBtn.Info.IsHidden = true;
			prevBtn.Info.IsVisible = false;

			nextBtn.Info.CanBeInteract = false;
			nextBtn.Info.IsHidden = true;
			nextBtn.Info.IsVisible = false;
		}
	}

	public override void OnInitialization()
	{
		base.OnInitialization();
		PanelColor = Color.Transparent;
		BorderColor = Color.Transparent;
		Info.SetMargin(0);
		Info.IsSensitive = true;

		carousel = new UIMissionCarousel();
		carousel.Info.HiddenOverflow = true;
		Register(carousel);

		prevBtn = new UIBlock();
		prevBtn.Info.CanBeInteract = true;
		prevBtn.Info.IsHidden = true;
		prevBtn.Info.IsVisible = false;
		prevBtn.Info.IsSensitive = true;
		prevBtn.PanelColor = Color.Transparent;
		prevBtn.BorderWidth = 0;
		prevBtn.Events.OnLeftDown += e =>
		{
			if (!IconGroup.IsFirstIcon)
			{
				IconGroup.Prev();
			}
		};
		Register(prevBtn);
		var prevIcon = new UIImage(ModAsset.MissionIconArrow.Value, Color.White);
		prevBtn.Register(prevIcon);
		prevIcon.SourceRectangle = new Rectangle(0, 0, 35, 25);
		prevIcon.Info.SetToCenter();

		nextBtn = new UIBlock();
		nextBtn.Info.CanBeInteract = true;
		nextBtn.Info.IsHidden = true;
		nextBtn.Info.IsVisible = false;
		nextBtn.Info.IsSensitive = true;
		nextBtn.PanelColor = Color.Transparent;
		nextBtn.BorderWidth = 0;
		nextBtn.Events.OnLeftDown += e =>
		{
			if (!IconGroup.IsLastIcon)
			{
				IconGroup.Next();
			}
		};
		Register(nextBtn);
		var nextIcon = new UIImage(ModAsset.MissionIconArrow.Value, Color.White);
		nextBtn.Register(nextIcon);
		nextIcon.SourceRectangle = new Rectangle(105, 0, 35, 25);
		nextIcon.Info.SetToCenter();
		prevIcon.Info.Width.SetValue(70);
		prevIcon.Info.SetToCenter();
		nextIcon.Info.Width.SetValue(70);
		nextIcon.Info.SetToCenter();

		prevBtn.Events.OnMouseHover += e => PrevBtnMouseHover(prevIcon, nextIcon);
		prevBtn.Events.OnMouseOut += e => PrevBtnMouseOut(prevIcon, nextIcon);
		nextBtn.Events.OnMouseHover += e => NextBtnMouseHover(prevIcon, nextIcon);
		nextBtn.Events.OnMouseOut += e => NextBtnMouseOut(prevIcon, nextIcon);
	}

	public void PrevBtnMouseHover(UIImage prevIcon, UIImage nextIcon)
	{
		prevIcon.SourceRectangle = new Rectangle(0, 25, 35, 25);
		prevIcon.Color = Color.White;
		prevIcon.Info.Width.SetValue(70);
		prevIcon.Info.SetToCenter();
		if (IconGroup.IsFirstIcon)
		{
			prevIcon.SourceRectangle = new Rectangle(0, 0, 35, 25);
			prevIcon.Color = Color.Gray;
		}
		else
		{
			prevIcon.Info.Left.SetValue(nextIcon.Info.Left.Pixel - MathF.Sin((float)Main.time * 0.1f) * 10);
		}
		if (!IconGroup.IsLastIcon)
		{
			nextIcon.SourceRectangle = new Rectangle(105, 0, 35, 25);
			nextIcon.Color = Color.White * 0.75f;
		}
	}

	public void PrevBtnMouseOut(UIImage prevIcon, UIImage nextIcon)
	{
		prevIcon.SourceRectangle = new Rectangle(0, 0, 35, 25);
		prevIcon.Color = Color.White * 0.75f;
		if (IconGroup.IsFirstIcon)
		{
			prevIcon.Color = Color.Gray;
		}
		prevIcon.Info.Width.SetValue(70);
		prevIcon.Info.SetToCenter();
	}

	public void NextBtnMouseHover(UIImage prevIcon, UIImage nextIcon)
	{
		nextIcon.SourceRectangle = new Rectangle(105, 25, 35, 25);
		nextIcon.Color = Color.White;
		nextIcon.Info.Width.SetValue(70);
		nextIcon.Info.SetToCenter();
		if (IconGroup.IsLastIcon)
		{
			nextIcon.SourceRectangle = new Rectangle(105, 0, 35, 25);
			nextIcon.Color = Color.Gray;
		}
		else
		{
			nextIcon.Info.Left.SetValue(nextIcon.Info.Left.Pixel + MathF.Sin((float)Main.time * 0.1f) * 10);
		}
		if (!IconGroup.IsFirstIcon)
		{
			prevIcon.SourceRectangle = new Rectangle(0, 0, 35, 25);
			prevIcon.Color = Color.White * 0.75f;
		}
	}

	public void NextBtnMouseOut(UIImage prevIcon, UIImage nextIcon)
	{
		nextIcon.SourceRectangle = new Rectangle(105, 0, 35, 25);
		nextIcon.Color = Color.White * 0.75f;
		if (IconGroup.IsLastIcon)
		{
			nextIcon.Color = Color.Gray;
		}
		nextIcon.Info.Width.SetValue(70);
		nextIcon.Info.SetToCenter();
	}

	public override void Calculation()
	{
		base.Calculation();

		carousel.Info.Width.SetFull();
		carousel.Info.Height.SetFull();

		int arrow_offset_H = 180;

		prevBtn.Info.Width.SetValue(70 * Scale);
		prevBtn.Info.Height.SetValue(50 * Scale);
		prevBtn.Info.Left.SetValue((-70 - arrow_offset_H) * Scale, 0.5f);
		prevBtn.Info.Top.SetValue(103 * Scale);

		nextBtn.Info.Width.SetValue(70 * Scale);
		nextBtn.Info.Height.SetValue(50 * Scale);
		nextBtn.Info.Left.SetValue(arrow_offset_H * Scale, 0.5f);
		nextBtn.Info.Top.SetValue(103 * Scale);
	}

	public class UIMissionCarousel : BaseElement
	{
		private const int Padding = 144;
		private const int MaxWidth = 128;
		private const int MinWidth = 64;

		private float globalMotionOffset = 0;

		private List<UIMissionIconCarouselItem> iconUIElements;
		private MissionIconGroup iconGroup;

		public MissionIconGroup IconGroup
		{
			get => iconGroup;
			set
			{
				iconGroup = value;

				ChildrenElements.Clear();
				if (iconGroup != null)
				{
					iconUIElements = iconGroup.Icons.Select(icon => new UIMissionIconCarouselItem(icon)).ToList();
					foreach (var iE in iconUIElements)
					{
						Register(iE);
					}
				}
			}
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);

			if (IconGroup == null)
			{
				return;
			}

			// Update global offset moving to current icon
			float deltaTime = (float)gt.ElapsedGameTime.TotalSeconds;
			float targetOffset = -Padding * IconGroup.CurrentIndex;

			float lerpFactor = 1f - (float)Math.Pow(0.01f, deltaTime);
			globalMotionOffset = MathHelper.Lerp(globalMotionOffset, targetOffset, lerpFactor);

			if (Math.Abs(globalMotionOffset - targetOffset) < 0.5f)
			{
				globalMotionOffset = targetOffset;
			}

			for (int i = 0; i < iconUIElements.Count; i++)
			{
				var scale = MissionContainer.Scale;
				var offsetX = (int)((Padding * i + globalMotionOffset) * MissionContainer.Scale);
				var offsetY = 0;

				var lerpValue = Math.Clamp(Math.Abs(offsetX) / (Padding * scale), 0, 1);
				var width = (int)MathHelper.Lerp((int)(MaxWidth * scale), (int)(MinWidth * scale), lerpValue);

				float drawColorV = (1 - lerpValue) * 0.6f + 0.4f;
				float alpha = (1 - lerpValue) * 0.7f + 0.3f;
				var drawColor = new Color(drawColorV, drawColorV, drawColorV, alpha);

				float bgCV = lerpValue * 0.04f + 0.2f;
				float bgA = (1 - lerpValue) * 0.7f + 0.3f;
				var bgC = new Color(bgCV, bgCV, bgCV, bgA);

				offsetX += HitBox.Width / 2 - width;
				offsetY += HitBox.Height / 2 - width;

				var icon = iconUIElements[i];
				icon.PanelColor = bgC;
				icon.BorderColor = Color.Gray;
				icon.Info.HiddenOverflow = true;
				icon.Info.Width.SetValue(width * 2);
				icon.Info.Height.SetValue(width * 2);
				icon.Info.Left.SetValue(offsetX);
				icon.Info.Top.SetValue(offsetY);
				icon.Color = drawColor;
				icon.Scale = width / (float)MaxWidth;
			}
		}

		public override void Draw(SpriteBatch sb)
		{
			if (iconGroup is not null && iconGroup.IconCount > 1)
			{
				Texture2D tex = ModAsset.MissionIconArrow.Value;
				int centerY = (int)Info.HitBox.Center().Y;
				sb.Draw(tex, new Rectangle(Info.HitBox.X, centerY - 1, Info.HitBox.Width, 2), new Rectangle(31, 12, 1, 1), Color.White);
			}

			// TODO: Add MissionStar to a mission(default 1);
			int MissionStar = 5;
			for (int k = 0; k < MissionStar; k++)
			{
				Texture2D star = ModAsset.MissionLevelStar.Value;
				Vector2 pos = Info.HitBox.Bottom() + new Vector2((k - (MissionStar - 1) / 2f) * 50, 24);
				sb.Draw(star, pos, null, Color.White, 0, star.Size() * 0.5f, 2f, SpriteEffects.None, 0);
			}
			base.Draw(sb);
		}

		protected override void DrawChildren(SpriteBatch sb)
		{
			if (IconGroup == null || IconGroup.IconCount == 0)
			{
				return;
			}

			int centerIndex = 0;
			float minValue = 1f;

			for (int i = 0; i < iconGroup.IconCount; i++)
			{
				var offsetX = Padding * i + globalMotionOffset;
				var value = Math.Clamp(Math.Abs(offsetX) / (Padding * 0.9f), 0, 1);
				if (value < minValue)
				{
					minValue = value;
					centerIndex = i;
				}
			}

			for (int i = 0; i < iconUIElements.Count; i++)
			{
				if (i != centerIndex)
				{
					iconUIElements[i].Draw(sb);
				}
			}

			iconUIElements[centerIndex].Draw(sb);
		}
	}

	public class UIMissionIconCarouselItem : UIBlock
	{
		private readonly MissionIconBase _icon;

		public Color Color { get; set; } = Color.White;

		public float Scale { get; set; } = 1f;

		public UIMissionIconCarouselItem(MissionIconBase icon)
		{
			_icon = icon;
			Events.OnMouseHover += Events_OnMouseHover;
		}

		private void Events_OnMouseHover(BaseElement baseElement)
		{
			MissionContainer.Instance.MouseText = _icon.Tooltip;
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			UIMissionBlock.DrawMissionPanel(sb, Info.TotalHitBox, Color.White * (PanelColor.A / 255f));
			int x = Info.TotalHitBox.X;
			int y = Info.TotalHitBox.Y;
			int w = Info.TotalHitBox.Width;
			int h = Info.TotalHitBox.Height;
			Texture2D tex_side = ModAsset.MissionTopicFrame_bottom_side.Value;
			sb.Draw(tex_side, new Rectangle(x + (w - 51) / 2, y + h - 7, 51, 7), null, Color.White * (PanelColor.A / 255f));
		}

		protected override void DrawChildren(SpriteBatch sb)
		{
			_icon.Draw(sb, HitBox, Color, Scale * 2.5f);
		}
	}
}
