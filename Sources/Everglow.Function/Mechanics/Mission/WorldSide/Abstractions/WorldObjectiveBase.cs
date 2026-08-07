using Everglow.Commons.Mechanics.Mission.WorldSide.MissionStructure;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;

public abstract class WorldObjectiveBase : IDeltaSyncObjective
{
	public WorldObjectiveBase()
	{
	}

	public bool Completed { get; private set; }

	public int ObjectiveID { get; set; }

	public WorldObjectiveBase Next { get; set; }

	public virtual float Progress { get; } = 1f;

	/// <summary>
	/// Objective rewards, different from <see cref="WorldMissionBase.RewardItems"/>
	/// </summary>
	public List<Item> RewardItems { get; } = [];

	public bool RewardClaimed { get; private set; } = false;

	public virtual bool NeedDeltaSync { get; protected set; } = false;

	public abstract bool CheckCompletion();

	/// <summary>
	/// Invoked by <see cref="WorldStructuralObjectiveContainer.Add(WorldObjectiveBase)"/>.
	/// <para/>In this hook you can do initializations, like load vanilla textures.
	/// </summary>
	public virtual void OnInitialize()
	{
		AssetUtils.LoadVanillaItemTextures(RewardItems.Select(x => x.type));
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
					Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_Misc(WorldMissionBase.RewardItemsSourceContext), item, item.stack);
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
	}

	public virtual void Activate(WorldMissionBase sourceMission)
	{
	}

	public virtual void Deactivate()
	{
	}

	public abstract void GetObjectivesText();

	public virtual void LoadData(TagCompound tag)
	{
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
		tag.Add(nameof(Completed), Completed);
		tag.Add(nameof(RewardClaimed), RewardClaimed);
	}

	public virtual void NetSend(BinaryWriter writer)
	{
		writer.Write(Completed);
		writer.Write(RewardClaimed);
	}

	public virtual void NetReceive(BinaryReader reader)
	{
		Completed = reader.ReadBoolean();
		RewardClaimed = reader.ReadBoolean();
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
