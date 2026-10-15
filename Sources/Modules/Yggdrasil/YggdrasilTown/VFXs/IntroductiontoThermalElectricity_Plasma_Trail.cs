using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class IntroductiontoThermalElectricity_Plasma_Trail : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public float Timer;
	public float MaxTime;
	public float Rotation;
	public float Scale;
	public Vector2 Position;
	public Vector2 Velocity;
	public GradientColor PlasmaColor = new();
	public Queue<Vector2> Trails = new Queue<Vector2>();

	public void AddGradientColor()
	{
		PlasmaColor.colorList.Add((new Color(255, 255, 255, 255), 0));
		PlasmaColor.colorList.Add((new Color(255, 255, 255, 255), 0.2f));
		PlasmaColor.colorList.Add((new Color(56, 155, 255, 255), 0.3f));
		PlasmaColor.colorList.Add((new Color(255, 105, 91, 255), 0.8f));
		PlasmaColor.colorList.Add((new Color(0, 0, 0, 0), 1f));
	}

	public override void Update()
	{
		if (PlasmaColor.colorList.Count <= 0)
		{
			AddGradientColor();
		}
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Trails.Enqueue(Position);
		if (Trails.Count > 20)
		{
			Trails.Dequeue();
		}
		Position += Velocity;
		Velocity *= 0.96f;
		Scale *= 0.99f;
		float timeLeftValue = Timer / MaxTime;
		Color lightColor = PlasmaColor.GetColor(timeLeftValue);
		Lighting.AddLight(Position, lightColor.ToVector3() * Scale * 0.1f);
	}

	public override void Draw()
	{
		if (PlasmaColor.colorList.Count <= 0)
		{
			AddGradientColor();
		}
		Vector2 drawPos;
		float timeLeftValue = Timer / MaxTime;
		Color drawColor;
		var bars = new List<Vertex2D>();

		for (int k = 0; k < Trails.Count; k++)
		{
			drawPos = Trails.ToArray()[k];
			drawColor = PlasmaColor.GetColor(timeLeftValue - k / 20f + 1);
			AddVertexs(bars, drawPos, drawColor * 0.5f);
		}
		drawColor = PlasmaColor.GetColor(timeLeftValue);
		AddVertexs(bars, Position, drawColor);

		Ins.Batch.Draw(ModAsset.IntroductiontoThermalElectricity_Plasma.Value, bars, PrimitiveType.TriangleList);
	}

	public void AddVertexs(List<Vertex2D> bars, Vector2 position, Color color)
	{
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		bars.Add(position + toCorner, color, new Vector3(0, 0, 0));
		bars.Add(position + toCorner.RotatedBy(MathHelper.PiOver2), color, new Vector3(1, 0, 0));
		bars.Add(position + toCorner.RotatedBy(MathHelper.Pi), color, new Vector3(0, 1, 0));

		bars.Add(position + toCorner, color, new Vector3(0, 0, 0));
		bars.Add(position + toCorner.RotatedBy(MathHelper.Pi), color, new Vector3(0, 1, 0));
		bars.Add(position + toCorner.RotatedBy(-MathHelper.PiOver2), color, new Vector3(1, 1, 0));
	}
}
