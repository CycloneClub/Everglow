using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.UI;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;

public class UIMissionOperationTip : UIMissionDetailMaskContentBase<UIMissionDetailTipContent>
{
	private static readonly Color ButtonInitialColor = new Color(0.0f, 0.0f, 0.0f, 0.1f);
	private static readonly Color ButtonHoverColor = new Color(0f, 0f, 0f, 0.5f);

	public enum TipType
	{
		Information,
		Warning,
		Confirmation,
	}

	private TipType _mod;
	private string _tipTextStr;
	private string _yesTextStr;
	private string _noTextStr;
	private Action<PlayerMissionBase> _yesAction;

	private UIBlock _main;

	private UIBlock _tip;
	private UITextPlus _tipText;

	private UIBlock _yes;
	private UITextPlus _yesText;

	private UIBlock _no;
	private UITextPlus _noText;

	public UIMissionOperationTip()
	{
	}

	public UIMissionOperationTip(PlayerMissionBase? mission, TipType type, string tipText, Action<PlayerMissionBase> yes = null, string yesText = null, string noText = null)
	{
		_mission = mission;
		_mod = type;
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

		var scale = MissionContainer.Scale;

		_main = new UIBlock();
		_main.Info.Width.SetValue(360 * scale);
		_main.Info.Height.SetValue(240 * scale);
		_main.PanelColor = Color.Transparent;
		Register(_main);
		_main.Info.SetToCenter();

		_tip = new UIBlock();
		_tip.Info.Width.SetValue(300 * scale);
		_tip.Info.Height.SetValue(120 * scale);
		_tip.Info.Top.SetValue(30 * scale);
		_tip.Info.Left.SetValue(30 * scale);
		_tip.PanelColor = ButtonHoverColor;
		_tip.Info.SetMargin(0);
		_tip.Info.HiddenOverflow = true;
		_main.Register(_tip);

		_tipText = new UITextPlus(_tipTextStr ?? "你好！");
		_tipText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
		_tipText.StringDrawer.Init(_tipText.Text);
		_tipText.StringDrawer.SetWordWrap(_tip.Info.Width.Pixel);
		_tipText.Info.Width.SetFull();
		_tipText.Info.Height.SetFull();
		_tipText.Info.SetMargin(5 * scale);
		_tip.Register(_tipText);

		if (_mod == TipType.Confirmation)
		{
			_yes = new UIBlock();
			_yes.Info.Width.SetValue(120 * scale);
			_yes.Info.Height.SetValue(40 * scale);
			_yes.Info.Left.SetValue(30 * scale);
			_yes.Info.Top.SetValue(170 * scale);
			_yes.Info.SetMargin(0);
			_yes.Info.IsSensitive = true;
			_yes.PanelColor = ButtonInitialColor;
			_yes.Events.OnLeftClick += e =>
			{
				_yesAction?.Invoke(_mission);
				Hide(e);
			};
			_yes.Events.OnMouseHover += e => _yes.PanelColor = ButtonHoverColor;
			_yes.Events.OnMouseOut += e => _yes.PanelColor = ButtonInitialColor;
			_main.Register(_yes);

			_yesText = new UITextPlus(_yesTextStr ?? "OK");
			_yesText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_yesText.StringDrawer.Init(_yesText.Text);
			_yes.Register(_yesText);
			_yesText.Info.SetToCenter();

			_no = new UIBlock();
			_no.Info.Width.SetValue(120 * scale);
			_no.Info.Height.SetValue(40 * scale);
			_no.Info.Left.SetValue(210 * scale);
			_no.Info.Top.SetValue(170 * scale);
			_no.Info.SetMargin(0);
			_no.Info.IsSensitive = true;
			_no.PanelColor = ButtonInitialColor;
			_no.Events.OnLeftClick += Hide;
			_no.Events.OnMouseHover += e => _no.PanelColor = ButtonHoverColor;
			_no.Events.OnMouseOut += e => _no.PanelColor = ButtonInitialColor;
			_main.Register(_no);

			_noText = new UITextPlus(_noTextStr ?? "NO");
			_noText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_noText.StringDrawer.Init(_noText.Text);
			_no.Register(_noText);
			_noText.Info.SetToCenter();
		}
		else
		{
			_yes = new UIBlock();
			_yes.Info.Width.SetValue(120 * scale);
			_yes.Info.Height.SetValue(40 * scale);
			_yes.Info.Left.SetValue(120 * scale);
			_yes.Info.Top.SetValue(170 * scale);
			_yes.Info.SetMargin(0);
			_yes.Info.IsSensitive = true;
			_yes.PanelColor = ButtonInitialColor;
			_yes.Events.OnLeftClick += e =>
			{
				_yesAction?.Invoke(_mission);
				Hide(e);
			};
			_yes.Events.OnMouseHover += e => _yes.PanelColor = ButtonHoverColor;
			_yes.Events.OnMouseOut += e => _yes.PanelColor = ButtonInitialColor;
			_main.Register(_yes);

			_yesText = new UITextPlus(_yesTextStr ?? "OK");
			_yesText.StringDrawer.DefaultParameters.SetParameter("FontSize", 36f * scale);
			_yesText.StringDrawer.Init(_yesText.Text);
			_yes.Register(_yesText);
			_yesText.Info.SetToCenter();
		}
	}

	protected override void DrawChildren(SpriteBatch sb)
	{
		// Draw background
		var texture = ModAsset.Mission_MarbleBoard.Value;
		//sb.Draw(texture, _main.Info.TotalHitBox, new Rectangle(900, 300, 360, 240), Color.White);

		base.DrawChildren(sb);
	}
}
