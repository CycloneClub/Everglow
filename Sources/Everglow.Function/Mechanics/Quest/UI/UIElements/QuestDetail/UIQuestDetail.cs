using Everglow.Commons.DataStructures;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Utilities;
using static Everglow.Commons.Mechanics.Quest.UI.QuestContainer;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;

public class UIQuestDetail : UIBlock, IDrawable_InRt2D
{
	private static readonly Color ComponentColor = new Color(0.2f, 0.2f, 0.2f, 0.005f);
	private static readonly Color ChangeButtonHoverColor = Color.White;
	private static readonly Color MaskButtonColor = Color.White;
	private static readonly Color MaskButtonHoverColor = new Color(1f, 1f, 1f, 0f);

	private static UIQuestItem SelectedItem => Instance.SelectedItem;

	private static float FontSize => 30f * Instance.ResolutionFactor;

	private UIQuestIcon _icon;

	private UIQuestBlock _description;
	private UIContainerPanel _descriptionContainer;
	private UIQuestTextVerticalScrollbar _descriptionTextScrollbar;

	private UIQuestBlock _objective;
	private UIContainerPanel _objectiveContainer;
	private UIQuestTextVerticalScrollbar _objectiveTextScrollbar;
	private UITextPlus _objectiveHeader;
	private readonly List<UIQuestObjectiveItem> _objectiveItems = [];

	private UIQuestDurationBar _objectiveDurationBar;

	private UIBlock _objectiveTree;
	private UIImage _objectiveTreeIcon;

	private UIQuestButton _objectiveChangeQuest;
	private UITextPlus _objectiveChangeText;

	private UIRewardsStripe _rewardsPanel;

	// TODO: Add QuestStar to a quest(default 1);
	private UIQuestStarLevel _questLevel;

	private float oldWidth;

	private float oldHeight;

	/// <summary>
	/// Link with <see cref="AnimationState"/>.
	/// </summary>
	public float AnimationTimer = 0;

	/// <summary>
	/// 0: None, 1:TryToQuit, 2:CompleteAndClear, 3:Fail
	/// </summary>
	public int AnimationState;

	public override void OnInitialization()
	{
		base.OnInitialization();

		// Headshot
		_icon = new UIQuestIcon(null);
		Register(_icon);

		// Stars
		_questLevel = new UIQuestStarLevel();
		_questLevel.Stars = 3;
		_questLevel.Info.Width.SetValue(100);
		_questLevel.Info.Height.SetValue(40);
		Register(_questLevel);

		// Description
		_description = new UIQuestBlock();
		_description.PanelColor = ComponentColor;
		_description.BorderColor = Color.Gray;
		_description.QuestBlockStyle = 0;
		Register(_description);

		_descriptionTextScrollbar = new UIQuestTextVerticalScrollbar();
		_description.Register(_descriptionTextScrollbar);

		_descriptionContainer = new UIContainerPanel();
		_descriptionContainer.SetVerticalScrollbar(_descriptionTextScrollbar);
		_description.Register(_descriptionContainer);

		// Objective
		_objective = new UIQuestBlock();
		_objective.PanelColor = ComponentColor;
		_objective.BorderColor = Color.Gray;
		_objective.QuestBlockStyle = 1;
		Register(_objective);

		_objectiveTextScrollbar = new UIQuestTextVerticalScrollbar();
		_objective.Register(_objectiveTextScrollbar);

		_objectiveContainer = new UIContainerPanel();
		_objectiveContainer.SetVerticalScrollbar(_objectiveTextScrollbar);
		_objective.Register(_objectiveContainer);

		_objectiveDurationBar = new UIQuestDurationBar();
		_objectiveDurationBar.Events.OnMouseHover += e =>
		{
			Instance.MouseText = TextDefinition.GetObjectiveDurationTooltip(_objectiveDurationBar.CurrentDuration, _objectiveDurationBar.MaxDuration);
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
		_objectiveTree.Events.OnMouseHover += e => Instance.MouseText = "Quest Tree";
		_objectiveTree.Events.OnLeftClick += e =>
		{
			DetailSub.Show<UIQuestTree>(SelectedItem?.View);
		};
		_objective.Register(_objectiveTree);

		_objectiveTreeIcon = new UIImage(ModAsset.ToQuestTreeSurface.Value, Color.White);
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
		_objectiveChangeQuest = new UIQuestButton();
		_objectiveChangeQuest.Info.IsSensitive = true;
		_objectiveChangeQuest.PanelColor = ChangeButtonHoverColor;
		_objectiveChangeQuest.Events.OnLeftDown += OnClickChange;
		_objectiveChangeQuest.Events.OnMouseHover += e =>
		{
			if (GetFirstAction(SelectedItem?.Entry).HasValue)
			{
				_objectiveChangeQuest.PanelColor = Color.White;
				_objectiveChangeQuest.OnSelect = true;
				UpdateChangeButton("255,245,193");
			}
		};
		_objectiveChangeQuest.Events.OnMouseOut += e =>
		{
			_objectiveChangeQuest.PanelColor = Color.White;
			_objectiveChangeQuest.OnSelect = false;
			UpdateChangeButton("45,38,33");
		};
		_objective.Register(_objectiveChangeQuest);

		_objectiveChangeText = new UITextPlus(string.Empty);
		_objectiveChangeText.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		_objectiveChangeText.StringDrawer.Init(_objectiveChangeText.Text);
		_objectiveChangeQuest.Register(_objectiveChangeText);

		_rewardsPanel = new UIRewardsStripe();
		Register(_rewardsPanel);
	}

	public override void Calculation()
	{
		base.Calculation();
		Info.CanBeInteract = AnimationState == 0;
		Info.HiddenOverflow = true;
		_objectiveChangeQuest.Info.CanBeInteract = GetFirstAction(SelectedItem?.Entry).HasValue;

		float detailPanelWidth = (Info.Width.Pixel - 120) / 2f;
		float detailPanelDistance = 40;

		_icon.Info.Width.SetValue(480 * Scale);
		_icon.Info.Height.SetValue(256 * Scale);
		_icon.Info.Left.SetValue(detailPanelDistance + detailPanelWidth / 2f - 240);
		_icon.Info.Top.SetValue(93 * Scale);

		_questLevel.Info.Width.SetValue(detailPanelWidth * Scale);
		_questLevel.Info.Height.SetValue(40);
		_questLevel.Info.Left.SetValue(detailPanelDistance * Scale);
		_questLevel.Info.Top.SetValue(354);

		_description.Info.Width.SetValue(detailPanelWidth * Scale);
		_description.Info.Height.SetValue((ParentElement.Info.Height.Pixel - 560) * Scale);
		_description.Info.Left.SetValue(detailPanelDistance * Scale);
		_description.Info.Top.SetValue(400);

		_descriptionContainer.Info.Width.SetValue(PositionStyle.Full.Pixel - 54f);
		_descriptionContainer.Info.Height.SetValue(_descriptionTextScrollbar.Info.Height);
		_descriptionContainer.Info.Left.SetValue(PositionStyle.Full - _descriptionTextScrollbar.Info.Left - _descriptionTextScrollbar.Info.Width);
		_descriptionContainer.Info.Top.SetValue(_descriptionTextScrollbar.Info.Top);

		_descriptionTextScrollbar.Info.Height.SetValue(-50f, 1f);
		_descriptionTextScrollbar.Info.SetToCenter();
		_descriptionTextScrollbar.Info.Left.SetValue(-20f, 1f);

		_objective.Info.Width.SetValue(detailPanelWidth * Scale);
		_objective.Info.Height.SetValue((ParentElement.Info.Height.Pixel - 400) * Scale);
		_objective.Info.Left.SetValue((detailPanelDistance + detailPanelWidth + detailPanelDistance) * Scale);
		_objective.Info.Top.SetValue(60);

		_objectiveContainer.Info.Width.SetValue(PositionStyle.Full.Pixel - 54f);
		_objectiveContainer.Info.Height.SetValue(-200, 1f);
		_objectiveContainer.Info.Left.SetValue(30);
		_objectiveContainer.Info.Top.SetValue(_objectiveTextScrollbar.Info.Top);

		_objectiveTextScrollbar.Info.Height.SetValue(-50f, 1f);
		_objectiveTextScrollbar.Info.SetToCenter();
		_objectiveTextScrollbar.Info.Left.SetValue(-20f, 1f);

		_objectiveTree.Info.Width.SetValue(38 * Scale);
		_objectiveTree.Info.Height.SetValue(85 * Scale);
		_objectiveTree.Info.Left.SetValue(100 * Scale);
		_objectiveTree.Info.Top.SetValue(-130f, 1f);

		_objectiveTreeIcon.Info.Width = _objectiveTree.Info.Width;
		_objectiveTreeIcon.Info.Height = _objectiveTree.Info.Height;

		float changeButtonWidth = (_objective.Info.HitBox.Width - 200) * Scale;
		_objectiveChangeQuest.Info.Width.SetValue(changeButtonWidth);
		_objectiveChangeQuest.Info.Height.SetValue(40 * Scale);
		_objectiveChangeQuest.Info.Left.SetValue((-changeButtonWidth - 50) * Scale, 1);
		_objectiveChangeQuest.Info.Top.SetValue(-70 * Scale, 1);

		_objectiveDurationBar.Info.Left.SetValue((-changeButtonWidth - 20) * Scale, 1);
		_objectiveDurationBar.Info.Top.SetValue(-120f, 1f);
		_objectiveDurationBar.Info.Width.SetValue(changeButtonWidth - 60);
		_objectiveDurationBar.Info.Height.SetValue(46);

		_rewardsPanel.Info.Width.SetValue(detailPanelWidth);
		_rewardsPanel.Info.Height.SetValue(256 * Scale);
		_rewardsPanel.Info.Left.SetValue((detailPanelDistance + detailPanelWidth + detailPanelDistance) * Scale);
		_rewardsPanel.Info.Top.SetValue(-240 * Scale, 1f);

		if (oldWidth != Info.Width.Pixel || oldHeight != Info.Height.Pixel)
		{
			if (SelectedItem != null)
			{
				ResetTexts();
				SetTexts(SelectedItem.View);
			}
		}

		oldWidth = Info.Width.Pixel;
		oldHeight = Info.Height.Pixel;
	}

	public static void HideQuestSubContent() => DetailSub.HideCurrent();

	public static void HideQuestTip() => DetailTip.HideCurrent();

	public void ResetQuestDetail()
	{
		HideQuestSubContent();
		HideQuestTip();

		_icon.SetIconGroup(null);
		_rewardsPanel.SetRewards([]);
		ResetTexts();
	}

	public void SetQuestDetail(UIQuestItem questItem)
	{
		ResetQuestDetail();

		if (questItem != null)
		{
			HideQuestSubContent();

			QuestView quest = questItem.View;
			var iconGroup = new QuestIconGroup(quest.Icons);
			_icon.SetIconGroup(iconGroup);
			_rewardsPanel.SetRewards(quest.Rewards);
			_descriptionTextScrollbar.WheelValue = 0f;

			SetTexts(quest);
		}
	}

	public void SetTexts(QuestView quest)
	{
		var des = new UITextPlus(TextDefinition.GetQuestDetailText(quest));
		des.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		des.StringDrawer.Init(des.Text);
		_descriptionContainer.AddElement(des);
		des.StringDrawer.SetWordWrap(_descriptionContainer.HitBox.Width - _descriptionTextScrollbar.InnerScale.X);

		SetObjectiveText(quest);
	}

	public void RefreshObjectives(QuestView quest) => SetObjectiveText(quest);

	private void SetObjectiveText(QuestView quest)
	{
		IReadOnlyList<ObjectiveLineView> lines = TextDefinition.GetQuestObjectiveLines(quest);
		if (_objectiveHeader is null || _objectiveItems.Count != lines.Count)
		{
			RebuildObjectiveItems(quest.Identity, lines);
		}
		else
		{
			for (int i = 0; i < lines.Count; i++)
			{
				_objectiveItems[i].SetLine(lines[i]);
			}
		}

		LayoutObjectiveItems();
	}

	private void RebuildObjectiveItems(QuestIdentity questIdentity, IReadOnlyList<ObjectiveLineView> lines)
	{
		_objectiveContainer.ClearAllElements();
		_objectiveItems.Clear();

		float contentWidth = Math.Max(1f, _objectiveContainer.HitBox.Width - _objectiveTextScrollbar.InnerScale.X);
		_objectiveHeader = new UITextPlus("目标：");
		_objectiveHeader.StringDrawer.DefaultParameters.SetParameter("FontSize", FontSize);
		_objectiveHeader.StringDrawer.Init(_objectiveHeader.Text);
		_objectiveHeader.StringDrawer.SetWordWrap(contentWidth);
		_objectiveHeader.Calculation();

		List<BaseElement> elements = [_objectiveHeader];
		foreach (ObjectiveLineView line in lines)
		{
			var item = new UIQuestObjectiveItem(
				line,
				FontSize,
				contentWidth,
				objectiveId => Service.TryExecute(new QuestAction(questIdentity, QuestActionType.Retry, objectiveId)));
			_objectiveItems.Add(item);
			elements.Add(item);
		}
		_objectiveContainer.AddElements(elements);
	}

	private void LayoutObjectiveItems()
	{
		const float itemSpacing = 6f;
		float top = 0f;

		_objectiveHeader.Info.Top.SetValue(top);
		_objectiveHeader.Calculation();
		top += _objectiveHeader.Info.Height.Pixel + itemSpacing;

		foreach (UIQuestObjectiveItem item in _objectiveItems)
		{
			item.Info.Top.SetValue(top);
			item.Calculation();
			top += item.Info.Height.Pixel + itemSpacing;
		}
		_objectiveContainer.Calculation();
	}

	private void ResetTexts()
	{
		_descriptionTextScrollbar.WheelValue = 0f;
		_descriptionContainer.ClearAllElements();

		_objectiveTextScrollbar.WheelValue = 0f;
		_objectiveContainer.ClearAllElements();
		_objectiveHeader = null;
		_objectiveItems.Clear();

		// _rewardTextScrollbar.WheelValue = 0f;
		// _rewardContainer.ClearAllElements();
	}

	/// <summary>
	/// Base operations for quest
	/// </summary>
	/// <param name="e"></param>
	public void OnClickChange(BaseElement e)
	{
		if (SelectedItem == null)
		{
			return;
		}

		QuestAction? action = GetFirstAction(SelectedItem.Entry);
		if (!action.HasValue)
		{
			return;
		}

		if (action.Value.Type == QuestActionType.Cancel)
		{
			AnimationState = 1;
			var tip = new UIQuestOperationTip(SelectedItem.Entry, UIQuestOperationTip.TipType.Confirmation, "是否放弃任务", DiscardQuest, "是", "否");
			tip.HideMask += ClearAnimation;
			DetailTip.Show(tip);
		}
		else
		{
			Service.TryExecute(action.Value);
		}
	}

	public void ClearAnimation(BaseElement _)
	{
		if (AnimationState == 1)
		{
			AnimationState = 0;
			AnimationTimer = 0;
		}
	}

	/// <summary>
	/// Fail the selected quest if it can be cancelled.
	/// </summary>
	/// <param name="entry"></param>
	public void DiscardQuest(QuestPresentationEntry entry)
	{
		AnimationState = 0;
		QuestAction? cancel = FindAction(entry, QuestActionType.Cancel);
		if (cancel.HasValue)
		{
			Service.TryExecute(cancel.Value);
		}
	}

	private static QuestAction? GetFirstAction(QuestPresentationEntry entry) =>
		entry is not null && entry.Actions.Count > 0 ? entry.Actions[0] : null;

	private static QuestAction? FindAction(QuestPresentationEntry entry, QuestActionType type)
	{
		if (entry is null)
		{
			return null;
		}

		foreach (QuestAction action in entry.Actions)
		{
			if (action.Type == type)
			{
				return action;
			}
		}

		return null;
	}

	/// <summary>
	/// 更新按钮的文字, color示例:"45,38,33"
	/// </summary>
	public void UpdateChangeButton(string color)
	{
		_objectiveChangeText.Text = TextDefinition.GetQuestActionText(SelectedItem?.Entry, color);
		if (SelectedItem is not null)
		{
			_objectiveChangeText.Calculation();
			_objectiveChangeText.Info.SetToCenter();
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		if (SelectedItem is null)
		{
			Texture2D tex = ModAsset.QuestIconBoard.Value;
			sb.Draw(tex, Info.TotalHitBox, new Rectangle(16, 16, 16, 16), Color.White);
		}
		else
		{
			var uiSystem = ModContent.GetInstance<UISystem>();

			// Visual effects for current quest detail interface(Submit, Fail, Quiting...)
			if (Ins.VisualQuality.High && uiSystem.UI_Screen is not null)
			{
				SpriteBatchState previousState = GraphicsUtils.GetState(sb).Value;
				var renderTargetState = new SpriteBatchState(
					SpriteSortMode.Immediate,
					BlendState.AlphaBlend,
					SamplerState.PointWrap,
					DepthStencilState.None,
					previousState.RasterizerState,
					Matrix.Identity,
					null);

				using (renderTargetState.BeginScope(sb))
				{
					if (AnimationState > 0)
					{
						if (AnimationTimer < 60)
						{
							AnimationTimer++;
						}
						switch (AnimationState)
						{
							case 1:
								float value = AnimationTimer / 8f;
								value = Math.Clamp(value, 0, 1f);
								var effect = ModAsset.QuestDetailBlur.Value;
								effect.Parameters["uSize"].SetValue(new Vector2(uiSystem.UI_Screen.Width, uiSystem.UI_Screen.Height));
								effect.Parameters["uBlurValue"].SetValue(value);
								effect.Parameters["uDelta"].SetValue(3f);
								effect.CurrentTechnique.Passes["Blur"].Apply();
								break;
						}
					}
					else if (AnimationTimer > 0)
					{
						AnimationTimer--;
					}

					if (AnimationState < 3)
					{
						sb.Draw(uiSystem.UI_Screen, Vector2.zeroVector, new Color(1f, 1f, 1f, 1f));
					}
					else
					{
						float value = AnimationTimer / 60f;
						value = Math.Clamp(value, 0, 1f);
						sb.Draw(uiSystem.UI_Screen, Vector2.zeroVector, Color.Lerp(Color.White, Color.Red, value));
					}
				}
			}
			else
			{
				base.Draw(sb);
			}
		}
	}

	public void Draw_InRt2D(SpriteBatch sb)
	{
		base.Draw(sb);
	}

	protected override void DrawSelf(SpriteBatch sb)
	{
		Texture2D tex = ModAsset.QuestIconBoard.Value;
		sb.Draw(tex, Info.TotalHitBox, new Rectangle(16, 16, 16, 16), Color.White);
		base.DrawSelf(sb);
	}
}
