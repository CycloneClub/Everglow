using System.Reflection;
using Everglow.Commons;
using Everglow.Commons.Mechanics.Cooldown;
using Everglow.Commons.Utilities;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.YggdrasilTown;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Microsoft.Xna.Framework;
using SubworldLibrary;
using Terraria;
using Terraria.ModLoader.IO;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class PlayerCooldownMigrationTests
{
	private readonly List<Action> restore = [];
	private CooldownTestContext context = null!;
	private Player player = null!;
	private EverglowPlayer cooldownPlayer = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		player = context.Player;
		cooldownPlayer = context.Cooldowns;
		context.Register(AnemoShellCooldown.ID, typeof(AnemoShellCooldown));
		context.Register(ArenaShieldCooldown.ID, typeof(ArenaShieldCooldown));
		context.Register(ElderberryDashCooldown.ID, typeof(ElderberryDashCooldown));
		foreach (string id in new[] { "AnemoShellCooldown", "ArenaShieldCooldown", "ElderberryDashCooldown" })
		{
			Assert.IsNotNull(CooldownRegistry.GetNet(id), $"Missing migrated cooldown: {id}");
		}
	}

	[TestCleanup]
	public void Cleanup()
	{
		for (int i = restore.Count - 1; i >= 0; i--)
		{
			restore[i]();
		}
		restore.Clear();
		context.Dispose();
	}

	[TestMethod]
	public void AnemoCooldownTicksWhileUnequippedAndDeadAtItsOriginalResetHook()
	{
		var anemo = context.Attach<AnemoPlayer>();
		player.AddCooldown("AnemoShellCooldown", anemo.ProtectionCooldown);
		player.dead = true;
		cooldownPlayer.UpdateDead();
		anemo.ResetEffects();
		Assert.AreEqual(anemo.ProtectionCooldown - 1, cooldownPlayer.cooldowns["AnemoShellCooldown"].timeLeft);
		Assert.IsFalse(anemo.AnemoEquipped);

		for (int i = 0; i < anemo.ProtectionCooldown - 1; i++)
		{
			anemo.ResetEffects();
		}
		Assert.IsFalse(player.HasCooldown("AnemoShellCooldown"));
	}

	[TestMethod]
	public void ElderberryDashStartsAtFiftyAndCountsTheActivationFrame()
	{
		var dash = context.Attach<ElderberryBuffDash>();
		dash.ElderberryBuff = true;
		dash.DashDir = ElderberryBuffDash.DashRight;
		dash.PreUpdateMovement();
		Assert.AreEqual(8f, player.velocity.X);
		Assert.AreEqual(49, cooldownPlayer.cooldowns["ElderberryDashCooldown"].timeLeft);
		Assert.AreEqual(34, dash.DashTimer);

		for (int i = 0; i < 49; i++)
		{
			player.velocity = Vector2.Zero;
			dash.PreUpdateMovement();
			Assert.AreEqual(Vector2.Zero, player.velocity, "Dash must remain blocked during the last decrement.");
		}
		Assert.IsFalse(player.HasCooldown("ElderberryDashCooldown"));
		dash.PreUpdateMovement();
		Assert.AreEqual(8f, player.velocity.X);
		Assert.AreEqual(49, cooldownPlayer.cooldowns["ElderberryDashCooldown"].timeLeft);
	}

	[TestMethod]
	public void ElderberryCooldownContinuesAfterBuffExpiresWithoutExtendingDashEffect()
	{
		var dash = context.Attach<ElderberryBuffDash>();
		player.AddCooldown("ElderberryDashCooldown", 2);
		dash.ElderberryBuff = false;
		dash.DashDir = ElderberryBuffDash.DashRight;
		dash.PreUpdateMovement();
		Assert.AreEqual(1, cooldownPlayer.cooldowns["ElderberryDashCooldown"].timeLeft);
		Assert.AreEqual(0, dash.DashTimer);
		dash.PreUpdateMovement();
		Assert.IsFalse(player.HasCooldown("ElderberryDashCooldown"));
		Assert.AreEqual(Vector2.Zero, player.velocity);
	}

	[TestMethod]
	public void ArenaCooldownPausesOutsideArena()
	{
		var current = typeof(SubworldSystem).GetField("current", BindingFlags.Static | BindingFlags.NonPublic)!;
		var original = current.GetValue(null);
		restore.Add(() => current.SetValue(null, original));
		current.SetValue(null, null);
		var arena = context.Attach<ArenaPlayer>();
		player.AddCooldown("ArenaShieldCooldown", 60);
		arena.PostUpdateEquips();
		Assert.AreEqual(60, cooldownPlayer.cooldowns["ArenaShieldCooldown"].timeLeft);
	}

	[TestMethod]
	public void MigratedCooldownsDoNotTickAutomaticallyOrPersistInSavedPlayerData()
	{
		string[] ids = ["AnemoShellCooldown", "ArenaShieldCooldown", "ElderberryDashCooldown"];
		foreach (string id in ids)
		{
			player.AddCooldown(id, 60);
		}
		cooldownPlayer.PostUpdateMiscEffects();
		cooldownPlayer.UpdateDead();
		foreach (string id in ids)
		{
			Assert.AreEqual(60, cooldownPlayer.cooldowns[id].timeLeft);
			Assert.IsFalse(cooldownPlayer.cooldowns[id].cooldown.IsVisible);
		}
		var saved = new TagCompound();
		cooldownPlayer.SaveData(saved);
		Assert.AreEqual(0, saved.GetCompound(EverglowPlayer.PlayerDataCooldownsKey).Count);
	}
}
