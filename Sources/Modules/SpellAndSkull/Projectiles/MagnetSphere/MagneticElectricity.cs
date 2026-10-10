using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.SpellAndSkull.Projectiles.MagnetSphere;

internal abstract class ShaderDraw : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;

	public ShaderDraw()
	{
	}

	public ShaderDraw(Vector2 position, Vector2 velocity, params float[] ai)
	{
		this.position = position;
		this.velocity = velocity;
		this.ai = ai; // 可以认为params传入的都是右值，可以直接引用
	}
}

internal class MagneticElectricityPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.CursedFlame;
		effect.Value.Parameters["uNoise"].SetValue(ModAsset.Perlin.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.ZoomMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D FlameColor = ModAsset.Magnetic_Color.Value;
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

[Pipeline(typeof(MagneticElectricityPipeline), typeof(BloomPipeline))]
internal class MagneticElectricity : ShaderDraw
{
	public List<Vector2> oldPos = new List<Vector2>();
	public float timer;
	public float maxTime;

	public MagneticElectricity()
	{
	}

	public MagneticElectricity(int maxTime, Vector2 position, Vector2 velocity, params float[] ai)
		: base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		for (int a = 0; a < 6; a++)
		{
			position += velocity;
			if (Main.rand.NextBool(16))
			{
				velocity = velocity.RotatedBy(Main.rand.NextFloat(-1.2f, 1.2f));
			}

			oldPos.Add(position);
			if (oldPos.Count > 60)
			{
				oldPos.RemoveAt(0);
			}

			velocity *= 0.99f;
			timer++;
			if (timer > maxTime)
			{
				Active = false;
			}

			velocity = velocity.RotatedBy(ai[1]);

			float delC = ai[2] * 0.05f * (float)Math.Sin((maxTime - timer) / 40d * Math.PI);
			Lighting.AddLight((int)(position.X / 16), (int)(position.Y / 16), 0.45f * delC, 0.85f * delC, 0f);
			if (Collision.SolidCollision(position, 0, 0))
			{
				velocity *= 0.5f;
				timer += 2;
			}
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
			var drawcRope = new Color(fx * fx * fx * 2, 0.5f, 1, 150 / 255f);
			float width = ai[2];
			if (i > len - 10)
			{
				width *= (len - i) / 10f;
			}

			if (i < 70)
			{
				// width *= 10 / (float)i;
				width *= i / 70f;
			}

			if (timer > maxTime - 10)
			{
				width *= (maxTime - timer) / 10f;
			}

			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width, drawcRope, new Vector3(0 + ai[0], i / 320f, 0));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width, drawcRope, new Vector3(0.05f + ai[0], i / 320f, 0));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}

	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;
}
