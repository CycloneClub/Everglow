using Everglow.Commons.Mechanics.Quest.UI;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestTree : UIQuestDetailMaskContentBase<UIQuestDetailSubContent>
{
	private UIBlock _back;
	private UIImage _backIcon;

	private UIImage _treeRoot;

	public override void OnInitialization()
	{
		base.OnInitialization();

		var scale = QuestContainer.Scale;

		// Tree
		_back = new UIBlock();
		_back.Info.Width.SetValue(56 * scale);
		_back.Info.Height.SetValue(83 * scale);
		_back.Info.Left.SetValue(36 * scale);
		_back.Info.Top.SetValue(100 * scale);
		_back.Info.SetMargin(0);
		_back.PanelColor = Color.Transparent;
		_back.BorderWidth = 0;
		_back.Info.IsSensitive = true;
		_back.Events.OnMouseHover += e => QuestContainer.Instance.MouseText = "Quest Detail";
		_back.Events.OnLeftClick += Hide;
		Register(_back);

		_backIcon = new UIImage(ModAsset.ToPanelSurface.Value, Color.White);
		_backIcon.Info.Width = _back.Info.Width;
		_backIcon.Info.Height = _back.Info.Height;
		_backIcon.Events.OnMouseHover += e => _backIcon.Color = new Color(1f, 1f, 1f, 0f);
		_backIcon.Events.OnMouseOut += e => _backIcon.Color = Color.White;
		_back.Register(_backIcon);

		_treeRoot = new UIImage(ModAsset.QuestTreeJoint.Value, Color.White);
		_treeRoot.Info.Width.SetValue(205 * scale);
		_treeRoot.Info.Height.SetValue(213 * scale);
		Register(_treeRoot);
		_treeRoot.Info.SetToCenter();
	}
}
