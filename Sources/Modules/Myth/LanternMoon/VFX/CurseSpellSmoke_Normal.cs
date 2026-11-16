namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(CurseSpellSmokePipeline))]
public class CurseSpellSmoke_Normal : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float[] ai;

	public override void Update()
	{
		Position += Velocity;
		Velocity *= 0.95f;
		if (Scale < 60)
		{
			Scale += 2f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Timer++;
		}
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		Texture2D tex = Commons.ModAsset.Point.Value;
		float timeValue = (float)(Main.time * 0.002);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0f, 0f, 0), new Vector3(ai[0], timeValue, pocession)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1f, 0), new Vector3(ai[0], timeValue + 0.4f, pocession)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1f, 0, 0), new Vector3(ai[0] + 0.4f, timeValue, pocession)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1f, 1f, 0), new Vector3(ai[0] + 0.4f, timeValue + 0.4f, pocession)),
		};
		Ins.Batch.Draw(tex, bars, PrimitiveType.TriangleStrip);
	}
}
