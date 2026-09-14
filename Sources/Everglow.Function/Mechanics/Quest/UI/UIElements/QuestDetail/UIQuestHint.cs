using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestHint : UIBlock
{
	private QuestView _quest;
	private UITextPlus _title;
	private UITextPlus _hint;
	private float _layoutWidth = -1f;
	private float _fontSize;

	public UIQuestHint()
	{
		Info.IsVisible = false;
		Info.IsSensitive = true;
		Info.InteractiveMask = true;
		Info.HiddenOverflow = true;
		Info.SetMargin(0);
		PanelColor = new Color(224, 216, 196);
		BorderColor = new Color(110, 94, 70);
	}

	public override void OnInitialization()
	{
		base.OnInitialization();

		_title = new UITextPlus(string.Empty);
		_hint = new UITextPlus(string.Empty);
		_title.CenterX = _hint.CenterX = PositionStyle.Half;
		Register(_title);
		Register(_hint);
		SetQuest(_quest);
	}

	public void SetQuest(QuestView quest)
	{
		bool changedQuest = _quest?.Identity != quest?.Identity;
		_quest = quest;
		Info.IsVisible = QuestHintDisplay.IsVisible(quest);
		if (_title is null)
		{
			return;
		}

		string title = QuestHintDisplay.GetDisplayName(quest);
		string hint = QuestHintDisplay.GetHint(quest);
		if (changedQuest || _title.Text != title || _hint.Text != hint)
		{
			_title.Text = title;
			_hint.Text = hint;
			_layoutWidth = -1f;
			Calculation();
		}
	}

	public override void Calculation()
	{
		base.Calculation();
		if (_title is null)
		{
			return;
		}

		float margin = 36f * QuestContainer.Scale;
		float width = Math.Max(1f, Info.Size.X - 2f * margin);
		float fontSize = 30f * QuestContainer.Scale;
		if (_layoutWidth != width || _fontSize != fontSize)
		{
			_layoutWidth = width;
			_fontSize = fontSize;
			FormatText(_title, fontSize * 1.2f, width);
			FormatText(_hint, fontSize, width);
		}

		float titleHeight = _title.Info.Height.Pixel;
		float hintHeight = _hint.Info.Height.Pixel;
		float spacing = titleHeight > 0f && hintHeight > 0f ? margin : 0f;
		_title.CenterY = (-(hintHeight + spacing) / 2f, 0.5f);
		_hint.CenterY = ((titleHeight + spacing) / 2f, 0.5f);
		_title.Calculation();
		_hint.Calculation();
	}

	private static void FormatText(UITextPlus text, float fontSize, float width)
	{
		text.StringDrawer.DefaultParameters.SetParameter("FontSize", fontSize);
		text.StringDrawer.DefaultParameters.SetParameter("Color", "45,38,33,255");
		text.StringDrawer.Init(text.Text);
		text.StringDrawer.SetWordWrap(width);
		text.Calculation();
	}
}
