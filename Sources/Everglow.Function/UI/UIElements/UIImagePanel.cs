namespace Everglow.Commons.UI.UIElements
{
	public class UIImagePanel : UIImage
	{
		public bool CanDrag = true;
		private bool dragging = false;
		private Vector2 startPoint = Vector2.Zero;

		public UIImagePanel(Texture2D texture, Color color) : base(texture, color)
		{
		}

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
	}
}
