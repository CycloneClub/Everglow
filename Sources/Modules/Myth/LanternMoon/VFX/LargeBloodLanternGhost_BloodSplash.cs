namespace Everglow.Myth.LanternMoon.VFX;

[Pipeline(typeof(LargeBloodLanternGhost_BloodSplashPipeline))]
public class LargeBloodLanternGhost_BloodSplash : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> oldPos = new List<Vector2>();
	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float alpha;

	public override void Update()
	{
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		position += velocity * 0.001f;
		oldPos.Add(position);
		if (oldPos.Count > 15)
		{
			oldPos.RemoveAt(0);
		}

		velocity.Y += 0.14f;
		timer++;
		if (timer > maxTime)
		{
			Active = false;
		}

		velocity = velocity.RotatedBy(ai[1]);
		scale += 0.4f;
		if (Collision.SolidCollision(position, 0, 0))
		{
			velocity *= 0.2f;
			if (velocity.Length() < 0.02f)
			{
				Active = false;
			}
		}
		if (Main.tile[(int)(position.X / 16f), (int)(position.Y / 16f)].LiquidAmount > 0)
		{
			scale += 0.02f;
			alpha += 0.004f;
			velocity *= 0.9f;
			if (MathF.Abs(velocity.X) > 2)
			{
				velocity.X *= 0.8f;
			}
			velocity += new Vector2(Main.rand.NextFloat(0.5f), 0).RotatedByRandom(6.283) + new Vector2(0, 1.2f - Math.Abs(velocity.X) * alpha * 2f);
			position += velocity * 0.5f;
			timer -= 0.4f;
		}
		else
		{
			position += velocity;
		}
		float pocession = 1 - timer / maxTime;
		float c = pocession * scale * 0.04f;
		Lighting.AddLight(position, c * 0.8f, c * 0.1f, 0);
	}

	public override void Draw()
	{
		Vector2[] pos = oldPos.Reverse<Vector2>().ToArray();
		float pocession = timer / maxTime;
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
			float width = scale * (float)Math.Sin(i / (double)len * Math.PI);
			Vector2 pointUp = oldPos[i] + normal * width;
			Vector2 pointDown = oldPos[i] - normal * width;
			var widthUp = new Vector2(normal.X * width, 0);
			Vector2 widthDown = -new Vector2(normal.X * width, 0);
			if (Main.tile[(int)(pointUp.X / 16f), (int)(pointUp.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthUp *= MathF.Sqrt(alpha) * 4f;
			}
			else
			{
				widthUp *= 0f;
			}
			if (Main.tile[(int)(pointDown.X / 16f), (int)(pointDown.Y / 16f) - 1].LiquidAmount > 0)
			{
				widthDown *= MathF.Sqrt(alpha) * 4f;
			}
			else
			{
				widthDown *= 0f;
			}

			bars[2 * i - 1] = new Vertex2D(oldPos[i] + normal * width + widthUp, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 17f, pocession));
			bars[2 * i] = new Vertex2D(oldPos[i] - normal * width + widthDown, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0.6f + ai[0], (i + 15 - len) / 17f, pocession));
		}
		bars[0] = new Vertex2D((bars[1].position + bars[2].position) * 0.5f, Color.White, new Vector3(0.5f, 0, 0));
		Ins.Batch.Draw(ModAsset.HeatMap_Large_Blood_Lantern_Ghost_Blood.Value, bars, PrimitiveType.TriangleStrip);
	}
}
