namespace Everglow.Yggdrasil.YggdrasilTown.VFXs.TownNPCAttack;

public class Georg_Hammer_JumpHit_Smog_FirePipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.Georg_Hammer_JumpHit_Smog_Fire;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_flame_0.Value);
		effect.Parameters["uLine"].SetValue(Commons.ModAsset.TrailV.Value);
		Ins.Batch.BindTexture<Vertex2D>(ModAsset.Georg_Hammer_JumpHit_Smog_Fire_heatmap.Value);
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.LinearClamp, RasterizerState.CullNone);
		effect.CurrentTechnique.Passes[0].Apply();
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(Georg_Hammer_JumpHit_Smog_FirePipeline))]
public class Georg_Hammer_JumpHit_Smog_Fire : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public List<Vector2> oldPos = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Alpha;

	public override void Update()
	{
		oldPos.Add(Position);
		if (oldPos.Count > 200)
		{
			oldPos.RemoveAt(0);
		}

		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}

		Velocity *= 0.9f;
		Position += Velocity;
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float fx = Timer / MaxTime;
		int len = pos.Length;
		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = oldPos[i] - oldPos[i - 1];
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
			var lightColorWithPos = new Color(MathF.Pow(fx, 0.6f), 1f, 1f, 1f);
			float width = (float)Math.Sin(MathF.Pow((i - 1) / (float)(len - 2), 0.2f) * Math.PI);
			bars.Add(oldPos[i] + normal * Scale, lightColorWithPos, new Vector3(0, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.4f));
			bars.Add(oldPos[i] - normal * Scale, lightColorWithPos, new Vector3(1, (i + 15 - len) / 75f + Timer / 15000f, fx - width * 0.4f));
		}
		if (len <= 2)
		{
			for (int i = 1; i < 3; i++)
			{
				var lightColorWithPos = new Color(1f, 1f, 1f, 0);
				bars.Add(Position, lightColorWithPos, new Vector3(0, (i + 15 - len) / 75f + Timer / 15000f, fx));
				bars.Add(Position, lightColorWithPos, new Vector3(1, (i + 15 - len) / 75f + Timer / 15000f, fx));
			}
		}

		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
