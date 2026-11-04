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
		context.Attach<TropicalSmoothieBuff.TropicalSmoothiePlayer>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
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
