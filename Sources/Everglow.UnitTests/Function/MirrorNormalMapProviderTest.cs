using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorNormalMapProviderTest
{
	[TestMethod]
	public void NormalMap_IsPeriodicUnitLengthAndPointsOutward()
	{
		Vector3 normal = MirrorNormalMapProvider.SampleNormal(0.25f, 0);
		Assert.AreEqual(0.4472136f, normal.X, 0.00001f);
		Assert.AreEqual(0.8944272f, normal.Z, 0.00001f);
		Assert.AreEqual(1f, normal.Length(), 0.00001f);
		Assert.IsTrue(Vector3.Distance(normal, MirrorNormalMapProvider.SampleNormal(1.25f, 1)) < 0.00001f);
	}

	[TestMethod]
	public void FunhouseNormals_ArePeriodicUnitLengthWithPositiveDepth()
	{
		foreach (MirrorNormalPattern pattern in Enum.GetValues<MirrorNormalPattern>())
		{
			for (int i = 0; i < 8; i++)
			{
				float u = i / 7f;
				float v = 1 - u;
				Vector3 normal = MirrorNormalMapProvider.SampleNormal(u, v, pattern);
				Assert.AreEqual(1f, normal.Length(), 0.00001f, pattern.ToString());
				Assert.IsTrue(normal.Z > 0.25f, pattern.ToString());
				Assert.IsTrue(Vector3.Distance(normal, MirrorNormalMapProvider.SampleNormal(u + 1, v - 2, pattern)) < 0.00001f, pattern.ToString());
			}
		}
	}

	[TestMethod]
	public void FunhouseWaves_DistortOnlyTheirSelectedAxis()
	{
		Vector3 horizontal = MirrorNormalMapProvider.SampleNormal(0.25f, 0.25f, MirrorNormalPattern.HorizontalWave);
		Vector3 vertical = MirrorNormalMapProvider.SampleNormal(0.25f, 0.25f, MirrorNormalPattern.VerticalWave);
		Assert.AreEqual(0f, horizontal.Y);
		Assert.IsTrue(horizontal.X > 0);
		Assert.AreEqual(0f, vertical.X);
		Assert.IsTrue(vertical.Y > 0);
	}

	[TestMethod]
	public void FunhouseRadialPatterns_SampleInwardOutwardAndTangentially()
	{
		Vector3 bulge = MirrorNormalMapProvider.SampleNormal(0.65f, 0.5f, MirrorNormalPattern.Bulge);
		Vector3 pinch = MirrorNormalMapProvider.SampleNormal(0.65f, 0.5f, MirrorNormalPattern.Pinch);
		Vector3 twist = MirrorNormalMapProvider.SampleNormal(0.65f, 0.5f, MirrorNormalPattern.Twist);
		Assert.IsTrue(bulge.X < 0);
		Assert.IsTrue(pinch.X > 0);
		Assert.AreEqual(-bulge.X, pinch.X, 0.00001f);
		Assert.AreEqual(0f, twist.X, 0.00001f);
		Assert.IsTrue(twist.Y > 0);
	}

	[TestMethod]
	public void FunhouseRadialPatterns_JoinWithoutSeamsAtCellEdges()
	{
		foreach (var pattern in new[] { MirrorNormalPattern.Bulge, MirrorNormalPattern.Pinch, MirrorNormalPattern.Twist })
		{
			Assert.IsTrue(Vector3.Distance(Vector3.UnitZ, MirrorNormalMapProvider.SampleNormal(0, 0.3f, pattern)) < 0.00001f);
			Assert.IsTrue(Vector3.Distance(Vector3.UnitZ, MirrorNormalMapProvider.SampleNormal(0.3f, 1, pattern)) < 0.00001f);
			Assert.IsTrue(Vector3.Distance(MirrorNormalMapProvider.SampleNormal(0.9999f, 0.3f, pattern), MirrorNormalMapProvider.SampleNormal(0.0001f, 0.3f, pattern)) < 0.00001f);
		}
	}
}
