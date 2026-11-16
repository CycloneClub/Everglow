using Everglow.Commons.Templates.Weapons.StabbingSwords.VFX;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class BlackRingVFXPipeline : Pipeline
{
	public override void BeginRender()
	{
		effect = ModAsset.DrawPrim3D;
		Ins.Batch.Begin(BlendState.AlphaBlend);
	}

	public override void EndRender()
	{
		var camPos = new Vector3(Main.screenWidth / 2 + Main.screenPosition.X, Main.screenHeight / 2 + Main.screenPosition.Y, 0);
		var matrix = Matrix.CreateLookAt(camPos, new Vector3(camPos.X, camPos.Y, 1), Vector3.Down);
		matrix *= Matrix.CreatePerspectiveFieldOfView(MathHelper.Pi / 2f, Main.graphics.GraphicsDevice.Viewport.AspectRatio, 1, 2000);

		effect.Value.Parameters["uTransform"].SetValue(matrix);
		effect.Value.CurrentTechnique.Passes[0].Apply();

		Ins.Batch.End();
	}

	public override void Load()
	{
		Ins.Batch.RegisterVertex<Vertex3D_2>();
	}
}

[Pipeline(typeof(Draw3DPipieline))]
public class BlackRingVFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 pos;
	public Vector2 vel;
	public float scale = 30;
	public Color color;
	public int timeleft = 20;
	public float alpha = 1f;
	public float width = 10;
	public int maxtime = 20;

	public BlackRingVFX()
	{
		alpha = 0.6f;
		timeleft = maxtime = 10;
		randomRot = Main.rand.NextFloatDirection() * 0.5f;
	}

	public static Vector3 RotatedBy(Vector3 v, Vector3 u, float ang)// v以u为轴旋转
	{
		float cos = (float)Math.Cos(ang);
		return v * cos + Vector3.Dot(v, u) * u * (1 - cos) + Vector3.Cross(u, v) * (float)Math.Sin(ang);
	}

	public float speed = 1f;
	public float randomRot = 0;

	public override void Update()
	{
		if (timeleft < maxtime * 2f / 3f)
		{
			alpha *= 0.75f;
			speed *= 0.9f;
		}
		width = 5;
		timeleft--;
		if (timeleft <= 0)
		{
			Kill();
		}

		pos -= vel * speed * 30f / maxtime;

		scale += 0.6f;
	}

	public override void Draw()
	{
		List<Vertex3D_2> vertices = new();
		Color c = color * alpha;
		Color lightC = Lighting.GetColor((int)(pos.X / 16), (int)(pos.Y / 16));
		c.R = (byte)(lightC.R * c.R / 255f);
		c.G = (byte)(lightC.G * c.G / 255f);
		c.B = (byte)(lightC.B * c.B / 255f);
		c.A = 0;
		float ssc = Main.Transform.M11;
		float timeValue = timeleft / (float)maxtime;
		for (int i = 0; i <= 30; i++)
		{
			float a = i * MathHelper.TwoPi / 30f;
			Vector3 v = RotatedBy(Vector3.unitZ, new Vector3(vel.X, vel.Y, 0), a);
			Vector3 rAix = Vector3.Cross(new Vector3(vel.X, vel.Y, 0), Vector3.unitZ);
			v = RotatedBy(v * (1 - timeValue * timeValue * timeValue) * 1.2f, rAix, 0);
			Vector3 p = new Vector3(pos.X, pos.Y, 525 / ssc) + v * scale;
			Vector3 p2 = new Vector3(pos.X, pos.Y, 525 / ssc) + v * scale * 1.5f;
			vertices.Add(new(p, new Vector3(i / 30f, 0, 0), c));
			vertices.Add(new(p2 - new Vector3(vel.X, vel.Y, 0) * width * 7 * (timeleft / (float)maxtime), new Vector3(i / 30f, 1, 0), c));
		}
		Ins.Batch.Draw(Commons.ModAsset.Trail_2_Black_Thick.Value, vertices, PrimitiveType.TriangleStrip);
	}
}
