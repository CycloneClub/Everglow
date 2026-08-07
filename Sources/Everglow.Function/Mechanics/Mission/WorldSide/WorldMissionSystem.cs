using SubworldLibrary;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Mission.WorldSide;

public class WorldMissionSystem : ModSystem, ICopyWorldData
{
	private const string MissionManagerKey = "MissionManagerData";

	public WorldMissionManager Manager { get; private set; }

	public override void Load()
	{
		Manager = new();
		Manager.Load();
	}

	public override void Unload()
	{
		Manager.Unload();
		Manager = null;
	}

	public override void SetStaticDefaults()
	{
		Manager.Initialize();
	}

	public override void NetSend(BinaryWriter writer)
	{
		Manager.NetSend(writer);
	}

	public override void NetReceive(BinaryReader reader)
	{
		Manager.NetReceive(reader);
	}

	public override void LoadWorldData(TagCompound tag)
	{
		if (tag.TryGet<TagCompound>(MissionManagerKey, out var data))
		{
			Manager.LoadData(data);
		}
	}

	public override void SaveWorldData(TagCompound tag)
	{
		var data = new TagCompound();
		Manager.SaveData(data);
		tag.Add(MissionManagerKey, data);
	}

	public override void OnWorldUnload()
	{
		Manager.Reset();
	}

	void ICopyWorldData.CopyMainWorldData()
	{
		var tag = new TagCompound();
		SaveWorldData(tag);
		SubworldSystem.CopyWorldData("EverglowMissionManager", tag);
	}

	void ICopyWorldData.ReadCopiedMainWorldData()
	{
		var tag = SubworldSystem.ReadCopiedWorldData<TagCompound>("EverglowMissionManager");
		LoadWorldData(tag);
	}
}
