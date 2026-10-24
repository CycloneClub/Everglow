using System.Reflection;
using Everglow.Yggdrasil.WorldGeneration;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class TownAmberDistributionTests
{
	private delegate int ScatterVeins(Rectangle generationBounds, Rectangle townBounds, int targetVeinCount, float minVeinSpacing, UnifiedRandom random, Func<Point, bool> canPlacePatchCenter, Func<Point, bool> tryPlaceVein);

	private static TDelegate GetGenerationDelegate<TDelegate>(string methodName)
		where TDelegate : Delegate
	{
		var method = typeof(YggdrasilTownGeneration).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method, $"Missing amber placement stage: {methodName}");
		return method.CreateDelegate<TDelegate>();
	}

	[TestMethod]
	public void VeinsStayInsideTheMineAndKeepTheirDistance()
	{
		var scatterVeins = GetGenerationDelegate<ScatterVeins>("ScatterAmberVeins");
		var getMineBounds = GetGenerationDelegate<Func<int, int, Rectangle>>("GetMineBounds");
		var isInGenerationArea = GetGenerationDelegate<Func<Point, int, int, bool>>("IsInAmberGenerationArea");
		var mineBounds = getMineBounds(2000, 21000);
		Assert.AreEqual(new Rectangle(40, 20180, 1010, 770), mineBounds);
		var townBounds = new Rectangle(218, 20388, 730, 299);
		var placedCenters = new List<Point>();
		int placedCount = scatterVeins(mineBounds, townBounds, 60, 40, new UnifiedRandom(42), point => isInGenerationArea(point, 2000, 21000), point =>
		{
			if (!isInGenerationArea(point, 2000, 21000))
			{
				return false;
			}
			placedCenters.Add(point);
			return true;
		});
		Assert.AreEqual(60, placedCount);
		Assert.AreEqual(placedCount, placedCenters.Count);
		Assert.IsTrue(placedCenters.All(point => mineBounds.Contains(point) && isInGenerationArea(point, 2000, 21000) && !townBounds.Contains(point)));
		for (int i = 0; i < placedCenters.Count; i++)
		{
			for (int j = i + 1; j < placedCenters.Count; j++)
			{
				Assert.IsTrue(Vector2.Distance(placedCenters[i].ToVector2(), placedCenters[j].ToVector2()) >= 40);
			}
		}
	}

	[TestMethod]
	public void MineRoofRejectsPillarsAboveTheCurve()
	{
		var isInGenerationArea = GetGenerationDelegate<Func<Point, int, int, bool>>("IsInAmberGenerationArea");
		Assert.IsFalse(isInGenerationArea(new Point(1000, 20380), 2000, 21000));
		Assert.IsTrue(isInGenerationArea(new Point(1000, 20550), 2000, 21000));
		Assert.IsFalse(isInGenerationArea(new Point(40, 20210), 2000, 21000));
		Assert.IsTrue(isInGenerationArea(new Point(40, 20220), 2000, 21000));
		Assert.IsFalse(isInGenerationArea(new Point(1050, 20800), 2000, 21000));
		Assert.IsFalse(isInGenerationArea(new Point(100, 20950), 2000, 21000));
	}

	[TestMethod]
	public void ScatterLeavesRichPatchesAndSparseAreas()
	{
		var scatterVeins = GetGenerationDelegate<ScatterVeins>("ScatterAmberVeins");
		var placedCenters = new List<Point>();
		int placedCount = scatterVeins(new Rectangle(0, 0, 1200, 900), Rectangle.Empty, 80, 40, new UnifiedRandom(42), _ => true, point =>
		{
			placedCenters.Add(point);
			return true;
		});
		Assert.AreEqual(80, placedCount);
		var density = new int[12];
		foreach (Point point in placedCenters)
		{
			density[point.X / 300 + point.Y / 300 * 4]++;
		}
		Assert.IsTrue(density.Max() >= 14, "There should be a rich patch with at least twice the average density.");
		Assert.IsTrue(density.Min() <= 3, "Some areas should remain sparse.");
		Assert.IsTrue(placedCenters.Any(point => placedCenters.Any(other => other != point && Vector2.Distance(point.ToVector2(), other.ToVector2()) < 80)), "Rich patches should contain neighboring groups less than 80 tiles apart.");
	}

	[TestMethod]
	public void RejectedSitesDoNotReserveSpaceAndFailedSearchTerminates()
	{
		var scatterVeins = GetGenerationDelegate<ScatterVeins>("ScatterAmberVeins");
		int placementAttempts = 0;
		int placedCount = scatterVeins(new Rectangle(0, 0, 40, 40), Rectangle.Empty, 1, 80, new UnifiedRandom(3), _ => true, _ => ++placementAttempts > 10);
		Assert.AreEqual(1, placedCount);
		Assert.AreEqual(11, placementAttempts);
		placementAttempts = 0;
		placedCount = scatterVeins(new Rectangle(0, 0, 40, 40), Rectangle.Empty, 1, 80, new UnifiedRandom(3), _ => true, _ =>
		{
			placementAttempts++;
			return false;
		});
		Assert.AreEqual(0, placedCount);
		Assert.IsTrue(placementAttempts > 0 && placementAttempts < 10000);
	}

	[TestMethod]
	public void ThinEdgesAndHeavilyClippedShapesAreRejected()
	{
		var collectTiles = GetGenerationDelegate<Func<Point, HashSet<Point>, Func<Point, bool>, List<Point>>>("CollectAmberVeinTiles");
		HashSet<Point> shape = [new(0, 0), new(1, 0), new(-1, 0)];
		Assert.AreEqual(0, collectTiles(Point.Zero, shape, point => point.Y == 0).Count);
		shape.UnionWith([new(12, 0), new(13, 0), new(14, 0)]);
		Assert.AreEqual(0, collectTiles(Point.Zero, shape, point => point.X < 10).Count);
	}

	[TestMethod]
	public void BuriedVeinsKeepTheirFootprintWithoutReplacingProtectedTiles()
	{
		var collectTiles = GetGenerationDelegate<Func<Point, HashSet<Point>, Func<Point, bool>, List<Point>>>("CollectAmberVeinTiles");
		HashSet<Point> shape = [new(0, 0), new(1, 0), new(2, 0), new(3, 0), new(4, 0), new(5, 0), new(6, 0), new(12, 0)];
		var center = new Point(100, 200);
		var protectedTile = new Point(112, 200);
		var tiles = collectTiles(center, shape, point => point != protectedTile);
		Assert.AreEqual(7, tiles.Count);
		Assert.IsFalse(tiles.Contains(protectedTile));
		Assert.IsTrue(tiles.Contains(center));
		Assert.IsTrue(tiles.All(point => shape.Contains(point - center)));
	}
}
