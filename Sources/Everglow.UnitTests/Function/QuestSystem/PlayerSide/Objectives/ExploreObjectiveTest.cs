using Everglow.Commons.Mechanics.Quest.PlayerSide;
using Everglow.Commons.Mechanics.Quest.PlayerSide.Objectives;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.Personalities;

namespace Everglow.UnitTests.Function.QuestSystem;

[TestClass]
[DoNotParallelize]
public class ExploreObjectiveTest
{
	private bool _originalDedServ;
	private int _originalMyPlayer;
	private Player _originalLocalPlayer;

	private sealed class StubBiome : IShoppingBiome
	{
		private readonly bool _inBiome;

		public StubBiome(bool inBiome)
		{
			_inBiome = inBiome;
		}

		public string NameKey => "Stub";

		public bool IsInBiome(Player player) => _inBiome;
	}

	[TestInitialize]
	public void Initialize()
	{
		Terraria.Program.SavePath = string.Empty;
		_originalDedServ = Main.dedServ;
		_originalMyPlayer = Main.myPlayer;
		_originalLocalPlayer = Main.player[Main.myPlayer];
		Main.dedServ = true;
		Main.player[Main.myPlayer] = new Player();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.player[Main.myPlayer] = _originalLocalPlayer;
		Main.myPlayer = _originalMyPlayer;
		Main.dedServ = _originalDedServ;
	}

	[TestMethod]
	public void Constructor_NegativeMoveRequirement_Throws()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new ExploreObjective(new ForestBiome(), -1f));
	}

	[TestMethod]
	public void Update_CountsFullIntervalOfMovementTowardRequirement()
	{
		// Player quests tick every UpdateInterval frames. Credit velocity.Length() * UpdateInterval
		// so stated walk distance matches real movement, without treating teleports as walking.
		const float speed = 5f;
		float requirement = speed * PlayerQuestManager.UpdateInterval;
		var objective = new ExploreObjective(new StubBiome(true), requirement);
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.IsTrue(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_DoesNotCreditMoreThanOneIntervalOfMovement()
	{
		const float speed = 5f;
		float requirement = (speed * PlayerQuestManager.UpdateInterval) + 0.01f;
		var objective = new ExploreObjective(new StubBiome(true), requirement);
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_DoesNotCountMovementOutsideBiome()
	{
		const float speed = 5f;
		float requirement = speed * PlayerQuestManager.UpdateInterval;
		var objective = new ExploreObjective(new StubBiome(false), requirement);
		Main.LocalPlayer.velocity = new Vector2(speed, 0f);

		objective.Update();

		Assert.IsFalse(objective.CheckCompletion());
	}

	[TestMethod]
	public void Update_DoesNotCountTeleportDisplacement()
	{
		const float requirement = 100f;
		var objective = new ExploreObjective(new StubBiome(true), requirement);
		Main.LocalPlayer.velocity = Vector2.Zero;
		Main.LocalPlayer.position = Vector2.Zero;
		objective.Update();

		Main.LocalPlayer.position = new Vector2(10000f, 0f);
		objective.Update();

		Assert.IsFalse(objective.CheckCompletion());
	}
}
