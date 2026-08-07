using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.UI.UIElements;
using Everglow.Commons.Mechanics.Mission.UI.UIElements.MissionDetail;
using Everglow.Commons.UI;
using Everglow.Commons.UI.UIElements;
using Everglow.Commons.Utilities;
using Microsoft.CodeAnalysis;
using ReLogic.Graphics;
using Terraria.GameContent;
using static Everglow.Commons.Mechanics.Mission.PlayerSide.PlayerMissionManager;

namespace Everglow.Commons.Mechanics.Mission.UI;

public class MissionContainer : UIContainerElement
{
	public static MissionContainer Instance => (MissionContainer)UISystem.EverglowUISystem.Elements[typeof(MissionContainer).FullName];

	public static UIMissionDetailSubContent DetailSub => Instance._missionDetailSubContent;

	public static UIMissionDetailTipContent DetailTip => Instance._missionDetailTip;

	public static UIMissionList List => Instance._missionList;

	public static UIMissionBackground Background => Instance._panelBackground;

	public static UIMissionFilter Filter => Instance._missionFilter;

	/// <summary>
	/// Scale factor for all UI elements in the mission system
	/// </summary>
	public static float Scale => Instance.ResolutionFactor;

	[Obsolete]
	public const int PanelWidth = 1360;
	[Obsolete]
	public const int PanelHeight = 800;

	public int CurrentPanelWidth = 1840;

	public int CurrentPanelHeight = 1024;

	// ==================== UI elements ==================== //
	private UIBlock _panel;
	private UIMissionBackground _panelBackground;
	//private UIBlock _panelCoverContainer;
	//private UIImage _panelCover;

	private UIMissionDetail _missionDetail;
	private UIMissionDetailSubContent _missionDetailSubContent;
	private UIMissionDetailTipContent _missionDetailTip;

	private UIMissionList _missionList;
	private UIMissionFilter _missionFilter;
	private UIMissionSource _missionSourceHeadshot;

	private UIBlock _close;

	// ==================== Private data fields ==================== //
	private float resolutionFactor = 1;

	/// <summary>
	/// The factor used to scale the UI elements inside the mission system.
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

	/// <summary>
	/// UI instance of the selected mission.
	/// <para/>Use <see cref="ChangeSelectedItem(UIMissionItem)"/> to change this value.
	/// <para/>If <c>null</c>, no mission item is selected.
	/// </summary>
	public UIMissionItem SelectedItem { get; private set; }

	public MissionContainer()
	{
		UpdateResolutionFactor(ScreenUtils.CurrentResolution);

		Player.Hooks.OnEnterWorld += OnEnterWorld;
		Main.OnResolutionChanged += OnResolutionChanged_Adapt;
	}

	public void Unload()
	{
		Player.Hooks.OnEnterWorld -= OnEnterWorld;
		Main.OnResolutionChanged -= OnResolutionChanged_Adapt;
	}

	/// <summary>
	/// Close mission panel on enter world
	/// </summary>
	/// <param name="player"></param>
	private void OnEnterWorld(Player player)
	{
		if (player.whoAmI == Main.myPlayer)
		{
			Close();
			UpdateResolutionFactor(ScreenUtils.CurrentResolution);
			RefreshMissionContainer();
			_panel.Info.SetToCenter();
		}
	}

	/// <summary>
	/// Update resolution factor and refresh ui on resolution changed
	/// </summary>
	/// <param name="resolution"></param>
	private void OnResolutionChanged_Adapt(Vector2 resolution)
	{
		UpdateResolutionFactor(resolution);
		_panel.Info.SetToCenter();
	}

	/// <summary>
	/// Update resolution factor and refresh ui
	/// </summary>
	/// <param name="resolution"></param>
	private void UpdateResolutionFactor(Vector2 resolution)
	{
		if (resolution.X > CurrentPanelWidth && resolution.Y > CurrentPanelHeight)
		{
			ResolutionFactor = 1;
		}
		else
		{
			if (resolution.X / resolution.Y > CurrentPanelWidth / (float)CurrentPanelHeight)
			{
				ResolutionFactor = resolution.Y / CurrentPanelHeight;
			}
			else
			{
				ResolutionFactor = resolution.X / CurrentPanelWidth;
			}
		}
	}

	private void RefreshMissionContainer()
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
		_panelBackground = new UIMissionBackground();
		_panelBackground.Info.HiddenOverflow = true;
		_panelBackground.Info.SetMargin(0);
		_panelBackground.Info.CanBeInteract = false;
		_panelBackground.ShowBorder = (false, false, false, false);
		_panel.Register(_panelBackground);

		// Mission filter
		_missionFilter = new UIMissionFilter();
		_panel.Register(_missionFilter);

		// Mission source headshot
		_missionSourceHeadshot = new UIMissionSource();
		_panel.Register(_missionSourceHeadshot);

		// Mission details
		_missionDetail = new UIMissionDetail();
		_missionDetail.PanelColor = Color.Transparent;
		_missionDetail.BorderWidth = 0;
		_panel.Register(_missionDetail);

		// Mission detail mask
		_missionDetailSubContent = new UIMissionDetailSubContent();
		_missionDetailSubContent.BorderWidth = 0;
		_missionDetailSubContent.Info.InteractiveMask = true;
		_missionDetailSubContent.Info.IsVisible = false;
		_panel.Register(_missionDetailSubContent);

		// Mission detail tip
		_missionDetailTip = new UIMissionDetailTipContent();
		_missionDetailTip.BorderWidth = 0;
		_missionDetailTip.Info.InteractiveMask = true;
		_missionDetailTip.Info.IsVisible = false;
		_panel.Register(_missionDetailTip);

		// Mission list
		_missionList = new UIMissionList();
		_missionList.PanelColor = Color.Transparent;
		_missionList.BorderWidth = 0;
		_panel.Register(_missionList);

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

		if (_panel.Info.Width.Pixel > 0 && _panel.Info.Height.Pixel > 0)
		{
			CurrentPanelWidth = (int)_panel.Info.Width.Pixel;
			CurrentPanelHeight = (int)_panel.Info.Height.Pixel;
		}
		float width = CurrentPanelWidth;
		float height = CurrentPanelHeight;

		_panel.Info.Width.SetValue(width, 0f);
		_panel.Info.Height.SetValue(height, 0f);

		_panelBackground.Info.Width.SetFull();
		_panelBackground.Info.Height.SetFull();

		_missionFilter.Info.Top.SetValue(35 * ResolutionFactor);
		_missionFilter.Info.Left.SetValue(95 * ResolutionFactor);

		_missionSourceHeadshot.Info.Top.SetValue((210 - 40) * ResolutionFactor);
		_missionSourceHeadshot.Info.Left.SetValue((270 - 40) * ResolutionFactor);

		_missionDetail.Info.Left.SetValue(740, 0);
		_missionDetail.Info.Top.SetValue(60);
		_missionDetail.Info.Width.SetValue(width - 800);
		_missionDetail.Info.Height.SetValue(height - 120);

		_missionDetailSubContent.Info.Left.SetValue(608 * ResolutionFactor);
		_missionDetailSubContent.Info.Top.SetValue(46 * ResolutionFactor);
		_missionDetailSubContent.Info.Width.SetValue(710 * ResolutionFactor, 0f);
		_missionDetailSubContent.Info.Height.SetValue(724 * ResolutionFactor, 0f);

		_missionDetailTip.Info.Left.SetValue(608 * ResolutionFactor);
		_missionDetailTip.Info.Top.SetValue(46 * ResolutionFactor);
		_missionDetailTip.Info.Width.SetValue(710 * ResolutionFactor, 0f);
		_missionDetailTip.Info.Height.SetValue(724 * ResolutionFactor, 0f);

		_missionList.Info.Top.SetValue(410f * ResolutionFactor, 0);
		_missionList.Info.Left.SetValue(80f * ResolutionFactor, 0);
		_missionList.Info.Width.SetValue(384f * ResolutionFactor, 0f);
		_missionList.Info.Height.SetValue(height - 540 * ResolutionFactor, 0f);

		_close.Info.Width.SetValue(88 * ResolutionFactor);
		_close.Info.Height.SetValue(38 * ResolutionFactor);
		_close.Info.Left.SetValue(PositionStyle.Full - _close.Info.Width + (1, 0));

		//_panelCoverContainer.Info.Width.SetFull();
		//_panelCoverContainer.Info.Height.SetFull();

		//_panelCover.Info.Width.SetFull();
		//_panelCover.Info.Height.SetFull();
		//_panelCover.SourceRectangle = new Rectangle(0, 0, (int)(CurrentPanelWidth * ResolutionFactor), (int)(CurrentPanelHeight * ResolutionFactor));
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		// Auto refresh mission list
		if (NeedRefresh)
		{
			RefreshList();

			NeedRefresh = false;
		}

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
		// Open mission panel in different mode based on the arguments.
		if (args.Length == 1) // Open NPC mission panel
		{
			// Take the first argument as the NPC mode.
			if (args[0] is MissionSourceBase source)
			{
				// Set NPC mode and source NPC.
				_missionSourceHeadshot.Source = source;
			}
			else
			{
				// Throw an exception if the argument type is incorrect.
				throw new ArgumentException("Invalid argument types. Expected: nPCMode (bool) and nPCSource (int).");
			}
		}
		else // Open global mission panel
		{
			_missionSourceHeadshot.Source = null;
		}

		RefreshMissionContainer();

		// Display the mission panel.
		base.Show(args);
	}

	/// <summary>
	/// 打开任务面板，同时选中指定任务
	/// </summary>
	/// <param name="missionName"></param>
	public void ShowWithMission(string missionName)
	{
		Show();

		foreach (var missionItem in _missionList.MissionItems)
		{
			if (missionItem.Mission.Name == missionName)
			{
				ChangeSelectedItem(missionItem);
				return;
			}
		}
	}

	/// <summary>
	/// 刷新任务列表
	/// </summary>
	public void RefreshList()
	{
		_missionList.RefreshList(_missionFilter.PoolTypeValue, _missionFilter.MissionTypeValue, _missionSourceHeadshot.Source);
		_panelBackground.SetSpectrumColor(_missionFilter.PoolTypeValue, _missionFilter.MissionTypeValue);

		// Re-select the selected mission item
		var filteredList = _missionList.MissionItems.Where(e => e.Mission.Name == SelectedItem.Mission.Name);
		if (SelectedItem != null && filteredList.Any())
		{
			ChangeSelectedItem(filteredList.First());
		}
		else
		{
			ChangeSelectedItem(null);
		}
	}

	/// <summary>
	/// 改变选中的任务
	/// </summary>
	/// <param name="item"></param>
	public void ChangeSelectedItem(UIMissionItem item)
	{
		// 更新选中的任务
		var oldSelectedItem = SelectedItem;
		SelectedItem = item;

		// 更新选中的任务的颜色
		oldSelectedItem?.OnUnselected();
		SelectedItem?.OnSelected();

		_missionDetail.UpdateChangeButton("45,38,33");
		_missionDetail.SetMissionDetail(item);
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
