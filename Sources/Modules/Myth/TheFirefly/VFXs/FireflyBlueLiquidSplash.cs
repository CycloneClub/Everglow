namespace Everglow.Myth.TheFirefly.VFXs;

public class FireflyBlueLiquidSplashPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.FireflyBlueLiquidSplash;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_cell.Value);
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D flameColor = ModAsset.HeatMap_FireflyBlueLiquidSplash.Value;
		Ins.Batch.BindTexture<Vertex2D>(flameColor);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(FireflyBlueLiquidSplashPipeline))]
public class FireflyBlueLiquidSplash : Visual
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

	public override void Update()
	{
		Position += Velocity * 0.001f;
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
		float progress = 1 - Timer / MaxTime;
		float c = progress * Scale * 0.04f;
		Lighting.AddLight(Position, 0, c * 0.3f, c * 0.8f);
	}

	public override void Draw()
	{
		Vector2[] pos = OldPositions.Reverse<Vector2>().ToArray();
		float progress = Timer / MaxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPositions[i] - OldPositions[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			float width = Scale * (float)Math.Sin(i / (double)len * Math.PI);
			Vector2 pointUp = OldPositions[i] + normal * width;
			Vector2 pointDown = OldPositions[i] - normal * width;
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

			bars[2 * i - 1] = new Vertex2D(OldPositions[i] + normal * width + widthUp, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 17f, progress));
			bars[2 * i] = new Vertex2D(OldPositions[i] - normal * width + widthDown, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0.6f + ai[0], (i + 15 - len) / 17f, progress));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
