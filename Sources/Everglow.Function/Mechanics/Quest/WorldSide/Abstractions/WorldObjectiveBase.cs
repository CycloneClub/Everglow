using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.WorldSide.Structure;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract class WorldObjectiveBase : IDeltaSyncObjective
{
	private const string TimerElapsedTimeSaveKey = "TimerElapsedTime";

	public WorldObjectiveBase()
	{
	}

	public bool Completed { get; private set; }

	public QuestTimer Timer { get; private set; }

	public bool IsTimedOut => Timer?.IsExpired == true;

	internal bool CanProgress => !Completed && !IsTimedOut;

	public int ObjectiveID { get; set; }

	public string Description { get; private set; } = string.Empty;

	public virtual float Progress { get; } = 1f;

	/// <summary>
	/// Objective rewards, different from <see cref="WorldQuestBase.RewardItems"/>
	/// </summary>
	public List<Item> RewardItems { get; } = [];

	public bool RewardClaimed { get; private set; } = false;

	public virtual bool NeedDeltaSync { get; protected set; } = false;

	public WorldObjectiveBase WithDescription(string description)
	{
		Description = description;
		return this;
	}

	public WorldObjectiveBase WithTimeLimit(int timeLimit)
	{
		Timer = new QuestTimer(timeLimit);
		return this;
	}

	public abstract bool CheckCompletion();

	/// <summary>
	/// Invoked by <see cref="WorldObjectiveContainer.Add(WorldObjectiveBase)"/>.
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
			if (!RewardClaimed)
			{
				foreach (var item in RewardItems)
				{
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc(WorldQuestBase.RewardItemsSourceContext), item, item.stack);
				}

				RewardClaimed = true;
			}

			Completed = true;
		}
	}

	public virtual void ResetProgress()
	{
		RewardClaimed = false;
		Completed = false;
		Timer?.Reset();
	}

	public virtual void Activate(WorldQuestBase sourceQuest)
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

		if (tag.TryGet<bool>(nameof(Completed), out var completed))
		{
			Completed = completed;
		}

		if (tag.TryGet<bool>(nameof(RewardClaimed), out var hasGiven))
		{
			RewardClaimed = hasGiven;
		}
	}

	public virtual void SaveData(TagCompound tag)
	{
		if (Timer is not null)
		{
			tag.Add(TimerElapsedTimeSaveKey, Timer.ElapsedTime);
		}

		tag.Add(nameof(Completed), Completed);
		tag.Add(nameof(RewardClaimed), RewardClaimed);
	}

	public virtual void NetSend(BinaryWriter writer)
	{
		writer.Write(Completed);
		writer.Write(RewardClaimed);
		if (Timer is not null)
		{
			writer.Write(Timer.ElapsedTime);
		}
	}

	public virtual void NetReceive(BinaryReader reader)
	{
		Completed = reader.ReadBoolean();
		RewardClaimed = reader.ReadBoolean();
		if (Timer is not null)
		{
			Timer.RestoreElapsedTime(reader.ReadInt32());
		}
	}

	public virtual void SendDelta(BinaryWriter bw)
	{
	}

	public virtual void ReceiveDelta(BinaryReader br)
	{
	}

	public virtual void SendMain(BinaryWriter bw)
	{
	}

	public virtual void ReceiveMain(BinaryReader br)
	{
	}
}
