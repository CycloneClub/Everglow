namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class JellyBallSparkTrail : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Queue<Vector2> Trails = new Queue<Vector2>();

	public override void Update()
	{
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		Trails.Enqueue(Position);
		if (Trails.Count > 40)
		{
			Trails.Dequeue();
		}
		Velocity *= 0.9f;
		Position += Velocity;
		Velocity = Velocity.RotatedBy(ai[1]);
		ai[1] *= 0.95f;
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (Collision.SolidCollision(Position - new Vector2(Scale), (int)(Scale * 2), (int)(Scale * 2)))
		{
			Position -= Velocity;
			Velocity *= 0;
			Timer += 10;
		}
		Lighting.AddLight(Position, 0, Scale * 0.01f, Scale * 0.1f);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale);

		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < Trails.Count; i++)
		{
			Vector2 pos = Trails.ToArray()[i];
			float size = i / (float)Trails.Count;
			float colorLerp = (ai[0] - Scale) / ai[0] - size + 1;
			Color lightColor;
			if ((ai[0] - Scale) / ai[0] > 0.5f)
			{
				lightColor = Color.Lerp(new Color(0.05f, 0.1f, 0.85f, 0.5f), new Color(0f, 0f, 0.25f, 0.5f), (colorLerp - 0.5f) * 2f);
			}
			else
			{
				lightColor = Color.Lerp(new Color(0.15f, 0.7f, 15f, 0.5f), new Color(0.05f, 0.1f, 0.85f, 0.5f), colorLerp * 2);
			}
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor * size, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + Rotation) * size, lightColor * size, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor * size, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + Rotation) * size, lightColor * size, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor * size, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor * size, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.JellyBallSparkTrail.Value, bars, PrimitiveType.TriangleList);
	}
}
