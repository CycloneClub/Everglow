using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class IchorSplashPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.IchorSplash;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(ModAsset.Noise_cell.Value);
		Texture2D flameColor = ModAsset.HeatMap_ichorSplash.Value;
		Ins.Batch.BindTexture<Vertex2D>(flameColor);
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.LinearClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(IchorSplashPipeline), typeof(BloomPipeline))]
public class IchorSplash : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> OldPositions = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Alpha;

	public IchorSplash()
	{
	}

	public override void Update()
	{
		Position += Velocity * 0.001f;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Active = false;
			return;
		}
		OldPositions.Add(Position);
		if (OldPositions.Count > 15)
		{
			OldPositions.RemoveAt(0);
		}

		Velocity.Y += 0.14f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);
		Scale += 0.4f;
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= 0.8f;
			if (Velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		if (Main.tile[(int)(Position.X / 16f), (int)(Position.Y / 16f)].LiquidAmount > 0)
		{
			Scale += 0.02f;
			Alpha += 0.004f;
			Velocity *= 0.9f;
			if (MathF.Abs(Velocity.X) > 2)
			{
				Velocity.X *= 0.8f;
			}
			Velocity += new Vector2(Main.rand.NextFloat(0.5f), 0).RotatedByRandom(6.283) + new Vector2(0, 1.2f - Math.Abs(Velocity.X) * Alpha * 2f);
			Position += Velocity * 0.5f;
			Timer -= 0.4f;
		}
		else
		{
			Position += Velocity;
		}
		float progress = 1 - Timer / MaxTime;
		float c = progress * Scale * 0.04f;
		Lighting.AddLight(Position, c * 0.8f, c * 0.4f, 0);
	}

	public override void Draw()
	{
		Vector2[] pos = OldPositions.Reverse<Vector2>().ToArray();
		float progress = Timer / MaxTime;
		int len = pos.Length;
		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPositions[i] - OldPositions[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			float width = Scale * (float)Math.Sin(i / (double)len * Math.PI);
			Vector2 pointUp = OldPositions[i] + normal * width;
			Vector2 pointDown = OldPositions[i] - normal * width;
			Vector2 widthUp = new Vector2(normal.X * width, 0);
			Vector2 widthDown = -new Vector2(normal.X * width, 0);

			if (pointUp.X <= 320 || pointUp.X >= Main.maxTilesX * 16 - 320)
			{
				Active = false;
				return;
			}
			if (pointUp.Y <= 320 || pointUp.Y >= Main.maxTilesY * 16 - 320)
			{
				Active = false;
				return;
			}

			if (pointDown.X <= 320 || pointDown.X >= Main.maxTilesX * 16 - 320)
			{
				Active = false;
				return;
			}
			if (pointDown.Y <= 320 || pointDown.Y >= Main.maxTilesY * 16 - 320)
			{
				Active = false;
				return;
			}

			bars.Add(OldPositions[i] + normal * width + widthUp, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 17f, progress));
			bars.Add(OldPositions[i] - normal * width + widthDown, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0.6f + ai[0], (i + 15 - len) / 17f, progress));
		}
		if (len <= 2)
		{
			for (int i = 1; i < 3; i++)
			{
				var lightColorWithPos = new Color(1f, 1f, 1f, 0);
				bars.Add(Position, lightColorWithPos, new Vector3(0, (i + 15 - len) / 75f + Timer / 15000f, progress));
				bars.Add(Position, lightColorWithPos, new Vector3(1, (i + 15 - len) / 75f + Timer / 15000f, progress));
			}
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
