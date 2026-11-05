using System.Reflection;
using Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Modules.Yggdrasil;

[TestClass]
public class VampireMatBallRenderingTests
{
	[TestMethod]
	public void FullGlowRemainsVisibleBeyondEveryViewportEdge()
	{
		Assert.IsTrue(IsVisible(new Vector2(-99, 90), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsTrue(IsVisible(new Vector2(419, 90), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsTrue(IsVisible(new Vector2(160, -99), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsTrue(IsVisible(new Vector2(160, 279), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(-102, 90), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(422, 90), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(160, -102), 1, new Vector2(200), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(160, 282), 1, new Vector2(200), 1, Matrix.Identity));
	}

	[TestMethod]
	public void BallFrameIsIncludedWhenGlowIsNarrower()
	{
		Assert.IsTrue(IsVisible(new Vector2(-44, 90), 1, new Vector2(20), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(-47, 90), 1, new Vector2(20), 1, Matrix.Identity));
	}

	[TestMethod]
	public void VortexGlowUsesItsDoubledScale()
	{
		Assert.IsTrue(IsVisible(new Vector2(-39, 90), 0.4f, new Vector2(100), 2, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(-39, 90), 0.4f, new Vector2(100), 1, Matrix.Identity));
		Assert.IsFalse(IsVisible(new Vector2(-42, 90), 0.4f, new Vector2(100), 2, Matrix.Identity));
	}

	[TestMethod]
	public void ZoomAroundViewportCenterChangesVisibleWorldRange()
	{
		Matrix zoomOut = Matrix.CreateTranslation(-160, -90, 0) * Matrix.CreateScale(0.5f, 0.5f, 1) * Matrix.CreateTranslation(160, 90, 0);
		Assert.IsTrue(IsVisible(new Vector2(-80, 90), 1, new Vector2(20), 1, zoomOut));
		Assert.IsFalse(IsVisible(new Vector2(-300, 90), 1, new Vector2(20), 1, zoomOut));
		Matrix zoomIn = Matrix.CreateTranslation(-160, -90, 0) * Matrix.CreateScale(2, 2, 1) * Matrix.CreateTranslation(160, 90, 0);
		Assert.IsFalse(IsVisible(new Vector2(-10, 90), 1, new Vector2(20), 1, zoomIn));
	}

	[TestMethod]
	public void FlippedCameraTransformsCenterAndPreservesPositiveExtents()
	{
		Matrix flipped = Matrix.CreateScale(-1, -1, 1) * Matrix.CreateTranslation(320, 180, 0);
		Assert.IsTrue(IsVisible(new Vector2(350, 210), 1, new Vector2(20), 1, flipped));
		Assert.IsFalse(IsVisible(new Vector2(400, 90), 1, new Vector2(20), 1, flipped));
	}

	[TestMethod]
	public void RotatedCameraIncludesTheFullNonSquareGlow()
	{
		Matrix rotated = Matrix.CreateRotationZ(MathHelper.PiOver2);
		Assert.IsTrue(IsVisible(new Vector2(100, 70), 1, new Vector2(40, 200), 1, rotated));
		Assert.IsFalse(IsVisible(new Vector2(100, 110), 1, new Vector2(40, 200), 1, rotated));
	}

	[TestMethod]
	public void CameraTranslationIsAppliedBeforeViewportTest()
	{
		Matrix translated = Matrix.CreateTranslation(1000, 0, 0);
		Assert.IsTrue(IsVisible(new Vector2(-900, 90), 1, new Vector2(20), 1, translated));
		Assert.IsFalse(IsVisible(new Vector2(100, 90), 1, new Vector2(20), 1, translated));
	}

	private static bool IsVisible(Vector2 screenCenter, float scale, Vector2 starSize, float starScaleMultiplier, Matrix transform)
	{
		Type? helper = typeof(VampireMat_Attack_Proj_Ball_Small_Group).Assembly.GetType("Everglow.Yggdrasil.KelpCurtain.Projectiles.Enemies.VampireMatBallRendering");
		Assert.IsNotNull(helper, "Ball rendering needs a shared visibility test for its complete transformed sprite bounds.");
		MethodInfo? method = helper.GetMethod("IsVisible", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		Assert.IsNotNull(method);
		return (bool)method.Invoke(null, [screenCenter, scale, starSize, starScaleMultiplier, transform, 320, 180])!;
	}
}
