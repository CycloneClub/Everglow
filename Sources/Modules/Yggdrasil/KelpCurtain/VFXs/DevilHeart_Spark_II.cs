using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class DevilHeart_Spark_II : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Queue<Vector2> Trails = new Queue<Vector2>();

	public GradientColor DHSparkColor = new();

	public void FillGradientColor()
	{
		DHSparkColor.colorList.Add((new Color(191, 241, 255, 250), 0));
		DHSparkColor.colorList.Add((new Color(221, 155, 255, 250), 0.08f));
		DHSparkColor.colorList.Add((new Color(225, 68, 223, 250), 0.18f));
		DHSparkColor.colorList.Add((new Color(155, 0, 130, 250), 0.24f));
		DHSparkColor.colorList.Add((new Color(25, 0, 230, 150), 0.44f));
		DHSparkColor.colorList.Add((new Color(5, 0, 160, 150), 0.64f));
		DHSparkColor.colorList.Add((new Color(24, 24, 24, 50), 1));
	}

	public override void Update()
	{
		Trails.Enqueue(Position);
		if (Trails.Count > 20)
		{
			Trails.Dequeue();
		}
		Position += Velocity;
		Velocity *= 0.9f;
		Velocity = Velocity.RotatedBy(ai[1] * 0.05f);
		ai[1] += ai[2];
		ai[2] *= 0.95f;
		ai[1] *= 0.95f;
		Scale = ai[0] * (1 - MathF.Sin(Timer / MaxTime * MathF.PI * 0.5f));
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (DHSparkColor.colorList.Count <= 0)
		{
			FillGradientColor();
		}
		float timeLeftValue = Timer / MaxTime;
		var lightColor = new Color(221, 155, 255, 250);
		if (DHSparkColor.colorList.Count > 0)
		{
			lightColor = DHSparkColor.GetColor(timeLeftValue);
		}
		Lighting.AddLight(Position, lightColor.ToVector3() * Scale * 0.1f);
	}

	public override void Draw()
	{
		if (DHSparkColor.colorList.Count <= 0)
		{
			FillGradientColor();
		}
		var toCorner = new Vector2(0, Scale);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < Trails.Count; i++)
		{
			float size = i / (float)Trails.Count;
			float timeLeftValue = Timer / MaxTime;
			var lightColor = new Color(221, 155, 255, 250);
			if (DHSparkColor.colorList.Count > 0)
			{
				lightColor = DHSparkColor.GetColor(timeLeftValue);
			}
			lightColor.A = (byte)(lightColor.A * size);
			Vector2 pos = Trails.ToArray()[i];
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + Rotation) * size, lightColor, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + Rotation) * size, lightColor, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + Rotation) * size, lightColor, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + Rotation) * size, lightColor, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.DevilHeart_Spark.Value, bars, PrimitiveType.TriangleList);
	}
}
