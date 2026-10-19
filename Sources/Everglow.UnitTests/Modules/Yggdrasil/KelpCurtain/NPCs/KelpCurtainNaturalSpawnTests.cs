using System.Reflection;
using Everglow.Yggdrasil.KelpCurtain.NPCSpawn;

namespace Everglow.UnitTests.Modules.Yggdrasil.KelpCurtain.NPCs;

[TestClass]
public class KelpCurtainNaturalSpawnTests
{
	[TestMethod]
	[DataRow(2, 2)]
	[DataRow(6, 6)]
	[DataRow(7, 7)]
	[DataRow(40, 7)]
	public void DepthCountsWaterAboveTheFloor(int waterDepth, int expected)
	{
		// 原生 SpawnTileY 是地面：地面及下面全干，上方才是水柱。
		Func<int, bool> water = y => y < 100 && y >= 100 - waterDepth;
		Assert.AreEqual(expected, CountWaterAbove(100, 7, water));
	}

	[TestMethod]
	public void WaterColumnStopsAtDryGapAndRespectsScanLimit()
	{
		Assert.AreEqual(2, CountWaterAbove(100, 128, y => y == 99 || y == 98 || y < 96));
		Assert.AreEqual(128, CountWaterAbove(1000, 128, _ => true));
		Assert.AreEqual(0, CountWaterAbove(100, 128, y => y > 100));
	}

	private static int CountWaterAbove(int floorY, int limit, Func<int, bool> water) =>
		(int)typeof(KelpCurtainSpawnConditions).GetMethod("CountWaterAbove", BindingFlags.Static | BindingFlags.NonPublic)!
			.Invoke(null, [floorY, limit, water])!;
}
