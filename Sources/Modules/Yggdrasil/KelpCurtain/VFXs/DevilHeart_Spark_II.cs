using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class DevilHeart_Spark_II : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;
	public Queue<Vector2> trails = new Queue<Vector2>();

	public GradientColor dHSparkColor = new();

	public void FillGradientColor()
	{
		dHSparkColor.colorList.Add((new Color(191, 241, 255, 250), 0));
		dHSparkColor.colorList.Add((new Color(221, 155, 255, 250), 0.08f));
		dHSparkColor.colorList.Add((new Color(225, 68, 223, 250), 0.18f));
		dHSparkColor.colorList.Add((new Color(155, 0, 130, 250), 0.24f));
		dHSparkColor.colorList.Add((new Color(25, 0, 230, 150), 0.44f));
		dHSparkColor.colorList.Add((new Color(5, 0, 160, 150), 0.64f));
		dHSparkColor.colorList.Add((new Color(24, 24, 24, 50), 1));
	}

	public override void Update()
	{
		trails.Enqueue(position);
		if (trails.Count > 20)
		{
			trails.Dequeue();
		}
		position += velocity;
		velocity *= 0.9f;
		velocity = velocity.RotatedBy(ai[1] * 0.05f);
		ai[1] += ai[2];
		ai[2] *= 0.95f;
		ai[1] *= 0.95f;
		scale = ai[0] * (1 - MathF.Sin(timer / maxTime * MathF.PI * 0.5f));
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}
		if (dHSparkColor.colorList.Count <= 0)
		{
			FillGradientColor();
		}
		float timeLeftValue = timer / maxTime;
		var lightColor = new Color(221, 155, 255, 250);
		if (dHSparkColor.colorList.Count > 0)
		{
			lightColor = dHSparkColor.GetColor(timeLeftValue);
		}
		Lighting.AddLight(position, lightColor.ToVector3() * scale * 0.1f);
	}

	public override void Draw()
	{
		if (dHSparkColor.colorList.Count <= 0)
		{
			FillGradientColor();
		}
		var toCorner = new Vector2(0, scale);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < trails.Count; i++)
		{
			float size = i / (float)trails.Count;
			float timeLeftValue = timer / maxTime;
			var lightColor = new Color(221, 155, 255, 250);
			if (dHSparkColor.colorList.Count > 0)
			{
				lightColor = dHSparkColor.GetColor(timeLeftValue);
			}
			lightColor.A = (byte)(lightColor.A * size);
			Vector2 pos = trails.ToArray()[i];
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + rotation) * size, lightColor, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + rotation) * size, lightColor, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + rotation) * size, lightColor, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + rotation) * size, lightColor, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + rotation) * size, lightColor, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + rotation) * size, lightColor, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.DevilHeart_Spark.Value, bars, PrimitiveType.TriangleList);
	}
}
