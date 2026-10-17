using Terraria.ModLoader.IO;

namespace Everglow.Yggdrasil.YggdrasilTown.Travelers;

public enum TravelerOption
{
	SpawnSeconds,
	NpcLimit,
	ActorLimit,
	StreetWeight,
	BuildingWeight,
	PylonWeight,
	DoorCooldownSeconds,
	PylonCooldownSeconds,
	ActivityMinSeconds,
	ActivityMaxSeconds,
	TrainPassengers,
	TrainInitialPassengers,
}

public sealed class TravelerOptions
{
	public sealed record Definition(TravelerOption Option, string English, string Chinese, float Min, float Max, float Step, float Default);

	public static IReadOnlyList<Definition> Definitions { get; } = new Definition[]
	{
		new(TravelerOption.SpawnSeconds, "Spawn interval (s)", "刷新间隔（秒）", 0.25f, 10, 0.25f, 1),
		new(TravelerOption.NpcLimit, "NPC population", "NPC 人数上限", 4, 80, 1, 24),
		new(TravelerOption.ActorLimit, "Lightweight population", "轻量人数上限", 8, 120, 1, 48),
		new(TravelerOption.StreetWeight, "Street arrival weight", "街道来源权重", 0, 20, 1, 8),
		new(TravelerOption.BuildingWeight, "Building arrival weight", "建筑来源权重", 0, 20, 1, 3),
		new(TravelerOption.PylonWeight, "Pylon arrival weight", "晶塔来源权重", 0, 5, 1, 1),
		new(TravelerOption.DoorCooldownSeconds, "Door cooldown (s)", "门口刷新冷却（秒）", 2, 60, 1, 10),
		new(TravelerOption.PylonCooldownSeconds, "Pylon cooldown (s)", "晶塔刷新冷却（秒）", 10, 180, 5, 60),
		new(TravelerOption.ActivityMinSeconds, "Minimum stay (s)", "最短停留（秒）", 1, 30, 1, 3),
		new(TravelerOption.ActivityMaxSeconds, "Maximum stay (s)", "最长停留（秒）", 1, 30, 1, 9),
		new(TravelerOption.TrainPassengers, "Alight per train stop", "每站最多下客数", 0, 6, 1, 2),
		new(TravelerOption.TrainInitialPassengers, "Initial train passengers", "列车初始载客数", 0, 12, 1, 6),
	};

	private readonly float[] values = Definitions.Select(definition => definition.Default).ToArray();

	public float this[TravelerOption option]
	{
		get => values[(int)option];
		set
		{
			var definition = Definitions[(int)option];
			float finite = float.IsFinite(value) ? value : definition.Default;
			values[(int)option] = Math.Clamp(MathF.Round(finite / definition.Step) * definition.Step, definition.Min, definition.Max);
			if (option == TravelerOption.ActivityMinSeconds)
			{
				values[(int)TravelerOption.ActivityMaxSeconds] = Math.Max(this[option], this[TravelerOption.ActivityMaxSeconds]);
			}
			if (option == TravelerOption.ActivityMaxSeconds)
			{
				values[(int)TravelerOption.ActivityMinSeconds] = Math.Min(this[option], this[TravelerOption.ActivityMinSeconds]);
			}
		}
	}

	public int SpawnIntervalTicks => (int)(this[TravelerOption.SpawnSeconds] * 60);

	public int ActivityMinTicks => (int)(this[TravelerOption.ActivityMinSeconds] * 60);

	public int ActivityMaxTicks => (int)(this[TravelerOption.ActivityMaxSeconds] * 60);

	public int PopulationLimit(TravelerMode mode) => mode == TravelerMode.Off ? 0
		: (int)this[mode == TravelerMode.NPC ? TravelerOption.NpcLimit : TravelerOption.ActorLimit];

	public int TotalSourceWeight => (int)(this[TravelerOption.StreetWeight] + this[TravelerOption.BuildingWeight] + this[TravelerOption.PylonWeight]);

	/// <summary>Street = 0, building = 1, pylon = 2. All zero weights suspend natural arrivals.</summary>
	public int ChooseSource(int roll)
	{
		if (TotalSourceWeight == 0)
		{
			return -1;
		}
		roll = Math.Clamp(roll, 0, TotalSourceWeight - 1);
		if (roll < this[TravelerOption.StreetWeight])
		{
			return 0;
		}
		return roll < this[TravelerOption.StreetWeight] + this[TravelerOption.BuildingWeight] ? 1 : 2;
	}

	public void ApplyPreset(int density)
	{
		foreach (var definition in Definitions)
		{
			values[(int)definition.Option] = definition.Default;
		}
		this[TravelerOption.SpawnSeconds] = density == 0 ? 4 : density == 2 ? 0.5f : 1;
		this[TravelerOption.NpcLimit] = density == 0 ? 12 : density == 2 ? 48 : 24;
		this[TravelerOption.ActorLimit] = density == 0 ? 24 : density == 2 ? 96 : 48;
	}

	public TagCompound Save()
	{
		var tag = new TagCompound();
		foreach (var definition in Definitions)
		{
			tag[definition.Option.ToString()] = this[definition.Option];
		}
		return tag;
	}

	public void Load(TagCompound tag)
	{
		ApplyPreset(1);
		foreach (var definition in Definitions)
		{
			if (tag.TryGet<object>(definition.Option.ToString(), out var stored) && stored is float value)
			{
				this[definition.Option] = value;
			}
		}
	}
}
