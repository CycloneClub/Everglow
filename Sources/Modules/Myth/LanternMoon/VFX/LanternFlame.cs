namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(LanternFlamePipeline), typeof(BloomPipeline))]
public class LanternFlameDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float RotateSpeed;
	public float Rotation;

	public override void Update()
	{
		Position += Velocity;
		Velocity *= 0.9f;

		if (Scale < 160)
		{
			Scale += 2f;
		}
		Timer++;
		Rotation += RotateSpeed;
		RotateSpeed *= 0.96f;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
		float value = Timer / MaxTime;
		var color = Vector3.Lerp(new Vector3(2f, 1.4f, 1f), new Vector3(0.5f, 0, 0), value);
		Lighting.AddLight(Position, color * Scale * 0.02f * (1 - value));
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		float timeValue = (float)(Main.timeForVisualEffects * 0.002);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		float light = 1f;
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, pocession), new Vector3(ai[0], timeValue, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, pocession), new Vector3(ai[0], timeValue + 0.4f, light)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, pocession), new Vector3(ai[0] + 0.4f, timeValue, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession), new Vector3(ai[0] + 0.4f, timeValue + 0.4f, light)),
		};
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
