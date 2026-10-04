namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class JellyBallElectricExplosionWave : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

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
		Velocity *= 0;
		Scale += ai[0];
		ai[0] *= 0.95f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (Collision.SolidCollision(Position - new Vector2(Scale), (int)(Scale * 2), (int)(Scale * 2)))
		{
			Position -= Velocity;
			Velocity *= 0;
			Timer += 10;
		}
		Lighting.AddLight(Position, Scale * 0.1f, 0, 0);
	}

	public override void Draw()
	{
		List<Vertex2D> bars = new List<Vertex2D>();
		int sideCount = 9;
		for (int i = 0; i <= sideCount; ++i)
		{
			Vector2 drawPos = Position;
			Color drawC = new Color(0.7f, 0.7f, 0.7f, 0.7f);
			Vector2 star = new Vector2(0, 70 * Scale).RotatedBy(i * Math.PI / sideCount * 4 + Rotation);
			float width = 60f;
			if (MaxTime - Timer < 20)
			{
				width = (MaxTime - Timer) * 3;
			}
			Vector2 innerStar = Utils.SafeNormalize(star, Vector2.zeroVector) * width;
			if (star.Length() < width)
			{
				innerStar = Vector2.zeroVector;
			}
			else
			{
				innerStar = star - innerStar;
			}
			bars.Add(new Vertex2D(drawPos + star, drawC, new Vector3(i / 4f, 0.75f, 0)));
			bars.Add(new Vertex2D(drawPos + innerStar, drawC, new Vector3(i / 4f, 0.5f, 0)));
		}
		Ins.Batch.Draw(Commons.ModAsset.Trail_0_blackWhite.Value, bars, PrimitiveType.TriangleStrip);
		bars = new List<Vertex2D>();
		for (int i = 0; i <= sideCount; ++i)
		{
			Vector2 drawPos = Position;
			Color drawC = Color.Lerp(new Color(0.7f, 0.9f, 1f, 0), new Color(0.1f, 0.4f, 0.9f, 0), Timer / MaxTime);
			Vector2 star = new Vector2(0, 70 * Scale).RotatedBy(i * Math.PI / sideCount * 4 + Rotation);
			float width = 60f;
			if (MaxTime - Timer < 20)
			{
				width = (MaxTime - Timer) * 3;
			}
			Vector2 innerStar = Utils.SafeNormalize(star, Vector2.zeroVector) * width;
			if (star.Length() < width)
			{
				innerStar = Vector2.zeroVector;
			}
			else
			{
				innerStar = star - innerStar;
			}
			bars.Add(new Vertex2D(drawPos + star, drawC, new Vector3(i / 4f, 0.25f, 0)));
			bars.Add(new Vertex2D(drawPos + innerStar, drawC, new Vector3(i / 4f, 0, 0)));
		}
		Ins.Batch.Draw(Commons.ModAsset.Trail_0_blackWhite.Value, bars, PrimitiveType.TriangleStrip);
	}
}
