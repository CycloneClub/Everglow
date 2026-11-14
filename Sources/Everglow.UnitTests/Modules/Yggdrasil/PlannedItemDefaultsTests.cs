using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.UnitTests.Function;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Ammos;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Potions;
using Everglow.Yggdrasil.YggdrasilTown.Items.Weapons.Eldamber;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Everglow.Yggdrasil.YggdrasilTown.Dusts;

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
		using var context = new CooldownTestContext();
		var bonuses = context.Attach<StatBonusPlayer>();
		bonuses.ResetEffects();
		var player = context.Player;
		player.statManaMax2 = 80;
		player.manaCost = 1f;
		var originalGenericDamage = player.GetDamage(DamageClass.Generic);
		float genericDamage = player.GetDamage(DamageClass.Generic).ApplyTo(100f);
		float genericCrit = player.GetCritChance(DamageClass.Generic);

		float magicBefore = player.GetTotalDamage(DamageClass.Magic).ApplyTo(100f);
		float meleeBefore = player.GetTotalDamage(DamageClass.Melee).ApplyTo(100f);
		new EldamberBreastplate().UpdateEquip(player);
		bonuses.PostUpdateEquips();
		Assert.AreEqual(80, player.statManaMax2, "The shared chest must not grant the mage helmet's mana.");
		Assert.IsTrue(player.GetDamage(DamageClass.Generic).ApplyTo(100f) > genericDamage);
		Assert.AreEqual(1f, player.manaCost, "A chest alone must not grant the set bonus.");
		// Start a separate complete-outfit frame from the original base attributes.
		bonuses.ResetEffects();
		player.statManaMax2 = 80;
		player.manaCost = 1f;
		player.GetDamage(DamageClass.Generic) = originalGenericDamage;
		new EldamberBreastplate().UpdateEquip(player);
		new EldamberHairpin().UpdateEquip(player);
		new EldamberBoots().UpdateEquip(player);
		bonuses.PostUpdateEquips();
		Assert.IsTrue(player.statManaMax2 > 80);
		Assert.IsTrue(player.GetTotalDamage(DamageClass.Magic).ApplyTo(100f) > magicBefore);
		Assert.IsTrue(player.GetTotalDamage(DamageClass.Melee).ApplyTo(100f) > meleeBefore);
		Assert.IsTrue(player.GetCritChance(DamageClass.Generic) > genericCrit);
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
	public void JumpPotionCanBeConsumed()
	{
		var potion = Defaults<JumpPotion>();
		Assert.IsTrue(potion.useTime > 0 && potion.useAnimation > 0 && potion.buffTime > 0 && potion.consumable);
	}

	[TestMethod]
	public void ManaSpringOreRequiresTheTwilightCrystalPickaxeTier()
	{
		var tile = new ManaSpringOre_Tile();
		bool oldServer = Main.dedServ;
		bool[][] flags = [Main.tileSolid, Main.tileMergeDirt, Main.tileBlendAll, Main.tileBlockLight, Main.tileShine2, TileID.Sets.ChecksForMerge, TileID.Sets.DrawsWalls];
		bool[] oldFlags = flags.Select(flag => flag[tile.Type]).ToArray();
		var host = new StoneScaleWood();
		var hostInstance = typeof(ContentInstance<StoneScaleWood>).GetProperty(nameof(ContentInstance<StoneScaleWood>.Instance))!;
		var dustInstance = typeof(ContentInstance<ManaSpringOreDust>).GetProperty(nameof(ContentInstance<ManaSpringOreDust>.Instance))!;
		object? oldHost = hostInstance.GetValue(null);
		object? oldDust = dustInstance.GetValue(null);
		bool[]? oldMergeRow = Main.tileMerge[host.Type];
		bool oldMerge = oldMergeRow?[tile.Type] ?? false;
		try
		{
			Main.dedServ = true;
			hostInstance.SetValue(null, host);
			dustInstance.SetValue(null, new ManaSpringOreDust());
			Main.tileMerge[host.Type] ??= new bool[Main.tileSolid.Length];
			tile.SetStaticDefaults();
			Assert.AreEqual(Defaults<MiningPowerPickaxe_3>().pick, tile.MinPick);
			Assert.IsTrue(Defaults<MiningPowerPickaxe_2>().pick < tile.MinPick);
			Assert.IsTrue(Defaults<EldamberPickaxe>().pick < tile.MinPick);
			Assert.IsFalse(tile.CanExplode(0, 0), "Explosives must not bypass the ore's progression gate.");
		}
		finally
		{
			Main.dedServ = oldServer;
			for (int i = 0; i < flags.Length; i++)
			{
				flags[i][tile.Type] = oldFlags[i];
			}
			if (oldMergeRow is not null)
			{
				oldMergeRow[tile.Type] = oldMerge;
			}
			Main.tileMerge[host.Type] = oldMergeRow!;
			hostInstance.SetValue(null, oldHost);
			dustInstance.SetValue(null, oldDust);
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
		Assert.IsTrue(Defaults<EldamberPickaxe>().pick > 0);
		Assert.IsTrue(Defaults<EldamberShortsword>().shoot > ProjectileID.None && Defaults<EldamberShortsword>().noMelee);
		Assert.IsTrue(Defaults<EldamberBow>().shootSpeed > 0);
	}
}
