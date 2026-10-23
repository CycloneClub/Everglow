using System.Reflection;
using Everglow.Commons;
using Everglow.Commons.Physics.MassSpringSystem;
using Everglow.Commons.Templates.Weapons.StabbingSwords;
using Everglow.Commons.TileHelper;
using Everglow.Commons.Utilities;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class FunctionCooldownMigrationTests
{
	private const string RecoveryID = "StabbingSwordRecoveryCooldown";
	private const string SwitchID = "HangingTileSwitchCooldown";
	private CooldownTestContext context = null!;
	private StabbingSwordStaminaPlayer stamina = null!;
	private HangingTile_Player hanging = null!;
	private Dictionary<Point, Player> originalGraspingPlayers = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		context.Register(RecoveryID, typeof(StabbingSwordRecoveryCooldown));
		context.Register(SwitchID, typeof(HangingTileSwitchCooldown));
		stamina = context.Attach<StabbingSwordStaminaPlayer>();
		hanging = context.Attach<HangingTile_Player>();
		originalGraspingPlayers = HangingTile.RopeGraspingPlayer;
		HangingTile.RopeGraspingPlayer = [];
	}

	[TestCleanup]
	public void Cleanup()
	{
		HangingTile.RopeGraspingPlayer = originalGraspingPlayers;
		context.Dispose();
	}

	[TestMethod]
	public void Stamina_FirstDepletionWaitsFortyFiveIdleUpdatesBeforeRecovery()
	{
		stamina.CheckStamina(10);
		stamina.PostUpdate();
		Assert.IsTrue(context.Player.HasCooldown(RecoveryID));
		Assert.AreEqual(44, Remaining(RecoveryID));
		for (int i = 1; i < 45; i++)
		{
			stamina.PostUpdate();
		}
		Assert.AreEqual(90f, StaminaValue());
		Assert.IsFalse(context.Player.HasCooldown(RecoveryID));
		stamina.PostUpdate();
		Assert.AreEqual(90.5f, StaminaValue());
	}

	[TestMethod]
	public void Stamina_FullResourcePausesInitialRecoveryDelay()
	{
		for (int i = 0; i < 60; i++)
		{
			stamina.PostUpdate();
		}
		Assert.AreEqual(45, Remaining(RecoveryID));
		stamina.CheckStamina(10);
		stamina.PostUpdate();
		Assert.AreEqual(44, Remaining(RecoveryID));
		Assert.AreEqual(90f, StaminaValue());
	}

	[TestMethod]
	public void Stamina_AnyItemAnimationRefreshesDelayWithoutAutomaticDoubleTick()
	{
		stamina.CheckStamina(10);
		stamina.PostUpdate();
		context.Cooldowns.PostUpdateMiscEffects();
		Assert.AreEqual(44, Remaining(RecoveryID));
		context.Player.itemAnimation = 1;
		stamina.PostUpdate();
		Assert.AreEqual(45, Remaining(RecoveryID));
		stamina.PostUpdate();
		Assert.AreEqual(45, Remaining(RecoveryID));
		context.Player.itemAnimation = 0;
		stamina.PostUpdate();
		Assert.AreEqual(44, Remaining(RecoveryID));
	}

	[TestMethod]
	public void Stamina_ExhaustionKeepsBothRecoveryContributionsAfterWaiting()
	{
		stamina.CheckStamina(100);
		for (int i = 0; i < 45; i++)
		{
			stamina.PostUpdate();
		}
		Assert.IsTrue(stamina.StaminaRecovering);
		Assert.AreEqual(0f, StaminaValue());
		stamina.PostUpdate();
		Assert.AreEqual(1.5f, StaminaValue());
	}

	[TestMethod]
	public void Stamina_DeathRetainsPausedDelayButSaveOmitsIt()
	{
		stamina.CheckStamina(10);
		stamina.PostUpdate();
		context.Cooldowns.UpdateDead();
		Assert.AreEqual(44, Remaining(RecoveryID));
		var tag = new TagCompound();
		context.Cooldowns.SaveData(tag);
		Assert.IsFalse(tag.GetCompound(EverglowPlayer.PlayerDataCooldownsKey).ContainsKey(RecoveryID));
		stamina.PostUpdate();
		Assert.AreEqual(43, Remaining(RecoveryID));
	}

	[TestMethod]
	public void Rope_FailedAttachStillLocksSwitchAndRemovalForThirtyPlayerUpdates()
	{
		var tile = new TestHangingTile();
		var point = new Point(10, 20);
		var rope = Rope.Create_Vine(Vector2.Zero, 2);
		var occupant = new Player();
		HangingTile.RopeGraspingPlayer[point] = occupant;
		tile.AddPlayerToRope(context.Player, rope, point);
		Assert.AreSame(occupant, HangingTile.RopeGraspingPlayer[point]);
		Assert.AreEqual(30, Remaining(SwitchID));
		context.Cooldowns.PostUpdateMiscEffects();
		Assert.AreEqual(30, Remaining(SwitchID));
		HangingTile.RopeGraspingPlayer[point] = context.Player;
		for (int i = 0; i < 29; i++)
		{
			hanging.PostUpdate();
		}
		tile.RemovePlayerFromRope(context.Player, rope, point);
		Assert.IsTrue(HangingTile.RopeGraspingPlayer.ContainsKey(point));
		hanging.PostUpdate();
		tile.RemovePlayerFromRope(context.Player, rope, point);
		Assert.IsFalse(HangingTile.RopeGraspingPlayer.ContainsKey(point));
		Assert.IsFalse(context.Player.HasCooldown(SwitchID));
	}

	[TestMethod]
	public void Rope_DeathDoesNotClearSwitchLockAndSaveDoesNotPersistIt()
	{
		var point = new Point(10, 20);
		HangingTile.RopeGraspingPlayer[point] = new Player();
		new TestHangingTile().AddPlayerToRope(context.Player, Rope.Create_Vine(Vector2.Zero, 2), point);
		context.Cooldowns.UpdateDead();
		Assert.AreEqual(30, Remaining(SwitchID));
		var tag = new TagCompound();
		context.Cooldowns.SaveData(tag);
		Assert.IsFalse(tag.GetCompound(EverglowPlayer.PlayerDataCooldownsKey).ContainsKey(SwitchID));
		hanging.PostUpdate();
		Assert.AreEqual(29, Remaining(SwitchID));
	}

	private int Remaining(string id) => context.Cooldowns.cooldowns[id].timeLeft;

	private float StaminaValue() => (float)typeof(StabbingSwordStaminaPlayer)
		.GetField("stamina", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(stamina)!;

	private sealed class TestHangingTile : HangingTile
	{
	}
}
