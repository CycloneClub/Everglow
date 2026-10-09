namespace Everglow.Myth.Misc.Projectiles.Weapon.Melee.PrimordialJadeWinged_Spear;

public class FilthyFragilePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.FilthyFragile;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.FilthyFragile_Color.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Point.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointWrap;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(FilthyFragilePipeline))]
public class FilthyFragileDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public Vector2 coord;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;
	public float rotation2;
	public float omega;
	public float phi;

	public FilthyFragileDust() { }

	public override void Update()
	{
		position += velocity;
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
		}
		velocity *= 0.92f;
		if (timer < 40)
		{
			scale *= 0.95f;
		}
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= -0.2f;
			scale *= 0.9f;
		}
		var tile = Main.tile[(int)(position.X / 16), (int)(position.Y / 16)];
		if (position.Y % 1 < tile.LiquidAmount / 256f)
		{
			timer += 120;
		}
		if (scale < 0.5f)
		{
			timer += 20;
		}
		Lighting.AddLight(position, 0, 0.06f * scale, 0.12f * scale);
	}

	public override void Draw()
	{
		float pocession = timer / maxTime;
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		int sideCount = 5;
		if (coord.X < 0.3f)
		{
			sideCount = 3;
		}
		Vector2[] Corner = new Vector2[sideCount];
		for (int x = 0; x < sideCount; x++)
		{
			Corner[x] = toCorner.RotatedBy(x / (float)(sideCount) * Math.Tau);
			Corner[x].Y *= MathF.Sin(phi + (float)(Main.time * 0.03 * omega));
			Corner[x] = Corner[x].RotatedBy(rotation2);
		}
		Color lightColor = new Color(255, 0, 0, 0);
		float reflectionLight = (1 - pocession) * MathF.Pow((MathF.Sin(phi + (float)(Main.time * 0.03 * omega + 1.57f)) + 1), 4) * 1.6f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < sideCount; x++)
		{
			bars.Add(new Vertex2D(position, lightColor, new Vector3(0.5f, ai[0], reflectionLight)));
			bars.Add(new Vertex2D(position + Corner[x], lightColor, new Vector3(0.5f, ai[1], reflectionLight)));
		}
		bars.Add(new Vertex2D(position, lightColor, new Vector3(0.5f, ai[0], reflectionLight)));
		bars.Add(new Vertex2D(position + Corner[0], lightColor, new Vector3(0.5f, ai[1], reflectionLight)));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
