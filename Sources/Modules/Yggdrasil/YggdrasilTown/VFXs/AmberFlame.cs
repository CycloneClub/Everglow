namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class AmberFlamePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.AmberFlame;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.AmberFlame_heatMap.Value);
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

[Pipeline(typeof(AmberFlamePipeline), typeof(BloomPipeline))]
public class AmberFlameDust : Visual
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
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
		}
		Velocity *= 0.98f;
		Scale *= 0.96f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Velocity = Velocity.RotatedBy(ai[1]);
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
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.3f;
		Lighting.AddLight(Position, c * 0.7f, c * 0.5f, 0);
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color lightColor = Lighting.GetColor((int)(Position.X / 16f), (int)(Position.Y / 16f));
		var normalVel = Vector2.Normalize(Velocity);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + normalVel * 8 * Scale + toCorner, new Color(0, 0f, pocession, 0.0f), lightColor.ToVector3()),
			new Vertex2D(Position + normalVel * 8 * Scale + toCorner.RotatedBy(Math.PI * 0.5), new Color(0, 1f, pocession, 0.0f), lightColor.ToVector3()),

			new Vertex2D(Position - normalVel * 8 * Scale + toCorner.RotatedBy(Math.PI * 1.5), new Color(1, 0f, pocession, 0.0f), lightColor.ToVector3()),
			new Vertex2D(Position - normalVel * 8 * Scale + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1f, pocession, 0.0f), lightColor.ToVector3()),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
