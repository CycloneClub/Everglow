namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class ArmorPiercingSpark : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public int Frame = 0;

	public override void Update()
	{
		Timer++;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Position += Velocity;
		Velocity *= 0.98f;
		if (!Collision.SolidCollision(Position - new Vector2(Scale) * 0.5f, (int)Scale, (int)Scale))
		{
			Velocity.Y += 0.25f;
		}
		else
		{
			ai[0] = 3;
			Velocity *= 0;
		}
		if (ai[0] > 2)
		{
			if (Timer % 2 == 0)
			{
				Frame++;
			}
		}
		if (Main.rand.NextBool(15))
		{
			ai[0] = 3;
		}
		if (Frame >= 3)
		{
			Active = false;
			return;
		}
		Lighting.AddLight(Position, new Vector3(0.9f, 1f, 0.8f));
	}

	public override void Draw()
	{
		float frameCount = 4;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		var drawColor = new Color(1f, 1f, 1f, 0);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.ArmorPiercingSpark.Value, bars, PrimitiveType.TriangleList);
	}
}
