namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(JellyBallGelDropPipeline))]
public class JellyBallGelDrop : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public JellyBallGelDrop()
	{
	}

	public override void Update()
	{
		Position += Velocity;
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
		Velocity *= 0.98f;
		Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.21f * Scale * 0.1f);
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= -0.02f;
			if (Ins.VisualQuality.Low)
			{
				Timer += 4;
			}
			else
			{
				if (!Main.rand.NextBool(4))
				{
					Timer -= 1;
				}
			}
		}
		var tile = Main.tile[(int)(Position.X / 16), (int)(Position.Y / 16)];
		if (Position.Y % 1 < tile.LiquidAmount / 256f)
		{
			Timer += 120;
		}
		if (Scale < 0.5f)
		{
			Timer += 20;
		}
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.08f;
		Lighting.AddLight(Position, 0, c * 0.1f, c * 0.8f);
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime * 0.6f;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color lightColor = Color.White;
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + Velocity + toCorner, lightColor, new Vector3(0, 0, pocession)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), lightColor, new Vector3(0, 1, pocession)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), lightColor, new Vector3(1, 0, pocession)),
			new Vertex2D(Position - Velocity * ai[1] + toCorner.RotatedBy(Math.PI * 1), lightColor, new Vector3(1, 1, pocession)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
