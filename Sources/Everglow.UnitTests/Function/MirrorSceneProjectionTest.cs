using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorSceneProjectionTest
{
	[TestMethod]
	public void OrdinaryMirror_PreservesRelativePositionsInAlreadyZoomedScene()
	{
		Matrix view = Matrix.CreateScale(2, 2, 1) * Matrix.CreateTranslation(10, 20, 0);
		Matrix projection = MirrorMathTest.CreateScreenProjection(new(150, 100), Vector2.Zero, 1, false, Vector3.Zero, 480, view);
		Assert.AreEqual(new Vector2(210, 80), Vector2.Transform(new Vector2(210, 80), projection));
		Assert.AreEqual(new Vector2(410, 180), Vector2.Transform(new Vector2(410, 180), projection));
	}

	[TestMethod]
	public void SideMirror_SwapsObjectsAcrossCommonAnchor()
	{
		Matrix projection = MirrorMathTest.CreateScreenProjection(new(150, 100), Vector2.Zero, 1, true, Vector3.Zero, 480, Matrix.Identity);
		Assert.AreEqual(new Vector2(200, 80), Vector2.Transform(new Vector2(100, 80), projection));
		Assert.AreEqual(new Vector2(100, 80), Vector2.Transform(new Vector2(200, 80), projection));
	}

	[TestMethod]
	public void SceneOffset_IsInWorldPixelsWithoutApplyingCameraZoomTwice()
	{
		Matrix view = Matrix.CreateScale(2, 2, 1) * Matrix.CreateTranslation(10, 20, 0);
		Matrix projection = MirrorMathTest.CreateScreenProjection(new(150, 100), new(12, -5), 1, false, Vector3.Zero, 480, view);
		Assert.AreEqual(new Vector2(234, 70), Vector2.Transform(new Vector2(210, 80), projection));
	}

	[TestMethod]
	public void DefinitionVariants_ReplaceSelectedValuesWithoutMutatingTheirSource()
	{
		var original = new MirrorReflectionDefinition(0.4f, 0.8f) { Offset = new(18, 6), Blur = 2, Yaw = 15, FlipHorizontally = true };
		var variant = original with { Offset = new(-12, original.Offset.Y), Blur = 0, FlipHorizontally = false };
		Assert.AreEqual(new Vector2(-12, 6), variant.Offset);
		Assert.AreEqual(0f, variant.Blur);
		Assert.AreEqual(15f, variant.Yaw);
		Assert.IsFalse(variant.FlipHorizontally);
		Assert.AreEqual(0.4f, variant.Opacity);
		Assert.AreEqual(0.8f, variant.Strength);
		Assert.AreEqual(2f, original.Blur);
	}

	[TestMethod]
	public void OrdinaryDefinition_UsesExpectedDefaults()
	{
		var definition = new MirrorReflectionDefinition(1);
		Assert.AreEqual(new Vector2(12, 0), definition.Offset);
		Assert.AreEqual(1f, definition.ReflectionScale);
		Assert.AreEqual(0.5f, definition.Blur);
		Assert.AreEqual(0f, definition.NormalStrength);
		Assert.AreEqual(96f, definition.NormalPeriod);
		Assert.AreEqual(480f, definition.Distance);
		Assert.AreEqual(MirrorNormalPattern.Wave, definition.NormalPattern);
	}
}
