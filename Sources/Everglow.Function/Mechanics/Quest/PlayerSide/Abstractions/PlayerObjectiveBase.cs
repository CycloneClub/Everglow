using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

public abstract class PlayerObjectiveBase : ITagCompoundEntity
{
	private const string TimerElapsedTimeSaveKey = "TimerElapsedTime";

	public bool Completed { get; private set; }

	public QuestTimer Timer { get; private set; }

	public bool IsTimedOut => Timer?.IsExpired == true;

	internal bool CanProgress => !Completed && !IsTimedOut;

	public int ObjectiveID { get; set; }

	public string Description { get; private set; } = string.Empty;

	public virtual float Progress { get; } = 1f;

	/// <summary>
	/// Objective rewards, different from <see cref="PlayerQuestBase.RewardItems"/>
	/// </summary>
	public List<Item> RewardItems { get; } = [];

	public bool HasGivenRewardItems { get; private set; } = false;

	public abstract bool CheckCompletion();

	/// <summary>
	/// Invoked by <see cref="PlayerObjectiveContainer.Add(PlayerObjectiveBase)"/>.
	/// <para/>Override this hook to perform objective-specific initialization.
	/// </summary>
	public virtual void OnInitialize()
	{
	}

	/// <summary>
	/// Update inside the objective
	/// </summary>
	public virtual void Update()
	{
	}

	/// <summary>
	/// Complete the objective.
	/// </summary>
	public virtual void Complete()
	{
		if (!Completed)
		{
			if (!HasGivenRewardItems)
			{
				foreach (var item in RewardItems)
				{
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc(PlayerQuestBase.RewardItemsSourceContext), item, item.stack);
				}

				HasGivenRewardItems = true;
			}

			Completed = true;
		}
	}

	public PlayerObjectiveBase WithDescription(string description)
	{
		Description = description;
		return this;
	}

	public PlayerObjectiveBase WithTimeLimit(int timeLimit)
	{
		Timer = new QuestTimer(timeLimit);
		return this;
	}

	public virtual void ResetProgress()
	{
		Completed = false;
		Timer?.Reset();
	}

	/// <summary>
	/// Restores completion state saved by the PlayerSide structural node.
	/// Objective-specific persistence remains owned by derived objectives.
	/// </summary>
	internal void RestoreStructuralCompletionState(bool completed) => Completed = completed;

	public virtual void Activate(PlayerQuestBase sourceQuest)
	{
	}

	public virtual void Deactivate()
	{
	}

	public abstract void GetObjectivesIcon(QuestIconGroup iconGroup);

	public abstract string GetObjectiveText();

	public virtual void LoadData(TagCompound tag)
	{
		if (Timer is not null)
		{
			int elapsedTime = tag.TryGet<int>(TimerElapsedTimeSaveKey, out var storedElapsedTime)
				? storedElapsedTime
				: 0;
			Timer.RestoreElapsedTime(elapsedTime);
		}

		if (tag.TryGet<bool>(nameof(HasGivenRewardItems), out var hasGiven))
		{
			HasGivenRewardItems = hasGiven;
		}
	}

	public virtual void SaveData(TagCompound tag)
	{
		if (Timer is not null)
		{
			tag.Add(TimerElapsedTimeSaveKey, Timer.ElapsedTime);
		}

		tag.Add(nameof(HasGivenRewardItems), HasGivenRewardItems);
	}
}
