using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail
{
	public class UIQuestRewardHorizontalScrollbar : UIHorizontalScrollbar
	{
		public override float Scale => 1f;

		public override float LeftMax => 4f * Scale;

		public override float LeftMin => 4f * Scale;

		private bool _mouseOver = false;

		/// <summary>
		/// 上下箭头
		/// </summary>
		private UIBlock _scrollbarArrow = new UIBlock();

		/// <summary>
		/// 轨道
		/// </summary>
		private UIBlock _scrollbarTrack = new UIBlock();

		/// <summary>
		/// 滑块
		/// </summary>
		private UIBlock _scrollbarThumb = new UIBlock();

		public UIQuestRewardHorizontalScrollbar()
		{
			Info.Width.SetValue(2f, 0f);
			_innerScale = new Vector2(58f, 22f);
			AlwaysOnLight = true;

			Events.OnMouseHover += e =>
			{
				_mouseOver = true;
			};
			Events.OnMouseOut += e =>
			{
				_mouseOver = false;
			};

			_scrollbarArrow.ShowBorder = _scrollbarTrack.ShowBorder = _scrollbarThumb.ShowBorder = (false, false, false, false);
			_scrollbarArrow.PanelColor = Color.Transparent;
			_scrollbarTrack.PanelColor = Color.Transparent;
			_scrollbarThumb.PanelColor = Color.Transparent;

			var mask = new UIBlock();
			mask.Info.Width.SetValue(4f, 0f);
			mask.Info.Height.SetValue(1f, 0f);
			mask.Info.CanBeInteract = false;
			mask.Info.SetToCenter();
			mask.Info.Top.SetValue(1f, 0f);
			mask.ShowBorder = (false, false, false, false);
			mask.PanelColor = _scrollbarArrow.PanelColor;
			Register(mask);

			mask = new UIBlock();
			mask.Info.Width.SetValue(4f, 0f);
			mask.Info.Height.SetValue(1f, 0f);
			mask.Info.CanBeInteract = false;
			mask.Info.SetToCenter();
			mask.Info.Top.SetValue(-2f, 1f);
			mask.ShowBorder = (false, false, false, false);
			mask.PanelColor = _scrollbarArrow.PanelColor;

			Register(mask);
		}

		public override void Update(GameTime gt)
		{
			ChildrenElements.ForEach(child =>
			{
				if (child != null && child.IsVisible)
				{
					child.Update(gt);
				}
			});

			if (IsVisible)
			{
				Events.Update(this, gt);
			}
			var innerWidth = 1f * _innerScale.X;
			float width = Info.TotalSize.X - LeftMax - LeftMin - innerWidth;
			if (_isMouseDown)
			{
				WheelValue = (Main.mouseX -
					Info.TotalLocation.X - LeftMin - innerWidth / 2f) / width;
			}

			if (_wheelValue != _waitToWheelValue)
			{
				_wheelValue += (_waitToWheelValue - _wheelValue) / 4f;
				Calculation();
			}
		}

		public override bool ContainsPoint(Point point)
		{
			return base.ContainsPoint(point);
		}

		public override void Calculation()
		{
			base.Calculation();
			_scrollbarArrow.Info.TotalHitBox = _scrollbarTrack.Info.TotalHitBox = Info.TotalHitBox;

			var innerWidth = _innerScale.X;
			float width = Info.TotalSize.X - LeftMax - LeftMin - innerWidth;
			var left = LeftMin + width * WheelValue;
			_scrollbarThumb.Info.TotalHitBox = new Rectangle(
				(int)(Info.TotalLocation.X + left),
				(int)(Info.TotalLocation.Y - (_innerScale.Y - Info.TotalSize.Y) / 2f),
				(int)_innerScale.X,
				(int)_innerScale.Y);
			_scrollbarArrow.Info.TotalHitBox.Width = Math.Max(2, _scrollbarThumb.Info.TotalHitBox.X - Info.TotalHitBox.X - 1);
			_scrollbarTrack.Info.TotalHitBox.X = Math.Max(
				_scrollbarTrack.Info.TotalHitBox.Right - Info.TotalHitBox.Width - 2,
				_scrollbarThumb.Info.TotalHitBox.Right + 1);
			_scrollbarTrack.Info.TotalHitBox.Width = Math.Max(2, Info.TotalHitBox.Right - _scrollbarTrack.Info.TotalHitBox.X);
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			var trackTexture = ModAsset.QuestSideRollingGroove_H.Value;
			var trackScale = new Vector2(1, Info.TotalHitBox.Height / (float)trackTexture.Height);
			sb.Draw(trackTexture, Info.TotalHitBox.Left() + new Vector2(0, 0), new Rectangle(0, 0, 6, 7), Color.White, 0, new Vector2(6f, 3.5f), 2, SpriteEffects.None, 0);
			int posY = (int)Info.TotalHitBox.Left().Y;
			int posX = Info.TotalHitBox.X;
			int width = Info.TotalHitBox.Width;
			sb.Draw(trackTexture, new Rectangle(posX, posY - 1, width, 2), new Rectangle(6, 3, 5, 1), Color.White);

			// sb.Draw(trackTexture, Info.HitBox, new Rectangle(6, 3, 5, 1), Color.White * 0.5f);
			sb.Draw(trackTexture, Info.TotalHitBox.Right() + new Vector2(0, 0), new Rectangle(11, 0, 6, 7), Color.White, 0, new Vector2(0f, 3.5f), 2, SpriteEffects.None, 0);
			var thumbTexture = ModAsset.QuestSideRollingBlock_H.Value;
			var thumbFrame = new Rectangle(0, 0, 29, 11);
			if (_mouseOver || _isMouseDown)
			{
				thumbFrame = new Rectangle(0, 11, 29, 11);
			}
			sb.Draw(thumbTexture, _scrollbarThumb.Info.TotalHitBox.Center(), thumbFrame, Color.White, 0, thumbFrame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
		}

		public override void Draw(SpriteBatch sb) => base.Draw(sb);
	}
}
