namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class BoneHeart_VFX : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public Vector2 Velocity;
	public float Omega;
	public float Beta;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public Color BoneHeartColor;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		Position += Velocity;
		Rotation = Velocity.ToRotation() + MathHelper.PiOver4;
		Velocity = Velocity.RotatedBy(Omega);
		Omega += Beta;
		Velocity *= 0.94f;
		Omega *= 0.94f;
		Beta *= 0.94f;
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		float timeValue = Math.Clamp((1 - pocession) * 8, 0, 5);
		float frameCount = 6;
		float frameY = (int)timeValue % frameCount;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = BoneHeartColor;
		if (pocession < 0.8f)
		{
			drawColor = Color.Lerp(drawColor, BoneHeartColor, (pocession - 0.2f) / 0.6f);
		}
		if (pocession < 0.2f)
		{
			drawColor = Color.Lerp(BoneHeartColor, Color.Transparent, 1 - pocession / 0.2f);
		}

		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.BoneHeart_VFX.Value, bars, PrimitiveType.TriangleList);
	}
}
