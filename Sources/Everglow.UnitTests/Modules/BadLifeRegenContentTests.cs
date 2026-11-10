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
	private float criticalDamage;
	private float addCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		criticalDamage = FoodBuffModPlayer.CriticalDamage;
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
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
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	public void SuppressionPreservesOtherNegativeSourcesInEveryOrder()
	{
		ModBuff[] effects = [LoadPlantBuff(), new RedAlgaeDebuff(), new GrubSoupBuff()];
		int? loss = null;
		foreach (int[] order in new[] { new[] { 0, 1, 2 }, new[] { 0, 2, 1 }, new[] { 1, 0, 2 }, new[] { 1, 2, 0 }, new[] { 2, 0, 1 }, new[] { 2, 1, 0 } })
		{
			bonuses.ResetEffects();
			context.Player.lifeRegen = 20;
			Apply(effects[order[0]], effects[order[1]], effects[order[2]]);
			context.Player.lifeRegen += 10;
			bonuses.PostUpdateMiscEffects();
			Assert.AreEqual(30, context.Player.lifeRegen);
			bonuses.UpdateBadLifeRegen();
			loss ??= context.Player.lifeRegen;
			Assert.AreEqual(loss.Value, context.Player.lifeRegen);
		}

		Assert.IsTrue(loss!.Value < 0);
	}

	[TestMethod]
	public void AllFourFoodsUseExecutedMangoHookRegardlessOfOrder()
	{
		int Measure(bool mango, bool mangoFirst)
		{
			bonuses.ResetEffects();
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
			Assert.AreEqual(0f, context.Player.lifeRegenTime);
			return context.Player.lifeRegen;
		}

		int mangoFirst = Measure(true, true);
		Assert.AreEqual(mangoFirst, Measure(true, false));
		Assert.AreNotEqual(mangoFirst, Measure(false, false));
	}

	[TestMethod]
	[DataRow(-1, true)]
	[DataRow(-2, true)]
	[DataRow(-3, true)]
	[DataRow(-3, false)]
	public void HungerCapturesWorldAndSatietyButReadsFinalCapacity(int satiety, bool starvation)
	{
		var hunger = context.Attach<FoodModPlayer>();
		int Settle(int extraLife)
		{
			bonuses.ResetEffects();
			context.Player.starving = false;
			Main.dontStarveWorld = starvation;
			typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, satiety);
			context.Player.lifeRegen = 10;
			context.Player.lifeRegenTime = 80;
			context.Player.statLifeMax2 = 400;
			hunger.PostUpdateBuffs();
			Main.dontStarveWorld = !starvation;
			typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, 0);
			context.Player.statLifeMax2 += extraLife;
			bonuses.PostUpdateEquips();
			bonuses.UpdateBadLifeRegen();
			return context.Player.lifeRegen;
		}

		int regen = Settle(250);
		Assert.AreEqual(650, context.Player.statLifeMax2);
		Assert.AreEqual(starvation && satiety == -3, context.Player.starving);
		Assert.AreEqual(80f, context.Player.lifeRegenTime, "Hunger does not reset the natural-bonuses timer.");
		if (!starvation)
		{
			Assert.AreEqual(10, regen);
		}
		else if (satiety == -2)
		{
			int atOriginalCapacity = Settle(0);
			Assert.IsTrue(regen < atOriginalCapacity);
			Assert.IsTrue(atOriginalCapacity < 0);
		}
		else
		{
			Assert.AreEqual(0, regen);
		}
	}

	[TestMethod]
	public void NonSuppressingLossesSubtractFromPositiveRegenerationAndUseFinalLife()
	{
		int Settle(int extraLife)
		{
			bonuses.ResetEffects();
			context.Player.lifeRegen = 100;
			context.Player.lifeRegenTime = 80;
			context.Player.statLifeMax2 = 400;
			Apply(new Charred(), new RedAlgaeDebuff(), new AuburnSelfReinforcing());
			// Lighting is a graphics dependency: capture the already-evaluated high-light branch.
			bonuses.ShadowPotionInBrightLight = true;
			context.Player.statLifeMax2 += extraLife;
			bonuses.PostUpdateEquips();
			bonuses.UpdateBadLifeRegen();
			return context.Player.lifeRegen;
		}

		int atFinalCapacity = Settle(250);
		Assert.IsTrue(atFinalCapacity < Settle(0));
		Assert.AreEqual(80f, context.Player.lifeRegenTime, "ShadowPotion resets this earlier in its own light-dependent branch.");
	}

	[TestMethod]
	public void BothInfernosClampAfterLossesWithoutReplacingAStrongerCombinedLoss()
	{
		int Measure(bool inferno, float speed)
		{
			bonuses.ResetEffects();
			context.Player.lifeRegen = 0;
			Apply(new Charred(), new RedAlgaeDebuff(), new AuburnSelfReinforcing());
			// Buff VFX require the game renderer; these flags are captured by their player hooks.
			bonuses.FireflyInferno = bonuses.AcytaeaInferno = inferno;
			context.Player.velocity = new Vector2(speed, 0);
			bonuses.UpdateBadLifeRegen();
			return context.Player.lifeRegen;
		}

		int Clamp(float speed)
		{
			bonuses.ResetEffects();
			context.Player.lifeRegen = 0;
			bonuses.FireflyInferno = bonuses.AcytaeaInferno = true;
			context.Player.velocity = new Vector2(speed, 0);
			bonuses.UpdateBadLifeRegen();
			return context.Player.lifeRegen;
		}

		int losses = Measure(false, 0f);
		Assert.AreEqual(Math.Min(losses, Clamp(0f)), Measure(true, 0f));
		Assert.AreEqual(Math.Min(losses, Clamp(10f)), Measure(true, 10f));
		Assert.IsTrue(Measure(true, 10f) < Measure(true, 0f));
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
		Assert.IsTrue(context.Player.statLifeMax2 > 200);
		int settled = context.Player.lifeRegen;
		Assert.IsTrue(settled >= 4);
		bonuses.UpdateBadLifeRegen();
		Assert.IsTrue(context.Player.lifeRegen < settled);
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
		context.Player.lifeRegen = 20;
		bonuses.UpdateBadLifeRegen();
		int grubOnly = context.Player.lifeRegen;
		Assert.IsTrue(grubOnly < 20);
		bonuses.ResetEffects();
		Apply(new GrubSoupBuff(), new MangoBuff());
		context.Player.lifeRegen = 20;
		bonuses.UpdateBadLifeRegen();
		Assert.AreNotEqual(grubOnly, context.Player.lifeRegen, "Mango must also expire.");
	}

	[TestMethod]
	public void SevereHungerUsesNativeLifeRegenerationDamage()
	{
		Main.dontStarveWorld = true;
		var hunger = context.Attach<FoodModPlayer>();
		typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, -3);
		context.Player.statLifeMax2 = context.Player.statLife = 100;
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
			var updateLifeRegen = typeof(Player).GetMethod("UpdateLifeRegen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!;
			var regen = new List<int>();
			foreach (int maxLife in new[] { 100, 200, 650 })
			{
				context.Player.statLifeMax2 = context.Player.statLife = maxLife;
				context.Player.lifeRegen = 20;
				context.Player.lifeRegenTime = 80;
				updateLifeRegen.Invoke(context.Player, null);
				regen.Add(context.Player.lifeRegen);
				Assert.AreEqual(0f, context.Player.lifeRegenTime);
			}

			Assert.IsTrue(regen[0] > regen[1] && regen[1] > regen[2]);
			Assert.IsTrue(regen[2] < 0);
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
