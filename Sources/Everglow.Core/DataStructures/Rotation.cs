namespace Everglow.Commons.DataStructures;

[DebuggerDisplay("Angle = {Coefficient} π")]
public struct Rotation
{
	private float _radian;

	public Rotation(float radian)
	{
		_radian = radian;
	}

	public Rotation(Vector2 vector)
	{
		_radian = MathF.Atan2(vector.Y, vector.X);
	}

	/// <summary>
	/// 将弧度转换为角度，范围在 -180 ~ 180 之间
	/// </summary>
	public float Angle
	{
		readonly get => _radian * 180 / MathHelper.Pi;
		set => _radian = Wrap(value / 180 * MathHelper.Pi);
	}

	/// <summary>
	/// 弧度值，范围在 -Pi ~ Pi 之间
	/// </summary>
	public float Radian
	{
		readonly get => _radian;
		set => _radian = Wrap(value);
	}

	/// <summary>
	/// Pi的倍数
	/// </summary>
	public float Coefficient
	{
		readonly get => _radian / MathHelper.Pi;
		set => _radian = Wrap(value * MathHelper.Pi);
	}

	/// <summary>
	/// 余弦值
	/// </summary>
	public float Cos
	{
		readonly get => MathF.Cos(_radian);
		set => _radian = MathF.Acos(value);
	}

	/// <summary>
	/// 旋转矩阵
	/// </summary>
	public readonly Matrix RotationMatrix => Matrix.CreateRotationZ(_radian);

	/// <summary>
	/// 正弦值
	/// </summary>
	public float Sin
	{
		readonly get => MathF.Sin(_radian);
		set => _radian = MathF.Asin(value);
	}

	/// <summary>
	/// 方向向量
	/// </summary>
	public readonly Vector2 XAxis => new(Cos, Sin);

	/// <summary>
	/// 关于X轴对称的角度
	/// </summary>
	public readonly float XFilpAngle => _radian switch
	{
		< 0 => -MathHelper.Pi - _radian,
		_ => MathHelper.Pi - _radian,
	};

	/// <summary>
	/// 法向向量
	/// </summary>
	public readonly Vector2 YAxis => new(-Sin, Cos);

	/// <summary>
	/// 关于Y轴对称的角度
	/// </summary>
	public readonly float YFilpAngle => -_radian;

	/// <summary>
	/// 将一个弧度限制在 -Pi ~ Pi 之间
	/// </summary>
	/// <param name="radian"> </param>
	/// <returns> </returns>
	public static float Wrap(float radian)
	{
		while (radian >= MathHelper.Pi)
		{
			radian -= MathHelper.TwoPi;
		}

		while (radian < -MathHelper.Pi)
		{
			radian += MathHelper.TwoPi;
		}

		return radian;
	}

	/// <summary>
	/// 选择就近的旋转方向进行逼近
	/// </summary>
	/// <param name="target"> </param>
	/// <param name="value"> </param>
	/// <returns> </returns>
	public static Rotation Approach(Rotation start, Rotation target, float value)
	{
		float dis = Math.Abs(start._radian - target._radian);
		bool clockwise = dis < MathHelper.Pi;
		if ((clockwise ? dis : MathHelper.TwoPi - dis) <= value)
			return target;
		return new Rotation(start._radian + value * (clockwise ^ start._radian > target._radian ? 1 : -1));
	}

	/// <summary>
	/// 两个弧度之间的最近距离
	/// </summary>
	/// <param name="from"> </param>
	/// <param name="to"> </param>
	/// <returns> </returns>
	public static float Distance(Rotation from, Rotation to)
	{
		float dis = Math.Abs(from._radian - to._radian);
		return dis >= MathHelper.Pi ? MathHelper.TwoPi - dis : dis;
	}

	/// <summary>
	/// 符合直觉的角度线性插值，选择就近的旋转方向
	/// </summary>
	/// <param name="from"> </param>
	/// <param name="to"> </param>
	/// <param name="value"> </param>
	/// <returns> </returns>
	public static Rotation Lerp(Rotation from, Rotation to, float value)
	{
		if (Math.Abs(from._radian - to._radian) > MathHelper.Pi)
			to._radian -= Math.Sign(to._radian) * MathHelper.TwoPi;
		return from * (1 - value) + to * value;
	}

	/// <summary>
	/// 给定从起点到重点旋转方向与圈数的角度线性插值
	/// </summary>
	/// <param name="from"> </param>
	/// <param name="to"> </param>
	/// <param name="value"> </param>
	/// <param name="direction"> 顺时针 ： 1， 逆时针 ： -1 </param>
	/// <param name="turns"> 圈数 </param>
	/// <returns> </returns>
	public static Rotation Lerp(Rotation from, Rotation to, float value, int direction = 1, int turns = 0)
	{
		while (direction * (to._radian - from._radian) < MathHelper.TwoPi * turns)
		{
			to += MathHelper.TwoPi * direction;
		}

		return from * (1 - value) + to * value;
	}

	public override readonly int GetHashCode() => _radian.GetHashCode();

	public override readonly string ToString() => _radian.ToString();

	#region Operator

	public static explicit operator float(Rotation rot) => rot._radian;

	public static explicit operator Vector2(Rotation rot) => rot.XAxis;

	public static implicit operator Rotation(float v) => new(v);

	public static Rotation operator -(Rotation rot, float angle)
	{
		return new Rotation(rot._radian - angle);
	}

	public static Rotation operator -(float angle, Rotation rot)
	{
		return new Rotation(angle - rot._radian);
	}

	public static Rotation operator -(Rotation a, Rotation b) => new(a._radian - b._radian);

	public static Rotation operator -(Rotation rot) => new(-rot._radian);

	public static Rotation operator *(Rotation rot, float angle) => new(rot._radian * angle);

	public static Rotation operator *(float angle, Rotation rot) => rot * angle;

	public static Rotation operator *(Rotation a, Rotation b) => new(a._radian * b._radian);

	public static Vector2 operator *(Rotation rot, Vector2 vec) => new(vec.X * rot.Cos - vec.Y * rot.Sin, vec.X * rot.Sin + vec.Y * rot.Cos);

	public static Vector2 operator *(Vector2 vec, Rotation rot) => rot * vec;

	public static Rotation operator /(Rotation rot, float angle) => new(rot._radian / angle);

	public static Rotation operator /(float angle, Rotation rot) => new(angle / rot._radian);

	public static Vector2 operator /(Vector2 vec, Rotation rot) => new(vec.X * rot.Cos + vec.Y * rot.Sin, -vec.X * rot.Sin + vec.Y * rot.Cos);

	public static Rotation operator +(Rotation rot, float angle)
	{
		return new Rotation(rot._radian + angle);
	}

	public static Rotation operator +(float angle, Rotation rot) => rot + angle;

	public static Rotation operator +(Rotation a, Rotation b) => new(a._radian + b._radian);

	#endregion Operator
}
