using System.Reflection;
using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food;
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
public class FoodStatBonusMigrationTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;
	private FoodBuffModPlayer foodBuffs = null!;
	private bool dontStarveWorld;
	private bool raining;
	private float criticalDamage;
	private float addCritDamage;
	private float jumpSpeed;
	private int jumpHeight;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		jumpSpeed = Player.jumpSpeed;
		jumpHeight = Player.jumpHeight;
		dontStarveWorld = Main.dontStarveWorld;
		raining = Main.raining;
		criticalDamage = FoodBuffModPlayer.CriticalDamage;
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		foodBuffs = context.Attach<FoodBuffModPlayer>();
		bonuses.ResetEffects();
		foodBuffs.ResetEffects();
	}

	[TestCleanup]
	public void Cleanup()
	{
		Main.dontStarveWorld = dontStarveWorld;
		Main.raining = raining;
		FoodBuffModPlayer.CriticalDamage = criticalDamage;
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		Player.jumpSpeed = jumpSpeed;
		Player.jumpHeight = jumpHeight;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void EscargotScalesFinalBunnyStewJumpWithoutChangingAdditiveBonuses(bool reverse)
	{
		context.Player.jumpSpeedBoost = 2;
		ModBuff[] buffs = [new BunnyStewBuff(), new EscargotBuff()];
		Apply(reverse ? buffs.Reverse() : buffs);
		Assert.AreEqual(3.5f, context.Player.jumpSpeedBoost);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(3.5f, context.Player.jumpSpeedBoost);
		Player.jumpSpeed = 5.01f;
		Player.jumpHeight = 15;
		typeof(Player).GetMethod("UpdateJumpHeight", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!
			.Invoke(context.Player, null);
		foodBuffs.PostUpdateRunSpeeds();
		Assert.AreEqual(2.5025f, Player.jumpSpeed, 0.00001f);
		Assert.AreEqual(20, Player.jumpHeight);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void WingPercentagesCombineAdditivelyIncludingEscargot(bool reverse)
	{
		ModBuff[] buffs = [new RoastedBirdBuff(), new RoastedDuckBuff(), new EscargotBuff()];
		Apply(reverse ? buffs.Reverse() : buffs);
		foodBuffs.PostUpdateBuffs();
		context.Player.wingTimeMax = 200;
		bonuses.PostUpdateEquips();
		foodBuffs.PostUpdateEquips();
		Assert.AreEqual(90, context.Player.wingTimeMax);
	}

	[TestMethod]
	public void BananaUsesNativeBuffConditionsWithoutRestrictingAmmoSaving()
	{
		context.Player.HeldItem.type = ItemID.Shotgun;
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1f;
		context.Cooldowns.ammoCost = 1f;
		Apply([new BananaBuff()]);
		foodBuffs.PostUpdateBuffs();
		Assert.AreEqual(1.05f, context.Player.GetAttackSpeed(DamageClass.Ranged), 0.00001f);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.05f, context.Player.GetAttackSpeed(DamageClass.Ranged), 0.00001f);
		Assert.AreEqual(0.95f, context.Cooldowns.ammoCost, 0.00001f);
	}

	[TestMethod]
	public void HungerReadsFinalLifeCapacityAndFoodPoisonUsesLateMangoState()
	{
		Main.dontStarveWorld = true;
		var hunger = context.Attach<FoodModPlayer>();
		typeof(FoodModPlayer).GetProperty(nameof(FoodModPlayer.SatietyLevel))!.SetValue(hunger, -2);
		context.Player.statLifeMax2 = 400;
		context.Player.lifeRegen = 10;
		context.Player.lifeRegenTime = 80;
		Apply([new BBQRibsBuff(), new GrubSoupBuff(), new MonsterLasagnaBuff(), new BaconBuff()]);
		hunger.PostUpdateBuffs();
		foodBuffs.PostUpdateBuffs();
		Apply([new MangoBuff()]);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(12, context.Player.lifeRegen, "Food regeneration settles before negative-regeneration suppression.");
		context.Player.GetModPlayer<StatBonusPlayer>().UpdateBadLifeRegen();
		Assert.AreEqual(450, context.Player.statLifeMax2);
		Assert.AreEqual(-9, context.Player.lifeRegen, "Clear positive regeneration once, then add hunger -4 and Mango-adjusted poison -2/-3.");
		Assert.AreEqual(0f, context.Player.lifeRegenTime);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void RainConditionIsCollectedOnceAndRefreshedNextFrame(bool rainingAtCollection)
	{
		Main.raining = rainingAtCollection;
		context.Player.moveSpeed = 1.4f;
		Apply([new SalmonInPepperBuff()]);
		Main.raining = !rainingAtCollection;
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(1.47f + (rainingAtCollection ? 0.15f : 0f), context.Player.moveSpeed, 0.00001f);

		bonuses.ResetEffects();
		context.Player.moveSpeed = 1.4f;
		Apply([new SalmonInPepperBuff()]);
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(1.47f + (rainingAtCollection ? 0f : 0.15f), context.Player.moveSpeed, 0.00001f);
	}

	[TestMethod]
	public void KiwiScaleUsesActualQueriedWeaponAndLeavesToolsUnchanged()
	{
		context.Player.HeldItem.type = ItemID.Wood;
		Apply([new KiwiJuiceBuff(), new KiwiFruitBuff(), new KiwiIceCreamBuff()]);
		var sword = new Item { type = ItemID.WoodenSword, stack = 1, damage = 10 };
		float scale = 1;
		new FoodBuffGlobalItem().ModifyItemScale(sword, context.Player, ref scale);
		Assert.AreEqual(2.8f, scale, 0.00001f);
		sword.pick = 10;
		scale = 1;
		new FoodBuffGlobalItem().ModifyItemScale(sword, context.Player, ref scale);
		Assert.AreEqual(1f, scale);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void SummonFoodBonusesAddWithNativeArmorBonus(bool reverse)
	{
		context.Player.GetAttackSpeed(DamageClass.Summon) = 1f;
		ModBuff[] buffs = [new AleBuff(), new KiwiIceCreamBuff()];
		Apply(reverse ? buffs.Reverse() : buffs);
		// DevilHeartHelmet.UpdateArmorSet runs after Buff.Update and uses this generic API.
		context.Player.GetAttackSpeed<SummonDamageClass>() += 0.15f;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.15f + 0.05f + 0.5f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void LauncherEffectsShareTheCollectedWeaponCondition(bool holdingLauncher)
	{
		int launcher = Everglow.Commons.Utilities.ItemUtils.Ranged.Launchers.First();
		context.Player.HeldItem.type = holdingLauncher ? launcher : ItemID.Wood;
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1f;
		context.Player.GetCritChance(DamageClass.Ranged) = 0;
		context.Player.GetDamage(DamageClass.Ranged) = StatModifier.Default;
		Apply([new B_52Buff()]);
		Assert.AreEqual(holdingLauncher ? 25f : 0f, context.Player.GetCritChance(DamageClass.Ranged));
		Assert.AreEqual(holdingLauncher ? 150f : 100f, context.Player.GetDamage(DamageClass.Ranged).ApplyTo(100));
		context.Player.HeldItem.type = holdingLauncher ? ItemID.Wood : launcher;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(holdingLauncher ? 2f : 1f, context.Player.GetAttackSpeed(DamageClass.Ranged));

		bonuses.ResetEffects();
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1f;
		Apply([new B_52Buff()]);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(holdingLauncher ? 1f : 2f, context.Player.GetAttackSpeed(DamageClass.Ranged));
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MangosteenSelectsItsContributionWhenTheBuffUpdates(bool holdingBoomerang)
	{
		int boomerang = Everglow.Commons.Utilities.ItemUtils.Melee.Boomerangs.First();
		context.Player.HeldItem.type = holdingBoomerang ? boomerang : ItemID.Wood;
		context.Player.GetAttackSpeed(DamageClass.Ranged) = 1f;
		Apply([new MangosteenBuff()]);
		context.Player.HeldItem.type = holdingBoomerang ? ItemID.Wood : boomerang;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(holdingBoomerang ? 1.5f : 1f, context.Player.GetAttackSpeed(DamageClass.Ranged));
	}

	private void Apply(IEnumerable<ModBuff> buffs)
	{
		int index = 0;
		foreach (var buff in buffs)
		{
			buff.Update(context.Player, ref index);
		}
	}
}
