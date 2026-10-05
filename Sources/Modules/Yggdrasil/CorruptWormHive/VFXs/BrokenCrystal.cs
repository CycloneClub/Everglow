using Terraria.GameContent;

namespace Everglow.Yggdrasil.CorruptWormHive.VFXs;

[Pipeline(typeof(WCSPipeline))]
internal class BrokenCrystal : Visual
{
	public Vector2 Position;
	public Vector2 Velocity;
	public int timeLeft;
	public float size;
	public float omega;
	public float Rotation;

	private float theta;

	private Vector2 vS1;
	private Vector2 vS2;
	private Vector2 vS3;

	private Vector2 p1;
	private Vector2 p2;
	private Vector2 p3;

	private Vector2 po1;
	private Vector2 po2;
	private Vector2 po3;
	private float ramdomC;

	private float ros;

	public override void OnSpawn()
	{
		timeLeft = Main.rand.Next(387, 399);
		ramdomC = 0;
		theta += Main.rand.NextFloat(-3.14f, 3.14f);
		ros = Main.rand.NextFloat(-0.15f, 0.15f);
		p1 = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
		p2 = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
		p3 = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
		base.OnSpawn();
	}

	public override void Update()
	{
		Position += Velocity;
		timeLeft -= 1;
		if (timeLeft <= 0)
		{
			Kill();
		}

		if (timeLeft <= 30)
		{
			Velocity *= 0.98f;
		}

		Velocity = Velocity.RotatedBy(omega);
		omega += Main.rand.NextFloat(-0.05f, 0.05f);
		if (Math.Abs(omega) > 0.15)
		{
			omega *= 0.98f;
		}

		ramdomC += 3;

		theta += ros;
		po1 = new Vector2(p1.X, p1.Y * (float)Math.Sin(theta)).RotatedBy(Rotation) * 90 * size;
		po2 = new Vector2(p2.X, p2.Y * (float)Math.Sin(theta)).RotatedBy(Rotation) * 90 * size;
		po3 = new Vector2(p3.X, p3.Y * (float)Math.Sin(theta)).RotatedBy(Rotation) * 90 * size;
		Rotation -= omega * 0.66f;
		Velocity *= 0.99f;
		size *= 0.99f;
		if (ramdomC > 300)
		{
			base.Update();
		}
	}

	public override void Draw()
	{
		var Vy = new List<Vertex2D>();
		Color colorD = Color.White;
		Vector2 v1 = po1 + Position;
		Vector2 v2 = po2 + Position;
		Vector2 v3 = po3 + Position;
		if (vS1 == Vector2.Zero)
		{
			vS1 = v1 - Main.screenPosition;
		}

		if (vS2 == Vector2.Zero)
		{
			vS2 = v2 - Main.screenPosition;
		}

		if (vS3 == Vector2.Zero)
		{
			vS3 = v3 - Main.screenPosition;
		}

		Vy.Add(new Vertex2D(v1, colorD, new Vector3(vS1.X / Main.screenTarget.Width, vS1.Y / Main.screenTarget.Height, 0)));
		Vy.Add(new Vertex2D(v2, colorD, new Vector3(vS2.X / Main.screenTarget.Width, vS2.Y / Main.screenTarget.Height, 0)));
		Vy.Add(new Vertex2D(v3, colorD, new Vector3(vS3.X / Main.screenTarget.Width, vS3.Y / Main.screenTarget.Height, 0)));

		GraphicsDevice gd = Main.graphics.GraphicsDevice;

		var Co0 = new Color(255, 0, 15);
		int DrawBase = (int)(300 - ramdomC);
		var Vx = new List<Vertex2D>();
		colorD = new Color(DrawBase / 4, DrawBase / 180, DrawBase / 180, 155);
		Vx.Add(new Vertex2D(po1 + Position, colorD, new Vector3(0, 0, 0)));
		Vx.Add(new Vertex2D(po2 + Position, colorD, new Vector3(0, 0, 0)));
		Vx.Add(new Vertex2D(po3 + Position, colorD, new Vector3(0, 0, 0)));
		gd.Textures[0] = TextureAssets.MagicPixel.Value;
		gd.DrawUserPrimitives(PrimitiveType.TriangleList, Vx.ToArray(), 0, Vx.Count - 2);
	}

	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;
}
