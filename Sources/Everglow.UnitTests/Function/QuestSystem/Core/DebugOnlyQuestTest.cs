using System.Reflection;
using Everglow.Commons.FeatureFlags;
using Everglow.Commons.Mechanics.Quest.Core;
using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.Presentation;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Abstractions;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Everglow.Commons.Utilities;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class DebugOnlyQuestTest
{
	private static readonly PropertyInfo ConfigInstance = typeof(ContentInstance<EverglowConfig>).GetProperty("Instance")!;
	private object? oldConfig;
	private EverglowConfig config = null!;
	private readonly List<Type> registeredSerializers = [];

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldConfig = ConfigInstance.GetValue(null);
		config = new EverglowConfig();
		ConfigInstance.SetValue(null, config);
		RegisterSerializer(typeof(bool), new BoolTagSerializer());
		RegisterSerializer(typeof(KeyValuePair<int, int>), new TagCompoundUtils.KeyValuePairSerializer_Int_Int());
	}

	[TestCleanup]
	public void Cleanup()
	{
		ConfigInstance.SetValue(null, oldConfig);
		var serializers = (IDictionary<Type, TagSerializer>)typeof(TagSerializer)
			.GetField("serializers", BindingFlags.Static | BindingFlags.NonPublic)!.GetValue(null)!;
		foreach (Type type in registeredSerializers)
		{
			serializers.Remove(type);
		}
		registeredSerializers.Clear();
	}

	private void RegisterSerializer(Type type, TagSerializer serializer)
	{
		if (!TagSerializer.TryGetSerializer(type, out _))
		{
			typeof(TagSerializer).GetMethod("AddSerializer", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [serializer]);
			registeredSerializers.Add(type);
		}
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void AddQuest_FiltersInheritedDebugMarkerOnBothSides(bool enabled)
	{
		config.debugMode = enabled;
		var player = new PlayerQuestManager();
		var world = new WorldQuestManager();
		player.AddQuest(new DebugPlayerQuest(), PlayerQuestState.Available, showText: false);
		player.AddQuest(new RegularPlayerQuest(), PlayerQuestState.Available, showText: false);
		world.AddQuest(new DebugWorldQuest());
		world.AddQuest(new RegularWorldQuest());

		Assert.AreEqual(enabled ? 2 : 1, player.Quests.Count);
		Assert.AreEqual(2, world.Quests.Count);
		Assert.IsNotNull(player.GetQuest(nameof(RegularPlayerQuest)));
		Assert.IsNotNull(world.GetQuest(nameof(RegularWorldQuest)));
		Assert.IsNotNull(world.GetQuest(nameof(DebugWorldQuest)));
	}

	[TestMethod]
	public void ApplyData_DropsOldDebugQuestAndKeepsNormalProgressOnNextSave()
	{
		var manager = new PlayerQuestManager();
		var regular = new RegularPlayerQuest();
		manager.ApplyData(new PlayerQuestManagerData(new Dictionary<int, int> { [1] = 7 }, [new DebugPlayerQuest(), regular]));
		var saved = new TagCompound();
		manager.SaveData(saved);

		Assert.AreSame(regular, manager.Quests.Single());
		Assert.AreEqual(7, manager.NPCKillCounter[1]);
		var quests = saved.GetList<TagCompound>("_quests");
		Assert.AreEqual(1, quests.Count);
		Assert.AreEqual(typeof(RegularPlayerQuest).FullName, quests[0].GetString("Type"));
	}

	[TestMethod]
	public void WorldLoad_DoesNotRestoreFilteredQuestFromOldSave()
	{
		var manager = new WorldQuestManager();
		manager.AddQuest(new DebugWorldQuest());
		manager.AddQuest(new RegularWorldQuest());
		manager.LoadData(new TagCompound
		{
			[nameof(DebugWorldQuest)] = new TagCompound { ["State"] = (int)WorldQuestState.Completed },
			[nameof(RegularWorldQuest)] = new TagCompound { ["State"] = (int)WorldQuestState.Completed },
		});
		var saved = new TagCompound();
		manager.SaveData(saved);

		Assert.IsFalse(saved.ContainsKey(nameof(DebugWorldQuest)));
		Assert.AreEqual(WorldQuestState.Locked, manager.GetQuest<DebugWorldQuest>().State);
		Assert.AreEqual(WorldQuestState.Completed, manager.GetQuest(nameof(RegularWorldQuest)).State);
		Assert.IsTrue(saved.ContainsKey(nameof(RegularWorldQuest)));
	}

	private class RegularPlayerQuest : PlayerQuestBase
	{
	}

	[TestMethod]
	public void Presentation_HidesDebugQuestWithoutRemovingRegistration()
	{
		var player = new PlayerQuestManager();
		var world = new WorldQuestManager();
		var quest = new DebugWorldQuest();
		world.AddQuest(quest);
		var presentation = new QuestPresentationService(player, new PlayerQuestActions(player), world, new WorldQuestActions(world));

		Assert.AreEqual(0, presentation.GetAll().Count);
		Assert.IsFalse(presentation.TryGet(new QuestIdentity(QuestSide.World, quest.Name, quest.Name), out _));
		Assert.AreSame(quest, world.GetQuest<DebugWorldQuest>());
	}

	[DebugOnlyQuest]
	private abstract class DebugPlayerBase : PlayerQuestBase
	{
	}

	private sealed class DebugPlayerQuest : DebugPlayerBase
	{
	}

	private class RegularWorldQuest : WorldQuestBase
	{
		public RegularWorldQuest()
		{
			Objectives.Add(new WorldReachObjective(_ => false));
		}
	}

	[DebugOnlyQuest]
	private abstract class DebugWorldBase : RegularWorldQuest
	{
	}

	private sealed class DebugWorldQuest : DebugWorldBase
	{
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FullSync_WithOppositeLocalDebugSetting_PreservesStreamAndProgress(bool serverDebug)
	{
		config.debugMode = serverDebug;
		var server = new WorldQuestManager();
		server.AddQuest(new DebugWorldQuest());
		server.AddQuest(new RegularWorldQuest());
		server.LoadData(new TagCompound
		{
			[nameof(DebugWorldQuest)] = new TagCompound { ["State"] = (int)WorldQuestState.Completed },
			[nameof(RegularWorldQuest)] = new TagCompound { ["State"] = (int)WorldQuestState.Completed },
		});
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream, System.Text.Encoding.UTF8, leaveOpen: true);
		server.NetSend(writer);
		writer.Write(123456);
		writer.Flush();

		config.debugMode = !serverDebug;
		var client = new WorldQuestManager();
		client.AddQuest(new DebugWorldQuest());
		client.AddQuest(new RegularWorldQuest());
		stream.Position = 0;
		using var reader = new BinaryReader(stream);
		client.NetReceive(reader);

		Assert.AreEqual(123456, reader.ReadInt32());
		Assert.AreEqual(stream.Length, stream.Position);
		Assert.AreEqual(2, client.Quests.Count);
		Assert.AreEqual(WorldQuestState.Completed, client.GetQuest(nameof(RegularWorldQuest)).State);
		Assert.AreEqual(serverDebug ? WorldQuestState.Completed : WorldQuestState.Locked, client.GetQuest<DebugWorldQuest>().State);
	}
}
