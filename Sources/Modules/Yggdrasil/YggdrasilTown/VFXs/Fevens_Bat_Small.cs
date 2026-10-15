namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class Fevens_Bat_Small : Visual
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
		Rotation = MathHelper.PiOver4 * 3;
		var StartPos = new Vector2(ai[1], ai[2]);
		if ((StartPos - Position).Length() > 40 && Timer < MaxTime - 60)
		{
			Velocity = Vector2.Normalize(StartPos - Position).RotatedByRandom(0.9) * 3;
		}
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		float timeValue = (float)(Main.time * 0.24 + ai[0]);
		float frameCount = 4;
		float frameY = (int)timeValue % frameCount;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = Color.White;
		if (pocession < 0.8f)
		{
			drawColor = Color.Lerp(drawColor, Color.Red, (pocession - 0.2f) / 0.6f);
		}
		if (pocession < 0.2f)
		{
			drawColor = Color.Lerp(Color.Red, Color.Transparent, 1 - pocession / 0.2f);
		}
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.Fevens_Bat.Value, bars, PrimitiveType.TriangleList);
	}
}
