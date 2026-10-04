namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class Fevens_LaserSpark : Visual
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

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Position += Velocity;
		Rotation = Velocity.ToRotation();
		Velocity *= 0.96f;
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		Vector2 width = new Vector2(0, Scale * 3f).RotatedBy(Rotation);
		Vector2 height = new Vector2(Scale * MathF.Max(3f, Velocity.Length() * 15), 0).RotatedBy(Rotation);
		Color drawColor = new Color(1f, 0.3f, 0.3f, 0);
		if (pocession < 0.8f)
		{
			drawColor = Color.Lerp(drawColor, new Color(1f, 0, 0, 0), (pocession - 0.2f) / 0.6f);
		}
		if (pocession < 0.2f)
		{
			drawColor = Color.Lerp(new Color(1f, 0, 0, 0), Color.Transparent, 1 - pocession / 0.2f);
		}
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position - width - height, drawColor, new Vector3(0, 0, 0)),
			new Vertex2D(Position + width - height, drawColor, new Vector3(1, 0, 0)),
			new Vertex2D(Position - width + height, drawColor, new Vector3(0, 1, 0)),

			new Vertex2D(Position - width + height, drawColor, new Vector3(0, 1, 0)),
			new Vertex2D(Position + width - height, drawColor, new Vector3(1, 0, 0)),
			new Vertex2D(Position + width + height, drawColor, new Vector3(1, 1, 0)),
		};
		Ins.Batch.Draw(ModAsset.Fevens_ArrowTrail.Value, bars, PrimitiveType.TriangleList);
	}
}
