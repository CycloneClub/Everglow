using System.Reflection;
using Everglow.Commons.Mechanics.Events;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Everglow.UnitTests.Function.Events;

[TestClass]
[DoNotParallelize]
public class EventSystemTest
{
	private int oldMode;
	private bool oldDedServ;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		oldMode = Main.netMode;
		oldDedServ = Main.dedServ;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.dedServ = true;
		new EventSystem().Unload();
	}

	[TestCleanup]
	public void Cleanup()
	{
		new EventSystem().Unload();
		Main.netMode = oldMode;
		Main.dedServ = oldDedServ;
	}

	[TestMethod]
	[DataRow(true)]
	[DataRow(false)]
	public void EventLayerKeepsProgressBarsInUISpace(bool hasVanillaInvasionLayer)
	{
		var before = new LegacyGameInterfaceLayer("Before", () => true);
		var minimap = new LegacyGameInterfaceLayer("Vanilla: Map / Minimap", () => true, InterfaceScaleType.UI);
		var layers = new List<GameInterfaceLayer> { before };
		if (hasVanillaInvasionLayer)
		{
			layers.Add(new LegacyGameInterfaceLayer("Vanilla: Invasion Progress Bars", () => true, InterfaceScaleType.UI));
		}
		layers.Add(minimap);

		new EventSystem().ModifyInterfaceLayers(layers);

		Assert.AreEqual(3, layers.Count);
		Assert.AreSame(before, layers[0]);
		Assert.AreSame(minimap, layers[2]);
		Assert.AreEqual(InterfaceScaleType.UI, layers[1].ScaleType,
			"Progress bars positioned at the screen edge must not use world zoom.");
	}

	[TestMethod]
	public void HiddenReplicaEventDoesNotTouchTheSpriteBatch()
	{
		var e = new TestReplicaEvent();
		bool oldPaused = Main.gamePaused;
		try
		{
			Main.gamePaused = false;
			e.Draw(null!);
		}
		finally
		{
			Main.gamePaused = oldPaused;
		}
	}

	[TestMethod]
	public void RepeatedActivationDoesNotDuplicateGameplay()
	{
		var e = new TestEvent();
		Assert.IsTrue(EventSystem.Activate(e));
		Assert.IsFalse(EventSystem.Activate(e));
		Assert.AreEqual(1, e.Activations);
	}

	[TestMethod]
	public void EventCanStopDuringUpdateWithoutSkippingOtherEvents()
	{
		var first = new TestEvent { StopOnUpdate = true };
		var second = new TestEvent();
		EventSystem.Activate(first);
		EventSystem.Activate(second);
		new EventSystem().PostUpdateEverything();
		Assert.IsFalse(first.Active);
		Assert.AreEqual(1, second.Updates);
	}

	[TestMethod]
	public void LeavingWorldClearsActiveEvents()
	{
		var e = new TestEvent();
		EventSystem.Activate(e);
		new EventSystem().OnWorldUnload();
		Assert.IsFalse(e.Active);
		new EventSystem().PostUpdateEverything();
		Assert.AreEqual(0, e.Updates);
	}

	[TestMethod]
	public void ClientCannotStartStopOrUpdateNetworkedGameplay()
	{
		var e = CreateNetworkedEvent();
		Main.netMode = NetmodeID.MultiplayerClient;
		Assert.IsFalse(EventSystem.Activate(e));
		Main.netMode = NetmodeID.SinglePlayer;
		EventSystem.Activate(e);
		Main.netMode = NetmodeID.MultiplayerClient;
		Assert.IsFalse(EventSystem.Deactivate(e));
		new EventSystem().PostUpdateEverything();
		Assert.AreEqual(0, e.Updates);
		Assert.AreEqual(1, e.ClientUpdates);
	}

	[TestMethod]
	public void LateJoinSnapshotRestoresProgressWithoutStartingAgain()
	{
		var e = CreateNetworkedEvent();
		EventSystem.Activate(e);
		e.Value = 7;
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		new EventSystem().NetSend(writer);
		new EventSystem().ClearWorld();
		Main.netMode = NetmodeID.MultiplayerClient;
		stream.Position = 0;
		new EventSystem().NetReceive(new BinaryReader(stream));
		Assert.IsTrue(e.Active);
		Assert.AreEqual(7, e.Value);
		Assert.AreEqual(1, e.Activations);
	}

	[TestMethod]
	public void WorldSnapshotDoesNotOverwriteServerState()
	{
		var e = CreateNetworkedEvent();
		EventSystem.Activate(e);
		e.Value = 7;
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		new EventSystem().NetSend(writer);
		new EventSystem().ClearWorld();
		Main.netMode = NetmodeID.Server;
		stream.Position = 0;
		new EventSystem().NetReceive(new BinaryReader(stream));
		Assert.IsFalse(e.Active);
		Assert.AreEqual(0, e.Value);
	}

	[TestMethod]
	public void RepeatedWorldSnapshotsUpdateProgressAndStopWithoutReactivation()
	{
		var e = CreateNetworkedEvent();
		EventSystem.Activate(e);
		e.Value = 7;
		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		new EventSystem().NetSend(writer);
		Main.netMode = NetmodeID.MultiplayerClient;
		for (int i = 0; i < 2; i++)
		{
			stream.Position = 0;
			new EventSystem().NetReceive(new BinaryReader(stream));
		}
		Assert.IsTrue(e.Active);
		Assert.AreEqual(7, e.Value);
		Assert.AreEqual(1, e.Activations);
		new EventSystem().PostUpdateEverything();
		Assert.AreEqual(1, e.ClientUpdates);

		Main.netMode = NetmodeID.SinglePlayer;
		EventSystem.Deactivate(e);
		e.Value = 12;
		stream.SetLength(0);
		stream.Position = 0;
		new EventSystem().NetSend(writer);
		Main.netMode = NetmodeID.MultiplayerClient;
		stream.Position = 0;
		new EventSystem().NetReceive(new BinaryReader(stream));
		Assert.IsFalse(e.Active);
		Assert.AreEqual(12, e.Value);
		Assert.AreEqual(1, e.Activations);
		new EventSystem().PostUpdateEverything();
		Assert.AreEqual(1, e.ClientUpdates);
	}

	[TestMethod]
	public void KillDispatchContinuesWhenAnEventCompletes()
	{
		var first = new TestEvent { SortRank = 2 };
		var second = new TestEvent();
		var inactive = new TestEvent();
		first.OnKilled = () => EventSystem.Deactivate(first);
		EventSystem.Activate(first);
		EventSystem.Activate(second);
		EventSystem.Activate(inactive);
		EventSystem.Deactivate(inactive);

		var npc = new NPC { active = true, life = 0 };
		new EventGlobalNPC().OnKill(npc);

		Assert.IsFalse(first.Active);
		Assert.AreEqual(1, first.Kills);
		Assert.AreEqual(1, second.Kills);
		Assert.AreEqual(0, inactive.Kills);
		Assert.AreSame(npc, second.LastKilled);
	}

	[TestMethod]
	[DataRow(NetmodeID.SinglePlayer, 1)]
	[DataRow(NetmodeID.Server, 1)]
	[DataRow(NetmodeID.MultiplayerClient, 0)]
	public void KillDispatchRunsOnlyOnAuthority(int mode, int expectedKills)
	{
		var e = new TestEvent();
		EventSystem.Activate(e);
		Main.netMode = mode;
		new EventGlobalNPC().OnKill(new NPC { active = true, life = 0 });
		Assert.AreEqual(expectedKills, e.Kills);
	}

	[TestMethod]
	public void KillDispatchSkipsStoppedEventsAndDefersNewEvents()
	{
		var first = new TestEvent { SortRank = 2 };
		var stopped = new TestEvent();
		var added = new TestEvent();
		first.OnKilled = () =>
		{
			EventSystem.Deactivate(stopped);
			EventSystem.Activate(added);
		};
		EventSystem.Activate(first);
		EventSystem.Activate(stopped);

		new EventGlobalNPC().OnKill(new NPC { active = true, life = 0 });

		Assert.AreEqual(1, first.Kills);
		Assert.AreEqual(0, stopped.Kills);
		Assert.AreEqual(0, added.Kills);
		Assert.IsTrue(added.Active);
	}

	private static TestEvent CreateNetworkedEvent()
	{
		var e = new TestEvent { Synchronize = true };
		typeof(ModType).GetProperty(nameof(ModType.Mod))!.SetValue(e, new TestMod());
		e.Register();
		return e;
	}

	private sealed class TestReplicaEvent : ReplicaEvent
	{
	}

	private sealed class TestMod : Mod
	{
		public override string Name => "EventTests";
	}

	private sealed class TestEvent : ModEvent
	{
		private readonly string name = Guid.NewGuid().ToString("N");
		public override string Name => name;
		public bool Synchronize;
		public override bool Networked => Synchronize;
		public int Value;
		public int ClientUpdates;
		public override void PostUpdateEverythingClient() => ClientUpdates++;
		public override void NetSend(BinaryWriter writer) => writer.Write(Value);
		public override void NetReceive(BinaryReader reader) => Value = reader.ReadInt32();
		public override void ClearWorld()
		{
			base.ClearWorld();
			Value = 0;
		}
		public int Kills;
		public NPC? LastKilled;
		public Action? OnKilled;
		public override void OnNPCKilled(NPC npc)
		{
			Kills++;
			LastKilled = npc;
			OnKilled?.Invoke();
		}
		public int Activations;
		public int Updates;
		public bool StopOnUpdate;
		public override bool CanActivate(params object[] args) => true;
		public override void OnActivate(params object[] args) => Activations++;
		public override void PostUpdateEverything()
		{
			Updates++;
			if (StopOnUpdate)
			{
				EventSystem.Deactivate(this);
			}
		}
	}
}
