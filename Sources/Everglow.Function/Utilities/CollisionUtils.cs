using Everglow.Commons.Physics.Colliders;
using Everglow.Commons.Physics.DataStructures;
using Everglow.Commons.Physics.Enums;
using Everglow.Commons.Utilities;

namespace Everglow.Commons.Utilities;

public static partial class CollisionUtils
{
	public const float Epsilon = 1e-4f;

	public static AABB ToAABB(this Edge line)
	{
		var min = Vector2.Min(line.begin, line.end);
		var max = Vector2.Max(line.begin, line.end);
		return new AABB(min, max - min);
	}

	public static AABB ToAABB(this List<Vector2> vertices)
	{
		var min = new Vector2(float.MaxValue, float.MaxValue);
		var max = new Vector2(float.MinValue, float.MinValue);
		foreach (var v in vertices)
		{
			min = Vector2.Min(min, v);
			max = Vector2.Max(max, v);
		}
		return new AABB(min, max - min);
	}

	public static AABB ToAABB(this Rectangle rectangle) => new(rectangle.TopLeft(), rectangle.Size());

	/// <summary>
	/// 判断两个AABB是否相交，边缘重叠视为相交
	/// </summary>
	/// <param name="a"></param>
	/// <param name="b"></param>
	/// <param name="allowEdge">为 true 时允许边缘接触而不判为相交；为 false 时仅边缘接触也判为相交。</param>
	/// <returns></returns>
	public static bool Intersect(this AABB a, AABB b, bool allowEdge = false)
	{
		if (allowEdge)
		{
			if (a.position.X >= b.position.X + b.size.X || a.position.X + a.size.X <= b.position.X ||
			a.position.Y >= b.position.Y + b.size.Y || a.position.Y + a.size.Y <= b.position.Y)
			{
				return false;
			}
			return true;
		}
		else
		{
			if (a.position.X > b.position.X + b.size.X || a.position.X + a.size.X < b.position.X ||
			a.position.Y > b.position.Y + b.size.Y || a.position.Y + a.size.Y < b.position.Y)
			{
				return false;
			}
			return true;
		}
	}

	public static bool Intersect(this AABB a, AABB b, out AABB area)
	{
		if (a.position.X > b.position.X + b.size.X || a.position.X + a.size.X < b.position.X ||
			a.position.Y > b.position.Y + b.size.Y || a.position.Y + a.size.Y < b.position.Y)
		{
			area = default;
			return false;
		}
		var topLeft = Vector2.Max(a.TopLeft, b.TopLeft);
		var bottomRight = Vector2.Min(a.BottomRight, b.BottomRight);
		area = new AABB(topLeft, bottomRight - topLeft);
		return true;
	}

	public static bool Intersect(this Edge a, Edge b, out float factor)
	{
		if (!a.ToAABB().Intersect(b.ToAABB()))
		{
			factor = 0;
			return false;
		}

		factor = (b.end - b.begin).Cross(a.begin - b.begin);
		float u = (a.end - a.begin).Cross(a.begin - b.begin);
		float denom = (b.end - b.begin).Cross(a.end - a.begin);
		if (Math.Abs(denom) < Epsilon)
		{
			return false;
		}

		factor /= denom;
		u /= denom;

		return factor >= 0 && factor <= 1 && u >= 0 && u <= 1;
	}

	public static bool Intersect(this Edge a, Edge b) => Intersect(a, b, out _);

	public static bool Intersect(this Edge line, AABB aabb)
	{
		if (aabb.Contain(line.begin) || aabb.Contain(line.end))
		{
			return true;
		}

		if (line.Intersect(new Edge(aabb.TopLeft, aabb.BottomRight)) || line.Intersect(new Edge(aabb.TopRight, aabb.BottomLeft)))
		{
			return true;
		}

		return false;
	}

	public static bool Intersect(this AABB aabb, Edge line)
	{
		if (aabb.Contain(line.begin) || aabb.Contain(line.end))
		{
			return true;
		}

		if (line.Intersect(new Edge(aabb.TopLeft, aabb.BottomRight)) || line.Intersect(new Edge(aabb.TopRight, aabb.BottomLeft)))
		{
			return true;
		}

		return false;
	}

	public static bool Intersect(this Edge edge, AABB aabb, out Array2<Direction> directions)
	{
		directions = new Array2<Direction>();
		var it = directions.GetEnumerator();
		it.MoveNext();
		bool result = false;
		if (new Edge(aabb.TopLeft, aabb.TopRight).Intersect(edge))
		{
			it.Current = Direction.Top;
			if (!it.MoveNext())
			{
				return true;
			}

			result = true;
		}
		if (new Edge(aabb.TopRight, aabb.BottomRight).Intersect(edge))
		{
			it.Current = Direction.Right;
			if (!it.MoveNext())
			{
				return true;
			}

			result = true;
		}
		if (new Edge(aabb.BottomRight, aabb.BottomLeft).Intersect(edge))
		{
			it.Current = Direction.Bottom;
			if (!it.MoveNext())
			{
				return true;
			}

			result = true;
		}
		if (new Edge(aabb.BottomLeft, aabb.TopLeft).Intersect(edge))
		{
			it.Current = Direction.Left;
			if (!it.MoveNext())
			{
				return true;
			}

			result = true;
		}
		return result;
	}

	public static bool Intersect(this Circle circle, AABB aabb)
	{
		Vector2 or = (circle.position - aabb.Center).Abs();
		if (or.X > aabb.Width / 2)
		{
			if (or.Y > aabb.Height / 2)
			{
				return or.Distance(aabb.size / 2) <= circle.radius;
			}
			else
			{
				return or.X - aabb.Width / 2 <= circle.radius;
			}
		}
		else
		{
			return or.Y - aabb.Height / 2 <= circle.radius;
		}
	}

	public static bool Intersect(this Circle circle, AABB aabb, out Direction dir)
	{
		Vector2 or = circle.position - aabb.Center;
		bool flipX = or.X < 0;
		bool flipY = or.Y < 0;
		or = or.Abs();
		dir = Direction.None;
		bool result;
		if (or.X > aabb.Width / 2)
		{
			if (or.Y > aabb.Height / 2)
			{
				result = or.Distance(aabb.size / 2) <= circle.radius;
				if (result)
				{
					dir = (flipX, flipY) switch
					{
						(true, true) => Direction.TopLeft,
						(false, true) => Direction.TopRight,
						(true, false) => Direction.BottomLeft,
						(false, false) => Direction.BottomRight,
					};
				}
				return result;
			}
			else
			{
				result = or.X - aabb.Width / 2 <= circle.radius;
				if (result)
				{
					dir = flipX ? Direction.Left : Direction.Right;
				}

				return result;
			}
		}
		else
		{
			result = or.Y - aabb.Height / 2 <= circle.radius;
			if (result)
			{
				dir = flipY ? Direction.Top : Direction.Bottom;
			}

			return result;
		}
	}

	public static bool Intersect(float u0, float v0, float u1, float v1)
	{
		return !(u1 > v0 || v1 < u0);
	}

	/// <summary>
	/// 两个斜矩形
	/// </summary>
	/// <param name="p1"></param>
	/// <param name="p2"></param>
	/// <param name="line1Width"></param>
	/// <param name="p3"></param>
	/// <param name="p4"></param>
	/// <param name="line2Width"></param>
	/// <returns></returns>
	public static bool Intersect(Vector2 p1, Vector2 p2, float line1Width, Vector2 p3, Vector2 p4, float line2Width)
	{
		// 计算四边形的四条边
		Vector2 width1 = (p2 - p1).SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * line1Width / 2f;
		Vector2 width2 = (p4 - p3).SafeNormalize(Vector2.zeroVector).RotatedBy(MathHelper.PiOver2) * line2Width / 2f;
		Vector2[] lines1 = { p1 - width1, p2 - width1, p2 + width1, p1 + width1 };
		Vector2[] lines2 = { p3 - width2, p4 - width2, p4 + width2, p3 + width2 };

		// 检查每条边是否与另一矩形相交
		for (int i = 0; i < lines1.Length; i++)
		{
			for (int j = 0; j < lines2.Length; j++)
			{
				if (MathUtils.DoLineSegmentsIntersectByAlgebra(lines1[i], lines1[(i + 1) % lines1.Length], lines2[j], lines2[(j + 1) % lines2.Length]))
				{
					return true;
				}
			}
		}

		// 判定是否存在包含
		for (int i = 0; i < lines1.Length; i++)
		{
			double totalRad = 0;
			for (int j = 0; j < lines2.Length; j++)
			{
				Vector2 side1 = lines2[j] - lines1[i];
				Vector2 side2 = lines2[(j + 1) % lines2.Length] - lines1[i];
				double cosTheta = Vector2.Dot(side1, side2) / (side2.Length() * side1.Length());
				double rad = Math.Acos(cosTheta);
				totalRad += rad;
			}
			if (totalRad > 6.2831)
			{
				return true;
			}
		}
		for (int j = 0; j < lines1.Length; j++)
		{
			double totalRad = 0;
			for (int i = 0; i < lines2.Length; i++)
			{
				Vector2 side1 = lines1[i] - lines2[j];
				Vector2 side2 = lines1[(i + 1) % lines1.Length] - lines2[j];
				double cosTheta = Vector2.Dot(side1, side2) / (side2.Length() * side1.Length());
				double rad = Math.Acos(cosTheta);
				totalRad += rad;
			}
			if (totalRad > 6.2831)
			{
				return true;
			}
		}
		return false;
	}

	public static bool Contain(this AABB aabb, Vector2 position)
	{
		return aabb.position.X <= position.X && position.X <= aabb.position.X + aabb.size.X &&
			aabb.position.Y <= position.Y && position.Y <= aabb.position.Y + aabb.size.Y;
	}

	public static bool Contain(this Circle circle, Vector2 position) =>
		(position - circle.position).Dot() <= circle.radius * circle.radius;

	public static bool Contain(this Edge edge, Vector2 position)
	{
		if (edge.begin == edge.end)
		{
			return position == edge.begin;
		}

		Vector2 v = edge.end - edge.begin;
		Vector2 w = position - edge.begin;
		float c1 = Vector2.Dot(w, v);
		if (c1 <= 0)
		{
			return position == edge.begin;
		}
		float c2 = Vector2.Dot(v, v);
		if (c2 <= c1)
		{
			return position == edge.end;
		}

		return Math.Abs(MathUtils.Cross(v, w)) / v.Length() < Epsilon;
	}

	public static float Distance(this Edge edge, Vector2 point)
	{
		if (edge.begin == edge.end)
		{
			return point.Distance(edge.begin);
		}

		Vector2 v = edge.end - edge.begin;
		Vector2 w = point - edge.begin;
		float c1 = Vector2.Dot(w, v);
		if (c1 <= 0)
		{
			return point.Distance(edge.begin);
		}

		float c2 = Vector2.Dot(v, v);
		if (c2 <= c1)
		{
			return point.Distance(edge.end);
		}

		return point.Distance(edge.begin + v * c1 / c2);
	}

	public static float Distance(this float a, float b) => Math.Abs(a - b);

	public static float Distance(this Circle circle, Vector2 point) => Math.Max(0, (point - circle.position).Length() - circle.radius);

	public static float Distance(this AABB aabb, Vector2 point)
	{
		var dx = Math.Max(aabb.position.X - point.X, 0);
		dx = Math.Max(dx, point.X - (aabb.position.X + aabb.size.X));

		var dy = Math.Max(aabb.position.Y - point.Y, 0);
		dy = Math.Max(dy, point.Y - (aabb.position.Y + aabb.size.Y));

		return MathF.Sqrt(dx * dx + dy * dy);
	}

	public static AABB Scan(this AABB aabb, Vector2 move)
	{
		var vs = new Vector2[4] { aabb.TopLeft, aabb.BottomRight, aabb.TopLeft + move, aabb.BottomRight + move };
		Vector2 min = new(float.MaxValue, float.MaxValue), max = new(float.MinValue, float.MinValue);
		foreach (var v in vs)
		{
			min = Vector2.Min(v, min);
			max = Vector2.Max(v, max);
		}
		return new AABB(min, max - min);
	}

	public static bool FloatEquals(float a, float b) => Math.Abs(a - b) <= Epsilon;

	public static float Projection(Vector2 dir, Vector2 vec)
	{
		return Vector2.Dot(Vector2.Normalize(dir), vec);
	}

	public static List<float> Projection(Vector2 dir, params Vector2[] vec)
	{
		var vs = new List<float>(vec.Length);
		for (int i = 0; i < vec.Length; i++)
		{
			vs.Add(Projection(dir, vec[i]));
		}
		return vs;
	}

	public static List<float> Projection(Vector2 dir, IEnumerable<Vector2> vec)
	{
		var vs = new List<float>();
		foreach (var v in vec)
		{
			vs.Add(Projection(dir, v));
		}
		return vs;
	}

	public static Vector2 MinValue(this IEnumerable<Vector2> vector2s)
	{
		var v = new Vector2(float.MaxValue, float.MaxValue);
		foreach (var it in vector2s)
		{
			v = Vector2.Min(it, v);
		}
		return v;
	}

	public static Vector2 MaxValue(this IEnumerable<Vector2> vector2s)
	{
		var v = new Vector2(0, 0);
		foreach (var it in vector2s)
		{
			v = Vector2.Max(it, v);
		}
		return v;
	}

	public static bool PointOnLine(Vector2 A, Vector2 d, Vector2 C)
	{
		return Math.Abs(MathUtils.Cross(d, C - A)) < 1e-2;
	}

	public static bool PointOnSegment(Vector2 A, Vector2 B, Vector2 C)
	{
		return Math.Abs(MathUtils.Cross(B - A, C - A)) < 1e-2
			&& Vector2.Dot(C - A, B - A) * Vector2.Dot(C - B, A - B) >= 0;
	}

	/// <summary>
	/// 获取AB两动点形成线段和C点的连续碰撞结果，并返回时间
	/// </summary>
	/// <param name="A"></param>
	/// <param name="vA"></param>
	/// <param name="B"></param>
	/// <param name="vB"></param>
	/// <param name="C"></param>
	/// <param name="vC"></param>
	/// <param name="t"></param>
	/// <returns></returns>
	public static bool CCD_SegmentPoint(Vector2 A, Vector2 vA, Vector2 B, Vector2 vB,
		Vector2 C, Vector2 vC, out float t)
	{
		static bool IsInValidTime(float t) => t < 0 || float.IsNaN(t) || float.IsInfinity(t);

		Vector2 E0 = B - A;
		Vector2 E1 = vB - vA;
		Vector2 E2 = C - A;
		Vector2 E3 = vC - vA;

		// Vector2 a1 = E0.X * E2.Y + E0.X * E3.Y * t + E1.X * t * E2.Y + E1.X * E3.Y * t * t;
		// Vector2 a2 = E0.Y * E2.X + E0.Y * E3.X * t + E1.Y * t * E2.X + E1.Y * E3.X * t * t;
		double a = E1.X * E3.Y - E1.Y * E3.X;
		double b = E0.X * E3.Y - E0.Y * E3.X + E1.X * E2.Y - E1.Y * E2.X;
		double c = E0.X * E2.Y - E0.Y * E2.X;

		if (Math.Abs(a) < 1e-4)
		{
			t = (float)(-c / b);
			if (IsInValidTime(t))
			{
				return false;
			}

			return PointOnSegment(A + vA * t, B + vB * t, C + vC * t);
		}

		double det = b * b - 4 * a * c;

		t = 0;

		if (det < 0)
		{
			return false;
		}

		double detSqrt = Math.Sqrt(det);
		double q;
		if (b < 0)
		{
			q = -.5 * (b - detSqrt);
		}
		else
		{
			q = -.5 * (b + detSqrt);
		}

		float t1 = (float)(q / a);
		float t2 = (float)(c / q);

		if (IsInValidTime(t1) && IsInValidTime(t2))
		{
			return false;
		}
		else if (t1 >= 0 && IsInValidTime(t2))
		{
			t = t1;
			return PointOnSegment(A + vA * t, B + vB * t, C + vC * t);
		}
		else if (IsInValidTime(t1) && t2 >= 0)
		{
			t = t2;
			return PointOnSegment(A + vA * t, B + vB * t, C + vC * t);
		}
		else if (t1 >= 0 && t2 >= 0)
		{
			if (t1 > t2)
			{
				(t2, t1) = (t1, t2);
			}
			if (PointOnSegment(A + vA * t1, B + vB * t1, C + vC * t1))
			{
				t = t1;
				return true;
			}
			t = t2;
			return PointOnSegment(A + vA * t, B + vB * t, C + vC * t);
		}
		return false;
	}

	public static AABBCollider2D GetCollider(this Entity entity) => new(new AABB(entity.position.X, entity.position.Y, entity.width, entity.height));
}
