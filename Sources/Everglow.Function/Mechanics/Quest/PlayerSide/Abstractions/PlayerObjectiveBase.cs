using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Structure;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;
using Terraria.Localization;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

public abstract class PlayerObjectiveBase : ITagCompoundEntity
{
	private const string TimerElapsedTimeSaveKey = "TimerElapsedTime";

	public bool Completed { get; private set; }

	public QuestTimer Timer { get; private set; }

	public bool IsTimedOut => Timer?.IsExpired == true;

	public bool IsRetriable { get; private set; }

	internal bool CanProgress => !Completed && !IsTimedOut;

	public int ObjectiveID { get; set; }

	public string LocalizationKey { get; set; } = string.Empty;

	public string Description => GetText("Description");

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

	/// <summary>
	/// Adds reward items and returns this objective for fluent configuration.
	/// </summary>
	public PlayerObjectiveBase WithRewards(params Item[] rewards)
	{
		RewardItems.AddRange(rewards);
		return this;
	}

	public PlayerObjectiveBase WithTimeLimit(int timeLimit, bool retriable = true)
	{
		Timer = new QuestTimer(timeLimit);
		IsRetriable = retriable;
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

	public string GetText(string field)
	{
		string key = LocalizationKey + "." + field;
		return Language.Exists(key) ? Language.GetTextValue(key) : string.Empty;
	}

	public virtual string GetObjectiveText() => GetText("ObjectiveText");

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
