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

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float rotation;

	public VaporDust()
	{
	}

	public VaporDust(int maxTime, Vector2 position, Vector2 velocity, float scale, float rotation, params float[] ai)
	{
		this.maxTime = maxTime;
		this.position = position;
		this.velocity = velocity;
		this.scale = scale;
		this.rotation = rotation;
		this.ai = ai;
	}

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
		velocity *= 0.9f;
		velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (scale < 160)
		{
			scale += 0.1f;
		}
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);
		if (Collision.SolidCollision(position, 0, 0))
		{
			timer++;
		}
	}

	public override void Draw()
	{
		float pocession = timer / maxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		Color lightColor = Lighting.GetColor((int)(position.X / 16f), (int)(position.Y / 16f));
		Vector3 drawC = lightColor.ToVector3() * 0.2f + new Vector3(lightColor.ToVector3().Length() / 3f);
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(position + toCorner, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), drawC),
			new Vertex2DSmog(position + toCorner.RotatedBy(Math.PI * 0.5), new Color(1, 0, pocession, 0), new Vector3(ai[0] + 0.2f * scale / 70f, timeValue, 0), drawC),

			new Vertex2DSmog(position + toCorner.RotatedBy(Math.PI * 1.5), new Color(0, 1, pocession, 0), new Vector3(ai[0], timeValue + 0.2f * scale / 70f, 0), drawC),
			new Vertex2DSmog(position + toCorner.RotatedBy(Math.PI * 1), new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.2f * scale / 70f, timeValue + 0.2f * scale / 70f, 0), drawC),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(VaporPipeline), typeof(HaloPipeline))]
public class VaporDust2 : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

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
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
		}
		velocity *= 0.9f;
		velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (scale < 160)
		{
			scale += 0.1f;
		}
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);
		if (Collision.SolidCollision(position, 0, 0))
		{
			timer++;
		}
	}

	public override void Draw()
	{
		float pocession = timer / maxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		Vector2 pos0 = position + toCorner;
		Vector2 pos1 = position + toCorner.RotatedBy(Math.PI * 0.5);
		Vector2 pos2 = position + toCorner.RotatedBy(Math.PI * 1.5);
		Vector2 pos3 = position + toCorner.RotatedBy(Math.PI * 1.0);
		Color lightColor0 = Lighting.GetColor((int)(pos0.X / 16f), (int)(pos0.Y / 16f));
		Color lightColor1 = Lighting.GetColor((int)(pos1.X / 16f), (int)(pos1.Y / 16f));
		Color lightColor2 = Lighting.GetColor((int)(pos2.X / 16f), (int)(pos2.Y / 16f));
		Color lightColor3 = Lighting.GetColor((int)(pos3.X / 16f), (int)(pos3.Y / 16f));
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(pos0, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), lightColor0.ToVector3() * 0.2f + new Vector3(lightColor0.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos1, new Color(0, 1, pocession, 0), new Vector3(ai[0] + 0.4f * scale / 70f, timeValue, 0), lightColor1.ToVector3() * 0.2f + new Vector3(lightColor1.ToVector3().Length() / 3f)),

			new Vertex2DSmog(pos2, new Color(1, 0, pocession, 0), new Vector3(ai[0], timeValue + 0.4f * scale / 70f, 0), lightColor2.ToVector3() * 0.2f + new Vector3(lightColor2.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos3, new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.4f * scale / 70f, timeValue + 0.4f * scale / 70f, 0), lightColor3.ToVector3() * 0.2f + new Vector3(lightColor3.ToVector3().Length() / 3f)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(VaporPipeline), typeof(HaloPipeline))]
public class VaporDust3 : Visual
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
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
		}
		velocity *= 0.9f;
		velocity += new Vector2(Main.windSpeedCurrent * 0.02f, -0.08f + ai[0]);
		if (scale < 160)
		{
			scale += 0.1f;
		}
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);
		if (Collision.SolidCollision(position, 0, 0))
		{
			timer++;
		}
	}

	public override void Draw()
	{
		float pocession = timer / maxTime;
		pocession = MathF.Pow(pocession, 0.3f);
		pocession = 1 - MathF.Sin(pocession * MathF.PI);
		float timeValue = (float)(Main.time * 0.0003f);
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		Vector2 pos0 = position + toCorner;
		Vector2 pos1 = position + toCorner.RotatedBy(Math.PI * 0.5);
		Vector2 pos2 = position + toCorner.RotatedBy(Math.PI * 1.5);
		Vector2 pos3 = position + toCorner.RotatedBy(Math.PI * 1.0);
		Color lightColor0 = Lighting.GetColor((int)(pos0.X / 16f), (int)(pos0.Y / 16f));
		Color lightColor1 = Lighting.GetColor((int)(pos1.X / 16f), (int)(pos1.Y / 16f));
		Color lightColor2 = Lighting.GetColor((int)(pos2.X / 16f), (int)(pos2.Y / 16f));
		Color lightColor3 = Lighting.GetColor((int)(pos3.X / 16f), (int)(pos3.Y / 16f));
		List<Vertex2DSmog> bars = new List<Vertex2DSmog>()
		{
			new Vertex2DSmog(pos0, new Color(0, 0, pocession, 0), new Vector3(ai[0], timeValue, 0), lightColor0.ToVector3() * 0.2f + new Vector3(lightColor0.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos1, new Color(0, 1, pocession, 0), new Vector3(ai[0] + 0.4f * scale / 70f, timeValue, 0), lightColor1.ToVector3() * 0.2f + new Vector3(lightColor1.ToVector3().Length() / 3f)),

			new Vertex2DSmog(pos2, new Color(1, 0, pocession, 0), new Vector3(ai[0], timeValue + 0.4f * scale / 70f, 0), lightColor2.ToVector3() * 0.2f + new Vector3(lightColor2.ToVector3().Length() / 3f)),
			new Vertex2DSmog(pos3, new Color(1, 1, pocession, 0), new Vector3(ai[0] + 0.4f * scale / 70f, timeValue + 0.4f * scale / 70f, 0), lightColor3.ToVector3() * 0.2f + new Vector3(lightColor3.ToVector3().Length() / 3f)),
		};

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
