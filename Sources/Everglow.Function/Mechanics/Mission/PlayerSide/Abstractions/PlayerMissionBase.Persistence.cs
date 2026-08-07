using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.PlayerSide.Abstractions;

public abstract partial class PlayerMissionBase : ITagCompoundEntity
{
	/// <summary>
	/// 保存任务
	/// </summary>
	/// <param name="tag"></param>
	public virtual void SaveData(TagCompound tag)
	{
		tag.Add(nameof(State), (int)State);
		tag.Add(TimeSaveKey, Time);
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
		// Legacy flat `_missions` saves (after pool→list, before State persistence) omit this key.
		// Enum default is Accepted (= 0); leaving it would wrongly Activate() via ApplyData.
		// Missing State → Available (not activated on load). Pre-flat partitioned keys
		// (`Everglow.MissionManage.{pool}.*`) are a manager-level format and not handled here.
		if (tag.TryGet<int>(nameof(State), out var state) && Enum.IsDefined(typeof(PlayerMissionState), state))
		{
			State = (PlayerMissionState)state;
		}
		else
		{
			State = PlayerMissionState.Available;
		}

		if (tag.TryGet<long>(TimeSaveKey, out var mt))
		{
			Time = mt;
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
			if (State == PlayerMissionState.Completed)
			{
				State = PlayerMissionState.Accepted;
			}
		}

		AssetUtils.LoadVanillaItemTextures(RewardItems.Select(x => x.type));
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
