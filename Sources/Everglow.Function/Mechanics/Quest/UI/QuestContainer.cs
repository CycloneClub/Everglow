using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.Mechanics.Quest.UI.UIElements;
using Everglow.Commons.Mechanics.Quest.UI.UIElements.QuestDetail;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Microsoft.CodeAnalysis;
using ReLogic.Graphics;
using Spine;
using Terraria.GameContent;

namespace Everglow.Commons.Mechanics.Quest.UI;

public class QuestContainer : UIContainerElement
{
	public static QuestContainer Instance => (QuestContainer)UISystem.EverglowUISystem.Elements[typeof(QuestContainer).FullName];

	public static UIQuestDetailSubContent DetailSub => Instance._questDetailSubContent;

	public static UIQuestDetailTipContent DetailTip => Instance._questDetailTip;

	public static UIQuestFilter Filter => Instance._questFilter;

	public static QuestPresentationService Service => ModContent.GetInstance<QuestPresentationSystem>().Service;

	/// <summary>
	/// Scale factor for all UI elements in the quest system
	/// </summary>
	public static float Scale => Instance.ResolutionFactor;

	public int CurrentPanelWidth = 1840;

	public int CurrentPanelHeight = 1024;

	// ==================== UI elements ==================== //
	private UIBlock _panel;
	private UIQuestBackground _panelBackground;

	// private UIBlock _panelCoverContainer;
	// private UIImage _panelCover;
	private UIQuestDetail _questDetail;
	private UIQuestDetailSubContent _questDetailSubContent;
	private UIQuestDetailTipContent _questDetailTip;

	private UIQuestList _questList;
	private UIQuestFilter _questFilter;
	private UIQuestSource _questSourceHeadshot;

	private UIBlock _close;

	// ==================== Private data fields ==================== //
	private float resolutionFactor = 1;

	/// <summary>
	/// The factor used to scale the UI elements inside the quest system.
	/// </summary>
	public float ResolutionFactor
	{
		get => resolutionFactor;
		private set
		{
			resolutionFactor = Math.Max(value, 0.5f);
		}
	}

	/// <summary>
	/// Mouse text for tooltip
	/// </summary>
	public string MouseText { get; set; } = string.Empty;

	private QuestIdentity? _selectedQuest;

	/// <summary>
	/// UI instance of the selected quest.
	/// <para/>Use <see cref="ChangeSelectedItem(UIQuestItem)"/> to change this value.
	/// <para/>If <c>null</c>, no quest item is selected.
	/// </summary>
	public UIQuestItem SelectedItem => _selectedQuest is QuestIdentity identity
		? _questList?.QuestItems.FirstOrDefault(item => item.View.Identity == identity)
		: null;

	public QuestContainer()
	{
		Player.Hooks.OnEnterWorld += OnEnterWorld;
		Main.OnResolutionChanged += OnResolutionChanged_Adapt;
	}

	public void SubscribePresentationEvents(QuestPresentationSystem presentationSystem)
	{
		presentationSystem.QuestAdded += _ => RefreshQuestContainer();
		presentationSystem.QuestRemoved += _ => RefreshQuestContainer();
		presentationSystem.QuestStatusUpdated += _ => RefreshQuestContainer();
		presentationSystem.QuestObjectiveUpdated += OnQuestObjectiveUpdated;
	}

	public void Unload()
	{
		Player.Hooks.OnEnterWorld -= OnEnterWorld;
		Main.OnResolutionChanged -= OnResolutionChanged_Adapt;
	}

	/// <summary>
	/// Close quest panel on enter world
	/// </summary>
	/// <param name="player"></param>
	private void OnEnterWorld(Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			Close();
			RefreshQuestContainer();
			_panel.Info.SetToCenter();
		}
	}

	/// <summary>
	/// Update resolution factor and refresh ui on resolution changed
	/// </summary>
	/// <param name="resolution"></param>
	private void OnResolutionChanged_Adapt(Vector2 resolution)
	{
		_panel.Info.SetToCenter();
	}

	private void RefreshQuestContainer()
	{
		if (!Main.gameMenu)
		{
			RefreshList();
		}
	}

	public override void OnInitialization()
	{
		base.OnInitialization();

		_panel = new UIBlock();
		_panel.PanelColor = Color.Transparent;
		_panel.CanDrag = true;
		_panel.CanLeftResize = true;
		_panel.CanRightResize = true;
		_panel.CanTopResize = true;
		_panel.CanBottomResize = true;
		_panel.MinWidthPixel = 1200;
		_panel.MinHeightPixel = 640;
		_panel.Info.Width.SetValue(2048);
		_panel.Info.Height.SetValue(1440);
		_panel.Info.SetToCenter();
		Register(_panel);

		// Background image
		_panelBackground = new UIQuestBackground();
		_panelBackground.Info.HiddenOverflow = true;
		_panelBackground.Info.SetMargin(0);
		_panelBackground.Info.CanBeInteract = false;
		_panelBackground.ShowBorder = (false, false, false, false);
		_panel.Register(_panelBackground);

		// Quest filter
		_questFilter = new UIQuestFilter();
		_panel.Register(_questFilter);

		// Quest source headshot
		_questSourceHeadshot = new UIQuestSource();
		_panel.Register(_questSourceHeadshot);

		// Quest details
		_questDetail = new UIQuestDetail();
		_questDetail.PanelColor = Color.Transparent;
		_questDetail.BorderWidth = 0;
		_panel.Register(_questDetail);

		// Quest detail mask
		_questDetailSubContent = new UIQuestDetailSubContent();
		_questDetailSubContent.BorderWidth = 0;
		_questDetailSubContent.Info.InteractiveMask = true;
		_questDetailSubContent.Info.IsVisible = false;
		_panel.Register(_questDetailSubContent);

		// Quest detail tip
		_questDetailTip = new UIQuestDetailTipContent();
		_questDetailTip.BorderWidth = 0;
		_questDetailTip.Info.InteractiveMask = true;
		_questDetailTip.Info.IsVisible = false;
		_panel.Register(_questDetailTip);

		// Quest list
		_questList = new UIQuestList();
		_questList.PanelColor = Color.Transparent;
		_questList.BorderWidth = 0;
		_panel.Register(_questList);

		// Close button
		_close = new UIBlock();
		_close.PanelColor = Color.Transparent;
		_close.BorderColor = Color.Transparent;
		_close.Info.IsSensitive = true;
		_close.Info.SetMargin(0);
		_close.Events.OnLeftDown += e => Close();
		_close.Events.OnMouseHover += e =>
		{
			MouseText = "Close";
			_close.PanelColor = Color.Gray;
		};
		_close.Events.OnMouseOver += e => _close.PanelColor = Color.Gray;
		_close.Events.OnMouseOut += e =>
		{
			MouseText = string.Empty;
			_close.PanelColor = Color.Transparent;
		};
		_panel.Register(_close);
	}

	public override void Calculation()
	{
		base.Calculation();
		bool shouldSetToCenter = false;
		if (CurrentPanelWidth > Main.screenWidth)
		{
			_panel.Info.Width.SetValue(Main.screenWidth, 0);
			shouldSetToCenter = true;
		}
		if (CurrentPanelHeight > Main.screenHeight)
		{
			_panel.Info.Height.SetValue(Main.screenHeight, 0);
			shouldSetToCenter = true;
		}
		if (_panel.Info.Width.Pixel > 0 && _panel.Info.Height.Pixel > 0)
		{
			CurrentPanelWidth = (int)_panel.Info.Width.Pixel;
			CurrentPanelHeight = (int)_panel.Info.Height.Pixel;
			if (CurrentPanelWidth < _panel.MinWidthPixel)
			{
				CurrentPanelWidth = _panel.MinWidthPixel;
				shouldSetToCenter = true;
			}
			if (CurrentPanelHeight < _panel.MinHeightPixel)
			{
				CurrentPanelHeight = _panel.MinHeightPixel;
				shouldSetToCenter = true;
			}
		}
		float width = CurrentPanelWidth;
		float height = CurrentPanelHeight;

		_panel.Info.Width.SetValue(width, 0f);
		_panel.Info.Height.SetValue(height, 0f);
		if (shouldSetToCenter)
		{
			_panel.Info.SetToCenter();
		}

		_panelBackground.Info.Width.SetFull();
		_panelBackground.Info.Height.SetFull();

		_questSourceHeadshot.Info.Top.SetValue((210 - 40) * ResolutionFactor);
		_questSourceHeadshot.Info.Left.SetValue((270 - 40) * ResolutionFactor);

		int squzzeLeftLimit = 1500;
		float leftPartWidth = 740;
		float detailWidth = width - 800;
		if (width < squzzeLeftLimit)
		{
			leftPartWidth -= squzzeLeftLimit - width;
			detailWidth = squzzeLeftLimit - 800;
		}
		_questDetail.Info.Left.SetValue(leftPartWidth, 0);
		_questDetail.Info.Top.SetValue(60);
		_questDetail.Info.Width.SetValue(detailWidth);
		_questDetail.Info.Height.SetValue(height - 120);

		_questDetailSubContent.Info.Left.SetValue(leftPartWidth, 0);
		_questDetailSubContent.Info.Top.SetValue(60);
		_questDetailSubContent.Info.Width.SetValue(detailWidth);
		_questDetailSubContent.Info.Height.SetValue(height - 120);

		_questDetailTip.Info.Left.SetValue(leftPartWidth, 0);
		_questDetailTip.Info.Top.SetValue(60);
		_questDetailTip.Info.Width.SetValue(detailWidth);
		_questDetailTip.Info.Height.SetValue(height - 120);

		float questListWidth = 660;
		if (width < squzzeLeftLimit)
		{
			questListWidth = 660 - (squzzeLeftLimit - width);
		}

		_questList.Info.Top.SetValue(410f * ResolutionFactor, 0);
		_questList.Info.Left.SetValue(40f * ResolutionFactor, 0);
		_questList.Info.Width.SetValue(questListWidth, 0f);
		_questList.Info.Height.SetValue(height - 480 * ResolutionFactor, 0f);

		_questFilter.Info.Top.SetValue(35);
		_questFilter.Info.Left.SetValue(leftPartWidth * 0.5f - 350 * 0.5f);
		_questFilter.Info.Width.SetValue(350);
		_questFilter.Info.Height.SetValue(350);

		_close.Info.Width.SetValue(88 * ResolutionFactor);
		_close.Info.Height.SetValue(38 * ResolutionFactor);
		_close.Info.Left.SetValue(PositionStyle.Full - _close.Info.Width + (1, 0));

		// _panelCoverContainer.Info.Width.SetFull();
		// _panelCoverContainer.Info.Height.SetFull();

		// _panelCover.Info.Width.SetFull();
		// _panelCover.Info.Height.SetFull();
		// _panelCover.SourceRectangle = new Rectangle(0, 0, (int)(CurrentPanelWidth * ResolutionFactor), (int)(CurrentPanelHeight * ResolutionFactor));
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);
		Calculation();
	}

	/// <summary>
	/// 打开任务面板
	/// 该方法用于显示任务面板，并根据传入的参数设置 NPC 模式和 NPC 来源。
	/// <para/>打开全局任务面板：无需参数
	/// <para/>打开NPC任务面板：
	/// 1. nPCMode (bool): 表示是否启用 NPC 模式。
	/// 2. nPCSource (int): 表示来源 NPC 的 ID。
	/// </summary>
	/// <param name="args"></param>
	/// <exception cref="ArgumentException">
	/// 如果参数类型不正确，抛出此异常。
	/// </exception>
	public override void Show(params object[] args)
	{
		// Open quest panel in different mode based on the arguments.
		if (args.Length == 1) // Open NPC quest panel
		{
			// Take the first argument as the NPC mode.
			if (args[0] is QuestSourceBase source)
			{
				// Set NPC mode and source NPC.
				_questSourceHeadshot.Source = source;
			}
			else
			{
				// Throw an exception if the argument type is incorrect.
				throw new ArgumentException("Invalid argument types. Expected: nPCMode (bool) and nPCSource (int).");
			}
		}
		else // Open global quest panel
		{
			_questSourceHeadshot.Source = null;
		}

		RefreshQuestContainer();

		// Display the quest panel.
		base.Show(args);
	}

	/// <summary>
	/// 打开任务面板，同时选中指定任务
	/// </summary>
	/// <param name="questName"></param>
	public void ShowWithQuest(string questName)
	{
		Show();

		foreach (var questItem in _questList.QuestItems)
		{
			if (questItem.View.Identity.DefinitionId == questName)
			{
				ChangeSelectedItem(questItem);
				return;
			}
		}
	}

	/// <summary>
	/// 刷新任务列表
	/// </summary>
	public void RefreshList()
	{
		QuestIdentity? selectedQuest = _selectedQuest;
		_questList.RefreshList(_questFilter.QuestStateValue, _questFilter.QuestTypeValue, _questSourceHeadshot.Source);

		if (selectedQuest is QuestIdentity identity)
		{
			UIQuestItem selectedItem = _questList.QuestItems.FirstOrDefault(item => item.View.Identity == identity);
			ChangeSelectedItem(selectedItem);
		}
		else
		{
			ChangeSelectedItem(null);
		}
	}

	private void OnQuestObjectiveUpdated(QuestIdentity identity)
	{
		if (SelectedItem is not { } selectedItem
			|| selectedItem.View.Identity != identity
			|| !Service.TryGet(identity, out QuestPresentationEntry entry))
		{
			return;
		}

		selectedItem.UpdateEntry(entry);
		_questDetail.RefreshObjectives(entry.View);
	}

	/// <summary>
	/// 改变选中的任务
	/// </summary>
	/// <param name="item"></param>
	public void ChangeSelectedItem(UIQuestItem item)
	{
		// 更新选中的任务
		var oldSelectedItem = SelectedItem;
		_selectedQuest = item?.View.Identity;

		// 更新选中的任务的颜色
		oldSelectedItem?.OnUnselected();
		SelectedItem?.OnSelected();

		_questDetail.UpdateChangeButton("45,38,33");
		_questDetail.SetQuestDetail(item);

		if (item is not null && item.View.State == QuestViewState.Failed)
		{
			_questDetail.AnimationState = 3;
			var fail = new UIQuestOperationFail("任务失败", yesText: "确认");
			DetailTip.Show(fail);
		}
		else
		{
			_questDetail.AnimationState = 0;
			_questDetail.AnimationTimer = 0;
			DetailTip.HideCurrent();
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		base.Draw(sb);

		// Draw tooltip
		if (!string.IsNullOrEmpty(MouseText))
		{
			var pos = Main.MouseScreen + new Vector2(10f, 18f);
			var textSize = FontAssets.MouseText.Value.MeasureString(MouseText);

			if (pos.X + textSize.X > Main.screenWidth)
			{
				pos.X = Main.screenWidth - textSize.X;
			}
			if (pos.Y + textSize.Y > Main.screenHeight)
			{
				pos.Y = Main.screenHeight - textSize.Y;
			}
			if (pos.X < 0)
			{
				pos.X = 0;
			}
			if (pos.Y < 0)
			{
				pos.Y = 0;
			}

			var PanelColor = new Color(191, 106, 106);
			Texture2D texture = ModAsset.Panel.Value;
			var textureSize = new Point(texture.Width, texture.Height);
			var rectangle = new Rectangle((int)pos.X, (int)pos.Y, (int)textSize.X, (int)textSize.Y);

			// Draw 4 corners
			sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y), new Rectangle(0, 0, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y), new Rectangle(textureSize.X - 6, 0, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X, rectangle.Y + rectangle.Height - 6), new Rectangle(0, textureSize.Y - 6, 6, 6), PanelColor);
			sb.Draw(texture, new Vector2(rectangle.X + rectangle.Width - 6, rectangle.Y + rectangle.Height - 6), new Rectangle(textureSize.X - 6, textureSize.Y - 6, 6, 6), PanelColor);

			// Draw main part
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y, rectangle.Width - 12, 6), new Rectangle(6, 0, textureSize.X - 12, 6), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + rectangle.Height - 6, rectangle.Width - 12, 6), new Rectangle(6, textureSize.Y - 6, textureSize.X - 12, 6), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(0, 6, 6, textureSize.Y - 12), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + rectangle.Width - 6, rectangle.Y + 6, 6, rectangle.Height - 12), new Rectangle(textureSize.X - 6, 6, 6, textureSize.Y - 12), PanelColor);
			sb.Draw(texture, new Rectangle(rectangle.X + 6, rectangle.Y + 6, rectangle.Width - 12, rectangle.Height - 12), new Rectangle(6, 6, textureSize.X - 12, textureSize.Y - 12), PanelColor);

			// Draw text
			sb.DrawString(FontAssets.MouseText.Value, MouseText, pos + new Vector2(0f, 5f), Color.Cyan);

			MouseText = string.Empty;
		}
	}
}
