namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class Rock_Concentrating_dust : Visual
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
		Projectile proj = Main.projectile[(int)ai[1]];
		if (proj == null || !proj.active || proj.type != ai[2])
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		Trails.Enqueue(Position);
		if (Trails.Count > 10)
		{
			Trails.Dequeue();
		}
		Position += Velocity;
		Velocity *= 0.95f;
		Vector2 toProj = proj.Center - Position - Velocity;
		if (toProj.Length() < 10)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		Velocity += Vector2.Normalize(toProj) * .04f * Scale;
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale);
		Vector4 lightColor = Lighting.GetColor(Position.ToTileCoordinates()).ToVector4();
		Color drawColor = new Color(0.4f * lightColor.X, 0.3f * lightColor.X, 0.3f * lightColor.Y, lightColor.Z) * ((1 + MathF.Cos(ai[3])) * 0.2f + 0.4f);
		drawColor.A = 255;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i < Trails.Count; i++)
		{
			Vector2 pos = Trails.ToArray()[i];
			float size = i / (float)Trails.Count;
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, drawColor * size, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + Rotation) * size, drawColor * size, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, drawColor * size, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + Rotation) * size, drawColor * size, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, drawColor * size, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, drawColor * size, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.BloodFlame_noise.Value, bars, PrimitiveType.TriangleList);
	}
}
