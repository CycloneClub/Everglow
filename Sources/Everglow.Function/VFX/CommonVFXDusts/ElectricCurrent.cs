using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class ElectricCurrentPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.ElectricCurrent;
		effect.Value.Parameters["uHeatMap"].SetValue(ModAsset.HeatMap_electricCurrent.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uLight"].SetValue(0.4f);
		Texture2D FlameColor = ModAsset.Trail.Value;
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

[Pipeline(typeof(ElectricCurrentPipeline))]
public class ElectricCurrent : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> oldPos = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;

	public ElectricCurrent()
	{
	}

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		if (Timer < 2)
		{
			for (int i = 0; i < 16; i++)
			{
				UpdateInside();
			}
		}
		UpdateInside();
	}

	private void UpdateInside()
	{
		if (Position.X <= 720 || Position.X >= Main.maxTilesX * 16 - 720)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 720 || Position.Y >= Main.maxTilesY * 16 - 720)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		oldPos.Add(Position);

		for (int x = 0; x < oldPos.Count; x++)
		{
			oldPos[x] += new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(6.283);
		}

		// if (Main.tile[(int)(Position.X / 16f), (int)(Position.Y / 16f)].LiquidAmount > 0)
		// {
		// Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f) / (Scale * Scale) * 108f) * Main.rand.NextFloat(0.75f, 1.25f);
		// Timer -= 0.4f;
		// Vector2 newPosX = Position + new Vector2(Velocity.X, 0);
		// if (Main.tile[(int)(newPosX.X / 16f), (int)(newPosX.Y / 16f)].LiquidAmount <= 0)
		// {
		// Dust d = Dust.NewDustDirect(Position - Velocity * 1f, 0, 0, ModContent.DustType<ElectricMiddleDust>(), 0, 0);
		// d.Scale = Main.rand.NextFloat(0.85f, 1.15f) * Scale / 300f;
		// Velocity.X *= -1;
		// Position += Velocity * 2;
		// }

		// Vector2 newPosY = Position + new Vector2(0, Velocity.Y);
		// if (Main.tile[(int)(newPosY.X / 16f), (int)(newPosY.Y / 16f)].LiquidAmount <= 0 || newPosY.Y % 16 > Main.tile[(int)(newPosY.X / 16f), (int)(newPosY.Y / 16f)].LiquidAmount / 16f)
		// {
		// Dust d = Dust.NewDustDirect(Position - Velocity * 1f, 0, 0, ModContent.DustType<ElectricMiddleDust>(), 0, 0);
		// d.Scale = Main.rand.NextFloat(0.85f, 1.15f) * Scale / 300f;
		// Velocity.Y *= -1;
		// Position += Velocity * 2;
		// }
		// }
		// else
		// {
		// Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f) / Scale * 12f) * Main.rand.NextFloat(0.75f, 1.25f);
		// }
		Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f) / Scale * 12f) * Main.rand.NextFloat(0.75f, 1.25f);
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.04f;
		Lighting.AddLight(Position, c * 0.7f, c * 0.7f, c * 0.9f);
		Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f) / Scale * 12f + ai[2]);
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float pocession = Timer / MaxTime;
		int len = pos.Length;

		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];

			Vector2 normal2 = oldPos[i] - oldPos[i - 1];
			if (i < len - 1)
			{
				normal2 = oldPos[i + 1] - oldPos[i];
			}
			normal += normal2;
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);

			float k = i / (float)len;
			bars.Add(oldPos[i] + normal * Scale, new Color(pocession + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.3f));
			bars.Add(oldPos[i] - normal * Scale, new Color(pocession + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(3.4f + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.7f));
		}
		if (bars.Count < 2)
		{
			bars.Add(Position, Color.Transparent, Vector3.zero);
			bars.Add(Position, Color.Transparent, Vector3.zero);

			bars.Add(Position, Color.Transparent, Vector3.zero);
			bars.Add(Position, Color.Transparent, Vector3.zero);
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(ElectricCurrentPipeline))]
public class ElectricCurrentDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> oldPos = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;

	public ElectricCurrentDust()
	{
	}

	public override void Update()
	{
		UpdateInside();
		UpdateInside();
	}

	private void UpdateInside()
	{
		if (Position.X <= 720 || Position.X >= Main.maxTilesX * 16 - 720)
		{
			Timer = MaxTime;
		}
		if (Position.Y <= 720 || Position.Y >= Main.maxTilesY * 16 - 720)
		{
			Timer = MaxTime;
		}
		oldPos.Add(Position);
		if (oldPos.Count > 6)
		{
			oldPos.RemoveAt(0);
		}

		for (int x = 0; x < oldPos.Count; x++)
		{
			oldPos[x] += new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(6.283);
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		if (Collision.SolidCollision(Position, 0, 0))
		{
			Velocity *= 0;
			Scale *= 0.9f;
		}
		if (Main.tile[(int)(Position.X / 16f), (int)(Position.Y / 16f)].LiquidAmount > 0)
		{
			Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f) / (Scale * Scale) * 108f) * Main.rand.NextFloat(3.75f, 4.25f);
			Timer -= 0.4f;
			Vector2 newPosX = Position + new Vector2(Velocity.X, 0);
			if (Main.tile[(int)(newPosX.X / 16f), (int)(newPosX.Y / 16f)].LiquidAmount <= 0)
			{
				Velocity.X *= -1;
				Position += Velocity * 2;
			}

			Vector2 newPosY = Position + new Vector2(0, Velocity.Y);
			if (Main.tile[(int)(newPosY.X / 16f), (int)(newPosY.Y / 16f)].LiquidAmount <= 0 || newPosY.Y % 16 > Main.tile[(int)(newPosY.X / 16f), (int)(newPosY.Y / 16f)].LiquidAmount / 16f)
			{
				Velocity.Y *= -1;
				Position += Velocity * 2;
			}
		}
		else
		{
			Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f)) * Main.rand.NextFloat(0.75f, 1.25f);
			Velocity.Y += 0.1f;
		}
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.04f;
		Lighting.AddLight(Position, c * 0.7f, c * 0.7f, c * 0.9f);
		Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f) / Scale * 48f * ai[2]);
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float pocession = Timer / MaxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];

			Vector2 normal2 = oldPos[i] - oldPos[i - 1];
			if (i < len - 1)
			{
				normal2 = oldPos[i + 1] - oldPos[i];
			}
			normal += normal2;
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);

			float k = i / (float)len;
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * Scale, new Color(pocession + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.3f));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * Scale, new Color(pocession + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(3.4f + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.7f));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
