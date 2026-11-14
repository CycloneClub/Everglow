using System.Reflection;
using Everglow.Yggdrasil.WorldGeneration;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class JellyBallHotbedGenerationTests
{
	// Layout palette: 0 terrain, 1 air, 2 jelly, 3 crystal, 4 membrane, 5 fresh, 8 honey, 9 protected, 10 cave air.
	private static byte[,] Generate(byte[,] source, int seed)
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout");
		Assert.IsNotNull(type, "The lower hotbed generator is missing.");
		var method = type.GetMethod("Generate", BindingFlags.Public | BindingFlags.Static);
		Assert.IsNotNull(method);
		return (byte[,])method.Invoke(null, [source, seed])!;
	}

	private static byte[,] OldHotbed()
	{
		var source = new byte[350, 480];
		for (int x = 35; x < 315; x++)
		{
			for (int y = 45; y < 305; y++)
			{
				source[x, y] = (byte)((x % 35 < 20) ? 2 : 1);
			}
		}
		return source;
	}

	[TestMethod]
	public void CentralConnectionOnlyTouchesTheLastFewRowsOfTheOldRegion()
	{
		var source = OldHotbed();
		var result = Generate(source, 42);
		// Fixture's actual lower edge is row 304. Decorating existing jelly is allowed;
		// filling old cracks or carving old terrain above the last six rows is not.
		for (int x = 125; x <= 225; x++)
		{
			for (int y = 190; y < 298; y++)
			{
				if (source[x, y] is 0 or 1)
				{
					Assert.AreEqual(source[x, y], result[x, y], $"The connection invaded the old region at {x},{y}.");
				}
			}
		}
	}

	[TestMethod]
	public void UnevenOldTipsConnectToTheLowerWallThroughCellTissue()
	{
		var source = OldHotbed();
		for (int x = 125; x <= 225; x++)
		{
			int end = 265 + ((x - 125) / 20) * 7;
			for (int y = end + 1; y <= 304; y++)
			{
				source[x, y] = 0;
			}
		}
		for (int seed = 0; seed < 12; seed++)
		{
			var result = Generate(source, seed);
			foreach (int x in new[] { 145, 180, 215 })
			{
				int end = 265 + ((x - 125) / 20) * 7;
				var visited = new HashSet<(int X, int Y)> { (x, end) };
				var queue = new Queue<(int X, int Y)>();
				queue.Enqueue((x, end));
				bool connected = false;
				while (queue.TryDequeue(out var point))
				{
					connected |= point.Y >= 430;
					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							var next = (X: point.X + dx, Y: point.Y + dy);
							if (next.X > 0 && next.X < 350 && next.Y > 0 && next.Y < 480
								&& result[next.X, next.Y] is 2 or 3 or 5 && visited.Add(next))
							{
								queue.Enqueue(next);
							}
						}
					}
				}
				Assert.IsTrue(connected, $"Old tip {x},{end} remains separated from the lower wall, seed {seed}.");
			}
		}
	}

	[TestMethod]
	public void DeepOldEdgeStillBuildsATerminalConnection()
	{
		var source = OldHotbed();
		for (int x = 35; x < 315; x++)
		{
			for (int y = 305; y <= 400; y++)
			{
				source[x, y] = (byte)(x % 35 < 20 ? 2 : 1);
			}
		}
		var result = Generate(source, 42);
		int connection = 0;
		for (int x = 163; x <= 187; x++)
		{
			for (int y = 402; y < 415; y++)
			{
				if (result[x, y] is 2 or 3 or 5)
				{
					connection++;
				}
			}
		}
		Assert.IsTrue(connection > 150, "A deep old tip must still have a substantial connection below it.");
	}

	[TestMethod]
	public void UpperTerrainAndProtectedStructuresSurvive()
	{
		var source = OldHotbed();
		source[175, 390] = 9;
		var result = Generate(source, 42);
		Assert.AreEqual((byte)9, result[175, 390]);
		for (int x = 0; x < 350; x++)
		{
			for (int y = 0; y < 190; y++)
			{
				if (source[x, y] != 2)
				{
					Assert.AreEqual(source[x, y], result[x, y], $"Upper terrain changed at {x},{y}.");
				}
			}
		}
		Assert.IsTrue(Enumerable.Range(45, 120).Any(y => result[54, y] == 5), "Fresh patches must reach exposed OLD surfaces.");
	}

	[TestMethod]
	public void PoolCannotLeakToExteriorAcrossMultipleSeeds()
	{
		for (int seed = 0; seed < 12; seed++)
		{
			var result = Generate(OldHotbed(), seed);
			var water = new List<(int X, int Y)>();
			for (int x = 0; x < 350; x++)
			{
				for (int y = 0; y < 480; y++)
				{
					if (result[x, y] == 8)
					{
						water.Add((x, y));
					}
				}
			}
			Assert.IsTrue(water.Count > 400, $"Missing pool for seed {seed}.");
			int surface = water.Min(p => p.Y);
			var queue = new Queue<(int X, int Y)>(water);
			var seen = water.ToHashSet();
			while (queue.TryDequeue(out var point))
			{
				Assert.IsTrue(point.X > 0 && point.X < 349 && point.Y < 479, $"Pool leaks for seed {seed}.");
				foreach (var next in new[] { (point.X - 1, point.Y), (point.X + 1, point.Y), (point.X, point.Y - 1), (point.X, point.Y + 1) })
				{
					if (next.Item2 >= surface && result[next.Item1, next.Item2] is 1 or 8 or 10 && seen.Add(next))
					{
						queue.Enqueue(next);
					}
				}
			}
		}
	}

	[TestMethod]
	public void SurfacePatchesCannotReachThroughForeignTerrain()
	{
		var source = OldHotbed();
		for (int x = 25; x < 110; x++)
		{
			for (int y = 30; y < 180; y++)
			{
				source[x, y] = x == 80 ? (byte)0 : x == 81 ? (byte)1 : (byte)2;
			}
		}
		var result = Generate(source, 42);
		for (int y = 40; y < 170; y++)
		{
			Assert.AreEqual((byte)2, result[79, y], "Fresh secretion cannot jump across a foreign solid block to reach air.");
		}
	}

	[TestMethod]
	[DataRow(false)]
	[DataRow(true)]
	public void FreshPatchesGrowIntoTissueWithoutCoatingTheWholeSurface(bool membrane)
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var source = new byte[350, 480];
		for (int x = 30; x < 320; x++)
		{
			source[x, 299] = membrane ? (byte)4 : (byte)1;
			for (int y = 300; y < 440; y++)
			{
				source[x, y] = 2;
			}
		}
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [source, seed], null)!;
			type.GetMethod("FreshenSurfaces", flags)!.Invoke(layout, null);
			var result = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			var connected = new HashSet<(int X, int Y)>();
			var queue = new Queue<(int X, int Y)>();
			for (int x = 30; x < 320; x++)
			{
				if (result[x, 300] == 5)
				{
					connected.Add((x, 300));
					queue.Enqueue((x, 300));
				}
			}
			Assert.IsTrue(connected.Count is >= 10 and < 175, $"Fresh patches should remain intermittent, seed {seed}.");
			while (queue.TryDequeue(out var point))
			{
				foreach (var direction in new[] { (-1, 0), (1, 0), (0, -1), (0, 1) })
				{
					var next = (X: point.X + direction.Item1, Y: point.Y + direction.Item2);
					if (result[next.X, next.Y] == 5 && connected.Add(next))
					{
						queue.Enqueue(next);
					}
				}
			}
			Assert.IsTrue(connected.Count(point => point.Y >= 305) > 15, $"Fresh replacement is still a thin surface skin, seed {seed}.");
			Assert.AreEqual(result.Cast<byte>().Count(tile => tile == 5), connected.Count, "Every patch must remain attached to its exposed surface or membrane.");
			for (int x = 0; x < 350; x++)
			{
				for (int y = 0; y < 480; y++)
				{
					Assert.IsTrue(result[x, y] == source[x, y] || (source[x, y] == 2 && result[x, y] == 5), "Freshening changed the geometry or replaced a foreign material.");
				}
			}
		}
	}

	[TestMethod]
	public void FreshPatchInnerEdgeFadesIntoTheOriginalTissue()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var source = new byte[350, 480];
		var region = new bool[350, 480];
		for (int x = 30; x < 320; x++)
		{
			source[x, 299] = 1;
			for (int y = 300; y < 440; y++)
			{
				source[x, y] = 2;
				region[x, y] = x >= 70 && x <= 150 && y <= 320;
			}
		}
		var layout = Activator.CreateInstance(type, flags, null, [source, 42], null)!;
		type.GetMethod("ApplyFreshPatch", flags)!.Invoke(layout, [source, region, false]);
		var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
		int core = Enumerable.Range(75, 70).Count(x => tiles[x, 315] == 5);
		int middle = Enumerable.Range(75, 70).Count(x => tiles[x, 319] == 5);
		int edge = Enumerable.Range(75, 70).Count(x => tiles[x, 320] == 5);
		Assert.AreEqual(70, core, "Feathering must retain a solid patch interior.");
		Assert.IsTrue(edge > 10 && edge < middle && middle < core, "The internal border needs a gradual material mixture instead of a hard cutoff.");
		Assert.IsTrue(Enumerable.Range(75, 70).All(x => tiles[x, 321] == 2), "Feathering must not spread beyond the infiltration region.");
	}

	[TestMethod]
	public void FresheningDoesNotChangeNucleusPositionsOrShapes()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var source = OldHotbed();
		var layout = Activator.CreateInstance(type, flags, null, [source, 42], null)!;
		foreach (string stage in new[] { "GrowCells", "FitPoolFloor", "CarveChamber", "BuildPool", "FillPackingPockets", "ScatterPeripheralCells", "PlaceCrystals" })
		{
			type.GetMethod(stage, flags)!.Invoke(layout, null);
		}
		var before = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
		var after = Generate(source, 42);
		CollectionAssert.AreEqual(before.Cast<byte>().Select(tile => tile == 3).ToArray(), after.Cast<byte>().Select(tile => tile == 3).ToArray());
	}

	[TestMethod]
	public void NeckIsMostlyTissueAndMembraneFresheningIsSparse()
	{
		var result = Generate(OldHotbed(), 42);
		int tissue = 0;
		for (int x = 163; x <= 187; x++)
		{
			// Check the retained cap immediately below the old edge, above the raised vault.
			for (int y = 305; y < 315; y++)
			{
				if (result[x, y] is 2 or 3 or 5)
				{
					tissue++;
				}
			}
		}
		Assert.IsTrue(tissue > 25 * 10 * 0.65, "The neck cap should remain a perforated tissue mass, not an open shaft.");
		int fresh = 0;
		int contacts = 0;
		for (int x = 5; x < 345; x++)
		{
			for (int y = 5; y < 475; y++)
			{
				if (result[x, y - 1] == 4 && result[x, y] is 2 or 5)
				{
					contacts++;
					fresh += result[x, y] == 5 ? 1 : 0;
				}
			}
		}
		Assert.IsTrue(contacts > 20);
		Assert.IsTrue(fresh < contacts * 0.4, "Fresh patches must not form a second membrane.");
	}

	[TestMethod]
	public void LowerCaveInteriorContainsNoNativeRockBetweenCells()
	{
		var result = Generate(OldHotbed(), 42);
		for (int x = 80; x < 270; x++)
		{
			for (int y = 400; y < 420; y++)
			{
				Assert.AreNotEqual((byte)0, result[x, y], $"Native rock remains inside the lower cave at {x},{y}.");
			}
		}
	}

	[TestMethod]
	public void LowerCaveHasNoAirGapBetweenItsCellWallAndNativeTerrain()
	{
		for (int seed = 0; seed < 12; seed++)
		{
			var result = Generate(OldHotbed(), seed);
			for (int x = 5; x < 345; x++)
			{
				for (int y = 305; y < 475; y++)
				{
					if (result[x, y] is not (1 or 8 or 10))
					{
						continue;
					}
					for (int dx = -1; dx <= 1; dx++)
					{
						for (int dy = -1; dy <= 1; dy++)
						{
							Assert.AreNotEqual((byte)0, result[x + dx, y + dy], $"Unlined cave gap at {x},{y}, seed {seed}.");
						}
					}
				}
			}
		}
	}

	[TestMethod]
	public void HotbedBoundsStayInsideTheWorldGenerationSafeMargin()
	{
		var method = typeof(YggdrasilTownGeneration).GetMethod("GetJellyBallHotbedBounds", BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		var bounds = (Microsoft.Xna.Framework.Rectangle)method.Invoke(null, [2000, 21000])!;
		Assert.IsTrue(bounds.Right <= 1980, "Do not generate cells in the padding beyond SafeGetTile's clamp.");
		Assert.IsTrue(bounds.Bottom <= 20980);
		Assert.IsTrue(bounds.Contains(1900, 20610), "The observatory must remain inside the inspected area.");
	}

	[TestMethod]
	public void CentralVaultRisesWellAboveThePoolWithoutBecomingAPinhole()
	{
		var result = Generate(OldHotbed(), 42);
		var open = new HashSet<(int X, int Y)>();
		var queue = new Queue<(int X, int Y)>();
		int surface = -1;
		for (int y = 0; y < 480; y++)
		{
			if (result[175, y] == 8)
			{
				surface = y;
				queue.Enqueue((175, y));
				open.Add((175, y));
				break;
			}
		}
		while (queue.TryDequeue(out var p))
		{
			foreach (var n in new[] { (p.X - 1, p.Y), (p.X + 1, p.Y), (p.X, p.Y - 1), (p.X, p.Y + 1) })
			{
				if (n.Item1 >= 0 && n.Item1 < 350 && n.Item2 >= 0 && n.Item2 < 480 && result[n.Item1, n.Item2] is 1 or 8 or 10 && open.Add(n))
				{
					queue.Enqueue(n);
				}
			}
		}
		// 普通细胞的圆润凸起会让最高点浮动；检查实际净高，不将轮廓锁在固定行。
		Assert.IsTrue(surface >= 0 && open.Any(p => surface - p.Y >= 60), "The connected vault needs at least 60 tiles of clearance above the pool.");
		Assert.IsTrue(Enumerable.Range(163, 25).Count(x => open.Contains((x, 340))) >= 18, "The elevated space must have usable width, not be a tiny hole.");
	}

	[TestMethod]
	public void VaultConcentratesItsRiseInTheCentralThird()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		object layout = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, [OldHotbed(), 42], null)!;
		var roof = type.GetMethod("ChamberRoof", BindingFlags.Instance | BindingFlags.NonPublic)!;
		float At(float x) => (float)roof.Invoke(layout, [x])!;
		foreach (int side in new[] { -1, 1 })
		{
			float innerRise = At(175 + side * 34) - At(175);
			float outerRise = At(175 + side * 101) - At(175 + side * 34);
			Assert.IsTrue(innerRise > outerRise * 1.3f, "The central third should rise more sharply than the outer two thirds.");
		}
	}

	[TestMethod]
	public void UpperTransitionContainsOnlyASparseSetOfMainCells()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			type.GetMethod("GrowCells", flags)!.Invoke(layout, null);
			float surface = (float)type.GetProperty("PoolSurface", flags)!.GetValue(layout)!;
			var cells = (System.Collections.IEnumerable)type.GetField("nucleusCandidates", flags)!.GetValue(layout)!;
			int upper = 0;
			foreach (var cell in cells)
			{
				float x = (float)cell.GetType().GetProperty("X")!.GetValue(cell)!;
				float y = (float)cell.GetType().GetProperty("Y")!.GetValue(cell)!;
				upper += Math.Abs(x - 175) < 105 && y < surface ? 1 : 0;
			}
			Assert.IsTrue(upper is >= 2 and <= 6, $"Upper transition has {upper} main cells, seed {seed}; avoid several overlapping rows of nuclei.");
		}
	}

	[TestMethod]
	public void BasalBudsAttachBySmallOverlapsAndKeepTheirWholeBodies()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			for (int x = 30; x < 320; x++)
			{
				for (int y = 410; y <= 440; y++)
				{
					tiles[x, y] = 2;
				}
			}
			var before = (byte[,])tiles.Clone();
			type.GetMethod("GrowBasalBuds", flags)!.Invoke(layout, null);
			var cells = (System.Collections.IList)type.GetField("nucleusCandidates", flags)!.GetValue(layout)!;
			Assert.IsTrue(cells.Count is >= 1 and <= 3, "Buds should be occasional attached lobes, not another continuous cell row.");
			foreach (var cell in cells)
			{
				var body = ((IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!).ToArray();
				float overlap = body.Count(point => before[point.X, point.Y] == 2) / (float)body.Length;
				Assert.IsTrue(overlap is >= 0.035f and <= 0.12f, "The bud must attach by a narrow root, rather than merge into the whole wall.");
				Assert.IsTrue(body.All(point => tiles[point.X, point.Y] == 2), "A bud was clipped.");
			}
		}
	}

	[TestMethod]
	public void AuxiliaryPackingDoesNotCreateNuclei()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), 42], null)!;
		type.GetMethod("GrowCells", flags)!.Invoke(layout, null);
		var candidates = (System.Collections.IList)type.GetField("nucleusCandidates", flags)!.GetValue(layout)!;
		var cells = (System.Collections.IList)type.GetField("cells", flags)!.GetValue(layout)!;
		Assert.IsTrue(candidates.Count > 0 && candidates.Count < cells.Count, "Packing cells must be excluded from nucleus candidates.");
		candidates.Clear();
		type.GetMethod("PlaceCrystals", flags)!.Invoke(layout, null);
		var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
		Assert.IsFalse(tiles.Cast<byte>().Contains((byte)3), "No ordinary candidates means no nuclei, even with a packed wall.");
	}

	[TestMethod]
	public void NucleusAreaScalesWithItsOwnCell()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var cellType = type.GetNestedType("Cell", BindingFlags.NonPublic)!;
		int previousArea = 0;
		foreach (var size in new[] { (X: 10f, Y: 14f), (X: 18f, Y: 22f) })
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), 42], null)!;
			var cell = Activator.CreateInstance(cellType, [175f, 450f, size.X, size.Y, 0f])!;
			type.GetMethod("AddCell", flags)!.Invoke(layout, [cell, true, false, true, false]);
			var body = ((IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!).ToArray();
			type.GetMethod("PlaceCrystals", flags)!.Invoke(layout, null);
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			int area = body.Count(point => tiles[point.X, point.Y] == 3);
			Assert.IsTrue(area > previousArea, "A larger cell needs a larger nucleus, not a random fixed-size cross.");
			Assert.IsTrue(area / (float)body.Length is >= 0.025f and <= 0.055f, "Nucleus area should remain proportional to its cell.");
			previousArea = area;
		}
	}

	[TestMethod]
	public void NearbyMainCellsBothKeepTheirNuclei()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), 42], null)!;
		var cellType = type.GetNestedType("Cell", BindingFlags.NonPublic)!;
		foreach (float x in new[] { 120f, 128f })
		{
			var cell = Activator.CreateInstance(cellType, [x, 450f, 8f, 8f, 0f])!;
			type.GetMethod("AddCell", flags)!.Invoke(layout, [cell, true, false, true, false]);
		}
		type.GetMethod("PlaceCrystals", flags)!.Invoke(layout, null);
		var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
		Assert.AreEqual((byte)3, tiles[120, 451]);
		Assert.AreEqual((byte)3, tiles[128, 451], "Nearby ordinary cells must not lose nuclei to a spacing filter.");
	}

	[TestMethod]
	public void OldLowerEdgeScatterUsesOnlyTheExterior120DegreeSector()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var source = new byte[330, 486];
		for (int x = 5; x < 325; x++)
		{
			for (int y = 5; y < 270; y++)
			{
				if (MathF.Pow((x - 175) / 105f, 2) + MathF.Pow((y - 120) / 135f, 2) < 1)
				{
					source[x, y] = 2;
				}
			}
		}
		for (int seed = 0; seed < 6; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [source, seed], null)!;
			type.GetMethod("ScatterOldLowerEdge", flags)!.Invoke(layout, null);
			var cells = (System.Collections.IList)type.GetField("cells", flags)!.GetValue(layout)!;
			Assert.IsTrue(cells.Count >= 6, $"Expected several scattered cells along the old lower edge, seed {seed}.");
			foreach (var cell in cells)
			{
				var cellType = cell!.GetType();
				float rx = (float)cellType.GetProperty("RadiusX")!.GetValue(cell)!;
				float ry = (float)cellType.GetProperty("RadiusY")!.GetValue(cell)!;
				Assert.IsTrue(rx < 7 && ry / rx >= 1.6f && ry / rx <= 2.4f, "Peripheral splash cells need a longer axis and a compact head.");
				float angle = (float)cellType.GetProperty("SplashAngle")!.GetValue(cell)!;
				float cx = (float)cellType.GetProperty("X")!.GetValue(cell)!;
				float cy = (float)cellType.GetProperty("Y")!.GetValue(cell)!;
				var body = ((IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!).ToArray();
				var sections = body.Select(point => ((point.X - cx) * MathF.Sin(angle) + (point.Y - cy) * MathF.Cos(angle)) / ry).ToArray();
				int head = sections.Count(along => along >= 0.15f && along < 0.55f);
				int tail = sections.Count(along => along >= -0.75f && along < -0.35f);
				Assert.IsTrue(head > tail * 1.5f, "Splash cells need a round head and a distinctly narrower inward tail.");
				foreach (var point in body)
				{
					Assert.IsTrue(MathF.Abs(MathF.Atan2(point.X - 175, point.Y - 120)) <= MathF.PI / 3);
					Assert.AreEqual((byte)0, source[point.X, point.Y], "Scatter must stay outside the original tissue.");
					for (int dx = -2; dx <= 2; dx++)
					{
						for (int dy = -2; dy <= 2; dy++)
						{
							Assert.AreEqual((byte)0, source[point.X + dx, point.Y + dy], "A scattered cell touches an old tip.");
						}
					}
				}
			}
		}
	}

	[TestMethod]
	public void PeripheralCellsDoNotChangeTheExistingInteriorLayout()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var source = OldHotbed();
		var baseline = Activator.CreateInstance(type, flags, null, [source, 42], null)!;
		foreach (string stage in new[] { "GrowCells", "FitPoolFloor", "CarveChamber", "BuildPool", "FillPackingPockets", "FreshenSurfaces", "PlaceCrystals" })
		{
			type.GetMethod(stage, flags)!.Invoke(baseline, null);
		}
		var original = (byte[,])type.GetField("tiles", flags)!.GetValue(baseline)!;
		var result = Generate(source, 42);
		for (int x = 0; x < original.GetLength(0); x++)
		{
			for (int y = 0; y < original.GetLength(1); y++)
			{
				if (original[x, y] is not (0 or 1))
				{
					Assert.AreEqual(original[x, y], result[x, y], $"Peripheral decoration changed existing tissue or pool at {x},{y}.");
				}
			}
		}
	}

	[TestMethod]
	public void PeripheralCellsStayWholeAndSeparateFromTheCompletedWall()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 6; seed++)
		{
			var source = new byte[330, 480];
			var old = OldHotbed();
			for (int x = 0; x < 330; x++)
			{
				for (int y = 0; y < 480; y++)
				{
					source[x, y] = old[x + 10, y];
				}
			}
			var layout = Activator.CreateInstance(type, flags, null, [source, seed], null)!;
			foreach (string stage in new[] { "GrowCells", "FitPoolFloor", "CarveChamber", "BuildPool", "FillPackingPockets" })
			{
				type.GetMethod(stage, flags)!.Invoke(layout, null);
			}
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			var before = (byte[,])tiles.Clone();
			var cells = (System.Collections.IList)type.GetField("cells", flags)!.GetValue(layout)!;
			int count = cells.Count;
			type.GetMethod("ScatterPeripheralCells", flags)!.Invoke(layout, null);
			Assert.IsTrue(cells.Count - count >= 6, $"Too few peripheral droplets, seed {seed}.");
			for (int i = count; i < cells.Count; i++)
			{
				var body = ((IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cells[i]])!).ToArray();
				Assert.IsTrue(body.All(point => tiles[point.X, point.Y] == 2), "A peripheral cell was clipped.");
				Assert.IsTrue(body.All(point => before[point.X, point.Y] is 0 or 1), "Peripheral droplets must not fill or alter the completed wall and pool.");
				foreach (var point in body)
				{
					for (int dx = -2; dx <= 2; dx++)
					{
						for (int dy = -2; dy <= 2; dy++)
						{
							Assert.IsFalse(before[point.X + dx, point.Y + dy] is 2 or 4 or 8, "A scattered droplet touches the wall or pool.");
						}
					}
				}
			}
		}
	}

	[TestMethod]
	public void EveryMainCellRetainsANucleusWithoutSpacingRejection()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			foreach (string stage in new[] { "GrowCells", "FitPoolFloor", "CarveChamber", "BuildPool", "FillPackingPockets", "ScatterPeripheralCells", "FreshenSurfaces", "PlaceCrystals" })
			{
				type.GetMethod(stage, flags)!.Invoke(layout, null);
			}
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			var candidates = (System.Collections.IList)type.GetField("nucleusCandidates", flags)!.GetValue(layout)!;
			foreach (var cell in candidates)
			{
				var body = (IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!;
				Assert.IsTrue(body.Any(point => tiles[point.X, point.Y] == 3), $"Main cell lost its nucleus: {cell}, seed {seed}.");
			}
			Assert.IsTrue(tiles.Cast<byte>().Count(material => material == 3) >= candidates.Count, "Each main cell needs its own nucleus allocation.");
		}
	}

	[TestMethod]
	public void PoolHillsStaySubmergedAndMembraneLipsContainTheLiquid()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			foreach (string stage in new[] { "GrowCells", "FitPoolFloor", "CarveChamber", "BuildPool" })
			{
				type.GetMethod(stage, flags)!.Invoke(layout, null);
			}
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			int surface = (int)(float)type.GetProperty("PoolSurface", flags)!.GetValue(layout)!;
			for (int x = 95; x <= 255; x++)
			{
				for (int y = 0; y <= surface; y++)
				{
					Assert.AreNotEqual((byte)4, tiles[x, y], $"Pool hill rises above the liquid at {x},{y}, seed {seed}.");
				}
			}
			foreach (int x in new[] { 73, 276 })
			{
				Assert.IsTrue(tiles[x, surface - 3] is 2 or 4 or 5, "The continuous membrane lip must rise above the liquid.");
				Assert.AreNotEqual((byte)8, tiles[x, surface], "Liquid must stay inside the membrane lips.");
			}
		}
	}

	[TestMethod]
	public void PackingFillsInternalPocketsAndShortGaps()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), 42], null)!;
		var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
		var excavated = (bool[,])type.GetField("excavated", flags)!.GetValue(layout)!;
		for (int x = 270; x <= 320; x++)
		{
			for (int y = 380; y <= 430; y++)
			{
				tiles[x, y] = 2;
				excavated[x, y] = true;
				if ((x >= 280 && x <= 284 && y >= 395 && y <= 399) || (x >= 299 && x <= 301))
				{
					tiles[x, y] = 10;
				}
			}
		}
		type.GetMethod("SealCellPacking", flags)!.Invoke(layout, null);
		Assert.AreEqual((byte)2, tiles[282, 397], "An enclosed pocket must be filled.");
		Assert.AreEqual((byte)2, tiles[300, 405], "Nearby bodies must be joined across the short gap.");
	}

	[TestMethod]
	public void PoolMembraneCoversIntactCellsWithThreeToSixTileThickness()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			type.GetMethod("GrowCells", flags)!.Invoke(layout, null);
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			var body = (byte[,])tiles.Clone();
			foreach (string stage in new[] { "FitPoolFloor", "CarveChamber", "BuildPool" })
			{
				type.GetMethod(stage, flags)!.Invoke(layout, null);
			}
			var floor = (float[])type.GetField("fittedFloor", flags)!.GetValue(layout)!;
			for (int x = 73; x <= 276; x++)
			{
				Assert.IsTrue(floor[x] < 475, $"Missing actual basin support at {x}, seed {seed}.");
			}
			int total = 0;
			var tops = new List<int>();
			for (int x = 125; x <= 225; x++)
			{
				int top = 480;
				for (int y = 0; y < 480; y++)
				{
					if (body[x, y] == 2)
					{
						Assert.IsTrue(tiles[x, y] is 2 or 5, "Membrane must fill above intact cells, not repaint a cut through them.");
					}
					if (tiles[x, y] == 4)
					{
						total++;
						top = Math.Min(top, y);
						if (y > 0 && tiles[x, y - 1] == 8)
						{
							int distanceSquared = int.MaxValue;
							for (int dx = -6; dx <= 6; dx++)
							{
								for (int dy = -6; dy <= 6; dy++)
								{
									if (y + dy >= 0 && y + dy < 480 && body[x + dx, y + dy] == 2)
									{
										distanceSquared = Math.Min(distanceSquared, dx * dx + dy * dy);
									}
								}
							}
							Assert.IsTrue(distanceSquared >= 9 && distanceSquared <= 25, $"Uneven membrane normal thickness at {x},{y}, seed {seed}: distance squared {distanceSquared}.");
						}
					}
				}
				tops.Add(top);
			}
			Assert.IsTrue(total / 101f >= 3 && total / 101f <= 6, $"Membrane should stay within 3–6 tiles, seed {seed}.");
			Assert.IsTrue(tops.Max() - tops.Min() >= 5, $"Membrane must follow the piled cells, seed {seed}.");
		}
	}

	[TestMethod]
	public void OrdinaryCellsCanProtrudePastTheRoofGuide()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var layout = Activator.CreateInstance(type, flags, null, [OldHotbed(), seed], null)!;
			type.GetMethod("GrowCells", flags)!.Invoke(layout, null);
			var cells = (System.Collections.IEnumerable)type.GetField("cells", flags)!.GetValue(layout)!;
			float surface = (float)type.GetProperty("PoolSurface", flags)!.GetValue(layout)!;
			int protrudingCells = 0;
			foreach (var cell in cells)
			{
				float radius = (float)cell.GetType().GetProperty("RadiusX")!.GetValue(cell)!;
				if (radius < 8)
				{
					continue;
				}
				var footprint = (IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!;
				if (footprint.Any(point => Math.Abs(point.X - 175) < 70 && point.Y < surface - 5
					&& point.Y > (float)type.GetMethod("ChamberRoof", flags)!.Invoke(layout, [(float)point.X])! + 4))
				{
					protrudingCells++;
				}
			}
			Assert.IsTrue(protrudingCells >= 2, $"Ordinary cells must cross the guide instead of fitting a smooth contour, seed {seed}.");
		}
	}

	[TestMethod]
	public void AcceptedCellsKeepTheirEntireFootprintsAtProtectedAndWorldBoundaries()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
		for (int seed = 0; seed < 12; seed++)
		{
			var source = OldHotbed();
			for (int x = 55; x < 70; x++)
			{
				for (int y = 365; y < 380; y++)
				{
					source[x, y] = 9;
				}
			}
			var layout = Activator.CreateInstance(type, flags, null, [source, seed], null)!;
			type.GetMethod("GrowCells", flags)!.Invoke(layout, null);
			var tiles = (byte[,])type.GetField("tiles", flags)!.GetValue(layout)!;
			var cells = (System.Collections.IEnumerable)type.GetField("cells", flags)!.GetValue(layout)!;
			int count = 0;
			foreach (var cell in cells)
			{
				count++;
				var footprint = (IEnumerable<(int X, int Y)>)type.GetMethod("CellFootprint", flags)!.Invoke(layout, [cell])!;
				foreach (var point in footprint)
				{
					Assert.IsTrue(point.X > 3 && point.X < 346 && point.Y > 3 && point.Y < 476, $"Cell crossed world margin: {point}, seed {seed}.");
					Assert.AreNotEqual((byte)9, source[point.X, point.Y], $"Cell crossed protected terrain: {point}, seed {seed}.");
					Assert.AreEqual((byte)2, tiles[point.X, point.Y], $"Accepted cell was clipped: {point}, seed {seed}.");
				}
			}
			Assert.IsTrue(count > 50, "Whole-cell placement must still build the lower cave.");
		}
	}

	[TestMethod]
	public void CavityAndLiquidPassesNeverCutPlacedCellSilhouettes()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		for (int seed = 0; seed < 12; seed++)
		{
			object layout = Activator.CreateInstance(type, BindingFlags.Instance | BindingFlags.NonPublic, null, [OldHotbed(), seed], null)!;
			type.GetMethod("GrowCells", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(layout, null);
			var tiles = (byte[,])type.GetField("tiles", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(layout)!;
			var before = (byte[,])tiles.Clone();
			foreach (string stage in new[] { "FitPoolFloor", "CarveChamber", "BuildPool" })
			{
				type.GetMethod(stage, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(layout, null);
				for (int x = 0; x < 350; x++)
				{
					for (int y = 0; y < 480; y++)
					{
						if (before[x, y] == 2)
						{
							Assert.IsTrue(tiles[x, y] is >= 2 and <= 5, $"{stage} cut cell tissue at {x},{y}, seed {seed}.");
						}
					}
				}
			}
		}
	}

	[TestMethod]
	public void ReferenceNervesPreserveTheLayoutAndProtectedMaterials()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		var trace = type.GetMethod("TraceReferenceNerves", BindingFlags.Public | BindingFlags.Static)!;
		var layout = Generate(OldHotbed(), 42);
		var before = (byte[,])layout.Clone();
		var nerves = (byte[,])trace.Invoke(null, [layout])!;
		CollectionAssert.AreEqual(before.Cast<byte>().ToArray(), layout.Cast<byte>().ToArray(), "The reference overlay must not regenerate or carve the biome.");
		int tissue = 0;
		for (int x = 0; x < 350; x++)
		{
			for (int y = 0; y < 480; y++)
			{
				if (nerves[x, y] == 0)
				{
					continue;
				}
				Assert.IsTrue(layout[x, y] is 2 or 5, "Nerves may replace only existing jelly tissue, never empty space, nuclei, membrane, honey or foreign terrain.");
				tissue += layout[x, y] is 2 or 5 ? 1 : 0;
			}
		}
		Assert.IsTrue(tissue > 200, "The solid tissue must retain its nerve curves.");
	}

	[TestMethod]
	public void ReferenceNerveSkeletonKeepsSolidCoverageButSkipsEmptySpace()
	{
		var type = typeof(YggdrasilTownGeneration).Assembly.GetType("Everglow.Yggdrasil.WorldGeneration.JellyBallHotbedLayout")!;
		var trace = type.GetMethod("TraceReferenceNerves", BindingFlags.Public | BindingFlags.Static)!;
		var solid = new byte[330, 486];
		var empty = new byte[330, 486];
		for (int x = 0; x < 330; x++)
		{
			for (int y = 0; y < 486; y++)
			{
				solid[x, y] = 2;
				empty[x, y] = 1;
			}
		}
		var nerves = (byte[,])trace.Invoke(null, [solid])!;
		var throughGaps = (byte[,])trace.Invoke(null, [empty])!;
		Assert.IsTrue(throughGaps.Cast<byte>().All(value => value == 0), "Empty cells must not receive nerves, including background nerves.");
		var points = new HashSet<(int X, int Y)>();
		for (int x = 0; x < 330; x++)
		{
			for (int y = 0; y < 486; y++)
			{
				if (nerves[x, y] != 0)
				{
					points.Add((x, y));
				}
			}
		}
		Assert.IsTrue(points.Count > 700);
		Assert.IsTrue(nerves.Cast<byte>().Contains((byte)1) && nerves.Cast<byte>().Contains((byte)2));
		int solidSquares = 0;
		for (int x = 0; x < 329; x++)
		{
			for (int y = 0; y < 485; y++)
			{
				if (nerves[x, y] != 0 && nerves[x + 1, y] != 0 && nerves[x, y + 1] != 0 && nerves[x + 1, y + 1] != 0)
				{
					solidSquares++;
				}
			}
		}
		Assert.IsTrue(solidSquares < points.Count * 0.05f, "Nerve strokes have become broad pixel bands instead of thin curves.");
		Assert.IsTrue(points.Max(point => point.X) - points.Min(point => point.X) > 250, "Reference nerves must cover the lower region instead of shrinking into its center.");
		Assert.IsTrue(points.Min(point => point.Y) < 270, "Upper nerves must reach the lower part of the old region.");
	}

	[TestMethod]
	public void GenerationIsReproducibleAndKeepsWorldMargins()
	{
		var source = OldHotbed();
		var first = Generate(source, 20);
		var again = Generate(source, 20);
		CollectionAssert.AreEqual(first.Cast<byte>().ToArray(), again.Cast<byte>().ToArray());
		CollectionAssert.AreEqual(OldHotbed().Cast<byte>().ToArray(), source.Cast<byte>().ToArray());
		for (int x = 0; x < 350; x++)
		{
			Assert.AreEqual(source[x, 479], first[x, 479]);
		}
		Assert.IsTrue(first.Cast<byte>().Contains((byte)3), "No crystals.");
	}
}
