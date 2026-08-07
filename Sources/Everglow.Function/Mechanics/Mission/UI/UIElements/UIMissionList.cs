using Everglow.Commons.Mechanics.Mission.Core;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.PlayerSide;
using Everglow.Commons.Mechanics.Mission.Presentation;
using Everglow.Commons.Mechanics.Mission.UI;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Mission.UI.UIElements;

public class UIMissionList : UIBlock
{
	private UIContainerPanel _missionList;
	private UIMissionListScrollbar _missionScrollbar;

	public List<UIMissionItem> MissionItems => _missionList.Elements.ConvertAll(x => x as UIMissionItem);

	public override void OnInitialization()
	{
		Info.SetMargin(0);

		// Mission list
		_missionList = new UIContainerPanel();
		_missionList.Events.OnUpdate += (e, gt) => MissionContainer.Background?.SetChainValue(_missionList.VerticalScrollDistance);
		Register(_missionList);

		// Mission list scrollbar
		_missionScrollbar = new UIMissionListScrollbar();
		_missionList.SetVerticalScrollbar(_missionScrollbar);
		Register(_missionScrollbar);

		_missionList.Info.Width.SetValue(_missionScrollbar.Info.Left);
	}

	public override void Calculation()
	{
		base.Calculation();

		_missionScrollbar.Info.Left.SetValue(PositionStyle.Full - _missionScrollbar.Info.Width - (4f, 0f));
		_missionScrollbar.Info.Height.SetValue(PositionStyle.Full - (20, 0f));
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_missionList is not null)
		{
			var hideList = MissionContainer.Filter.SpectrumBlockedAtInner || MissionContainer.Filter.SpectrumBlockedAtOuter;
			_missionList.Info.IsHidden = hideList;
			_missionList.Info.IsVisible = !hideList;

			float ElementSpacing = 10 * MissionContainer.Scale;
			PositionStyle top = (4 * MissionContainer.Scale, 0f);
			foreach (var mI in _missionList.Elements)
			{
				mI.OnInitialization();
				mI.Info.Top.SetValue(top);

				top += mI.Info.Height;
				top.Pixel += ElementSpacing;
			}
		}
	}

	/// <summary>
	/// 刷新任务列表
	/// </summary>
	public void RefreshList(PlayerMissionState? poolType, MissionType? missionType, MissionSourceBase missionSource)
	{
		// 筛选任务状态，获得初始列表
		var missions = poolType.HasValue
			? PlayerMissionManager.GetMissionPool(poolType.Value)
			: Enum.GetValues<PlayerMissionState>().Select(PlayerMissionManager.GetMissionPool).SelectMany(x => x);

		// 筛选来源NPC
		if (missionSource is not null) // NPC模式，去掉非对应NPC的任务
		{
			missions = missions.Where(m => m.Source == missionSource || m.SubSource == missionSource);
		}
		else // 全局模式，去掉有来源NPC的未接取任务
		{
			missions = missions.Where(m => !(m.State is PlayerMissionState.Available && m.Source is not null && m.Source != MissionSourceBase.Default));
		}

		// 筛选任务类型
		if (missionType.HasValue)
		{
			missions = missions.Where(m => m.Type == missionType);
		}

		// 排序
		missions = missions.Order(PlayerMissionComparer.Instance);

		// 生成任务UI元素
		List<BaseElement> elements = [];
		float ElementSpacing = 10 * MissionContainer.Scale;
		PositionStyle top = (4 * MissionContainer.Scale, 0f);
		foreach (var m in missions.ToList())
		{
			if (!m.IsVisible)
			{
				continue;
			}

			var element = (BaseElement)Activator.CreateInstance(m.BindingUIItem, [m]);
			element.OnInitialization();
			element.Info.Top.SetValue(top);
			element.Events.OnLeftDown += e =>
			{
				if (MissionContainer.Instance.SelectedItem != e)
				{
					MissionContainer.Instance.ChangeSelectedItem((UIMissionItem)e);
				}
			};

			elements.Add(element);

			top += element.Info.Height;
			top.Pixel += ElementSpacing;
		}

		_missionList.ClearAllElements();
		_missionList.AddElements(elements);
	}
}
