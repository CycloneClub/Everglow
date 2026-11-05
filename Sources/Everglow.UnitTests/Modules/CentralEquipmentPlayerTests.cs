using System.Reflection;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class CentralEquipmentPlayerTests
{
	private static readonly FieldInfo ReapplyHooks = typeof(BuffLoader).GetField("HookReApplyPlayer", BindingFlags.Static | BindingFlags.NonPublic)!;
	private object? originalReapplyHooks;
	private UnifiedRandom originalRandom = null!;
	private double originalTime;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalReapplyHooks = ReapplyHooks.GetValue(null);
		ReapplyHooks.SetValue(null, Array.Empty<Func<int, Player, int, int, bool>>());
		originalRandom = Main.rand;
		originalTime = Main.time;
	}

	[TestCleanup]
	public void Cleanup()
	{
		ReapplyHooks.SetValue(null, originalReapplyHooks);
		Main.rand = originalRandom;
		Main.time = originalTime;
	}

	[TestMethod]
	public void CentralManaHooksPreserveNativeSmoothieReduction()
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		var player = context.Player;
		player.statManaMax2 = 180;
		player.statMana = 100;
		player.manaCost = 0.8f;
		player.manaRegenBonus = 7;
		player.manaRegen = 20;
		player.manaRegenCount = 40;
		new CelesteStoneWaistPendant().UpdateAccessory(player, false);
		int index = 0;
		new DevilHeartSetBuff().Update(player, ref index);
		new TropicalSmoothieBuff().Update(player, ref index);

		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(0.3f, player.manaCost, 0.000001f);
		Assert.AreEqual(10, player.manaRegenBonus);
		Assert.AreEqual(0, player.manaRegen);
		Assert.AreEqual(0, player.manaRegenCount);
		Assert.IsTrue(player.manaRegenDelay > 0);
	}

	[TestMethod]
	public void FowlbeastVulnerabilityCombinesWithNativeEnduranceOnEachHit()
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<EmptyFowlbeast.EmptyFowlbeastPlayer>();
		context.Player.buffType[0] = BuffID.OnFire;
		context.Player.buffTime[0] = 60;
		new EmptyFowlbeast().UpdateAccessory(context.Player, false);
		new MoltenCore().UpdateAccessory(context.Player, false);
		for (int hit = 0; hit < 2; hit++)
		{
			var modifiers = new Player.HurtModifiers();
			bonuses.ModifyHitByNPC(new NPC(), ref modifiers);
			modifiers.FinalDamage *= 1 - context.Player.endurance;
			bonuses.ModifyHurt(ref modifiers);
			Assert.AreEqual(114f, modifiers.FinalDamage.ApplyTo(100), 0.0001f);
		}
		bonuses.ResetEffects();
		var unequipped = new Player.HurtModifiers();
		bonuses.ModifyHitByNPC(new NPC(), ref unequipped);
		bonuses.ModifyHurt(ref unequipped);
		Assert.AreEqual(100f, unequipped.FinalDamage.ApplyTo(100));
	}

	[TestMethod]
	[DataRow(0, 1200)]
	[DataRow(60, 1200)]
	[DataRow(1800, 1800)]
	public void SpikedParmaRefreshesThornsWithoutShorteningLongerBuff(int existingTime, int expected)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<SpikedParmaPlayer>();
		if (existingTime > 0)
		{
			context.Player.buffType[0] = BuffID.Thorns;
			context.Player.buffTime[0] = existingTime;
		}
		new SpikedParma().UpdateAccessory(context.Player, false);
		bonuses.PostHurt(new Player.HurtInfo());
		int slot = context.Player.FindBuffIndex(BuffID.Thorns);
		Assert.IsTrue(slot >= 0);
		Assert.AreEqual(expected, context.Player.buffTime[slot]);
	}

	[TestMethod]
	[DataRow(11, 200, 198, 189, 198)]
	[DataRow(12, 200, 180, 193, 190)]
	[DataRow(12, 190, 180, 183, 190)]
	[DataRow(100, 198, 198, 103, 200)]
	public void RecoveryBandKeepsDamageThresholdAndCapacityLimits(int damage, int life, int mana, int expectedLife, int expectedMana)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<RecoveryBandPlayer>();
		Main.netMode = NetmodeID.Server; // Skip combat text without starting graphics.
		var player = context.Player;
		player.statLifeMax2 = player.statManaMax2 = 200;
		player.statLife = life;
		player.statMana = mana;
		new RecoveryBand().UpdateAccessory(player, false);
		var info = new Player.HurtInfo { Damage = damage };
		bonuses.OnHurt(info);
		Assert.AreEqual(life, player.statLife, "OnHurt runs before native damage subtraction.");
		Assert.AreEqual(mana, player.statMana);
		player.statLife -= damage;
		bonuses.PostHurt(info);
		Assert.AreEqual(expectedLife, player.statLife);
		Assert.AreEqual(expectedMana, player.statMana);
	}

	[TestMethod]
	[DataRow(true, 200)]
	[DataRow(false, 198)]
	public void RecoveryBandPostHurtRespectsCapacityAndUnequipping(bool equipped, int expectedLife)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<RecoveryBandPlayer>();
		Main.netMode = NetmodeID.Server;
		context.Player.statLifeMax2 = context.Player.statManaMax2 = 200;
		context.Player.statLife = context.Player.statMana = 198;
		new RecoveryBand().UpdateAccessory(context.Player, false);
		if (!equipped)
		{
			bonuses.ResetEffects();
		}
		bonuses.PostHurt(new Player.HurtInfo { Damage = 12 });
		Assert.AreEqual(expectedLife, context.Player.statLife);
		Assert.AreEqual(expectedLife, context.Player.statMana);
	}

	[TestMethod]
	[DataRow(false, 0.1, 130)]
	[DataRow(false, 0.8, 100)]
	[DataRow(true, 0.1, 100)]
	public void FuneraryGoodsKeepsKillAndChanceConditions(bool targetActive, double roll, int expectedMana)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<FuneraryGoodsPlayer>();
		Main.netMode = NetmodeID.Server;
		Main.rand = new FixedRandom(roll);
		context.Player.statManaMax2 = 200;
		context.Player.statMana = 100;
		new FuneraryGoods().UpdateAccessory(context.Player, false);
		bonuses.OnHitNPC(new NPC { active = targetActive }, new NPC.HitInfo(), 10);
		Assert.AreEqual(expectedMana, context.Player.statMana);
	}

	[TestMethod]
	[DataRow(0.1, true)]
	[DataRow(0.8, false)]
	public void HeatEmblemKeepsChanceAndDuration(double roll, bool expected)
	{
		using var context = new CooldownTestContext();
		var playerType = typeof(HeatEmblem).Assembly.GetType("Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace.HeatEmblemPlayer")!;
		var bonuses = (ModPlayer)typeof(CooldownTestContext).GetMethod(nameof(CooldownTestContext.Attach))!
			.MakeGenericMethod(playerType).Invoke(context, null)!;
		Main.rand = new FixedRandom(roll);
		new HeatEmblem().UpdateAccessory(context.Player, false);
		bonuses.OnHurt(new Player.HurtInfo { Damage = 1 });
		Assert.AreEqual(expected, context.Player.HasBuff(BuffID.OnFire));
		if (expected)
		{
			Assert.AreEqual(180, context.Player.buffTime[context.Player.FindBuffIndex(BuffID.OnFire)]);
		}
	}

	[TestMethod]
	[DataRow(60, 39, 0.1, 75)]
	[DataRow(60, 40, 0.1, 40)]
	[DataRow(61, 39, 0.1, 39)]
	[DataRow(60, 39, 0.8, 39)]
	public void ThermalConductorKeepsTimeThresholdAndChanceAlongsideSuppression(int time, int mana, double roll, int expectedMana)
	{
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		Main.netMode = NetmodeID.Server;
		Main.time = time;
		Main.rand = new FixedRandom(roll);
		context.Player.statManaMax2 = 160;
		context.Player.statMana = mana;
		new ThermalConductor().UpdateAccessory(context.Player, false);
		int index = 0;
		new DevilHeartSetBuff().Update(context.Player, ref index);
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(expectedMana, context.Player.statMana);
		Assert.AreEqual(expectedMana != mana, context.Player.HasBuff(BuffID.CursedInferno));
		Assert.AreEqual(0, context.Player.manaRegen);
	}

	private sealed class FixedRandom(double roll) : UnifiedRandom
	{
		public override double NextDouble() => roll;
	}
}
