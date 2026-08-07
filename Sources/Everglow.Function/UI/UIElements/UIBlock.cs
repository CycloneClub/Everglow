using Spine;
using Terraria.GameContent;

namespace Everglow.Commons.UI.UIElements
{
	public class UIBlock : BaseElement
	{
		public bool CanDrag = false;
		private bool dragging = false;

		public bool CanLeftResize = false;
		private bool leftResizing = false;
		public bool CanRightResize = false;
		private bool rightResizing = false;
		public bool CanTopResize = false;
		private bool topResizing = false;
		public bool CanBottomResize = false;
		private bool bottomResizing = false;

		public int MinWidthPixel = 100;
		public int MinHeightPixel = 100;

		public Color PanelColor = new Color(71, 92, 172);
		public Color BorderColor = Color.Black;
		public (bool LeftBorder, bool TopBorder, bool RightBorder, bool BottomBorder) ShowBorder = (true, true, true, true);
		public int BorderWidth = 2;
		private Vector2 _mousePosCache = Vector2.Zero;

		public UIBlock()
		{
			Info.SetMargin(2f);
		}

		public override void LoadEvents()
		{
			base.LoadEvents();
			Events.OnMouseHover += (element) =>
			{
				Vector2 mouseScreenCenterPixelPos = Main.MouseScreen - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
				if (CanLeftResize && Math.Abs(mouseScreenCenterPixelPos.X - Info.Left.Pixel) < 6)
				{
					UISystem.Instance.LeftResizing = true;
				}
				if (CanRightResize && Math.Abs(mouseScreenCenterPixelPos.X - (Info.Left.Pixel + Info.TotalSize.X)) < 6)
				{
					UISystem.Instance.RightResizing = true;
				}
				if (CanTopResize && Math.Abs(mouseScreenCenterPixelPos.Y - Info.Top.Pixel) < 6)
				{
					UISystem.Instance.TopResizing = true;
				}
				if (CanBottomResize && Math.Abs(mouseScreenCenterPixelPos.Y - (Info.Top.Pixel + Info.TotalSize.Y)) < 6)
				{
					UISystem.Instance.BottomResizing = true;
				}
			};
			Events.OnLeftDown += (element) =>
			{
				Vector2 mouseScreenCenterPixelPos = Main.MouseScreen - new Vector2(Main.screenWidth, Main.screenHeight) * 0.5f;
				if (Math.Abs(mouseScreenCenterPixelPos.X - Info.Left.Pixel) < 6)
				{
					leftResizing = true;
					_mousePosCache = Main.MouseScreen;
				}
				if (Math.Abs(mouseScreenCenterPixelPos.X - (Info.Left.Pixel + Info.TotalSize.X)) < 6)
				{
					rightResizing = true;
					_mousePosCache = Main.MouseScreen;
				}
				if (Math.Abs(mouseScreenCenterPixelPos.Y - Info.Top.Pixel) < 6)
				{
					topResizing = true;
					_mousePosCache = Main.MouseScreen;
				}
				if (Math.Abs(mouseScreenCenterPixelPos.Y - (Info.Top.Pixel + Info.TotalSize.Y)) < 6)
				{
					bottomResizing = true;
					_mousePosCache = Main.MouseScreen;
				}
				if (CanDragAndNoResize())
				{
					dragging = true;
					_mousePosCache = Main.MouseScreen;
				}
			};
			Events.OnLeftUp += (element) =>
			{
				dragging = false;
				leftResizing = false;
				rightResizing = false;
				topResizing = false;
				bottomResizing = false;
			};
		}

		private bool CanDragAndNoResize()
		{
			return CanDrag && !leftResizing && !rightResizing && !topResizing && !bottomResizing;
		}

		public override void Calculation()
		{
			base.Calculation();
			if (Info.TotalSize.X < 1)
			{
				Info.TotalSize.X = 1;
				Info.TotalHitBox.Width = (int)Info.TotalSize.X;
			}
			if (Info.TotalSize.Y < 1)
			{
				Info.TotalSize.Y = 1;
				Info.TotalHitBox.Height = (int)Info.TotalSize.Y;
			}
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);
			Vector2 offset = Main.MouseScreen - _mousePosCache;
			_mousePosCache = Main.MouseScreen;
			if (CanDrag && dragging)
			{
				Info.Left.Pixel += offset.X;
				Info.Top.Pixel += offset.Y;
			}
			if (CanLeftResize && leftResizing)
			{
				Info.Left.Pixel += offset.X;
				Info.Width.Pixel -= offset.X;
				if (Info.Width.Pixel < MinWidthPixel)
				{
					Info.Left.Pixel = Info.Left.Pixel + Info.Width.Pixel - MinWidthPixel;
					Info.Width.Pixel = MinWidthPixel;
				}
			}
			if (CanRightResize && rightResizing)
			{
				Info.Width.Pixel += offset.X;
				if (Info.Width.Pixel < MinWidthPixel)
				{
					Info.Width.Pixel = MinWidthPixel;
				}
			}
			if (CanTopResize && topResizing)
			{
				Info.Top.Pixel += offset.Y;
				Info.Height.Pixel -= offset.Y;
				if (Info.Height.Pixel < MinHeightPixel)
				{
					Info.Top.Pixel = Info.Top.Pixel + Info.Height.Pixel - MinHeightPixel;
					Info.Height.Pixel = MinHeightPixel;
				}
			}
			if (CanBottomResize && bottomResizing)
			{
				Info.Height.Pixel += offset.Y;
				if (Info.Height.Pixel < MinHeightPixel)
				{
					Info.Height.Pixel = MinHeightPixel;
				}
			}
			if (offset != Vector2.zeroVector)
			{
				Calculation();
			}
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			base.DrawSelf(sb);
			Texture2D texture = TextureAssets.MagicPixel.Value;
			sb.Draw(texture, Info.TotalHitBox, PanelColor);
			if (ShowBorder.LeftBorder)
			{
				if (ShowBorder.BottomBorder)
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X, Info.TotalHitBox.Y, BorderWidth, Info.TotalHitBox.Height - BorderWidth),
						BorderColor);
				}
				else
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X, Info.TotalHitBox.Y, BorderWidth, Info.TotalHitBox.Height),
						BorderColor);
				}
			}
			if (ShowBorder.TopBorder)
			{
				if (ShowBorder.LeftBorder)
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X + BorderWidth, Info.TotalHitBox.Y, Info.TotalHitBox.Width - BorderWidth, BorderWidth),
						BorderColor);
				}
				else
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X, Info.TotalHitBox.Y, Info.TotalHitBox.Width, BorderWidth),
						BorderColor);
				}
			}
			if (ShowBorder.RightBorder)
			{
				if (ShowBorder.TopBorder)
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.Right - BorderWidth, Info.TotalHitBox.Y + BorderWidth, BorderWidth, Info.TotalHitBox.Height - BorderWidth),
						BorderColor);
				}
				else
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.Right - BorderWidth, Info.TotalHitBox.Y, BorderWidth, Info.TotalHitBox.Height),
						BorderColor);
				}
			}
			if (ShowBorder.BottomBorder)
			{
				if (ShowBorder.RightBorder)
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X, Info.TotalHitBox.Bottom - BorderWidth, Info.TotalHitBox.Width - BorderWidth, BorderWidth),
						BorderColor);
				}
				else
				{
					sb.Draw(
						texture,
						new Rectangle(Info.TotalHitBox.X, Info.TotalHitBox.Bottom - BorderWidth, Info.TotalHitBox.Width, BorderWidth),
						BorderColor);
				}
			}
		}
	}
}
