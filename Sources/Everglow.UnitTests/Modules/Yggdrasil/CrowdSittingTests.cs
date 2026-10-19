using System.Reflection;
using Everglow.Yggdrasil.Mechanics.TownCrowd;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
[DoNotParallelize]
public class CrowdSittingTests
{
	[TestInitialize]
	public void Initialize() => Program.SavePath = string.Empty;

	[TestMethod]
	[DataRow(-1, -4f)]
	[DataRow(1, -4f)]
	[DataRow(1, 0f)]
	public void LightweightLegSlicesMatchInstalledTmlDrawSittingLegs(int direction, float seatOffset)
	{
		var method = typeof(CrowdAppearance).GetMethod("SittingLegStrip", BindingFlags.Static | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		var player = new Player { width = 16, height = 48, direction = direction, legFrame = new Rectangle(0, 0, 40, 56) };
		var position = new Vector2(100, 200);
		var draw = new PlayerDrawSet
		{
			drawPlayer = player,
			Position = position - new Vector2(8, 0) + Main.screenPosition,
			legVect = new Vector2(20, 28),
			seatYOffset = seatOffset,
			DrawDataCache = [],
		};
		object?[] native = [draw, null, Color.White, 0, false, null];
		typeof(Terraria.DataStructures.PlayerDrawLayers).GetMethod("DrawSittingLegs", BindingFlags.Static | BindingFlags.NonPublic)!.Invoke(null, native);
		var expected = ((PlayerDrawSet)native[0]!).DrawDataCache;
		Assert.AreEqual(3, expected.Count);
		for (int index = 0; index < 3; index++)
		{
			object?[] args = [position, direction, 2 - index, seatOffset, null, null];
			method.Invoke(null, args);
			Assert.AreEqual(expected[index].sourceRect, (Rectangle)args[4]!);
			Assert.AreEqual(expected[index].position - expected[index].origin,
				(Vector2)args[5]! - new Vector2(20, 0));
		}
	}
}
