using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class LightningDust_Trail : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;
	public Queue<Vector2> trails = new Queue<Vector2>();

	public override void Update()
	{
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		trails.Enqueue(position);
		if (trails.Count > 40)
		{
			trails.Dequeue();
		}
		velocity += new Vector2(0, 0.2f);
		position += velocity;
		velocity *= 0.95f;
		scale *= 0.96f;
		timer++;
		if (timer > maxTime)
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

		var toCorner = new Vector2(0, scale);
		var bars = new List<Vertex2D>();
		for (int i = 0; i < trails.Count; i++)
		{
			Color drawColor = gradientColor.GetColor((1 - i / (float)trails.Count) + timer / maxTime);
			Vector2 pos = trails.ToArray()[i];
			float size = i / (float)trails.Count;
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + rotation) * size, drawColor * size, new Vector3(1, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0.5 + rotation) * size, drawColor * size, new Vector3(0, 0, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + rotation) * size, drawColor * size, new Vector3(0, 1, 0));

			bars.Add(pos + toCorner.RotatedBy(Math.PI * -0.5 + rotation) * size, drawColor * size, new Vector3(1, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 0 + rotation) * size, drawColor * size, new Vector3(0, 1, 0));
			bars.Add(pos + toCorner.RotatedBy(Math.PI * 1 + rotation) * size, drawColor * size, new Vector3(1, 0, 0));
		}
		Ins.Batch.Draw(ModAsset.BloodFlame_noise.Value, bars, PrimitiveType.TriangleList);
		Color lightColor = gradientColor.GetColor(timer / maxTime);
		Lighting.AddLight(position, new Vector3(lightColor.R, lightColor.G, lightColor.B) / 255f * scale * 0.1f);
	}
}
