using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class MagicalBoomerangDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 Position;
	public Vector2 Velocity;
	public float[] ai;
	public float Timer;
	public float MaxTime;
	public float Scale;
	public float MaxScale;
	public float Rotation;
	public int Frame = 0;

	public bool HasGravity = false;

	public override void Update()
	{
		Timer++;
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
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		Position += Velocity;
		if (!HasGravity)
		{
			Velocity *= 0.9f;
			Scale *= 0.9f;
		}
		else
		{
			Velocity *= 0.95f;
			Velocity.Y += 0.05f;
			Scale *= 0.98f;
		}
		Frame = (int)(Timer / MaxTime * 3f);
		float value = (MaxTime - Timer) / MaxTime;
		Lighting.AddLight(Position, Vector3.Lerp(new Vector3(1f, 1.5f, 2.2f), new Vector3(0f, 0f, 2.2f), 1 - value) * value);
	}

	public override void Draw()
	{
		float frameCount = 3;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, Scale).RotatedBy(Rotation);
		float value = (MaxTime - Timer) / MaxTime;
		GradientColor gradientColor = new GradientColor();
		if (gradientColor.colorList.Count <= 0)
		{
			gradientColor.colorList.Add((new Color(1f, 1f, 1f, 0), 0));
			gradientColor.colorList.Add((new Color(0f, 0.5f, 1f, 0), 0.4f));
			gradientColor.colorList.Add((new Color(0f, 0f, 1f, 0), 0.5f));
			gradientColor.colorList.Add((new Color(0f, 0f, 0f, 0), 1f));
		}
		var drawColor = gradientColor.GetColor(1 - value);
		var bars = new List<Vertex2D>()
		{
			new Vertex2D(Position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(Position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.MagicalBoomerangDust.Value, bars, PrimitiveType.TriangleList);
	}
}
