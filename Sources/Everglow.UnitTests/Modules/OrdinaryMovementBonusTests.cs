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

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
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
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(BurgerBuff), -0.4f)]
	[DataRow(typeof(HotdogBuff), -0.2f)]
	[DataRow(typeof(EscargotBuff), -0.75f)]
	[DataRow(typeof(MilkshakeBuff), 4f)]
	[DataRow(typeof(FruitJuiceBuff), 1f)]
	[DataRow(typeof(FruitSaladBuff), 0.2f)]
	[DataRow(typeof(GoldenDelightBuff), 0.2f)]
	[DataRow(typeof(SashimiBuff), 0.2f)]
	[DataRow(typeof(SalmonWarshipBuff), 0.1f)]
	[DataRow(typeof(SalmonInPepperBuff), 0.07f)]
	[DataRow(typeof(JellyBallStick), -0.5f)]
	public void OrdinaryEffectsAddOnceToExistingMoveSpeed(Type effectType, float bonus)
	{
		var buff = (ModBuff)Activator.CreateInstance(effectType)!;
		int index = 0;
		buff.Update(context.Player, ref index);
		Assert.AreEqual(1.4f + bonus, context.Player.moveSpeed, 0.000001f);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(1.4f + bonus, context.Player.moveSpeed, 0.000001f);
		Assert.AreEqual(3f, context.Player.maxRunSpeed);
		Assert.AreEqual(6f, context.Player.accRunSpeed);
		Assert.AreEqual(0.08f, context.Player.runAcceleration);
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void FoodBonusesAndSlowAddInEveryOrder(int first, int second, int third)
	{
		ModBuff[] effects = [new FruitSaladBuff(), new SalmonWarshipBuff(), new EscargotBuff()];
		int index = 0;
		effects[first].Update(context.Player, ref index);
		effects[second].Update(context.Player, ref index);
		effects[third].Update(context.Player, ref index);
		Assert.AreEqual(0.95f, context.Player.moveSpeed, 0.000001f);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(0.95f, context.Player.moveSpeed, 0.000001f);
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
		Assert.AreEqual(2.49f, context.Player.moveSpeed, 0.000001f);
		Assert.AreEqual(3f, context.Player.maxRunSpeed);
		Assert.AreEqual(0.08f, context.Player.runAcceleration);
		kelp.ResetEffects();
		context.Player.moveSpeed = 1.4f;
		kelp.UpdateEquips();
		Assert.AreEqual(1.4f, context.Player.moveSpeed);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void AccessoryPenaltiesAreCombinedOnceAndLeaveBootSpeedUntouched(bool reverse)
	{
		context.Attach<HardenedWitherbarkHeart.HardenedWitherbarkHeartPlayer>();
		ModItem[] effects = [new HardenedWitherbarkHeart(), new ThornTurtleShell()];
		foreach (var effect in reverse ? effects.Reverse() : effects)
		{
			effect.UpdateAccessory(context.Player, false);
		}
		Assert.AreEqual(0.78f, context.Player.moveSpeed, 0.000001f);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(0.78f, context.Player.moveSpeed, 0.000001f);
		Assert.AreEqual(6f, context.Player.accRunSpeed);
	}

	[TestMethod]
	public void FireflyBadgeAddsTenPercentToExistingMoveSpeed()
	{
		var badge = LoadAccessory("Myth", "Everglow.Myth.TheFirefly.Items.Accessories.FireflyBorageBadge");
		badge.UpdateAccessory(context.Player, false);
		Assert.AreEqual(1.5f, context.Player.moveSpeed, 0.000001f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.5f, context.Player.moveSpeed, 0.000001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void WhiteCrownUsesFinalCapacityBeforeVanillaMovementAndClearsWhenUnequipped(bool reverse)
	{
		var crown = LoadAccessory("PlantAndFarm", "Everglow.PlantAndFarm.Items.Accessories.WhiteCrown");
		context.Player.statLifeMax2 = 200;
		Action equip = () => crown.UpdateAccessory(context.Player, false);
		Action capacity = () => context.Player.statLifeMax2 += 100;
		(reverse ? capacity : equip)();
		(reverse ? equip : capacity)();
		context.Player.statLifeMax2 -= 30;
		Assert.AreEqual(1.4f, context.Player.moveSpeed);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(270, context.Player.statLifeMax2);
		Assert.AreEqual(1.9f, context.Player.moveSpeed, 0.000001f); // Preserve the existing tenth-step formula.
		Assert.AreEqual(3f, context.Player.maxRunSpeed);
		bonuses.ResetEffects();
		context.Player.moveSpeed = 1.4f;
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(1.4f, context.Player.moveSpeed);
	}

	private static ModItem LoadAccessory(string module, string typeName)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", module, "bin", configuration, "net8.0", $"Everglow.{module}.dll"));
		return (ModItem)Activator.CreateInstance(Assembly.LoadFrom(path).GetType(typeName, true)!)!;
	}
}
