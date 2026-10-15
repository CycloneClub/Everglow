using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;

public abstract partial class WorldQuestBase
{
	private const string StateKey = nameof(State);
	private const string TimeKey = nameof(Time);
	private const string RewardPlayerKey = nameof(RewardClaimedPlayers);
	private const string ObjectivesSaveKey = nameof(Objectives);

	public void LoadData(TagCompound tag)
	{
		WorldQuestState oldState = State;
		if (tag.TryGet<int>(StateKey, out var ms))
		{
			State = (WorldQuestState)ms;
		}

		if (tag.TryGet<int>(TimeKey, out var mt))
		{
			Time = mt;
		}

		_rewardClaimedPlayers.Clear();
		if (tag.TryGet<IList<string>>(RewardPlayerKey, out var rp))
		{
			_rewardClaimedPlayers.UnionWith(rp);
		}

		if (tag.TryGet<TagCompound>(ObjectivesSaveKey, out var o))
		{
			Objectives.LoadData(o);
		}
		if (!RecoverInvalidObjectiveState())
		{
			ApplyObjectiveSnapshot(oldState, State);
		}
	}

	public void SaveData(TagCompound tag)
	{
		tag.Add(StateKey, (int)State);
		tag.Add(TimeKey, Time);
		tag.Add(RewardPlayerKey, _rewardClaimedPlayers.ToList());

		var o = new TagCompound();
		Objectives.SaveData(o);
		tag.Add(ObjectivesSaveKey, o);
	}

	private bool RecoverInvalidObjectiveState()
	{
		if (!Objectives.RecoveredInvalidState)
		{
			return false;
		}

		Time = 0;
		_rewardClaimedPlayers.Clear();
		if (State == WorldQuestState.Completed)
		{
			State = WorldQuestState.Active;
		}

		ResetProgress();
		if (State == WorldQuestState.Active)
		{
			Activate();
		}
		return true;
	}
}
