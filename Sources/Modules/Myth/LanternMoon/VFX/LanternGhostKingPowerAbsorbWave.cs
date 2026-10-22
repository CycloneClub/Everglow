namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class LanternGhostKingPowerAbsorbWave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float Timer;
	public float MaxTime;

	public float Fade => 1;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		for (int k = 0; k < 4; k++)
		{
			DrawWave((MaxTime - Timer) * 3f - k * 300f);
		}
		Texture2D tex = Commons.ModAsset.StarSlashGray.Value;
		float starScale = 1f + MathF.Sin(Timer * 0.1f) * 0.5f;
		Color starColor = Color.Lerp(new Color(0.5f, 0, 0, 0), new Color(1f, 1f, 0.7f, 0), 1 - starScale / 1.5f);
		Ins.Batch.Draw(tex, Position, null, starColor, 0, tex.Size() * 0.5f, new Vector2(starScale, 1), SpriteEffects.None);
		Ins.Batch.Draw(tex, Position, null, starColor, MathHelper.PiOver2, tex.Size() * 0.5f, new Vector2(starScale, 2 + starScale), SpriteEffects.None);
	}

	public void DrawWave(float range)
	{
		float maxDis = 500 + Timer / 5f;
		float colorValue = maxDis - range;
		colorValue *= 1 / maxDis;
		if (range < 0)
		{
			return;
		}
		int count = (int)(range / 10f + 1);
		count = Math.Max(count, 15);
		List<Vertex2D> bars = new List<Vertex2D>();
		Color drawColor = new Color(1f, 1f, 1f, Fade) * colorValue;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			Vector2 endPos = radius * 0.5f;
			float endCoord = 1;

			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, endCoord, 0));
			bars.Add(Position + endPos, drawColor, new Vector3(r / (float)count * 3, 0, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_full_black.Value, bars, PrimitiveType.TriangleStrip);

		bars = new List<Vertex2D>();
		drawColor = Color.Lerp(new Color(0.5f, 0f, 0f, 0f), new Color(1f, 0.3f, 0.3f, 0), MathF.Pow(Fade, 2)) * Fade * colorValue;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			Vector2 endPos = radius * 0.5f;
			float endCoord = 1;

			bars.Add(Position + radius, drawColor, new Vector3(r / (float)count * 3, endCoord, 0));
			bars.Add(Position + endPos, drawColor, new Vector3(r / (float)count * 3, 0, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Wave_full.Value, bars, PrimitiveType.TriangleStrip);

		bars = new List<Vertex2D>();
		drawColor = Color.Lerp(new Color(0.5f, 0f, 0f, 0f), new Color(1f, 0.2f, 0.1f, 0), MathF.Pow(Fade, 2)) * Fade * colorValue;
		for (int r = 0; r <= count; r++)
		{
			Vector2 radius = new Vector2(range, 0).RotatedBy(r * MathHelper.TwoPi / count);
			float xCoord = (float)(r / (double)count * 6d);
			bars.Add(Position + radius * 0.96f, drawColor * 0, new Vector3(xCoord, range * 0.005f * 0.96f, 0));
			bars.Add(Position + radius * 0.5f, drawColor, new Vector3(xCoord, range * 0.005f * 0.5f, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, bars, PrimitiveType.TriangleStrip);
	}
}
