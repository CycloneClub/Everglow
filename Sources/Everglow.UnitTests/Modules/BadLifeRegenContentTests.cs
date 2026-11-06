using System.Reflection;
using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Core;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class BadLifeRegenContentTests
{
	private delegate void NaturalLifeRegenHook(ref float regen);

	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private bool originalStarvation;
	private UnifiedRandom originalRandom = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalStarvation = Main.dontStarveWorld;
		originalRandom = Main.rand;
		Main.rand = new NoDustRandom();
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		context.Attach<FoodBuffModPlayer>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.dontStarveWorld = originalStarvation;
		Main.rand = originalRandom;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void SuppressionPreservesOtherNegativeSourcesInEveryOrder(int first, int second, int third)
	{
		context.Player.lifeRegen = 20;
		ModBuff[] effects = [LoadPlantBuff(), new RedAlgaeDebuff(), new GrubSoupBuff()];
		Apply(effects[first], effects[second], effects[third]);
		context.Player.lifeRegen += 10;
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(30, context.Player.lifeRegen);
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(-21, context.Player.lifeRegen);
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void AllFourFoodsUseExecutedMangoHookRegardlessOfOrder(bool mango, bool mangoFirst)
	{
		context.Player.lifeRegen = -7;
		context.Player.lifeRegenTime = 80;
		if (mango && mangoFirst)
		{
			Apply(new MangoBuff());
		}
		Apply(new GrubSoupBuff(), new MonsterLasagnaBuff(), new SashimiBuff(), new ShuckedOysterBuff());
		if (mango && !mangoFirst)
		{
			Apply(new MangoBuff());
		}
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(mango ? -16 : -25, context.Player.lifeRegen);
		Assert.AreEqual(0f, context.Player.lifeRegenTime);
	}

	[TestMethod]
	[DataRow(-1, true, 0)]
	[DataRow(-2, true, -6)]
	[DataRow(-3, true, 0)]
	[DataRow(-3, false, 10)]
	public void HungerCapturesWorldAndSatietyButReadsFinalCapacity(int satiety, bool starvation, int expected)
	{
		Main.dontStarveWorld = starvation;
		var hunger = context.Attach<FoodModPlayer>();
		typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, satiety);
		context.Player.lifeRegen = 10;
		context.Player.lifeRegenTime = 80;
		context.Player.statLifeMax2 = 400;
		hunger.PostUpdateBuffs();
		Main.dontStarveWorld = !starvation;
		typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, 0);
		context.Player.statLifeMax2 += 250;
		bonuses.PostUpdateEquips();
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(650, context.Player.statLifeMax2);
		Assert.AreEqual(starvation && satiety == -3, context.Player.starving);
		Assert.AreEqual(expected, context.Player.lifeRegen);
		Assert.AreEqual(80f, context.Player.lifeRegenTime, "Hunger does not reset the natural-bonuses timer.");
	}

	[TestMethod]
	public void NonSuppressingLossesSubtractFromPositiveRegenerationAndUseFinalLife()
	{
		context.Player.lifeRegen = 100;
		context.Player.lifeRegenTime = 80;
		context.Player.statLifeMax2 = 400;
		Apply(new Charred(), new RedAlgaeDebuff(), new AuburnSelfReinforcing());
		// Lighting is a graphics dependency: capture the already-evaluated high-light branch.
		bonuses.ShadowPotionInBrightLight = true;
		context.Player.statLifeMax2 += 250;
		bonuses.PostUpdateEquips();
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(44, context.Player.lifeRegen);
		Assert.AreEqual(80f, context.Player.lifeRegenTime, "ShadowPotion resets this earlier in its own light-dependent branch.");
	}

	[TestMethod]
	[DataRow(0f, -50)]
	[DataRow(10f, -85)]
	public void BothInfernosClampAfterLossesWithoutReplacingAStrongerCombinedLoss(float speed, int expected)
	{
		context.Player.lifeRegen = 0;
		Apply(new Charred(), new RedAlgaeDebuff(), new AuburnSelfReinforcing());
		// Buff VFX require the game renderer; these flags are captured by their player hooks.
		bonuses.FireflyInferno = true;
		bonuses.AcytaeaInferno = true;
		context.Player.velocity = new Vector2(speed, 0);
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(expected, context.Player.lifeRegen);
	}

	[TestMethod]
	public void CentralHooksSettleEquipmentThenPositiveRegenThenPoison()
	{
		context.Player.statLifeMax2 = 200;
		context.Player.statLife = 190;
		context.Player.lifeRegen = 4;
		new Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.BloodTearCrystalCrown().UpdateAccessory(context.Player, false);
		Apply(new GrubSoupBuff());
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(230, context.Player.statLifeMax2);
		Assert.AreEqual(5, context.Player.lifeRegen);
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(-4, context.Player.lifeRegen);
		bonuses.ResetEffects();
		context.Player.lifeRegen = 4;
		bonuses.PostUpdateMiscEffects();
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(4, context.Player.lifeRegen);
	}

	[TestMethod]
	public void ResetClearEveryCapturedSource()
	{
		Apply(LoadPlantBuff(), new GrubSoupBuff(), new MonsterLasagnaBuff(), new SashimiBuff(), new ShuckedOysterBuff(), new MangoBuff(), new Charred(), new RedAlgaeDebuff(), new AuburnSelfReinforcing());
		bonuses.HungerSatietyLevel = -3;
		bonuses.PlagueLifeRegenLoss = 8;
		bonuses.ShadowPotionInBrightLight = true;
		bonuses.FireflyInferno = bonuses.AcytaeaInferno = true;
		bonuses.ResetEffects();
		context.Player.lifeRegen = 20;
		context.Player.lifeRegenTime = 80;
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(20, context.Player.lifeRegen);
		Assert.AreEqual(80f, context.Player.lifeRegenTime);
		bonuses.ResetEffects();
		Apply(new GrubSoupBuff());
		bonuses.UpdateBadLifeRegen();
		Assert.AreEqual(-4, context.Player.lifeRegen, "Mango must also expire.");
	}

	[TestMethod]
	[DataRow(100, -4)]
	[DataRow(200, -8)]
	[DataRow(650, -26)]
	public void SevereHungerUsesNativeLifeRegenerationDamage(int maxLife, int expected)
	{
		Main.dontStarveWorld = true;
		var hunger = context.Attach<FoodModPlayer>();
		typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, -3);
		context.Player.statLifeMax2 = context.Player.statLife = maxLife;
		context.Player.lifeRegen = 20;
		context.Player.lifeRegenTime = 80;
		hunger.PostUpdateBuffs();
		Assert.IsTrue(context.Player.starving);

		var instances = (ModPlayer[])typeof(Player).GetField("modPlayers", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(context.Player)!;
		var savedHooks = new List<(FieldInfo Field, object? Value)>();
		var originalSceneMetrics = Main.SceneMetrics;
		try
		{
			Main.SceneMetrics = new SceneMetrics();
			// Populate only the native dispatcher hooks used by UpdateLifeRegen.
			foreach (string name in new[] { "UpdateBadLifeRegen", "UpdateLifeRegen", "NaturalLifeRegen" })
			{
				var field = typeof(PlayerLoader).GetField("Hook" + name, BindingFlags.Static | BindingFlags.NonPublic)!;
				savedHooks.Add((field, field.GetValue(null)));
				var hooks = name switch
				{
					"UpdateBadLifeRegen" => HookList<ModPlayer>.Create<Action>(p => p.UpdateBadLifeRegen),
					"UpdateLifeRegen" => HookList<ModPlayer>.Create<Action>(p => p.UpdateLifeRegen),
					_ => HookList<ModPlayer>.Create<NaturalLifeRegenHook>(p => p.NaturalLifeRegen),
				};
				hooks.Update(instances);
				field.SetValue(null, hooks);
			}
			typeof(Player).GetMethod("UpdateLifeRegen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.Invoke(context.Player, null);
			Assert.AreEqual(expected, context.Player.lifeRegen);
			Assert.AreEqual(0f, context.Player.lifeRegenTime);
		}
		finally
		{
			Main.SceneMetrics = originalSceneMetrics;
			foreach (var (field, value) in savedHooks)
			{
				field.SetValue(null, value);
			}
		}
	}

	private void Apply(params ModBuff[] buffs)
	{
		int index = 0;
		foreach (var buff in buffs)
		{
			buff.Update(context.Player, ref index);
		}
	}

	private static ModBuff LoadPlantBuff()
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Plant", "bin", configuration, "net8.0", "Everglow.Plant.dll"));
		return (ModBuff)Activator.CreateInstance(Assembly.LoadFrom(path).GetType("Everglow.Plant.Buffs.CactusBallBuff", true)!)!;
	}

	private sealed class NoDustRandom : UnifiedRandom
	{
		public override int Next(int maxValue) => maxValue - 1;
	}
}
