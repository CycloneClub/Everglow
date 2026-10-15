namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(ColorDissolvePipeline))]
public class RedAlgaeDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public float Fade = 1f;
	public int Frame = 0;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (MaxTime - Timer < 60)
		{
			Fade = (MaxTime - Timer) / 60f;
		}
		if (Timer < 10)
		{
			Scale = Timer / 10f * MaxScale;
		}
		else
		{
			Scale = MaxScale;
		}
		Position += Velocity;
		Velocity = Velocity.RotatedBy(ai[0]);
		ai[0] *= 0.96f;
		Velocity *= 0.98f;
		if (ai.Length >= 2)
		{
			Rotation += ai[1];
		}
		else
		{
			Rotation += 0.05f;
		}
	}

	public override void Draw()
	{
		float frameCount = 4;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale * 38).RotatedBy(Rotation);
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates());
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, Fade)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, Fade)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, Fade)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, Fade)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, Fade)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, Fade)),
		};
		Ins.Batch.Draw(ModAsset.RedAlgaeDust.Value, bars, PrimitiveType.TriangleList);
	}
}
