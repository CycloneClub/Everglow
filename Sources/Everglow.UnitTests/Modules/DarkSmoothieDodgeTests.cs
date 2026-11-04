using Everglow.Food.Buffs;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class DarkSmoothieDodgeTests
{
	private CooldownTestContext context = null!;
	private FoodBuffModPlayer effects = null!;
	private UnifiedRandom originalRandom = null!;
	private int originalMyPlayer;
	private float originalCriticalDamage;
	private float originalAddCritDamage;
	private int dodgeEffects;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalRandom = Main.rand;
		originalMyPlayer = Main.myPlayer;
		originalCriticalDamage = FoodBuffModPlayer.CriticalDamage;
		originalAddCritDamage = FoodBuffModPlayer.AddCritDamage;
		context = new CooldownTestContext();
		effects = context.Attach<FoodBuffModPlayer>();
		Main.myPlayer = context.Player.whoAmI;
		On_Player.NinjaDodge += CountDodgeWithoutGraphics;
	}

	[TestCleanup]
	public void Cleanup()
	{
		On_Player.NinjaDodge -= CountDodgeWithoutGraphics;
		Main.rand = originalRandom;
		Main.myPlayer = originalMyPlayer;
		FoodBuffModPlayer.CriticalDamage = originalCriticalDamage;
		FoodBuffModPlayer.AddCritDamage = originalAddCritDamage;
		context.Dispose();
	}

	[TestMethod]
	public void SuccessfulFoodDodgeReportsCancellationAndRunsNativeDodgeEffect()
	{
		ApplyBuff();
		Main.rand = new FixedRandom(0);
		Assert.IsTrue(effects.FreeDodge(new Player.HurtInfo { Dodgeable = true }));
		Assert.AreEqual(1, dodgeEffects);
	}

	[TestMethod]
	[DataRow(0.49, true)]
	[DataRow(0.5, false)]
	[DataRow(0.51, false)]
	public void DodgeChanceMatchesFiftyPercentThreshold(double roll, bool expected)
	{
		ApplyBuff();
		Main.rand = new FixedRandom(roll);
		Assert.AreEqual(expected, effects.FreeDodge(new Player.HurtInfo { Dodgeable = true }));
		Assert.AreEqual(expected ? 1 : 0, dodgeEffects);
	}

	[TestMethod]
	public void FailedFoodDodgeLeavesHitForTheNativeDamagePipeline()
	{
		ApplyBuff();
		Main.rand = new FixedRandom(0.99);
		Assert.IsFalse(effects.FreeDodge(new Player.HurtInfo { Dodgeable = true }));
		Assert.AreEqual(0, dodgeEffects);
	}

	[TestMethod]
	public void BuffExpirationRemovesDodgeOnTheNextFrame()
	{
		ApplyBuff();
		effects.ResetEffects();
		Main.rand = new FixedRandom(0);
		Assert.IsFalse(effects.FreeDodge(new Player.HurtInfo { Dodgeable = true }));
		Assert.AreEqual(0, dodgeEffects);
	}

	[TestMethod]
	public void DamageModificationDoesNotTriggerAnUnreportedDodge()
	{
		ApplyBuff();
		Main.rand = new FixedRandom(0);
		var modifiers = new Player.HurtModifiers();
		effects.ModifyHurt(ref modifiers);
		Assert.AreEqual(0, dodgeEffects);
	}

	private void ApplyBuff()
	{
		int index = 0;
		new SmoothieofDarknessBuff().Update(context.Player, ref index);
	}

	private void CountDodgeWithoutGraphics(On_Player.orig_NinjaDodge orig, Player player)
	{
		// Verify native dodge is invoked without loading its dust/shader assets.
		dodgeEffects++;
	}

	private sealed class FixedRandom(double value) : UnifiedRandom
	{
		public override double NextDouble() => value;

		public override int Next(int maxValue) => (int)(value * maxValue);
	}
}
