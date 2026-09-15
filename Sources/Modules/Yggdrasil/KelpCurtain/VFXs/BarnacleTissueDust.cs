using Spine;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class BarnacleTissueDust : Visual
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
		if (trails.Count > 6)
		{
			trails.Dequeue();
		}
		position += velocity;
		velocity *= 0.95f;
		scale *= 0.96f;
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}
		Lighting.AddLight(position, scale * 0.016f, scale * 0.0004f, scale * 0.008f);
	}

	public override void Draw()
	{
		var lightColor = new Color(1f, 0.18f, 0.12f, 0f);
		float timeValue = (float)Main.time * 0.07f + ai[1];
		var bars = new List<Vertex2D>();
		if (trails.Count <= 2)
		{
			bars.Add(position, Color.Transparent, new Vector3(timeValue, 0, 0));
			bars.Add(position, Color.Transparent, new Vector3(timeValue, 1, 0));
			bars.Add(position, Color.Transparent, new Vector3(timeValue, 0, 0));
			bars.Add(position, Color.Transparent, new Vector3(timeValue, 1, 0));
		}
		else
		{
			for (int i = 0; i < trails.Count - 1; i++)
			{
				Vector2 pos = trails.ToArray()[i];
				Vector2 dir = pos - trails.ToArray()[i + 1];
				dir = dir.NormalizeSafe();
				float size = i / (float)trails.Count;
				bars.Add(pos + dir.RotatedBy(MathHelper.PiOver2) * scale, lightColor * size, new Vector3(i / 12f + timeValue, 0, 0));
				bars.Add(pos + dir.RotatedBy(-MathHelper.PiOver2) * scale, lightColor * size, new Vector3(i / 12f + timeValue, 1, 0));
			}
		}
		Ins.Batch.Draw(ModAsset.BarnacleTissueDust.Value, bars, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(ModAsset.BarnacleTissueDust.Value, bars, PrimitiveType.TriangleStrip);
	}
}
