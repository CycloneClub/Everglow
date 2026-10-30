using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class PlannedItemDefaultsTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	private static Item Defaults<T>()
		where T : ModItem
	{
		var modItem = (T)RuntimeHelpers.GetUninitializedObject(typeof(T));
		var item = (Item)RuntimeHelpers.GetUninitializedObject(typeof(Item));
		typeof(ModType<Item>).GetProperty("Entity")!.SetValue(modItem, item);
		modItem.SetDefaults();
		return item;
	}

	[TestMethod]
	public void EldamberBulletHasAProjectileAndLaunchSpeed()
	{
		Assert.IsTrue(Defaults<EldamberBullet>().shoot > ProjectileID.None && Defaults<EldamberBullet>().shootSpeed > 0);
	}

	[TestMethod]
	public void EldamberArmorHasDefenseAndDimensions()
	{
		foreach (var item in new[] { Defaults<EldamberHelmet>(), Defaults<EldamberBreastplate>(), Defaults<EldamberBoots>() })
		{
			Assert.IsTrue(item.defense > 0 && item.width > 0 && item.height > 0);
		}
	}

	[TestMethod]
	public void EldamberPiecesApplyMagicBonusesWithoutRequiringAFullSet()
	{
		var player = new Player { statManaMax2 = 80, manaCost = 1f };
		float magicDamage = player.GetDamage(DamageClass.Magic).ApplyTo(100f);
		float meleeDamage = player.GetDamage(DamageClass.Melee).ApplyTo(100f);
		float magicCrit = player.GetCritChance(DamageClass.Magic);
		float rangedCrit = player.GetCritChance(DamageClass.Ranged);

		new EldamberBreastplate().UpdateEquip(player);
		Assert.AreEqual(120, player.statManaMax2, "The chest must also grant mana in mixed sets.");
		Assert.AreEqual(1f, player.manaCost, "A chest alone must not grant the set bonus.");
		new EldamberHelmet().UpdateEquip(player);
		new EldamberBoots().UpdateEquip(player);
		Assert.AreEqual(magicDamage + 8f, player.GetDamage(DamageClass.Magic).ApplyTo(100f), 0.00001f);
		Assert.AreEqual(magicCrit + 4f, player.GetCritChance(DamageClass.Magic));
		Assert.AreEqual(meleeDamage, player.GetDamage(DamageClass.Melee).ApplyTo(100f));
		Assert.AreEqual(rangedCrit, player.GetCritChance(DamageClass.Ranged));
		Assert.AreEqual(1f, player.manaCost, "Only the complete-set hook applies mana reduction.");
	}

	[TestMethod]
	public void EldamberSetRequiresBothMatchingBodyAndLegs()
	{
		var bodyInstance = typeof(ContentInstance<EldamberBreastplate>).GetProperty("Instance")!;
		var legsInstance = typeof(ContentInstance<EldamberBoots>).GetProperty("Instance")!;
		object? originalBody = bodyInstance.GetValue(null);
		object? originalLegs = legsInstance.GetValue(null);
		try
		{
			var body = new EldamberBreastplate();
			var legs = new EldamberBoots();
			typeof(ModType<Item>).GetProperty("Entity")!.SetValue(body, new Item { type = 6001 });
			typeof(ModType<Item>).GetProperty("Entity")!.SetValue(legs, new Item { type = 6002 });
			bodyInstance.SetValue(null, body);
			legsInstance.SetValue(null, legs);
			var helmet = new EldamberHelmet();
			var other = new Item { type = ItemID.WoodBreastplate };
			Assert.IsTrue(helmet.IsArmorSet(new Item(), body.Item, legs.Item));
			Assert.IsFalse(helmet.IsArmorSet(new Item(), other, legs.Item));
			Assert.IsFalse(helmet.IsArmorSet(new Item(), body.Item, other));
		}
		finally
		{
			bodyInstance.SetValue(null, originalBody);
			legsInstance.SetValue(null, originalLegs);
		}
	}

	[TestMethod]
	public void JumpPotionCanBeConsumedForAFiveMinuteBuff()
	{
		var potion = Defaults<JumpPotion>();
		Assert.AreEqual(5 * 60 * 60, potion.buffTime);
		Assert.IsTrue(potion.useTime > 0 && potion.useAnimation > 0 && potion.buffTime > 0 && potion.consumable);
	}

	[TestMethod]
	public void CraftableEquipmentHasUsableCombatAndToolDefaults()
	{
		foreach (var item in new[] { Defaults<EldamberBroadsword>(), Defaults<EldamberShortsword>(), Defaults<EldamberBow>(), Defaults<EldamberAxe>(), Defaults<EldamberHammer>(), Defaults<EldamberPickaxe>() })
		{
			Assert.IsTrue(item.damage > 0 && item.useTime > 0 && item.useAnimation > 0 && item.width > 0 && item.height > 0);
		}
		Assert.IsTrue(Defaults<EldamberAxe>().axe > 0);
		Assert.IsTrue(Defaults<EldamberHammer>().hammer > 0);
		Assert.AreEqual(75, Defaults<EldamberPickaxe>().pick);
		Assert.IsTrue(Defaults<EldamberShortsword>().shoot > ProjectileID.None && Defaults<EldamberShortsword>().noMelee);
		Assert.IsTrue(Defaults<EldamberBow>().shootSpeed > 0);
	}
}
