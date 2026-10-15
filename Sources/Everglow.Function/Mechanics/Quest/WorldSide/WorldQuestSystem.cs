using SubworldLibrary;
using Terraria.ModLoader.IO;

namespace Everglow.Commons.Mechanics.Quest.WorldSide;

public class WorldQuestSystem : ModSystem, ICopyWorldData
{
	private const string QuestManagerKey = "QuestManagerData";

	public WorldQuestManager Manager { get; private set; }

	public WorldQuestActions Actions { get; private set; }

	public override void Load()
	{
		Manager = new();
		Actions = new WorldQuestActions(Manager);
		Manager.Load();
	}

	public override void Unload()
	{
		Manager.Unload();
		Manager = null;
		Actions = null;
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
		if (tag.TryGet<TagCompound>(QuestManagerKey, out var data))
		{
			Manager.LoadData(data);
		}
	}

	public override void SaveWorldData(TagCompound tag)
	{
		var data = new TagCompound();
		Manager.SaveData(data);
		tag.Add(QuestManagerKey, data);
	}

	public override void OnWorldUnload()
	{
		Manager.Reset();
	}

	void ICopyWorldData.CopyMainWorldData()
	{
		var tag = new TagCompound();
		SaveWorldData(tag);
		SubworldSystem.CopyWorldData("EverglowQuestManager", tag);
	}

	void ICopyWorldData.ReadCopiedMainWorldData()
	{
		var tag = SubworldSystem.ReadCopiedWorldData<TagCompound>("EverglowQuestManager");
		LoadWorldData(tag);
	}
}
