namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class BloodSpellShake : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public Vector2 Position;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float Rotation;

	public override void Update()
	{
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
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		Color lightColor = Color.Red;
		float timeValue = Timer / MaxTime;
		float radiusScale = timeValue * Scale;
		List<Vertex2D> bars = new List<Vertex2D>();
		for (int i = 0; i <= 60; i++)
		{
			float Radius = radiusScale * (MathF.Sin(i / 5f * MathHelper.Pi) * MathF.Sin(timeValue * 9 + ai[0]) * 0.2f + 1);
			bars.Add(Position + new Vector2(0, -(60 * timeValue + 30) * Radius).RotatedBy(i / 60f * MathHelper.TwoPi + Rotation), Color.Transparent, new Vector3(i / 60f, timeValue * 0.8f, 0));
			bars.Add(Position + new Vector2(0, -90 * Radius).RotatedBy(i / 60f * MathHelper.TwoPi + Rotation), lightColor, new Vector3(i / 60f, timeValue * 0.8f + 0.6f, 0));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_cell.Value, bars, PrimitiveType.TriangleStrip);
	}
}
