using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.UI.UIElements;


namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestOperationTip : UIQuestDetailMaskContentBase<UIQuestDetailTipContent>
{
	private static readonly Color ButtonHoverColor = new Color(0f, 0f, 0f, 0.5f);

	public enum TipType
	{
		Information,
		Warning,
		Confirmation,
	}

	private TipType _tipContent;
	private string _tipTextStr;
	private string _yesTextStr;
	private string _noTextStr;
	private QuestPresentationEntry _entry;
	private Action<QuestPresentationEntry> _yesAction;

	private UIBlock _main;

	private UIQuestBlock _tip;
	private UITextPlus _tipText;

	private UIQuestButton _yes;
	private UITextPlus _yesText;

	private UIQuestButton _no;
	private UITextPlus _noText;

	public UIQuestOperationTip()
	{
	}

	public UIQuestOperationTip(QuestPresentationEntry entry, TipType type, string tipText, Action<QuestPresentationEntry> yes = null, string yesText = null, string noText = null)
	{
		_entry = entry;
		_tipContent = type;
		_tipTextStr = tipText;
		_yesTextStr = yesText;
		_noTextStr = noText;
		_yesAction = yes;
	}

	public override void OnInitialization()
	{
		base.OnInitialization();
		Info.HiddenOverflow = true;
		Info.SetMargin(0);

		var scale = QuestContainer.Scale;

		_main = new UIBlock();
		_main.PanelColor = Color.Transparent;
		Register(_main);
		_main.Info.Width.SetFull();
		_main.Info.Height.SetFull();
		_main.Info.SetToCenter();

		_tip = new UIQuestBlock();
		_tip.Info.Width.SetValue(320);
		_tip.Info.Height.SetValue(200);
		_tip.Info.Left.SetValue(-_tip.Info.Width.Pixel * 0.5f, 0.5f);
		_tip.Info.Top.SetValue(-120, 0.5f);
		_tip.QuestBlockStyle = 2;
		_tip.PanelColor = ButtonHoverColor;
		_tip.Info.SetMargin(0);
		_tip.Info.HiddenOverflow = true;
		_main.Register(_tip);

		_tipText = new UITextPlus(_tipTextStr ?? "你好！");
		_tipText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
		_tipText.StringDrawer.Init(_tipText.Text);
		_tipText.StringDrawer.SetWordWrap(_tip.Info.Width.Pixel);
		_tipText.Info.Width.SetValue(-48, 1);
		_tipText.Info.Height.SetValue(-48, 1);
		_tipText.Info.SetToCenter();
		_tipText.Info.SetMargin(5 * scale);
		_tip.Register(_tipText);

		if (_tipContent == TipType.Confirmation)
		{
			_yes = NewButton();
			_yes.Events.OnLeftClick += e =>
			{
				if (_entry is not null)
				{
					_yesAction?.Invoke(_entry);
				}
				Hide(e);
			};
			_yes.Events.OnMouseHover += e =>
			{
				_yes.OnSelect = true;
			};
			_yes.Events.OnMouseOut += e =>
			{
				_yes.OnSelect = false;
			};
			_main.Register(_yes);

			_yesText = new UITextPlus(_yesTextStr ?? "OK");
			_yesText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_yesText.StringDrawer.DefaultParameters.SetParameter("Color", "45,38,33");
			_yesText.StringDrawer.Init(_yesText.Text);
			_yes.Register(_yesText);
			_yesText.Info.SetToCenter();

			_no = NewButton();
			_no.Events.OnLeftClick += Hide;
			_no.Events.OnMouseHover += e =>
			{
				_no.OnSelect = true;
			};
			_no.Events.OnMouseOut += e =>
			{
				_no.OnSelect = false;
			};
			_main.Register(_no);

			_noText = new UITextPlus(_noTextStr ?? "NO");
			_noText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_noText.StringDrawer.DefaultParameters.SetParameter("Color", "45,38,33");
			_noText.StringDrawer.Init(_noText.Text);
			_no.Register(_noText);
			_noText.Info.SetToCenter();
		}
		else
		{
			_yes = NewButton();
			_yes.Events.OnMouseHover += e =>
			{
				_yes.OnSelect = true;
			};
			_yes.Events.OnMouseOut += e =>
			{
				_yes.OnSelect = false;
			};
			_yes.Events.OnLeftClick += e =>
			{
				if (_entry is not null)
				{
					_yesAction?.Invoke(_entry);
				}
				Hide(e);
			};
			_main.Register(_yes);

			_yesText = new UITextPlus(_yesTextStr ?? "OK");
			_yesText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_yesText.StringDrawer.DefaultParameters.SetParameter("Color", "45,38,33");
			_yesText.StringDrawer.Init(_yesText.Text);
			_yes.Register(_yesText);
			_yesText.Info.SetToCenter();
		}
	}

	public UIQuestButton NewButton()
	{
		var button = new UIQuestButton();
		button.Info.Width.SetValue(120);
		button.Info.Height.SetValue(40);
		button.Info.Left.SetValue(Info.HitBox.Width / 2f - 120 - 40);
		button.Info.Top.SetValue(Info.HitBox.Height * 0.63f, 0);
		button.Info.SetMargin(0);
		button.Info.IsSensitive = true;
		button.PanelColor = Color.White;
		return button;
	}

	public override void Calculation()
	{
		if (_yes.OnSelect)
		{
			_yesText.Text = TextDefinition.GetColoredText(_yesTextStr, "255,245,193");
		}
		if (!_yes.OnSelect)
		{
			_yesText.Text = TextDefinition.GetColoredText(_yesTextStr, "45,38,33");
		}
		if (_no is not null)
		{
			if (_no.OnSelect)
			{
				_noText.Text = TextDefinition.GetColoredText(_noTextStr, "255,245,193");
			}
			if (!_no.OnSelect)
			{
				_noText.Text = TextDefinition.GetColoredText(_noTextStr, "45,38,33");
			}
		}
		_main.Info.Width.SetFull();
		_main.Info.Height.SetFull();
		_main.Info.SetToCenter();

		_tip.Info.Width.SetValue(320);
		_tip.Info.Height.SetValue(200);
		_tip.Info.Left.SetValue(-_tip.Info.Width.Pixel * 0.5f, 0.5f);
		_tip.Info.Top.SetValue(-120, 0.5f);

		_tipText.Info.Width.SetValue(-48, 1);
		_tipText.Info.Height.SetValue(-48, 1);
		_tipText.Info.SetToCenter();

		_yes.Info.Width.SetValue(120);
		_yes.Info.Height.SetValue(40);
		_yes.Info.Left.SetValue(Info.HitBox.Width / 2f - 120 - 40);
		_yes.Info.Top.SetValue(Info.HitBox.Height * 0.63f, 0);

		if (_no is not null)
		{
			_no.Info.Width.SetValue(120);
			_no.Info.Height.SetValue(40);
			_no.Info.Left.SetValue(Info.HitBox.Width / 2f + 40);
			_no.Info.Top.SetValue(Info.HitBox.Height * 0.63f, 0);
		}
		else
		{
			_yes.Info.Width.SetValue(120);
			_yes.Info.Height.SetValue(40);
			_yes.Info.Left.SetValue(Info.HitBox.Width / 2f - 60);
			_yes.Info.Top.SetValue(Info.HitBox.Height * 0.63f, 0);
		}
		base.Calculation();
	}

	public override void Draw(SpriteBatch sb)
	{
		if (_yes.Info.Width.Pixel != 120)
		{
			return;
		}
		base.Draw(sb);
	}

	protected override void DrawChildren(SpriteBatch sb)
	{
		base.DrawChildren(sb);
	}
}
