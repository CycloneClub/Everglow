namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WarpPipeline))]
public class WarpLanternWave : Visual
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
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(Range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			Vector2 dir = radius.NormalizeSafe() * 0.5f + new Vector2(0.5f);
			Color outColor = new Color(dir.X, dir.Y, Fade * 0.2f, 1);
			Color inColor = new Color(dir.X, dir.Y, 0, 1);
			bars.Add(Position + radius, outColor, new Vector3(r / (float)count, 0, 0));
			bars.Add(Position + radius * 0.6f, inColor, new Vector3(r / (float)count, 1, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_Full.Value, bars, PrimitiveType.TriangleStrip);
	}
}
