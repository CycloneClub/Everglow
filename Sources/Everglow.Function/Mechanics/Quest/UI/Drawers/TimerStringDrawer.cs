using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.StringDrawerSystem;
using Everglow.Commons.UI.StringDrawerSystem.DrawerItems.TextDrawers;
using FontStashSharp;

namespace Everglow.Commons.Mechanics.Quest.UI.Drawers;

internal class TimerStringDrawer : TextDrawer
{
	public string QuestName;
	public int TimerStyle = 0;

	protected override Vector2 GetTextSize(string text)
	{
		if (!TryGetQuest(out QuestView quest))
			return Vector2.Zero;
		text = TextDefinition.GetRemainingTimeText(quest.RemainingTime);
		return base.GetTextSize(text);
	}

	public override void Init(StringDrawer stringDrawer, string originalText, string name, StringParameters stringParameters)
	{
		base.Init(stringDrawer, originalText, name, stringParameters);
		if (stringParameters == null)
			return;
		QuestName = stringParameters.GetString("QuestName",
			stringDrawer.DefaultParameters.GetString("MSTQuestName", string.Empty));
		TimerStyle = stringParameters.GetInt("TimerStyle",
			stringDrawer.DefaultParameters.GetInt("MSTTimerStyle", 0));

	}

	public override void Draw(SpriteBatch sb)
	{
		if (!TryGetQuest(out QuestView quest))
			return;
		var pos = Position;
		string text = TextDefinition.GetRemainingTimeText(quest.RemainingTime);
		sb.DrawString(Font, text, Position + Offset, Color, Scale, Rotation,
			Origin, LayerDepth, CharacterSpacing, 0, TextStyle,
			FontSystemEffect, EffectAmount);
	}

	private bool TryGetQuest(out QuestView quest)
	{
		quest = null;
		QuestPresentationService service = QuestContainer.Service;
		if (service is null)
		{
			return false;
		}

		quest = service.GetAll()
			.FirstOrDefault(entry => entry.View.Identity.Side == QuestSide.Player && entry.View.Identity.DefinitionId == QuestName)
			?.View;
		return quest is not null;
	}
}
