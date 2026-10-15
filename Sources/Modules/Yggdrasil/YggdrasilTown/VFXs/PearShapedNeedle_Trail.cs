using Everglow.Commons.Graphics;
using Terraria.Map;

namespace Everglow.Yggdrasil.YggdrasilTown.VFXs;

[Pipeline(typeof(WCSPipeline))]
public class PearShapedNeedle_Trail : Visual
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawNPCs;

	public float Timer;
	public float MaxTime;
	public Vector2 Position_Start;
	public Vector2 Position_End;

	public override void Update()
	{
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
	}

	public override void Draw()
	{
		float timeValue = Timer / MaxTime;
		Vector2 direction_V = (Position_Start - Position_End).NormalizeSafe().RotatedBy(MathHelper.PiOver2);
		Vector2 width = direction_V * (1 - timeValue) * 30;
		Color darkColor = Color.White * 0.8f;
		var bars_dark = new List<Vertex2D>();
		var bars = new List<Vertex2D>();
		for (int i = 0; i <= 10; i++)
		{
			float valueLerp = i / 10f;
			Vector2 currentPos = Vector2.Lerp(Position_Start, Position_End, valueLerp);
			Color drawColor = Lighting.GetColor(currentPos.ToTileCoordinates()) * 2.5f;
			drawColor.A = 0;
			bars_dark.Add(currentPos + width, darkColor, new Vector3(0, valueLerp * 0.8f, 0));
			bars_dark.Add(currentPos - width, darkColor, new Vector3(1, valueLerp * 0.8f, 0));

			bars.Add(currentPos + width, drawColor, new Vector3(0, valueLerp * 0.8f, 0));
			bars.Add(currentPos - width, drawColor, new Vector3(1, valueLerp * 0.8f, 0));
		}

		Ins.Batch.Draw(Commons.ModAsset.StarSlash_black.Value, bars_dark, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.StarSlash.Value, bars, PrimitiveType.TriangleStrip);
	}
}
