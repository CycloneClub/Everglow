using System.Reflection;
using Everglow.Yggdrasil.WorldGeneration;
using Microsoft.Xna.Framework;
using Terraria.Utilities;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class TownAmberShapeTests
{
	private static TDelegate GetGenerationDelegate<TDelegate>(string methodName)
		where TDelegate : Delegate
	{
		var method = typeof(YggdrasilTownGeneration).GetMethod(methodName, BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method, $"Missing amber geometry stage: {methodName}");
		return method.CreateDelegate<TDelegate>();
	}

	[TestMethod]
	public void VeinSizesFollowTheConfiguredDistribution()
	{
		var createShape = GetGenerationDelegate<Func<UnifiedRandom, HashSet<Point>>>("CreateAmberVeinShape");
		var sizes = new List<double>();
		for (int seed = 0; seed < 5000; seed++)
		{
			var shape = createShape(new UnifiedRandom(seed));
			Assert.IsTrue(shape.Count > 0, $"Seed {seed} produced an empty group.");
			int width = shape.Max(point => point.X) - shape.Min(point => point.X) + 1;
			int height = shape.Max(point => point.Y) - shape.Min(point => point.Y) + 1;
			sizes.Add(Math.Sqrt(width * height));
		}
		double mean = sizes.Average();
		double variance = sizes.Average(size => (size - mean) * (size - mean));
		// Linear size is the geometric mean of the rasterized width and height.
		// Allow for tile quantization and disappearing sub-tile edge fragments.
		Assert.AreEqual(8.2415, mean, 0.3, $"Mean group size: {mean:F3}");
		Assert.AreEqual(11.41, variance, 0.7, $"Group size variance: {variance:F3}");
	}

	[TestMethod]
	public void RelaxationIsIndependentOfPointTraversalOrder()
	{
		var relax = GetGenerationDelegate<Action<Vector2[], float, int>>("RelaxAmberPoints");
		Vector2[] points = [new(-1, 0), new(0.5f, 0.2f), new(0, 1), new(2, -0.5f)];
		Vector2[] reversed = points.Reverse().ToArray();
		relax(points, 4, 8);
		relax(reversed, 4, 8);
		Array.Reverse(reversed);
		for (int i = 0; i < points.Length; i++)
		{
			Assert.IsTrue(Vector2.Distance(points[i], reversed[i]) < 0.0001f, $"Point {i} depends on traversal order.");
		}
	}

	[TestMethod]
	public void ClosePointsRepelWhileIsolatedPointsMoveTowardTheFixedCenter()
	{
		var relax = GetGenerationDelegate<Action<Vector2[], float, int>>("RelaxAmberPoints");
		Vector2[] pair = [new(-0.1f, 0), new(0.1f, 0)];
		relax(pair, 4, 1);
		Assert.IsTrue(Vector2.Distance(pair[0], pair[1]) > 0.2f);
		Assert.IsTrue((pair[0] + pair[1]).Length() < 0.0001f);
		Vector2[] isolated = [new(8, 4)];
		relax(isolated, 4, 8);
		Assert.IsTrue(isolated[0].Length() < new Vector2(8, 4).Length());
		Assert.IsTrue(isolated[0].X > 0 && isolated[0].Y > 0);
	}

	[TestMethod]
	public void CoincidentPointsSeparateWithoutNonfiniteCoordinates()
	{
		var relax = GetGenerationDelegate<Action<Vector2[], float, int>>("RelaxAmberPoints");
		Vector2[] points = [Vector2.Zero, Vector2.Zero, Vector2.Zero];
		relax(points, 4, 16);
		Assert.IsTrue(points.All(point => float.IsFinite(point.X) && float.IsFinite(point.Y)));
		Assert.AreEqual(3, points.Distinct().Count());
	}

	[TestMethod]
	public void NonintersectingCirclesKeepTheirFullOutline()
	{
		var rasterize = GetGenerationDelegate<Func<Vector2[], float[], float, HashSet<Point>>>("RasterizeAmberCircles");
		var shape = rasterize([Vector2.Zero, new(7, 0)], [5, 1], 1.25f);
		Assert.AreEqual(86, shape.Count); // Radius-five disk: 81 cells; radius-one disk: 5 cells.
		Assert.IsTrue(shape.Contains(new Point(5, 0)), "A nonintersecting neighbor must not clip the larger circle.");
		Assert.IsTrue(shape.Contains(new Point(-5, 0)));
		Assert.IsFalse(shape.Contains(new Point(4, 4)), "The exterior must remain circular.");
	}

	[TestMethod]
	public void IntersectingCirclesHaveStraightFacesAndARockGap()
	{
		var rasterize = GetGenerationDelegate<Func<Vector2[], float[], float, HashSet<Point>>>("RasterizeAmberCircles");
		var shape = rasterize([new(-2, 0), new(2, 0)], [3, 3], 1.25f);
		Assert.AreEqual(46, shape.Count);
		Assert.IsFalse(shape.Any(point => point.X == 0), "The shared bisector must remain rock.");
		for (int y = -2; y <= 2; y++)
		{
			Assert.IsTrue(shape.Contains(new Point(-1, y)));
			Assert.IsTrue(shape.Contains(new Point(1, y)));
		}
		Assert.IsTrue(shape.Contains(new Point(-5, 0)));
		Assert.IsTrue(shape.Contains(new Point(5, 0)));
		Assert.IsFalse(shape.Contains(new Point(-5, 1)));
		Assert.IsTrue(shape.SetEquals(rasterize([new(2, 0), new(-2, 0)], [3, 3], 1.25f)));
	}

	[TestMethod]
	public void VeinsAreNonemptyReproducibleAndVaried()
	{
		var createShape = GetGenerationDelegate<Func<UnifiedRandom, HashSet<Point>>>("CreateAmberVeinShape");
		var sizes = new HashSet<int>();
		for (int seed = 0; seed < 100; seed++)
		{
			var shape = createShape(new UnifiedRandom(seed));
			Assert.IsTrue(shape.Count > 0);
			Assert.IsTrue(shape.SetEquals(createShape(new UnifiedRandom(seed))));
			sizes.Add(shape.Count);
		}
		Assert.IsTrue(sizes.Count > 1);
	}
}
