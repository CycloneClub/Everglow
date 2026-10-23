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
