using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class LightningDust_Trail : Visual
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
		Velocity += new Vector2(0, 0.2f);
		Position += Velocity;
		Velocity *= 0.95f;
		Scale *= 0.96f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		GradientColor gradientColor = new GradientColor();
		gradientColor.colorList.Add((new Color(1f, 1f, 1f, 0), 0));
		gradientColor.colorList.Add((new Color(140, 232, 255, 0), 0.2f));
		gradientColor.colorList.Add((new Color(252, 170, 255, 0), 0.4f));
		gradientColor.colorList.Add((new Color(255, 166, 114, 0), 0.6f));
		gradientColor.colorList.Add((new Color(255, 84, 81, 0), 0.8f));
		gradientColor.colorList.Add((new Color(0, 0, 0, 0), 1));

		var toCorner = new Vector2(0, Scale);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < Trails.Count; i++)
		{
			Color drawColor = gradientColor.GetColor((1 - i / (float)Trails.Count) + Timer / MaxTime);
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
		Color lightColor = gradientColor.GetColor(Timer / MaxTime);
		Lighting.AddLight(Position, new Vector3(lightColor.R, lightColor.G, lightColor.B) / 255f * Scale * 0.1f);
	}
}
