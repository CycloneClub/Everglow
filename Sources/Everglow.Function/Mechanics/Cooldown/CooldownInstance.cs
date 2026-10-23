using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Cooldown;

public sealed class CooldownInstance
{
	private const string NetIDSaveKey = "netID";
	private const string TimeMaxSaveKey = "timeMax";
	private const string TimeLeftSaveKey = "timeLeft";

	public CooldownInstance(Player player, CooldownNet cdN, int timeMax, int timeLeft)
	{
		this.netID = cdN.NetID;
		this.player = player;
		this.timeMax = timeMax;
		this.timeLeft = timeLeft;
		SetCooldown(cdN);
	}

	public CooldownInstance(Player player, CooldownNet cdN, int timeMax)
		: this(player, cdN, timeMax, timeMax)
	{
	}

	public CooldownInstance(BinaryReader reader)
	{
		netID = reader.ReadUInt16();
		player = Main.player[reader.ReadByte()];
		timeMax = reader.ReadInt32();
		timeLeft = reader.ReadInt32();

		string id = CooldownRegistry.Registry[netID].ID;
		SetCooldown(CooldownRegistry.GetNet(id));
	}

	internal void SetCooldown(CooldownNet cdN)
	{
		Type cdBaseT = CooldownRegistry.NameToType[cdN.ID];
		var cooldown = Activator.CreateInstance(cdBaseT) as CooldownBase;
		cooldown.Instance = this;
		this.cooldown = cooldown;
	}

	internal ushort netID;

	public Player player;

	public int timeMax;

	public int timeLeft;

	public float Progress => timeMax != 0 ? Math.Clamp(timeLeft / (float)timeMax, 0f, 1f) : 0;

	public CooldownBase cooldown;

	/// <summary>Advances once and reports natural completion.</summary>
	internal bool Tick()
	{
		if (cooldown.CanTickDown)
		{
			timeLeft--;
		}

		cooldown.Update();
		if (timeLeft > 0)
		{
			return false;
		}

		cooldown.OnCompleted();
		return true;
	}

	internal TagCompound Save()
	{
		return new TagCompound
		{
			[NetIDSaveKey] = (int)netID,
			[TimeMaxSaveKey] = timeMax,
			[TimeLeftSaveKey] = timeLeft,
		};
	}

	internal static CooldownInstance Load(TagCompound tag, string id, Player player)
	{
		if (!tag.TryGet<int>(NetIDSaveKey, out var netID))
		{
			return null;
		}

		var cdNet = CooldownRegistry.GetNet(id);
		if (cdNet == null)
		{
			Ins.Logger.Warn($"Cooldown with ID '{id}' not found in CooldownRegistry. Skipping loading for this cooldown.");
			return null;
		}

		if (cdNet.NetID != netID)
		{
			Ins.Logger.Warn($"Cooldown with ID '{id}' has mismatched netID. Expected {cdNet.NetID}, got {netID}. Skipping loading for this cooldown.");
		}

		var timeLeft = tag.GetInt(nameof(CooldownInstance.timeLeft));
		var timeMax = tag.GetInt(nameof(CooldownInstance.timeMax));

		return new CooldownInstance(player, cdNet, timeMax, timeLeft);
	}

	internal void Write(BinaryWriter writer)
	{
		writer.Write(netID);
		writer.Write((byte)player.whoAmI);
		writer.Write(timeMax);
		writer.Write(timeLeft);
	}
}
