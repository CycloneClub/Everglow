using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail
{
	public class UIQuestTextVerticalScrollbar : UIVerticalScrollbar
	{
		public override float Scale => 1f;

		public override float TopMax => 4f * Scale;

		public override float TopMin => 4f * Scale;

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

		public UIQuestTextVerticalScrollbar()
		{
			Info.Width.SetValue(2f, 0f);
			_innerScale = new Vector2(22f, 50f);
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

			var innerHeight = 1f * _innerScale.Y;
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			if (_isMouseDown)
			{
				WheelValue = (Main.mouseY -
					Info.TotalLocation.Y - TopMin - innerHeight / 2f) / height;
			}

			if (_wheelValue != _waitToWheelValue)
			{
				_wheelValue += (_waitToWheelValue - _wheelValue) / 4f;
				Calculation();
			}
		}

		public override bool ContainsPoint(Point point)
		{
			Rectangle containBar = Info.HitBox;
			containBar.X -= 10;
			containBar.Width += 20;
			return base.ContainsPoint(point) || containBar.Contains(point.X, point.Y);
		}

		public override void Calculation()
		{
			base.Calculation();
			_scrollbarArrow.Info.TotalHitBox = _scrollbarTrack.Info.TotalHitBox = Info.TotalHitBox;

			var innerHeight = _innerScale.Y;
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			var top = TopMin + height * WheelValue;
			_scrollbarThumb.Info.TotalHitBox = new Rectangle(
				(int)(Info.TotalLocation.X - (UIScrollbarInnerTexture.Width * _innerScale.X - Info.TotalSize.X) / 2f),
				(int)(Info.TotalLocation.Y + top),
				(int)_innerScale.X,
				(int)_innerScale.Y);
			_scrollbarArrow.Info.TotalHitBox.Height = Math.Max(2, _scrollbarThumb.Info.TotalHitBox.Y - Info.TotalHitBox.Y - 1);
			_scrollbarTrack.Info.TotalHitBox.Y = Math.Max(
				_scrollbarTrack.Info.TotalHitBox.Bottom - Info.TotalHitBox.Height - 2,
				_scrollbarThumb.Info.TotalHitBox.Bottom + 1);
			_scrollbarTrack.Info.TotalHitBox.Height = Math.Max(2, Info.TotalHitBox.Bottom - _scrollbarTrack.Info.TotalHitBox.Y);
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			var trackTexture = ModAsset.QuestSideRollingGroove.Value;
			var trackScale = new Vector2(1, Info.TotalHitBox.Height / (float)trackTexture.Height);
			sb.Draw(trackTexture, Info.TotalHitBox.Top() + new Vector2(0, -8), new Rectangle(0, 0, 7, 6), Color.White, 0, new Vector2(3.5f, 0), 2, SpriteEffects.None, 0);
			sb.Draw(trackTexture, new Rectangle(Info.TotalHitBox.Left - 6, Info.TotalHitBox.Top + 4, 14, Info.TotalHitBox.Height - 10), new Rectangle(0, 7, 7, 6), Color.White);
			sb.Draw(trackTexture, Info.TotalHitBox.Top() + new Vector2(0, Info.TotalHitBox.Height - 6), new Rectangle(0, 14, 7, 7), Color.White, 0, new Vector2(3.5f, 0), 2, SpriteEffects.None, 0);
			var thumbTexture = ModAsset.QuestSideRollingBlock.Value;
			var thumbFrame = new Rectangle(0, 0, 11, 25);
			if (_mouseOver || _isMouseDown)
			{
				thumbFrame = new Rectangle(11, 0, 11, 25);
			}
			sb.Draw(thumbTexture, _scrollbarThumb.Info.TotalHitBox.Center(), thumbFrame, Color.White, 0, thumbFrame.Size() * 0.5f, 2f, SpriteEffects.None, 0);
		}
	}
}
