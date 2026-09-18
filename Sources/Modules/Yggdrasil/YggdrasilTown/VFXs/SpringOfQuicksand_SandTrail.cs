namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(SpringOfQuicksand_SandTrail_Pipeline))]
public class SpringOfQuicksand_SandTrail : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Fade = 0;
	public float Scale;
	private Queue<Vector2> trails = new Queue<Vector2>();

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
		trails.Enqueue(Position);
		if (trails.Count > 60)
		{
			trails.Dequeue();
		}
		if (Collision.IsWorldPointSolid(Position))
		{
			Fade += 0.1f;
			if (Fade > 1f)
			{
				Active = false;
				return;
			}
		}
		Position += Velocity;
		Velocity.Y += 0.25f * Scale / 20f;
		Velocity *= 0.95f;

		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
		}
	}

	public override void Draw()
	{
		float timeValue = Timer / 30f;
		float timeFade = timeValue < 1 ? timeValue : 1f;
		if (MaxTime - Timer < 30f)
		{
			timeFade *= (MaxTime - Timer) / 30f;
		}
		List<Vertex2D> bars = new List<Vertex2D>();
		if (trails.Count <= 2)
		{
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));

			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));
			bars.Add(Position, Color.Transparent, new Vector3(0, 0, 0));

			Ins.Batch.Draw(Commons.ModAsset.Trail_12.Value, bars, PrimitiveType.TriangleStrip);
			return;
		}
		for (int i = 0; i < trails.Count; i++)
		{
			Vector2 pos = trails.ToArray()[i];
			Vector2 dir = Velocity.NormalizeSafe();
			if (i > 0)
			{
				dir = (trails.ToArray()[i - 1] - trails.ToArray()[i]).NormalizeSafe();
			}
			dir = dir.RotatedBy(MathHelper.PiOver2) * Scale;
			Color envLight = Lighting.GetColor(pos.ToTileCoordinates()) * (1 - Fade) * timeFade;
			float zCoord = i / ((float)trails.Count - 1);
			zCoord = MathF.Sin(zCoord * MathHelper.Pi);
			bars.Add(pos + dir, envLight, new Vector3(i / 40f - timeValue, 0, zCoord));
			bars.Add(pos - dir, envLight, new Vector3(i / 40f - timeValue, 1, zCoord));
		}
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars, PrimitiveType.TriangleStrip);
	}
}
