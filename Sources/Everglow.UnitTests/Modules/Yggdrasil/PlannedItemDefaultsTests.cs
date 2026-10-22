using System.Reflection;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class PlannedItemDefaultsTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	private static Item Defaults<T>() where T : ModItem
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
		Assert.IsTrue(Defaults<EldamberBullet>().shoot > 0 && Defaults<EldamberBullet>().shootSpeed > 0);
	}

	[TestMethod]
	public void EldamberArmorHasDefenseAndDimensions()
	{
		foreach (var item in new[] { Defaults<EldamberHelmet>(), Defaults<EldamberBreastplate>(), Defaults<EldamberBoots>() })
		{
			Assert.IsTrue(item.defense > 0 && item.width > 0 && item.height > 0);
		}
	}
}
