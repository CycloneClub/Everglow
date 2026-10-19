using Terraria.Map;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

public class EvilMusicRemnant_Minion_gorePipeline : Pipeline
{
	public override void BeginRender()
	{
		Ins.Batch.Begin();
		effect.Value.Parameters["uTransform"].SetValue(
			Matrix.CreateTranslation(new Vector3(-Main.screenPosition, 0)) *
			Main.GameViewMatrix.TransformationMatrix *
			Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, 0, 1));
		effect.Value.CurrentTechnique.Passes[0].Apply();
		effect.Value.Parameters["uColorSet"].SetValue(new Vector4(0.15f, 0.05f, 0.3f, 1f));
		effect.Value.Parameters["uNoise"].SetValue(Commons.ModAsset.Noise_Cell.Value);
	}

	public override void EndRender()
	{
		Ins.Batch.End();
	}

	public override void Load()
	{
		effect = ModAsset.EvilMusicRemnant_Minion_Dissolve;
	}
}

[Pipeline(typeof(EvilMusicRemnant_Minion_gorePipeline))]
public class EvilMusicRemnant_Minion_gore : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Rotation;
	public int Style;

	public override void Update()
	{
		Timer++;
		Position += Velocity;
		if (Position.X <= 320 || Position.X >= Main.maxTilesX * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 320 || Position.Y >= Main.maxTilesY * 16 - 320)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Rotation += ai[0] * 0.4f;
		ai[0] *= 0.95f;
		if (!Collision.SolidCollision(Position - new Vector2(12.5f), 25, 25))
		{
			Velocity += new Vector2(0, 0.15f);
		}
		else
		{
			if (Velocity.Length() < 10f)
			{
				Velocity *= 0;
				ai[0] *= 0f;
				return;
			}
		}
		if (Velocity.Y > 12f)
		{
			Velocity.Y *= 0.95f;
		}
		if (Collision.SolidCollision(Position - new Vector2(12.5f) + new Vector2(Velocity.X, 0), 25, 25))
		{
			if (Velocity.Length() < 10f)
			{
				Velocity *= 0;
				ai[0] *= 0f;
				return;
			}
			ai[0] *= 0.5f;
			Velocity.X *= -Main.rand.NextFloat(1f);
			Position += Velocity;
			Velocity += new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(MathHelper.TwoPi) * MathF.Min(Velocity.Length(), 2f);
		}
		if (Collision.SolidCollision(Position - new Vector2(12.5f) + new Vector2(0, Velocity.Y), 25, 25))
		{
			if (Velocity.Length() < 10f)
			{
				Velocity *= 0;
				ai[0] *= 0f;
				return;
			}
			ai[0] *= 0.5f;
			Velocity.Y *= -Main.rand.NextFloat(1f);
			Position += Velocity;
			Velocity += new Vector2(0, Main.rand.NextFloat(0.3f)).RotatedByRandom(MathHelper.TwoPi) * MathF.Min(Velocity.Length(), 2f);
		}
	}

	public override void Draw()
	{
		float frameCount = 6;
		float frameY = Style;
		float dissolvePro = -0.4f;
		if (MaxTime - Timer < 120)
		{
			dissolvePro = 1 - (MaxTime - Timer) / 120f;
			dissolvePro *= 1.4f;
			dissolvePro -= 0.4f;
		}
		Vector2 toCorner = new Vector2(0, 25 * MathF.Sqrt(2)).RotatedBy(Rotation);
		Color drawColor = Lighting.GetColor((Position + new Vector2(25)).ToTileCoordinates());
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, dissolvePro)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, dissolvePro)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, dissolvePro)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, dissolvePro)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, dissolvePro)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, dissolvePro)),
		};
		Ins.Batch.Draw(ModAsset.EvilMusicRemnant_Minion_gore.Value, bars, PrimitiveType.TriangleList);
	}
}
