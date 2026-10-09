using Everglow.Commons.DataStructures;

namespace Everglow.Yggdrasil.YggdrasilTown.NPCs.KingJellyBall;

public class KingJellyBall_HealPipeline : Pipeline
{
	public override void Load()
	{
		effect = ModAsset.KingJellyBall_HealReflection;
	}

	public override void BeginRender()
	{
		var effect = this.effect.Value;
		var projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1);
		var model = Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) * Main.GameViewMatrix.TransformationMatrix;
		effect.Parameters["uTransform"].SetValue(model * projection);
		effect.Parameters["uSize"].SetValue(0.003f);
		effect.Parameters["uThredshold"].SetValue(0.2f);
		effect.CurrentTechnique.Passes["Test"].Apply();

		Ins.Batch.BindTexture<Vertex2D>(Commons.ModAsset.Noise_perlin.Value);
		Main.graphics.graphicsDevice.SamplerStates[1] = SamplerState.PointWrap;
		Main.graphics.graphicsDevice.Textures[1] = Commons.ModAsset.Trail.Value;
		Ins.Batch.Begin(BlendState.AlphaBlend, DepthStencilState.None, SamplerState.PointWrap, RasterizerState.CullNone);
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}
}

[Pipeline(typeof(KingJellyBall_HealPipeline))]
public class KingJellyBall_Heal : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public NPC MyKingJellyBallOwner;

	public Vector2 Position;

	public float Rotation;

	public float Scale;

	public float Timer;

	public float MaxTime;

	public float[] ai;

	public override void OnSpawn()
	{
	}

	public override void Update()
	{
		if (MyKingJellyBallOwner == null || !MyKingJellyBallOwner.active || MyKingJellyBallOwner.type != ModContent.NPCType<KingJellyBall>() || MyKingJellyBallOwner.life <= 0)
		{
			Active = false;
			return;
		}
		KingJellyBall kingJellyBall = MyKingJellyBallOwner.ModNPC as KingJellyBall;
		if (kingJellyBall == null)
		{
			Active = false;
			return;
		}
		Position = MyKingJellyBallOwner.Center;
		Rotation = MyKingJellyBallOwner.rotation;
		Scale = MyKingJellyBallOwner.scale;
		if (Timer < MaxTime)
		{
			Timer++;
		}
		else
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		float timeValue = (float)Main.time * 0.03f;
		List<Vertex2D> jellyBallBodyInner = new List<Vertex2D>();

		// adjust center base on the polar funtion graph.
		Vector2 offset = new Vector2(0, -60) + new Vector2(0, -120 * (Scale - 0.3f));
		Vector2 offsetedCenter = Position + offset;
		int step = 150;
		Color drawColor = new Color(0.1f, 0.6f, 1f, 0f);
		float mulColor = 1f;
		if (Timer < 10)
		{
			mulColor *= Timer / 10f;
		}
		if (Timer > MaxTime - 11)
		{
			mulColor *= (MaxTime - Timer) / 10f;
		}
		drawColor *= mulColor;
		for (int theta = 0; theta <= step; theta++)
		{
			float a = 200;
			float b = 110 + 10 * MathF.Sin(timeValue);
			float angle = theta / (float)step * MathHelper.TwoPi;
			float r = a - b * MathF.Sin(angle);

			// noise wave
			for (int k = 0; k < 6; k++)
			{
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi + MathF.Sin(timeValue * MathF.Pow(2, k * 0.2f)) * 0.22f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi - timeValue * 0.33f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 2;
				r += MathF.Sin((theta / (float)step * MathHelper.TwoPi + timeValue * 0.65f) * 8 * MathF.Pow(2, k)) / MathF.Pow(2f, k) * 1;
			}
			r *= Scale;
			Vector2 toDistance = new Vector2(-r, 0).RotatedBy(angle);
			toDistance.Y *= 1.3f;
			Vector2 width = Vector2.Normalize(toDistance) * 2f;
			jellyBallBodyInner.Add(offsetedCenter, drawColor, new Vector3(offsetedCenter + new Vector2(ai[0], ai[1]), (Timer / MaxTime - 0.5f) * 2 + 0.5f));
			jellyBallBodyInner.Add(offsetedCenter + toDistance - width, drawColor * 0.3f, new Vector3(offsetedCenter + new Vector2(ai[0], ai[1]) + new Vector2(Timer * 0.03f), (Timer / MaxTime - 0.5f) * 2 + 0.5f));
		}
		if (jellyBallBodyInner.Count >= 2)
		{
			Ins.Batch.Draw(jellyBallBodyInner, PrimitiveType.TriangleStrip);
			// Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, jellyBallBodyInner.ToArray(), 0, jellyBallBodyInner.Count - 2);
		}
	}
}
