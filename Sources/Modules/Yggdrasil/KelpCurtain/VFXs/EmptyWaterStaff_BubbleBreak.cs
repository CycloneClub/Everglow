using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class EmptyWaterStaff_BubbleBreak : Visual
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
		Velocity += new Vector2(0, 0.02f);
		Position += Velocity;
		Velocity *= 0.8f;
		Scale *= 0.96f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		var toCorner = new Vector2(0, Scale);
		var bars = new List<Vertex2D>();
		Color drawColor = new Color(204, 238, 255);
		drawColor = Lighting.GetColor(Position.ToTileCoordinates(), drawColor);
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
