using Everglow.Commons.VFX.Scene;

namespace Everglow.Yggdrasil.KelpCurtain.Tiles.DeathJadeLake.WaterDeliveryHoles;

[Pipeline(typeof(WCSPipeline_PointWrap))]
public class WaterDeliveryHole_VFX_Release : TileVFX
{
	public override CodeLayer DrawLayer => CodeLayer.PostDrawTiles;

	public float Rotation;

	public float Timer;

	public float MaxTime;

	public override void Update()
	{
		base.Update();
		Timer++;
		if (Timer > MaxTime)
		{
			Active = false;
			return;
		}
		float fadeTime = 1f;
		if (Timer < 10f)
		{
			fadeTime *= Timer / 10f;
		}
		float timeLeft = MaxTime - Timer;
		if (timeLeft < 30)
		{
			fadeTime *= timeLeft / 30f;
		}
		Lighting.AddLight(Position, new Vector3(0.4f, 0.7f, 1.1f) * fadeTime * 4);
	}

	public override void Draw()
	{
		float fadeTime = 1f;
		if (Timer < 10f)
		{
			fadeTime *= Timer / 10f;
		}
		float timeLeft = MaxTime - Timer;
		if (timeLeft < 30)
		{
			fadeTime *= timeLeft / 30f;
		}
		float timeValue = (float)(Main.time * 0.01f);
		var bars_side_left = new List<Vertex2D>();
		var bars_side_right = new List<Vertex2D>();
		var bars_side_left_dark = new List<Vertex2D>();
		var bars_side_right_dark = new List<Vertex2D>();

		var bars_strongBloom_left = new List<Vertex2D>();
		var bars_strongBloom_right = new List<Vertex2D>();
		Color strongBloomColor = Color.Transparent;
		for (int k = -2; k < 30; k++)
		{
			var pos = new Vector2(k * 5, 0).RotatedBy(Rotation);
			var pos2 = new Vector2(k * 2, 0).RotatedBy(Rotation);
			float value = k / 30f;
			float fade = 1f;
			if (k > 20)
			{
				fade *= (30 - k) / 10f;
			}
			Color drawColor = Color.Lerp(new Color(0.5f, 1f, 1f, 0f), new Color(0.4f, 0.7f, 1f, 0f), value);
			drawColor = Color.Lerp(drawColor, new Color(0f, 0f, 0.6f, 0), value) * fade * fadeTime;
			var drawColor_dark = new Color(0, 0, 0, fade * 0.6f) * fade * fadeTime;
			float coordX = 1 - MathF.Pow(value, 2);

			bars_side_left.Add(Position + pos + new Vector2(0, -30 - k).RotatedBy(Rotation), drawColor * 0f, new Vector3(coordX + timeValue, 0, 0));
			bars_side_left.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_right.Add(Position + pos + new Vector2(0, 30 + k).RotatedBy(Rotation), drawColor * 0f, new Vector3(coordX + timeValue, 1, 0));
			bars_side_right.Add(Position + pos + new Vector2(0, 0), drawColor, new Vector3(coordX + timeValue, 0.5f, 0));


			if (k < Timer)
			{
				strongBloomColor = drawColor * 2 * fadeTime;
			}
			else
			{
				strongBloomColor *= 0.5f;
			}
			bars_strongBloom_left.Add(Position + pos2 + new Vector2(0, -25 - k).RotatedBy(Rotation), strongBloomColor * 0f, new Vector3(coordX + timeValue, 0, 0));
			bars_strongBloom_left.Add(Position + pos2 + new Vector2(0, 0), strongBloomColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_strongBloom_right.Add(Position + pos2 + new Vector2(0, 25 + k).RotatedBy(Rotation), strongBloomColor * 0f, new Vector3(coordX + timeValue, 1, 0));
			bars_strongBloom_right.Add(Position + pos2 + new Vector2(0, 0), strongBloomColor, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_left_dark.Add(Position + pos + new Vector2(0, -30 - k).RotatedBy(Rotation), drawColor_dark * 0f, new Vector3(coordX + timeValue, 0, 0));
			bars_side_left_dark.Add(Position + pos + new Vector2(0, 0), drawColor_dark, new Vector3(coordX + timeValue, 0.5f, 0));

			bars_side_right_dark.Add(Position + pos + new Vector2(0, 30 + k).RotatedBy(Rotation), drawColor_dark * 0, new Vector3(coordX + timeValue, 1, 0));
			bars_side_right_dark.Add(Position + pos + new Vector2(0, 0), drawColor_dark, new Vector3(coordX + timeValue, 0.5f, 0));
		}

		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3_black.Value, bars_side_left_dark, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3_black.Value, bars_side_right_dark, PrimitiveType.TriangleStrip);

		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars_side_left, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_flame_3.Value, bars_side_right, PrimitiveType.TriangleStrip);

		Ins.Batch.Draw(Commons.ModAsset.Noise_melting.Value, bars_strongBloom_left, PrimitiveType.TriangleStrip);
		Ins.Batch.Draw(Commons.ModAsset.Noise_melting.Value, bars_strongBloom_right, PrimitiveType.TriangleStrip);
	}
}
