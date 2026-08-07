using Everglow.Commons.Mechanics.Mission.PlayerSide.MissionStructure;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

public abstract partial class PlayerMissionBase : ITagCompoundEntity
{
	/// <summary>
	/// 任务目标数据
	/// </summary>
	public PlayerStructuralObjectiveContainer Objectives { get; set; }

	/// <summary>
	/// 当前任务目标
	/// </summary>
	public PlayerObjectiveBase CurrentObjective => Objectives.CurrentObjective;

	/// <summary>
	/// 任务进度，最大应为 1f
	/// </summary>
	public virtual float Progress
	{
		get => Objectives.Progress;
	}

	/// <summary>
	/// 任务时限
	/// <br>单位为帧</br>
	/// <br>86400帧为泰拉内一天</br>
	/// <br>值为-1时即不限时</br>
	/// </summary>
	public virtual long TimeLimit => -1;

	/// <summary>
	/// 是否启用计时器
	/// </summary>
	public bool EnableTime => TimeLimit > 0;

	/// <summary>
	/// 任务计时器
	/// <br>单位为帧</br>
	/// </summary>
	public virtual long Time { get; set; }

	/// <summary>
	/// Mission state, managed by <see cref="PlayerMissionManager"/>.
	/// </summary>
	/// <remarks>
	/// Should only be changed in <see cref="PlayerMissionManager"/> to keep the mission syncing to its pool collection.
	/// </remarks>
	public PlayerMissionState State { get; set; }

	/// <summary>
	/// 任务可提交状态的旧状态
	/// <para/>该属性不需要持久化，保证每次重新进入世界时都会发送信息
	/// </summary>
	public bool OldCheckComplete { get; internal set; } = false;

	/// <summary>
	/// 检查任务是否完成
	/// </summary>
	/// <returns></returns>
	public virtual bool CheckComplete() => Objectives.Completed;

	/// <summary>
	/// 检查任务是否过期
	/// </summary>
	/// <returns></returns>
	public virtual bool CheckExpire() => TimeLimit > 0 ? Time >= TimeLimit : false;

	/// <summary>
	/// 任务可提交状态改变后HOOK
	/// </summary>
	public virtual void OnCheckCompleteChange()
	{
		PlayerMissionManager.NeedRefresh = true;
	}

	/// <summary>
	/// 每帧更新
	/// </summary>
	/// <param name="gt"></param>
	public virtual void Update()
	{
		UpdateTime();

		if (Objectives.Update(this))
		{
			Main.NewText($"[{Name}]任务当前目标已完成", 250, 250, 150);
			PlayerMissionManager.NeedRefresh = true;
		}
	}

	/// <summary>
	/// 检查任务是否过期
	/// <para/>重写该方法以扩展过期条件
	/// </summary>
	protected void UpdateTime()
	{
		if (EnableTime)
		{
			Time += PlayerMissionManager.UpdateInterval;

			if (Time > TimeLimit)
			{
				Time = TimeLimit;
			}
		}
	}

	/// <summary>
	/// 任务完成时
	/// <para/>对于完成HOOK，请重写<see cref="PostComplete"/>方法
	/// </summary>
	public void OnComplete()
	{
		if (!PreComplete())
		{
			return;
		}

		PlayerMissionManager.MoveMission(this, PlayerMissionState.Accepted, PlayerMissionState.Completed);

		IsVisible = true;
		PlayerMissionManager.NeedRefresh = true;

		Main.NewText($"[{Name}]任务已完成", 150, 250, 150);

		PostComplete();
	}

	/// <summary>
	/// 任务完成 <see cref="OnComplete"/> 前HOOK.
	/// </summary>
	/// <returns></returns>
	public virtual bool PreComplete() => true;

	/// <summary>
	/// 任务完成 <see cref="OnComplete"/> 后HOOK.
	/// </summary>
	public virtual void PostComplete()
	{
		foreach (var item in RewardItems)
		{
			Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc(RewardItemsSourceContext), item, item.stack);
		}
	}

	/// <summary>
	/// 任务到期时
	/// </summary>
	public virtual void OnExpire()
	{
		PlayerMissionManager.MoveMission(this, PlayerMissionState.Accepted, PlayerMissionState.Overdue);
	}

	/// <summary>
	/// 任务失败时
	/// </summary>
	public virtual void OnFail()
	{
		PlayerMissionManager.MoveMission(this, PlayerMissionState.Accepted, PlayerMissionState.Failed);
	}

	/// <summary>
	/// 任务接取时
	/// </summary>
	public virtual void Activate()
	{
		Objectives.Activate(this);
	}

	/// <summary>
	/// 任务取消接取时
	/// </summary>
	public virtual void Deactivate()
	{
		Objectives.Deactivate();
	}

	/// <summary>
	/// 重置任务进度
	/// </summary>
	public virtual void Reset()
	{
		Objectives.ResetProgress();
	}
}
