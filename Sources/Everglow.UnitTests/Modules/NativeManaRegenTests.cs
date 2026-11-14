using Everglow.Commons.Mechanics.PlayerBonuses;
using System.Reflection;
using Everglow.Food.Buffs;
using Everglow.Food.Buffs.ModFoodBuffs;
using Everglow.Food.Buffs.VanillaDrinkBuffs;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class NativeManaRegenTests
{
	private CooldownTestContext context = null!;
	private float addCritDamage;
	private StatBonusPlayer bonuses = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		addCritDamage = FoodBuffModPlayer.AddCritDamage;
	}

	[TestCleanup]
	public void Cleanup()
	{
		FoodBuffModPlayer.AddCritDamage = addCritDamage;
		context.Dispose();
	}

	[TestMethod]
	[DataRow(typeof(ApricotBuff))]
	[DataRow(typeof(TeacupBuff))]
	[DataRow(typeof(SeafoodPizzaBuff))]
	public void FoodAddsToNativeRegenInput(Type buffType)
	{
		context.Player.manaRegenBonus = 7;
		int index = 0;
		((ModBuff)Activator.CreateInstance(buffType)!).Update(context.Player, ref index);
		Assert.IsTrue(context.Player.manaRegenBonus > 7);
	}

	[TestMethod]
	[DataRow(0, 0f)]
	[DataRow(200, 5f)]
	[DataRow(1000, 100f)]
	public void DevilHeartStopsNativeRegenerationDespiteLaterBonuses(int bonus, float delayBonus)
	{
		var player = context.Player;
		player.statManaMax2 = 300;
		player.statMana = 100;
		player.manaRegenCount = 0;
		bonuses.DevilHeartSet = true;
		player.manaRegenBonus = bonus;
		player.manaRegenDelayBonus = delayBonus;
		player.manaRegenBuff = true;
		player.usedArcaneCrystal = true;
		bonuses.PostUpdateMiscEffects();
		UpdateManaRegen(player);
		Assert.AreEqual(0, player.manaRegen);
		Assert.AreEqual(100, player.statMana);
	}

	[TestMethod]
	public void FoodBonusSurvivesNativeRegenerationRecalculation()
	{
		var player = context.Player;
		player.statManaMax2 = 300;
		player.statMana = 100;
		player.manaRegenBuff = true;
		player.manaRegenBonus = 7;
		player.manaRegenCount = 0;
		UpdateManaRegen(player);
		int withoutFood = player.manaRegen;
		player.statMana = 100;
		player.manaRegen = 999;
		player.manaRegenCount = 0;
		int index = 0;
		new ApricotBuff().Update(player, ref index);
		UpdateManaRegen(player);
		Assert.AreNotEqual(999, player.manaRegen);
		Assert.IsTrue(player.manaRegen > withoutFood);
	}

	[TestMethod]
	[DataRow(60, false)]
	[DataRow(61, true)]
	[DataRow(139, true)]
	[DataRow(140, false)]
	public void PendantUsesFinalCapacityAndStrictInterval(int mana, bool inside)
	{
		var player = context.Player;
		player.statManaMax = player.statManaMax2 = 100;
		player.statMana = mana;
		player.manaRegenBonus = 7;
		new CelesteStoneWaistPendant().UpdateAccessory(player, false);
		Assert.IsTrue(player.statManaMax2 > 100);
		player.statManaMax2 = 200;
		bonuses.PostUpdateMiscEffects();
		if (inside)
		{
			Assert.IsTrue(player.manaRegenBonus > 7);
		}
		else
		{
			Assert.AreEqual(7, player.manaRegenBonus);
		}
	}

	[TestMethod]
	public void PendantStacksWithCapacityAndRegenEquipmentInEitherOrder()
	{
		(int Capacity, int EquippedRegen, int SettledRegen) Measure(bool reverse)
		{
			bonuses.ResetEffects();
			var player = context.Player;
			player.statManaMax = player.statManaMax2 = 100;
			player.statMana = 90;
			player.manaRegenBonus = 2;
			Action[] equip =
			[
				() => new CelesteStoneWaistPendant().UpdateAccessory(player, false),
				() => new EldamberHairpin().UpdateEquip(player),
				() => new ManaThermalPipeline().UpdateAccessory(player, false),
			];
			foreach (var action in reverse ? equip.Reverse() : equip)
			{
				action();
			}

			int capacity = player.statManaMax2;
			int equippedRegen = player.manaRegenBonus;
			bonuses.PostUpdateMiscEffects();
			return (capacity, equippedRegen, player.manaRegenBonus);
		}

		var forward = Measure(false);
		var backward = Measure(true);
		Assert.AreEqual(forward, backward);
		Assert.IsTrue(forward.Capacity > 100);
		Assert.IsTrue(forward.EquippedRegen > 2);
		Assert.AreNotEqual(forward.EquippedRegen, forward.SettledRegen);
	}

	[TestMethod]
	public void SpecificEffectsClearOnReset()
	{
		bonuses.CelesteStoneWaistPendant = bonuses.DevilHeartSet = true;
		bonuses.ResetEffects();
		context.Player.statMana = 100;
		context.Player.statManaMax2 = 200;
		context.Player.manaRegenBonus = 7;
		context.Player.manaRegenDelay = 0;
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(7, context.Player.manaRegenBonus);
		Assert.AreEqual(0f, context.Player.manaRegenDelay);
	}

	[TestMethod]
	public void OverloadPreservesLongerDelayAndDoesNotLeavePermanentSuppression()
	{
		var player = context.Player;
		player.manaRegenDelay = 60;
		bonuses.DevilHeartSet = true;
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(60f, player.manaRegenDelay);
		player.manaRegenDelay = 0;
		bonuses.ResetEffects();
		bonuses.DevilHeartSet = true;
		bonuses.PostUpdateMiscEffects();
		bonuses.ResetEffects();
		player.statManaMax2 = 300;
		player.statMana = 100;
		for (int i = 0; i < 3; i++)
		{
			bonuses.ResetEffects();
			bonuses.PostUpdateMiscEffects();
			UpdateManaRegen(player);
		}
		Assert.IsTrue(player.manaRegen > 0);
	}

	private static void UpdateManaRegen(Player player)
	{
		typeof(Player).GetMethod("UpdateManaRegen", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)!.Invoke(player, null);
	}
}
