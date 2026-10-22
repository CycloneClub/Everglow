using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class TownCrateLootTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(typeof(LampWoodCrate_Item))]
	[DataRow(typeof(TwilightCrate_Item))]
	[DataRow(typeof(UrbanCrate_Item))]
	public void EveryCrateRuleHasAtLeastOnePossibleOutcome(Type type)
	{
		var crate = (ModItem)RuntimeHelpers.GetUninitializedObject(type);
		var loot = new ItemLoot(1, new ItemDropDatabase());
		crate.ModifyItemLoot(loot);
		Assert.IsTrue(loot.Get().Count > 0);
		foreach (var rule in loot.Get())
		{
			List<DropRateInfo> rates = [];
			rule.ReportDroprates(rates, new DropRateInfoChainFeed(1));
			Assert.IsTrue(rates.Count > 0, $"{type.Name}: {rule.GetType().Name} contains an empty pool");
		}
	}
}
