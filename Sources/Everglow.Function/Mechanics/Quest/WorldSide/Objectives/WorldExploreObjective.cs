using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldExploreObjective : WorldObjectiveBase
{
	public WorldExploreObjective()
	{
	}

	public WorldExploreObjective(int distance, Func<Player, bool> condition, string objectiveText)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(objectiveText);
		Distance = distance;
		Condition = condition;
		this.objectiveText = objectiveText;
	}

	private float _localDistance;
	private string objectiveText = string.Empty;

	public int Distance { get; private set; }

	public Func<Player, bool> Condition { get; private set; }

	public float CurrentDistance { get; private set; }

	public override float Progress => Math.Clamp(CurrentDistance / Distance, 0, 1);

	public override bool NeedDeltaSync => _localDistance > 0;

	public override bool CheckCompletion() => CurrentDistance >= Distance;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
	}

	public override string GetObjectiveText() => $"{objectiveText} ({Math.Round(CurrentDistance)}/{Distance})";

	public override void Update()
	{
		var player = Main.LocalPlayer;
		if (Condition(player))
		{
			float traveled = player.velocity.Length() * WorldQuestManager.UpdateInterval;
			if (NetUtils.IsSingle)
			{
				CurrentDistance += traveled;
			}
			else if (NetUtils.IsClient)
			{
				_localDistance += traveled;
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		CurrentDistance = 0;
		_localDistance = 0;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(CurrentDistance), CurrentDistance);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet(nameof(CurrentDistance), out float distance))
		{
			CurrentDistance = distance;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(CurrentDistance);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		CurrentDistance = reader.ReadSingle();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(_localDistance);
		_localDistance = 0;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		var distance = br.ReadSingle();
		CurrentDistance += distance;
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(CurrentDistance);
	}

	public override void ReceiveMain(BinaryReader br)
	{
		CurrentDistance = br.ReadSingle();
	}
}
