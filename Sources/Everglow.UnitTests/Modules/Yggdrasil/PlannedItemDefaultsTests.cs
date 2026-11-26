using Everglow.Commons.Mechanics.PlayerBonuses;
using Everglow.UnitTests.Function;
using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Tools;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Eldamber;
using Everglow.Yggdrasil.YggdrasilTown.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
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
	public void ManaSpringOreRequiresTheTwilightCrystalPickaxeTier()
	{
		var tile = new ManaSpringOre_Tile();
		bool oldServer = Main.dedServ;
		bool[][] flags = [Main.tileSolid, Main.tileMergeDirt, Main.tileBlendAll, Main.tileBlockLight, Main.tileShine2, TileID.Sets.ChecksForMerge, TileID.Sets.DrawsWalls];
		bool[] oldFlags = flags.Select(flag => flag[tile.Type]).ToArray();
		var host = new StoneScaleWood();
		var hostInstance = typeof(ContentInstance<StoneScaleWood>).GetProperty(nameof(ContentInstance<StoneScaleWood>.Instance))!;
		var dustInstance = typeof(ContentInstance<ManaSpringOreDust>).GetProperty(nameof(ContentInstance<ManaSpringOreDust>.Instance))!;
		var oreInstance = typeof(ContentInstance<ManaSpringOre>).GetProperty(nameof(ContentInstance<ManaSpringOre>.Instance))!;
		object? oldHost = hostInstance.GetValue(null);
		object? oldDust = dustInstance.GetValue(null);
		object? oldOre = oreInstance.GetValue(null);
		bool[]? oldMergeRow = Main.tileMerge[host.Type];
		bool oldMerge = oldMergeRow?[tile.Type] ?? false;
		bool oldSpelunker = Main.tileSpelunker[tile.Type];
		short oldPriority = Main.tileOreFinderPriority[tile.Type];
		bool oldOreSet = TileID.Sets.Ore[tile.Type];
		try
		{
			Main.dedServ = true;
			hostInstance.SetValue(null, host);
			dustInstance.SetValue(null, new ManaSpringOreDust());
			oreInstance.SetValue(null, new NamedManaSpringOre());
			Main.tileMerge[host.Type] ??= new bool[Main.tileSolid.Length];
			tile.SetStaticDefaults();
			Assert.AreEqual(Defaults<MiningPowerPickaxe_3>().pick, tile.MinPick);
			Assert.AreEqual(Defaults<TwilightCrystalPickaxe>().pick, tile.MinPick, "The ordinary pickaxe route must also unlock mana ore.");
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
			Main.tileSpelunker[tile.Type] = oldSpelunker;
			Main.tileOreFinderPriority[tile.Type] = oldPriority;
			TileID.Sets.Ore[tile.Type] = oldOreSet;
			hostInstance.SetValue(null, oldHost);
			dustInstance.SetValue(null, oldDust);
			oreInstance.SetValue(null, oldOre);
		}
	}

	// Map registration reads the item display name. A real name needs a loaded mod and language manager.
	private sealed class NamedManaSpringOre : ManaSpringOre
	{
		public override LocalizedText DisplayName => LocalizedText.Empty;
	}
}
