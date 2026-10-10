using System.Reflection;
using Everglow.Yggdrasil;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Cil;
using MonoMod.Utils;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class YggdrasilMapMigrationTest
{
	private delegate ref T[,] CacheAccessor<T>(MapRenderer? renderer);

	private static CacheAccessor<T> Access<T>(string name) => typeof(YggdrasilWorld)
		.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!
		.CreateDelegate<CacheAccessor<T>>();

	[TestInitialize]
	public void Initialize()
	{
		Program.SavePath = string.Empty;
	}

	[TestMethod]
	[DataRow("checkMap")]
	[DataRow("DrawToMap")]
	[DataRow("CheckMapTargets")]
	public void MapCacheLoops_UseResizedDimensions(string methodName)
	{
		var method = typeof(MapRenderer).GetMethod(methodName, BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)!;
		using var definition = new DynamicMethodDefinition(method);
		using var context = new ILContext(definition.Definition);
		typeof(YggdrasilWorld).GetMethod("ResizeMapLoops", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, [context]);
		Assert.IsFalse(context.Body.Instructions.Any(instruction =>
			instruction.MatchLdsfld<MapRenderer>("numTargetsX") || instruction.MatchLdsfld<MapRenderer>("numTargetsY")));
	}
	[TestMethod]
	public void TallWorld_ExpandsEveryMapCacheWithoutCreatingGraphicsResources()
	{
		var targets = Access<RenderTarget2D>("MapTargets");
		var initialized = Access<bool>("MapInitialized");
		var contentLost = Access<bool>("MapContentLost");
		var queues = Access<List<Point16>>("MapChangeQueues");
		var oldTargets = targets(null);
		var oldInitialized = initialized(null);
		var oldContentLost = contentLost(null);
		var oldQueues = queues(null);
		const int oldEdgeRow = 1;
		var existingQueue = new List<Point16>();
		int oldWidth = Main.maxTilesX;
		int oldHeight = Main.maxTilesY;
		bool oldClear = Main.clearMap;
		bool oldRefresh = Main.refreshMap;
		try
		{
			targets(null) = new RenderTarget2D[5, 2];
			initialized(null) = new bool[5, 2];
			contentLost(null) = new bool[5, 2];
			queues(null) = new List<Point16>[5, 2];
			queues(null)[0, 0] = existingQueue;
			initialized(null)[0, oldEdgeRow] = true;
			Main.maxTilesX = 2000;
			Main.maxTilesY = 21000;
			typeof(YggdrasilWorld).GetMethod("EnsureMapCapacity", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, null);

			int requiredRows = Main.maxTilesY / MapRenderer.textureMaxHeight + 2;
			Assert.IsTrue(targets(null).GetLength(1) >= requiredRows);
			Assert.AreEqual(targets(null).GetLength(1), initialized(null).GetLength(1));
			Assert.AreEqual(targets(null).GetLength(1), contentLost(null).GetLength(1));
			Assert.AreEqual(targets(null).GetLength(1), queues(null).GetLength(1));
			Assert.AreSame(existingQueue, queues(null)[0, 0]);
			Assert.IsNotNull(queues(null)[0, requiredRows - 1]);
			Assert.IsNull(targets(null)[0, requiredRows - 1]);
			Assert.IsFalse(initialized(null)[0, oldEdgeRow], "The previous short edge texture must be recreated as a full interior target.");
			Assert.IsTrue(Main.clearMap && Main.refreshMap);
		}
		finally
		{
			targets(null) = oldTargets;
			initialized(null) = oldInitialized;
			contentLost(null) = oldContentLost;
			queues(null) = oldQueues;
			Main.maxTilesX = oldWidth;
			Main.maxTilesY = oldHeight;
			Main.clearMap = oldClear;
			Main.refreshMap = oldRefresh;
		}
	}
}
