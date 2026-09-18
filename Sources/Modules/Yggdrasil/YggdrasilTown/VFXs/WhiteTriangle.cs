namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class WhiteTriangle : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;

	public override void Update()
	{
		position += velocity;
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		velocity *= 0.99f;
		velocity += new Vector2(0, Main.rand.NextFloat(0.04f)).RotatedByRandom(6.283);
		velocity.Y -= 0.01f;
		if (velocity.Y > 0)
		{
			velocity.Y *= 0.8f;
		}
		scale = ai[0] * MathF.Sin(timer / maxTime * MathF.PI);
		timer++;
		if (timer > maxTime)
		{
			Active = false;
			return;
		}

		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= -0.2f;
			timer += 10;
		}
		rotation += ai[1];
		Lighting.AddLight(position, scale * 0.01f, scale * 0.01f, scale * 0.01f);
	}

	public override void Draw()
	{
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		Color lightColor = Color.White;

		// lightColor = Color.Lerp(color2, color1, Math.Clamp((606 * 16 - position.X) / (8f * 16),0, 1));
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
				new Vertex2D(position + deltaY, lightColor, new Vector3(0, 0, 0)),
				new Vertex2D(position + deltaY + toCorner.RotatedBy(Math.PI * 0.5 + rotation), lightColor, new Vector3(0, 1, 0)),
				new Vertex2D(position + deltaY + toCorner.RotatedBy(Math.PI * 0 + rotation), lightColor, new Vector3(1, 0, 0)),
			};
			Ins.Batch.Draw(Terraria.GameContent.TextureAssets.MagicPixel.Value, bars, PrimitiveType.TriangleList);
		}
	}
}
