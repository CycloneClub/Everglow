namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class FlameDust0 : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawPlayers;

	public Player MyOwner;
	public Vector2 Position;
	public Vector2 Velocity;
	public Vector2 StartPos = Vector2.zeroVector;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public int Frame = 0;

	public override void OnSpawn()
	{
		base.OnSpawn();
	}

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
		if (StartPos == Vector2.zeroVector && MyOwner != null)
		{
			StartPos = MyOwner.Center;
		}
		Position += Velocity;
		Velocity *= 0.9f;
		Frame = (int)(Timer / MaxTime * 5f);
		Lighting.AddLight(Position, new Vector3(0.9f, 0.6f, 0f));
	}

	public override void Draw()
	{
		float frameCount = 5;
		float frameY = Frame;
		float xCount = 3;
		float frameX = ai[0] / xCount;
		float frameCoordWidth = 1f / xCount;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		var drawColor = new Color(1f, 1f, 1f, 1f);
		var postOffsetPos = Position + MyOwner.Center - StartPos;
		if (ai[1] == 1)
		{
			postOffsetPos = Position;
		}
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(postOffsetPos + toCorner, drawColor, new Vector3(frameX, frameY / frameCount, 0)),
			new Vertex2D(postOffsetPos + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(frameX + frameCoordWidth, frameY / frameCount, 0)),
			new Vertex2D(postOffsetPos + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(frameX, (frameY + 1) / frameCount, 0)),

			new Vertex2D(postOffsetPos + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(frameX, (frameY + 1) / frameCount, 0)),
			new Vertex2D(postOffsetPos + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(frameX + frameCoordWidth, frameY / frameCount, 0)),
			new Vertex2D(postOffsetPos + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(frameX + frameCoordWidth, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.FlameDust0.Value, bars, PrimitiveType.TriangleList);
	}
}
