namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class TwilightCrystalVFXDust_Charge : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float maxScale;
	public float Rotation;
	public int Frame = 0;

	public NPC Owner;

	public Vector2 TargetPos;

	public override void Update()
	{
		Timer++;
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
		if (Owner != null)
		{
			TargetPos = Owner.Center + new Vector2(Owner.direction * 9, -7 + Owner.gfxOffY);
		}
		Vector2 toTarget = TargetPos - Position;
		Velocity = toTarget.NormalizeSafe();
		if (toTarget.Length() < 1f)
		{
			Active = false;
			return;
		}
		Position += Velocity;
		Rotation = Velocity.ToRotationSafe() - MathHelper.PiOver4;
		Scale += 0.15f;
		Frame = 2;
		if (Main.rand.NextBool(6))
		{
			Frame = 1;
		}
		if (Main.rand.NextBool(12))
		{
			Frame = 0;
		}
		Lighting.AddLight(Position, new Vector3(0f, 0.3f, 0.7f) * Scale / 8f);
	}

	public override void Draw()
	{
		float frameCount = 3;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		var drawColor = new Color(1f, 1f, 1f, 1f);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.TwilightCrystalVFXDust.Value, bars, PrimitiveType.TriangleList);
	}
}
