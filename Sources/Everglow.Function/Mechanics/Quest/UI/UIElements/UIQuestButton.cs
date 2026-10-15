using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

public class UIQuestButton : UIBlock
{
	public bool OnSelect = false;

	public override void Draw(SpriteBatch sb)
	{
		int x = Info.HitBox.X;
		int y = Info.HitBox.Y;
		int w = Info.HitBox.Width;
		int h = Info.HitBox.Height;
		int styleX = 0;
		if (OnSelect)
		{
			styleX = 9;
		}
		Texture2D tex = ModAsset.QuestChangeButton.Value;
		sb.Draw(tex, new Rectangle(x, y, 4, 4), new Rectangle(0 + styleX, 0, 4, 4), PanelColor);
		sb.Draw(tex, new Rectangle(x + 4, y, w - 8, 4), new Rectangle(4 + styleX, 0, 1, 4), PanelColor);
		sb.Draw(tex, new Rectangle(x + w - 4, y, 4, 4), new Rectangle(5 + styleX, 0, 4, 4), PanelColor);

		sb.Draw(tex, new Rectangle(x, y + 4, 4, h - 8), new Rectangle(0 + styleX, 4, 4, 2), PanelColor);
		sb.Draw(tex, new Rectangle(x + 4, y + 4, w - 8, h - 8), new Rectangle(4 + styleX, 4, 1, 2), PanelColor);
		sb.Draw(tex, new Rectangle(x + w - 4, y + 4, 4, h - 8), new Rectangle(5 + styleX, 4, 4, 2), PanelColor);

		sb.Draw(tex, new Rectangle(x, y + h - 4, 4, 4), new Rectangle(0 + styleX, 6, 4, 4), PanelColor);
		sb.Draw(tex, new Rectangle(x + 4, y + h - 4, w - 8, 4), new Rectangle(4 + styleX, 6, 1, 4), PanelColor);
		sb.Draw(tex, new Rectangle(x + w - 4, y + h - 4, 4, 4), new Rectangle(5 + styleX, 6, 4, 4), PanelColor);
		DrawChildren(sb);
	}
}
