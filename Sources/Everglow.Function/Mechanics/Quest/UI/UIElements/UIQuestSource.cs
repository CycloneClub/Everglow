using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

/// <summary>
/// Displays the selected quest's source in the quest detail panel.
/// </summary>
public class UIQuestSource : BaseElement
{
	private QuestSourceIcon _icon;

	public void SetSource(QuestSourceBase source, QuestSourceBase subSource = null)
	{
		_icon = source is null ? null : QuestSourceIcon.Create(source, subSource);
	}

	public override void OnInitialization()
	{
		base.OnInitialization();
		Events.OnMouseHover += _ =>
		{
			if (_icon is not null)
			{
				QuestContainer.Instance.MouseText = _icon.Tooltip;
			}
		};
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);
		_icon?.Draw(sb, HitBox, Color.White, QuestContainer.Scale);
	}
}
