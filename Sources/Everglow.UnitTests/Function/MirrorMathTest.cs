using System.Reflection;
using Everglow.Commons.Mechanics.MirrorReflection.Core;
using Microsoft.Xna.Framework;

namespace Everglow.UnitTests.Function;

[TestClass]
public class MirrorMathTest
{
	// 直接绑定 Renderer 的纯计算方法，保留测试且不扩大生产代码的可见范围。
	internal static readonly Func<Vector2, Vector2, float, bool, Vector3, float, Matrix, Matrix> CreateScreenProjection =
		Bind<Func<Vector2, Vector2, float, bool, Vector3, float, Matrix, Matrix>>("CreateScreenProjection");

	internal static readonly Func<float, int, float[]> CreateGaussianKernel =
		Bind<Func<float, int, float[]>>("CreateGaussianKernel");

	internal static readonly Func<Vector2, Vector2, Vector2, Matrix, Rectangle> GetScreenRectangle =
		Bind<Func<Vector2, Vector2, Vector2, Matrix, Rectangle>>("GetScreenRectangle");

	[TestMethod]
	public void GaussianKernel_IsNormalizedSymmetricAndPeaksAtCenter()
	{
		float[] kernel = CreateGaussianKernel(1.5f, 9);
		Assert.AreEqual(1f, kernel.Sum(), 0.00001f);
		for (int i = 0; i < 4; i++)
		{
			Assert.AreEqual(kernel[i], kernel[8 - i], 0.00001f);
			Assert.IsTrue(kernel[i] < kernel[i + 1]);
		}
	}

	[TestMethod]
	[DataRow(3)]
	[DataRow(5)]
	[DataRow(9)]
	public void GaussianQuality_PreservesEnergySymmetryAndShaderBufferSize(int sampleCount)
	{
		float[] kernel = CreateGaussianKernel(1.5f, sampleCount);
		Assert.AreEqual(9, kernel.Length);
		Assert.AreEqual(1f, kernel.Sum(), 0.00001f);
		for (int i = 0; i < sampleCount; i++)
		{
			Assert.IsTrue(kernel[i] > 0);
			Assert.AreEqual(kernel[i], kernel[sampleCount - 1 - i], 0.00001f);
		}
		Assert.IsTrue(kernel.Skip(sampleCount).All(weight => weight == 0));
	}

	[TestMethod]
	public void GaussianQuality_RejectsUnsupportedSampleBudgets()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CreateGaussianKernel(1.5f, 1));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CreateGaussianKernel(1.5f, 4));
	}

	[TestMethod]
	public void GaussianKernel_WiderSigmaSpreadsWeight()
	{
		float[] sharp = CreateGaussianKernel(0.5f, 9);
		float[] soft = CreateGaussianKernel(3, 9);
		Assert.IsTrue(sharp[4] > soft[4]);
		Assert.IsTrue(sharp[0] < soft[0]);
	}

	[TestMethod]
	public void GaussianKernel_RejectsInvalidSigma()
	{
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CreateGaussianKernel(0, 9));
		Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => CreateGaussianKernel(float.NaN, 9));
	}

	[TestMethod]
	public void Projection_ZeroAnglesPreserveOldTuningAndFootAnchor()
	{
		Matrix matrix = CreateScreenProjection(new(50, 144), new(12, -10), 1.5f, true, Vector3.Zero, 480, Matrix.Identity);
		Assert.AreEqual(new Vector4(77, 104, 0, 1), Vector4.Transform(new Vector4(40, 124, 0, 1), matrix));
		Assert.AreEqual(new Vector4(62, 134, 0, 1), Vector4.Transform(new Vector4(50, 144, 0, 1), matrix));
	}

	[TestMethod]
	public void Projection_YawCreatesPerspectiveWithoutMovingAnchor()
	{
		Matrix matrix = CreateScreenProjection(Vector2.Zero, Vector2.Zero, 1, false, new(0, MathHelper.Pi / 3, 0), 480, Matrix.Identity);
		Vector4 projected = Vector4.Transform(new Vector4(100, 0, 0, 1), matrix);
		Assert.AreEqual(50f, projected.X, 0.0001f);
		Assert.AreEqual(1.18042195f, projected.W, 0.00001f);
		Assert.AreEqual(Vector4.UnitW, Vector4.Transform(Vector4.UnitW, matrix));
	}

	private static T Bind<T>(string name)
		where T : Delegate
		=> typeof(MirrorReflectionDefinition).Assembly
			.GetType("Everglow.Commons.Mechanics.MirrorReflection.Core.MirrorReflectionRenderer", throwOnError: true)!
			.GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!
			.CreateDelegate<T>();
}
