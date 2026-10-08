using Everglow.Commons.Enums;
using Everglow.Commons.Vertex;
using Everglow.Commons.VFX;
using Everglow.Commons.VFX.CommonVFXDusts;
using static Everglow.Commons.Vertex.VertexExtensions;

namespace Everglow.EternalResolve.VFXs;

[Pipeline(typeof(ElectricCurrentPipeline))]
public class YoenLeZedElecticFlow : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawDusts;

	public List<Vector2> OldPositions = new List<Vector2>();
	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		if (Timer < 2)
		{
			for (int i = 0; i < 16; i++)
			{
				UpdateInside();
			}
		}
		UpdateInside();
	}

	private void UpdateInside()
	{
		if (Position.X <= 720 || Position.X >= Main.maxTilesX * 16 - 720)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		if (Position.Y <= 720 || Position.Y >= Main.maxTilesY * 16 - 720)
		{
			Timer = MaxTime;
			Active = false;
			return;
		}
		OldPositions.Add(Position);

		for (int x = 0; x < OldPositions.Count; x++)
		{
			OldPositions[x] += new Vector2(0, Main.rand.NextFloat(2f)).RotatedByRandom(6.283);
		}
		Position += Velocity.RotatedBy(Main.rand.NextFloat(-1f, 1f) / Scale * 4f) * Main.rand.NextFloat(0.75f, 1.25f);
		Velocity = Velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f) / Scale * 12f * ai[1]);
	}

	public override void Draw()
	{
		Vector2[] positions = OldPositions.Reverse<Vector2>().ToArray();
		float progress = Timer / MaxTime;
		int len = positions.Length;

		var bars = new List<Vertex2D>();
		for (int i = 1; i < len; i++)
		{
			Vector2 normal = OldPositions[i] - OldPositions[i - 1];

			Vector2 normal2 = OldPositions[i] - OldPositions[i - 1];
			if (i < len - 1)
			{
				normal2 = OldPositions[i + 1] - OldPositions[i];
			}
			normal += normal2;
			normal = Vector2.Normalize(normal).RotatedBy(Math.PI * 0.5);

			float k = i / (float)len;
			bars.Add(OldPositions[i] + normal * Scale, new Color(progress + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(0 + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.3f));
			bars.Add(OldPositions[i] - normal * Scale, new Color(progress + 1 - MathF.Sin(k * MathF.PI), 0, 0, 0), new Vector3(3.4f + ai[0], (i + 15 - len) / 10f + Timer / 1500f * Velocity.Length(), 0.7f));

			float inverseProgress = 1 - progress;
			float c = inverseProgress * 1.0f;
			Lighting.AddLight(OldPositions[i], new Vector3(MathF.Pow(c, 0.5f) * 0.7f, c * 0.9f, c * c * 2.6f) * Scale / 30f);
		}
		if (bars.Count < 2)
		{
			bars.Add(Vector2.zeroVector, Color.Transparent, Vector3.zero);
			bars.Add(Vector2.zeroVector, Color.Transparent, Vector3.zero);

			bars.Add(Vector2.zeroVector, Color.Transparent, Vector3.zero);
			bars.Add(Vector2.zeroVector, Color.Transparent, Vector3.zero);
		}
		Ins.Batch.Draw(bars, PrimitiveType.TriangleStrip);
	}
}
