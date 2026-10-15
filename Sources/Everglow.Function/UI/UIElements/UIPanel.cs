namespace Everglow.Commons.UI.UIElements
{
	public class UIPanel : BaseElement
	{
		public Color PanelColor = new Color(255, 255, 246);
		public bool CanDrag = false;
		private bool dragging = false;
		private Vector2 startPoint = Vector2.Zero;

		public override void LoadEvents()
		{
			base.LoadEvents();
			Events.OnLeftDown += element =>
			{
				if (CanDrag && !dragging)
				{
					dragging = true;
					startPoint = Main.MouseScreen;
				}
			};
			Events.OnLeftClick += element =>
			{
				if (CanDrag)
					dragging = false;
			};
		}

		public override void OnInitialization()
		{
			base.OnInitialization();
			Info.SetMargin(2f);
		}

		public override void Calculation()
		{
			base.Calculation();
			if (Info.TotalSize.X < 28)
			{
				Info.TotalSize.X = 28;
				Info.TotalHitBox.Width = (int)Info.TotalSize.X;
			}
			if (Info.TotalSize.Y < 28)
			{
				Info.TotalSize.Y = 28;
				Info.TotalHitBox.Height = (int)Info.TotalSize.Y;
			}
		}

		public override void Update(GameTime gt)
		{
			base.Update(gt);
			if (CanDrag && startPoint != Main.MouseScreen && dragging)
			{
				var offestValue = Main.MouseScreen - startPoint;
				Info.Left.Pixel += offestValue.X;
				Info.Top.Pixel += offestValue.Y;
				startPoint = Main.MouseScreen;

				Calculation();
			}
		}

		protected override void DrawSelf(SpriteBatch sb)
		{
			base.DrawSelf(sb);
			Texture2D texture = ModAsset.Panel.Value;
			Point textureSize = new Point(texture.Width, texture.Height);
			Rectangle rectangle = Info.TotalHitBox;
			//绘制四个角
			sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y), new Rectangle(0, 0, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y), new Rectangle(textureSize.X - 6, 0, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 6), new Rectangle(0, textureSize.Y - 6, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y + rectangle.Height - 6), new Rectangle(textureSize.X - 6, textureSize.Y - 6, 6, 6), PanelColor);
			//绘制本体
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y, rectangle.Width - 12, 6), new Rectangle(6, 0, textureSize.X - 12, 6), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + rectangle.Height - 6, rectangle.Width - 12, 6), new Rectangle(6, textureSize.Y - 6, textureSize.X - 12, 6), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(0, 6, 6, textureSize.Y - 12), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + rectangle.Width - 6, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(textureSize.X - 6, 6, 6, textureSize.Y - 12), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + 6, rectangle.Width - 12, rectangle.Height - 12), new Rectangle(6, 6, textureSize.X - 12, textureSize.Y - 12), PanelColor);
		}
	}
}
