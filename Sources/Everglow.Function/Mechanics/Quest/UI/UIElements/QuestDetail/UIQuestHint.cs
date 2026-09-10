using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestHint : UIBlock
{
	private QuestView _quest;
	private UIContainerPanel _content;
	private UIQuestTextVerticalScrollbar _scrollbar;
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
		_scrollbar = new UIQuestTextVerticalScrollbar();
		Register(_scrollbar);
		_content = new UIContainerPanel();
		_content.SetVerticalScrollbar(_scrollbar);
		Register(_content);

		_title = new UITextPlus(string.Empty);
		_hint = new UITextPlus(string.Empty);
		_content.AddElements([_title, _hint]);
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
			if (changedQuest)
			{
				_scrollbar.WheelValue = 0f;
			}
			_layoutWidth = -1f;
			Calculation();
		}
	}

	public override void Calculation()
	{
		base.Calculation();
		if (_content is null)
		{
			return;
		}

		float margin = 36f * QuestContainer.Scale;
		_scrollbar.Info.Top.SetValue(margin);
		_scrollbar.Info.Left.SetValue(-margin, 1f);
		_scrollbar.Info.Height.SetValue(-2f * margin, 1f);
		_scrollbar.Calculation();
		_content.Info.Left.SetValue(margin);
		_content.Info.Top.SetValue(margin);
		_content.Info.Width.SetValue(-2f * margin - _scrollbar.InnerScale.X, 1f);
		_content.Info.Height.SetValue(-2f * margin, 1f);
		_content.Calculation();

		float width = Math.Max(1f, _content.HitBox.Width - 8f);
		float fontSize = 30f * QuestContainer.Scale;
		if (_layoutWidth != width || _fontSize != fontSize)
		{
			_layoutWidth = width;
			_fontSize = fontSize;
			FormatText(_title, fontSize * 1.2f, width);
			FormatText(_hint, fontSize, width);
			_title.Info.Top.SetValue(0f);
			_hint.Info.Top.SetValue(_title.Info.Height.Pixel + margin);
			_title.Calculation();
			_hint.Calculation();
			_content.Calculation();
		}
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
