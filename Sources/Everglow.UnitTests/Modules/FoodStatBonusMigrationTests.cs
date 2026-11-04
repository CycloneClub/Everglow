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

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
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
		context.Dispose();
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

	private void Apply(IEnumerable<ModBuff> buffs)
	{
		int index = 0;
		foreach (var buff in buffs)
		{
			buff.Update(context.Player, ref index);
		}
	}
}
