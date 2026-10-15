using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.Presentation.Views;
using Everglow.Commons.UI.UIElements;

namespace Everglow.Commons.Mechanics.Quest.UI.UIElements;

public class UIQuestList : UIBlock
{
	private UIContainerPanel _questList;
	private UIQuestListScrollbar _questScrollbar;

	public List<UIQuestItem> QuestItems => _questList.Elements.ConvertAll(x => x as UIQuestItem);

	public override void OnInitialization()
	{
		Info.SetMargin(0);

		// Quest list
		_questList = new UIContainerPanel();
		Register(_questList);

		// Quest list scrollbar
		_questScrollbar = new UIQuestListScrollbar();
		_questList.SetVerticalScrollbar(_questScrollbar);
		Register(_questScrollbar);

		_questList.Info.Width.SetValue(_questScrollbar.Info.Left);
	}

	public override void Calculation()
	{
		base.Calculation();
		_questScrollbar.Info.Left.SetValue(PositionStyle.Full - _questScrollbar.Info.Width - (64f, 0f));
		_questScrollbar.Info.Top.SetValue(8, 0);
		_questScrollbar.Info.Width.SetValue(33);
		_questScrollbar.Info.Height.SetValue(PositionStyle.Full - (8, 0));
	}

	public override void Update(GameTime gt)
	{
		base.Update(gt);

		if (_questList is not null)
		{
			var hideList = QuestContainer.Filter.SpectrumBlockedAtInner || QuestContainer.Filter.SpectrumBlockedAtOuter;
			_questList.Info.IsHidden = hideList;
			_questList.Info.IsVisible = !hideList;
			PositionStyle top = (6 * QuestContainer.Scale, 0f);
			foreach (var mI in _questList.Elements)
			{
				mI.OnInitialization();
				mI.Info.Top.SetValue(top);

				top += mI.Info.Height;
			}
		}
	}

	public override void Draw(SpriteBatch sb)
	{
		Texture2D background = Commons.ModAsset.Drop.Value;
		var newHitBox = HitBox;
		newHitBox.Y += 8;
		newHitBox.Height -= 8;
		newHitBox.X += 24;
		newHitBox.Width -= 120;
		sb.Draw(background, newHitBox, new Rectangle(0, 0, 1, 1), Color.White * 0.5f);
		base.Draw(sb);
	}

	/// <summary>
	/// 刷新任务列表
	/// </summary>
	public void RefreshList(QuestViewState? questState, QuestType? questType, QuestSourceBase questSource)
	{
		// 筛选任务状态，获得初始列表
		IEnumerable<QuestPresentationEntry> quests = QuestContainer.Service.GetAll();
		if (questState.HasValue)
		{
			quests = quests.Where(entry => entry.View.State == questState.Value);
		}

		// 筛选来源NPC
		if (questSource is not null) // NPC模式，去掉非对应NPC的任务
		{
			quests = quests.Where(entry => entry.View.Source == questSource || entry.View.SubSource == questSource);
		}
		else // 全局模式，去掉有来源NPC的未接取任务
		{
			quests = quests.Where(entry => !(entry.View.State is QuestViewState.Available && entry.View.Source is not null && entry.View.Source != QuestSourceBase.Default));
		}

		// 筛选任务类型
		if (questType.HasValue)
		{
			quests = quests.Where(entry => entry.View.Type == questType);
		}

		// 排序
		quests = quests.OrderBy(entry => entry.View, QuestViewComparer.Instance);

		// 生成任务UI元素
		List<BaseElement> elements = [];
		float ElementSpacing = 10 * QuestContainer.Scale;
		PositionStyle top = (4 * QuestContainer.Scale, 0f);
		foreach (QuestPresentationEntry entry in quests)
		{
			if (!entry.View.Visible)
			{
				continue;
			}

			var element = new UIQuestItem(entry);
			element.OnInitialization();
			element.Info.Top.SetValue(top);
			element.Events.OnLeftDown += e =>
			{
				if (QuestContainer.Instance.SelectedItem != e)
				{
					QuestContainer.Instance.ChangeSelectedItem((UIQuestItem)e);
				}
			};

			elements.Add(element);

			top += element.Info.Height;
			top.Pixel += ElementSpacing;
		}

		_questList.ClearAllElements();
		_questList.AddElements(elements);
	}
}
