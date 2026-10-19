namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class KillLanternMoonMobsWave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float Speed;
	public float Range;
	public float Timer;
	public float MaxTime;
	public float SpeedDecay;

	public float Fade => 1f;

	public override void Update()
	{
		Timer++;
		Range = MathF.Pow(Timer / 90f, 8) * 3600f;
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
			Vector2 endPos = radius * 0.75f;
			float endCoord = 1;
			if (radius.Length() < 200)
			{
				endPos = Vector2.zeroVector;
				endCoord = radius.Length() / 200f;
			}

			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, 0, 0));
			bars.Add(Position + endPos, drawColor, new Vector3(r / (float)count * 3, endCoord, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_full_black.Value, bars, PrimitiveType.TriangleStrip);

		bars = new List<Vertex2D>();
		drawColor = Color.Lerp(new Color(0.5f, 0f, 0f, 0f), new Color(1f, 0.3f, 0.3f, 0), MathF.Pow(Fade, 2)) * Fade;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(Range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			Vector2 endPos = radius * 0.75f;
			float endCoord = 1;
			if (radius.Length() < 200)
			{
				endPos = Vector2.zeroVector;
				endCoord = radius.Length() / 200f;
			}

			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, 0, 0));
			bars.Add(Position + endPos, drawColor, new Vector3(r / (float)count * 3, endCoord, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_full.Value, bars, PrimitiveType.TriangleStrip);

		bars = new List<Vertex2D>();
		drawColor = Color.Lerp(new Color(0.5f, 0f, 0f, 0f), new Color(1f, 0.2f, 0.1f, 0), MathF.Pow(Fade, 2)) * Fade;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(Range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			bars.Add(Position + radius * 0.96f, drawColor, new Vector3(r / (float)count * 3, Range * 0.001f * 0.96f, 0));
			bars.Add(Position + radius * 0.25f, drawColor * 0, new Vector3(r / (float)count * 3, Range * 0.001f * 0.23f, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, bars, PrimitiveType.TriangleStrip);
	}
}
