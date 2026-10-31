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
using ManaSpringOreTile = Everglow.Yggdrasil.YggdrasilTown.Tiles.ManaSpringOre;

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
		foreach (var item in new[] { Defaults<EldamberHairpin>(), Defaults<EldamberBreastplate>(), Defaults<EldamberBoots>() })
		{
			Assert.IsTrue(item.defense > 0 && item.width > 0 && item.height > 0);
		}
	}

	[TestMethod]
	public void EldamberSharedPiecesGiveGenericOffenseAndOnlyMageHelmetGrantsMana()
	{
		var player = new Player { statManaMax2 = 80, manaCost = 1f };
		float genericDamage = player.GetDamage(DamageClass.Generic).ApplyTo(100f);
		float genericCrit = player.GetCritChance(DamageClass.Generic);

		new EldamberBreastplate().UpdateEquip(player);
		Assert.AreEqual(80, player.statManaMax2, "The shared chest must not grant the mage helmet's mana.");
		Assert.AreEqual(genericDamage + 4f, player.GetDamage(DamageClass.Generic).ApplyTo(100f), 0.00001f);
		Assert.AreEqual(1f, player.manaCost, "A chest alone must not grant the set bonus.");
		new EldamberHairpin().UpdateEquip(player);
		new EldamberBoots().UpdateEquip(player);
		Assert.AreEqual(120, player.statManaMax2);
		Assert.AreEqual(108f, player.GetTotalDamage(DamageClass.Magic).ApplyTo(100f), 0.00001f);
		Assert.AreEqual(104f, player.GetTotalDamage(DamageClass.Melee).ApplyTo(100f), 0.00001f);
		Assert.AreEqual(genericCrit + 4f, player.GetCritChance(DamageClass.Generic));
		Assert.AreEqual(1f, player.manaCost, "Only the complete-set hook applies mana reduction.");
	}

	[TestMethod]
	[DataRow("EldamberHairpin")]
	[DataRow("EldamberWarhelm")]
	public void EldamberSetRequiresBothMatchingBodyAndLegs(string helmetName)
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
			var helmetType = typeof(EldamberHairpin).Assembly.GetType(typeof(EldamberHairpin).Namespace + "." + helmetName);
			Assert.IsNotNull(helmetType);
			var helmet = (ModItem)Activator.CreateInstance(helmetType)!;
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
	public void ManaSpringOreRequiresTheTwilightCrystalPickaxeTier()
	{
		var tile = new ManaSpringOreTile();
		bool oldServer = Main.dedServ;
		bool oldSolid = Main.tileSolid[tile.Type];
		bool oldBlockLight = Main.tileBlockLight[tile.Type];
		bool oldOre = TileID.Sets.Ore[tile.Type];
		try
		{
			Main.dedServ = true;
			tile.SetStaticDefaults();
			Assert.AreEqual(Defaults<MiningPowerPickaxe_3>().pick, tile.MinPick);
			Assert.IsTrue(Defaults<MiningPowerPickaxe_2>().pick < tile.MinPick);
			Assert.IsTrue(Defaults<EldamberPickaxe>().pick < tile.MinPick);
			Assert.IsFalse(tile.CanExplode(0, 0), "Explosives must not bypass the ore's progression gate.");
		}
		finally
		{
			Main.dedServ = oldServer;
			Main.tileSolid[tile.Type] = oldSolid;
			Main.tileBlockLight[tile.Type] = oldBlockLight;
			TileID.Sets.Ore[tile.Type] = oldOre;
		}
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
