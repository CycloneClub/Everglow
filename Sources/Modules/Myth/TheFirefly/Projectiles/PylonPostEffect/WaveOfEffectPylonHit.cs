namespace Everglow.Myth.TheFirefly.Projectiles.PylonPostEffect;

public abstract class ShaderDraw : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;

	public ShaderDraw()
	{
	}

	public ShaderDraw(Vector2 position, Vector2 velocity, params float[] ai)
	{
		this.Position = position;
		this.Velocity = velocity;
		this.ai = ai; // 可以认为params传入的都是右值，可以直接引用
	}
}

public class WaveOfEffectPylonHit_CorruptPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.WaveOfEffectPylonHit_Corrupt;
		effect.Value.Parameters["uNoise"].SetValue(ModAsset.HiveCyberNoise.Value);
		effect.Value.Parameters["uPowder"].SetValue(ModAsset.NoiseSand.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D flameColor = ModAsset.WaveOfEffectPylonHit_Corrupt_Color.Value;
		Ins.Batch.BindTexture<Vertex2D>(flameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.NonPremultiplied, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(WaveOfEffectPylonHit_CorruptPipeline), typeof(BloomPipeline))]
public class WaveOfEffectPylonHit_CorruptWave : ShaderDraw
{
	/// <summary>
	/// ai[0]x相位
	/// ai[1]波速
	/// ai[2]宽度
	/// </summary>
	public float Timer;
	public float MaxTime;
	public float Radius;

	public WaveOfEffectPylonHit_CorruptWave()
	{
	}

	public WaveOfEffectPylonHit_CorruptWave(Vector2 position, Vector2 velocity, params float[] ai)
		: base(position, velocity, ai)
	{
	}

	public override void Update()
	{
		Position += Velocity;
		Radius += ai[1] * ((MaxTime - Timer) / MaxTime);
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		float delC = ai[2] * 0.05f * (float)Math.Sin((MaxTime - Timer) / MaxTime * Math.PI);
		Lighting.AddLight(Position, 0.015f * delC, 0, 0.45f * delC);
	}

	public override void Draw()
	{
		float fx = Timer / MaxTime;
		int len = (int)(Radius / 3f);
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 + 2];
		for (int i = 0; i < len + 1; i++)
		{
			Vector2 normal = new Vector2(0, 1).RotatedBy(i / (double)len * Math.PI * 2);
			Vector2 radialOffset = normal * Radius;

			var drawcRope = new Color(fx * fx * fx * 2 - 0.1f, 0.5f, 1, 150 / 255f);
			float width = ai[2];
			float texCoordWidth = 0.37f;
			if (width > radialOffset.Length())
			{
				texCoordWidth *= radialOffset.Length() / width;
				width = radialOffset.Length();
			}

			bars[2 * i] = new Vertex2D(Position + radialOffset, drawcRope, new Vector3(ai[0] + Timer / MaxTime, i / (float)len * 2, 0.8f - fx));
			bars[2 * i + 1] = new Vertex2D(Position + radialOffset - normal * width, drawcRope, new Vector3(texCoordWidth + ai[0] + Timer / MaxTime, i / (float)len * 2, 0.8f - fx));
		}

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}

internal class WaveOfEffectPylonHit_CrimsonPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.WaveOfEffectPylonHit_Crimson;
		effect.Value.Parameters["uNoise"].SetValue(ModAsset.HiveCyberNoise.Value);
		effect.Value.Parameters["uPowder"].SetValue(ModAsset.NoiseSand.Value);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D flameColor = ModAsset.WaveOfEffectPylonHit_Crimson_Color.Value;
		Ins.Batch.BindTexture<Vertex2D>(flameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.NonPremultiplied, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(WaveOfEffectPylonHit_CrimsonPipeline), typeof(BloomPipeline))]
public class WaveOfEffectPylonHit_CrimsonWave : ShaderDraw
{
	/// <summary>
	/// ai[0]x相位
	/// ai[1]波速
	/// ai[2]宽度
	/// </summary>
	public float Timer;
	public float MaxTime;
	public float Radius;

	public WaveOfEffectPylonHit_CrimsonWave()
	{
	}

	public WaveOfEffectPylonHit_CrimsonWave(int maxTime, Vector2 position, Vector2 velocity, params float[] ai)
		: base(position, velocity, ai)
	{
		this.MaxTime = maxTime;
	}

	public override void Update()
	{
		Position += Velocity;
		Radius += ai[1] * ((MaxTime - Timer) / MaxTime);
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		float delC = ai[2] * 0.05f * (float)Math.Sin((MaxTime - Timer) / MaxTime * Math.PI);
		Lighting.AddLight(Position, 0.015f * delC, 0, 0.45f * delC);
	}

	public override void Draw()
	{
		float fx = Timer / MaxTime;
		int len = (int)(Radius / 3f);
		if (len <= 2)
		{
			return;
		}

		var bars = new Vertex2D[len * 2 + 2];
		for (int i = 0; i < len + 1; i++)
		{
			Vector2 normal = new Vector2(0, 1).RotatedBy(i / (double)len * Math.PI * 2);
			Vector2 radialOffset = normal * Radius;

			var drawcRope = new Color(fx * fx * fx * 2 - 0.1f, 0.5f, 1, 150 / 255f);
			float width = ai[2];
			float texCoordWidth = 0.37f;
			if (width > radialOffset.Length())
			{
				texCoordWidth *= radialOffset.Length() / width;
				width = radialOffset.Length();
			}

			bars[2 * i] = new Vertex2D(Position + radialOffset, drawcRope, new Vector3(ai[0] + Timer / MaxTime, i / (float)len * 2, 0.8f - fx));
			bars[2 * i + 1] = new Vertex2D(Position + radialOffset - normal * width, drawcRope, new Vector3(texCoordWidth + ai[0] + Timer / MaxTime, i / (float)len * 2, 0.8f - fx));
		}

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
