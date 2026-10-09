using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.Pipelines;
using ReLogic.Content;

namespace Everglow.SpellAndSkull.Projectiles.CursedFlames;

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

internal class CursedFlamePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.CursedFlame;
	}
	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		Texture2D FlameColor = ModAsset.Cursed_Color.Value;
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
[Pipeline(typeof(Commons.VFX.CommonVFXDusts.CurseFlamePipeline), typeof(BloomPipeline))]
internal class CursedFlameDust : ShaderDraw
{
	private Vector2 vsadd = Vector2.Zero;
	public List<Vector2> oldPos = new List<Vector2>();
	public float timer;
	public float maxTime;
	public CursedFlameDust() { }
	public CursedFlameDust(int maxTime, Vector2 position, Vector2 velocity, params float[] ai) : base(position, velocity, ai)
	{
		this.maxTime = maxTime;
	}

	public override void Update()
	{
		position += velocity;
		oldPos.Add(position);
		if (oldPos.Count > 15)
			oldPos.RemoveAt(0);
		velocity *= 0.96f;
		timer++;
		if (timer > maxTime)
			Active = false;
		velocity = velocity.RotatedBy(ai[1]);

		for (int f = oldPos.Count - 1; f > 0; f--)
		{
			if (oldPos[f] != Vector2.Zero)
				oldPos[f] += vsadd;
		}
		float delC = ai[2] * 0.05f * (float)Math.Sin((maxTime - timer) / 40d * Math.PI);
		Lighting.AddLight((int)(position.X / 16), (int)(position.Y / 16), 0.45f * delC, 0.85f * delC, 0f);
		if (Collision.SolidCollision(position, 0, 0))
			Active = false;
	}

	public override void Draw()
	{
		float pocession = timer / maxTime;
		float timeValue = (float)(Main.time * 0.002);
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		int len = pos.Length;
		if (len <= 2)
			return;
		var bars = new Vertex2D[len * 2 - 1];
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			float coordValue = i / (float)len;
			var drawcRopeUp = new Color(0.25f + coordValue * 0.5f, 0, pocession, 0);
			var drawcRopeDown = new Color(0.25f + coordValue * 0.5f, 1, pocession, 0);
			float width = ai[2] * (float)Math.Sin(coordValue * Math.PI);
			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width, drawcRopeUp, new Vector3(ai[0] + coordValue * 0.4f, timeValue, 0.8f));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width, drawcRopeDown, new Vector3(ai[0] + coordValue * 0.4f, timeValue + 0.4f, 0.8f));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
