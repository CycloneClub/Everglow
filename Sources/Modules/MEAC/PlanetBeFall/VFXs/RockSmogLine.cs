using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;

namespace Everglow.MEAC.PlanetBeFall.VFXs;

public class RockSmogLinePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.RockSmogLine;
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_0.Value);
		effect.Value.Parameters["uLine"].SetValue(Commons.ModAsset.TrailV.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Ins.Batch.BindTexture<Vertex2D>(ModAsset.RockSmogLine_Heatmap.Value);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(RockSmogLinePipeline))]
public class RockSmogLine : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts; // 这个绘制层在火焰之之后，被火焰覆盖

	public List<Vector2> OldPositions = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Alpha;

	public RockSmogLine()
	{
	}

	public override void Update()
	{
		OldPositions.Add(Position);
		if (OldPositions.Count > 200)
		{
			OldPositions.RemoveAt(0);
		}

		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= 0.2f;
			if (Velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		Velocity *= 0.95f;
		Position += Velocity;
	}

	public override void Draw()
	{
		Vector2[] pos = OldPositions.Reverse<Vector2>().ToArray();
		float fx = Timer / MaxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPositions[i] - OldPositions[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			Color light = Lighting.GetColor((int)(OldPositions[i].X / 16f), (int)(OldPositions[i].Y / 16f));
			var lightColorWithPos = new Color(fx * fx * fx, light.R / 255f * (1 - Alpha), light.G / 255f * (1 - Alpha), light.B / 255f * (1 - Alpha));
			float width = (float)Math.Sin(MathF.Pow((i - 1) / (float)(len - 2), 0.2f) * Math.PI);
			bars.Add(OldPositions[i] + normal * Scale, lightColorWithPos, new Vector3(0, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.3f));
			bars.Add(OldPositions[i] - normal * Scale, lightColorWithPos, new Vector3(1, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.3f));
		}
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
		}
	}
}

[Pipeline(typeof(RockSmogLinePipeline))]
public class RockSmogLine_front : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles; // 这个绘制层在火焰之前，是原来的版本

	public List<Vector2> OldPositions = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Alpha;

	public override void Update()
	{
		OldPositions.Add(Position);
		if (OldPositions.Count > 200)
		{
			OldPositions.RemoveAt(0);
		}

		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= 0.2f;
			if (Velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		Velocity *= 0.95f;
		Position += Velocity;
	}

	public override void Draw()
	{
		Vector2[] pos = OldPositions.Reverse<Vector2>().ToArray();
		float fx = Timer / MaxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPositions[i] - OldPositions[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			Color light = Lighting.GetColor((int)(OldPositions[i].X / 16f), (int)(OldPositions[i].Y / 16f));
			var lightColorWithPos = new Color(fx * fx * fx, light.R / 255f * (1 - Alpha), light.G / 255f * (1 - Alpha), light.B / 255f * (1 - Alpha));
			float width = (float)Math.Sin(MathF.Pow((i - 1) / (float)(len - 2), 0.2f) * Math.PI);
			bars.Add(OldPositions[i] + normal * Scale, lightColorWithPos, new Vector3(0, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.3f));
			bars.Add(OldPositions[i] - normal * Scale, lightColorWithPos, new Vector3(1, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.3f));
		}
		if (bars.Count > 0)
		{
			Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
		}
	}
}
