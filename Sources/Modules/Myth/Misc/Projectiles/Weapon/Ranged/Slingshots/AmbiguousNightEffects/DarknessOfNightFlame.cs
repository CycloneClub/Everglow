using ReLogic.Content;

namespace Everglow.Myth.Misc.Projectiles.Weapon.Ranged.Slingshots.AmbiguousNightEffects;

internal abstract class ShaderDraw : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;

	public ShaderDraw() { }

	public ShaderDraw(Vector2 position, Vector2 velocity, params float[] ai)
	{
		this.position = position;
		this.velocity = velocity;
		this.ai = ai; // 可以认为params传入的都是右值，可以直接引用
	}
}

internal class DarknessOfNightPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModContent.Request<Effect>("Everglow/Myth/Misc/Projectiles/Weapon/Ranged/Slingshots/AmbiguousNightEffects/DarknessOfNightFlame", AssetRequestMode.ImmediateLoad);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(ModAsset.HiveCyberNoise.Value);
		effect.Parameters["uPowder"].SetValue(ModAsset.NoiseSand.Value);
		Texture2D FlameColor = ModAsset.DarknessOfNight_Color.Value;
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

[Pipeline(typeof(DarknessOfNightPipeline), typeof(BloomPipeline))]
internal class DarknessOfNightDust : ShaderDraw
{
	/// <summary>
	/// ai[0]x相位
	/// ai[1]角速度
	/// ai[2]宽度
	/// </summary>
	private Vector2 vsadd = Vector2.Zero;
	public List<Vector2> oldPos = new List<Vector2>();
	public float timer;
	public float maxTime;

	public DarknessOfNightDust() { }

	public DarknessOfNightDust(int maxTime, Vector2 position, Vector2 velocity, params float[] ai) : base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		position += velocity;
		velocity.Y += 0.045f;
		oldPos.Add(position);
		if (oldPos.Count > 15)
		{
			oldPos.RemoveAt(0);
		}

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
		float delC = ai[2] * 0.05f * (float)Math.Sin((maxTime - timer) / maxTime * Math.PI);
		Lighting.AddLight((int)(position.X / 16), (int)(position.Y / 16), 0.015f * delC, 0, 0.45f * delC);
		if (Collision.SolidCollision(position, 0, 0))
		{
			Active = false;
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
			var drawcRope = new Color(fx * fx * fx * 2 - 0.1f, 0.5f, 1, 150 / 255f);
			float width = ai[2] * (float)Math.Sin(i / (double)len * Math.PI);
			if (timer < 10)
			{
				width *= timer / 10f;
			}
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width, drawcRope, new Vector3(0 + ai[0], (i + 15 - len) / 80f, 0.8f - fx));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width, drawcRope, new Vector3(0.07f + ai[0], (i + 15 - len) / 80f, 0.8f - fx));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

[Pipeline(typeof(DarknessOfNightPipeline), typeof(BloomPipeline))]
internal class DarknessOfNightWave : ShaderDraw
{
	/// <summary>
	/// ai[0]x相位
	/// ai[1]波速
	/// ai[2]宽度
	/// </summary>
	public float timer;
	public float maxTime;
	public float radius;

	public DarknessOfNightWave() { }

	public DarknessOfNightWave(int maxTime, Vector2 position, Vector2 velocity, params float[] ai) : base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		position += velocity;
		radius += ai[1] * ((maxTime - timer) / maxTime);
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		float delC = ai[2] * 0.05f * (float)Math.Sin((maxTime - timer) / maxTime * Math.PI);
		Lighting.AddLight((int)(position.X / 16), (int)(position.Y / 16), 0.015f * delC, 0, 0.45f * delC);
	}

	public override void Draw()
	{
		float fx = timer / maxTime;
		int len = (int)(radius / 3f);
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 + 2];
		for (int i = 0; i < len + 1; i++)
		{
			Vector2 normal = new Vector2(0, 1).RotatedBy(i / (double)len * Math.PI * 2);
			Vector2 radiousDraw = normal * radius;

			var drawcRope = new Color(fx * fx * fx * 2 - 0.1f, 0.5f, 1, 150 / 255f);
			float width = ai[2];
			float texCoordWidth = 0.37f;
			if (width > radiousDraw.Length())
			{
				texCoordWidth *= radiousDraw.Length() / width;
				width = radiousDraw.Length();
			}

			bars[2 * i] = new Vertex2D(position + radiousDraw, drawcRope, new Vector3(ai[0] + timer / maxTime, i / (float)len * 2, 0.8f - fx));
			bars[2 * i + 1] = new Vertex2D(position + radiousDraw - normal * width, drawcRope, new Vector3(texCoordWidth + ai[0] + timer / maxTime, i / (float)len * 2, 0.8f - fx));
		}

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
