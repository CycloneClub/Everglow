namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

public class LichenSlimeStarPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.LichenSlimeStar;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_LichenSlimeStar.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D lightness = Commons.ModAsset.StarSlash.Value;
		Ins.Batch.BindTexture<Vertex2D>(lightness);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(LichenSlimeStarPipeline))]
public class LichenSlimeStar : Visual
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
		Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.04f * Scale * 0.1f);
		Scale *= 0.98f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position + new Vector2(Velocity.X, 0), 0, 0))
		{
			Velocity.X *= -0.4f;
			Timer += 10;
		}
		if (Collision.SolidCollision(Position + new Vector2(0, Velocity.Y), 0, 0))
		{
			Velocity.Y *= -0.4f;
			Timer += 10;
		}
		var tile = Main.tile[(int)(Position.X / 16), (int)(Position.Y / 16)];
		if (Position.Y % 1 < tile.LiquidAmount / 256f)
		{
			Timer += 120;
		}
		if (Scale < 0.05f)
		{
			Timer += 20;
		}
		Lighting.AddLight(Position, 0.35f * Scale, 0.5f * Scale, 0.25f * Scale);
	}

	public override void Draw()
	{
		Color lightColor = new Color(0.7f, 1f, 0.4f, 0);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + new Vector2(-25 * Scale, -25) * Scale, lightColor, new Vector3(0, 0, 0)),
			new Vertex2D(Position + new Vector2(-25 * Scale, 25) * Scale, lightColor, new Vector3(0, 1, 0)),
			new Vertex2D(Position + new Vector2(25 * Scale, -25) * Scale, lightColor, new Vector3(1, 0, 0)),

			new Vertex2D(Position + new Vector2(25 * Scale, -25) * Scale, lightColor, new Vector3(1, 0, 0)),
			new Vertex2D(Position + new Vector2(-25 * Scale, 25) * Scale, lightColor, new Vector3(0, 1, 0)),
			new Vertex2D(Position + new Vector2(25 * Scale, 25) * Scale, lightColor, new Vector3(1, 1, 0)),

			new Vertex2D(Position + new Vector2(-25, -25 * Scale) * Scale, lightColor, new Vector3(0, 1, 0)),
			new Vertex2D(Position + new Vector2(-25, 25 * Scale) * Scale, lightColor, new Vector3(1, 1, 0)),
			new Vertex2D(Position + new Vector2(25, -25 * Scale) * Scale, lightColor, new Vector3(0, 0, 0)),

			new Vertex2D(Position + new Vector2(25, -25 * Scale) * Scale, lightColor, new Vector3(0, 0, 0)),
			new Vertex2D(Position + new Vector2(-25, 25 * Scale) * Scale, lightColor, new Vector3(1, 1, 0)),
			new Vertex2D(Position + new Vector2(25, 25 * Scale) * Scale, lightColor, new Vector3(1, 0, 0)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleList);
	}
}
