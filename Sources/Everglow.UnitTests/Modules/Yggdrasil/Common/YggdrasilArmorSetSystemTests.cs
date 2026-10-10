using System.Reflection;
using Everglow.Yggdrasil.Common;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.CyanVine;
using Everglow.Yggdrasil.YggdrasilTown.Items.Armors.Trainee;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil.Common;

[TestClass]
[DoNotParallelize]
public class YggdrasilArmorSetSystemTests
{
	private static readonly MethodInfo CreateSetMethod = typeof(YggdrasilArmorSetSystem)
		.GetMethod("CreateSet", BindingFlags.Static | BindingFlags.NonPublic)!;

	private List<ArmorSetBonus> originalSets = null!;
	private ArmorSetBonus[][] originalLookup = null!;
	private bool originalDedServ;

	private sealed class BodyBonus : ModItem
	{
		public override void UpdateArmorSet(Player player) => player.statDefense += 5;
	}

	private sealed class LegBonus : ModItem
	{
		public override void UpdateArmorSet(Player player) => player.lifeRegen += 3;
	}

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
		originalDedServ = Main.dedServ;
		Main.dedServ = true;
		originalSets = ArmorSetBonuses.All;
		originalLookup = ArmorSetBonuses.SetsContaining;
		ArmorSetBonuses.All = [];
	}

	[TestCleanup]
	public void Cleanup()
	{
		ArmorSetBonuses.All = originalSets;
		ArmorSetBonuses.SetsContaining = originalLookup;
		Main.dedServ = originalDedServ;
	}

	[TestMethod]
	[DataRow(10, 7, 4)]
	[DataRow(11, 8, 5)]
	public void CompleteCombination_SelectsHeadVariantAndAppliesEveryPieceOnce(int headType, int defense, int lifeRegen)
	{
		RegisterVariants();
		var player = new Player { statDefense = Player.DefenseStat.Default, lifeRegen = 0 };
		var context = new ArmorSetBonus.QueryContext { HeadItem = headType, BodyItem = 20, LegItem = 30 };

		var selected = ArmorSetBonuses.GetCompleteSet(context);
		Assert.IsNotNull(selected);
		selected.Effect(player);

		Assert.AreEqual(defense, (int)player.statDefense);
		Assert.AreEqual(lifeRegen, player.lifeRegen);
	}

	[TestMethod]
	[DataRow(10, 20, 31)]
	[DataRow(10, 21, 30)]
	[DataRow(12, 20, 30)]
	public void MixedCombination_DoesNotEnableSetBonus(int head, int body, int legs)
	{
		RegisterVariants();

		var selected = ArmorSetBonuses.GetCompleteSet(new ArmorSetBonus.QueryContext
		{
			HeadItem = head,
			BodyItem = body,
			LegItem = legs,
		});

		Assert.IsNull(selected);
	}

	[TestMethod]
	[DataRow(0)]
	[DataRow(30)]
	public void TwoPieceSet_DoesNotRequireSpecificLegArmor(int legs)
	{
		ArmorSetBonuses.All.Add(CreateSet(
			new SafetyHelmets().NewInstance(new Item { type = 10 }),
			new BodyBonus().NewInstance(new Item { type = 20 }),
			null));
		ArmorSetBonuses.BuildLookup();
		var player = new Player { statDefense = Player.DefenseStat.Default, lifeRegen = 0 };

		var selected = ArmorSetBonuses.GetCompleteSet(new ArmorSetBonus.QueryContext
		{
			HeadItem = 10,
			BodyItem = 20,
			LegItem = legs,
		});
		Assert.IsNotNull(selected);
		selected.Effect(player);

		Assert.AreEqual(7, (int)player.statDefense);
		Assert.AreEqual(1, player.lifeRegen);
	}

	private static void RegisterVariants()
	{
		var body = new BodyBonus().NewInstance(new Item { type = 20 });
		var legs = new LegBonus().NewInstance(new Item { type = 30 });
		ArmorSetBonuses.All.Add(CreateSet(new SafetyHelmets().NewInstance(new Item { type = 10 }), body, legs));
		ArmorSetBonuses.All.Add(CreateSet(new CyanHeavylet().NewInstance(new Item { type = 11 }), body, legs));
		ArmorSetBonuses.BuildLookup();
	}

	private static ArmorSetBonus CreateSet(ModItem head, ModItem body, ModItem? legs)
	{
		return (ArmorSetBonus)CreateSetMethod.Invoke(null, [head, body, legs, LocalizedText.Empty])!;
	}
}
