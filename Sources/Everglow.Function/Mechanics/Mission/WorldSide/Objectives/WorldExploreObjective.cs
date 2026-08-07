using Everglow.Commons.Mechanics.Mission.WorldSide.Abstractions;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide.Objectives;

public class WorldExploreObjective : WorldObjectiveBase
{
	public WorldExploreObjective()
	{
	}

	public WorldExploreObjective(int distance, Func<Player, bool> condition)
	{
		Distance = distance;
		Condition = condition;
	}

	private float _localDistance;

	public int Distance { get; private set; }

	public Func<Player, bool> Condition { get; private set; }

	public float CurrentDistance { get; private set; }

	public override float Progress => Math.Clamp(CurrentDistance / Distance, 0, 1);

	public override bool NeedDeltaSync => _localDistance > 0;

	public override bool CheckCompletion() => CurrentDistance >= Distance;

	public override void Update()
	{
		var player = Main.LocalPlayer;
		if (Condition(player))
		{
			if (NetUtils.IsSingle)
			{
				CurrentDistance += player.velocity.Length();
			}
			else if (NetUtils.IsClient)
			{
				_localDistance += player.velocity.Length();
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		CurrentDistance = 0;
		_localDistance = 0;
	}

	public override void GetObjectivesText() => throw new NotImplementedException();

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
