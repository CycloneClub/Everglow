using System.IO;
using Everglow.Commons.Mechanics.Quest.WorldSide;
using Everglow.Commons.Mechanics.Quest.WorldSide.Objectives;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class WorldExploreObjectiveTest
{
	private bool _originalDedServ;
	private int _originalNetMode;
	private int _originalMyPlayer;
	private Player _originalLocalPlayer;

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Main.dedServ;
		_originalNetMode = Main.netMode;
		_originalMyPlayer = Main.myPlayer;
		_originalLocalPlayer = Main.player[Main.myPlayer];
		Main.dedServ = true;
		Main.netMode = NetmodeID.SinglePlayer;
		Main.myPlayer = 0;
		Main.player[Main.myPlayer] = new Player();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player[Main.myPlayer] = _originalLocalPlayer;
		Main.myPlayer = _originalMyPlayer;
		Main.netMode = _originalNetMode;
		Main.dedServ = _originalDedServ;
	}

	[TestMethod]
	public void Update_CountsFullIntervalOfMovementTowardRequirement()
	{
		// World quests tick every UpdateInterval frames (1 in DEBUG, 30 in Release).
		// Credit velocity.Length() * UpdateInterval so walk distance matches real movement
		// without treating teleports as walking.
		const float speed = 5f;
		int requirement = (int)(speed * WorldQuestManager.UpdateInterval);
		var objective = new WorldExploreObjective(requirement, _ => true, "explore");
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.AreEqual(speed * WorldQuestManager.UpdateInterval, objective.CurrentDistance);
		Assert.IsTrue(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_DoesNotCountMovementWhenConditionFails()
	{
		const float speed = 5f;
		int requirement = (int)(speed * WorldQuestManager.UpdateInterval);
		var objective = new WorldExploreObjective(requirement, _ => false, "explore");
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.AreEqual(0f, objective.CurrentDistance);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_DoesNotCountTeleportDisplacement()
	{
		var objective = new WorldExploreObjective(100, _ => true, "explore");
		Main.LocalPlayer.velocity = Vector2.Zero;
		Main.LocalPlayer.position = Vector2.Zero;
		objective.Update();

		Main.LocalPlayer.position = new Vector2(10000f, 0f);
		objective.Update();

		Assert.AreEqual(0f, objective.CurrentDistance);
		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_Client_AccumulatesScaledDelta()
	{
		Main.netMode = NetmodeID.MultiplayerClient;
		const float speed = 5f;
		var objective = new WorldExploreObjective(int.MaxValue, _ => true, "explore");
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.IsTrue(objective.NeedDeltaSync);

		using var stream = new MemoryStream();
		using var writer = new BinaryWriter(stream);
		objective.SendDelta(writer);
		stream.Position = 0;
		using var reader = new BinaryReader(stream);

		Assert.AreEqual(speed * WorldQuestManager.UpdateInterval, reader.ReadSingle());
		Assert.IsFalse(objective.NeedDeltaSync);
	}
}
