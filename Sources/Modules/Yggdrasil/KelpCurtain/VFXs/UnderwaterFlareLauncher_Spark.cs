using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(UnderwaterFlareLauncher_SparkPipeline), typeof(BloomPipeline))]
public class UnderwaterFlareLauncher_Spark : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float Temperature;
	public bool noGravity;

	public override void Update()
	{
		ai[1] *= 0.99f;
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
		if (!noGravity)
		{
			Velocity += new Vector2(0, 0.6f);
		}

		Scale *= 0.95f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
		if (Scale < 0.5f)
		{
			Timer += 20;
		}
		var tile = TileUtils.SafeGetTile(Position.ToTileCoordinates());
		if (Position.Y % 1 >= tile.LiquidAmount / 256f)
		{
			Timer += 1200;
		}
		GradientColor gradientColor = new GradientColor();
		gradientColor.colorList.Add((new Color(0.3f, 0.2f, 0.3f, 1), 0));
		gradientColor.colorList.Add((new Color(0.7f, 0.1f, 0.2f, 1), 0.2f));
		gradientColor.colorList.Add((new Color(0.5f, 0.4f, 0.3f, 1), 0.5f));
		gradientColor.colorList.Add((new Color(0.4f, 0.4f, 0.7f, 1), 0.7f));
		gradientColor.colorList.Add((new Color(1f, 1f, 1f, 1), 1f));
		Vector3 light = gradientColor.GetColor(Temperature).ToVector3();
		Lighting.AddLight(Position, light * 0.05f * Scale);
	}

	public override void Draw()
	{
		float pocession = 0.5f;
		Vector2 toCorner = new Vector2(0, Scale * 0.2f).RotatedBy(Rotation);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner + Velocity * 3, new Color(0, 0, pocession, 0.0f), new Vector3(Temperature, 0, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, pocession, 0.0f), new Vector3(Temperature, 0, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, pocession, 0.0f), new Vector3(Temperature, 0, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1) - Velocity, new Color(1, 1, pocession, 0.0f), new Vector3(Temperature, 0, 0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
