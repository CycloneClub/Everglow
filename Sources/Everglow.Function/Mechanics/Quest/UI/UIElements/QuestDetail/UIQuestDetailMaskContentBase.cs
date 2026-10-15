using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public abstract class UIQuestDetailMaskContentBase<TMask> : UIBlock
	where TMask : UIQuestDetailMaskBase<TMask>, new()
{
	protected QuestView _quest;

	public event Action<BaseElement> HideMask;

	public virtual Color? BackgroundColor => null;

	public override void OnInitialization()
	{
		Info.Width.SetFull();
		Info.Height.SetFull();
		Info.SetMargin(0);
		PanelColor = Color.Transparent;
		BorderWidth = 0;
	}

	protected void Hide(BaseElement _)
	{
		HideMask?.Invoke(this);
	}

	public virtual void SetQuest(QuestView quest)
	{
		_quest = quest;
	}

	public virtual void Reset()
	{
		_quest = null;
	}
}
