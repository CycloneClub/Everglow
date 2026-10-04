namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class Fevens_LightingBoltDust : Visual
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
		if (Trails.Count > 15)
		{
			Trails.Dequeue();
		}
		Position += Velocity;
		if (Main.rand.NextBool(8))
		{
			Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f));
		}
		Velocity *= 0.95f;
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Lighting.AddLight(Position, Scale * 0.1f, 0, 0);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale);
		Color lightColor = new Color(0.5f, 0, 0, 0.5f);
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < Trails.Count; i++)
		{
			Vector2 pos = Trails.ToArray()[i];
			float size = i / (float)Trails.Count;
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor * size, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + Rotation) * size, lightColor * size, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor * size, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + Rotation) * size, lightColor * size, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor * size, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor * size, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.BloodFlame_noise.Value, bars, PrimitiveType.TriangleList);
	}
}
