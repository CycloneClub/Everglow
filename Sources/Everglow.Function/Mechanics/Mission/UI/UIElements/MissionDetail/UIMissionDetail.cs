using System.Text;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.UI.UIElements;
using static Everglow.Commons.Mechanics.Mission.UI.MissionContainer;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;

public class UIMissionDetail : UIBlock
{
	private static readonly Color ComponentColor = new Color(0.2f, 0.2f, 0.2f, 0.005f);
	private static readonly Color ChangeButtonHoverColor = Color.White;
	private static readonly Color MaskButtonColor = Color.White;
	private static readonly Color MaskButtonHoverColor = new Color(1f, 1f, 1f, 0f);

	private static UIMissionItem SelectedItem => Instance.SelectedItem;

	private static float FontSize => 30f * Instance.ResolutionFactor;

	private UIMissionIcon _icon;

	private UIMissionBlock _description;
	private UIContainerPanel _descriptionContainer;
	private UIMissionTextVerticalScrollbar _descriptionTextScrollbar;

	private UIMissionBlock _objective;
	private UIContainerPanel _objectiveContainer;
	private UIMissionTextVerticalScrollbar _objectiveTextScrollbar;
	private UIMissionHourglassTimer _objectiveTimer;

	private UIMissionDurationBar _objectiveDurationBar;

	private UIBlock _objectiveTree;
	private UIImage _objectiveTreeIcon;

	private UIMissionButton _objectiveChangeMission;
	private UITextPlus _objectiveChangeText;

	private float oldScale;

	private float oldWidth;

	private float oldHeight;

	public class ChangeButtonText
	{
		public const string Failed = "失败";
		public const string Overdue = "过期";
		public const string Completed = "完成";
		public const string Cancel = "放弃";
		public const string Commit = "提交";
		public const string Accept = "接取";
		public const string Unknown = "未知";
		public const string Yes = "是";
		public const string No = "否";
	}

	public override void OnInitialization()
	{
		base.OnInitialization();

		// Headshot
		_icon = new UIMissionIcon(null);
		Register(_icon);

		// Description
		_description = new UIMissionBlock();
		_description.PanelColor = ComponentColor;
		_description.BorderColor = Color.Gray;
		_description.MissionBlockStyle = 0;
		Register(_description);

		_descriptionTextScrollbar = new UIMissionTextVerticalScrollbar();
		_description.Register(_descriptionTextScrollbar);

		_descriptionContainer = new UIContainerPanel();
		_descriptionContainer.SetVerticalScrollbar(_descriptionTextScrollbar);
		_description.Register(_descriptionContainer);

		// Objective
		_objective = new UIMissionBlock();
		_objective.PanelColor = ComponentColor;
		_objective.BorderColor = Color.Gray;
		_objective.MissionBlockStyle = 1;
		Register(_objective);

		_objectiveTextScrollbar = new UIMissionTextVerticalScrollbar();
		_objective.Register(_objectiveTextScrollbar);

		_objectiveContainer = new UIContainerPanel();
		_objectiveContainer.SetVerticalScrollbar(_objectiveTextScrollbar);
		_objective.Register(_objectiveContainer);

		_objectiveTimer = new UIMissionHourglassTimer();
		_objectiveTimer.MaxTime = 120;
		_objectiveTimer.Events.OnMouseHover += e =>
		{
			Instance.MouseText = "Time Remain: " + (int)(_objectiveTimer.Timer / 60f) + "s";
			_objectiveTimer.OnSelect = true;
		};
		_objectiveTimer.Events.OnMouseOut += e =>
		{
			_objectiveTimer.OnSelect = false;
		};
		_objective.Register(_objectiveTimer);

		_objectiveDurationBar = new UIMissionDurationBar();
		_objectiveDurationBar.Events.OnMouseHover += e =>
		{
			Instance.MouseText = "Duration: " + (int)_objectiveDurationBar.CurrentDuration + "/" + (int)_objectiveDurationBar.MaxDuration;
			_objectiveDurationBar.OnSelect = true;
		};
		_objectiveDurationBar.Events.OnMouseOut += e =>
		{
			_objectiveDurationBar.OnSelect = false;
		};
		_objective.Register(_objectiveDurationBar);

		_objectiveTree = new UIBlock();
		_objectiveTree.Info.SetMargin(0);
		_objectiveTree.PanelColor = Color.Transparent;
		_objectiveTree.BorderWidth = 0;
		_objectiveTree.Info.IsSensitive = true;
		_objectiveTree.Events.OnMouseHover += e => Instance.MouseText = "Mission Tree";
		_objectiveTree.Events.OnLeftClick += e =>
		{
			DetailSub.Show<UIMissionTree>(SelectedItem?.Mission);
		};
		_objective.Register(_objectiveTree);

		_objectiveTreeIcon = new UIImage(ModAsset.ToMissionTreeSurface.Value, Color.White);
		_objectiveTreeIcon.SourceRectangle = new Rectangle(0, 0, 38, 85);
		_objectiveTreeIcon.Events.OnMouseHover += e =>
		{
			_objectiveTreeIcon.Color = MaskButtonHoverColor;
			_objectiveTreeIcon.SourceRectangle = new Rectangle(38, 0, 38, 85);
		};
		_objectiveTreeIcon.Events.OnMouseOut += e =>
		{
			_objectiveTreeIcon.Color = MaskButtonColor;
			_objectiveTreeIcon.SourceRectangle = new Rectangle(0, 0, 38, 85);
		};
		_objectiveTree.Register(_objectiveTreeIcon);

		// Button
		_objectiveChangeMission = new UIMissionButton();
		_objectiveChangeMission.Info.IsSensitive = true;
		_objectiveChangeMission.PanelColor = ChangeButtonHoverColor;
		_objectiveChangeMission.Events.OnLeftDown += OnClickChange;
		_objectiveChangeMission.Events.OnMouseHover += e =>
		{
			if (SelectedItem != null
				&& SelectedItem.Mission.State != PlayerMissionState.Overdue
				&& SelectedItem.Mission.State != PlayerMissionState.Failed)
			{
				_objectiveChangeMission.PanelColor = Color.White;
				_objectiveChangeMission.OnSelect = true;
				UpdateChangeButton("255,245,193");
			}
		};
		_objectiveChangeMission.Events.OnMouseOut += e =>
		{
			_objectiveChangeMission.PanelColor = Color.White;
			_objectiveChangeMission.OnSelect = false;
			UpdateChangeButton("45,38,33");
		};
		_objective.Register(_objectiveChangeMission);

		_objectiveChangeText = new UITextPlus(string.Empty);
		_objectiveChangeText.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		_objectiveChangeText.StringDrawer.Init(_objectiveChangeText.Text);
		_objectiveChangeMission.Register(_objectiveChangeText);
	}

	public override void Calculation()
	{
		base.Calculation();

		float detailPanelWidth = (Info.Width.Pixel - 120) / 2f;
		float detailPanelDistance = 40;

		_icon.Info.Width.SetValue(420 * Scale);
		_icon.Info.Height.SetValue(256 * Scale);
		_icon.Info.Left.SetValue(detailPanelDistance + detailPanelWidth / 2f - 210);
		_icon.Info.Top.SetValue(93 * Scale);

		_description.Info.Width.SetValue(detailPanelWidth * Scale);
		_description.Info.Height.SetValue((ParentElement.Info.Height.Pixel - 560) * Scale);
		_description.Info.Left.SetValue(detailPanelDistance * Scale);
		_description.Info.Top.SetValue(400);

		_descriptionContainer.Info.Width.SetValue(PositionStyle.Full.Pixel - 54f);
		_descriptionContainer.Info.Height.SetValue(_descriptionTextScrollbar.Info.Height);
		_descriptionContainer.Info.Left.SetValue(PositionStyle.Full - _descriptionTextScrollbar.Info.Left - _descriptionTextScrollbar.Info.Width);
		_descriptionContainer.Info.Top.SetValue(_descriptionTextScrollbar.Info.Top);

		_descriptionTextScrollbar.Info.Height.SetValue(-60f, 1f);
		_descriptionTextScrollbar.Info.SetToCenter();
		_descriptionTextScrollbar.Info.Left.SetValue(-20f, 1f);

		_objective.Info.Width.SetValue(detailPanelWidth * Scale);
		_objective.Info.Height.SetValue((ParentElement.Info.Height.Pixel - 400) * Scale);
		_objective.Info.Left.SetValue((detailPanelDistance + detailPanelWidth + detailPanelDistance) * Scale);
		_objective.Info.Top.SetValue(60);

		_objectiveContainer.Info.Width.SetValue(PositionStyle.Full.Pixel - 54f);
		_objectiveContainer.Info.Height.SetValue(_objectiveTextScrollbar.Info.Height);
		_objectiveContainer.Info.Left.SetValue(30);
		_objectiveContainer.Info.Top.SetValue(_objectiveTextScrollbar.Info.Top);

		_objectiveTextScrollbar.Info.Height.SetValue(-60f, 1f);
		_objectiveTextScrollbar.Info.SetToCenter();
		_objectiveTextScrollbar.Info.Left.SetValue(-20f, 1f);

		_objectiveTimer.Info.Left.SetValue(17f);
		_objectiveTimer.Info.Top.SetValue(-148f, 1f);
		_objectiveTimer.Info.Width.SetValue(62);
		_objectiveTimer.Info.Height.SetValue(116);

		_objectiveTree.Info.Width.SetValue(38 * Scale);
		_objectiveTree.Info.Height.SetValue(85 * Scale);
		_objectiveTree.Info.Left.SetValue(100 * Scale);
		_objectiveTree.Info.Top.SetValue(-130f, 1f);

		_objectiveTreeIcon.Info.Width = _objectiveTree.Info.Width;
		_objectiveTreeIcon.Info.Height = _objectiveTree.Info.Height;

		float changeButtonWidth = (_objective.Info.HitBox.Width - 200) * Scale;
		_objectiveChangeMission.Info.Width.SetValue(changeButtonWidth);
		_objectiveChangeMission.Info.Height.SetValue(40 * Scale);
		_objectiveChangeMission.Info.Left.SetValue((-changeButtonWidth - 50) * Scale, 1);
		_objectiveChangeMission.Info.Top.SetValue(-70 * Scale, 1);

		_objectiveDurationBar.Info.Left.SetValue((-changeButtonWidth - 20) * Scale, 1);
		_objectiveDurationBar.Info.Top.SetValue(-120f, 1f);
		_objectiveDurationBar.Info.Width.SetValue(changeButtonWidth - 60);
		_objectiveDurationBar.Info.Height.SetValue(46);

		if (oldWidth != Info.Width.Pixel || oldHeight != Info.Height.Pixel)
		{
			if (SelectedItem != null)
			{
				ResetTexts();
				SetTexts(SelectedItem.Mission);
			}
		}

		oldWidth = Info.Width.Pixel;
		oldHeight = Info.Height.Pixel;
	}

	public static void HideMissionSubContent() => DetailSub.Info.IsVisible = false;

	public void ResetMissionDetail()
	{
		HideMissionSubContent();

		_icon.SetIconGroup(null);
		ResetTexts();
	}

	public void SetMissionDetail(UIMissionItem missionItem)
	{
		ResetMissionDetail();

		if (missionItem != null)
		{
			HideMissionSubContent();

			PlayerMissionBase mission = missionItem.Mission;
			_icon.SetIconGroup(mission.Icon);
			_descriptionTextScrollbar.WheelValue = 0f;

			SetTexts(mission);
		}
	}

	public void SetTexts(PlayerMissionBase mission)
	{
		var desText = new StringBuilder();

		// Time limit
		if (mission.TimeLimit > 0)
		{
			desText.Append(mission.GetTime() + "\n");
		}

		// Description
		desText.Append("描述：\n");
		if (string.IsNullOrWhiteSpace(mission.Description))
		{
			desText.Append("无\n");
		}
		else
		{
			desText.Append(mission.Description + "\n");
		}
		var des = new UITextPlus(desText.ToString());
		des.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		des.StringDrawer.Init(des.Text);
		_descriptionContainer.AddElement(des);
		des.StringDrawer.SetWordWrap(_descriptionContainer.HitBox.Width - _descriptionTextScrollbar.InnerScale.X);

		// Objectives
		var objText = new StringBuilder();
		objText.Append("目标：\n");
		foreach (var objective in mission.GetObjectives())
		{
			objText.Append(objective);
		}
		var obj = new UITextPlus(objText.ToString());
		obj.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		obj.StringDrawer.Init(obj.Text);
		_objectiveContainer.AddElement(obj);
		obj.StringDrawer.SetWordWrap(_objectiveContainer.HitBox.Width - _objectiveTextScrollbar.InnerScale.X);

		// Rewards
		// var rewText = new StringBuilder();
		// rewText.Append("奖励：\n");
		// rewText.Append(mission.GetRewards());
		// var rew = new UITextPlus(rewText.ToString());
		// rew.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		// rew.StringDrawer.Init(rew.Text);
		// _rewardContainer.AddElement(rew);
		// rew.StringDrawer.SetWordWrap(_rewardContainer.HitBox.Width - _rewardTextScrollbar.InnerScale.X);
	}

	private void ResetTexts()
	{
		_descriptionTextScrollbar.WheelValue = 0f;
		_descriptionContainer.ClearAllElements();

		_objectiveTextScrollbar.WheelValue = 0f;
		_objectiveContainer.ClearAllElements();

		// _rewardTextScrollbar.WheelValue = 0f;
		// _rewardContainer.ClearAllElements();
	}

	/// <summary>
	/// Base operations for mission
	/// </summary>
	/// <param name="e"></param>
	public void OnClickChange(BaseElement e)
	{
		if (SelectedItem == null)
		{
			return;
		}

		if (SelectedItem.Mission.State == PlayerMissionState.Accepted) // Accepted missions
		{
			if (SelectedItem.Mission.CheckComplete()) // Completed
			{
				// Commit the mission
				SelectedItem.Mission.OnComplete();
				PlayerMissionManager.NeedRefresh = true;
			}
			else // Incompleted
			{
				// Discard the mission (Second confirmation)
				DetailTip.Show(new UIMissionOperationTip(SelectedItem?.Mission, UIMissionOperationTip.TipType.Confirmation, "是否放弃任务", DiscardMission, "是", "否"));
			}
		}
		else if (SelectedItem.Mission.State == PlayerMissionState.Available) // Available missions
		{
			// Accept the mission
			PlayerMissionManager.MoveMission(SelectedItem.Mission, PlayerMissionState.Available, PlayerMissionState.Accepted);
			PlayerMissionManager.NeedRefresh = true;
		}
	}

	public static void DiscardMission(PlayerMissionBase m)
	{
		if (SelectedItem != null
			&& SelectedItem.Mission.State == PlayerMissionState.Accepted
			&& SelectedItem.Mission.Cancellable
			&& !SelectedItem.Mission.CheckComplete())
		{
			PlayerMissionManager.MoveMission(SelectedItem.Mission, PlayerMissionState.Accepted, PlayerMissionState.Failed);
			PlayerMissionManager.NeedRefresh = true;
		}
	}

	/// <summary>
	/// 更新按钮的文字, color示例:"45,38,33"
	/// </summary>
	public void UpdateChangeButton(string color)
	{
		if (SelectedItem != null)
		{
			if (SelectedItem.Mission.State == PlayerMissionState.Available)
			{
				_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Accept}',Color='{color}']";
			}
			else if (SelectedItem.Mission.State == PlayerMissionState.Accepted)
			{
				if (SelectedItem.Mission.CheckComplete())
				{
					_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Commit}',Color='{color}']";
				}
				else
				{
					_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Cancel}',Color='{color}']";
				}
			}
			else if (SelectedItem.Mission.State == PlayerMissionState.Completed)
			{
				_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Completed}',Color='{color}']";
			}
			else if (SelectedItem.Mission.State == PlayerMissionState.Overdue)
			{
				_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Overdue}',Color='{color}']";
			}
			else if (SelectedItem.Mission.State == PlayerMissionState.Failed)
			{
				_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Failed}',Color='{color}']";
			}
			else
			{
				_objectiveChangeText.Text = $"[TextDrawer,Text='{ChangeButtonText.Unknown}',Color='{color}']";
			}
			_objectiveChangeText.Calculation();
			_objectiveChangeText.Info.SetToCenter();
		}
		else
		{
			_objectiveChangeText.Text = "[TextDrawer,Text='',Color='{color}']";
		}
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		Texture2D tex = ModAsset.MissionIconBoard.Value;
		sb.Draw(tex, Info.TotalHitBox, new Rectangle(16, 16, 16, 16), Color.White);
		base.DrawSelf(sb);
	}
}
