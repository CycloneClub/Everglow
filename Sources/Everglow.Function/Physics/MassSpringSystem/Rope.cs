namespace Everglow.Commons.Physics.MassSpringSystem;

public class Rope : IMassSpringMesh
{
	public const float Gravity = 9f;

	private Mass[] _masses;

	private ElasticConstrain[] _springs;

	/// <summary>
	/// 自动生成一串由位置决定的绳子链
	/// </summary>
	/// <param name="count">绳子包含的质点数量，相邻质点之间各分配一个弹簧约束。</param>
	private Rope(int count)
	{
		_masses = new Mass[count];
		_springs = new ElasticConstrain[count - 1];
	}

	public ElasticConstrain[] ElasticConstrains => _springs;

	public Mass[] Masses => _masses;

	/// <summary>
	/// Create a vine-like rope, in equilibrium, each mass is equally spaced from its neighbors, and the first mass is static.<br/>
	/// F = kx, k = 1, F = mg = 9, x = 16.
	/// </summary>
	/// <param name="position"></param>
	/// <param name="count"></param>
	/// <param name="mass"></param>
	/// <param name="last_elasticity">To ensure each mass is equally spaced from its neighbors at equilibrium, the elasticity should be calculated from a basic value.</param>
	/// <param name="springLength"></param>
	/// <returns></returns>
	public static Rope Create_Vine(Vector2 position, int count, float mass = 1, float last_elasticity = 1, float springLength = 7)
	{
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			var m = rope._masses[i] = new Mass(mass, position + new Vector2(0, 16 * i), i == 0);

			// give a tiny force to break the initial balance.
			m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength, last_elasticity * (count - i));
			}
		}
		return rope;
	}

	/// <summary>
	/// Experimental codes.
	/// </summary>
	/// <param name="vine"></param>
	/// <param name="amount"></param>
	/// <returns></returns>
	public static Rope Grow_Vine(Rope vine, int amount)
	{
		if (amount <= 0)
		{
			return vine;
		}
		int count = vine.Masses.Length + amount;
		float mass = vine.Masses[0].Value;
		float elasticity = vine.ElasticConstrains[^1].Stiffness;
		float springLength = vine.ElasticConstrains[^1].RestLength;
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			if (i - amount >= 0)
			{
				var m = rope._masses[i] = new Mass(mass, vine._masses[i - amount].Position + new Vector2(0, amount * springLength), false);
				m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			}
			else
			{
				var m = rope._masses[i] = new Mass(mass, vine._masses[0].Position + new Vector2(0, (i - amount) * springLength), i == 0);
				m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			}
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength, elasticity * (count - i));
			}
		}
		return rope;
	}

	public static Rope Cut_Vine(Rope vine, int amount)
	{
		if (amount <= 0)
		{
			return vine;
		}
		int count = vine.Masses.Length - amount;
		if (count <= 1)
		{
			return vine;
		}
		float mass = vine.Masses[0].Value;
		float elasticity = vine.ElasticConstrains[^1].Stiffness;
		float springLength = vine.ElasticConstrains[^1].RestLength;
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			var m = rope._masses[i] = new Mass(mass, vine._masses[i + amount].Position, i == 0);

			// give a tiny force to break the initial balance.
			m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength, elasticity * (count - i));
			}
		}
		return rope;
	}

	/// <summary>
	/// Create a new rope.
	/// </summary>
	/// <param name="masses"></param>
	/// <param name="springLength_elasticity">X for spring length, Y for elasticity.</param>
	/// <returns></returns>
	public static Rope Create(List<Mass> masses, List<Vector2> springLength_elasticity)
	{
		int count = Math.Min(masses.Count, springLength_elasticity.Count);
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			var m = rope._masses[i] = masses[i];

			// give a tiny force to break the initial balance.
			m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength_elasticity[i - 1].X, springLength_elasticity[i - 1].Y);
			}
		}
		return rope;
	}

	/// <summary>
	/// Two points are given to connect the rope and both ends are fixed.<br/>
	/// Knot is a special mass point with different mass value. Just like lightbulbs in a rope, normal masses simulate the rope and knot masses simulate the lightbulbs. <br/>
	/// Spring length is decided by the count, start and end. <br/>
	/// </summary>
	/// <param name="start">Position of the first mass.</param>
	/// <param name="end">Position of the last mass.</param>
	/// <param name="count">The number of masses.</param>
	/// <param name="elasticity"></param>
	/// <param name="mass">The weight of normal mass.</param>
	/// <param name="knotDistance">How many normal masses between 2 knots</param>
	/// <param name="knotMass"> </param>
	/// <returns> </returns>
	public static Rope Create_Fixed_StartAndEnd_WithKnots(Vector2 start, Vector2 end, int count, float elasticity, float mass, int knotDistance = 0, float knotMass = 1)
	{
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			var position = Vector2.Lerp(start, end, i / (count - 1f));
			float specialMass = mass;
			if (knotDistance > 0)
			{
				if (i % knotDistance == knotDistance / 2)
				{
					specialMass = knotMass;
				}
			}
			var m = rope._masses[i] = new Mass(specialMass, position, i == 0 || i == count - 1);

			// give a tiny force to break the initial balance.
			m.Velocity = new Vector2(0, 0.1f).RotatedBy(i);
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					(prev.Position - m.Position).Length(), elasticity);
			}
		}
		return rope;
	}

	/// <summary>
	/// Create a rope at start and extending towards down.<br />
	/// Mass at <paramref name="start" /> is fixed.
	/// </summary>
	/// <param name="start">Position of the first mass.</param>
	/// <param name="count">The number of masses.</param>
	/// <param name="elasticity"></param>
	/// <param name="mass">The weight of normal mass.</param>
	/// <param name="springLength">Rest length of each spring between adjacent masses.</param>
	/// <returns> </returns>
	public static Rope Create_Fixed_StartPos(Vector2 start, int count, float elasticity, float mass, float springLength)
	{
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			var position = start + new Vector2(0, mass * 10 * i);
			var m = rope._masses[i] = new Mass(mass, position, i == 0);
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength, elasticity);
			}
		}
		return rope;
	}

	/// <summary>
	/// Create a rope at start and extending towards down.<br />
	/// Last mass usually should be very heavy to simulate pendant lamp.
	/// </summary>
	/// <param name="start">Position of the first mass.</param>
	/// <param name="count">The number of masses.</param>
	/// <param name="elasticity"></param>
	/// <param name="mass"></param>
	/// <param name="endMass"></param>
	/// <param name="offsetY"></param>
	/// <param name="springLength"></param>
	/// <returns></returns>
	public static Rope Create_Fixed_Start_Heavy_End(Vector2 start, int count, float elasticity, float mass, float endMass, int offsetY = 0, float springLength = 6)
	{
		Rope rope = new Rope(count);
		for (int i = 0; i < count; i++)
		{
			int posY = i - offsetY;
			if (posY < 0)
			{
				posY = 0;
			}
			var position = start + new Vector2(0, mass * 10 * posY);
			var m = rope._masses[i] = new Mass(mass, position, i == 0);
			if (i == count - 1)
			{
				m.Value = endMass;
			}
			if (i != 0)
			{
				var prev = rope._masses[i - 1];
				rope._springs[i - 1] = new ElasticConstrain(prev, rope._masses[i],
					springLength, elasticity);
			}
		}
		return rope;
	}

	public void ApplyForce_Gravity_Wind()
	{
		for (int i = 0; i < _masses.Length; i++)
		{
			Mass m = _masses[i];
			m.Force += new Vector2(2 * (MathF.Sin((float)Main.timeForVisualEffects / 72f + m.Position.X / 13f + m.Position.Y / 4f) + 0.9f), 0)
				* Main.windSpeedCurrent
				+ new Vector2(0, Gravity * m.Value);
		}
	}

	public void ApplyForce_Gravity()
	{
		for (int i = 0; i < _masses.Length; i++)
		{
			Mass m = _masses[i];
			m.Force += new Vector2(0, Gravity * m.Value);
		}
	}

	public void ApplyForce_Wind()
	{
		for (int i = 0; i < _masses.Length; i++)
		{
			Mass m = _masses[i];
			m.Force += new Vector2(2 * (MathF.Sin((float)Main.timeForVisualEffects / 72f + m.Position.X / 13f + m.Position.Y / 4f) + 0.9f), 0)
				* Main.windSpeedCurrent;
		}
	}

	public void ApplyForce_VelocityDecay(float decayValue = 0.05f)
	{
		for (int i = 0; i < _masses.Length; i++)
		{
			Mass m = _masses[i];
			m.Force -= m.Velocity * decayValue;
		}
	}

	public void ApplyForceSpecial(int index, Vector2 force)
	{
		_masses[index].Force += force;
	}
}
