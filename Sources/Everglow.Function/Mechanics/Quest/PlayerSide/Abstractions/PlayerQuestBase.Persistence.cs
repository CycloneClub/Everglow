using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;

public abstract partial class PlayerQuestBase : ITagCompoundEntity
{
	/// <summary>
	/// 保存任务
	/// </summary>
	/// <param name="tag"></param>
	public virtual void SaveData(TagCompound tag)
	{
		tag.Add(nameof(State), (int)State);
		tag.Add(TimeSaveKey, Time);
		tag.Add(nameof(InstanceId), InstanceId);
		tag.Add(nameof(IsVisible), IsVisible);

		Objectives.SaveData(tag);
	}

	/// <summary>
	/// 保存任务目标
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="objectives"></param>
	public static void SaveObjectives(TagCompound tag, IEnumerable<PlayerObjectiveBase> objectives, string key = nameof(Objectives))
	{
		var oTags = new List<TagCompound>();
		foreach (var o in objectives)
		{
			var ot = new TagCompound();
			o.SaveData(ot);
			oTags.Add(ot);
		}
		tag.Add(key, oTags);
	}

	/// <summary>
	/// 加载任务
	/// </summary>
	/// <param name="tag"></param>
	public virtual void LoadData(TagCompound tag)
	{
		if (tag.TryGet<string>(nameof(InstanceId), out var instanceId)
			&& Guid.TryParseExact(instanceId, "N", out _))
		{
			InstanceId = instanceId;
		}

		// Legacy flat `_quests` saves (after pool→list, before State persistence) omit this key.
		// Enum default is Accepted (= 0); leaving it would wrongly Activate() via ApplyData.
		// Missing State → Available (not activated on load). Pre-flat partitioned keys
		// (`Everglow.QuestManage.{pool}.*`) are a manager-level format and not handled here.
		if (tag.TryGet<int>(nameof(State), out var state) && Enum.IsDefined(typeof(PlayerQuestState), state))
		{
			State = (PlayerQuestState)state;
		}
		else
		{
			State = PlayerQuestState.Available;
		}

		if (tag.ContainsKey(TimeSaveKey) && tag[TimeSaveKey] is int mt)
		{
			Time = mt;
		}
		else if (tag.ContainsKey(TimeSaveKey) && tag[TimeSaveKey] is long legacyMt)
		{
			Time = (int)Math.Clamp(legacyMt, int.MinValue, int.MaxValue);
		}

		if (tag.TryGet<bool>(nameof(IsVisible), out var isVisible))
		{
			IsVisible = isVisible;
		}

		Objectives.LoadData(tag);
		if (Objectives.RecoveredInvalidState)
		{
			Reset();
			Time = 0;
			OldCheckComplete = false;
			if (State == PlayerQuestState.Completed)
			{
				State = PlayerQuestState.Accepted;
			}
		}
	}

	/// <summary>
	/// 加载任务目标
	/// </summary>
	/// <param name="tag"></param>
	/// <param name="objectives"></param>
	public static void LoadObjectives(TagCompound tag, IEnumerable<PlayerObjectiveBase> objectives, string key = nameof(Objectives), bool useObjectiveID = true)
	{
		if (tag.TryGet<IList<TagCompound>>(key, out var oTags))
		{
			int index = 0;
			foreach (var o in objectives)
			{
				int tagIndex = useObjectiveID ? o.ObjectiveID : index++;
				if (oTags.Count <= tagIndex)
				{
					break;
				}

				o.LoadData(oTags[tagIndex]);
			}
		}
	}
}
