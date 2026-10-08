namespace Everglow.Myth.TheFirefly.VFXs;

public class MothBlueFirePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.MothBlueFire;
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_perlin.Value);
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.MothBlueFireColor.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
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

[Pipeline(typeof(MothBlueFirePipeline), typeof(BloomPipeline))]
public class MothBlueFireDust : Visual
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
		Velocity *= 0.9f;
		if (ai.Length >= 3)
		{
			Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, -0.1f) * ai[2];
		}
		else
		{
			Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, -0.1f);
		}
		if (Scale < 160)
		{
			Scale += 2f;
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Timer++;
		}
		float progress = 1 - Timer / MaxTime;
		float c = progress * Scale * 0.02f;
		Lighting.AddLight(Position, 0, 0, c);
	}

	public override void Draw()
	{
		float progress = Timer / MaxTime;
		float timeValue = (float)(Main.time * 0.002);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, new Color(0, 0, progress,  0.1f), new Vector3(ai[0], timeValue, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1, progress,  0.1f), new Vector3(ai[0], timeValue + 0.4f, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0, progress,  0.1f), new Vector3(ai[0] + 0.4f, timeValue, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, progress, 0.1f), new Vector3(ai[0] + 0.4f, timeValue + 0.4f, 0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
