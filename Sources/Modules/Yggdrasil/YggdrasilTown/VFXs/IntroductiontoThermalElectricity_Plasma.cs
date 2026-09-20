using Everglow.Commons.Graphics;
using Terraria.Map;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline), typeof(BloomPipeline))]
public class IntroductiontoThermalElectricity_Plasma : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public float Timer;
	public float MaxTime;
	public float Rotation;
	public float Scale;
	public Vector2 Position;
	public Vector2 Velocity;
	public GradientColor PlasmaColor = new();

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
		Vector2 drawPos = Position;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		float timeLeftValue = Timer / MaxTime;
		var drawColor = PlasmaColor.GetColor(timeLeftValue);

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
