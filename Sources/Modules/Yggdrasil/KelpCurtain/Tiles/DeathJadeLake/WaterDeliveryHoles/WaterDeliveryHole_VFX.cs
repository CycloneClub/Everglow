using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class WaterDeliveryHole_VFX : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public float Rotation;

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		float timeValue = (float)(Main.time * 0.01f);
		var bars_side_left = new List<Vertex2D>();
		var bars_side_right = new List<Vertex2D>();
		var bars_side_left_dark = new List<Vertex2D>();
		var bars_side_right_dark = new List<Vertex2D>();
		for (int k = -2; k < 30; k++)
		{
			var pos = new Vector2(k * 3, 0).RotatedBy(Rotation);
			float value = k / 30f;
			float fade = 1f;
			if (k > 20)
			{
				fade *= (30 - k) / 10f;
			}
			Color drawColor = Color.Lerp(new Color(0.4f, 0.7f, 1f, 0f), new Color(0f, 0.2f, 1f, 0f), value) * fade * 0.5f;
			var drawColor_dark = new Color(0, 0, 0, fade * 0.6f) * 0.5f;
			float coordX = MathF.Pow(value, 2);

			bars_side_left.Add(Position + pos + new Vector2(0, -20 - k).RotatedBy(Rotation), drawColor * 0f, new Vector3(coordX + timeValue, 0, 0));
			bars_side_left.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_right.Add(Position + pos + new Vector2(0, 20 + k).RotatedBy(Rotation), drawColor * 0f, new Vector3(coordX + timeValue, 1, 0));
			bars_side_right.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_left_dark.Add(Position + pos + new Vector2(0, -20 - k).RotatedBy(Rotation), drawColor_dark * 0f, new Vector3(coordX + timeValue, 0, 0));
			bars_side_left_dark.Add(Position + pos + new Vector2(0, 0), drawColor_dark, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_right_dark.Add(Position + pos + new Vector2(0, 20 + k).RotatedBy(Rotation), drawColor_dark * 0, new Vector3(coordX + timeValue, 1, 0));
			bars_side_right_dark.Add(Position + pos + new Vector2(0, 0), drawColor_dark, new Vector3(coordX + timeValue, 0.5f, 0));
		}

		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3_black.Value, bars_side_left_dark, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3_black.Value, bars_side_right_dark, PrimitiveType.TriangleStrip);

		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars_side_left, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars_side_right, PrimitiveType.TriangleStrip);
	}
}
