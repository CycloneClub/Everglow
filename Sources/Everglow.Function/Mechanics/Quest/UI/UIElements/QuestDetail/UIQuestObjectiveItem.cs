using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;
using static Everglow.Commons.Mechanics.Quest.UI.QuestContainer;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public sealed class UIQuestObjectiveItem : BaseElement
{
	private const float TimerWidth = 18f;
	private const float TimerHeight = 34f;
	private const float TimerColumnWidth = 72f;
	private const float TimerColumnSpacing = 8f;
	private const float TimerTextSpacing = 2f;
	private const float TimerTextFontScale = 0.8f;

	private readonly UITextPlus _text;
	private readonly UIQuestHourglassTimer _timer;
	private readonly UITextPlus _timerText;
	private readonly Action<int> _retryObjective;
	private int _objectiveId;
	private bool _canRetry;

	public UIQuestObjectiveItem(
		ObjectiveLineView line,
		float fontSize,
		float width,
		Action<int> retryObjective)
	{
		_retryObjective = retryObjective;
		Info.SetMargin(0f);
		Info.Width.SetValue(Math.Max(1f, width));

		_text = new UITextPlus(string.Empty);
		_text.StringDrawer.DefaultParameters.SetParameter("FontSize", fontSize);
		Register(_text);

		_timer = new UIQuestHourglassTimer();
		_timer.Info.Width.SetValue(TimerWidth);
		_timer.Info.Height.SetValue(TimerHeight);
		_timer.Info.Left.SetValue(-(TimerColumnWidth + TimerWidth) * 0.5f, 1f);
		_timer.Events.OnMouseHover += e =>
		{
			if (_canRetry)
			{
				Instance.MouseText = TextDefinition.GetObjectiveTimerTooltip();
			}
			_timer.OnSelect = true;
		};
		_timer.Events.OnMouseOut += e => _timer.OnSelect = false;
		_timer.Events.OnLeftClick += e =>
		{
			if (_canRetry)
			{
				_retryObjective(_objectiveId);
			}
		};
		Register(_timer);

		_timerText = new UITextPlus(string.Empty);
		_timerText.StringDrawer.DefaultParameters.SetParameter("FontSize", fontSize * TimerTextFontScale);
		Register(_timerText);

		SetLine(line);
	}

	public void SetLine(ObjectiveLineView line)
	{
		ArgumentNullException.ThrowIfNull(line);
		_objectiveId = line.Objective.Id;
		_canRetry = line.Objective.CanRetry;

		TimerView timer = line.Timer;
		bool hasTimer = timer is not null;

		_timer.Info.IsVisible = hasTimer;
		_timer.MaxTime = timer?.TimeLimit ?? 0;
		_timer.Timer = timer?.RemainingTime ?? 0;

		_timerText.Info.IsVisible = hasTimer;
		_timerText.Text = hasTimer
			? TextDefinition.GetObjectiveTimerText(timer.RemainingTime)
			: string.Empty;
		_timerText.Calculation();

		if (!hasTimer)
		{
			_timer.OnSelect = false;
		}

		_text.Text = line.Text;
		float timerSpace = hasTimer ? TimerColumnWidth + TimerColumnSpacing : 0f;
		_text.StringDrawer.SetWordWrap(Math.Max(1f, Info.Width.Pixel - timerSpace));
		_text.Calculation();

		float timerColumnHeight = hasTimer
			? TimerHeight + TimerTextSpacing + _timerText.Info.Height.Pixel
			: 0f;
		float height = Math.Max(_text.Info.Height.Pixel, timerColumnHeight);
		Info.Height.SetValue(height);
		_text.Info.Top.SetValue((height - _text.Info.Height.Pixel) * 0.5f);

		if (hasTimer)
		{
			float timerTop = (height - timerColumnHeight) * 0.5f;
			_timer.Info.Top.SetValue(timerTop);

			_timerText.Info.Left.SetValue(-(TimerColumnWidth + _timerText.Info.Width.Pixel) * 0.5f, 1f);
			_timerText.Info.Top.SetValue(timerTop + TimerHeight + TimerTextSpacing);
		}

		Calculation();
	}
}
