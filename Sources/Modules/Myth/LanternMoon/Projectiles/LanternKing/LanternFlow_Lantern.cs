namespace Everglow.Myth.LanternMoon.Projectiles.LanternKing;

[Pipeline(typeof(WCSPipeline))]
public class LanternFlow_Lantern : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 position;
	public Vector2 velocity;
	public float scale = 1;
	public float rotation = 0;
	public float alpha = 1;
	public int timer = 0;
	public int width;
	public int height;
	public Texture2D texture = ModAsset.LanternFlow.Value;
	public int maxTime = 60;
	public NPC npcOwner;

	public Vector2 TargetCenter => npcOwner != null ? npcOwner.Center + new Vector2(0, 50) : Vector2.Zero;

	public override void OnSpawn()
	{
		maxTime = 120;
		timer = 0;
		width = texture.Width;
		height = texture.Height;
	}

	public override void Update()
	{
		timer++;
		if (npcOwner != null && npcOwner.active)
		{
			velocity = velocity.RotatedBy(MathF.Sin((float)Main.time * 0.03f) * 0.02f);
		}
		velocity *= 0.98f;
		position += velocity;
		if (timer >= maxTime)
		{
			Active = false;
		}
		rotation = velocity.X / scale * 0.1f;
		float maxDis = 400;
		if ((TargetCenter - position).Length() < maxDis)
		{
			velocity += Vector2.Normalize(TargetCenter - position - velocity) * ((400 - (TargetCenter - position).Length()) / 400f);
			timer += (int)((maxDis - (TargetCenter - position).Length()) / 40f);
		}
		Lighting.AddLight(position, new Vector3(1f, 0.3f, 0) * scale * alpha);
		base.Update();
	}

	public override void Draw()
	{
		Vector2 v0 = position + new Vector2(-width, -height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v1 = position + new Vector2(width, -height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v2 = position + new Vector2(-width, height).RotatedBy(rotation) * 0.5f * scale;
		Vector2 v3 = position + new Vector2(width, height).RotatedBy(rotation) * 0.5f * scale;

		alpha = (maxTime - timer) / 40f;
		if (timer < 40)
		{
			alpha = timer / 40f;
		}
		alpha = Math.Clamp(alpha, 0.0f, 1.0f);

		Color c0 = new Color(1f, 1f, 1f, 0.3f) * alpha;

		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(v0, c0, new Vector3(0, 0, 0)),
			new Vertex2D(v1, c0, new Vector3(1, 0, 0)),

			new Vertex2D(v2, c0, new Vector3(0, 1, 0)),
			new Vertex2D(v3, c0, new Vector3(1, 1, 0)),
		};
		Ins.Batch.Draw(texture, bars, PrimitiveType.TriangleStrip);
	}
}
