using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Commons.Utilities;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.ModDrinkBuffs;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class FoodAdditiveBonusTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private float criticalDamage;
	private float addCritDamage;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		criticalDamage = FoodBuffModPlayer.CriticalDamage;
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		context.Attach<FoodBuffModPlayer>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(SeafoodDinnerBuff), "Generic", 0.06f)]
	[DataRow(typeof(SeafoodPizzaBuff), "Generic", 0.08f)]
	[DataRow(typeof(FruitJuiceBuff), "Generic", 0.2f)]
	[DataRow(typeof(FruitSaladBuff), "Generic", 0.03f)]
	[DataRow(typeof(GoldenDelightBuff), "Generic", 0.04f)]
	[DataRow(typeof(FroggleBunwichBuff), "Generic", 0.04f)]
	[DataRow(typeof(SweetshrimpSushiBuff), "Generic", 0.04f)]
	[DataRow(typeof(DryMartiniBuff), "Generic", 1f)]
	[DataRow(typeof(BananaDaiquiriBuff), "Ranged", 0.5f)]
	[DataRow(typeof(SugarCookieBuff), "Ranged", 0.05f)]
	[DataRow(typeof(CreamSodaBuff), "Melee", 0.4f)]
	[DataRow(typeof(SakeBuff), "Melee", 0.4f)]
	[DataRow(typeof(PotatoChipsBuff), "Melee", 0.04f)]
	[DataRow(typeof(HotdogBuff), "Melee", 0.08f)]
	[DataRow(typeof(PhoBuff), "Summon", 0.08f)]
	[DataRow(typeof(PrismaticPunchBuff), "Summon", 0.5f)]
	[DataRow(typeof(BowlofSoupBuff), "Magic", 0.05f)]
	[DataRow(typeof(TeacupBuff), "Magic", 0.6f)]
	[DataRow(typeof(TropicalSmoothieBuff), "Magic", 0.5f)]
	public void OrdinaryDamageAddsToExistingBonusWithoutMultiplyingOtherChannels(Type buffType, string category, float bonus)
	{
		DamageClass damageClass = category switch
		{
			"Melee" => DamageClass.Melee,
			"Ranged" => DamageClass.Ranged,
			"Summon" => DamageClass.Summon,
			"Magic" => DamageClass.Magic,
			_ => DamageClass.Generic,
		};
		var native = new StatModifier(1.5f, 1.2f, flat: 3, @base: 7);
		context.Player.GetDamage(damageClass) = native;
		Apply((ModBuff)Activator.CreateInstance(buffType)!);
		var actual = context.Player.GetDamage(damageClass);
		Assert.AreEqual(1.5f + bonus, actual.Additive, 0.000001f);
		Assert.AreEqual(native.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(native.Base, actual.Base);
		Assert.AreEqual(native.Flat, actual.Flat);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SeafoodBonusesAddAndDoNotApplyAgainAtSettlement(bool reverse)
	{
		context.Player.GetAttackSpeed(DamageClass.Generic) = 1.3f;
		context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.5f;
		ModBuff[] buffs = [new SeafoodDinnerBuff(), new SeafoodPizzaBuff()];
		foreach (var buff in reverse ? buffs.Reverse() : buffs)
		{
			Apply(buff);
		}
		Assert.AreEqual(1.44f, context.Player.GetAttackSpeed(DamageClass.Generic), 0.000001f);
		Assert.AreEqual(1.64f, context.Player.GetDamage(DamageClass.Generic).Additive, 0.000001f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.44f, context.Player.GetAttackSpeed(DamageClass.Generic), 0.000001f);
	}

	[TestMethod]
	public void KiwiIceCreamAddsFivePercentToExistingSummonSpeed()
	{
		context.Player.GetAttackSpeed(DamageClass.Summon) = 1.2f;
		Apply(new KiwiIceCreamBuff());
		Assert.AreEqual(1.25f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.25f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LauncherBuffUsesOrdinaryAdditionsOnlyWhileHoldingLauncher(bool holdingLauncher)
	{
		context.Player.HeldItem.type = holdingLauncher ? ItemUtils.Ranged.Launchers.First() : ItemID.Wood;
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1.25f;
		context.Player.GetDamage(DamageClass.Ranged) = new StatModifier(1.4f, 1.2f, flat: 3, @base: 7);
		Apply(new B_52Buff());
		Assert.AreEqual(holdingLauncher ? 2.25f : 1.25f, context.Player.GetAttackSpeed(DamageClass.Ranged), 0.000001f);
		Assert.AreEqual(holdingLauncher ? 1.9f : 1.4f, context.Player.GetDamage(DamageClass.Ranged).Additive, 0.000001f);
		Assert.AreEqual(1.2f, context.Player.GetDamage(DamageClass.Ranged).Multiplicative);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(1)]
	[DataRow(2)]
	[DataRow(3)]
	public void MangosteenKeepsItsExistingWeaponFilterAndDamageClassButAddsSpeed(int weaponKind)
	{
		context.Player.HeldItem.type = weaponKind switch
		{
			0 => ItemUtils.Melee.Boomerangs.First(),
			1 => ItemUtils.Melee.Flails.First(),
			2 => ItemUtils.Melee.Yoyos.First(),
			_ => ItemID.Wood,
		};
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1.2f;
		context.Player.GetAttackSpeed(DamageClass.Melee) = 1.6f;
		Apply(new MangosteenBuff());
		Assert.AreEqual(weaponKind < 3 ? 1.7f : 1.2f, context.Player.GetAttackSpeed(DamageClass.Ranged), 0.000001f);
		Assert.AreEqual(1.6f, context.Player.GetAttackSpeed(DamageClass.Melee));
	}

	[TestMethod]
	public void PrismaticPunchAddsKnockbackWithoutScalingExistingChannels()
	{
		context.Player.GetKnockback(DamageClass.Summon) = new StatModifier(1.2f, 1.4f, flat: 2, @base: 3);
		Apply(new PrismaticPunchBuff());
		var actual = context.Player.GetKnockback(DamageClass.Summon);
		Assert.AreEqual(1.7f, actual.Additive, 0.000001f);
		Assert.AreEqual(1.4f, actual.Multiplicative);
		Assert.AreEqual(2f, actual.Flat);
		Assert.AreEqual(3f, actual.Base);
	}

	[TestMethod]
	public void AleAddsRangeAndSpeedWithoutMultiplyingExistingBonuses()
	{
		context.Player.whipRangeMultiplier = 1.3f;
		context.Player.GetAttackSpeed(DamageClass.Summon) = 1.2f;
		Apply(new AleBuff());
		Assert.AreEqual(1.8f, context.Player.whipRangeMultiplier, 0.000001f);
		Assert.AreEqual(1.7f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.8f, context.Player.whipRangeMultiplier, 0.000001f);
		Assert.AreEqual(1.7f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
	}

	[TestMethod]
	[DataRow(typeof(WatermelonBuff), false, 0.08f)]
	[DataRow(typeof(WaterMelonJuiceBuff), false, 1f)]
	[DataRow(typeof(LemonadeBuff), true, 1f)]
	public void OrdinaryKnockbackAddsWithoutScalingOtherModifierChannels(Type buffType, bool ranged, float bonus)
	{
		var category = ranged ? DamageClass.Ranged : DamageClass.Generic;
		var original = new StatModifier(1.5f, 1.2f, flat: 3, @base: 7);
		context.Player.GetKnockback(category) = original;
		Apply((ModBuff)Activator.CreateInstance(buffType)!);
		var actual = context.Player.GetKnockback(category);
		Assert.AreEqual(1.5f + bonus, actual.Additive, 0.000001f);
		Assert.AreEqual(original.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(original.Base, actual.Base);
		Assert.AreEqual(original.Flat, actual.Flat);
	}

	[TestMethod]
	public void CoffeeAddsBuildingSpeedToExistingBonuses()
	{
		int originalNetMode = Main.netMode;
		try
		{
			Main.netMode = NetmodeID.Server; // Keep the light-emission call out of the renderer.
			context.Player.tileSpeed = 1.25f;
			context.Player.wallSpeed = 1.5f;
			Apply(new CoffeeCupBuff());
			Assert.AreEqual(10.25f, context.Player.tileSpeed);
			Assert.AreEqual(10.5f, context.Player.wallSpeed);
		}
		finally
		{
			Main.netMode = originalNetMode;
		}
	}

	[TestMethod]
	public void KiwiSizeBonusesAddToExistingScaleAndDoNotCompoundEachOther()
	{
		Apply(new KiwiJuiceBuff());
		Apply(new KiwiFruitBuff());
		Apply(new KiwiIceCreamBuff());
		var sword = new Item { type = ItemID.WoodenSword, stack = 1, damage = 10 };
		float scale = 1.3f;
		new FoodBuffGlobalItem().ModifyItemScale(sword, context.Player, ref scale);
		Assert.AreEqual(3.1f, scale, 0.000001f);
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void NachosAddsLauncherDamageAndKnockbackUsingTheQueriedWeapon(bool active, bool launcher)
	{
		context.Player.HeldItem.type = ItemID.Wood;
		if (active)
		{
			Apply(new NachosBuff());
		}
		var item = new Item { type = launcher ? ItemUtils.Ranged.Launchers.First() : ItemID.WoodenSword, stack = 1 };
		var original = new StatModifier(1.5f, 1.2f, flat: 3, @base: 7);
		var damage = original;
		var knockback = original;
		var global = new FoodBuffGlobalItem();
		global.ModifyWeaponDamage(item, context.Player, ref damage);
		global.ModifyWeaponKnockback(item, context.Player, ref knockback);
		float expected = 1.5f + (active && launcher ? 0.04f : 0);
		foreach (var actual in new[] { damage, knockback })
		{
			Assert.AreEqual(expected, actual.Additive, 0.000001f);
			Assert.AreEqual(original.Multiplicative, actual.Multiplicative);
			Assert.AreEqual(original.Base, actual.Base);
			Assert.AreEqual(original.Flat, actual.Flat);
		}
	}

	private void Apply(ModBuff buff)
	{
		int index = 0;
		buff.Update(context.Player, ref index);
	}
}
