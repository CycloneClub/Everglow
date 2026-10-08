namespace Everglow.Myth.TheFirefly.VFXs;

public class MothShimmerScalePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.MothShimmerScale;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.MothBlueFireColor.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Noise_turtleCrack.Value;
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

[Pipeline(typeof(MothShimmerScalePipeline))]
public class MothShimmerScaleDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public Vector2 Coord;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float Rotation2;
	public float Omega;
	public float Phi;

	public override void Update()
	{
		ai[0] *= 0.99f;
		Position += Velocity;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
		}
		Velocity *= 0.98f;
		if (ai.Length >= 3)
		{
			Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, -0.1f) * ai[2];
		}
		else
		{
			Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, -0.1f);
		}
		Scale *= 0.98f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[0]);
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= -0.2f;
			Timer += 10;
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
	}

	public override void Draw()
	{
		float progress = Timer / MaxTime;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Vector2[] corner = new Vector2[6];
		for (int x = 0; x < 6; x++)
		{
			corner[x] = toCorner.RotatedBy(x / 3d * Math.PI);
			corner[x].Y *= MathF.Sin(Phi + (float)(Main.time * 0.03 * Omega));
			corner[x] = corner[x].RotatedBy(Rotation2);
		}
		Color lightColor = new Color(15, 45, 255, 60);
		float reflectionLight = (1 - progress) * MathF.Pow(MathF.Sin(Phi + (float)(Main.time * 0.03 * Omega + 1.57f)) + 1, 4) * 1.6f;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int x = 0; x < 3; x++)
		{
			bars.Add(new Vertex2D(Position, lightColor, new Vector3(Coord, reflectionLight)));
			bars.Add(new Vertex2D(Position + corner[2 * x], lightColor, new Vector3(Coord, reflectionLight)));

			bars.Add(new Vertex2D(Position, lightColor, new Vector3(Coord, reflectionLight)));
			bars.Add(new Vertex2D(Position + corner[2 * x + 1], lightColor, new Vector3(Coord, reflectionLight)));
		}
		bars.Add(new Vertex2D(Position, lightColor, new Vector3(Coord, reflectionLight)));
		bars.Add(new Vertex2D(Position + corner[0], lightColor, new Vector3(Coord, reflectionLight)));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
