using Everglow.Myth.Common;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;

namespace Everglow.Myth.TheTusk.VFXs;

internal class BloodLiquidPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModContent.Request<Effect>("Everglow/Myth/TheTusk/VFXs/BloodLiquidVFX", AssetRequestMode.ImmediateLoad);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(ModAsset.HiveCyberNoise.Value);
		Texture2D FlameColor = ModAsset.BloodLiquid_Color.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

internal class ThickBloodLiquidPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModContent.Request<Effect>("Everglow/Myth/TheTusk/VFXs/BloodLiquidVFX", AssetRequestMode.ImmediateLoad);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(ModAsset.HiveCyberNoiseThicker.Value);
		Texture2D FlameColor = ModAsset.BloodLiquid_Color_Thicker.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(BloodLiquidPipeline))]
internal class BloodLiquidDust : ShaderDraw
{
	private Vector2 vsadd = Vector2.Zero;
	public List<Vector2> oldPos = new List<Vector2>();
	public float timer;
	public float maxTime;
	public float alpha;

	public BloodLiquidDust() { }

	public BloodLiquidDust(int maxTime, Vector2 position, Vector2 velocity, params float[] ai)
		: base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		position += velocity * 0.001f;
		oldPos.Add(position);
		if (oldPos.Count > 15)
		{
			oldPos.RemoveAt(0);
		}

		velocity.Y += 0.21f;
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);

		for (int f = oldPos.Count - 1; f > 0; f--)
		{
			if (oldPos[f] != Vector2.Zero)
			{
				oldPos[f] += vsadd;
			}
		}
		ai[2] += 0.4f;
		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= 0.2f;
			if (velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		if (Main.tile[(int)(position.X / 16f), (int)(position.Y / 16f)].LiquidAmount > 0)
		{
			ai[2] += 0.02f;
			alpha += 0.004f;
			velocity *= 0.9f;
			if (MathF.Abs(velocity.X) > 2)
			{
				velocity.X *= 0.8f;
			}
			velocity += new Vector2(Main.rand.NextFloat(0.5f), 0).RotatedByRandom(6.283) + new Vector2(0, 1.2f - Math.Abs(velocity.X) * alpha * 2f);
			position += velocity * 0.5f;
			timer -= 0.4f;
		}
		else
		{
			position += velocity;
		}
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float fx = timer / maxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			Color light = Lighting.GetColor((int)(oldPos[i].X / 16f), (int)(oldPos[i].Y / 16f));

			var drawcRope = new Color(Math.Min(fx * fx * fx + 0.2f - i / (float)len, 0.8f), light.R / 255f * (1 - alpha), light.G / 255f * (1 - alpha), light.B / 255f * (1 - alpha));
			float width = ai[2] * (float)Math.Sin(i / (double)len * Math.PI);
			Vector2 pointUp = oldPos[i] + normal * width;
			Vector2 pointDown = oldPos[i] - normal * width;
			Vector2 widthUp = new Vector2(normal.X * width, 0);
			Vector2 widthDown = -new Vector2(normal.X * width, 0);
			if (Main.tile[(int)(pointUp.X / 16f), (int)(pointUp.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthUp *= MathF.Sqrt(alpha) * 4f;
			}
			else
			{
				widthUp *= 0f;
			}
			if (Main.tile[(int)(pointDown.X / 16f), (int)(pointDown.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthDown *= MathF.Sqrt(alpha) * 4f;
			}
			else
			{
				widthDown *= 0f;
			}

			float drawAlpha = MathF.Pow(1 - alpha, 3f);
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width + widthUp, drawcRope, new Vector3(0 + ai[0], (i + 15 - len) / 30f + timer / 1500f * velocity.Length() * drawAlpha, drawAlpha));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width + widthDown, drawcRope, new Vector3(0.4f + ai[0], (i + 15 - len) / 30f + timer / 1500f * velocity.Length() * drawAlpha, drawAlpha));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(ThickBloodLiquidPipeline))]
internal class ThickBloodLiquidDust : ShaderDraw
{
	private Vector2 vsadd = Vector2.Zero;
	public List<Vector2> oldPos = new List<Vector2>();
	public float timer;
	public float maxTime;
	public float alpha;

	public ThickBloodLiquidDust() { }

	public ThickBloodLiquidDust(int maxTime, Vector2 position, Vector2 velocity, params float[] ai)
		: base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		position += velocity * 0.001f;
		oldPos.Add(position);
		if (oldPos.Count > 15)
		{
			oldPos.RemoveAt(0);
		}

		velocity.Y += 0.21f;
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);

		for (int f = oldPos.Count - 1; f > 0; f--)
		{
			if (oldPos[f] != Vector2.Zero)
			{
				oldPos[f] += vsadd;
			}
		}
		ai[2] += 0.4f;
		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= 0.2f;
			if (velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		if (Main.tile[(int)(position.X / 16f), (int)(position.Y / 16f)].LiquidAmount > 0)
		{
			ai[2] += 0.01f;
			alpha += 0.004f;
			velocity *= 0.9f;
			if (MathF.Abs(velocity.X) > 2)
			{
				velocity.X *= 0.8f;
			}
			velocity += new Vector2(Main.rand.NextFloat(0.5f), 0).RotatedByRandom(6.283) + new Vector2(0, 1.2f - Math.Abs(velocity.X) * alpha * 2f);
			position += velocity * 0.5f;
			timer -= 0.4f;
		}
		else
		{
			position += velocity;
		}
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float fx = timer / maxTime;
		int len = pos.Length;
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			Color light = Lighting.GetColor((int)(oldPos[i].X / 16f), (int)(oldPos[i].Y / 16f));

			var drawcRope = new Color(Math.Min(fx * fx * fx + 0.2f - i / (float)len, 0.8f), light.R / 255f * (1 - alpha), light.G / 255f * (1 - alpha), light.B / 255f * (1 - alpha));
			float width = ai[2] * (float)Math.Sin(i / (double)len * Math.PI / 2f + Math.PI / 2f);
			Vector2 pointUp = oldPos[i] + normal * width;
			Vector2 pointDown = oldPos[i] - normal * width;
			Vector2 widthUp = new Vector2(normal.X * width, 0);
			Vector2 widthDown = -new Vector2(normal.X * width, 0);
			if (Main.tile[(int)(pointUp.X / 16f), (int)(pointUp.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthUp *= alpha * 3f;
			}
			else
			{
				widthUp *= 0f;
			}
			if (Main.tile[(int)(pointDown.X / 16f), (int)(pointDown.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthDown *= alpha * 3f;
			}
			else
			{
				widthDown *= 0f;
			}
			float drawAlpha = MathF.Pow(1 - alpha, 3f);
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width + widthUp, drawcRope, new Vector3(0 + ai[0], (i + 15 - len) / 30f + timer / 1500f * velocity.Length(), drawAlpha));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width + widthDown, drawcRope, new Vector3(0.4f + ai[0], (i + 15 - len) / 30f + timer / 1500f * velocity.Length(), drawAlpha));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
