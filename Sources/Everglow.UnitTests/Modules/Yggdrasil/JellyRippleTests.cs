using System.Reflection;
using Everglow.Yggdrasil.YggdrasilTown.Tiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class JellyRippleTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(1, 1)]
	[DataRow(-1, 1)]
	[DataRow(1, -1)]
	[DataRow(-1, -1)]
	public void MixedStaircaseSharesItsInsideCornerBeforeTheWaveReachesBothArms(int dx, int dy)
	{
		var elbow = new Point(20, 20);
		var mainNerve = new Point(elbow.X + dx, elbow.Y);
		var branch = new Point(elbow.X, elbow.Y + dy);
		var corner = new Point(Math.Max(elbow.X, mainNerve.X), Math.Max(elbow.Y, branch.Y));
		Point[] points = [elbow, mainNerve, branch, new Point(mainNerve.X, branch.Y)];
		ModTile[] tiles = [new JellyBallSecretion(), new JellyBallNerve(), new JellyBallNerve_Thin()];
		var instances = tiles.Select(t => typeof(ContentInstance<>).MakeGenericType(t.GetType()).GetProperty("Instance")!).ToArray();
		var oldInstances = instances.Select(p => p.GetValue(null)).ToArray();
		var oldStates = points.Select(p => Main.tile[p.X, p.Y].Get<TileWallWireStateData>()).ToArray();
		var oldTypes = points.Select(p => Main.tile[p.X, p.Y].TileType).ToArray();
		var oldTick = Main.GameUpdateCount;
		var tick = typeof(Main).GetField("_gameUpdateCount", BindingFlags.Static | BindingFlags.NonPublic)!;
		try
		{
			for (int i = 0; i < points.Length; i++)
			{
				var tile = Main.tile[points[i].X, points[i].Y];
				tile.Get<TileWallWireStateData>() = default;
				if (i < tiles.Length)
				{
					typeof(ModBlockType).GetProperty("Type")!.SetValue(tiles[i], (ushort)(100 + i));
					instances[i].SetValue(null, tiles[i]);
					tile.TileType = tiles[i].Type;
					tile.HasTile = true;
				}
			}

			var ripples = new JellyRipples();
			const BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
			var emit = typeof(JellyRipples).GetMethod("Emit", flags)!.CreateDelegate<Action<Point, Point, float>>(ripples);
			emit(mainNerve, new Point(0, 1), 8f);
			tick.SetValue(null, oldTick + 1u);
			var getOffset = typeof(JellyRipples).GetMethod("GetCornerOffset", flags);
			Assert.IsNotNull(getOffset, "The three tiles must sample the same deformable corner.");
			var offset = getOffset.CreateDelegate<Func<Point, Point, Vector2>>(ripples);
			var displacement = offset(elbow, corner);
			Assert.AreNotEqual(Vector2.Zero, displacement);
			Assert.AreEqual(displacement, offset(mainNerve, corner));
			Assert.AreEqual(displacement, offset(branch, corner));

			ripples.PostUpdateEverything();
			tick.SetValue(null, oldTick + 2u);
			ripples.PostUpdateEverything();
			tick.SetValue(null, oldTick + 3u);
			var wave = typeof(JellyRipples).GetMethod("TryGetWave", flags)!;
			Assert.IsTrue((bool)wave.Invoke(ripples, [branch, Vector2.Zero])!);
		}
		finally
		{
			tick.SetValue(null, oldTick);
			for (int i = 0; i < points.Length; i++)
			{
				var tile = Main.tile[points[i].X, points[i].Y];
				tile.Get<TileWallWireStateData>() = oldStates[i];
				tile.TileType = oldTypes[i];
			}

			for (int i = 0; i < instances.Length; i++)
			{
				instances[i].SetValue(null, oldInstances[i]);
			}
		}
	}
}
