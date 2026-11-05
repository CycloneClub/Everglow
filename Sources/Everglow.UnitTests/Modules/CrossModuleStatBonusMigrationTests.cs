using System.Reflection;
using Everglow.Commons.Mechanics;
using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.Food.Buffs.VanillaFoodBuffs;
using Everglow.UnitTests.Function;
using Everglow.Yggdrasil.YggdrasilTown.Items.Accessories;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules;

[TestClass]
[DoNotParallelize]
public class CrossModuleStatBonusMigrationTests
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
		context.Player.GetDamage(DamageClass.Generic) = StatModifier.Default;
	}

	[TestCleanup]
	public void Cleanup() => context.Dispose();

	[TestMethod]
	public void RockDistanceAccumulatesAfterArmorSetFlagsAndFeedsNextSettlement()
	{
		var rock = context.Attach<Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Rock.RockArmorSetPlayer>();
		rock.ResetEffects();
		rock.UpdateEquips();
		// The engine enables armor-set flags after ModPlayer.UpdateEquips.
		rock.RockArmorSetEnable = true;
		// Simulate RockHelmet.UpdateArmorSet's native defense bonus without loading its localized set text.
		context.Player.statDefense += rock.DefenseBonus;
		bonuses.PostUpdateEquips();
		context.Player.oldPosition = Microsoft.Xna.Framework.Vector2.Zero;
		context.Player.position = new Microsoft.Xna.Framework.Vector2(240, 0);
		rock.PostUpdate();
		Assert.AreEqual(1, rock.DefenseBonus);
		bonuses.ResetEffects();
		rock.ResetEffects();
		rock.UpdateEquips();
		rock.RockArmorSetEnable = true;
		// Simulate RockHelmet.UpdateArmorSet's native defense bonus without loading its localized set text.
		context.Player.statDefense = new Player.DefenseStat();
		context.Player.statDefense += rock.DefenseBonus;
		Assert.AreEqual(1, (int)context.Player.statDefense);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1, (int)context.Player.statDefense);
	}

	[TestMethod]
	public void AuburnLegacySummonerSpeedStillTargetsWhipAttackSpeed()
	{
		new Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Auburn.AuburnBreastplate().UpdateEquip(context.Player);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.06f, context.Player.GetAttackSpeed(DamageClass.SummonMeleeSpeed), 0.00001f);
		Assert.AreEqual(1f, context.Player.GetAttackSpeed(DamageClass.Summon), 0.00001f);
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FoodCapacityAndThreeAccessoriesUseFinalCapacityAndDefense(bool reverse)
	{
		context.Player.statLifeMax2 = 200;
		context.Player.statLife = 200;
		context.Player.statDefense += 10;
		int index = 0;
		var crown = LoadPlantAccessory("OrangeCrown");
		var flower = LoadPlantAccessory("YellowBellFlower");
		Action[] updates =
		[
			() => new BBQRibsBuff().Update(context.Player, ref index),
			() => new FragmentsOfWreckage().UpdateAccessory(context.Player, false),
			() => crown.UpdateAccessory(context.Player, false),
			() => flower.UpdateAccessory(context.Player, false),
		];
		foreach (var update in reverse ? updates.Reverse() : updates)
		{
			update();
		}
		bonuses.PostUpdateEquips();
		Assert.AreEqual(250, context.Player.statLifeMax2);
		Assert.AreEqual(20, (int)context.Player.statDefense);
		Assert.AreEqual(106.5f, context.Player.GetDamage(DamageClass.Generic).ApplyTo(100), 0.0001f);
	}

	[TestMethod]
	public void NativeModifierChannelsComposeWithWreckage()
	{
		ref var modifier = ref context.Player.GetDamage(DamageClass.Generic);
		modifier += 0.2f;
		modifier *= 1.5f;
		modifier.Base += 3;
		modifier.Flat += 2;
		context.Player.statLifeMax2 = 200;
		context.Player.statLife = 80;
		new FragmentsOfWreckage().UpdateAccessory(context.Player, false);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(1.26f, modifier.Additive, 0.00001f);
		Assert.AreEqual(1.5f, modifier.Multiplicative, 0.00001f);
		Assert.AreEqual(26.57f, modifier.ApplyTo(10), 0.0001f);
	}

	[TestMethod]
	[DataRow(0, 1, 2)]
	[DataRow(0, 2, 1)]
	[DataRow(1, 0, 2)]
	[DataRow(1, 2, 0)]
	[DataRow(2, 0, 1)]
	[DataRow(2, 1, 0)]
	public void NativeFoodCapacityAndFixedRingCostFeedWreckage(int first, int second, int third)
	{
		var player = context.Player;
		player.statLifeMax2 = 200;
		player.statLife = 130;
		player.statDefense += 10;
		player.pickSpeed = 1f;
		player.inventory[0] = new Item { type = Terraria.ID.ItemID.GoldCoin, stack = 200 };
		context.Attach<RingOfMatterPlayer>().ResetEffects();
		var ring = new RingOfMatter();
		int buffIndex = 0;
		Action[] updates =
		[
			() => new BBQRibsBuff().Update(player, ref buffIndex),
			() => ring.UpdateAccessory(player, false),
			() => new FragmentsOfWreckage().UpdateAccessory(player, false),
		];
		updates[first]();
		updates[second]();
		updates[third]();
		Assert.AreEqual(226, player.statLifeMax2);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(226, player.statLifeMax2);
		Assert.AreEqual(112.8f, player.GetDamage(DamageClass.Generic).ApplyTo(100), 0.0001f);
		Assert.AreEqual(17, (int)player.statDefense);
		Assert.AreEqual(0.92f, player.pickSpeed, 0.00001f);
	}


	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	[DataRow(true, true)]
	public void BadgeAddsMeleeSpeedAndWhipsInheritItOnce(bool mothActive, bool reverse)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Myth", "bin", configuration, "net8.0", "Everglow.Myth.dll"));
		var assembly = Assembly.LoadFrom(path);
		var moth = assembly.GetType("Everglow.Myth.TheFirefly.NPCs.Bosses.CorruptMoth")!
			.GetField("CorruptMothNPC")!;
		var previousMoth = moth.GetValue(null);
		try
		{
			moth.SetValue(null, new NPC { active = mothActive });
			var item = (ModItem)Activator.CreateInstance(assembly.GetType("Everglow.Myth.TheFirefly.Items.Accessories.FireflyBorageBadge")!)!;
			var player = context.Player;
			player.GetCritChance(DamageClass.Generic) = 5;
			player.GetCritChance(DamageClass.Melee) = 4;
			player.GetCritChance(DamageClass.MeleeNoSpeed) = 2;
			player.GetAttackSpeed(DamageClass.Melee) = 1f;
			player.GetAttackSpeed(DamageClass.SummonMeleeSpeed) = 1.3f;
			player.GetAttackSpeed(DamageClass.Summon) = 1f;
			player.GetAttackSpeed(DamageClass.Generic) = 1.1f;
			Action equip = () => item.UpdateAccessory(player, false);
			Action otherBonus = () =>
			{
				player.GetAttackSpeed(DamageClass.Melee) += 0.2f;
				player.GetCritChance(DamageClass.Melee) += 15;
			};
			(reverse ? otherBonus : equip)();
			(reverse ? equip : otherBonus)();
			bonuses.PostUpdateEquips();
			float badgeBonus = mothActive ? 0.08f : 0;
			float critPenalty = mothActive ? 10 : 0;
			Assert.AreEqual(24f - critPenalty, player.GetTotalCritChance(DamageClass.Melee));
			Assert.AreEqual(26f - critPenalty, player.GetTotalCritChance(DamageClass.MeleeNoSpeed));
			Assert.AreEqual(2f, player.GetCritChance(DamageClass.MeleeNoSpeed));
			Assert.AreEqual(1.2f + badgeBonus, player.GetAttackSpeed(DamageClass.Melee), 0.00001f);
			Assert.AreEqual(1.3f, player.GetAttackSpeed(DamageClass.SummonMeleeSpeed), 0.00001f);
			Assert.AreEqual(1.6f + badgeBonus, player.GetTotalAttackSpeed(DamageClass.SummonMeleeSpeed), 0.00001f);
			Assert.AreEqual(1.1f, player.GetTotalAttackSpeed(DamageClass.Summon), 0.00001f);
		}
		finally
		{
			moth.SetValue(null, previousMoth);
		}
	}

	[TestMethod]
	[DataRow(false, false)]
	[DataRow(false, true)]
	[DataRow(true, false)]
	public void PaperAddsFixedPenaltiesAndRespectsGodMode(bool godMode, bool reverse)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "Myth", "bin", configuration, "net8.0", "Everglow.Myth.dll"));
		var buff = (ModBuff)Activator.CreateInstance(Assembly.LoadFrom(path).GetType("Everglow.Myth.LanternMoon.Buffs.PaperObstructed")!)!;
		var player = context.Player;
		player.creativeGodMode = godMode;
		player.GetCritChance(DamageClass.Generic) = 4;
		player.GetCritChance(DamageClass.Melee) = 11;
		player.GetDamage(DamageClass.Generic) = new StatModifier(1.2f, 1.5f, flat: 2, @base: 3);
		int index = 0;
		Action paper = () => buff.Update(player, ref index);
		Action otherBonus = () =>
		{
			player.GetCritChance(DamageClass.Generic) += 20;
			player.GetDamage(DamageClass.Generic) += 0.3f;
		};
		(reverse ? otherBonus : paper)();
		(reverse ? paper : otherBonus)();
		bonuses.PostUpdateEquips();
		Assert.AreEqual(godMode ? 24f : 14f, player.GetCritChance(DamageClass.Generic));
		Assert.AreEqual(godMode ? 35f : 25f, player.GetTotalCritChance(DamageClass.Melee));
		var damage = player.GetDamage(DamageClass.Generic);
		Assert.AreEqual(godMode ? 1.5f : 1.4f, damage.Additive, 0.000001f);
		Assert.AreEqual(1.5f, damage.Multiplicative);
		Assert.AreEqual(3f, damage.Base);
		Assert.AreEqual(2f, damage.Flat);
		Assert.AreEqual(!godMode, player.headcovered);
		Assert.AreEqual(!godMode, player.bleed);
	}

	private static ModItem LoadPlantAccessory(string name)
	{
		string configuration = new DirectoryInfo(AppContext.BaseDirectory).Parent!.Name;
		string path = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory,
			"..", "..", "..", "..", "Modules", "PlantAndFarm", "bin", configuration, "net8.0", "Everglow.PlantAndFarm.dll"));
		var type = Assembly.LoadFrom(path).GetType("Everglow.PlantAndFarm.Items.Accessories." + name, true)!;
		return (ModItem)Activator.CreateInstance(type)!;
	}
}
