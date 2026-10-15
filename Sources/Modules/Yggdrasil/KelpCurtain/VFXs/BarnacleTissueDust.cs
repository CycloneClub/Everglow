using Spine;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class BarnacleTissueDust : Visual
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
		if (Trails.Count > 6)
		{
			Trails.Dequeue();
		}
		Position += Velocity;
		Velocity *= 0.95f;
		Scale *= 0.96f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Lighting.AddLight(Position, Scale * 0.016f, Scale * 0.0004f, Scale * 0.008f);
	}

	public override void Draw()
	{
		var lightColor = new Color(1f, 0.18f, 0.12f, 0f);
		float timeValue = (float)Main.time * 0.07f + ai[1];
		var bars = new List<Vertex2D>();
		if (Trails.Count <= 2)
		{
			bars.Add(Position, Color.Transparent, new Vector3(timeValue, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(timeValue, 1, 0));
			bars.Add(Position, Color.Transparent, new Vector3(timeValue, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(timeValue, 1, 0));
		}
		else
		{
			for (int i = 0; i < Trails.Count - 1; i++)
			{
				Vector2 pos = Trails.ToArray()[i];
				Vector2 dir = pos - Trails.ToArray()[i + 1];
				dir = dir.NormalizeSafe();
				float size = i / (float)Trails.Count;
				bars.Add(pos + dir.RotatedBy(MathHelper.PiOver2) * Scale, lightColor * size, new Vector3(i / 12f + timeValue, 0, 0));
				bars.Add(pos + dir.RotatedBy(-MathHelper.PiOver2) * Scale, lightColor * size, new Vector3(i / 12f + timeValue, 1, 0));
			}
		}
		Ins.Batch.Draw(ModAsset.BarnacleTissueDust.Value, bars, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(ModAsset.BarnacleTissueDust.Value, bars, PrimitiveType.TriangleStrip);
	}
}
