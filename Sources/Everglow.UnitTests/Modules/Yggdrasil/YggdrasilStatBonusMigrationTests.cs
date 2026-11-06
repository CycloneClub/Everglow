using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.KelpCurtain.Buffs;
using Everglow.Yggdrasil.YggdrasilTown.Buffs;
using Everglow.Yggdrasil.KelpCurtain.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories.Furnace;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilStatBonusMigrationTests
{
	private CooldownTestContext context = null!;
	private StatBonusPlayer bonuses = null!;

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		context = new CooldownTestContext();
		bonuses = context.Attach<StatBonusPlayer>();
		context.Player.statDefense = new Player.DefenseStat();
		bonuses.ResetEffects();
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void CrownCapacityFeedsWreckageMissingLifeBonusesRegardlessOfEquipOrder(bool reverse)
	{
		var player = context.Player;
		player.statLifeMax = player.statLifeMax2 = player.statLife = 100;
		player.statDefense += 10;
		player.pickSpeed = 1f;
		player.GetDamage(DamageClass.Generic) = StatModifier.Default;
		RegisterInOrder(
			reverse,
			() => new BloodTearCrystalCrown().UpdateAccessory(player, false),
			() => new FragmentsOfWreckage().UpdateAccessory(player, false));

		Assert.AreEqual(130, player.statLifeMax2, "Native capacity additions must be visible before the deferred dependency chain.");
		bonuses.PostUpdateEquips();

		Assert.AreEqual(130, player.statLifeMax2);
		Assert.AreEqual(101.5f, player.GetDamage(DamageClass.Generic).ApplyTo(100f), 0.00001f);
		Assert.AreEqual(12, (int)player.statDefense);
		Assert.AreEqual(0.975f, player.pickSpeed, 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MoltenCoreAddsToShacklesEnduranceRegardlessOfEquipOrder(bool reverse)
	{
		var player = context.Player;
		var moltenCore = bonuses;
		player.endurance = 0.2f;
		player.buffType[0] = BuffID.OnFire;
		player.buffTime[0] = 60;
		RegisterInOrder(
			reverse,
			() => new MoltenCore().UpdateAccessory(player, false),
			() => new ShacklesBall().UpdateAccessory(player, false));

		bonuses.PostUpdateEquips();

		Assert.AreEqual(0.33f, player.endurance, 0.00001f);
		var modifiers = new Player.HurtModifiers();
		modifiers.FinalDamage *= Math.Max(1 - player.endurance, 0); // The native hurt pipeline applies endurance once.
		moltenCore.ModifyHurt(ref modifiers);
		Assert.AreEqual(1f - 0.2f - 0.08f - 0.05f, modifiers.FinalDamage.ApplyTo(1), 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void HardenedHeartAddsMovementPenaltyWithoutChangingBootBaseRegardlessOfEquipOrder(bool reverse)
	{
		var player = context.Player;
		context.Attach<HardenedWitherbarkHeart.HardenedWitherbarkHeartPlayer>();
		player.accRunSpeed = 3f;
		RegisterInOrder(
			reverse,
			() => new ForsakenBoots().UpdateAccessory(player, false),
			() => new HardenedWitherbarkHeart().UpdateAccessory(player, false));

		bonuses.PostUpdateEquips();
		bonuses.PostUpdateRunSpeeds();

		Assert.AreEqual(6.25f, player.accRunSpeed);
		Assert.AreEqual(0.55f, player.moveSpeed, 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void PendantReadsCombinedManaCapacityAndStacksPipelineRegenerationRegardlessOfEquipOrder(bool reverse)
	{
		var player = context.Player;
		player.statManaMax = player.statManaMax2 = 100;
		player.statMana = 90;
		player.manaRegenBonus = 2;
		RegisterInOrder(
			reverse,
			() => new CelesteStoneWaistPendant().UpdateAccessory(player, false),
			() => new EldamberHairpin().UpdateEquip(player),
			() => new ManaThermalPipeline().UpdateAccessory(player, false));

		bonuses.PostUpdateEquips();
		Assert.AreEqual(160, player.statManaMax2);
		Assert.AreEqual(6, player.manaRegenBonus, "The pipeline contributes natively; only the capacity-dependent pendant waits.");
		bonuses.PostUpdateMiscEffects();

		Assert.AreEqual(9, player.manaRegenBonus, "Ninety mana is within the pendant interval only after all capacity bonuses settle.");
	}

	[TestMethod]
	[DataRow(2f)]
	[DataRow(10f)]
	public void BootsSetEquipmentBaseWithoutOverwritingLaterSpeedCalculation(float calculatedSpeed)
	{
		var player = context.Player;
		player.accRunSpeed = 3f;
		new ForsakenBoots().UpdateAccessory(player, false);
		Assert.AreEqual(6.25f, player.accRunSpeed);
		player.accRunSpeed = calculatedSpeed; // Stand in for a subsequent vanilla slow, terrain, or mount calculation.
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateRunSpeeds();
		Assert.AreEqual(calculatedSpeed, player.accRunSpeed);
		Assert.AreEqual(1f + ForsakenBoots.MoveSpeedBonus / 100f, player.moveSpeed, 0.00001f);
	}

	[TestMethod]
	public void OrfeoBootsKeepTheirConditionalBaseSpeedInTheEquipmentHook()
	{
		var originalNpcs = Main.npc;
		try
		{
			Main.npc = [new NPC { active = true, position = context.Player.position }];
			new OrfeoBoots().UpdateAccessory(context.Player, false);
			Assert.AreEqual(6.75f, context.Player.accRunSpeed);
			context.Player.accRunSpeed = 2f;
			bonuses.PostUpdateEquips();
			bonuses.PostUpdateRunSpeeds();
			Assert.AreEqual(2f, context.Player.accRunSpeed);
			Assert.AreEqual(1f, context.Player.jumpSpeedBoost);

			bonuses.ResetEffects();
			Main.npc[0].active = false;
			new OrfeoBoots().UpdateAccessory(context.Player, false);
			Assert.AreEqual(6f, context.Player.accRunSpeed);
		}
		finally
		{
			Main.npc = originalNpcs;
		}
	}

	[TestMethod]
	public void DevilHeartSeparatesNativeManaSuppressionFromDefenseBonuses()
	{
		var player = context.Player;
		player.manaRegenBonus = 4;
		player.manaRegen = 9;
		player.manaRegenCount = 100;
		player.maxMinions = 1;
		int index = 0;
		new DevilHeartSetBuff().Update(player, ref index);
		bonuses.PostUpdateEquips();
		bonuses.PostUpdateMiscEffects();
		Assert.AreEqual(4, player.manaRegenBonus);
		Assert.AreEqual(0, player.manaRegen);
		Assert.AreEqual(0, player.manaRegenCount);
		Assert.AreEqual(2, player.maxMinions);
	}

	[TestMethod]
	public void EmptyFowlbeastAddsNativeMinionCapacityWithoutChangingOccupancy()
	{
		context.Attach<EmptyFowlbeast.EmptyFowlbeastPlayer>();
		context.Player.slotsMinions = 2;
		context.Player.maxMinions = 4;
		new EmptyFowlbeast().UpdateAccessory(context.Player, false);
		Assert.AreEqual(5, context.Player.maxMinions);
		Assert.AreEqual(2f, context.Player.slotsMinions);
		context.Player.slotsMinions = 1;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1f, context.Player.slotsMinions);
		Assert.AreEqual(5, context.Player.maxMinions);
	}

	[TestMethod]
	public void PlagueResetsItsTimerInTheDedicatedHookAndKeepsRegenContributions()
	{
		context.Player.buffTime[0] = 60;
		int index = 0;
		new Plague().Update(context.Player, ref index);
		context.Player.lifeRegenTime = -10;
		context.Player.lifeRegen = 8;
		context.Player.GetModPlayer<StatBonusPlayer>().UpdateBadLifeRegen();
		Assert.AreEqual(-Plague.PlayerLifeDecreasePerSecond * 2, context.Player.lifeRegen);
		Assert.AreEqual(0f, context.Player.lifeRegenTime);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void MoltenCoreUsesOneCollectedConditionForAllItsBonuses(bool burning)
	{
		var moltenCore = bonuses;
		context.Player.endurance = 0.2f;
		context.Player.buffType[0] = burning ? BuffID.OnFire : 0;
		context.Player.buffTime[0] = 60;
		new MoltenCore().UpdateAccessory(context.Player, false);
		Assert.AreEqual(burning ? 5 : 0, (int)context.Player.statDefense);
		context.Player.buffType[0] = burning ? 0 : BuffID.OnFire;
		bonuses.PostUpdateEquips();
		Assert.AreEqual(burning ? 0.25f : 0.2f, context.Player.endurance);
		var modifiers = new Player.HurtModifiers();
		moltenCore.ModifyHurt(ref modifiers);
		Assert.AreEqual(1f, modifiers.FinalDamage.ApplyTo(1), "The central player must not apply a second reduction.");
	}

	private static void RegisterInOrder(bool reverse, params Action[] contributions)
	{
		foreach (var contribution in reverse ? contributions.Reverse() : contributions)
		{
			contribution();
		}
	}
}
