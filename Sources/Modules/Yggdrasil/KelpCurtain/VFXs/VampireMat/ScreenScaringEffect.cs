namespace Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;

[Pipeline(typeof(ScreenScaringEffectPipeline))]
public class ScreenScaringEffect : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public int Timer;
	public int MaxTime;

	private readonly Vertex2D[] vertices = new Vertex2D[8];

	public override void Update()
	{
		Timer++;
		if (Timer >= MaxTime)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		if (MaxTime <= 0 || Timer >= MaxTime)
		{
			return;
		}
		float timeValue = MaxTime - Timer;
		float fade = timeValue / MaxTime;
		float offset = MathF.Sin(Main.GlobalTimeWrappedHourly * 2.4f) * MathF.Sqrt(timeValue) * 2;
		BuildOverlayVertices(vertices, new Vector2(Main.screenWidth, Main.screenHeight), offset, fade);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 0, 2);
		Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices, 4, 2);
	}

	/// <summary>
	/// Keep the original byte-quantized fade colors. The shader applies the five additive
	/// repetitions in float precision, avoiding both repeated screen draws and Color saturation.
	/// </summary>
	public static void BuildOverlayVertices(Span<Vertex2D> output, Vector2 size, float offset, float fade)
	{
		for (int channel = 0; channel < 2; channel++)
		{
			float x = channel == 0 ? offset : -offset;
			Color color = (channel == 0 ? new Color(0.8f, 0f, 0f, 0) : new Color(0f, 0.4f, 0f, 0)) * fade;
			int start = channel * 4;
			output[start] = new Vertex2D(new Vector2(x, 0), color, new Vector3(0, 0, 5));
			output[start + 1] = new Vertex2D(new Vector2(x + size.X, 0), color, new Vector3(1, 0, 5));
			output[start + 2] = new Vertex2D(new Vector2(x, size.Y), color, new Vector3(0, 1, 5));
			output[start + 3] = new Vertex2D(new Vector2(x + size.X, size.Y), color, new Vector3(1, 1, 5));
		}
	}
}
