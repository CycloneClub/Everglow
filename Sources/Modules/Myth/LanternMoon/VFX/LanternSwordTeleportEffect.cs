namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class LanternSwordTeleportEffect : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position_End;
	public Vector2 Position_Start;
	public float Timer;
	public float MaxTime;

	public float Duration => Timer / MaxTime;

	public override void Update()
	{
		Timer++;
		Lighting.AddLight(Duration.Lerp(Position_Start, Position_End), new Vector3(1f, 0, 0));
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float timeValue = (float)(Main.time * 0.03f);
		Vector2 normal = (Position_End - Position_Start).RotatedBy(MathHelper.PiOver2).NormalizeSafe() * 35f;
		Color normalColor = new Color(1f, 0, 0, 0);
		List<Vertex2D> barsR_dark = new List<Vertex2D>();
		List<Vertex2D> barsL_dark = new List<Vertex2D>();
		List<Vertex2D> barsR = new List<Vertex2D>();
		List<Vertex2D> barsL = new List<Vertex2D>();
		for (int t = -10; t < 110; t++)
		{
			float value = t / 100f;
			Vector2 pos = value.Lerp(Position_Start, Position_End);
			float distance = Math.Abs(value - Duration);
			float colorFade = Math.Clamp(0.5f - distance, 0, 0.5f) * 2;
			colorFade = (1 - MathF.Cos(colorFade * MathHelper.Pi)) * 0.5f;
			if (t < 0)
			{
				colorFade *= 1 + t / 10f;
			}
			if (t > 100)
			{
				colorFade *= 1 + (100 - t) / 10f;
			}
			barsR_dark.Add(pos + normal, Color.White * 0, new Vector3(value + timeValue, 0, 0));
			barsR_dark.Add(pos, Color.White * colorFade, new Vector3(value + timeValue, 0.5f, 0));
			barsL_dark.Add(pos - normal, Color.White * 0, new Vector3(value + timeValue, 1, 0));
			barsL_dark.Add(pos, Color.White * colorFade, new Vector3(value + timeValue, 0.5f, 0));

			barsR.Add(pos + normal, normalColor * 0, new Vector3(value + timeValue, 0, 0));
			barsR.Add(pos, normalColor * colorFade, new Vector3(value + timeValue, 0.5f, 0));
			barsL.Add(pos - normal, normalColor * 0, new Vector3(value + timeValue, 1, 0));
			barsL.Add(pos, normalColor * colorFade, new Vector3(value + timeValue, 0.5f, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3_Black.Value, barsR_dark, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3_Black.Value, barsL_dark, PrimitiveType.TriangleStrip);

		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, barsR, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_Flame_3.Value, barsL, PrimitiveType.TriangleStrip);
	}
}
