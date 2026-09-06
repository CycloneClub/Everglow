using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestActionButton : UIQuestButton
{
	private QuestPresentationEntry _entry;
	private UITextPlus _text;
	private float _fontSize;

	public UIQuestActionButton(Action<QuestAction> execute)
	{
		Info.IsSensitive = true;
		PanelColor = Color.White;
		Events.OnLeftDown += _ =>
		{
			if (Info.CanBeInteract && _entry is not null && _entry.Actions.Count > 0)
			{
				execute(_entry.Actions[0]);
			}
		};
		Events.OnMouseHover += _ =>
		{
			OnSelect = Info.CanBeInteract;
			UpdateText();
		};
		Events.OnMouseOut += _ =>
		{
			OnSelect = false;
			UpdateText();
		};
	}

	public override void OnInitialization()
	{
		base.OnInitialization();
		_text = new UITextPlus(string.Empty);
		_text.CenterX = _text.CenterY = PositionStyle.Half;
		Register(_text);
		SetEntry(_entry);
	}

	public void SetEntry(QuestPresentationEntry entry)
	{
		if (_entry?.View.Identity != entry?.View.Identity || entry is null || entry.Actions.Count == 0)
		{
			OnSelect = false;
		}
		_entry = entry;
		Info.IsVisible = entry is not null;
		Info.CanBeInteract = entry is not null && entry.Actions.Count > 0;
		UpdateText();
	}

	public override void Calculation()
	{
		base.Calculation();
		UpdateText();
	}

	private void UpdateText()
	{
		if (_text is null)
		{
			return;
		}
		float fontSize = 30f * QuestContainer.Scale;
		string text = TextDefinition.GetQuestActionText(_entry, OnSelect ? "255,245,193" : "45,38,33");
		if (_text.Text == text && _fontSize == fontSize)
		{
			return;
		}
		_fontSize = fontSize;
		_text.StringDrawer.DefaultParameters.SetParameter("FontSize", fontSize);
		_text.Text = text;
		_text.Calculation();
	}
}
