using Everglow.Commons.Mechanics.Mission.UI;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements
{
	internal class UIMissionListScrollbar : UIVerticalScrollbar
	{
		public override float Scale => 1f;

		public override float TopMax => 4f * Scale;

		public override float TopMin => 4f * Scale;

		private UIBlock _bar = new UIBlock();
		private UIBlock _inner = new UIBlock();

		public UIMissionListScrollbar()
		{
			Info.Width.SetValue(8f, 0f);
			_innerScale = new Vector2(12f, 46f) * MissionContainer.Scale;
			AlwaysOnLight = true;
			_bar.PanelColor = Color.Transparent;
			_inner.PanelColor = Color.Transparent;
			_inner.ShowBorder = (false, false, false, false);
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

			bool isMouseHover = ContainsPoint(Main.MouseScreen);
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
			var innerHeight = UIScrollbarInnerTexture.Height * _innerScale.Y;
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			var top = TopMin + height * WheelValue;
			var p = new Vector2(
				Info.TotalLocation.X - (UIScrollbarInnerTexture.Width * _innerScale.X - Info.TotalSize.X) / 2f,
				Info.TotalLocation.Y + top);
			var size = UIScrollbarInnerTexture.Size() * _innerScale;
			return base.ContainsPoint(point) || new Rectangle((int)Math.Round(p.X), (int)Math.Round(p.Y),
				(int)Math.Round(size.X), (int)Math.Round(size.Y)).Contains(point.X, point.Y);
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			_inner.Info.TotalHitBox = _bar.Info.TotalHitBox = Info.TotalHitBox;
			// _bar.Draw(sb);

			var innerHeight = _innerScale.Y;
			float height = Info.TotalSize.Y - TopMax - TopMin - innerHeight;
			var top = TopMin + height * WheelValue;
			_inner.Info.TotalHitBox = new Rectangle(
				(int)(Info.TotalLocation.X - (UIScrollbarInnerTexture.Width * _innerScale.X - Info.TotalSize.X) / 2f),
				(int)(Info.TotalLocation.Y + top),
				(int)_innerScale.X,
				(int)_innerScale.Y);
			//_inner.Draw(sb);
		}
	}
}
