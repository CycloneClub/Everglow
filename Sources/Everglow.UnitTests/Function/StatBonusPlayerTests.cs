using Everglow.Commons.Mechanics.PlayerBonuses;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Function;

[TestClass]
[DoNotParallelize]
public class StatBonusPlayerTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		ResetNativeStats(context.Player);
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void ConcreteEffectsUseFinalCapacityThenAdjustedDefenseRegardlessOfActivationOrder(bool reverse)
	{
		var player = context.Player;
		player.statLife = 100;
		player.statDefense += 10;
		Action[] enable =
		[
			() => player.statLifeMax2 += 100,
			() => player.statLifeMax2 -= 24,
			() => bonuses.OrangeCrown = true,
			() => bonuses.FragmentsOfWreckage = true,
			() => player.statDefense += 5,
			() => player.statDefense += 3,
			() => bonuses.FireflyBorageBadgeAgainstMoth = true,
			() => bonuses.YellowBellFlower = true,
			() => bonuses.WalnutClip = true,
		];
		foreach (var action in reverse ? enable.Reverse() : enable)
		{
			action();
		}
		Assert.AreEqual(276, player.statLifeMax2);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(276, player.statLifeMax2);
		Assert.AreEqual(27, (int)player.statDefense, "10 + 8 crown + 8 wreckage + 5 external + 3 rock, then native DefenseStat division rounds the badge penalty to 7.");
		Assert.AreEqual(1.06f + 0.054f + 0.095f, player.GetDamage(DamageClass.Generic).ApplyTo(1), 0.000001f);
		Assert.AreEqual(1.26f, player.GetAttackSpeed(DamageClass.Melee), 0.000001f);
		Assert.AreEqual(1f, player.GetAttackSpeed(DamageClass.SummonMeleeSpeed), 0.000001f);
		Assert.AreEqual(1f, player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
		Assert.AreEqual(0.9f, player.pickSpeed, 0.000001f);
	}

	[TestMethod]
	public void CapacityDependentEffectsReadNativeFixedLifeCosts()
	{
		var player = context.Player;
		player.statLifeMax2 = 203;
		player.statLife = 200;
		context.Player.statLifeMax2 += 50;
		player.statLifeMax2 -= 24;
		bonuses.OrangeCrown = bonuses.FragmentsOfWreckage = true;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(229, player.statLifeMax2);
		Assert.AreEqual(7, (int)player.statDefense, "Crown contributes 6 and wreckage contributes 1 from the same 229 capacity.");
		Assert.AreEqual(1.0145f, player.GetDamage(DamageClass.Generic).Additive, 0.000001f);
		Assert.AreEqual(1.2145f, player.GetAttackSpeed(DamageClass.Melee), 0.000001f);
		Assert.AreEqual(1f - 0.1f * (29f / 120f), player.pickSpeed, 0.000001f);
	}

	[TestMethod]
	[DataRow(220, 0f)]
	[DataRow(200, 0f)]
	[DataRow(140, 0.5f)]
	[DataRow(80, 1f)]
	[DataRow(1, 1f)]
	public void WreckageClampsMissingLifeAndAddsFixedSpeedToNativeBonus(int life, float scale)
	{
		context.Player.statLife = life;
		bonuses.FragmentsOfWreckage = true;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1f + 0.06f * scale, context.Player.GetDamage(DamageClass.Generic).Additive, 0.000001f);
		Assert.AreEqual(1.2f + 0.06f * scale, context.Player.GetAttackSpeed(DamageClass.Melee), 0.000001f);
		Assert.AreEqual((int)(8 * scale), (int)context.Player.statDefense);
		Assert.AreEqual(1f - 0.1f * scale, context.Player.pickSpeed, 0.000001f);
	}

	[TestMethod]
	[DataRow(49, 39)]
	[DataRow(50, 40)]
	[DataRow(51, 41)]
	public void BadgeDefenseThresholdPrecedesFlowerDamage(int defense, int expected)
	{
		context.Player.statDefense += defense;
		bonuses.FireflyBorageBadgeAgainstMoth = bonuses.YellowBellFlower = true;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(expected, (int)context.Player.statDefense);
		Assert.AreEqual(1 + expected * 0.002f, context.Player.GetDamage(DamageClass.Generic).Additive, 0.000001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void DefensePreservesPositiveNegativeComponentsAndExternalMultiplier(bool negativeContribution)
	{
		var expected = new Player.DefenseStat();
		expected += 30;
		expected -= 10;
		expected *= 1.3f;
		context.Player.statDefense = expected;
		bonuses.OrangeCrown = true;
		context.Player.statDefense += negativeContribution ? -10 : 10;
		context.Player.statDefense += 4;
		bonuses.FireflyBorageBadgeAgainstMoth = true;
		expected += 6;
		expected += negativeContribution ? -10 : 10;
		expected += 4;
		// Keep DefenseStat division: it scales the native multiplier and rounds only on conversion.
		expected -= expected <= 50 ? expected / 5 : 10;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(expected, context.Player.statDefense, "Do not collapse DefenseStat into a single integer before adding or subtracting defense.");
	}

	[TestMethod]
	[DataRow(99, 0.0025f)]
	[DataRow(100, 0f)]
	[DataRow(101, 0f)]
	public void FlowerAndWalnutAddToNativeDamageWithoutChangingItsOtherChannels(int life, float walnutBonus)
	{
		var player = context.Player;
		player.statLife = life;
		player.statDefense += 25;
		player.GetDamage(DamageClass.Generic) = new StatModifier(1.2f, 1.5f, flat: 2, @base: 3);
		bonuses.YellowBellFlower = bonuses.WalnutClip = true;
		bonuses.PostUpdateEquips();
		var actual = player.GetDamage(DamageClass.Generic);
		Assert.AreEqual(1.25f + walnutBonus, actual.Additive, 0.000001f);
		Assert.AreEqual(1.5f, actual.Multiplicative);
		Assert.AreEqual(3f, actual.Base);
		Assert.AreEqual(2f, actual.Flat);
	}

	[TestMethod]
	[DataRow(149, 2)]
	[DataRow(150, 2)]
	[DataRow(151, 3)]
	public void BloodTearReadsStrictFinalCapacityThresholdInLaterHook(int life, int expected)
	{
		context.Player.statLifeMax2 = 150;
		context.Player.statLife = life;
		context.Player.statLifeMax2 += 100;
		context.Player.statLifeMax2 -= 50;
		bonuses.BloodTearCrystalCrown = true;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(200, context.Player.statLifeMax2);
		Assert.AreEqual(2, context.Player.lifeRegen);
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(expected, context.Player.lifeRegen);
	}

	[TestMethod]
	public void ResetClearsEveryEffectAndAllowsFreshActivation()
	{
		EnableAll();
		bonuses.ResetEffects();
		AssertInactive(bonuses);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		AssertNativeStats(context.Player);
		bonuses.ResetEffects();
		bonuses.OrangeCrown = true;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(6, (int)context.Player.statDefense);
	}

	private void EnableAll()
	{
		bonuses.DevilHeartSet = true;
		bonuses.CelesteStoneWaistPendant = true;
		bonuses.WhiteCrown = true;
		bonuses.ThermalConductor = true;

		bonuses.OrangeCrown = bonuses.YellowBellFlower = bonuses.FragmentsOfWreckage = true;
		bonuses.WalnutClip = bonuses.FireflyBorageBadgeAgainstMoth = true;
		bonuses.BloodTearCrystalCrown = true;
		bonuses.CactusBall = bonuses.GrubSoup = bonuses.MonsterLasagna = bonuses.Sashimi = true;
		bonuses.ShuckedOyster = bonuses.Mango = bonuses.AuburnSelfReinforcing = true;
		bonuses.ShadowPotionInBrightLight = bonuses.FireflyInferno = bonuses.AcytaeaInferno = true;
		bonuses.HungerSatietyLevel = -3;
		bonuses.PlagueLifeRegenLoss = 8;
		bonuses.CharredLifeRegenLoss = 36;
		bonuses.RedAlgaeLifeRegenLoss = 12;
	}

	private static void AssertInactive(StatBonusPlayer state)
	{
		Assert.IsFalse(state.DevilHeartSet);
		Assert.IsFalse(state.CelesteStoneWaistPendant);
		Assert.IsFalse(state.WhiteCrown);
		Assert.IsFalse(state.ThermalConductor);

		Assert.IsFalse(state.OrangeCrown || state.YellowBellFlower || state.FragmentsOfWreckage || state.WalnutClip
			|| state.FireflyBorageBadgeAgainstMoth || state.BloodTearCrystalCrown);
		Assert.IsFalse(state.CactusBall || state.GrubSoup || state.MonsterLasagna || state.Sashimi
			|| state.ShuckedOyster || state.Mango || state.AuburnSelfReinforcing
			|| state.ShadowPotionInBrightLight || state.FireflyInferno || state.AcytaeaInferno);
		Assert.AreEqual(0, state.HungerSatietyLevel);
		Assert.AreEqual(0, state.PlagueLifeRegenLoss);
		Assert.AreEqual(0, state.CharredLifeRegenLoss);
		Assert.AreEqual(0, state.RedAlgaeLifeRegenLoss);
	}

	private static void ResetNativeStats(Player player)
	{
		player.statLifeMax2 = player.statLife = 200;
		player.statDefense = new Player.DefenseStat();
		player.GetDamage(DamageClass.Generic) = StatModifier.Default;
		player.GetAttackSpeed(DamageClass.Melee) = 1.2f;
		player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) = 1f;
		player.GetAttackSpeed(DamageClass.Summon) = 1f;
		player.GetCritChance(DamageClass.Generic) = 20f;
		player.pickSpeed = 1f;
		player.lifeRegen = 2;
	}

	private static void AssertNativeStats(Player player)
	{
		Assert.AreEqual(200, player.statLifeMax2);
		Assert.AreEqual(0, (int)player.statDefense);
		Assert.AreEqual(StatModifier.Default, player.GetDamage(DamageClass.Generic));
		Assert.AreEqual(1.2f, player.GetAttackSpeed(DamageClass.Melee));
		Assert.AreEqual(1f, player.GetAttackSpeed(DamageClass.SummonMeleeSpeed));
		Assert.AreEqual(20f, player.GetCritChance(DamageClass.Generic));
		Assert.AreEqual(1f, player.pickSpeed);
		Assert.AreEqual(2, player.lifeRegen);
	}
}
