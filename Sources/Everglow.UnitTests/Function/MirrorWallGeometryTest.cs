using Everglow.Commons.TileHelper;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorWallGeometryTest
{
	[TestMethod]
	public void PlayerReflection_FlipsAboutPlayerCenterWithoutMovingFeet()
	{
		Matrix reflection = MirrorWallGeometry.CreatePlayerReflectionTransform(50, Matrix.Identity);
		Assert.AreEqual(new Vector2(60, 144), Vector2.Transform(new Vector2(40, 144), reflection));
		Assert.AreEqual(new Vector2(50, 144), Vector2.Transform(new Vector2(50, 144), reflection));
	}

	[TestMethod]
	public void PlayerReflection_AppliesCameraZoomAfterReflection()
	{
		Matrix view = Matrix.CreateScale(1.5f, 1.5f, 1) * Matrix.CreateTranslation(12, -8, 0);
		Matrix reflection = MirrorWallGeometry.CreatePlayerReflectionTransform(50, view);
		Assert.AreEqual(new Vector2(102, 97), Vector2.Transform(new Vector2(40, 70), reflection));
	}

	[TestMethod]
	public void PlayerReflection_PlayersUseTheirOwnCenter()
	{
		Matrix first = MirrorWallGeometry.CreatePlayerReflectionTransform(50, Matrix.Identity);
		Matrix second = MirrorWallGeometry.CreatePlayerReflectionTransform(150, Matrix.Identity);
		Assert.AreEqual(new Vector2(60, 144), Vector2.Transform(new Vector2(40, 144), first));
		Assert.AreEqual(new Vector2(160, 144), Vector2.Transform(new Vector2(140, 144), second));
	}

	[TestMethod]
	public void ReflectionSlice_RetainsEntireWallCellAtFootHeight()
	{
		Assert.IsTrue(MirrorWallGeometry.TryGetReflectionSlice(new(100, 144, 16, 16), new(0, 0, 800, 600), out var slice));
		Assert.AreEqual(new Rectangle(100, 144, 16, 16), slice);
	}

	[TestMethod]
	public void ReflectionSlice_ClipsAtScreenEdgeWithoutChangingSamplePosition()
	{
		Assert.IsTrue(MirrorWallGeometry.TryGetReflectionSlice(new(-8, 20, 16, 16), new(0, 0, 800, 600), out var slice));
		Assert.AreEqual(new Rectangle(0, 20, 8, 16), slice);
	}

	[TestMethod]
	public void ReflectionSlice_OffscreenWallIsNotDrawn()
	{
		Assert.IsFalse(MirrorWallGeometry.TryGetReflectionSlice(new(-32, 20, 16, 16), new(0, 0, 800, 600), out _));
	}
}
