using System.Reflection;
using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class OrdinaryMovementBonusTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private bool raining;
	private float criticalDamage;
	private float addCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		criticalDamage = FoodBuffModPlayer.CriticalDamage;
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
		raining = Main.raining;
		Main.raining = false;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		context.Attach<FoodBuffModPlayer>();
		context.Player.moveSpeed = 1.4f;
		context.Player.maxRunSpeed = 3f;
		context.Player.accRunSpeed = 6f;
		context.Player.runAcceleration = 0.08f;
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.raining = raining;
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(BurgerBuff))]
	[DataRow(typeof(EscargotBuff))]
	[DataRow(typeof(MilkshakeBuff))]
	[DataRow(typeof(JellyBallStick))]
	public void OrdinaryEffectsAddOnceToExistingMoveSpeed(Type effectType)
	{
		var buff = (ModBuff)Activator.CreateInstance(effectType)!;
		int index = 0;
		buff.Update(context.Player, ref index);
		float boosted = context.Player.moveSpeed;
		Assert.AreNotEqual(1.4f, boosted);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(boosted, context.Player.moveSpeed, 0.000001f);
		Assert.AreEqual(3f, context.Player.maxRunSpeed);
		Assert.AreEqual(6f, context.Player.accRunSpeed);
		Assert.AreEqual(0.08f, context.Player.runAcceleration);
	}

	[TestMethod]
	public void FoodBonusesAndSlowAddInEveryOrder()
	{
		ModBuff[] effects = [new FruitSaladBuff(), new SalmonWarshipBuff(), new EscargotBuff()];
		var food = context.Player.GetModPlayer<FoodBuffModPlayer>();
		float? combined = null;
		foreach (int[] order in new[] { new[] { 0, 1, 2 }, new[] { 0, 2, 1 }, new[] { 1, 0, 2 }, new[] { 1, 2, 0 }, new[] { 2, 0, 1 }, new[] { 2, 1, 0 } })
		{
			food.ResetEffects();
			bonuses.ResetEffects();
			context.Player.moveSpeed = 1.4f;
			int index = 0;
			foreach (int effect in order)
			{
				effects[effect].Update(context.Player, ref index);
			}

			combined ??= context.Player.moveSpeed;
			Assert.AreEqual(combined.Value, context.Player.moveSpeed, 0.000001f);
			bonuses.PostUpdateEquips();
			bonuses.PostUpdateRunSpeeds();
			Assert.AreEqual(combined.Value, context.Player.moveSpeed, 0.000001f);
		}

		Assert.AreNotEqual(1.4f, combined!.Value);
	}

	[TestMethod]
	public void WaterEquipmentAddsToMoveSpeedWithoutMultiplyingOtherEquipment()
	{
		var kelp = context.Attach<KelpCurtainPlayer>();
		context.Player.wet = true;
		kelp.MolluscsSetBuff = true;
		kelp.MolluscsLeggings = true;
		kelp.CorrodedPearl = true;
		kelp.CrimsonMoonAlgaeGreaves = true;
		kelp.UpdateEquips();
		Assert.IsTrue(context.Player.moveSpeed > 1.4f);
		Assert.AreEqual(3f, context.Player.maxRunSpeed);
		Assert.AreEqual(0.08f, context.Player.runAcceleration);
		kelp.ResetEffects();
		context.Player.moveSpeed = 1.4f;
		kelp.UpdateEquips();
		Assert.AreEqual(1.4f, context.Player.moveSpeed);
	}

	[TestMethod]
	public void AccessoryPenaltiesAreCombinedOnceAndLeaveBootSpeedUntouched()
	{
		context.Attach<HardenedWitherbarkHeart.HardenedWitherbarkHeartPlayer>();
		ModItem[] effects = [new HardenedWitherbarkHeart(), new ThornTurtleShell()];
		float? penalized = null;
		foreach (bool reverse in new[] { false, true })
		{
			bonuses.ResetEffects();
			context.Player.moveSpeed = 1.4f;
			Array.Clear(context.Player.buffType);
			Array.Clear(context.Player.buffTime);
			foreach (var effect in reverse ? effects.Reverse() : effects)
			{
				effect.UpdateAccessory(context.Player, false);
			}

			penalized ??= context.Player.moveSpeed;
			Assert.AreEqual(penalized.Value, context.Player.moveSpeed, 0.000001f);
			bonuses.PostUpdateEquips();
			bonuses.PostUpdateRunSpeeds();
			Assert.AreEqual(penalized.Value, context.Player.moveSpeed, 0.000001f);
			Assert.AreEqual(6f, context.Player.accRunSpeed);
		}

		Assert.IsTrue(penalized!.Value < 1.4f);
	}

	[TestMethod]
	public void FireflyBadgeAddsToExistingMoveSpeedOnce()
	{
		var badge = LoadAccessory("Myth", "Everglow.Myth.TheFirefly.Items.Accessories.FireflyBorageBadge");
		badge.UpdateAccessory(context.Player, false);
		float boosted = context.Player.moveSpeed;
		Assert.IsTrue(boosted > 1.4f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(boosted, context.Player.moveSpeed, 0.000001f);
	}

	[TestMethod]
	public void WhiteCrownUsesFinalCapacityBeforeVanillaMovementAndClearsWhenUnequipped()
	{
		var crown = LoadAccessory("PlantAndFarm", "Everglow.PlantAndFarm.Items.Accessories.WhiteCrown");
		float? boosted = null;
		foreach (bool reverse in new[] { false, true })
		{
			bonuses.ResetEffects();
			context.Player.statLifeMax2 = 200;
			context.Player.moveSpeed = 1.4f;
			Action equip = () => crown.UpdateAccessory(context.Player, false);
			Action capacity = () => context.Player.statLifeMax2 += 100;
			(reverse ? capacity : equip)();
			(reverse ? equip : capacity)();
			context.Player.statLifeMax2 -= 30;
			Assert.AreEqual(1.4f, context.Player.moveSpeed);
			bonuses.PostUpdateEquips();
			bonuses.PostUpdateMiscEffects();
			Assert.AreEqual(270, context.Player.statLifeMax2);
			boosted ??= context.Player.moveSpeed;
			Assert.AreEqual(boosted.Value, context.Player.moveSpeed, 0.000001f);
			Assert.AreEqual(3f, context.Player.maxRunSpeed);
			bonuses.ResetEffects();
			context.Player.moveSpeed = 1.4f;
			bonuses.PostUpdateMiscEffects();
			Assert.AreEqual(1.4f, context.Player.moveSpeed);
		}

		Assert.AreNotEqual(1.4f, boosted!.Value);
	}

	private static ModItem LoadAccessory(string module, string typeName)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(
			AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", module, "bin", configuration, "net8.0", $"Everglow.{module}.dll"));
		return (ModItem)Activator.CreateInstance(Assembly.LoadFrom(path).GetType(typeName, true)!)!;
	}
}
