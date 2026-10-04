namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class AmberSparkPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.AmberSpark;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_AmberSpark.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Point.Value;
		Ins.Batch.BindTexture<Vertex2D>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(AmberSparkPipeline), typeof(BloomPipeline))]
public class AmberSparkDust : Visual
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
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
		}
		Velocity *= 0.98f;

		Scale *= 0.995f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		bool collide = false;
		if (Collision.SolidCollision(Position + new Vector2(Velocity.X, 0), 0, 0))
		{
			collide = true;
			Velocity.X *= -0.5f;
		}
		if (Collision.SolidCollision(Position + new Vector2(0, Velocity.Y), 0, 0))
		{
			collide = true;
			Velocity.Y *= -0.5f;
		}
		if (!collide)
		{
			Velocity += new Vector2(0, 0.44f);
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
		float c = pocession * Scale * 0.1f;
		Lighting.AddLight(Position, c * 0.5f, c * 0.2f, 0);
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, pocession, 0.0f), new Vector3(0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, pocession, 0.0f), new Vector3(0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, pocession, 0.0f), new Vector3(0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession, 0.0f), new Vector3(0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
