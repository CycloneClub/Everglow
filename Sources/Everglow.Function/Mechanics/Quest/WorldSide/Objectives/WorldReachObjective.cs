using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation.Icons;
using Everglow.Commons.Utilities;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;

public class WorldReachObjective : WorldObjectiveBase
{
	public WorldReachObjective()
	{
	}

	public WorldReachObjective(Func<Player, bool> condition, string objectiveText)
	{
		ArgumentException.ThrowIfNullOrWhiteSpace(objectiveText);
		Condition = condition;
		this.objectiveText = objectiveText;
	}

	private bool reaching;

	private bool oldReaching;

	private string objectiveText = string.Empty;

	public bool Reached { get; private set; }

	public Func<Player, bool> Condition { get; private set; }

	public override float Progress => Reached ? 1f : 0f;

	public override bool NeedDeltaSync { get; protected set; } = false;

	public override bool CheckCompletion() => Reached;

	public override void GetObjectivesIcon(QuestIconGroup iconGroup)
	{
	}

	public override string GetObjectiveText() => objectiveText;

	public override void Update()
	{
		if (NetUtils.IsSingle || NetUtils.IsMainServer)
		{
			foreach (var player in Main.ActivePlayers)
			{
				if (Condition(player))
				{
					Reached = true;
				}
			}
		}
		else if (NetUtils.IsSubServer)
		{
			oldReaching = reaching;
			reaching = false;

			foreach (var player in Main.ActivePlayers)
			{
				if (Condition(player))
				{
					reaching = true;

					if (!oldReaching)
					{
						NeedDeltaSync = true;
					}

					if (WorldQuestManager.NetUpdate)
					{
						NeedDeltaSync = true;
					}

					return;
				}
			}
		}
	}

	public override void ResetProgress()
	{
		base.ResetProgress();
		Reached = false;
		reaching = false;
		oldReaching = false;
	}

	public override void SaveData(TagCompound tag)
	{
		base.SaveData(tag);
		tag.Add(nameof(Reached), Reached);
	}

	public override void LoadData(TagCompound tag)
	{
		base.LoadData(tag);
		if (tag.TryGet(nameof(Reached), out bool reached))
		{
			Reached = reached;
		}
	}

	public override void NetSend(BinaryWriter writer)
	{
		base.NetSend(writer);
		writer.Write(Reached);
	}

	public override void NetReceive(BinaryReader reader)
	{
		base.NetReceive(reader);
		Reached = reader.ReadBoolean();
	}

	public override void SendDelta(BinaryWriter bw)
	{
		bw.Write(reaching);
		NeedDeltaSync = false;
	}

	public override void ReceiveDelta(BinaryReader br)
	{
		Reached |= br.ReadBoolean();
	}

	public override void SendMain(BinaryWriter bw)
	{
		bw.Write(Reached);
	}

	public override void ReceiveMain(BinaryReader br)
	{
		Reached = br.ReadBoolean();
	}
}
