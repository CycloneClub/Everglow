using Everglow.Yggdrasil.YggdrasilTown.Projectiles.Enemies;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(RockElemental_SuckingLinePipeline))]
public class RockElemental_SuckingLine : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Queue<Vector2> OldPos = new Queue<Vector2>();
	public Projectile VFXOwner;
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;

	public override void Update()
	{
		if (VFXOwner == null || !VFXOwner.active || VFXOwner.type != ModContent.ProjectileType<RockElemental_ThrowingStone>())
		{
			Active = false;
			return;
		}
		RockElemental_ThrowingStone rockElemental_ThrowingStone = VFXOwner.ModProjectile as RockElemental_ThrowingStone;
		if (rockElemental_ThrowingStone != null)
		{
			if (rockElemental_ThrowingStone.PolymerizationTimer < 0)
			{
				Timer += 2;
			}
			else
			{
				Velocity = Velocity.RotatedBy(ai[1]);
				Vector2 pierceAim = VFXOwner.Center - Velocity - Position;
				if (pierceAim.Length() < 30)
				{
					Timer += 2;
				}
				Velocity = Vector2.Lerp(Velocity, Utils.SafeNormalize(pierceAim, Vector2.zeroVector) * 9f, 0.1f);
			}
		}
		OldPos.Enqueue(Position);
		if (OldPos.Count > 30)
		{
			OldPos.Dequeue();
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Position += Velocity;
		float pocession = 1 - Timer / MaxTime;
		float c = pocession * Scale * 0.04f;
		Lighting.AddLight(Position, c * 0.5f, c * 0.1f, c * 0.8f);
	}

	public override void Draw()
	{
		int len = OldPos.Count;
		var bars = new List<Vertex2D>();
		if (len <= 2)
		{
			for (int i = 1; i < 3; i++)
			{
				bars.Add(Position, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 17f, 1));
				bars.Add(Position, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0.6f + ai[0], (i + 15 - len) / 17f, 1));
			}
		}
		else
		{
			Vector2[] pos = OldPos.Reverse().ToArray();
			for (int i = 1; i < len; i++)
			{
				float pocession = Timer / MaxTime;
				if (Timer - i < 20)
				{
					pocession += (20 - Timer + i) / 20f;
				}
				pocession = Math.Clamp(pocession, 0, 1);
				Vector2 normal = pos[i] - pos[i - 1];
				normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);
				float width = Scale * (float)Math.Sin(i / (double)len * Math.PI);
				bars.Add(pos[i] + normal * width, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 17f, pocession));
				bars.Add(pos[i] - normal * width, new Color(0.3f + ai[0], 0, 0, 0), new Vector3(0.6f + ai[0], (i + 15 - len) / 17f, pocession));
			}
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
