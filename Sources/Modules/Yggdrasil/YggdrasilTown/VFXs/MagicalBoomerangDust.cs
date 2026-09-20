using Everglow.Commons.Graphics;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class MagicalBoomerangDust : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public Vector2 position;
	public Vector2 velocity;
	public float[] ai;
	public float timer;
	public float maxTime;
	public float scale;
	public float maxScale;
	public float rotation;
	public int Frame = 0;

	public bool gravity = false;

	public override void Update()
	{
		timer++;
		if (position.X <= 320 || position.X >= Main.maxTilesX * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		if (position.Y <= 320 || position.Y >= Main.maxTilesY * 16 - 320)
		{
			timer = maxTime;
			Active = false;
			return;
		}
		if (timer > maxTime)
		{
			Active = false;
			return;
		}
		position += velocity;
		if (!gravity)
		{
			velocity *= 0.9f;
			scale *= 0.9f;
		}
		else
		{
			velocity *= 0.95f;
			velocity.Y += 0.05f;
			scale *= 0.98f;
		}
		Frame = (int)(timer / maxTime * 3f);
		float value = (maxTime - timer) / maxTime;
		Lighting.AddLight(position, Vector3.Lerp(new Vector3(1f, 1.5f, 2.2f), new Vector3(0f, 0f, 2.2f), 1 - value) * value);
	}

	public override void Draw()
	{
		float frameCount = 3;
		float frameY = Frame;
		Vector2 toCorner = new Vector2(0, scale).RotatedBy(rotation);
		float value = (maxTime - timer) / maxTime;
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
			new Vertex2D(position + toCorner, drawColor, new Vector3(0, frameY / frameCount, 0)),
			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),

			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 1.5), drawColor, new Vector3(0, (frameY + 1) / frameCount, 0)),
			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 0.5), drawColor, new Vector3(1, frameY / frameCount, 0)),
			new Vertex2D(position + toCorner.RotatedBy(Math.PI * 1), drawColor, new Vector3(1, (frameY + 1) / frameCount, 0)),
		};
		Ins.Batch.Draw(ModAsset.MagicalBoomerangDust.Value, bars, PrimitiveType.TriangleList);
	}
}
