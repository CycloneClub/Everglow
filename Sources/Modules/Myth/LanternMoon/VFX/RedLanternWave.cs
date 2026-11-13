namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class RedLanternWave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float Speed;
	public float Range;
	public float Timer;
	public float MaxTime;
	public float SpeedDecay;

	public float Fade => 1 - Timer / MaxTime;

	public override void Update()
	{
		Timer++;
		Range += Speed;
		Speed *= SpeedDecay;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		int count = (int)(Range / 10f + 1);
		List<Vertex2D> bars = new List<Vertex2D>();
		Color drawColor = new Color(1f, 1f, 1f, Fade);
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(Range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, 0, 0));
			bars.Add(Position + radius * 0.5f, drawColor, new Vector3(r / (float)count * 3, 1, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_Full_Black.Value, bars, PrimitiveType.TriangleStrip);

		bars = new List<Vertex2D>();
		drawColor = Color.Lerp(new Color(1f, 0f, 0f, 0f), new Color(1f, 0.9f, 0.6f, 0), MathF.Pow(Fade, 2)) * Fade;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(Range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, 0, 0));
			bars.Add(Position + radius * 0.5f, drawColor, new Vector3(r / (float)count * 3, 1, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_Full.Value, bars, PrimitiveType.TriangleStrip);
	}
}
