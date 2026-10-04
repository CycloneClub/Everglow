using ReLogic.Peripherals.RGB.SteelSeries;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class Leaf_VFX_Spin : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawProjectiles;

	public Vector2 Position;
	public float Omega;
	public float Radius;
	public float RotPos;
	public Vector2 RotatedCenter;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public Color LeafColor;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
		RotPos += Omega;
		Position = new Vector2(0, Radius).RotatedBy(RotPos) + RotatedCenter;
		Rotation += MathF.Sin(ai[0]) * 0.05f;
		Scale = MaxScale * MathF.Sin(Timer / MaxTime * MathHelper.Pi);
	}

	public override void Draw()
	{
		float pocession = 1 - Timer / MaxTime;
		float timeValue = (float)(Main.time * 0.24 + ai[0]);
		float frameCount = 8;
		float frameY = (int)timeValue % frameCount;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		Color drawColor = LeafColor;
		if (pocession < 0.8f)
		{
			drawColor = Color.Lerp(drawColor, LeafColor, (pocession - 0.2f) / 0.6f);
		}
		if (pocession < 0.2f)
		{
			drawColor = Color.Lerp(LeafColor, Color.Transparent, 1 - pocession / 0.2f);
		}
		Vector4 drawColorEffectedByEnvironment = drawColor.ToVector4() * Lighting.GetColor(Position.ToTileCoordinates()).ToVector4();
		drawColor = new Color(drawColorEffectedByEnvironment.X, drawColorEffectedByEnvironment.Y, drawColorEffectedByEnvironment.Z, drawColorEffectedByEnvironment.W);
		List<Vertex2D> bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.Leaf_VFX.Value, bars, PrimitiveType.TriangleList);
	}
}
