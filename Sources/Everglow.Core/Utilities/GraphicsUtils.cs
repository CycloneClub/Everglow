using Everglow.Commons.DataStructures;
using Terraria;
using Terraria.Graphics;

namespace Everglow.Commons.Utilities;

public static class GraphicsUtils
{
	private static Matrix ScreenProjectionMatrix => Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);

	private static Matrix ScreenTranslationMatrix => Matrix.CreateTranslation(new(-Main.screenPosition, 0));

	/// <summary>
	/// Transforming world coordinates into screen coordinates before applying the <see cref="SpriteViewMatrix.TransformationMatrix"/>.
	/// </summary>
	/// <param name="spriteViewMatrix"></param>
	/// <returns></returns>
	public static Matrix TransformationMatrix_WorldToScreen(this SpriteViewMatrix spriteViewMatrix) => ScreenTranslationMatrix * spriteViewMatrix.TransformationMatrix * ScreenProjectionMatrix;

	public static void Begin(this SpriteBatch spriteBatch, SpriteBatchState state)
	{
		spriteBatch.Begin(
			state.SortMode,
			state.BlendState,
			state.SamplerState,
			state.DepthStencilState,
			state.RasterizerState,
			state.Effect,
			state.TransformMatrix);
	}

	public static SpriteBatchState? GetState(this SpriteBatch spriteBatch)
	{
		if (!spriteBatch.beginCalled)
		{
			return null;
		}
		return new SpriteBatchState(
			spriteBatch.sortMode,
			spriteBatch.blendState,
			spriteBatch.samplerState,
			spriteBatch.depthStencilState,
			spriteBatch.rasterizerState,
			spriteBatch.transformMatrix,
			spriteBatch.customEffect);
	}

	/// <summary>
	/// 根据输入点的List获得一个使用CatmullRom样条平滑过后的路径
	/// </summary>
	/// <param name="origPath"></param>
	/// <param name="precision">
	/// null : 根据角度差自动适配取点个数<br/>
	/// not null ：最少为2
	/// </param>
	/// <returns></returns>
	public static List<Vector2> CatmullRom(IEnumerable<Vector2> origPath, int? precision = null)
	{
		int count = origPath.Count();
		if (count <= 2)
		{
			return origPath.ToList();
		}

		var path = new Vector2[count + 2];
		var it = origPath.GetEnumerator();
		int index = 0;
		while (it.MoveNext())
		{
			path[++index] = it.Current;
		}

		// 头尾增加两个不影响曲线效果的点
		path[0] = path[1] * 2 - path[2];
		path[^1] = path[^2] * 2 - path[^3];

		List<Vector2> result = new(count * 3);

		for (int i = 1; i < count; i++)
		{
			float rotCurrent = new Rotation(path[i] - path[i - 1]).Radian;
			float rotNext = new Rotation(path[i + 2] - path[i + 1]).Radian;
			int dom;
			if (precision is null)
			{
				// 根据当前和下一个节点所代表的向量的旋转差异来增加采样数量 如果旋转差异越大，采样数量就越大
				if (float.IsNaN(rotCurrent) || float.IsNaN(rotNext))
				{
					dom = 2;
				}
				else
				{
					float dis = Math.Abs(rotCurrent - rotNext);
					dom = (int)((dis >= MathHelper.Pi ? MathHelper.TwoPi - dis : dis) / 0.22f + 2);
				}
			}
			else
			{
				dom = precision.Value;
			}
			float factor = 1.0f / dom;
			for (float j = 0; j < 1.0f; j += factor)
			{
				result.Add(Vector2.CatmullRom(path[i - 1], path[i], path[i + 1], path[i + 2], j));
			}
		}
		result.Add(path[^2]);
		return result;
	}

	/// <summary>
	/// Use <see cref="CatmullRom(IEnumerable{Vector2}, int?)"/> to smooth a list of <see cref="Vector2"/>.
	/// </summary>
	/// <param name="vectors"></param>
	/// <returns><c>null</c> if the result is too short to draw.</returns>
	public static List<Vector2> Smooth(this IEnumerable<Vector2> vectors)
	{
		List<Vector2> smoothedTrailVecs = GraphicsUtils.CatmullRom(vectors);
		var smoothedTrail = smoothedTrailVecs[..^1];
		if (vectors.Any())
		{
			smoothedTrail.Add(vectors.Last());
		}

		int length = smoothedTrail.Count;
		if (length <= 3)
		{
			return null;
		}

		return smoothedTrail;
	}

	/// <summary>
	/// Use <see cref="CatmullRom(IEnumerable{Vector2}, int?)"/> to smooth a list of <see cref="Vector2"/>.
	/// </summary>
	/// <param name="vectors"></param>
	/// <param name="result">The smoothed path, or <c>null</c> if it is too short to draw.</param>
	/// <returns>Whether the smoothed path is long enough to draw.</returns>
	public static bool Smooth(this IEnumerable<Vector2> vectors, out List<Vector2> result)
	{
		result = Smooth(vectors);
		return result is not null;
	}

	/// <summary>
	/// 根据输入点的List获得一个使用CatmullRom样条平滑过后的路径
	/// </summary>
	/// <param name="origPath"></param>
	/// <param name="precision">
	/// null : 根据角度差自动适配取点个数<br/>
	/// not null ：最少为2
	/// </param>
	/// <returns></returns>
	public static List<Vector3> CatmullRom(IEnumerable<Vector3> origPath, int? precision = null)
	{
		int count = origPath.Count();
		if (count <= 2)
		{
			return origPath.ToList();
		}

		var path = new Vector3[count + 2];
		var it = origPath.GetEnumerator();
		int index = 0;
		while (it.MoveNext())
		{
			path[++index] = it.Current;
		}

		// 头尾增加两个不影响曲线效果的点
		path[0] = path[1] * 2 - path[2];
		path[^1] = path[^2] * 2 - path[^3];

		List<Vector3> result = new(count * 3);

		for (int i = 1; i < count; i++)
		{
			Vector3 dirCurrent = Vector3.Normalize(path[i] - path[i - 1]);
			Vector3 dirNext = Vector3.Normalize(path[i + 2] - path[i + 1]);

			float angle = Vector3.Dot(dirCurrent, dirNext);

			// 确保 angle 在 -1 到 1 之间
			angle = MathHelper.Clamp(angle, -1f, 1f);

			float rotCurrent = MathF.Acos(angle); // 计算夹角
			float rotNext = rotCurrent; // 根据需要，你可能想用不同的方法计算 rotNext

			int dom;

			if (precision is null)
			{
				// 根据当前和下一个节点所代表的向量的旋转差异来增加采样数量
				if (float.IsNaN(rotCurrent) || float.IsNaN(rotNext))
				{
					dom = 2;
				}
				else
				{
					float dis = Math.Abs(rotCurrent - rotNext);
					dom = (int)((dis >= MathHelper.Pi ? MathHelper.TwoPi - dis : dis) / 0.22f + 2);
				}
			}
			else
			{
				dom = precision.Value;
			}
			float factor = 1.0f / dom;
			for (float j = 0; j < 1.0f; j += factor)
			{
				result.Add(Vector3.CatmullRom(path[i - 1], path[i], path[i + 1], path[i + 2], j));
			}
		}
		result.Add(path[^2]);
		return result;
	}

	/// <summary>
	/// Use <see cref="CatmullRom(IEnumerable{Vector3}, int?)"/> to smooth a list of <see cref="Vector3"/>.
	/// </summary>
	/// <param name="vectors"></param>
	/// <returns><c>null</c> if the result is too short to draw.</returns>
	public static List<Vector3> Smooth(this IEnumerable<Vector3> vectors)
	{
		List<Vector3> smoothedTrailVecs = GraphicsUtils.CatmullRom(vectors);
		var smoothedTrail = smoothedTrailVecs[..^1];
		if (vectors.Any())
		{
			smoothedTrail.Add(vectors.Last());
		}

		int length = smoothedTrail.Count;
		if (length <= 3)
		{
			return null;
		}

		return smoothedTrail;
	}

	/// <summary>
	/// Use <see cref="CatmullRom(IEnumerable{Vector3}, int?)"/> to smooth a list of <see cref="Vector3"/>.
	/// </summary>
	/// <param name="vectors"></param>
	/// <param name="result">The smoothed path, or <c>null</c> if it is too short to draw.</param>
	/// <returns>Whether the smoothed path is long enough to draw.</returns>
	public static bool Smooth(this IEnumerable<Vector3> vectors, out List<Vector3> result)
	{
		result = Smooth(vectors);
		return result is not null;
	}

	/// <summary>
	/// 根据输入点的List获得一条贝塞尔曲线
	/// </summary>
	/// <param name="origPath"></param>
	/// <param name="precision">最少为2 ,不建议超过100</param>
	/// <returns></returns>
	/// <exception cref="ArgumentOutOfRangeException">If input contains element that <see cref="float.IsNaN(float)"/> or <see cref="float.IsInfinity(float)"/></exception>
	public static List<Vector2> BezierCurve(IEnumerable<Vector2> origPath, int precision = 10)
	{
		int count = origPath.Count() - 1;
		if (count <= 2)
		{
			return origPath.ToList();
		}
		if (precision < 2)
		{
			precision = 2;
		}

		float factor = 1.00f / (precision * count);
		List<Vector2> result = [];
		var p = origPath.ToArray();
		for (float t = 0; t < 1.0f; t += factor)
		{
			result.Add(GetBezierPoint(t, p));
		}

		return result;
	}

	private static Vector2 GetBezierPoint(float t, Vector2[] p)
	{
		int n = p.Length - 1;
		float u = 1 - t;

		Vector2 result = Vector2.Zero;

		for (int i = 0; i < n; i++)
		{
			var current = p[i];
			if (float.IsNaN(current.X) || float.IsNaN(current.Y) || float.IsInfinity(current.X) || float.IsInfinity(current.Y))
			{
				throw new ArgumentOutOfRangeException(nameof(p), "Bezier curve points cannot contain NaN or Infinity values.");
			}

			result += current * MathF.Pow(u, n - i) * MathF.Pow(t, i) * MathUtils.Combination(n, i);
		}
		return result;
	}
}
