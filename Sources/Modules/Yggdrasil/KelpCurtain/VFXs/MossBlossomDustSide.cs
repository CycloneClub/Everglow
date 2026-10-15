namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class MossBlossomDustSide : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float maxScale;
	public float Rotation;
	public int Frame = 0;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Position += Velocity;
		Velocity *= 0.9f;
		Frame = (int)(Timer / MaxTime * 6f);
		if (Frame >= 6)
		{
			Frame = 5;
		}
	}

	public override void Draw()
	{
		float frameCount = 6;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates());
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.MossBlossomDustSide.Value, bars, PrimitiveType.TriangleList);
	}
}
