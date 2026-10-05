using Spine;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class AvariceSuccessCube : Visual
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
		Position += Velocity;
		Velocity *= 0.95f;
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Rotation = ai[1];
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Lighting.AddLight(Position, 0, Scale * 0.06f, Scale * 0.08f);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale);
		Color lightColor = new Color(0f, 0.6f, 0.8f, 0.5f);
		List<Vertex2D> bars = new List<Vertex2D>();
		bars.Add(Position + toCorner.RotatedBy(Math.PI * 1 + Rotation), lightColor, new Vector3(1, 0, 0));
		bars.Add(Position + toCorner.RotatedBy(Math.PI * 0.5 + Rotation), lightColor, new Vector3(0, 0, 0));
		bars.Add(Position + toCorner.RotatedBy(Math.PI * 0 + Rotation), lightColor, new Vector3(0, 1, 0));

		bars.Add(Position + toCorner.RotatedBy(Math.PI * -0.5 + Rotation), lightColor, new Vector3(1, 1, 0));
		bars.Add(Position + toCorner.RotatedBy(Math.PI * 0 + Rotation), lightColor, new Vector3(0, 1, 0));
		bars.Add(Position + toCorner.RotatedBy(Math.PI * 1 + Rotation), lightColor, new Vector3(1, 0, 0));
		Ins.Batch.Draw(Commons.ModAsset.TileBlock.Value, bars, PrimitiveType.TriangleList);
	}
}
