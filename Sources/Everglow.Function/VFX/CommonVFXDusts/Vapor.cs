using Everglow.Commons.Enums;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class VaporPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.Vapor;
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_perlin.Value);
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_vapor.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D halo = Commons.ModAsset.Point.Value;
		Ins.Batch.BindTexture<Vertex2DSmog>(halo);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(VaporPipeline), typeof(HaloPipeline))]
public class VaporDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawBG;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public VaporDust()
	{
	}

	public VaporDust(int MaxTime, Vector2 Position, Vector2 Velocity, float Scale, float Rotation, params float[] ai)
	{
		this.MaxTime = MaxTime;
		this.Position = Position;
		this.Velocity = Velocity;
		this.Scale = Scale;
		this.Rotation = Rotation;
		this.ai = ai;
	}

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
		Velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (Scale < 160)
		{
			Scale += 0.1f;
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
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color lightColor = Lighting.GetColor((int)(Position.X / 16f), (int)(Position.Y / 16f));
		Vector3 drawC = lightColor.ToVector3() * 0.2f + new Vector3(lightColor.ToVector3().Length() / 3f);
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(Position + toCorner, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), drawC),
			new Vertex2DSmog(Position + toCorner.RotatedBy(Math.PI * 0.5), new Color(1, 0, pocession, 0), new Vector3(ai[0] + 0.2f * Scale / 70f, timeValue, 0), drawC),

			new Vertex2DSmog(Position + toCorner.RotatedBy(Math.PI * 1.5), new Color(0, 1, pocession, 0), new Vector3(ai[0], timeValue + 0.2f * Scale / 70f, 0), drawC),
			new Vertex2DSmog(Position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.2f * Scale / 70f, timeValue + 0.2f * Scale / 70f, 0), drawC),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(VaporPipeline), typeof(HaloPipeline))]
public class VaporDust2 : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

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
		Velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (Scale < 160)
		{
			Scale += 0.1f;
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
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Vector2 pos0 = Position + toCorner;
		Vector2 pos1 = Position + toCorner.RotatedBy(Math.PI * 0.5);
		Vector2 pos2 = Position + toCorner.RotatedBy(Math.PI * 1.5);
		Vector2 pos3 = Position + toCorner.RotatedBy(Math.PI * 1.0);
		Color lightColor0 = Lighting.GetColor((int)(pos0.X / 16f), (int)(pos0.Y / 16f));
		Color lightColor1 = Lighting.GetColor((int)(pos1.X / 16f), (int)(pos1.Y / 16f));
		Color lightColor2 = Lighting.GetColor((int)(pos2.X / 16f), (int)(pos2.Y / 16f));
		Color lightColor3 = Lighting.GetColor((int)(pos3.X / 16f), (int)(pos3.Y / 16f));
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(pos0, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), lightColor0.ToVector3() * 0.2f + new Vector3(lightColor0.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos1, new Color(0, 1, pocession, 0), new Vector3(ai[0] + 0.4f * Scale / 70f, timeValue, 0), lightColor1.ToVector3() * 0.2f + new Vector3(lightColor1.ToVector3().Length() / 3f)),

			new Vertex2DSmog(pos2, new Color(1, 0, pocession, 0), new Vector3(ai[0], timeValue + 0.4f * Scale / 70f, 0), lightColor2.ToVector3() * 0.2f + new Vector3(lightColor2.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos3, new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.4f * Scale / 70f, timeValue + 0.4f * Scale / 70f, 0), lightColor3.ToVector3() * 0.2f + new Vector3(lightColor3.ToVector3().Length() / 3f)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(VaporPipeline), typeof(HaloPipeline))]
public class VaporDust3 : Visual
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
		Velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (Scale < 160)
		{
			Scale += 0.1f;
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
	}

	public override void Draw()
	{
		float pocession = Timer / MaxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Vector2 pos0 = Position + toCorner;
		Vector2 pos1 = Position + toCorner.RotatedBy(Math.PI * 0.5);
		Vector2 pos2 = Position + toCorner.RotatedBy(Math.PI * 1.5);
		Vector2 pos3 = Position + toCorner.RotatedBy(Math.PI * 1.0);
		Color lightColor0 = Lighting.GetColor((int)(pos0.X / 16f), (int)(pos0.Y / 16f));
		Color lightColor1 = Lighting.GetColor((int)(pos1.X / 16f), (int)(pos1.Y / 16f));
		Color lightColor2 = Lighting.GetColor((int)(pos2.X / 16f), (int)(pos2.Y / 16f));
		Color lightColor3 = Lighting.GetColor((int)(pos3.X / 16f), (int)(pos3.Y / 16f));
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(pos0, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), lightColor0.ToVector3() * 0.2f + new Vector3(lightColor0.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos1, new Color(0, 1, pocession, 0), new Vector3(ai[0] + 0.4f * Scale / 70f, timeValue, 0), lightColor1.ToVector3() * 0.2f + new Vector3(lightColor1.ToVector3().Length() / 3f)),

			new Vertex2DSmog(pos2, new Color(1, 0, pocession, 0), new Vector3(ai[0], timeValue + 0.4f * Scale / 70f, 0), lightColor2.ToVector3() * 0.2f + new Vector3(lightColor2.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos3, new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.4f * Scale / 70f, timeValue + 0.4f * Scale / 70f, 0), lightColor3.ToVector3() * 0.2f + new Vector3(lightColor3.ToVector3().Length() / 3f)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
