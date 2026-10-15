namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class ActivatedDogStaff_Branch : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Projectile ParentEneity;
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public float Fade = 1f;
	public int Frame = 0;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		if (MaxTime - Timer < 10)
		{
			Fade *= 0.9f;
		}

		Vector2 toTarget = ParentEneity.Center - Position;
		float distanceValue = toTarget.Length();
		float power = 25f / (distanceValue + 1f);
		float vortexEyeRadius = ParentEneity.timeLeft;
		if (ParentEneity.active == false || (ParentEneity.Center - Position).Length() > 100)
		{
			vortexEyeRadius = 1;
		}
		float deviate = vortexEyeRadius / (distanceValue + 1f) * MathHelper.PiOver2;
		Vector2 acclerate = toTarget.NormalizeSafe().RotatedBy(deviate) * power;
		Velocity *= 0.9f;
		Velocity += acclerate;

		Position += Velocity;
		Rotation += MathF.Sin(ai[0]) * 0.05f;
	}

	public override void Draw()
	{
		float frameCount = 5;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = Lighting.GetColor(Position.ToTileCoordinates()) * Fade;
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.ActivatedDogStaff_Branch.Value, bars, PrimitiveType.TriangleList);
	}
}
