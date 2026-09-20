namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class RedAlgae_GasRing : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Fade;

	public override void Update()
	{
		if (Timer < 1)
		{
			Fade = 1;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (MaxTime - Timer < 60)
		{
			Fade *= 0.95f;
		}
	}

	public override void Draw()
	{
		float radius = Scale * 10 * MathF.Pow(Timer / MaxTime, 0.5f);
		Texture2D tex = Commons.ModAsset.Noise_spiderNet.Value;
		Texture2D tex_dark = Commons.ModAsset.Noise_spiderNet_dark.Value;
		List<Vertex2D> bars = new List<Vertex2D>();
		List<Vertex2D> bars_out = new List<Vertex2D>();
		List<Vertex2D> bars_dark = new List<Vertex2D>();
		List<Vertex2D> bars_out_dark = new List<Vertex2D>();
		for (int k = 0; k <= 30; k++)
		{
			float value = k / 30f;
			Vector2 ringPos = new Vector2(0, radius).RotatedBy(value * MathHelper.TwoPi) + Position;
			Vector2 ringPos2 = new Vector2(0, radius + 40).RotatedBy(value * MathHelper.TwoPi) + Position;
			Vector2 ringPos3 = new Vector2(0, radius + 80).RotatedBy(value * MathHelper.TwoPi) + Position;
			AddVertex(bars, ringPos, new Vector2(value * 3, 0), 0);
			AddVertex(bars, ringPos2, new Vector2(value * 3, 0.5f));

			AddVertex(bars_out, ringPos2, new Vector2(value * 3, 0.5f));
			AddVertex(bars_out, ringPos3, new Vector2(value * 3, 1f), 0);

			AddVertexDark(bars_dark, ringPos, new Vector2(value * 3, 0), 0);
			AddVertexDark(bars_dark, ringPos2, new Vector2(value * 3, 0.5f));

			AddVertexDark(bars_out_dark, ringPos2, new Vector2(value * 3, 0.5f));
			AddVertexDark(bars_out_dark, ringPos3, new Vector2(value * 3, 1f), 0);
		}
		Ins.Batch.Draw(tex_dark, bars_dark, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(tex_dark, bars_out_dark, PrimitiveType.TriangleStrip);

		Ins.Batch.Draw(tex, bars, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(tex, bars_out, PrimitiveType.TriangleStrip);
	}

	public void AddVertex(List<Vertex2D> bars, Vector2 worldPos, Vector2 coord, float colorFade = 1f)
	{
		float timeValue = Timer / 60f;
		Color color = Lighting.GetColor(worldPos.ToTileCoordinates(), new Color(0.7f, 0.1f, 0.4f, 0f)) * colorFade * Fade * Fade;
		color.A = 0;
		bars.Add(worldPos, color, new Vector3(coord.X, coord.Y + timeValue, 0));
	}

	public void AddVertexDark(List<Vertex2D> bars, Vector2 worldPos, Vector2 coord, float colorFade = 1f)
	{
		float timeValue = Timer / 60f;
		bars.Add(worldPos, Color.White * colorFade * Fade, new Vector3(coord.X, coord.Y + timeValue, 0));
	}
}
