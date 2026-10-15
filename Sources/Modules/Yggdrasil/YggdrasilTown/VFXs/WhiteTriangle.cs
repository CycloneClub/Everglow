namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class WhiteTriangle : Visual
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
		Velocity *= 0.99f;
		Velocity += new Vector2(0, Main.rand.NextFloat(0.04f)).RotatedByRandom(6.283);
		Velocity.Y -= 0.01f;
		if (Velocity.Y > 0)
		{
			Velocity.Y *= 0.8f;
		}
		Scale = ai[0] * MathF.Sin(Timer / MaxTime * MathF.PI);
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= -0.2f;
			Timer += 10;
		}
		Rotation += ai[1];
		Lighting.AddLight(Position, Scale * 0.01f, Scale * 0.01f, Scale * 0.01f);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color lightColor = Color.White;

		// lightColor = Color.Lerp(color2, color1, Math.Clamp((606 * 16 - Position.X) / (8f * 16),0, 1));
		int maxLength = 15;
		for (int y = 0; y < maxLength; y++)
		{
			if (y == 1)
			{
				lightColor *= 0.4f;
			}
			Vector2 deltaY = new Vector2(0, -y * 2);
			lightColor *= 0.8f;
			List<Vertex2D> bars = new List<Vertex2D>()
			{
				new Vertex2D(Position + deltaY, lightColor, new Vector3(0, 0, 0)),
				new Vertex2D(Position + deltaY + toCorner.RotatedBy(Math.PI * 0.5 + Rotation), lightColor, new Vector3(0, 1, 0)),
				new Vertex2D(Position + deltaY + toCorner.RotatedBy(Math.PI * 0 + Rotation), lightColor, new Vector3(1, 0, 0)),
			};
			Ins.Batch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, bars, PrimitiveType.TriangleList);
		}
	}
}
