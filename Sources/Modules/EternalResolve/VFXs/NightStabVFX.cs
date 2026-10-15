using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.EternalResolve.VFXs;

public class NightStabVFX : StabVFX
{
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
			vertices.Add(new(p, new Vector3(i / 30f, 0.5f, 0), Color.White * 0.4f));
			vertices.Add(new(p2 - new Vector3(Velocity.X, Velocity.Y, 0) * Width * 7 * (TimeLeft / (float)MaxTime), new Vector3(i / 30f, 1, 0), Color.White * 0.4f));
		}
		Ins.Batch.Draw(Commons.ModAsset.Trail_0_blackWhite.Value, vertices, PrimitiveType.TriangleStrip);
		vertices = new();
		for (int i = 0; i <= 30; i++)
		{
			float a = i * MathHelper.TwoPi / 30f;
			Vector3 v = RotatedBy(Vector3.unitZ, new Vector3(Velocity.X, Velocity.Y, 0), a);
			var rAix = Vector3.Cross(new Vector3(Velocity.X, Velocity.Y, 0), Vector3.unitZ);
			v = RotatedBy(v * (1 - timeValue * timeValue * timeValue) * 1.2f, rAix, 0);
			Vector3 p = new Vector3(Position.X, Position.Y, 525 / ssc) + v * Scale;
			Vector3 p2 = new Vector3(Position.X, Position.Y, 525 / ssc) + v * Scale * 1.5f;
			vertices.Add(new(p, new Vector3(i / 30f, 0, 0), c));
			vertices.Add(new(p2 - new Vector3(Velocity.X, Velocity.Y, 0) * Width * 7 * (TimeLeft / (float)MaxTime), new Vector3(i / 30f, 0.5f, 0), c));
		}
		Ins.Batch.Draw(Commons.ModAsset.Trail_0_blackWhite.Value, vertices, PrimitiveType.TriangleStrip);
	}
}
