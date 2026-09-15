namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class PearShapedNeedle_Dust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public float Timer;
	public float MaxTime;
	public float Rotation;
	public float Scale;
	public float RanSeed;
	public Vector2 Position;
	public Vector2 Velocity;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Position += Velocity;
		Velocity *= 0.6f;
		Scale *= 0.95f;
	}

	public override void Draw()
	{
		Vector2 drawPos = Position;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		float shineValue = (MathF.Sin(Timer * 0.1f + RanSeed) + 1) / 2f;
		shineValue = MathF.Pow(shineValue, 8) * 6;
		var drawColor = Lighting.GetColor(Position.ToTileCoordinates());
		drawColor *= shineValue;
		var bars = new List<Vertex2D>();
		bars.Add(drawPos + toCorner, drawColor, new Vector3(0, 0, 0));
		bars.Add(drawPos + toCorner.RotatedBy(MathHelper.PiOver2), drawColor, new Vector3(1, 0, 0));
		bars.Add(drawPos + toCorner.RotatedBy(MathHelper.Pi), drawColor, new Vector3(0, 1, 0));

		bars.Add(drawPos + toCorner, drawColor, new Vector3(0, 0, 0));
		bars.Add(drawPos + toCorner.RotatedBy(MathHelper.Pi), drawColor, new Vector3(0, 1, 0));
		bars.Add(drawPos + toCorner.RotatedBy(-MathHelper.PiOver2), drawColor, new Vector3(1, 1, 0));

		Ins.Batch.Draw(ModAsset.IntroductiontoThermalElectricity_Plasma.Value, bars, PrimitiveType.TriangleList);
	}
}
