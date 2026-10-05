namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(PurpleBombCaterpillarSmogPipeline))]
public class PurpleBombCaterpillarSmog : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

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
		Velocity *= 0.9f;

		if (Scale < 160)
		{
			Scale += 2f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		float timeValue = (float)(Main.time * 0.002);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Vector3 lightValue = Lighting.GetColor(Position.ToTileCoordinates()).ToVector3();
		float light = lightValue.Length();
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, pocession), new Vector3(ai[0], timeValue, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, pocession), new Vector3(ai[0], timeValue + 0.4f, light)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, pocession), new Vector3(ai[0] + 0.4f, timeValue, light)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession), new Vector3(ai[0] + 0.4f, timeValue + 0.4f, light)),
		};
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
