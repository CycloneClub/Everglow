using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake;

[Pipeline(typeof(WarpPipeline))]
public class WaterDeliveryHole_VFX_warp : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public float Rotation;

	public override void Update()
	{
		base.Update();
	}

	public override void Draw()
	{
		float timeValue = (float)(Main.time * 0.001f);
		var bars_side_left = new List<Vertex2D>();
		var bars_side_right = new List<Vertex2D>();
		for (int k = -2; k < 30; k++)
		{
			var pos = new Vector2(k * 3, 0).RotatedBy(Rotation);
			float value = k / 300f;
			float fade = 0.15f;
			if (k > 20)
			{
				fade *= (30 - k) / 10f;
			}
			Vector2 dir = new Vector2(1, 0).RotatedBy(Rotation + MathHelper.PiOver2) + Vector2.One;
			dir *= 0.5f;
			Color drawColor = new Color(dir.X, dir.Y, fade, 1f);
			Color drawColor_side = new Color(dir.X, dir.Y, 0, 1f);
			float coordX = MathF.Pow(value, 2);

			bars_side_left.Add(Position + pos + new Vector2(0, -20 - k).RotatedBy(Rotation), drawColor_side, new Vector3(coordX + timeValue, 0, 0));
			bars_side_left.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_right.Add(Position + pos + new Vector2(0, 20 + k).RotatedBy(Rotation), drawColor_side, new Vector3(coordX + timeValue, 1, 0));
			bars_side_right.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));
		}

		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2.Value, bars_side_left, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_2.Value, bars_side_right, PrimitiveType.TriangleStrip);
	}
}
