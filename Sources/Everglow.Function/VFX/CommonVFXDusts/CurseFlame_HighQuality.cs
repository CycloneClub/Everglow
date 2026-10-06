using Everglow.Commons.Enums;
using Everglow.Commons.Interfaces;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX.Pipelines;

namespace Everglow.Commons.VFX.CommonVFXDusts;

public class CurseFlame_HighQualityPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.CurseFlame_highQuality;
	}

	public override void BeginRender()
	{
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition.X, -Main.screenPosition.Y, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Value.Parameters["uTransform"].SetValue(model * projection);
		effect.Value.Parameters["uNoise"].SetValue(ModAsset.Noise_melting.Value);
		effect.Value.Parameters["uLight"].SetValue(ModAsset.Trail.Value);
		Texture2D FlameColor = ModAsset.Cursed_Color2.Value;
		Ins.Batch.BindTexture<Vertex2D>(FlameColor);
		Main.graphics.GraphicsDevice.SamplerStates[1] = SamplerState.AnisotropicClamp;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.AnisotropicWrap, RasterizerState.CullNone);
		effect.Value.CurrentTechnique.Passes[0].Apply();
	}

	public override void Render(IEnumerable<IVisual> visuals)
	{
		if (visuals.Count() > 0)
		{
			base.Render(visuals);
		}
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(CurseFlame_HighQualityPipeline), typeof(WarpAndFadePipeline), typeof(BloomPipeline))]
public class CurseFlame_HighQualityDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public List<Vector2> oldPos = new List<Vector2>();

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

		float delC = ai[2] * 0.05f * (float)Math.Sin((MaxTime - Timer) / 40d * Math.PI);
		Lighting.AddLight((int)(Position.X / 16), (int)(Position.Y / 16), 0.45f * delC, 0.85f * delC, 0f);
		if (Collision.SolidCollision(Position, 0, 0))
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		if (pocession < 0.2)
		{
			pocession = 0.4f;
		}
		if (Main.gamePaused)
		{
			pocession = 0.4f;
		}
		float timeValue = (float)(Main.time * 0.2);
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		int len = pos.Length;

		var bars = new List<Vertex2D>();
		if (len <= 2)
		{
			pocession = 0.4f;
			for (int i = 1; i < 3; i++)
			{
				float coordValue = (i - 1) / (float)len;
				var drawcRopeUp = new Color(0.25f + coordValue * 0.5f, 0, 0, 0);
				var drawcRopeDown = new Color(0.25f + coordValue * 0.5f, 1, 0, 0);
				bars.Add(new Vertex2D(Position, drawcRopeUp, new Vector3(ai[0] + coordValue * 0.4f, timeValue, pocession)));
				bars.Add(new Vertex2D(Position, drawcRopeDown, new Vector3(ai[0] + coordValue * 0.4f, timeValue + 0.4f, pocession)));
			}
		}
		else
		{
			for (int i = 1; i < len; i++)
			{
				Vector2 normal = oldPos[i] - oldPos[i - 1];
				normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
				float coordValue = (i - 1) / (float)len;
				var drawcRopeUp = new Color(0.25f + coordValue * 0.5f, 0, 0, 0);
				var drawcRopeDown = new Color(0.25f + coordValue * 0.5f, 1, 0, 0);
				float width = ai[2] * (float)Math.Sin(coordValue * Math.PI);
				bars.Add(new Vertex2D(oldPos[i] + normal * width, drawcRopeUp, new Vector3(ai[0] + coordValue * 0.4f, timeValue, pocession)));
				bars.Add(new Vertex2D(oldPos[i] - normal * width, drawcRopeDown, new Vector3(ai[0] + coordValue * 0.4f, timeValue + 0.4f, pocession)));
			}
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
