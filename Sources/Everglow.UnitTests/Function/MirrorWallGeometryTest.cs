using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorWallGeometryTest
{
	[TestMethod]
	public void ScreenRectangle_AdjacentWallsShareEdgesAtFractionalZoom()
	{
		Matrix view = Matrix.CreateScale(1.3f, 1.3f, 1) * Matrix.CreateTranslation(7.5f, -8.25f, 0);
		Vector2 camera = new(3.25f, 4.75f);
		Rectangle first = MirrorMathTest.GetScreenRectangle(new(160, 320), new(16), camera, view);
		Rectangle right = MirrorMathTest.GetScreenRectangle(new(176, 320), new(16), camera, view);
		Rectangle below = MirrorMathTest.GetScreenRectangle(new(160, 336), new(16), camera, view);
		Assert.AreEqual(first.Right, right.Left);
		Assert.AreEqual(first.Bottom, below.Top);
		Assert.AreEqual(first.Top, right.Top);
		Assert.AreEqual(first.Left, below.Left);
	}
}
