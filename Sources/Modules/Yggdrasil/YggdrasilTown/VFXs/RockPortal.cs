namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(RockPortalPipeline))]
public class RockPortal : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;

	public RockPortal()
	{
	}

	public override void Update()
	{
		if (Scale < MaxScale)
		{
			Scale += 2f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		float timeValue = (float)(Main.time * 0.001);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, pocession), new Vector3(0, timeValue, 1)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5) * 0.5f, new Color(0, 1, pocession), new Vector3(0, timeValue + 0.4f, 1)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5) * 0.5f, new Color(1, 0, pocession), new Vector3(1, timeValue, 1)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession), new Vector3(1, timeValue + 0.4f, 1)),
		};
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
