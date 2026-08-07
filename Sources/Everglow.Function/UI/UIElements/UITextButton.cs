using ReLogic.Graphics;

namespace Everglow.Commons.UI.UIElements
{
	public class UITextButton : UIPanel
	{
		public UIText UIText;

		public UITextButton(string text, DynamicSpriteFont font, float scale = 1f)
		{
			Info.IsSensitive = true;
			CanDrag = false;
			Info.SetMargin(0f);
			UIText = new UIText(text, font, scale);
			UIText.Color = Color.Black;
			UIText.CenterX = new PositionStyle(0f, 0.5f);
			UIText.CenterY = new PositionStyle(0f, 0.5f);
			Register(UIText);
		}
	}
}
