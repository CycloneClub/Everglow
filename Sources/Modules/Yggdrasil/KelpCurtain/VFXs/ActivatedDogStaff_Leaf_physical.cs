using Terraria.Map;

namespace Everglow.Yggdrasil.KelpCurtain.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class ActivatedDogStaff_Leaf_physical : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;
	public float Fade = 1f;

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
		if (ai[1] < 0)
		{
			Velocity *= 0.9f;
			Velocity.Y += 0.10f;
		}

		Vector2 acclerate = new Vector2(Main.windSpeedCurrent * 0.05f, 0.1f);
		Velocity *= 0.95f;
		Velocity += acclerate;

		Position += Velocity;
		Rotation += MathF.Sin(ai[0]) * 0.05f;
	}

	public override void Draw()
	{
		float timeValue = (float)(Main.time * 0.24 + ai[0]);
		float frameCount = 8;
		float frameY = (int)timeValue % frameCount;
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
		Ins.Batch.Draw(ModAsset.ActivatedDogStaff_Leaf.Value, bars, PrimitiveType.TriangleList);
	}
}
