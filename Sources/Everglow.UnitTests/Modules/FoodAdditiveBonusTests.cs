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
	private FoodBuffModPlayer food = null!;
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
		food = context.Attach<FoodBuffModPlayer>();
	}

	[TestCleanup]
	public void Cleanup()
	{
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(SeafoodDinnerBuff), "Generic")]
	[DataRow(typeof(SugarCookieBuff), "Ranged")]
	[DataRow(typeof(PotatoChipsBuff), "Melee")]
	[DataRow(typeof(PhoBuff), "Summon")]
	[DataRow(typeof(BowlofSoupBuff), "Magic")]
	public void OrdinaryDamageAddsToExistingBonusWithoutMultiplyingOtherChannels(Type buffType, string category)
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
		Assert.AreNotEqual(native.Additive, actual.Additive);
		Assert.AreEqual(native.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(native.Base, actual.Base);
		Assert.AreEqual(native.Flat, actual.Flat);
	}

	[TestMethod]
	public void SeafoodBonusesAddAndDoNotApplyAgainAtSettlement()
	{
		(float Speed, float Additive) Measure(bool reverse)
		{
			food.ResetEffects();
			bonuses.ResetEffects();
			context.Player.GetAttackSpeed(DamageClass.Generic) = 1.3f;
			context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default + 0.5f;
			ModBuff[] buffs = [new SeafoodDinnerBuff(), new SeafoodPizzaBuff()];
			foreach (var buff in reverse ? buffs.Reverse() : buffs)
			{
				Apply(buff);
			}

			return (context.Player.GetAttackSpeed(DamageClass.Generic), context.Player.GetDamage(DamageClass.Generic).Additive);
		}

		var forward = Measure(false);
		var backward = Measure(true);
		Assert.AreEqual(forward.Speed, backward.Speed, 0.000001f);
		Assert.AreEqual(forward.Additive, backward.Additive, 0.000001f);
		Assert.IsTrue(forward.Speed > 1.3f);
		Assert.IsTrue(forward.Additive > 1.5f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(backward.Speed, context.Player.GetAttackSpeed(DamageClass.Generic), 0.000001f);
		Assert.AreEqual(backward.Additive, context.Player.GetDamage(DamageClass.Generic).Additive, 0.000001f);
	}

	[TestMethod]
	public void KiwiIceCreamAddsToExistingSummonSpeedWithoutReapplyingAtSettlement()
	{
		context.Player.GetAttackSpeed(DamageClass.Summon) = 1.2f;
		Apply(new KiwiIceCreamBuff());
		float boosted = context.Player.GetAttackSpeed(DamageClass.Summon);
		Assert.IsTrue(boosted > 1.2f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(boosted, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LauncherBuffUsesOrdinaryAdditionsOnlyWhileHoldingLauncher(bool holdingLauncher)
	{
		context.Player.HeldItem.type = holdingLauncher ? ItemUtils.Ranged.Launchers.First() : ItemID.Wood;
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1.25f;
		var native = new StatModifier(1.4f, 1.2f, flat: 3, @base: 7);
		context.Player.GetDamage(DamageClass.Ranged) = native;
		Apply(new B_52Buff());
		var actual = context.Player.GetDamage(DamageClass.Ranged);
		if (holdingLauncher)
		{
			Assert.IsTrue(context.Player.GetAttackSpeed(DamageClass.Ranged) > 1.25f);
			Assert.IsTrue(actual.Additive > native.Additive);
		}
		else
		{
			Assert.AreEqual(1.25f, context.Player.GetAttackSpeed(DamageClass.Ranged));
			Assert.AreEqual(native.Additive, actual.Additive);
		}

		Assert.AreEqual(native.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(native.Base, actual.Base);
		Assert.AreEqual(native.Flat, actual.Flat);
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
		float ranged = context.Player.GetAttackSpeed(DamageClass.Ranged);
		if (weaponKind < 3)
		{
			Assert.IsTrue(ranged > 1.2f);
		}
		else
		{
			Assert.AreEqual(1.2f, ranged);
		}

		Assert.AreEqual(1.6f, context.Player.GetAttackSpeed(DamageClass.Melee));
	}

	[TestMethod]
	public void PrismaticPunchAddsKnockbackWithoutScalingExistingChannels()
	{
		var native = new StatModifier(1.2f, 1.4f, flat: 2, @base: 3);
		context.Player.GetKnockback(DamageClass.Summon) = native;
		Apply(new PrismaticPunchBuff());
		var actual = context.Player.GetKnockback(DamageClass.Summon);
		Assert.AreNotEqual(native.Additive, actual.Additive);
		Assert.AreEqual(native.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(native.Flat, actual.Flat);
		Assert.AreEqual(native.Base, actual.Base);
	}

	[TestMethod]
	public void AleAddsRangeAndSpeedWithoutMultiplyingExistingBonuses()
	{
		context.Player.whipRangeMultiplier = 1.3f;
		context.Player.GetAttackSpeed(DamageClass.Summon) = 1.2f;
		Apply(new AleBuff());
		float range = context.Player.whipRangeMultiplier;
		float speed = context.Player.GetAttackSpeed(DamageClass.Summon);
		Assert.IsTrue(range > 1.3f);
		Assert.IsTrue(speed > 1.2f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(range, context.Player.whipRangeMultiplier, 0.000001f);
		Assert.AreEqual(speed, context.Player.GetAttackSpeed(DamageClass.Summon), 0.000001f);
	}

	[TestMethod]
	[DataRow(typeof(WatermelonBuff), false)]
	[DataRow(typeof(LemonadeBuff), true)]
	public void OrdinaryKnockbackAddsWithoutScalingOtherModifierChannels(Type buffType, bool ranged)
	{
		var category = ranged ? DamageClass.Ranged : DamageClass.Generic;
		var original = new StatModifier(1.5f, 1.2f, flat: 3, @base: 7);
		context.Player.GetKnockback(category) = original;
		Apply((ModBuff)Activator.CreateInstance(buffType)!);
		var actual = context.Player.GetKnockback(category);
		Assert.AreNotEqual(original.Additive, actual.Additive);
		Assert.AreEqual(original.Multiplicative, actual.Multiplicative);
		Assert.AreEqual(original.Base, actual.Base);
		Assert.AreEqual(original.Flat, actual.Flat);
	}

	[TestMethod]
	public void CoffeeAddsTheSameBuildingSpeedToTileAndWall()
	{
		int originalNetMode = Main.netMode;
		try
		{
			Main.netMode = NetmodeID.Server; // Keep the light-emission call out of the renderer.
			const float Tile = 1.25f;
			const float Wall = 1.5f;
			context.Player.tileSpeed = Tile;
			context.Player.wallSpeed = Wall;
			Apply(new CoffeeCupBuff());
			float tileBonus = context.Player.tileSpeed - Tile;
			Assert.IsTrue(tileBonus > 0f);
			Assert.AreEqual(tileBonus, context.Player.wallSpeed - Wall, 0.000001f);
		}
		finally
		{
			Main.netMode = originalNetMode;
		}
	}

	[TestMethod]
	public void KiwiSizeBonusesAddToExistingScaleAndDoNotCompoundEachOther()
	{
		var sword = new Item { type = ItemID.WoodenSword, stack = 1, damage = 10 };
		float Delta(params ModBuff[] buffs)
		{
			food.ResetEffects();
			foreach (var buff in buffs)
			{
				Apply(buff);
			}

			float scale = 1.3f;
			new FoodBuffGlobalItem().ModifyItemScale(sword, context.Player, ref scale);
			return scale - 1.3f;
		}

		float juice = Delta(new KiwiJuiceBuff());
		float fruit = Delta(new KiwiFruitBuff());
		float iceCream = Delta(new KiwiIceCreamBuff());
		float combined = Delta(new KiwiJuiceBuff(), new KiwiFruitBuff(), new KiwiIceCreamBuff());
		Assert.IsTrue(juice > 0f && fruit > 0f && iceCream > 0f);
		Assert.AreEqual(juice + fruit + iceCream, combined, 0.000001f);
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
		if (active && launcher)
		{
			Assert.IsTrue(damage.Additive > original.Additive);
			Assert.AreEqual(damage.Additive, knockback.Additive, 0.000001f);
		}
		else
		{
			Assert.AreEqual(original.Additive, damage.Additive);
			Assert.AreEqual(original.Additive, knockback.Additive);
		}

		foreach (var actual in new[] { damage, knockback })
		{
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
