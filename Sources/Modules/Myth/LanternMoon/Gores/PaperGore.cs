using Everglow.Commons.VFX.CommonVFXDusts;

namespace Everglow.Myth.LanternMoon.Gores;

[Pipeline(typeof(BurningPipeline))]
public class PaperGore : BurningGore
{
	public override void Update()
	{
		float timevalue = timer / (float)maxTime;
		rotateSpeed = velocity.X / 80f;
		velocity = velocity.RotatedBy(MathF.Sin(timevalue * MathF.PI * 0.025f * ai[0]));
		velocity += new Vector2(MathF.Sin(timevalue * MathF.PI * 0.035f * ai[0]), Math.Abs(MathF.Cos(timevalue * MathF.PI * 0.035f * ai[0])) * 0.01f);
		velocity.X *= 0.99f;
		base.Update();

		if (timevalue > 0.25f)
		{
			if (Main.rand.NextBool(3))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireSparkDust
				{
					velocity = newVelocity + velocity * Main.rand.NextFloat(0f, 1f),
					Active = true,
					Visible = true,
					position = position + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0).RotatedByRandom(6.283) * (1 - timevalue) * width,
					maxTime = Main.rand.Next(37, 45) * (1 - timevalue),
					scale = Main.rand.NextFloat(0.1f, 12.0f) * (1 - timevalue),
					rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
			}
			if (Main.rand.NextBool(3))
			{
				Vector2 newVelocity = new Vector2(0, Main.rand.NextFloat(0f, 1f)).RotatedByRandom(MathHelper.TwoPi);
				var spark = new FireDust
				{
					Velocity = newVelocity + velocity * Main.rand.NextFloat(0f, 1f),
					Active = true,
					Visible = true,
					Position = position + new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), 0).RotatedByRandom(6.283) * (1 - timevalue) * width,
					MaxTime = Main.rand.Next(5, 15) * (1 - timevalue),
					Scale = Main.rand.NextFloat(0.1f, 6.0f) * (1 - timevalue),
					Rotation = Main.rand.NextFloat(6.283f),
					ai = new float[] { Main.rand.NextFloat(0.0f, 0.93f), Main.rand.NextFloat(-0.01f, 0.01f) },
				};
				Ins.VFXManager.Add(spark);
			}
		}
	}

	public override void DrawDissolvePart()
	{
		float timevalue = timer / (float)maxTime;
		float flipflag = MathF.Cos(rotation * ai[0] + ai[1]);

		Vector2 v0 = position + new Vector2(-width * flipflag, -height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v1 = position + new Vector2(width * flipflag, -height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v2 = position + new Vector2(-width * flipflag, height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v3 = position + new Vector2(width * flipflag, height).RotatedBy(rotation) * 0.5f * scale;

		alpha = 1;

		Color c0 = Lighting.GetColor((v0 / 16f).ToPoint()) * alpha;
		Color c1 = Lighting.GetColor((v1 / 16f).ToPoint()) * alpha;
		Color c2 = Lighting.GetColor((v2 / 16f).ToPoint()) * alpha;
		Color c3 = Lighting.GetColor((v3 / 16f).ToPoint()) * alpha;

		timevalue = MathF.Asin(timevalue * 2 - 1) / MathF.PI + 0.5f;

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, (float)timevalue)),
			new Vertex2D(v1, c1, new Vector3(1, 0, (float)timevalue)),

			new Vertex2D(v2, c2, new Vector3(0, 1, (float)timevalue)),
			new Vertex2D(v3, c3, new Vector3(1, 1, (float)timevalue)),
		};

		Ins.Batch.Draw(Texture, bars, PrimitiveType.TriangleStrip);
	}
}
