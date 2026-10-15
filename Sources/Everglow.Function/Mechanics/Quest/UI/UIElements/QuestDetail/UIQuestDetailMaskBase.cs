using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public abstract class UIQuestDetailMaskBase<TMask> : UIBlock
	where TMask : UIQuestDetailMaskBase<TMask>, new()
{
	private static readonly Color DefaultColor = new Color(0f, 0f, 0f, 0.3f);

	private UIBlock _container;
	private UIQuestDetailMaskContentBase<TMask> _content;

	public override void OnInitialization()
	{
		base.OnInitialization();

		PanelColor = DefaultColor;

		_container = new UIBlock();
		_container.Info.Width.SetFull();
		_container.Info.Height.SetFull();
		_container.PanelColor = Color.Transparent;
		_container.BorderColor = Color.Transparent;
		_container.Info.SetMargin(0);
		Register(_container);
	}

	public void Show(UIQuestDetailMaskContentBase<TMask> content)
	{
		HideCurrent();

		Info.IsVisible = true;

		_content = content;
		_content.HideMask += Hide;
		_container.Register(_content);

		PanelColor = _content.BackgroundColor ?? DefaultColor;
	}

	public void Show<TContent>(QuestView quest)
		where TContent : UIQuestDetailMaskContentBase<TMask>, new()
	{
		var content = new TContent();
		content.SetQuest(quest);
		Show(content);
	}

	public void HideCurrent()
	{
		if (_content is not null)
		{
			Hide(_content);
			return;
		}

		Info.IsVisible = false;
	}

	private void Hide(BaseElement element)
	{
		Info.IsVisible = false;

		if (element is not UIQuestDetailMaskContentBase<TMask> content)
		{
			throw new InvalidOperationException("Invalid call.");
		}

		_container.Remove(content);
		content.HideMask -= Hide;
		content.Reset();
		_content = null;

		PanelColor = DefaultColor;
	}
}
