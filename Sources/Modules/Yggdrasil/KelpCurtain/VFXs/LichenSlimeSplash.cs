namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

public class LichenSlimeSplashPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.LichenSlimeSplash;
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_cell.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uIlluminationThreshold"].SetValue(0.99f);
		effect.Parameters["uIlluminationThresholdII"].SetValue(0.05f);
		Texture2D FlameColor = ModAsset.HeatMap_LichenSlimeSplash.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(LichenSlimeSplashPipeline))]
public class LichenSlimeSplash : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> OldPos = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Alpha;

	public override void Update()
	{
		Position += Velocity * 0.001f;
		OldPos.Add(Position);
		if (OldPos.Count > 15)
		{
			OldPos.RemoveAt(0);
		}

		Velocity += new Vector2(Main.windSpeedCurrent * 0.1f, 0.04f * Scale * 0.1f);
		Velocity = Velocity.RotatedBy(ai[1]);
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Scale += 0.4f;
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= 0.2f;
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
	}

	public override void Draw()
	{
		Vector2[] pos = OldPos.Reverse<Vector2>().ToArray();
		float fx = Timer / MaxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPos[i] - OldPos[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			Color light = Lighting.GetColor((int)(OldPos[i].X / 16f), (int)(OldPos[i].Y / 16f));
			var lightColorWithPos = new Color(fx * fx * fx, light.R / 255f * (1 - Alpha), light.G / 255f * (1 - Alpha), light.B / 255f * (1 - Alpha));
			float width = Scale * (float)Math.Sin(i / (double)len * Math.PI);
			Vector2 pointUp = OldPos[i] + normal * width;
			Vector2 pointDown = OldPos[i] - normal * width;
			Vector2 widthUp = new Vector2(normal.X * width, 0);
			Vector2 widthDown = -new Vector2(normal.X * width, 0);
			if (Main.tile[(int)(pointUp.X / 16f), (int)(pointUp.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthUp *= MathF.Sqrt(Alpha) * 4f;
			}
			else
			{
				widthUp *= 0f;
			}
			if (Main.tile[(int)(pointDown.X / 16f), (int)(pointDown.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthDown *= MathF.Sqrt(Alpha) * 4f;
			}
			else
			{
				widthDown *= 0f;
			}

			bars[2 * i - 1] = new Vertex2D(OldPos[i] + normal * width + widthUp, lightColorWithPos, new Vector3(0 + ai[0], (i + 15 - len) / ai[2] + Timer / 1500f * Velocity.Length(), 0.3f + ai[0]));
			bars[2 * i] = new Vertex2D(OldPos[i] - normal * width + widthDown, lightColorWithPos, new Vector3(0.6f + ai[0], (i + 15 - len) / ai[2] + Timer / 1500f * Velocity.Length(), 0.3f + ai[0]));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
