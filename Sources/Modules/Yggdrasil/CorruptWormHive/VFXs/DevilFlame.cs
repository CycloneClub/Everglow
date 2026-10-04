using ReLogic.Content;

namespace Everglow.Yggdrasil.CorruptWormHive.VFXs;

internal abstract class ShaderDraw : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;

	public ShaderDraw(Vector2 Position, Vector2 Velocity, params float[] ai)
	{
		this.Position = Position;
		this.Velocity = Velocity;
		this.ai = ai; // 可以认为params传入的都是右值，可以直接引用
	}
}

internal class DevilFlamePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModContent.Request<Effect>(ModAsset.DevilFlame_Mod, AssetRequestMode.ImmediateLoad);
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_burn.Value);
		Texture2D FlameColor = ModAsset.DeathSickle_Color.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[0] = SamplerState.PointClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(DevilFlamePipeline), typeof(BloomPipeline))]
internal class DevilFlameDust : ShaderDraw
{
	private Vector2 vsadd = Vector2.Zero;
	public List<Vector2> oldPos = new List<Vector2>();
	public float Timer;
	public float MaxTime;

	public DevilFlameDust(int MaxTime, Vector2 Position, Vector2 Velocity, params float[] ai)
		: base(Position, Velocity, ai)
	{
		this.MaxTime = MaxTime;
	}

	public override void Update()
	{
		Position += Velocity;
		oldPos.Add(Position);
		if (oldPos.Count > 15)
		{
			oldPos.RemoveAt(0);
		}

		Velocity *= 0.96f;
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity = Velocity.RotatedBy(ai[1]);

		for (int f = oldPos.Count - 1; f > 0; f--)
		{
			if (oldPos[f] != Vector2.Zero)
			{
				oldPos[f] += vsadd;
			}
		}
		float delC = ai[2] * 0.05f * (float)Math.Sin((MaxTime - Timer) / 40d * Math.PI);
		Lighting.AddLight((int)(Position.X / 16), (int)(Position.Y / 16), 0.25f * delC, 0f, 0.95f * delC);
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float fx = Timer / MaxTime;
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
			var drawcRope = new Color(fx * fx * fx * 2, 0.5f, 1, 50 / 255f);
			float width = ai[2] * (float)Math.Sin(i / (double)len * Math.PI);
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width, drawcRope, new Vector3(0 + ai[0], i / 80f, 0));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width, drawcRope, new Vector3(0.05f + ai[0], i / 80f, 0));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}

	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;
}
