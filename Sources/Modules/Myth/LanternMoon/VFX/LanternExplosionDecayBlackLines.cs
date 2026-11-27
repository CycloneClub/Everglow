namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(BlackDecayPipeline), typeof(BloomPipeline))]
public class LanternExplosionDecayBlackLines : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public override void Update()
	{
		if (Scale < 1280)
		{
			Scale += ai[2];
		}
		ai[2] *= 0.9f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		pocession = MathF.Pow(pocession, 0.4f);
		if (Timer <= 2)
		{
			if (Scale <= 160)
			{
				pocession = 0;
			}
			else
			{
				pocession *= 0.5f;
			}
		}
		float timeValue = (float)(Main.timeForVisualEffects * 0.01f);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		float light = 1f;
		float scaleTex = 1.2f;
		float yTex = ai[1] + timeValue;
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, pocession, pocession), new Vector3(ai[0], yTex, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, pocession, pocession), new Vector3(ai[0], yTex + scaleTex, light)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, pocession, pocession), new Vector3(ai[0] + scaleTex, yTex, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession, pocession), new Vector3(ai[0] + scaleTex, yTex + scaleTex, light)),
		};
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
