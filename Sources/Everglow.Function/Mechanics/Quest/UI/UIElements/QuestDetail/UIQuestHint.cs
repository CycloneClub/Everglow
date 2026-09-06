using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestHint : UIBlock
{
	private QuestPresentationEntry _entry;
	private UITextPlus _title;
	private UITextPlus _hint;
	private UIQuestActionButton _actionButton;
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
		_actionButton = new UIQuestActionButton(action => QuestContainer.Service.TryExecute(action));
		Register(_actionButton);
		SetEntry(_entry);
	}

	public void SetEntry(QuestPresentationEntry entry)
	{
		var quest = entry?.View;
		bool changedQuest = _entry?.View.Identity != quest?.Identity;
		_entry = entry;
		Info.IsVisible = QuestHintDisplay.IsVisible(quest);
		if (_title is null)
		{
			return;
		}

		bool hadAction = _actionButton.IsVisible;
		_actionButton.SetEntry(entry);
		_actionButton.Info.IsVisible = entry is not null && entry.Actions.Count > 0;

		string title = QuestHintDisplay.GetDisplayName(quest);
		string hint = QuestHintDisplay.GetHint(quest);
		if (changedQuest || hadAction != _actionButton.IsVisible || _title.Text != title || _hint.Text != hint)
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

		float buttonHeight = 40f * QuestContainer.Scale;
		_actionButton.Info.Width.SetValue(Math.Min(width, 240f * QuestContainer.Scale));
		_actionButton.Info.Height.SetValue(buttonHeight);
		_actionButton.Info.Left.SetValue(-_actionButton.Info.Width.Pixel / 2f, 0.5f);
		_actionButton.Info.Top.SetValue(-margin - buttonHeight, 1f);
		_actionButton.Calculation();

		float contentOffset = _actionButton.IsVisible ? -(buttonHeight + margin) / 2f : 0f;
		float titleHeight = _title.Info.Height.Pixel;
		float hintHeight = _hint.Info.Height.Pixel;
		float spacing = titleHeight > 0f && hintHeight > 0f ? margin : 0f;
		_title.CenterY = (contentOffset - (hintHeight + spacing) / 2f, 0.5f);
		_hint.CenterY = (contentOffset + (titleHeight + spacing) / 2f, 0.5f);
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
