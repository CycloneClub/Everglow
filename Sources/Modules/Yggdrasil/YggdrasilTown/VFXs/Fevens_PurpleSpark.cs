namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class Fevens_PurpleSparkPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.Fevens_PurpleSpark;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_Fevens_PurpleSpark.Value);
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

[Pipeline(typeof(Fevens_PurpleSparkPipeline), typeof(BloomPipeline))]
public class Fevens_PurpleSparkDust : Visual
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
		Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.01f * Scale);
		Scale *= 0.99f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		Vector2 toCorner = new Vector2(Scale, 0);
		if (Velocity != Vector2.zeroVector)
		{
			toCorner = new Vector2(Scale, 0).RotatedBy(Velocity.ToRotation());
		}
		Color lightColor = new Color(0.8f, 0.3f, 1.4f, 0f);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner + Velocity * 3, new Color(0, 0f, pocession, 0.0f), lightColor.ToVector3()),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1f, pocession, 0.0f), lightColor.ToVector3()),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0f, pocession, 0.0f), lightColor.ToVector3()),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1) - Velocity, new Color(1, 1f, pocession, 0.0f), lightColor.ToVector3()),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
