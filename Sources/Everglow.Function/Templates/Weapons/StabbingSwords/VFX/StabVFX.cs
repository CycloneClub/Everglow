using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;

[Pipeline(typeof(Draw3DPipieline))]
public class StabVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Scale = 30;
	public Color StabEffectColor;
	public int TimeLeft = 20;
	public float Alpha = 1f;
	public float Width = 10;
	public int MaxTime = 20;

	public StabVFX()
	{
		Alpha = 0.6f;
		TimeLeft = MaxTime = 10;
		RandomRot = Main.rand.NextFloatDirection() * 0.5f;
	}

	public static Vector3 RotatedBy(Vector3 v, Vector3 u, float ang)// v以u为轴旋转
	{
		float cos = (float)Math.Cos(ang);
		return v * cos + Vector3.Dot(v, u) * u * (1 - cos) + Vector3.Cross(u, v) * (float)Math.Sin(ang);
	}

	public float Speed = 1f;
	public float RandomRot = 0;

	public override void Update()
	{
		if (TimeLeft < MaxTime * 2f / 3f)
		{
			Alpha *= 0.75f;
			Speed *= 0.9f;
		}
		Width = 5;
		TimeLeft--;
		if (TimeLeft <= 0)
		{
			Kill();
		}

		Position -= Velocity * Speed * 30f / MaxTime;

		Scale += 0.6f;
	}

	public override void Draw()
	{
		List<Vertex3D_2> vertices = new();
		Color c = StabEffectColor * Alpha;
		Color lightC = Lighting.GetColor((int)(Position.X / 16), (int)(Position.Y / 16));
		c.R = (byte)(lightC.R * c.R / 255f);
		c.G = (byte)(lightC.G * c.G / 255f);
		c.B = (byte)(lightC.B * c.B / 255f);
		c.A = 0;
		float ssc = Main.Transform.M11;
		float timeValue = TimeLeft / (float)MaxTime;
		for (int i = 0; i <= 30; i++)
		{
			float a = i * MathHelper.TwoPi / 30f;
			Vector3 v = RotatedBy(Vector3.unitZ, new Vector3(Velocity.X, Velocity.Y, 0), a);
			var rAix = Vector3.Cross(new Vector3(Velocity.X, Velocity.Y, 0), Vector3.unitZ);
			v = RotatedBy(v * (1 - timeValue * timeValue * timeValue) * 1.2f, rAix, 0);
			Vector3 p = new Vector3(Position.X, Position.Y, 525 / ssc) + v * Scale;
			Vector3 p2 = new Vector3(Position.X, Position.Y, 525 / ssc) + v * Scale * 1.5f;
			vertices.Add(new(p, new Vector3(i / 30f, 0, 0), c));
			vertices.Add(new(p2 - new Vector3(Velocity.X, Velocity.Y, 0) * Width * 7 * (TimeLeft / (float)MaxTime), new Vector3(i / 30f, 1, 0), c));
		}
		Ins.Batch.Draw(ModAsset.Trail_0.Value, vertices, PrimitiveType.TriangleStrip);
	}
}
