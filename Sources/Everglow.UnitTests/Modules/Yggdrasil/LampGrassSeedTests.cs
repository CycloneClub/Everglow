using System.Runtime.CompilerServices;
using Everglow.Yggdrasil.YggdrasilTown.Items.Placeables;
using Everglow.Yggdrasil.YggdrasilTown.Tiles.LampWood;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class LampGrassSeedTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	public void CuttingLampGrassHasOnePercentSeedChance()
	{
		var previousRandom = Main.rand;
		var instance = typeof(ContentInstance<LampGrassSeed>).GetProperty("Instance")!;
		var previousSeed = instance.GetValue(null);
		try
		{
			Main.rand = new UnifiedRandom(441);
			var seed = (LampGrassSeed)RuntimeHelpers.GetUninitializedObject(typeof(LampGrassSeed));
			var item = (Item)RuntimeHelpers.GetUninitializedObject(typeof(Item));
			item.type = ItemID.GrassSeeds;
			typeof(ModType<Item>).GetProperty("Entity")!.SetValue(seed, item);
			instance.SetValue(null, seed);
			var grass = (LampWood_Grass)RuntimeHelpers.GetUninitializedObject(typeof(LampWood_Grass));
			int count = 0;
			for (int i = 0; i < 10000; i++)
			{
				foreach (var drop in grass.GetItemDrops(0, 0))
				{
					Assert.AreEqual(ItemID.GrassSeeds, drop.type);
					Assert.AreEqual(1, drop.stack);
					count++;
				}
			}
			Assert.IsTrue(count is > 65 and < 135, $"Expected about 100 seeds, got {count}");
		}
		finally
		{
			Main.rand = previousRandom;
			instance.SetValue(null, previousSeed);
		}
	}
}
