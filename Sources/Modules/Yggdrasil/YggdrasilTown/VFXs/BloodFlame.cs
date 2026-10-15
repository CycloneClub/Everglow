namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class BloodFlame : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public override void Update()
	{
		Position += Velocity;
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
		Velocity *= MathF.Pow(1 - Timer / MaxTime, 5f);
		Velocity += new Vector2(0, ai[2]).RotatedBy(ai[1]);
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Rotation += ai[1];
		Lighting.AddLight(Position, Scale * 0.1f, 0, 0);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale);
		Color lightColor = new Color(0.5f, 0, 0, 0.5f);

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1 + Rotation), lightColor, new Vector3(1, 0, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5 + Rotation), lightColor, new Vector3(0, 0, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0 + Rotation), lightColor, new Vector3(0, 1, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * -0.5 + Rotation), lightColor, new Vector3(1, 1, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0 + Rotation), lightColor, new Vector3(0, 1, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1 + Rotation), lightColor, new Vector3(1, 0, 0)),
		};
		Ins.Batch.Draw(ModAsset.BloodFlame_noise.Value, bars, PrimitiveType.TriangleList);
	}
}
