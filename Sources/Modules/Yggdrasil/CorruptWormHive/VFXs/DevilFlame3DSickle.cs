using static Everglow.Yggdrasil.CorruptWormHive.Projectiles.Melee.TrueDeathSickle.TrueDeathSickle_Blade;

namespace Everglow.Yggdrasil.CorruptWormHive.VFXs;

[Pipeline(typeof(DevilFlamePipeline), typeof(BloomPipeline))]
internal class DevilFlame3DSickleDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector3 Position3D;
	public Vector3 Velocity3D;
	public Vector3 rotateAxis;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Queue<Vector3> Trails = new Queue<Vector3>();
	public int OwnerWhoAmI = -1;

	public override void OnSpawn()
	{
		Trails.Enqueue(Position3D - Velocity3D);
		base.OnSpawn();
	}

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		if (OwnerWhoAmI == -1)
		{
			Active = false;
			return;
		}
		Player player = Main.player[OwnerWhoAmI];
		Trails.Enqueue(Position3D);
		if (Trails.Count > 40)
		{
			Trails.Dequeue();
		}
		if (MaxTime - Timer < 22)
		{
			ai[2] = MathHelper.Lerp(ai[2], 1f, 0.18f);
		}
		Position3D += Velocity3D;
		Velocity3D *= 0.96f;
		Velocity3D.Y -= 3 * MathF.Abs(ai[1]);
		Velocity3D = RodriguesRotate(Velocity3D, rotateAxis, ai[1]);
		float delC = 1f * (float)Math.Sin((MaxTime - Timer) / 40d * Math.PI);
		float size;
		Lighting.AddLight(Projection2D(Position3D, Vector2.zeroVector, 500, out size) + player.Center + Offset, 0.25f * delC, 0f, 0.95f * delC);
	}

	public override void Draw()
	{
		if (OwnerWhoAmI == -1)
		{
			Active = false;
			return;
		}
		Player player = Main.player[OwnerWhoAmI];
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 1; i < Trails.Count; i++)
		{
			Vector3 pos3D = Trails.ToArray()[i];
			Vector3 pos3DOld = Trails.ToArray()[i - 1];
			float width = (i - 1) / (float)Math.Max(Trails.Count - 2f, 1);
			width = MathF.Sin(width * MathF.PI) * Scale;
			if (Trails.Count <= 4)
			{
				width = 0f;
			}
			float size;
			Vector2 posOld = Projection2D(pos3DOld, Vector2.zeroVector, 500, out size) + player.Center + Offset;
			Vector2 pos = Projection2D(pos3D, Vector2.zeroVector, 500, out size) + player.Center + Offset;
			Vector2 normal = Utils.SafeNormalize(pos - posOld, Vector2.zeroVector).RotatedBy(MathHelper.PiOver2);
			normal *= width * size;
			float timeValue = -(float)Main.timeForVisualEffects * 0.0015f;
			float fx = Timer / MaxTime;
			var drawcRope = new Color(fx * fx * fx * 2, 0.5f, 1, Math.Clamp(1 - fx, 0, 0.6f));
			bars.Add(pos - normal, drawcRope, new Vector3(timeValue + i / 60f, ai[0], ai[2]));
			bars.Add(pos + normal, drawcRope, new Vector3(timeValue + i / 60f, ai[0] + 0.3f, ai[2]));
		}
		if (bars.Count <= 2)
		{
			float size;
			Vector2 pos = Projection2D(Position3D, Vector2.zeroVector, 500, out size) + player.Center;
			bars.Add(pos, Color.White, new Vector3(0));
			bars.Add(pos, Color.White, new Vector3(0));
			bars.Add(pos, Color.White, new Vector3(0));
			bars.Add(pos, Color.White, new Vector3(0));
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
