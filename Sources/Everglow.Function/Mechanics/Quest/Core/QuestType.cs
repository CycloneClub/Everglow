namespace Everglow.Commons.Mechanics.Quest.Core;

/// <summary>
/// 任务类型
/// <para/>排序：主线＞支线＞成就＞挑战＞日常＞传说
/// </summary>
public enum QuestType
{
	/// <summary>
	/// 未分配类型的任务，用于适配旧数据 - 灰
	/// </summary>
	None,

	/// <summary>
	/// 主线任务 - 金
	/// </summary>
	MainStory,

	/// <summary>
	/// 支线任务 - 紫
	/// </summary>
	SideStory,

	/// <summary>
	/// 传说任务 - 彩
	/// </summary>
	Legend,

	/// <summary>
	/// 成就任务 - 绿
	/// </summary>
	Achievement,

	/// <summary>
	/// 日常任务 - 蓝
	/// </summary>
	Daily,

	/// <summary>
	/// 挑战任务 - 红
	/// </summary>
	Challenge,
}
