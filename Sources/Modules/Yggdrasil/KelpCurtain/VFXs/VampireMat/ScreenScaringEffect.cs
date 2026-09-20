namespace Everglow.Yggdrasil.KelpCurtain.VFXs.VampireMat;

[Pipeline(typeof(ScreenScaringEffectPipeline))]
public class ScreenScaringEffect : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PreDrawFilter;

	public int Timer;
	public int MaxTime;

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
		float timeValue = MaxTime - Timer;
		float fade = timeValue / MaxTime;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 2; j++)
			{
				Vector2 offset = new Vector2(MathF.Sin(Main.GlobalTimeWrappedHourly * 2.4f) * MathF.Pow(timeValue, 0.5f) * 2, 0);
				Color drawColor = new Color(0.8f, 0f, 0f, 0) * fade;
				if (j == 1)
				{
					drawColor = new Color(0f, 0.4f, 0f, 0) * fade;
					offset.X *= -1;
				}
				List<Vertex2D> bars = new List<Vertex2D>();
				bars.Add(offset + new Vector2(0, 0), drawColor, new Vector3(0, 0, 0));
				bars.Add(offset + new Vector2(Main.screenWidth, 0), drawColor, new Vector3(1, 0, 0));
				bars.Add(offset + new Vector2(0, Main.screenHeight), drawColor, new Vector3(0, 1, 0));
				bars.Add(offset + new Vector2(Main.screenWidth, Main.screenHeight), drawColor, new Vector3(1, 1, 0));
				if (bars.Count == 4)
				{
					Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, bars.ToArray(), 0, 2);
				}
			}
		}
	}
}
